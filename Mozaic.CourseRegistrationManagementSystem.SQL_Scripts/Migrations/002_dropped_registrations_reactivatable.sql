-- ============================================================================
-- Migration: dropped registrations are reactivatable (re-register in place)
--
-- Canonical drop status is 'Dropped'. Uniqueness is scoped to non-dropped
-- rows so a dropped registration never blocks re-registering; the register
-- flow flips the existing row back instead of inserting a duplicate.
-- Run once against CourseManagementDB. Idempotent (safe to re-run).
-- backup.sql is the source of truth for fresh builds; this catches up
-- existing databases. backup.sql itself is also updated by this change.
-- ============================================================================

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- 0. Normalize legacy soft-delete rows written as 'Cancelled'.
-- ----------------------------------------------------------------------------
UPDATE [dbo].[Registrations]
SET [Status]     = N'Dropped',
    [ModifiedOn] = SYSDATETIMEOFFSET()
WHERE [Status] = N'Cancelled';
GO

-- ----------------------------------------------------------------------------
-- 1. Drop the blanket unique constraint (it sees dropped rows).
-- ----------------------------------------------------------------------------
IF EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE [name] = N'UQ_Registrations_Student_Class'
      AND parent_object_id = OBJECT_ID(N'[dbo].[Registrations]')
)
    ALTER TABLE [dbo].[Registrations] DROP CONSTRAINT [UQ_Registrations_Student_Class];
GO

-- ----------------------------------------------------------------------------
-- 2. Filtered unique index: duplicates blocked only for non-dropped rows.
-- ----------------------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'UQ_Registrations_Student_Class_Active'
      AND object_id = OBJECT_ID(N'[dbo].[Registrations]')
)
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_Registrations_Student_Class_Active]
        ON [dbo].[Registrations] ([StudentId] ASC, [ClassId] ASC)
        WHERE ([Status] <> N'Dropped');
GO

-- ----------------------------------------------------------------------------
-- 3. usp_DeleteRegistration: canonical 'Dropped' status.
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteRegistration];
GO

CREATE PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id INT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    -- Registrations has no IsActive column; soft delete = mark as Dropped.
    -- The row is kept for history and reactivated in place on re-register.
    UPDATE [dbo].[Registrations]
    SET [Status]     = N'Dropped',
        [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- 4. usp_CreateRegistration: duplicate check scoped to non-dropped rows.
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS [dbo].[usp_CreateRegistration];
GO

CREATE PROCEDURE [dbo].[usp_CreateRegistration]
    @StudentId        INT,
    @ClassId          INT,
    @RegistrationDate DATETIME2,
    @Status           NVARCHAR(50),
    @CreatedBy        INT = 0,
    @NewId            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Student] WHERE [Id] = @StudentId)
        THROW 50003, 'StudentId does not exist.', 1;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Class] WHERE [Id] = @ClassId)
        THROW 50004, 'ClassId does not exist.', 1;

    IF EXISTS (SELECT 1 FROM [dbo].[Registrations]
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId AND [Status] <> N'Dropped')
        THROW 50005, 'Student is already registered for this class.', 1;

    INSERT INTO [dbo].[Registrations]
    ([StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (@StudentId,@ClassId,@RegistrationDate,@Status,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- 5. usp_UpdateRegistration: same scoping (reactivation path included).
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateRegistration];
GO

CREATE PROCEDURE [dbo].[usp_UpdateRegistration]
    @Id               INT,
    @StudentId        INT,
    @ClassId          INT,
    @RegistrationDate DATETIME2,
    @Status           NVARCHAR(50),
    @ModifiedBy       INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Registrations]
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId AND [Id] <> @Id AND [Status] <> N'Dropped')
        THROW 50005, 'Another registration already exists for this student/class.', 1;

    UPDATE [dbo].[Registrations]
    SET [StudentId]        = @StudentId,
        [ClassId]          = @ClassId,
        [RegistrationDate] = @RegistrationDate,
        [Status]           = @Status,
        [ModifiedBy]       = @ModifiedBy,
        [ModifiedOn]       = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- 6. usp_GetAllClasses: dropped rows no longer flag IsRegistered nor count
--    toward MyRegistrationsCount.
--
--    SUPERSEDED: usp_GetAllClasses has since been replaced by usp_GetClasses,
--    which carries the same Dropped handling (plus paging and status/search
--    filters) and lives in backup.sql. This block is kept as applied history;
--    do not re-run it on a database created from the current backup.sql, or
--    it will recreate the retired procedure.
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllClasses];
GO

CREATE PROCEDURE [dbo].[usp_GetAllClasses]
    @UserId     INT = NULL,         -- Map your input student/user ID here
    @PageNumber INT = 1,
    @PageSize   INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   > 100 SET @PageSize = 100;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -- Pages only the 'Id' column to keep memory grants minimal and
    -- avoid scanning wide columns during sorting.
    -------------------------------------------------------------------
    WITH PagedKeys AS (
        SELECT
            c.Id
        FROM dbo.[Class] AS c
        WHERE c.IsActive = 1
        ORDER BY
            c.StartDate ASC,   -- Primary sort
            c.Id ASC           -- Deterministic tiebreaker
        OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
    )
    -------------------------------------------------------------------
    -- STEP 2: Join back to base tables for ONLY the top 20 primary keys
    -------------------------------------------------------------------
    SELECT
        c.Id,
        c.CourseId,
        co.CourseName,
        c.ClassName,
        c.Instructor,
        c.Schedule          AS ScheduleDisplay,
        c.StartDate,
        c.EndDate,
        c.CurrentCapacity,
        c.MaxCapacity,
        CAST(CASE WHEN sr.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsRegistered
    FROM PagedKeys AS pk
             INNER JOIN dbo.[Class] AS c
                        ON c.Id = pk.Id
             LEFT JOIN dbo.[Course] AS co
                       ON co.Id = c.CourseId
             LEFT JOIN dbo.[Registrations] AS sr
                       ON sr.ClassId = c.Id
                           AND @UserId IS NOT NULL
                           AND sr.StudentId = @UserId  -- Matches the foreign key in your table DDL
                           AND sr.Status <> N'Dropped'  -- Dropped rows don't count as registered
    ORDER BY
        c.StartDate ASC,
        c.Id ASC;

    -------------------------------------------------------------------
    -- STEP 3: Single-pass Aggregates for Pagination Counters
    -------------------------------------------------------------------
    SELECT
        COUNT(*) AS TotalCount,
        COUNT(*) AS AvailableCount,
        ISNULL(SUM(CASE WHEN MaxCapacity - CurrentCapacity > 0
                            THEN MaxCapacity - CurrentCapacity
                        ELSE 0 END), 0) AS OpenSeatsCount,
        (
            SELECT COUNT(*)
            FROM dbo.[Registrations]
            WHERE @UserId IS NOT NULL
              AND StudentId = @UserId  -- Matches foreign key in Registrations
              AND Status <> N'Dropped'
        ) AS MyRegistrationsCount
    FROM dbo.[Class]
    WHERE IsActive = 1;
END;
GO

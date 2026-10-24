-- ============================================================================
-- Migration: class capacity guards
--
-- 1. CurrentCapacity is owned by the registration flow. A new class always
--    starts at 0, whatever the caller sends.
-- 2. MaxCapacity can never be lowered below the seats already taken, even if
--    a caller bypasses the BL check in ClassService.UpdateAsync.
--
-- Run once against CourseManagementDB. Idempotent (safe to re-run).
-- backup.sql is the source of truth for fresh builds; this catches up
-- existing databases.
-- ============================================================================

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- 1. Create: ignore @CurrentCapacity and always insert 0. The parameter is
--    kept so existing DAL calls keep working.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[usp_CreateClass]
    @CourseId        INT,
    @ClassName       NVARCHAR(50),
    @Instructor      NVARCHAR(50),
    @MaxCapacity     INT,
    @CurrentCapacity INT,       -- accepted for compatibility, always stored as 0
    @StartDate       DATETIME2,
    @EndDate         DATETIME2,
    @Schedule        INT,
    @IsActive        BIT,
    @CreatedBy       INT = 0,
    @NewId           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [Id] = @CourseId)
        THROW 50001, 'CourseId does not exist.', 1;

    -- A class always starts empty; enrollment is owned by the registration
    -- flow, so @CurrentCapacity is intentionally ignored here.
    INSERT INTO [dbo].[Class]
    ([CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
     [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@CourseId,@ClassName,@Instructor,@MaxCapacity,0,
         @StartDate,@EndDate,@Schedule,@IsActive,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- 2. Update: reject a max capacity below the stored enrollment. The same
--    procedure also persists enrollment changes from the registration flow;
--    those keep MaxCapacity unchanged, so the guard never blocks them.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateClass]
    @Id              INT,
    @CourseId        INT,
    @ClassName       NVARCHAR(50),
    @Instructor      NVARCHAR(50),
    @MaxCapacity     INT,
    @CurrentCapacity INT,
    @StartDate       DATETIME2,
    @EndDate         DATETIME2,
    @Schedule        INT,
    @IsActive        BIT,
    @ModifiedBy      INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [Id] = @CourseId)
        THROW 50001, 'CourseId does not exist.', 1;

    -- Never shrink a class below the seats already taken. The app checks the
    -- same rule, but this also protects direct/forged updates and races.
    IF @MaxCapacity < ISNULL((SELECT [CurrentCapacity] FROM [dbo].[Class] WHERE [Id] = @Id), 0)
        THROW 50006, 'MaxCapacity cannot be less than the current enrollment.', 1;

    UPDATE [dbo].[Class]
    SET [CourseId]        = @CourseId,
        [ClassName]       = @ClassName,
        [Instructor]      = @Instructor,
        [MaxCapacity]     = @MaxCapacity,
        [CurrentCapacity] = @CurrentCapacity,
        [StartDate]       = @StartDate,
        [EndDate]         = @EndDate,
        [Schedule]        = @Schedule,
        [IsActive]        = @IsActive,
        [ModifiedBy]      = @ModifiedBy,
        [ModifiedOn]      = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

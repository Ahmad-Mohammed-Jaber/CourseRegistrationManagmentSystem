-- ============================================================================
-- StoredProceduresV3.sql — Stored procedures matching TablesV2.sql
--
-- Run order: 1) TablesV2.sql  2) StoredProceduresV3.sql
-- Safe to re-run (drops all procedures first).
--
-- Conventions kept compatible with DAL/Providers/*.cs:
--   * Same procedure + parameter names (@Id OUTPUT for User/Student creates,
--     @NewId OUTPUT for Course/Class/Registration creates).
--   * Same result column names AND order; audit columns
--     (CreatedOn, ModifiedOn, CreatedBy, ModifiedBy) are APPENDED LAST so
--     ordinal-based mappers (detailed / with-classes projections) keep working.
--   * usp_UpdateStudent keeps @FullName (default NULL, ignored by default):
--     Student has no FullName column in V2 but the DAL still passes the
--     parameter. See section 5 for the optional [User].FullName sync.
--
-- Fixes applied vs StoredProceduresV2.sql:
--   * usp_SearchRegistrationsDetailed: removed s.FullName reference (column
--     was dropped in V2); now filters on u.FullName.
--   * usp_SearchRegistrationsDetailed: aliased co.Id AS CourseId to avoid
--     duplicate column name with r.Id.
--   * usp_RegistrationExists: aliased the result column as [Exists].
--   * usp_Delete*: now return @@ROWCOUNT AS RowsAffected.
--   * usp_Create*: now THROW on duplicate unique keys (UserName, CourseCode,
--     StudentNumber, UserId, StudentId+ClassId).
--   * Added usp_GetCourseByCode, usp_GetClassByName, usp_GetRegistrationByStudentAndClass.
--   * usp_UpdateStudent: optional @FullName now propagates to [User].FullName
--     when non-NULL (kept backward-compatible: NULL = no change).
--   * usp_GetStudentProfileByUserId: now returns both StudentId and UserId.
-- ============================================================================
USE [CourseManagementDB];
GO

-- =============================================
-- DROP ALL STORED PROCEDURES (safe re-run)
-- =============================================
IF OBJECT_ID('dbo.usp_GetStudentProfileByUserId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetStudentProfileByUserId];
IF OBJECT_ID('dbo.usp_UpdateUser', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_UpdateUser];
IF OBJECT_ID('dbo.usp_UpdateStudent', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_UpdateStudent];
IF OBJECT_ID('dbo.usp_UpdateRegistration', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_UpdateRegistration];
IF OBJECT_ID('dbo.usp_UpdateCourse', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_UpdateCourse];
IF OBJECT_ID('dbo.usp_UpdateClass', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_UpdateClass];
IF OBJECT_ID('dbo.usp_SearchUsers', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchUsers];
IF OBJECT_ID('dbo.usp_SearchStudents', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchStudents];
IF OBJECT_ID('dbo.usp_SearchRegistrationsDetailed', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchRegistrationsDetailed];
IF OBJECT_ID('dbo.usp_SearchRegistrations', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchRegistrations];
IF OBJECT_ID('dbo.usp_SearchCourses', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchCourses];
IF OBJECT_ID('dbo.usp_SearchClasses', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_SearchClasses];
IF OBJECT_ID('dbo.usp_RegistrationExists', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_RegistrationExists];
IF OBJECT_ID('dbo.usp_GetUserByUserName', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetUserByUserName];
IF OBJECT_ID('dbo.usp_GetUserById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetUserById];
IF OBJECT_ID('dbo.usp_GetStudentRegistrationsWithClasses', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetStudentRegistrationsWithClasses];
IF OBJECT_ID('dbo.usp_GetStudentByUserId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetStudentByUserId];
IF OBJECT_ID('dbo.usp_GetStudentById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetStudentById];
IF OBJECT_ID('dbo.usp_GetRegistrationsByStudentId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetRegistrationsByStudentId];
IF OBJECT_ID('dbo.usp_GetRegistrationsByClassId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetRegistrationsByClassId];
IF OBJECT_ID('dbo.usp_GetRegistrationById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetRegistrationById];
IF OBJECT_ID('dbo.usp_GetRegistrationByStudentAndClass', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetRegistrationByStudentAndClass];
IF OBJECT_ID('dbo.usp_GetCourseById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetCourseById];
IF OBJECT_ID('dbo.usp_GetCourseByCode', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetCourseByCode];
IF OBJECT_ID('dbo.usp_GetClassesByCourseId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetClassesByCourseId];
IF OBJECT_ID('dbo.usp_GetClassById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetClassById];
IF OBJECT_ID('dbo.usp_GetClassByName', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetClassByName];
IF OBJECT_ID('dbo.usp_GetAllUsers', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllUsers];
IF OBJECT_ID('dbo.usp_GetAllStudents', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllStudents];
IF OBJECT_ID('dbo.usp_GetAllRegistrationsDetailed', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllRegistrationsDetailed];
IF OBJECT_ID('dbo.usp_GetAllRegistrations', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllRegistrations];
IF OBJECT_ID('dbo.usp_GetAllCourses', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllCourses];
IF OBJECT_ID('dbo.usp_GetAllClasses', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetAllClasses];
IF OBJECT_ID('dbo.usp_DeleteUser', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_DeleteUser];
IF OBJECT_ID('dbo.usp_DeleteStudent', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_DeleteStudent];
IF OBJECT_ID('dbo.usp_DeleteRegistration', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_DeleteRegistration];
IF OBJECT_ID('dbo.usp_DeleteCourse', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_DeleteCourse];
IF OBJECT_ID('dbo.usp_DeleteClass', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_DeleteClass];
IF OBJECT_ID('dbo.usp_CreateUser', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_CreateUser];
IF OBJECT_ID('dbo.usp_CreateStudent', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_CreateStudent];
IF OBJECT_ID('dbo.usp_CreateRegistration', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_CreateRegistration];
IF OBJECT_ID('dbo.usp_CreateCourse', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_CreateCourse];
IF OBJECT_ID('dbo.usp_CreateClass', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_CreateClass];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- CLASS Stored Procedures
-- =============================================

-- ----------------------------------------------------------------------------
-- usp_CreateClass — returns generated Id via @NewId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_CreateClass]
    @CourseId        INT,
    @ClassName       NVARCHAR(50),
    @Instructor      NVARCHAR(50),
    @MaxCapacity     INT,
    @CurrentCapacity INT,
    @StartDate       DATETIME2,
    @EndDate         DATETIME2,
    @Schedule        INT,
    @IsActive        BIT,
    @CreatedBy       INT = 0,
    @NewId           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- FK guard: Course must exist
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [Id] = @CourseId)
        THROW 50001, 'CourseId does not exist.', 1;

    INSERT INTO [dbo].[Class]
        ([CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
         [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@CourseId,@ClassName,@Instructor,@MaxCapacity,@CurrentCapacity,
         @StartDate,@EndDate,@Schedule,@IsActive,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- usp_DeleteClass
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteClass]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Class] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetAllClasses
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllClasses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
        [StartDate],[EndDate],[Schedule],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Class];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetClassById
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetClassById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
        [StartDate],[EndDate],[Schedule],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Class]
    WHERE [Id] = @Id;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetClassByName — useful for unique-name checks
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetClassByName]
    @ClassName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
        [StartDate],[EndDate],[Schedule],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Class]
    WHERE [ClassName] = @ClassName;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetClassesByCourseId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetClassesByCourseId]
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
        [StartDate],[EndDate],[Schedule],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Class]
    WHERE [CourseId] = @CourseId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchClasses
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchClasses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
        [StartDate],[EndDate],[Schedule],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Class]
    WHERE [ClassName] LIKE '%' + @regex + '%'
       OR [Instructor] LIKE '%' + @regex + '%';
END
GO

-- ----------------------------------------------------------------------------
-- usp_UpdateClass
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_UpdateClass]
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

-- =============================================
-- COURSE Stored Procedures
-- =============================================

-- ----------------------------------------------------------------------------
-- usp_CreateCourse — returns generated Id via @NewId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_CreateCourse]
    @CourseCode  NVARCHAR(20),
    @CourseName  NVARCHAR(100),
    @CreditHours DECIMAL(4, 2),
    @Description NVARCHAR(MAX),
    @IsActive    BIT,
    @CreatedBy   INT = 0,
    @NewId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [CourseCode] = @CourseCode)
        THROW 50002, 'CourseCode already exists.', 1;

    INSERT INTO [dbo].[Course]
        ([CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@CourseCode,@CourseName,@CreditHours,@Description,@IsActive,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- usp_DeleteCourse
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteCourse]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Course] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetAllCourses
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllCourses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Course];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetCourseById
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetCourseById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Course]
    WHERE [Id] = @Id;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetCourseByCode
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetCourseByCode]
    @CourseCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Course]
    WHERE [CourseCode] = @CourseCode;
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchCourses
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchCourses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Course]
    WHERE [CourseCode] LIKE '%' + @regex + '%'
       OR [CourseName] LIKE '%' + @regex + '%';
END
GO

-- ----------------------------------------------------------------------------
-- usp_UpdateCourse
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_UpdateCourse]
    @Id          INT,
    @CourseCode  NVARCHAR(20),
    @CourseName  NVARCHAR(100),
    @CreditHours DECIMAL(4, 2),
    @Description NVARCHAR(MAX),
    @IsActive    BIT,
    @ModifiedBy  INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Course]
               WHERE [CourseCode] = @CourseCode AND [Id] <> @Id)
        THROW 50002, 'CourseCode already exists on another course.', 1;

    UPDATE [dbo].[Course]
    SET [CourseCode]  = @CourseCode,
        [CourseName]  = @CourseName,
        [CreditHours] = @CreditHours,
        [Description] = @Description,
        [IsActive]    = @IsActive,
        [ModifiedBy]  = @ModifiedBy,
        [ModifiedOn]  = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- =============================================
-- REGISTRATION Stored Procedures
-- =============================================

-- ----------------------------------------------------------------------------
-- usp_CreateRegistration — returns generated Id via @NewId
-- ----------------------------------------------------------------------------
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
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId)
        THROW 50005, 'Student is already registered for this class.', 1;

    INSERT INTO [dbo].[Registrations]
        ([StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (@StudentId,@ClassId,@RegistrationDate,@Status,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- usp_DeleteRegistration
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Registrations] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetAllRegistrations
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllRegistrations]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetRegistrationById
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetRegistrationById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations]
    WHERE [Id] = @Id;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetRegistrationByStudentAndClass
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetRegistrationByStudentAndClass]
    @StudentId INT,
    @ClassId   INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations]
    WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetRegistrationsByClassId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetRegistrationsByClassId]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations]
    WHERE [ClassId] = @ClassId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetRegistrationsByStudentId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetRegistrationsByStudentId]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations]
    WHERE [StudentId] = @StudentId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_RegistrationExists — result column aliased for safe mapping
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_RegistrationExists]
    @StudentId INT,
    @ClassId   INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS [Exists]
    FROM [dbo].[Registrations]
    WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchRegistrations
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchRegistrations]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[StudentId],[ClassId],[RegistrationDate],[Status],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[Registrations]
    WHERE [Status] LIKE '%' + @regex + '%';
END
GO

-- ----------------------------------------------------------------------------
-- usp_UpdateRegistration
-- ----------------------------------------------------------------------------
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
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId AND [Id] <> @Id)
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
-- usp_GetAllRegistrationsDetailed
-- Column order preserved from V2; audit columns appended last.
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllRegistrationsDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        r.[Id],
        r.[StudentId],
        u.[UserName],
        r.[ClassId],
        c.[ClassName],
        co.[Id]        AS [CourseId],
        co.[CourseName],
        r.[RegistrationDate],
        r.[Status],
        r.[CreatedOn],
        r.[ModifiedOn],
        r.[CreatedBy],
        r.[ModifiedBy]
    FROM [dbo].[Registrations] r
    INNER JOIN [dbo].[Student] s  ON r.[StudentId] = s.[Id]
    INNER JOIN [dbo].[User]    u  ON s.[UserId]    = u.[Id]
    INNER JOIN [dbo].[Class]   c  ON r.[ClassId]   = c.[Id]
    INNER JOIN [dbo].[Course]  co ON c.[CourseId]  = co.[Id];
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchRegistrationsDetailed
-- FIX: removed s.FullName (dropped in V2); now filters on u.FullName.
-- FIX: co.Id aliased as CourseId to avoid collision with r.Id.
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchRegistrationsDetailed]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        r.[Id],
        r.[StudentId],
        u.[UserName],
        r.[ClassId],
        c.[ClassName],
        co.[Id]        AS [CourseId],
        co.[CourseName],
        r.[RegistrationDate],
        r.[Status],
        r.[CreatedOn],
        r.[ModifiedOn],
        r.[CreatedBy],
        r.[ModifiedBy]
    FROM [dbo].[Registrations] r
    INNER JOIN [dbo].[Student] s  ON r.[StudentId] = s.[Id]
    INNER JOIN [dbo].[User]    u  ON s.[UserId]    = u.[Id]
    INNER JOIN [dbo].[Class]   c  ON r.[ClassId]   = c.[Id]
    INNER JOIN [dbo].[Course]  co ON c.[CourseId]  = co.[Id]
    WHERE u.[UserName]     LIKE '%' + @regex + '%'
       OR u.[FullName]     LIKE '%' + @regex + '%'   -- from [User], not Student
       OR c.[ClassName]    LIKE '%' + @regex + '%'
       OR co.[CourseName]  LIKE '%' + @regex + '%'
       OR r.[Status]       LIKE '%' + @regex + '%';
END
GO

-- =============================================
-- STUDENT Stored Procedures
-- =============================================

-- ----------------------------------------------------------------------------
-- usp_CreateStudent — returns generated Id via @Id OUTPUT
-- @FullName accepted for DAL compatibility; ignored here (no Student.FullName
-- in V2). If you want it to sync to [User].FullName, uncomment the UPDATE below.
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_CreateStudent]
    @UserId        INT,
    @StudentNumber INT,
    @FullName      NVARCHAR(100) = NULL,
    @Email         NVARCHAR(100),
    @Phone         NVARCHAR(20),
    @CreatedBy     INT = 0,
    @Id            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = @UserId)
        THROW 50006, 'UserId does not exist.', 1;

    IF EXISTS (SELECT 1 FROM [dbo].[Student] WHERE [UserId] = @UserId)
        THROW 50007, 'A Student already exists for this UserId.', 1;

    IF EXISTS (SELECT 1 FROM [dbo].[Student] WHERE [StudentNumber] = @StudentNumber)
        THROW 50008, 'StudentNumber already exists.', 1;

    INSERT INTO [dbo].[Student]
        ([UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    VALUES
        (@UserId,@StudentNumber,@Email,@Phone,@CreatedBy,@CreatedBy);

    SET @Id = SCOPE_IDENTITY();

    -- Optional: propagate @FullName to [User].FullName
    -- IF @FullName IS NOT NULL
    --     UPDATE [dbo].[User]
    --     SET [FullName] = @FullName, [ModifiedOn] = SYSDATETIMEOFFSET()
    --     WHERE [Id] = @UserId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_DeleteStudent
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteStudent]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Student] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetAllStudents — flattens Student + [User] identity columns
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.[Id],
        s.[UserId],
        s.[StudentNumber],
        s.[Email],
        s.[Phone],
        u.[UserName]  AS [UserName],
        u.[FullName]  AS [FullName],
        u.[Role]      AS [Role],
        u.[IsActive]  AS [IsActive],
        s.[CreatedOn],
        s.[ModifiedOn],
        s.[CreatedBy],
        s.[ModifiedBy]
    FROM [dbo].[Student] s
    INNER JOIN [dbo].[User] u ON s.[UserId] = u.[Id];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetStudentById
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetStudentById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.[Id],
        s.[UserId],
        s.[StudentNumber],
        s.[Email],
        s.[Phone],
        u.[UserName]  AS [UserName],
        u.[FullName]  AS [FullName],
        u.[Role]      AS [Role],
        u.[IsActive]  AS [IsActive],
        s.[CreatedOn],
        s.[ModifiedOn],
        s.[CreatedBy],
        s.[ModifiedBy]
    FROM [dbo].[Student] s
    INNER JOIN [dbo].[User] u ON s.[UserId] = u.[Id]
    WHERE s.[Id] = @Id;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetStudentByUserId
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetStudentByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.[Id],
        s.[UserId],
        s.[StudentNumber],
        s.[Email],
        s.[Phone],
        u.[UserName]  AS [UserName],
        u.[FullName]  AS [FullName],
        u.[Role]      AS [Role],
        u.[IsActive]  AS [IsActive],
        s.[CreatedOn],
        s.[ModifiedOn],
        s.[CreatedBy],
        s.[ModifiedBy]
    FROM [dbo].[Student] s
    INNER JOIN [dbo].[User] u ON s.[UserId] = u.[Id]
    WHERE s.[UserId] = @UserId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchStudents — FIX: filters on u.FullName (Student.FullName dropped)
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchStudents]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.[Id],
        s.[UserId],
        s.[StudentNumber],
        s.[Email],
        s.[Phone],
        u.[UserName]  AS [UserName],
        u.[FullName]  AS [FullName],
        u.[Role]      AS [Role],
        u.[IsActive]  AS [IsActive],
        s.[CreatedOn],
        s.[ModifiedOn],
        s.[CreatedBy],
        s.[ModifiedBy]
    FROM [dbo].[Student] s
    INNER JOIN [dbo].[User] u ON s.[UserId] = u.[Id]
    WHERE u.[FullName] LIKE '%' + @regex + '%'
       OR s.[Email]    LIKE '%' + @regex + '%';
END
GO

-- ----------------------------------------------------------------------------
-- usp_UpdateStudent
-- @FullName accepted for DAL compatibility. By default it is ignored
-- (Student has no FullName column in V2); when non-NULL it propagates to
-- [User].FullName (backward-compatible: NULL = no change).
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_UpdateStudent]
    @Id            INT,
    @UserId        INT,
    @StudentNumber INT,
    @FullName      NVARCHAR(100) = NULL,
    @Email         NVARCHAR(100),
    @Phone         NVARCHAR(20),
    @ModifiedBy    INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Student]
               WHERE [StudentNumber] = @StudentNumber AND [Id] <> @Id)
        THROW 50008, 'StudentNumber already exists on another student.', 1;

    UPDATE [dbo].[Student]
    SET [UserId]        = @UserId,
        [StudentNumber] = @StudentNumber,
        [Email]         = @Email,
        [Phone]         = @Phone,
        [ModifiedBy]    = @ModifiedBy,
        [ModifiedOn]    = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;

    IF @FullName IS NOT NULL
        UPDATE [dbo].[User]
        SET [FullName]   = @FullName,
            [ModifiedOn] = SYSDATETIMEOFFSET()
        WHERE [Id] = @UserId;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- =============================================
-- USER Stored Procedures
-- =============================================

-- ----------------------------------------------------------------------------
-- usp_CreateUser — returns generated Id via @Id OUTPUT (kept for DAL compat)
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_CreateUser]
    @UserName     NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName     NVARCHAR(100),
    @Role         INT,
    @IsActive     BIT,
    @CreatedBy    INT = 0,
    @Id           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[User] WHERE [UserName] = @UserName)
        THROW 50009, 'UserName already exists.', 1;

    INSERT INTO [dbo].[User]
        ([UserName],[PasswordHash],[FullName],[Role],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@UserName,@PasswordHash,@FullName,@Role,@IsActive,@CreatedBy,@CreatedBy);

    SET @Id = SCOPE_IDENTITY();
END
GO

-- ----------------------------------------------------------------------------
-- usp_DeleteUser
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteUser]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[User] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetAllUsers
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[User];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetUserById
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetUserById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[User]
    WHERE [Id] = @Id;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetUserByUserName
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetUserByUserName]
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[User]
    WHERE [UserName] = @UserName;
END
GO

-- ----------------------------------------------------------------------------
-- usp_SearchUsers
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_SearchUsers]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],
        [CreatedOn],[ModifiedOn],[CreatedBy],[ModifiedBy]
    FROM [dbo].[User]
    WHERE [UserName] LIKE '%' + @regex + '%'
       OR [FullName] LIKE '%' + @regex + '%';
END
GO

-- ----------------------------------------------------------------------------
-- usp_UpdateUser
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_UpdateUser]
    @Id           INT,
    @UserName     NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName     NVARCHAR(100),
    @Role         INT,
    @IsActive     BIT,
    @ModifiedBy   INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[User]
               WHERE [UserName] = @UserName AND [Id] <> @Id)
        THROW 50009, 'UserName already exists on another user.', 1;

    UPDATE [dbo].[User]
    SET [UserName]     = @UserName,
        [PasswordHash] = @PasswordHash,
        [FullName]     = @FullName,
        [Role]         = @Role,
        [IsActive]     = @IsActive,
        [ModifiedBy]   = @ModifiedBy,
        [ModifiedOn]   = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetStudentProfileByUserId
-- Canonical columns: StudentId + UserId (provider falls back to legacy Id).
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetStudentProfileByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.[Id] AS [StudentId],
        u.[Id] AS [UserId],
        s.[Id] AS [Id],
        u.[UserName],
        u.[FullName],
        u.[Role],
        s.[StudentNumber],
        s.[Email],
        s.[Phone]
    FROM [dbo].[User] u
    INNER JOIN [dbo].[Student] s ON s.[UserId] = u.[Id]
    WHERE u.[Id] = @UserId;
END
GO

-- ----------------------------------------------------------------------------
-- usp_GetStudentRegistrationsWithClasses
-- NOTE: Class PK aliased as Class_Id to avoid collision with Registration Id.
-- ----------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetStudentRegistrationsWithClasses]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        r.[Id],
        r.[StudentId],
        r.[ClassId],
        r.[RegistrationDate],
        r.[Status],
        c.[Id] AS [Class_Id],
        c.[CourseId],
        c.[ClassName],
        c.[Instructor],
        c.[MaxCapacity],
        c.[CurrentCapacity],
        c.[StartDate],
        c.[EndDate],
        c.[Schedule],
        c.[IsActive]
    FROM [dbo].[Registrations] r
    INNER JOIN [dbo].[Class] c ON r.[ClassId] = c.[Id]
    WHERE r.[StudentId] = @StudentId;
END
GO
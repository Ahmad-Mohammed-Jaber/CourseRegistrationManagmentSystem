USE [CourseManagementDB]
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET XACT_ABORT ON;
GO

-- ============================================================================
-- SECTION 0: DROP EVERYTHING (reverse dependency order)
-- ============================================================================

-- 0.1 Drop stored procedures first (no dependencies, but keep clean)
DROP PROCEDURE IF EXISTS [dbo].[usp_CreateClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteClass];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateCourse];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetCourses];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateCourse];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteCourse];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetUsers];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteUser];

-- Retired procedure names. They are no longer created (their logic lives in
-- usp_GetClasses / usp_GetUsers / usp_GetCourses / usp_GetStudents /
-- usp_GetRegistrations), but they are still dropped so re-running
-- this script cleans up installations that predate the rename.
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassByName];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassesByCourseId];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllUsers];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetPagedUsers];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetUserById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetUserByUserName];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchUsers];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllCourses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetCourseById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetCourseByCode];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchCourses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllStudents];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentByUserId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentProfileByUserId];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchStudents];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllRegistrations];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllRegistrationsDetailed];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationByStudentAndClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationsByClassId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationsByStudentId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentRegistrationsWithClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_RegistrationExists];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchRegistrations];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchRegistrationsDetailed];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateRefreshToken];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRefreshTokenByHash];
DROP PROCEDURE IF EXISTS [dbo].[usp_RevokeRefreshToken];
DROP PROCEDURE IF EXISTS [dbo].[usp_RevokeRefreshTokensForUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteExpiredRefreshTokens];
-- Obsolete/broken procedure left behind by older installations. The app does
-- not call it and its ReplacedByHash column is not part of the current schema.
DROP PROCEDURE IF EXISTS [dbo].[usp_MarkRefreshTokenRotated];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateStudent];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudents];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateStudent];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteStudent];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateRegistration];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrations];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateRegistration];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteRegistration];

DROP PROCEDURE IF EXISTS [dbo].[usp_InsertBusinessException];
DROP PROCEDURE IF EXISTS [dbo].[usp_InsertDatabaseException];
DROP PROCEDURE IF EXISTS [dbo].[usp_InsertViewError];
GO

-- 0.2 Drop tables (children first, then parents)
DROP TABLE IF EXISTS [dbo].[RefreshTokens];
DROP TABLE IF EXISTS [dbo].[Registrations];
DROP TABLE IF EXISTS [dbo].[Class];
DROP TABLE IF EXISTS [dbo].[Student];
DROP TABLE IF EXISTS [dbo].[Course];
DROP TABLE IF EXISTS [dbo].[User];
DROP TABLE IF EXISTS [dbo].[BusinessExceptions];
DROP TABLE IF EXISTS [dbo].[DatabaseExceptions];
DROP TABLE IF EXISTS [dbo].[ViewErrors];
GO

-- ============================================================================
-- SECTION 1: SCHEMA (Tables, Constraints, Defaults, Foreign Keys)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1.1 [User]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[User](
                             [Id] [int] IDENTITY(1,1) NOT NULL,
                             [UserName] [nvarchar](100) NOT NULL,
                             [PasswordHash] [nvarchar](255) NOT NULL,
                             [FullName] [nvarchar](100) NOT NULL,
                             [Role] [int] NOT NULL,
                             [IsActive] [bit] NOT NULL,
                             [CreatedOn] [datetimeoffset](7) NOT NULL,
                             [ModifiedOn] [datetimeoffset](7) NOT NULL,
                             [CreatedBy] [int] NOT NULL,
                             [ModifiedBy] [int] NOT NULL,
                             CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
                                 (
                                  [Id] ASC
                                     )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                             CONSTRAINT [UQ_User_UserName] UNIQUE NONCLUSTERED
                                 (
                                  [UserName] ASC
                                     )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.1b [RefreshTokens] (opaque refresh tokens; only hashes stored)
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[RefreshTokens](
                                      [Id] [int] IDENTITY(1,1) NOT NULL,
                                      [UserId] [int] NOT NULL,
                                      [TokenHash] [char](64) NOT NULL,
                                      [ExpiresAt] [datetimeoffset](7) NOT NULL,
                                      [CreatedAt] [datetimeoffset](7) NOT NULL,
                                      [RevokedAt] [datetimeoffset](7) NULL,
                                      CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED
                                          (
                                           [Id] ASC
                                              )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                                      CONSTRAINT [UQ_RefreshTokens_TokenHash] UNIQUE NONCLUSTERED
                                          (
                                           [TokenHash] ASC
                                              )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]
    (
     [UserId] ASC
        ) ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.2 [Course]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Course](
                               [Id] [int] IDENTITY(1,1) NOT NULL,
                               [CourseCode] [nvarchar](20) NOT NULL,
                               [CourseName] [nvarchar](100) NOT NULL,
                               [CreditHours] [decimal](4, 2) NOT NULL,
                               [Description] [nvarchar](max) NOT NULL,
                               [IsActive] [bit] NOT NULL,
                               [CreatedOn] [datetimeoffset](7) NOT NULL,
                               [ModifiedOn] [datetimeoffset](7) NOT NULL,
                               [CreatedBy] [int] NOT NULL,
                               [ModifiedBy] [int] NOT NULL,
                               CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED
                                   (
                                    [Id] ASC
                                       )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- CourseCode uniqueness is scoped to active rows (soft-deleted codes are reusable).
-- Enforced here for race safety; the BL check in CourseService mirrors it.
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Course_CourseCode_Active]
    ON [dbo].[Course] ([CourseCode] ASC)
    WHERE ([IsActive] = 1);
GO

-- ----------------------------------------------------------------------------
-- 1.3 [Student]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Student](
                                [Id] [int] IDENTITY(1,1) NOT NULL,
                                [UserId] [int] NOT NULL,
                                [StudentNumber] [int] NOT NULL,
                                [Email] [nvarchar](100) NOT NULL,
                                [Phone] [nvarchar](20) NOT NULL,
                                [CreatedOn] [datetimeoffset](7) NOT NULL,
                                [ModifiedOn] [datetimeoffset](7) NOT NULL,
                                [CreatedBy] [int] NOT NULL,
                                [ModifiedBy] [int] NOT NULL,
                                CONSTRAINT [PK_Students_1] PRIMARY KEY CLUSTERED
                                    (
                                     [Id] ASC
                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                                CONSTRAINT [UQ_Student_StudentNumber] UNIQUE NONCLUSTERED
                                    (
                                     [StudentNumber] ASC
                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                                CONSTRAINT [UQ_Student_UserId] UNIQUE NONCLUSTERED
                                    (
                                     [UserId] ASC
                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.4 [Class]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Class](
                              [Id] [int] IDENTITY(1,1) NOT NULL,
                              [CourseId] [int] NOT NULL,
                              [ClassName] [nvarchar](50) NOT NULL,
                              [Instructor] [nvarchar](50) NOT NULL,
                              [MaxCapacity] [int] NOT NULL,
                              [CurrentCapacity] [int] NOT NULL,
                              [StartDate] [datetime2](7) NOT NULL,
                              [EndDate] [datetime2](7) NOT NULL,
                              [Schedule] [int] NOT NULL,
                              [IsActive] [bit] NOT NULL,
                              [CreatedOn] [datetimeoffset](7) NOT NULL,
                              [ModifiedOn] [datetimeoffset](7) NOT NULL,
                              [CreatedBy] [int] NOT NULL,
                              [ModifiedBy] [int] NOT NULL,
                              CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED
                                  (
                                   [Id] ASC
                                      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.5 [Registrations]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Registrations](
                                      [Id] [int] IDENTITY(1,1) NOT NULL,
                                      [StudentId] [int] NOT NULL,
                                      [ClassId] [int] NOT NULL,
                                      [RegistrationDate] [datetime2](7) NOT NULL,
                                      [Status] [nvarchar](50) NOT NULL,
                                      [CreatedOn] [datetimeoffset](7) NOT NULL,
                                      [ModifiedOn] [datetimeoffset](7) NOT NULL,
                                      [CreatedBy] [int] NOT NULL,
                                      [ModifiedBy] [int] NOT NULL,
                                      CONSTRAINT [PK_Registrations] PRIMARY KEY CLUSTERED
                                          (
                                           [Id] ASC
                                              )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Registration uniqueness is scoped to non-dropped rows (dropped rows are
-- reactivated in place instead of re-inserted; see usp_DeleteRegistration).
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Registrations_Student_Class_Active]
ON [dbo].[Registrations] ([StudentId] ASC, [ClassId] ASC)
WHERE ([Status] <> N'Dropped');
GO

-- ----------------------------------------------------------------------------
-- 1.6 [BusinessExceptions]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[BusinessExceptions](
                                           [Id] [int] IDENTITY(1,1) NOT NULL,
                                           [TimestampUtc] [datetime2](7) NOT NULL,
                                           [Level] [nvarchar](10) NOT NULL,
                                           [Operation] [nvarchar](200) NOT NULL,
                                           [UserId] [int] NULL,
                                           [ExceptionType] [nvarchar](500) NOT NULL,
                                           [Message] [nvarchar](max) NOT NULL,
                                           [StackTrace] [nvarchar](max) NULL,
                                           CONSTRAINT [PK_BusinessExceptions] PRIMARY KEY CLUSTERED
                                               (
                                                [Id] ASC
                                                   )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.7 [DatabaseExceptions]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[DatabaseExceptions](
                                           [Id] [int] IDENTITY(1,1) NOT NULL,
                                           [TimestampUtc] [datetime2](7) NOT NULL,
                                           [Level] [nvarchar](10) NOT NULL,
                                           [Operation] [nvarchar](200) NOT NULL,
                                           [UserId] [int] NULL,
                                           [ExceptionType] [nvarchar](500) NOT NULL,
                                           [Message] [nvarchar](max) NOT NULL,
                                           [StackTrace] [nvarchar](max) NULL,
                                           CONSTRAINT [PK_DatabaseExceptions] PRIMARY KEY CLUSTERED
                                               (
                                                [Id] ASC
                                                   )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- ----------------------------------------------------------------------------
-- 1.8 [ViewErrors]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[ViewErrors](
                                   [Id] [int] IDENTITY(1,1) NOT NULL,
                                   [TimestampUtc] [datetime2](7) NOT NULL,
                                   [Level] [nvarchar](10) NOT NULL,
                                   [Operation] [nvarchar](200) NOT NULL,
                                   [UserId] [int] NULL,
                                   [ExceptionType] [nvarchar](500) NOT NULL,
                                   [Message] [nvarchar](max) NOT NULL,
                                   [StackTrace] [nvarchar](max) NULL,
                                   CONSTRAINT [PK_ViewErrors] PRIMARY KEY CLUSTERED
                                       (
                                        [Id] ASC
                                           )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- ============================================================================
-- SECTION 2: DEFAULT CONSTRAINTS
-- ============================================================================

ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_CurrentCapacity]  DEFAULT ((0)) FOR [CurrentCapacity]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_Schedule]  DEFAULT ((0)) FOR [Schedule]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_CreatedOn]  DEFAULT (sysdatetimeoffset()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_ModifiedOn]  DEFAULT (sysdatetimeoffset()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_ModifiedBy]  DEFAULT ((0)) FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_Description]  DEFAULT (N'') FOR [Description]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_CreatedOn]  DEFAULT (sysdatetimeoffset()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_ModifiedOn]  DEFAULT (sysdatetimeoffset()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF_Course_ModifiedBy]  DEFAULT ((0)) FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_Date]  DEFAULT (sysutcdatetime()) FOR [RegistrationDate]
GO
ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_Status]  DEFAULT (N'Registered') FOR [Status]
GO
ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_CreatedOn]  DEFAULT (sysdatetimeoffset()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_ModifiedOn]  DEFAULT (sysdatetimeoffset()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Registrations] ADD  CONSTRAINT [DF_Registrations_ModifiedBy]  DEFAULT ((0)) FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_Phone]  DEFAULT (N'') FOR [Phone]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_CreatedOn]  DEFAULT (sysdatetimeoffset()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_ModifiedOn]  DEFAULT (sysdatetimeoffset()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_ModifiedBy]  DEFAULT ((0)) FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_CreatedOn]  DEFAULT (sysdatetimeoffset()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_ModifiedOn]  DEFAULT (sysdatetimeoffset()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_ModifiedBy]  DEFAULT ((0)) FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_CreatedAt]  DEFAULT (sysdatetimeoffset()) FOR [CreatedAt]
GO

-- ============================================================================
-- SECTION 3: FOREIGN KEY CONSTRAINTS
-- ============================================================================

ALTER TABLE [dbo].[Class]  WITH CHECK ADD  CONSTRAINT [FK_Course_Class] FOREIGN KEY([CourseId])
    REFERENCES [dbo].[Course] ([Id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Class] CHECK CONSTRAINT [FK_Course_Class]
GO

ALTER TABLE [dbo].[Registrations]  WITH CHECK ADD  CONSTRAINT [FK_Registrations_Class] FOREIGN KEY([ClassId])
    REFERENCES [dbo].[Class] ([Id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Registrations] CHECK CONSTRAINT [FK_Registrations_Class]
GO

ALTER TABLE [dbo].[Registrations]  WITH CHECK ADD  CONSTRAINT [FK_Registrations_Students] FOREIGN KEY([StudentId])
    REFERENCES [dbo].[Student] ([Id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Registrations] CHECK CONSTRAINT [FK_Registrations_Students]
GO

ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Students_User] FOREIGN KEY([UserId])
    REFERENCES [dbo].[User] ([Id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_Students_User]
GO

ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_User] FOREIGN KEY([UserId])
    REFERENCES [dbo].[User] ([Id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_User]
GO

-- ============================================================================
-- SECTION 4: STORED PROCEDURES
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 4.1 CLASS
-- ----------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_CreateClass]
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

-- Single read entry point for classes. Serves every class read shape:
--   @Id           -> one class
--   @CourseId     -> the classes of one course
--   @UserId       -> adds the IsRegistered flag for that student
--   @Search       -> literal substring over class / instructor / course
--   @StatusFilter -> 'Active' | 'Inactive' | 'All'; NULL means no filter
--   @PageNumber   -> NULL returns every matching row; otherwise one page
--                    plus a second result set carrying the counters.
--
-- The filter is evaluated once into @Filtered (narrow key columns only);
-- the page and the counters are then derived from it, so the predicate
-- never has to be repeated.
CREATE PROCEDURE [dbo].[usp_GetClasses]
    @Id               INT = NULL,
    @CourseId         INT = NULL,
    @UserId           INT = NULL,
    @Search           NVARCHAR(100) = NULL,
    @StatusFilter     NVARCHAR(10) = NULL,
    @ClassNameFilter  NVARCHAR(100) = NULL,
    @InstructorFilter NVARCHAR(100) = NULL,
    @CourseFilter     NVARCHAR(100) = NULL,
    @PageNumber       INT = NULL,
    @PageSize         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber IS NOT NULL AND @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   IS NOT NULL AND @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   IS NOT NULL AND @PageSize   > 100 SET @PageSize = 100;

    -- 2. Escape LIKE wildcards so every filter matches literally.
    DECLARE @Pattern NVARCHAR(300) = NULL;
    IF @Search IS NOT NULL AND LEN(@Search) > 0
    BEGIN
        SET @Pattern = REPLACE(REPLACE(REPLACE(@Search, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @ClassNamePattern NVARCHAR(300) = NULL;
    IF @ClassNameFilter IS NOT NULL AND LEN(@ClassNameFilter) > 0
    BEGIN
        SET @ClassNamePattern = N'%' + REPLACE(REPLACE(REPLACE(@ClassNameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @InstructorPattern NVARCHAR(300) = NULL;
    IF @InstructorFilter IS NOT NULL AND LEN(@InstructorFilter) > 0
    BEGIN
        SET @InstructorPattern = N'%' + REPLACE(REPLACE(REPLACE(@InstructorFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @CoursePattern NVARCHAR(300) = NULL;
    IF @CourseFilter IS NOT NULL AND LEN(@CourseFilter) > 0
    BEGIN
        SET @CoursePattern = N'%' + REPLACE(REPLACE(REPLACE(@CourseFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -- Only the key columns are materialized, so sorting never scans the
    -- wide columns and the join below touches just the current page.
    -------------------------------------------------------------------
    DECLARE @Filtered TABLE (
        [Id]        INT NOT NULL PRIMARY KEY,
        [StartDate] DATETIME2 NOT NULL
    );

    INSERT INTO @Filtered ([Id], [StartDate])
    SELECT
        c.[Id],
        c.[StartDate]
    FROM dbo.[Class] AS c
             LEFT JOIN dbo.[Course] AS co
                       ON co.[Id] = c.[CourseId]
    WHERE (@Id IS NULL OR c.[Id] = @Id)
      AND (@CourseId IS NULL OR c.[CourseId] = @CourseId)
      AND (@ClassNamePattern IS NULL OR c.[ClassName] LIKE @ClassNamePattern ESCAPE N'\')
      AND (@InstructorPattern IS NULL OR c.[Instructor] LIKE @InstructorPattern ESCAPE N'\')
      AND (@CoursePattern IS NULL
           OR co.[CourseCode] LIKE @CoursePattern ESCAPE N'\'
           OR co.[CourseName] LIKE @CoursePattern ESCAPE N'\')
      AND (@StatusFilter IS NULL
           OR @StatusFilter = N'All'
           OR (@StatusFilter = N'Active' AND c.[IsActive] = 1)
           OR (@StatusFilter = N'Inactive' AND c.[IsActive] = 0))
      AND (@Pattern IS NULL
           OR c.[ClassName] LIKE @Pattern ESCAPE N'\'
           OR c.[Instructor] LIKE @Pattern ESCAPE N'\'
           OR co.[CourseName] LIKE @Pattern ESCAPE N'\'
           OR co.[CourseCode] LIKE @Pattern ESCAPE N'\');

    -------------------------------------------------------------------
    -- STEP 2: Cut the page out of the filtered keys
    -------------------------------------------------------------------
    DECLARE @Keys TABLE ([Id] INT NOT NULL PRIMARY KEY);

    IF @PageNumber IS NOT NULL
    BEGIN
        INSERT INTO @Keys ([Id])
        SELECT [Id]
        FROM @Filtered
        ORDER BY [StartDate] ASC, [Id] ASC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        INSERT INTO @Keys ([Id]) SELECT [Id] FROM @Filtered;
    END

    -------------------------------------------------------------------
    -- STEP 3: Join back to base tables for ONLY the current page.
    -------------------------------------------------------------------
    SELECT
        c.[Id],
        c.[CourseId],
        co.[CourseCode],
        co.[CourseName],
        c.[ClassName],
        c.[Instructor],
        c.[Schedule],
        c.[StartDate],
        c.[EndDate],
        c.[CurrentCapacity],
        c.[MaxCapacity],
        c.[IsActive],
        c.[CreatedOn],
        c.[ModifiedOn],
        c.[CreatedBy],
        c.[ModifiedBy],
        cb.[UserName] AS CreatedByName,
        mb.[UserName] AS ModifiedByName,
        CAST(CASE WHEN sr.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsRegistered
    FROM @Keys AS pk
             INNER JOIN dbo.[Class] AS c
                        ON c.[Id] = pk.[Id]
             LEFT JOIN dbo.[Course] AS co
                       ON co.[Id] = c.[CourseId]
             LEFT JOIN dbo.[User] AS cb
                       ON cb.[Id] = c.[CreatedBy]
             LEFT JOIN dbo.[User] AS mb
                       ON mb.[Id] = c.[ModifiedBy]
             LEFT JOIN dbo.[Registrations] AS sr
                       ON sr.[ClassId] = c.[Id]
                           AND @UserId IS NOT NULL
                           AND sr.[StudentId] = @UserId
                           AND sr.[Status] <> N'Dropped'
    ORDER BY
        c.[StartDate] ASC,
        c.[Id] ASC;

    -------------------------------------------------------------------
    -- STEP 4: Counters over the same filtered set
    -------------------------------------------------------------------
    IF @PageNumber IS NOT NULL
    BEGIN
        SELECT
            (SELECT COUNT(*) FROM @Filtered) AS TotalCount,
            ISNULL((
                SELECT SUM(CASE WHEN c.[MaxCapacity] - c.[CurrentCapacity] > 0
                                    THEN c.[MaxCapacity] - c.[CurrentCapacity]
                                ELSE 0 END)
                FROM @Filtered AS f
                         INNER JOIN dbo.[Class] AS c ON c.[Id] = f.[Id]), 0) AS OpenSeatsCount,
            (
                SELECT COUNT(*)
                FROM dbo.[Registrations]
                WHERE @UserId IS NOT NULL
                  AND [StudentId] = @UserId
                  AND [Status] <> N'Dropped'
            ) AS MyRegistrationsCount;
    END
END;
GO

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

CREATE PROCEDURE [dbo].[usp_DeleteClass]
    @Id INT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Class]
    SET [IsActive]   = 0,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- 4.2 COURSE
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

    IF EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [CourseCode] = @CourseCode AND [IsActive] = 1)
        THROW 50002, 'CourseCode already exists.', 1;

    INSERT INTO [dbo].[Course]
    ([CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@CourseCode,@CourseName,@CreditHours,@Description,@IsActive,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- Single read entry point for courses. Serves every course read shape:
--   @Id         -> one course
--   @CourseCode -> one course by its exact code
--   @Search     -> literal substring over course code / name / description
--   @PageNumber -> NULL returns every matching row; otherwise one page
--                  plus a second result set carrying TotalCount.
CREATE PROCEDURE [dbo].[usp_GetCourses]
    @Id                INT = NULL,
    @CourseCode        NVARCHAR(20) = NULL,
    @Search            NVARCHAR(100) = NULL,
    @CodeFilter        NVARCHAR(100) = NULL,
    @NameFilter        NVARCHAR(100) = NULL,
    @DescriptionFilter NVARCHAR(200) = NULL,
    @IsActive          BIT = NULL,
    @PageNumber        INT = NULL,
    @PageSize          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber IS NOT NULL AND @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   IS NOT NULL AND @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   IS NOT NULL AND @PageSize   > 100 SET @PageSize = 100;

    -- 2. Escape LIKE wildcards so every filter matches literally.
    DECLARE @Pattern NVARCHAR(300) = NULL;
    IF @Search IS NOT NULL AND LEN(@Search) > 0
    BEGIN
        SET @Pattern = REPLACE(REPLACE(REPLACE(@Search, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @CodePattern NVARCHAR(300) = NULL;
    IF @CodeFilter IS NOT NULL AND LEN(@CodeFilter) > 0
    BEGIN
        SET @CodePattern = N'%' + REPLACE(REPLACE(REPLACE(@CodeFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @NamePattern NVARCHAR(300) = NULL;
    IF @NameFilter IS NOT NULL AND LEN(@NameFilter) > 0
    BEGIN
        SET @NamePattern = N'%' + REPLACE(REPLACE(REPLACE(@NameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @DescriptionPattern NVARCHAR(300) = NULL;
    IF @DescriptionFilter IS NOT NULL AND LEN(@DescriptionFilter) > 0
    BEGIN
        SET @DescriptionPattern = N'%' + REPLACE(REPLACE(REPLACE(@DescriptionFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -------------------------------------------------------------------
    DECLARE @Filtered TABLE ([Id] INT NOT NULL PRIMARY KEY);

    INSERT INTO @Filtered ([Id])
    SELECT
        co.[Id]
    FROM dbo.[Course] AS co
    WHERE (@Id IS NULL OR co.[Id] = @Id)
      AND (@CourseCode IS NULL OR co.[CourseCode] = @CourseCode)
      AND (@CodePattern IS NULL OR co.[CourseCode] LIKE @CodePattern ESCAPE N'\')
      AND (@NamePattern IS NULL OR co.[CourseName] LIKE @NamePattern ESCAPE N'\')
      AND (@DescriptionPattern IS NULL OR co.[Description] LIKE @DescriptionPattern ESCAPE N'\')
      AND (@IsActive IS NULL OR co.[IsActive] = @IsActive)
      AND (@Pattern IS NULL
           OR co.[CourseCode] LIKE @Pattern ESCAPE N'\'
           OR co.[CourseName] LIKE @Pattern ESCAPE N'\'
           OR co.[Description] LIKE @Pattern ESCAPE N'\');

    -------------------------------------------------------------------
    -- STEP 2: Cut the page out of the filtered keys
    -------------------------------------------------------------------
    DECLARE @Keys TABLE ([Id] INT NOT NULL PRIMARY KEY);

    IF @PageNumber IS NOT NULL
    BEGIN
        INSERT INTO @Keys ([Id])
        SELECT [Id]
        FROM @Filtered
        ORDER BY [Id] ASC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        INSERT INTO @Keys ([Id]) SELECT [Id] FROM @Filtered;
    END

    -------------------------------------------------------------------
    -- STEP 3: Join back for ONLY the current page, audit actors included
    -------------------------------------------------------------------
    SELECT
        co.[Id],
        co.[CourseCode],
        co.[CourseName],
        co.[CreditHours],
        co.[Description],
        co.[IsActive],
        co.[CreatedOn],
        co.[ModifiedOn],
        co.[CreatedBy],
        co.[ModifiedBy],
        cb.[UserName] AS CreatedByName,
        mb.[UserName] AS ModifiedByName
    FROM @Keys AS pk
             INNER JOIN dbo.[Course] AS co
                        ON co.[Id] = pk.[Id]
             LEFT JOIN dbo.[User] AS cb
                       ON cb.[Id] = co.[CreatedBy]
             LEFT JOIN dbo.[User] AS mb
                       ON mb.[Id] = co.[ModifiedBy]
    ORDER BY
        co.[CourseCode] ASC,
        co.[Id] ASC;

    -------------------------------------------------------------------
    -- STEP 4: Counter over the same filtered set
    -------------------------------------------------------------------
    IF @PageNumber IS NOT NULL
    BEGIN
        SELECT COUNT(*) AS TotalCount FROM @Filtered;
    END
END;
GO

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

CREATE PROCEDURE [dbo].[usp_DeleteCourse]
    @Id INT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Course]
    SET [IsActive]   = 0,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- 4.3 USER
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

-- Single read entry point for users. Serves every user read shape:
--   @Id         -> one user
--   @UserName   -> exact match (login, uniqueness check)
--   @Search     -> literal substring over username / full name
--   @PageNumber -> NULL returns every matching row; otherwise one page
--                  plus a second result set carrying TotalCount.
--
-- PasswordHash is only carried when a single user is looked up by id or
-- user name, so it never travels with a list.
CREATE PROCEDURE [dbo].[usp_GetUsers]
    @Id             INT = NULL,
    @UserName       NVARCHAR(100) = NULL,
    @Search         NVARCHAR(100) = NULL,
    @UserNameFilter NVARCHAR(100) = NULL,
    @FullNameFilter NVARCHAR(100) = NULL,
    @Role           INT = NULL,
    @IsActive       BIT = NULL,
    @PageNumber     INT = NULL,
    @PageSize       INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber IS NOT NULL AND @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   IS NOT NULL AND @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   IS NOT NULL AND @PageSize   > 100 SET @PageSize = 100;

    -- 2. Escape LIKE wildcards so every filter matches literally.
    DECLARE @Pattern NVARCHAR(300) = NULL;
    IF @Search IS NOT NULL AND LEN(@Search) > 0
    BEGIN
        SET @Pattern = REPLACE(REPLACE(REPLACE(@Search, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @UserNamePattern NVARCHAR(300) = NULL;
    IF @UserNameFilter IS NOT NULL AND LEN(@UserNameFilter) > 0
    BEGIN
        SET @UserNamePattern = N'%' + REPLACE(REPLACE(REPLACE(@UserNameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @FullNamePattern NVARCHAR(300) = NULL;
    IF @FullNameFilter IS NOT NULL AND LEN(@FullNameFilter) > 0
    BEGIN
        SET @FullNamePattern = N'%' + REPLACE(REPLACE(REPLACE(@FullNameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -------------------------------------------------------------------
    DECLARE @Filtered TABLE ([Id] INT NOT NULL PRIMARY KEY);

    INSERT INTO @Filtered ([Id])
    SELECT
        u.[Id]
    FROM dbo.[User] AS u
    WHERE (@Id IS NULL OR u.[Id] = @Id)
      AND (@UserName IS NULL OR u.[UserName] = @UserName)
      AND (@UserNamePattern IS NULL OR u.[UserName] LIKE @UserNamePattern ESCAPE N'\')
      AND (@FullNamePattern IS NULL OR u.[FullName] LIKE @FullNamePattern ESCAPE N'\')
      AND (@Role IS NULL OR u.[Role] = @Role)
      AND (@IsActive IS NULL OR u.[IsActive] = @IsActive)
      AND (@Pattern IS NULL
           OR u.[UserName] LIKE @Pattern ESCAPE N'\'
           OR u.[FullName] LIKE @Pattern ESCAPE N'\');

    -------------------------------------------------------------------
    -- STEP 2: Cut the page out of the filtered keys
    -------------------------------------------------------------------
    DECLARE @Keys TABLE ([Id] INT NOT NULL PRIMARY KEY);

    IF @PageNumber IS NOT NULL
    BEGIN
        INSERT INTO @Keys ([Id])
        SELECT [Id]
        FROM @Filtered
        ORDER BY [Id] ASC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        INSERT INTO @Keys ([Id]) SELECT [Id] FROM @Filtered;
    END

    -------------------------------------------------------------------
    -- STEP 3: Join back to base tables for ONLY the current page.
    -------------------------------------------------------------------
    SELECT
        u.[Id],
        u.[UserName],
        u.[FullName],
        u.[Role],
        u.[IsActive],
        u.[CreatedOn],
        u.[ModifiedOn],
        u.[CreatedBy],
        u.[ModifiedBy],
        cb.[UserName] AS CreatedByName,
        mb.[UserName] AS ModifiedByName,
        CASE WHEN @Id IS NOT NULL OR @UserName IS NOT NULL
             THEN u.[PasswordHash]
             ELSE NULL
        END AS PasswordHash
    FROM @Keys AS pk
             INNER JOIN dbo.[User] AS u
                        ON u.[Id] = pk.[Id]
             LEFT JOIN dbo.[User] AS cb
                       ON cb.[Id] = u.[CreatedBy]
             LEFT JOIN dbo.[User] AS mb
                       ON mb.[Id] = u.[ModifiedBy]
    ORDER BY
        u.[UserName] ASC,
        u.[Id] ASC;

    -------------------------------------------------------------------
    -- STEP 4: Counter over the same filtered set
    -------------------------------------------------------------------
    IF @PageNumber IS NOT NULL
    BEGIN
        SELECT COUNT(*) AS TotalCount FROM @Filtered;
    END
END;
GO

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

CREATE PROCEDURE [dbo].[usp_DeleteUser]
    @Id INT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[User]
    SET [IsActive]   = 0,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- 4.3b REFRESH TOKENS
-- ----------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_CreateRefreshToken]
    @UserId     INT,
    @TokenHash  CHAR(64),
    @ExpiresAt  DATETIMEOFFSET(7),
    @Id         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[RefreshTokens] WHERE [TokenHash] = @TokenHash)
        THROW 50009, 'Refresh token hash already exists.', 1;

    INSERT INTO [dbo].[RefreshTokens]
    ([UserId],[TokenHash],[ExpiresAt])
    VALUES
        (@UserId,@TokenHash,@ExpiresAt);

    SET @Id = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE [dbo].[usp_GetRefreshTokenByHash]
@TokenHash CHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],[UserId],[TokenHash],[ExpiresAt],[CreatedAt],[RevokedAt]
    FROM [dbo].[RefreshTokens]
    WHERE [TokenHash] = @TokenHash;
END
GO

CREATE PROCEDURE [dbo].[usp_RevokeRefreshToken]
@TokenHash CHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[RefreshTokens]
    SET [RevokedAt] = SYSDATETIMEOFFSET()
    WHERE [TokenHash] = @TokenHash AND [RevokedAt] IS NULL;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

CREATE PROCEDURE [dbo].[usp_RevokeRefreshTokensForUser]
@UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[RefreshTokens]
    SET [RevokedAt] = SYSDATETIMEOFFSET()
    WHERE [UserId] = @UserId AND [RevokedAt] IS NULL;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

CREATE PROCEDURE [dbo].[usp_DeleteExpiredRefreshTokens]
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[RefreshTokens]
    WHERE [ExpiresAt] < SYSDATETIMEOFFSET()
      AND [RevokedAt] IS NOT NULL;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- 4.4 STUDENT
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
END
GO

-- Single read entry point for students. Serves every student read shape:
--   @Id         -> one student
--   @UserId     -> the student profile of one user
--   @Search     -> literal substring over student number / email / user name
--   @PageNumber -> NULL returns every matching row; otherwise one page
--                  plus a second result set carrying TotalCount.
--
-- The user's display columns are joined in, so no caller has to fetch the
-- user row separately. PasswordHash is only carried for a single-student
-- lookup, where the create/update flow needs it.
CREATE PROCEDURE [dbo].[usp_GetStudents]
    @Id                  INT = NULL,
    @UserId              INT = NULL,
    @Search              NVARCHAR(100) = NULL,
    @StudentNumberFilter NVARCHAR(30) = NULL,
    @FullNameFilter      NVARCHAR(100) = NULL,
    @UserNameFilter      NVARCHAR(100) = NULL,
    @EmailFilter         NVARCHAR(100) = NULL,
    @PhoneFilter         NVARCHAR(20) = NULL,
    @IsActive            BIT = NULL,
    @PageNumber          INT = NULL,
    @PageSize            INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber IS NOT NULL AND @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   IS NOT NULL AND @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   IS NOT NULL AND @PageSize   > 100 SET @PageSize = 100;

    -- 2. Escape LIKE wildcards so every filter matches literally.
    DECLARE @Pattern NVARCHAR(300) = NULL;
    IF @Search IS NOT NULL AND LEN(@Search) > 0
    BEGIN
        SET @Pattern = REPLACE(REPLACE(REPLACE(@Search, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @StudentNumberPattern NVARCHAR(300) = NULL;
    IF @StudentNumberFilter IS NOT NULL AND LEN(@StudentNumberFilter) > 0
    BEGIN
        SET @StudentNumberPattern = N'%' + REPLACE(REPLACE(REPLACE(@StudentNumberFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @FullNamePattern NVARCHAR(300) = NULL;
    IF @FullNameFilter IS NOT NULL AND LEN(@FullNameFilter) > 0
    BEGIN
        SET @FullNamePattern = N'%' + REPLACE(REPLACE(REPLACE(@FullNameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @UserNamePattern NVARCHAR(300) = NULL;
    IF @UserNameFilter IS NOT NULL AND LEN(@UserNameFilter) > 0
    BEGIN
        SET @UserNamePattern = N'%' + REPLACE(REPLACE(REPLACE(@UserNameFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @EmailPattern NVARCHAR(300) = NULL;
    IF @EmailFilter IS NOT NULL AND LEN(@EmailFilter) > 0
    BEGIN
        SET @EmailPattern = N'%' + REPLACE(REPLACE(REPLACE(@EmailFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @PhonePattern NVARCHAR(300) = NULL;
    IF @PhoneFilter IS NOT NULL AND LEN(@PhoneFilter) > 0
    BEGIN
        SET @PhonePattern = N'%' + REPLACE(REPLACE(REPLACE(@PhoneFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -------------------------------------------------------------------
    DECLARE @Filtered TABLE ([Id] INT NOT NULL PRIMARY KEY);

    INSERT INTO @Filtered ([Id])
    SELECT
        s.[Id]
    FROM dbo.[Student] AS s
             INNER JOIN dbo.[User] AS u ON u.[Id] = s.[UserId]
    WHERE (@Id IS NULL OR s.[Id] = @Id)
      AND (@UserId IS NULL OR s.[UserId] = @UserId)
      AND (@StudentNumberPattern IS NULL OR CAST(s.[StudentNumber] AS NVARCHAR(20)) LIKE @StudentNumberPattern ESCAPE N'\')
      AND (@FullNamePattern IS NULL OR u.[FullName] LIKE @FullNamePattern ESCAPE N'\')
      AND (@UserNamePattern IS NULL OR u.[UserName] LIKE @UserNamePattern ESCAPE N'\')
      AND (@EmailPattern IS NULL OR s.[Email] LIKE @EmailPattern ESCAPE N'\')
      AND (@PhonePattern IS NULL OR s.[Phone] LIKE @PhonePattern ESCAPE N'\')
      AND (@IsActive IS NULL OR u.[IsActive] = @IsActive)
      AND (@Pattern IS NULL
           OR CAST(s.[StudentNumber] AS NVARCHAR(20)) LIKE @Pattern ESCAPE N'\'
           OR s.[Email] LIKE @Pattern ESCAPE N'\'
           OR u.[UserName] LIKE @Pattern ESCAPE N'\'
           OR u.[FullName] LIKE @Pattern ESCAPE N'\');

    -------------------------------------------------------------------
    -- STEP 2: Cut the page out of the filtered keys
    -------------------------------------------------------------------
    DECLARE @Keys TABLE ([Id] INT NOT NULL PRIMARY KEY);

    IF @PageNumber IS NOT NULL
    BEGIN
        INSERT INTO @Keys ([Id])
        SELECT [Id]
        FROM @Filtered
        ORDER BY [Id] ASC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        INSERT INTO @Keys ([Id]) SELECT [Id] FROM @Filtered;
    END

    -------------------------------------------------------------------
    -- STEP 3: Join back for ONLY the current page, audit actors included
    -------------------------------------------------------------------
    SELECT
        s.[Id],
        s.[UserId],
        s.[StudentNumber],
        s.[Email],
        s.[Phone],
        u.[UserName],
        u.[FullName],
        u.[Role],
        u.[IsActive],
        s.[CreatedOn],
        s.[ModifiedOn],
        s.[CreatedBy],
        s.[ModifiedBy],
        cb.[UserName] AS CreatedByName,
        mb.[UserName] AS ModifiedByName,
        CASE WHEN @Id IS NOT NULL OR @UserId IS NOT NULL
             THEN u.[PasswordHash]
             ELSE NULL
        END AS PasswordHash
    FROM @Keys AS pk
             INNER JOIN dbo.[Student] AS s
                        ON s.[Id] = pk.[Id]
             INNER JOIN dbo.[User] AS u
                        ON u.[Id] = s.[UserId]
             LEFT JOIN dbo.[User] AS cb
                       ON cb.[Id] = s.[CreatedBy]
             LEFT JOIN dbo.[User] AS mb
                       ON mb.[Id] = s.[ModifiedBy]
    ORDER BY
        u.[UserName] ASC,
        s.[Id] ASC;

    -------------------------------------------------------------------
    -- STEP 4: Counter over the same filtered set
    -------------------------------------------------------------------
    IF @PageNumber IS NOT NULL
    BEGIN
        SELECT COUNT(*) AS TotalCount FROM @Filtered;
    END
END;
GO

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

CREATE PROCEDURE [dbo].[usp_DeleteStudent]
    @Id INT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    -- Student has no IsActive column; soft delete = deactivate the linked User login.
    -- The Student row itself is kept for history.
    UPDATE [dbo].[User]
    SET [IsActive]   = 0,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = (SELECT [UserId] FROM [dbo].[Student] WHERE [Id] = @Id);

    UPDATE [dbo].[Student]
    SET [ModifiedBy] = @ModifiedBy,
        [ModifiedOn] = SYSDATETIMEOFFSET()
    WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ----------------------------------------------------------------------------
-- 4.5 REGISTRATION
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
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId AND [Status] <> N'Dropped')
        THROW 50005, 'Student is already registered for this class.', 1;

    INSERT INTO [dbo].[Registrations]
    ([StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (@StudentId,@ClassId,@RegistrationDate,@Status,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- Single read entry point for registrations. Serves every registration read
-- shape:
--   @Id         -> one registration
--   @StudentId  -> the registrations of one student
--   @ClassId    -> the registrations for one class
--   @StudentId + @ClassId -> the single row for that pair (any status)
--   @Search     -> literal substring over student / class / course
--   @PageNumber -> NULL returns every matching row; otherwise one page
--                  plus a second result set carrying TotalCount.
--
-- The class, student, user and course are joined in, so no caller has to
-- fetch the related rows separately.
CREATE PROCEDURE [dbo].[usp_GetRegistrations]
    @Id           INT = NULL,
    @StudentId    INT = NULL,
    @ClassId      INT = NULL,
    @Search       NVARCHAR(100) = NULL,
    @StudentFilter NVARCHAR(100) = NULL,
    @CourseFilter  NVARCHAR(100) = NULL,
    @ClassFilter   NVARCHAR(100) = NULL,
    @StatusFilter  NVARCHAR(20) = NULL,
    @PageNumber   INT = NULL,
    @PageSize     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Sanitize Pagination Inputs
    IF @PageNumber IS NOT NULL AND @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize   IS NOT NULL AND @PageSize   < 1 SET @PageSize   = 20;
    IF @PageSize   IS NOT NULL AND @PageSize   > 100 SET @PageSize = 100;

    -- 2. Escape LIKE wildcards so every filter matches literally.
    DECLARE @Pattern NVARCHAR(300) = NULL;
    IF @Search IS NOT NULL AND LEN(@Search) > 0
    BEGIN
        SET @Pattern = REPLACE(REPLACE(REPLACE(@Search, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @StudentPattern NVARCHAR(300) = NULL;
    IF @StudentFilter IS NOT NULL AND LEN(@StudentFilter) > 0
    BEGIN
        SET @StudentPattern = N'%' + REPLACE(REPLACE(REPLACE(@StudentFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @CoursePattern NVARCHAR(300) = NULL;
    IF @CourseFilter IS NOT NULL AND LEN(@CourseFilter) > 0
    BEGIN
        SET @CoursePattern = N'%' + REPLACE(REPLACE(REPLACE(@CourseFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END
    DECLARE @ClassPattern NVARCHAR(300) = NULL;
    IF @ClassFilter IS NOT NULL AND LEN(@ClassFilter) > 0
    BEGIN
        SET @ClassPattern = N'%' + REPLACE(REPLACE(REPLACE(@ClassFilter, N'\', N'\\'), N'%', N'\%'), N'_', N'\_') + N'%';
    END

    -------------------------------------------------------------------
    -- STEP 1: Isolate Primary Keys ONLY (Erik Darling Paging Pattern)
    -------------------------------------------------------------------
    DECLARE @Filtered TABLE ([Id] INT NOT NULL PRIMARY KEY);

    INSERT INTO @Filtered ([Id])
    SELECT
        r.[Id]
    FROM dbo.[Registrations] AS r
             INNER JOIN dbo.[Student] AS s  ON r.[StudentId] = s.[Id]
             INNER JOIN dbo.[User]    AS u  ON s.[UserId]    = u.[Id]
             INNER JOIN dbo.[Class]   AS c  ON r.[ClassId]   = c.[Id]
             INNER JOIN dbo.[Course]  AS co ON c.[CourseId]  = co.[Id]
    WHERE (@Id IS NULL OR r.[Id] = @Id)
      AND (@StudentId IS NULL OR r.[StudentId] = @StudentId)
      AND (@ClassId IS NULL OR r.[ClassId] = @ClassId)
      AND (@StudentPattern IS NULL
           OR CAST(s.[StudentNumber] AS NVARCHAR(20)) LIKE @StudentPattern ESCAPE N'\'
           OR s.[Email] LIKE @StudentPattern ESCAPE N'\'
           OR u.[UserName] LIKE @StudentPattern ESCAPE N'\'
           OR u.[FullName] LIKE @StudentPattern ESCAPE N'\')
      AND (@CoursePattern IS NULL
           OR co.[CourseCode] LIKE @CoursePattern ESCAPE N'\'
           OR co.[CourseName] LIKE @CoursePattern ESCAPE N'\')
      AND (@ClassPattern IS NULL OR c.[ClassName] LIKE @ClassPattern ESCAPE N'\')
      AND (@StatusFilter IS NULL
           OR @StatusFilter = N'All'
           OR r.[Status] = @StatusFilter)
      AND (@Pattern IS NULL
           OR CAST(s.[StudentNumber] AS NVARCHAR(20)) LIKE @Pattern ESCAPE N'\'
           OR s.[Email] LIKE @Pattern ESCAPE N'\'
           OR u.[UserName] LIKE @Pattern ESCAPE N'\'
           OR u.[FullName] LIKE @Pattern ESCAPE N'\'
           OR c.[ClassName] LIKE @Pattern ESCAPE N'\'
           OR co.[CourseCode] LIKE @Pattern ESCAPE N'\'
           OR co.[CourseName] LIKE @Pattern ESCAPE N'\');

    -------------------------------------------------------------------
    -- STEP 2: Cut the page out of the filtered keys
    -------------------------------------------------------------------
    DECLARE @Keys TABLE ([Id] INT NOT NULL PRIMARY KEY);

    IF @PageNumber IS NOT NULL
    BEGIN
        INSERT INTO @Keys ([Id])
        SELECT [Id]
        FROM @Filtered
        ORDER BY [Id] ASC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        INSERT INTO @Keys ([Id]) SELECT [Id] FROM @Filtered;
    END

    -------------------------------------------------------------------
    -- STEP 3: Join back for ONLY the current page.
    -------------------------------------------------------------------
    SELECT
        r.[Id]                AS RegistrationId,
        r.[StudentId]         AS RegistrationStudentId,
        r.[ClassId]           AS RegistrationClassId,
        r.[RegistrationDate]  AS RegistrationDate,
        r.[Status]            AS RegistrationStatus,
        r.[CreatedOn]         AS RegistrationCreatedOn,
        r.[ModifiedOn]        AS RegistrationModifiedOn,
        r.[CreatedBy]         AS RegistrationCreatedBy,
        r.[ModifiedBy]        AS RegistrationModifiedBy,
        c.[Id]                AS ClassId,
        c.[CourseId]          AS ClassCourseId,
        c.[ClassName]         AS ClassName,
        c.[Instructor]        AS ClassInstructor,
        c.[MaxCapacity]       AS ClassMaxCapacity,
        c.[CurrentCapacity]   AS ClassCurrentCapacity,
        c.[StartDate]         AS ClassStartDate,
        c.[EndDate]           AS ClassEndDate,
        c.[Schedule]          AS ClassSchedule,
        c.[IsActive]          AS ClassIsActive,
        s.[Id]                AS StudentId,
        s.[UserId]            AS StudentUserId,
        s.[StudentNumber]     AS StudentNumber,
        s.[Email]             AS StudentEmail,
        s.[Phone]             AS StudentPhone,
        u.[UserName]          AS StudentUserName,
        u.[FullName]          AS StudentFullName,
        u.[Role]              AS StudentRole,
        u.[IsActive]          AS StudentIsActive,
        co.[Id]               AS CourseId,
        co.[CourseCode]       AS CourseCode,
        co.[CourseName]       AS CourseName,
        cb.[UserName]         AS CreatedByName,
        mb.[UserName]         AS ModifiedByName
    FROM @Keys AS pk
             INNER JOIN dbo.[Registrations] AS r ON r.[Id] = pk.[Id]
             INNER JOIN dbo.[Student] AS s        ON s.[Id] = r.[StudentId]
             INNER JOIN dbo.[User] AS u           ON u.[Id] = s.[UserId]
             INNER JOIN dbo.[Class] AS c          ON c.[Id] = r.[ClassId]
             INNER JOIN dbo.[Course] AS co        ON co.[Id] = c.[CourseId]
             LEFT JOIN dbo.[User] AS cb           ON cb.[Id] = r.[CreatedBy]
             LEFT JOIN dbo.[User] AS mb           ON mb.[Id] = r.[ModifiedBy]
    ORDER BY
        r.[RegistrationDate] DESC,
        r.[Id] DESC;

    -------------------------------------------------------------------
    -- STEP 4: Counter over the same filtered set
    -------------------------------------------------------------------
    IF @PageNumber IS NOT NULL
    BEGIN
        SELECT COUNT(*) AS TotalCount FROM @Filtered;
    END
END;
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
-- 4.6 EXCEPTION LOGGING
-- ----------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_InsertBusinessException]
    @TimestampUtc  DATETIME2(7),
    @Level         NVARCHAR(10),
    @Operation     NVARCHAR(200),
    @UserId        INT = NULL,
    @ExceptionType NVARCHAR(500),
    @Message       NVARCHAR(MAX),
    @StackTrace    NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[BusinessExceptions]
    ([TimestampUtc],[Level],[Operation],[UserId],[ExceptionType],[Message],[StackTrace])
    VALUES
        (@TimestampUtc,@Level,@Operation,@UserId,@ExceptionType,@Message,@StackTrace);
END
GO

CREATE PROCEDURE [dbo].[usp_InsertDatabaseException]
    @TimestampUtc  DATETIME2(7),
    @Level         NVARCHAR(10),
    @Operation     NVARCHAR(200),
    @UserId        INT = NULL,
    @ExceptionType NVARCHAR(500),
    @Message       NVARCHAR(MAX),
    @StackTrace    NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[DatabaseExceptions]
    ([TimestampUtc],[Level],[Operation],[UserId],[ExceptionType],[Message],[StackTrace])
    VALUES
        (@TimestampUtc,@Level,@Operation,@UserId,@ExceptionType,@Message,@StackTrace);
END
GO

CREATE PROCEDURE [dbo].[usp_InsertViewError]
    @TimestampUtc  DATETIME2(7),
    @Level         NVARCHAR(10),
    @Operation     NVARCHAR(200),
    @UserId        INT = NULL,
    @ExceptionType NVARCHAR(500),
    @Message       NVARCHAR(MAX),
    @StackTrace    NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[ViewErrors]
    ([TimestampUtc],[Level],[Operation],[UserId],[ExceptionType],[Message],[StackTrace])
    VALUES
        (@TimestampUtc,@Level,@Operation,@UserId,@ExceptionType,@Message,@StackTrace);
END
GO

-- ============================================================================
-- SECTION 5: SEED DATA (BL-compliant, easily testable)
-- ============================================================================
-- BL rules covered (see BL/Validation + BL/Services):
--  User: UserName required + UNIQUE (case-insensitive), FullName required,
--        Password >= 6 chars, Role 0=Admin / 1=Student, IsActive flag.
--  Student: StudentNumber = exactly 9 digits "YYYY S NNNN", YYYY in 2000..YEAR(GETDATE()),
--        S in (1,2,3); Email regex ^[^@\s]+@[^@\s]+\.[^@\s]+$; Phone regex ^\+?[\d\s\-()]{7,20}$
--        (NVARCHAR(20)); UserId must exist + UNIQUE; StudentNumber UNIQUE;
--        create auto-creates a Role=Student User; delete cascades to User.
--  Course: CourseCode required + UNIQUE (trimmed, case-insensitive), CourseName required,
--        CreditHours >= 0.
--  Class: ClassName required; CourseId must exist; MaxCapacity > 0; CurrentCapacity >= 0;
--        CurrentCapacity <= MaxCapacity; EndDate >= StartDate;
--        Update: MaxCapacity cannot go below current enrollment;
--        Register path: IsActive must be 1 AND CurrentCapacity < MaxCapacity AND no duplicate.
--  Registration: StudentId/ClassId > 0 and must exist; Status required (default 'Registered');
--        UNIQUE(StudentId, ClassId); drop requires ownership + student profile;
--        capacity counts every status EXCEPT 'Dropped'.
--  Schedule (Class.DaysOfWeek flags): None=0, Sun=2, Mon=4, Tue=8, Wed=16, Thu=32, Fri=64, Sat=128.
--  Search: pattern must be non-empty, valid .NET regex.
--  Cascades: User->Student->Registrations; Course->Class->Registrations (all ON DELETE CASCADE).
--
-- Test map (predictable Ids, searchable names):
--  Users:  1=admin (Role 0, no student row) | 2-5=active students | 6=INACTIVE student user |
--         7=orphan.user (active, NO student row -> "profile not found" test).
--  Students: 1-4=active (semesters 1,2,3,1 covered) | 5=inactive user's student, ZERO registrations
--        (empty-list test). Every number is 9-digit YYYY S NNNN.
--  Courses: 1-3=active core | 4=TEST404 INACTIVE course | 5=TMP999 active, NO classes (safe delete +
--        unique-code conflict test; deleting 5 touches nothing).
--  Classes: 1=OPEN (2/30) | 2=OPEN (1/25, Dropped excluded) | 3=OPEN (2/35) | 4=OPEN (1/30 Pending) |
--        5=CS101-FULL 2/2 (register -> "Class is full") | 6=CS101-ONELEFT 2/3 (last seat succeeds) |
--        7=MATH201-ARCHIVE INACTIVE (register -> "no longer active") |
--        8=ENG101-EMPTY 0/20 (empty class + safe delete).
--  Registrations: Registered x8 | Pending x1 (Id 4) | Dropped x1 (Id 11, excluded from capacity).
--        Duplicates: re-adding (Student 1, Class 1) -> conflict. Ownership: Reg 1 belongs to
--        Student 1 (Student 2 dropping it -> Forbidden).
--  All seed users share one bcrypt hash so every login is testable with the same known password.

BEGIN TRY
    BEGIN TRANSACTION SeedDemoData;

    -- ---- 5a. Users -------------------------------------------------------
    SET IDENTITY_INSERT [dbo].[User] ON;
    INSERT INTO [dbo].[User]
    ([Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, N'admin',             N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'System Administrator', 0, 1, 0, 0),
        (2, N'sara.ahmed',        N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Sara Ahmed',           1, 1, 0, 0),
        (3, N'omar.khaled',       N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Omar Khaled',          1, 1, 0, 0),
        (4, N'lina.hassan',       N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Lina Hassan',          1, 1, 0, 0),
        (5, N'yousef.nasser',     N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Yousef Nasser',        1, 1, 0, 0),
        (6, N'inactive.student',  N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Inactive Student',     1, 0, 0, 0),
        (7, N'orphan.user',       N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Orphan User',          1, 1, 0, 0);
    SET IDENTITY_INSERT [dbo].[User] OFF;

    -- ---- 5b. Students ----------------------------------------------------
    -- 9-digit YYYY S NNNN: semesters 1,2,3 all represented; years 2000..YEAR(GETDATE()).
    SET IDENTITY_INSERT [dbo].[Student] ON;
    INSERT INTO [dbo].[Student]
    ([Id],[UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 2, 202110001, N'sara.ahmed@university.edu',      N'+96281394557', 1, 1),
        (2, 3, 202220002, N'omar.khaled@university.edu',     N'+962791001002', 1, 1),
        (3, 4, 202330003, N'lina.hassan@university.edu',     N'+962791001003', 1, 1),
        (4, 5, 202210004, N'yousef.nasser@university.edu',   N'+962791001004', 1, 1),
        (5, 6, 202320005, N'inactive.student@university.edu', N'+962791001005', 1, 1);
    SET IDENTITY_INSERT [dbo].[Student] OFF;
    -- NOTE: User 1 (admin) and User 7 (orphan.user) intentionally have NO Student row.

    -- ---- 5c. Courses -----------------------------------------------------
    SET IDENTITY_INSERT [dbo].[Course] ON;
    INSERT INTO [dbo].[Course]
    ([Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, N'CS101',   N'Introduction to Programming', CAST(3.00 AS DECIMAL(4,2)), N'Fundamentals of programming using C#: variables, control flow, methods and OOP basics.', 1, 1, 1),
        (2, N'MATH201', N'Calculus II',                 CAST(4.00 AS DECIMAL(4,2)), N'Integration techniques, sequences, series and applications.',                              1, 1, 1),
        (3, N'ENG101',  N'English Composition',         CAST(2.00 AS DECIMAL(4,2)), N'Academic writing, essay structure and research skills.',                                   1, 1, 1),
        (4, N'TEST404', N'Inactive Test Course',        CAST(1.00 AS DECIMAL(4,2)), N'IsActive=0 course for testing inactive-course paths. Safe to activate in tests.',          0, 1, 1),
        (5, N'TMP999',  N'Temp Deletable Course',       CAST(1.00 AS DECIMAL(4,2)), N'Active course with NO classes. Safe target for DeleteCourse / duplicate-code tests.',      1, 1, 1);
    SET IDENTITY_INSERT [dbo].[Course] OFF;

    -- ---- 5d. Classes -----------------------------------------------------
    -- Schedule = DaysOfWeek flags: Mon+Wed=20, Tue+Thu=40, Sun+Tue+Thu=42, Mon..Fri=124.
    -- CurrentCapacity below already equals COUNT(Registrations WHERE Status <> 'Dropped'),
    -- so the 5f sync is idempotent and the FULL / ONE-LEFT / EMPTY states read correctly.
    SET IDENTITY_INSERT [dbo].[Class] ON;
    INSERT INTO [dbo].[Class]
    ([Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
     [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 1, N'CS101-A',         N'Dr. Ahmad Jaber',   30,  2, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 20,  1, 1, 1),
        (2, 1, N'CS101-B',         N'Eng. Rania Odeh',   25,  1, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 40,  1, 1, 1),
        (3, 2, N'MATH201-A',       N'Dr. Khalil Nasser', 35,  2, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 42,  1, 1, 1),
        (4, 3, N'ENG101-A',        N'Ms. Dana Haddad',   30,  1, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 124, 1, 1, 1),
        (5, 1, N'CS101-FULL',      N'Dr. Test Full',      2,  2, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 20,  1, 1, 1),
        (6, 1, N'CS101-ONELEFT',   N'Dr. Test OneLeft',   3,  2, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 40,  1, 1, 1),
        (7, 2, N'MATH201-ARCHIVE', N'Dr. Inactive',      20,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 42,  0, 1, 1),
        (8, 3, N'ENG101-EMPTY',    N'Ms. Empty',         20,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 124, 1, 1, 1);
    SET IDENTITY_INSERT [dbo].[Class] OFF;
    -- Class 7 is INACTIVE (register -> "no longer active"). Class 8 is EMPTY (safe delete).

    -- ---- 5e. Registrations -----------------------------------------------
    -- Statuses: Registered (counts), Pending (counts, Id 4), Dropped (excluded, Id 11).
    SET IDENTITY_INSERT [dbo].[Registrations] ON;
    INSERT INTO [dbo].[Registrations]
    ([Id],[StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (1,  1, 1, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (2,  1, 3, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (3,  2, 1, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (4,  2, 4, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Pending',    1, 1),
        (5,  3, 2, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (6,  4, 3, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (7,  3, 5, CAST(N'2026-09-08T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (8,  4, 5, CAST(N'2026-09-08T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (9,  1, 6, CAST(N'2026-09-09T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (10, 2, 6, CAST(N'2026-09-09T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (11, 4, 2, CAST(N'2026-09-10T00:00:00' AS DATETIME2(7)), N'Dropped',    1, 1);
    SET IDENTITY_INSERT [dbo].[Registrations] OFF;
    -- Student 5 has ZERO registrations (empty-list test). (Student 1, Class 1) = Id 1 (duplicate test).

    -- ---- 5f. Sync Class.CurrentCapacity (zero-safe, idempotent) ----------
    UPDATE c
    SET [CurrentCapacity] = ISNULL(counts.RegCount, 0),
        [ModifiedOn]      = SYSDATETIMEOFFSET()
    FROM [dbo].[Class] c
             LEFT JOIN (
        SELECT [ClassId], COUNT(*) AS RegCount
        FROM [dbo].[Registrations]
        WHERE [Status] <> N'Dropped'
        GROUP BY [ClassId]
    ) AS counts ON counts.[ClassId] = c.[Id];

    COMMIT TRANSACTION SeedDemoData;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION SeedDemoData;

    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[User]'),          'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[User]          OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Student]'),       'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Student]       OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Course]'),        'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Course]        OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Class]'),         'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Class]         OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Registrations]'), 'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Registrations] OFF;

    THROW;
END CATCH;
GO

-- ============================================================================
-- SECTION 6: VERIFICATION
-- ============================================================================

SELECT N'User'          AS [Table], COUNT(*) AS [Rows] FROM [dbo].[User]
UNION ALL SELECT N'Student',       COUNT(*) FROM [dbo].[Student]
UNION ALL SELECT N'Course',        COUNT(*) FROM [dbo].[Course]
UNION ALL SELECT N'Class',         COUNT(*) FROM [dbo].[Class]
UNION ALL SELECT N'Registrations', COUNT(*) FROM [dbo].[Registrations]
UNION ALL SELECT N'RefreshTokens', COUNT(*) FROM [dbo].[RefreshTokens];
GO

-- Capacity sanity check
SELECT [Id], [ClassName], [MaxCapacity], [CurrentCapacity],
       CASE WHEN [CurrentCapacity] >= [MaxCapacity] THEN N'FULL'
            WHEN [CurrentCapacity]  = [MaxCapacity] - 1 THEN N'ONE SEAT LEFT'
            ELSE N'OPEN' END AS [Status]
FROM [dbo].[Class]
ORDER BY [Id];
GO

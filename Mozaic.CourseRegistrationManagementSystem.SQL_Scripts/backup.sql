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
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassByName];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetClassesByCourseId];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchClasses];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateCourse];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllCourses];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetCourseById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetCourseByCode];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateCourse];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteCourse];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchCourses];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllUsers];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetUserById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetUserByUserName];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteUser];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchUsers];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateStudent];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllStudents];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentByUserId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentProfileByUserId];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateStudent];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteStudent];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchStudents];

DROP PROCEDURE IF EXISTS [dbo].[usp_CreateRegistration];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllRegistrations];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetAllRegistrationsDetailed];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationById];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationByStudentAndClass];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationsByClassId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetRegistrationsByStudentId];
DROP PROCEDURE IF EXISTS [dbo].[usp_GetStudentRegistrationsWithClasses];
DROP PROCEDURE IF EXISTS [dbo].[usp_UpdateRegistration];
DROP PROCEDURE IF EXISTS [dbo].[usp_DeleteRegistration];
DROP PROCEDURE IF EXISTS [dbo].[usp_RegistrationExists];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchRegistrations];
DROP PROCEDURE IF EXISTS [dbo].[usp_SearchRegistrationsDetailed];

DROP PROCEDURE IF EXISTS [dbo].[usp_InsertBusinessException];
DROP PROCEDURE IF EXISTS [dbo].[usp_InsertDatabaseException];
DROP PROCEDURE IF EXISTS [dbo].[usp_InsertViewError];
GO

-- 0.2 Drop tables (children first, then parents)
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Course_CourseCode] UNIQUE NONCLUSTERED 
(
	[CourseCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Registrations_Student_Class] UNIQUE NONCLUSTERED 
(
	[StudentId] ASC,
	[ClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
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

CREATE PROCEDURE [dbo].[usp_DeleteClass]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Class] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

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

    IF EXISTS (SELECT 1 FROM [dbo].[Course] WHERE [CourseCode] = @CourseCode)
        THROW 50002, 'CourseCode already exists.', 1;

    INSERT INTO [dbo].[Course]
        ([CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (@CourseCode,@CourseName,@CreditHours,@Description,@IsActive,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

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
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Course] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

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
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[User] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

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
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Student] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

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
               WHERE [StudentId] = @StudentId AND [ClassId] = @ClassId)
        THROW 50005, 'Student is already registered for this class.', 1;

    INSERT INTO [dbo].[Registrations]
        ([StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (@StudentId,@ClassId,@RegistrationDate,@Status,@CreatedBy,@CreatedBy);

    SET @NewId = SCOPE_IDENTITY();
END
GO

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

CREATE PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Registrations] WHERE [Id] = @Id;
    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

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
       OR u.[FullName]     LIKE '%' + @regex + '%'
       OR c.[ClassName]    LIKE '%' + @regex + '%'
       OR co.[CourseName]  LIKE '%' + @regex + '%'
       OR r.[Status]       LIKE '%' + @regex + '%';
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
-- SECTION 5: SEED DATA
-- ============================================================================

BEGIN TRY
    BEGIN TRANSACTION SeedDemoData;

    -- ---- 5a. Users -------------------------------------------------------
    SET IDENTITY_INSERT [dbo].[User] ON;
    INSERT INTO [dbo].[User]
        ([Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, N'admin',         N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'System Administrator', 0, 1, 0, 0),
        (2, N'sara.ahmed',    N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Sara Ahmed',           1, 1, 0, 0),
        (3, N'omar.khaled',   N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Omar Khaled',          1, 1, 0, 0),
        (4, N'lina.hassan',   N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Lina Hassan',          1, 1, 0, 0),
        (5, N'yousef.nasser', N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Yousef Nasser',        1, 1, 0, 0);
    SET IDENTITY_INSERT [dbo].[User] OFF;

    -- ---- 5b. Students ----------------------------------------------------
    SET IDENTITY_INSERT [dbo].[Student] ON;
    INSERT INTO [dbo].[Student]
        ([Id],[UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 2, 1001, N'sara.ahmed@university.edu',    N'+962-79-100-1001', 1, 1),
        (2, 3, 1002, N'omar.khaled@university.edu',   N'+962-79-100-1002', 1, 1),
        (3, 4, 1003, N'lina.hassan@university.edu',   N'+962-79-100-1003', 1, 1),
        (4, 5, 1004, N'yousef.nasser@university.edu', N'+962-79-100-1004', 1, 1);
    SET IDENTITY_INSERT [dbo].[Student] OFF;

    -- ---- 5c. Courses -----------------------------------------------------
    SET IDENTITY_INSERT [dbo].[Course] ON;
    INSERT INTO [dbo].[Course]
        ([Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, N'CS101',   N'Introduction to Programming', CAST(3.00 AS DECIMAL(4,2)), N'Fundamentals of programming using C#: variables, control flow, methods and OOP basics.', 1, 1, 1),
        (2, N'MATH201', N'Calculus II',                 CAST(4.00 AS DECIMAL(4,2)), N'Integration techniques, sequences, series and applications.',                              1, 1, 1),
        (3, N'ENG101',  N'English Composition',         CAST(2.00 AS DECIMAL(4,2)), N'Academic writing, essay structure and research skills.',                                   1, 1, 1);
    SET IDENTITY_INSERT [dbo].[Course] OFF;

    -- ---- 5d. Classes -----------------------------------------------------
    -- Schedule = DaysOfWeek flags: Mon+Wed=20, Tue+Thu=40, Sun+Tue+Thu=42, Mon..Fri=124
    SET IDENTITY_INSERT [dbo].[Class] ON;
    INSERT INTO [dbo].[Class]
        ([Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
         [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 1, N'CS101-A',   N'Dr. Ahmad Jaber',  30, 30, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 20,  1, 1, 1),
        (2, 1, N'CS101-B',   N'Eng. Rania Odeh',  25, 24, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 40,  1, 1, 1),
        (3, 2, N'MATH201-A', N'Dr. Khalil Nasser',35,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 42,  1, 1, 1),
        (4, 3, N'ENG101-A',  N'Ms. Dana Haddad',  30,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 124, 1, 1, 1);
    SET IDENTITY_INSERT [dbo].[Class] OFF;

    -- ---- 5e. Registrations -----------------------------------------------
    SET IDENTITY_INSERT [dbo].[Registrations] ON;
    INSERT INTO [dbo].[Registrations]
        ([Id],[StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 1, 1, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (2, 1, 3, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (3, 2, 1, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (4, 2, 4, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Pending',    1, 1),
        (5, 3, 2, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (6, 4, 3, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1);
    SET IDENTITY_INSERT [dbo].[Registrations] OFF;

    -- ---- 5f. Sync Class.CurrentCapacity (zero-safe) ----------------------
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
UNION ALL SELECT N'Registrations', COUNT(*) FROM [dbo].[Registrations];
GO

-- Capacity sanity check
SELECT [Id], [ClassName], [MaxCapacity], [CurrentCapacity],
       CASE WHEN [CurrentCapacity] >= [MaxCapacity] THEN N'FULL'
            WHEN [CurrentCapacity]  = [MaxCapacity] - 1 THEN N'ONE SEAT LEFT'
            ELSE N'OPEN' END AS [Status]
FROM [dbo].[Class]
ORDER BY [Id];
GO
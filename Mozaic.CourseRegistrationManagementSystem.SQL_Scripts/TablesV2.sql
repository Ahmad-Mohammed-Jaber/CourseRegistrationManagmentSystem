-- ============================================================================
-- TablesV2.sql — Fresh-rebuild schema matching the current C# entities
--   Shared/Entities/User.cs, Student.cs, Course.cs, Class.cs, Registration.cs
--
-- WHAT CHANGED vs the old Tables.sql (2026-08-03):
--   1. All PKs/FKs: UNIQUEIDENTIFIER  -->  INT IDENTITY(1,1)
--   2. Added audit columns to ALL 5 tables:
--      CreatedOn / ModifiedOn DATETIMEOFFSET, CreatedBy / ModifiedBy INT
--   3. Student.FullName REMOVED (transient; comes via JOIN to [User].FullName)
--   4. Course.CourseCode widened NVARCHAR(6) --> NVARCHAR(20)
--      FOLLOW-UP REQUIRED: widen @CourseCode NVARCHAR(6) in usp_CreateCourse /
--      usp_UpdateCourse and SqlDbType.NVarChar,6 in CourseDataProvider.cs
--      (4 call sites) to 20.
--   5. Student.StudentNumber INT NULL --> INT NOT NULL UNIQUE
--   6. Registrations gets UNIQUE(StudentId, ClassId)
--   7. Student.Phone widened NVARCHAR(15) --> NVARCHAR(20)
--      (seed values like '+962-79-100-1001' are 16 chars and were being
--      truncated with Msg 2628. FOLLOW-UP: check Student.cs [MaxLength]
--      and StudentDataProvider.cs SqlDbType.NVarChar,15 -> 20.)
--   8. Seed Class rows now demonstrate capacity edge cases:
--        CS101-A -> CurrentCapacity = MaxCapacity      (30/30, FULL)
--        CS101-B -> CurrentCapacity = MaxCapacity - 1  (24/25, one seat left)
--      See section 6f note: the sync UPDATE will overwrite these unless
--      matching registrations exist or the sync is skipped.
--
-- COLUMN NOTE: Registration.RegsitrationDate (entity typo) maps to
-- Registrations.RegistrationDate (correct spelling).
--
-- WARNING: this is a FRESH REBUILD script — it DROPs and recreates the
-- tables. Existing data is lost. Use only on dev / disposable DBs.
-- Run order: 1) TablesV2.sql  2) StoredProcedures.sql
-- ============================================================================
USE [CourseManagementDB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- 0. Drop existing tables (reverse dependency order)
-- ----------------------------------------------------------------------------
DROP TABLE IF EXISTS [dbo].[Registrations];
GO
DROP TABLE IF EXISTS [dbo].[Class];
GO
DROP TABLE IF EXISTS [dbo].[Student];
GO
DROP TABLE IF EXISTS [dbo].[Course];
GO
DROP TABLE IF EXISTS [dbo].[User];
GO

-- ----------------------------------------------------------------------------
-- 1. [User]
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[User](
    [Id]           INT IDENTITY(1,1) NOT NULL,
    [UserName]     NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [FullName]     NVARCHAR(100) NOT NULL,
    [Role]         INT NOT NULL,             -- 0 = Admin, 1 = Student
    [IsActive]     BIT NOT NULL CONSTRAINT [DF_User_IsActive] DEFAULT (1),
    [CreatedOn]    DATETIMEOFFSET NOT NULL CONSTRAINT [DF_User_CreatedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [ModifiedOn]   DATETIMEOFFSET NOT NULL CONSTRAINT [DF_User_ModifiedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [CreatedBy]    INT NOT NULL CONSTRAINT [DF_User_CreatedBy] DEFAULT (0),
    [ModifiedBy]   INT NOT NULL CONSTRAINT [DF_User_ModifiedBy] DEFAULT (0),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_User_UserName] UNIQUE ([UserName])
) ON [PRIMARY];
GO

-- ----------------------------------------------------------------------------
-- 2. Course
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Course](
    [Id]          INT IDENTITY(1,1) NOT NULL,
    [CourseCode]  NVARCHAR(20) NOT NULL,
    [CourseName]  NVARCHAR(100) NOT NULL,
    [CreditHours] DECIMAL(4, 2) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL CONSTRAINT [DF_Course_Description] DEFAULT (N''),
    [IsActive]    BIT NOT NULL CONSTRAINT [DF_Course_IsActive] DEFAULT (1),
    [CreatedOn]   DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Course_CreatedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [ModifiedOn]  DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Course_ModifiedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [CreatedBy]   INT NOT NULL CONSTRAINT [DF_Course_CreatedBy] DEFAULT (0),
    [ModifiedBy]  INT NOT NULL CONSTRAINT [DF_Course_ModifiedBy] DEFAULT (0),
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Course_CourseCode] UNIQUE ([CourseCode])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

-- ----------------------------------------------------------------------------
-- 3. Student
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Student](
    [Id]            INT IDENTITY(1,1) NOT NULL,
    [UserId]        INT NOT NULL,
    [StudentNumber] INT NOT NULL,
    [Email]         NVARCHAR(100) NOT NULL,
    [Phone]         NVARCHAR(20) NOT NULL CONSTRAINT [DF_Student_Phone] DEFAULT (N''),
    [CreatedOn]     DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Student_CreatedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [ModifiedOn]    DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Student_ModifiedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [CreatedBy]     INT NOT NULL CONSTRAINT [DF_Student_CreatedBy] DEFAULT (0),
    [ModifiedBy]    INT NOT NULL CONSTRAINT [DF_Student_ModifiedBy] DEFAULT (0),
    CONSTRAINT [PK_Students_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Student_StudentNumber] UNIQUE ([StudentNumber]),
    CONSTRAINT [UQ_Student_UserId] UNIQUE ([UserId]),
    CONSTRAINT [FK_Students_User] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
) ON [PRIMARY];
GO

-- ----------------------------------------------------------------------------
-- 4. Class
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Class](
    [Id]              INT IDENTITY(1,1) NOT NULL,
    [CourseId]        INT NOT NULL,
    [ClassName]       NVARCHAR(50) NOT NULL,
    [Instructor]      NVARCHAR(50) NOT NULL,
    [MaxCapacity]     INT NOT NULL,
    [CurrentCapacity] INT NOT NULL CONSTRAINT [DF_Class_CurrentCapacity] DEFAULT (0),
    [StartDate]       DATETIME2(7) NOT NULL,
    [EndDate]         DATETIME2(7) NOT NULL,
    [Schedule]        INT NOT NULL CONSTRAINT [DF_Class_Schedule] DEFAULT (0),
    [IsActive]        BIT NOT NULL CONSTRAINT [DF_Class_IsActive] DEFAULT (1),
    [CreatedOn]       DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Class_CreatedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [ModifiedOn]      DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Class_ModifiedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [CreatedBy]       INT NOT NULL CONSTRAINT [DF_Class_CreatedBy] DEFAULT (0),
    [ModifiedBy]      INT NOT NULL CONSTRAINT [DF_Class_ModifiedBy] DEFAULT (0),
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Course_Class] FOREIGN KEY ([CourseId])
        REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
) ON [PRIMARY];
GO

-- ----------------------------------------------------------------------------
-- 5. Registrations
-- ----------------------------------------------------------------------------
CREATE TABLE [dbo].[Registrations](
    [Id]               INT IDENTITY(1,1) NOT NULL,
    [StudentId]        INT NOT NULL,
    [ClassId]          INT NOT NULL,
    [RegistrationDate] DATETIME2(7) NOT NULL CONSTRAINT [DF_Registrations_Date] DEFAULT (SYSUTCDATETIME()),
    [Status]           NVARCHAR(50) NOT NULL CONSTRAINT [DF_Registrations_Status] DEFAULT (N'Registered'),
    [CreatedOn]        DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Registrations_CreatedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [ModifiedOn]       DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Registrations_ModifiedOn] DEFAULT (SYSDATETIMEOFFSET()),
    [CreatedBy]        INT NOT NULL CONSTRAINT [DF_Registrations_CreatedBy] DEFAULT (0),
    [ModifiedBy]       INT NOT NULL CONSTRAINT [DF_Registrations_ModifiedBy] DEFAULT (0),
    CONSTRAINT [PK_Registrations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Registrations_Student_Class] UNIQUE ([StudentId], [ClassId]),
    CONSTRAINT [FK_Registrations_Students] FOREIGN KEY ([StudentId])
        REFERENCES [dbo].[Student] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Registrations_Class] FOREIGN KEY ([ClassId])
        REFERENCES [dbo].[Class] ([Id]) ON DELETE CASCADE
) ON [PRIMARY];
GO

-- ============================================================================
-- 6. SEED DATA
--    DESTRUCTIVE REBUILD is above; this seed section is idempotent by itself
--    (INSERT ... WHERE NOT EXISTS), so it is safe to re-run on its own.
--    PasswordHash for ALL seed users:
--      $2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6
--    NOTE: all five accounts share ONE hash -> same password. For demo only.
-- ============================================================================
SET XACT_ABORT ON;
SET NOCOUNT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION SeedDemoData;

    -- ---- 6a. Users -------------------------------------------------------
    SET IDENTITY_INSERT [dbo].[User] ON;
    INSERT INTO [dbo].[User]
        ([Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],[CreatedBy],[ModifiedBy])
    SELECT v.[Id], v.[UserName], v.[PasswordHash], v.[FullName],
           v.[Role], v.[IsActive], v.[CreatedBy], v.[ModifiedBy]
    FROM (VALUES
        (1, N'admin',         N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'System Administrator', 0, 1, 0, 0),
        (2, N'sara.ahmed',    N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Sara Ahmed',           1, 1, 0, 0),
        (3, N'omar.khaled',   N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Omar Khaled',          1, 1, 0, 0),
        (4, N'lina.hassan',   N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Lina Hassan',          1, 1, 0, 0),
        (5, N'yousef.nasser', N'$2a$11$u0tQ2TtBT2Z7AZybsVXR0eA7yJKOCPsMj6.aZcqxd1Mre.y.RZzD6', N'Yousef Nasser',        1, 1, 0, 0)
    ) AS v([Id],[UserName],[PasswordHash],[FullName],[Role],[IsActive],[CreatedBy],[ModifiedBy])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[User] u WHERE u.[Id] = v.[Id]);
    SET IDENTITY_INSERT [dbo].[User] OFF;

    -- ---- 6b. Students ----------------------------------------------------
    SET IDENTITY_INSERT [dbo].[Student] ON;
    INSERT INTO [dbo].[Student]
        ([Id],[UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    SELECT v.[Id], v.[UserId], v.[StudentNumber], v.[Email], v.[Phone],
           v.[CreatedBy], v.[ModifiedBy]
    FROM (VALUES
        (1, 2, 1001, N'sara.ahmed@university.edu',    N'+962-79-100-1001', 1, 1),
        (2, 3, 1002, N'omar.khaled@university.edu',   N'+962-79-100-1002', 1, 1),
        (3, 4, 1003, N'lina.hassan@university.edu',   N'+962-79-100-1003', 1, 1),
        (4, 5, 1004, N'yousef.nasser@university.edu', N'+962-79-100-1004', 1, 1)
    ) AS v([Id],[UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Student] s WHERE s.[Id] = v.[Id]);
    SET IDENTITY_INSERT [dbo].[Student] OFF;

    -- ---- 6c. Courses -----------------------------------------------------
    SET IDENTITY_INSERT [dbo].[Course] ON;
    INSERT INTO [dbo].[Course]
        ([Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    SELECT v.[Id], v.[CourseCode], v.[CourseName], v.[CreditHours],
           v.[Description], v.[IsActive], v.[CreatedBy], v.[ModifiedBy]
    FROM (VALUES
        (1, N'CS101',   N'Introduction to Programming', CAST(3.00 AS DECIMAL(4,2)), N'Fundamentals of programming using C#: variables, control flow, methods and OOP basics.', 1, 1, 1),
        (2, N'MATH201', N'Calculus II',                 CAST(4.00 AS DECIMAL(4,2)), N'Integration techniques, sequences, series and applications.',                              1, 1, 1),
        (3, N'ENG101',  N'English Composition',         CAST(2.00 AS DECIMAL(4,2)), N'Academic writing, essay structure and research skills.',                                   1, 1, 1)
    ) AS v([Id],[CourseCode],[CourseName],[CreditHours],[Description],[IsActive],[CreatedBy],[ModifiedBy])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Course] c WHERE c.[Id] = v.[Id]);
    SET IDENTITY_INSERT [dbo].[Course] OFF;

    -- ---- 6d. Classes -----------------------------------------------------
    -- Schedule = DaysOfWeek flags: Mon+Wed=20, Tue+Thu=40, Sun+Tue+Thu=42, Mon..Fri=124
    -- Capacity edge cases:
    --   Id 1 (CS101-A): 30/30  -> FULL
    --   Id 2 (CS101-B): 24/25  -> one seat left
    SET IDENTITY_INSERT [dbo].[Class] ON;
    INSERT INTO [dbo].[Class]
        ([Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
         [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    SELECT v.[Id], v.[CourseId], v.[ClassName], v.[Instructor], v.[MaxCapacity],
           v.[CurrentCapacity], v.[StartDate], v.[EndDate], v.[Schedule],
           v.[IsActive], v.[CreatedBy], v.[ModifiedBy]
    FROM (VALUES
        (1, 1, N'CS101-A',   N'Dr. Ahmad Jaber',  30, 30, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 20,  1, 1, 1),
        (2, 1, N'CS101-B',   N'Eng. Rania Odeh',  25, 24, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 40,  1, 1, 1),
        (3, 2, N'MATH201-A', N'Dr. Khalil Nasser',35,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 42,  1, 1, 1),
        (4, 3, N'ENG101-A',  N'Ms. Dana Haddad',  30,  0, CAST(N'2026-09-01T00:00:00' AS DATETIME2(7)), CAST(N'2026-12-20T00:00:00' AS DATETIME2(7)), 124, 1, 1, 1)
    ) AS v([Id],[CourseId],[ClassName],[Instructor],[MaxCapacity],[CurrentCapacity],
           [StartDate],[EndDate],[Schedule],[IsActive],[CreatedBy],[ModifiedBy])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Class] c WHERE c.[Id] = v.[Id]);
    SET IDENTITY_INSERT [dbo].[Class] OFF;

    -- ---- 6e. Registrations ----------------------------------------------
    SET IDENTITY_INSERT [dbo].[Registrations] ON;
    INSERT INTO [dbo].[Registrations]
        ([Id],[StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    SELECT v.[Id], v.[StudentId], v.[ClassId], v.[RegistrationDate],
           v.[Status], v.[CreatedBy], v.[ModifiedBy]
    FROM (VALUES
        (1, 1, 1, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (2, 1, 3, CAST(N'2026-09-05T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (3, 2, 1, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (4, 2, 4, CAST(N'2026-09-06T00:00:00' AS DATETIME2(7)), N'Pending',    1, 1),
        (5, 3, 2, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1),
        (6, 4, 3, CAST(N'2026-09-07T00:00:00' AS DATETIME2(7)), N'Registered', 1, 1)
    ) AS v([Id],[StudentId],[ClassId],[RegistrationDate],[Status],[CreatedBy],[ModifiedBy])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Registrations] r WHERE r.[Id] = v.[Id]);
    SET IDENTITY_INSERT [dbo].[Registrations] OFF;

    -- ---- 6f. Sync Class.CurrentCapacity (zero-safe) ---------------------
    -- NOTE: this OVERWRITES the seeded CurrentCapacity values from 6d with
    -- the actual registration counts. So CS101-A will end up at 2, not 30,
    -- and CS101-B at 1, not 24. If you want the seeded 30/30 and 24/25 to
    -- survive, either comment this block out, or seed enough Registrations
    -- to match. See message below the script.
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

-- ----------------------------------------------------------------------------
-- 7. Seed verification
-- ----------------------------------------------------------------------------
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
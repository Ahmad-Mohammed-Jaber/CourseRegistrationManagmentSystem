-- ============================================================================
-- Reset seed data: wipe demo tables and re-insert the exact seed rows from
-- backup.sql (Section 5), restoring predictable Ids for tests.
--
-- Run against CourseManagementDB whenever the demo data gets dirty.
-- Idempotent: safe to re-run (it always deletes first, in one transaction).
-- Leaves RefreshTokens and exception-log tables alone, EXCEPT RefreshTokens
-- rows are cascade-deleted via FK_RefreshTokens_User when Users are wiped.
-- backup.sql is intentionally left untouched.
-- ============================================================================

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

BEGIN TRY
    BEGIN TRANSACTION ResetSeedData;

    -- ---- 1. Wipe (children first; FKs are ON DELETE CASCADE regardless) --
    DELETE FROM [dbo].[Registrations];
    DELETE FROM [dbo].[RefreshTokens];
    DELETE FROM [dbo].[Class];
    DELETE FROM [dbo].[Student];
    DELETE FROM [dbo].[Course];
    DELETE FROM [dbo].[User];

    DBCC CHECKIDENT ([dbo].[Registrations], RESEED, 0);
    DBCC CHECKIDENT ([dbo].[RefreshTokens],  RESEED, 0);
    DBCC CHECKIDENT ([dbo].[Class],          RESEED, 0);
    DBCC CHECKIDENT ([dbo].[Student],        RESEED, 0);
    DBCC CHECKIDENT ([dbo].[Course],         RESEED, 0);
    DBCC CHECKIDENT ([dbo].[User],           RESEED, 0);

    -- ---- 2a. Users -------------------------------------------------------
    -- 1=admin (Role 0, no student row) | 2-5=active students | 6=INACTIVE |
    -- 7=orphan.user (active, NO student row -> "profile not found" test).
    -- All share one bcrypt hash; every login uses the same known password.
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

    -- ---- 2b. Students ----------------------------------------------------
    -- 1-4=active | 5=inactive user's student, ZERO registrations (empty-list test).
    -- NOTE: User 1 (admin) and User 7 (orphan.user) intentionally have NO Student row.
    SET IDENTITY_INSERT [dbo].[Student] ON;
    INSERT INTO [dbo].[Student]
        ([Id],[UserId],[StudentNumber],[Email],[Phone],[CreatedBy],[ModifiedBy])
    VALUES
        (1, 2, 202110001, N'sara.ahmed@university.edu',       N'+96281394557', 1, 1),
        (2, 3, 202220002, N'omar.khaled@university.edu',      N'+962791001002', 1, 1),
        (3, 4, 202330003, N'lina.hassan@university.edu',      N'+962791001003', 1, 1),
        (4, 5, 202210004, N'yousef.nasser@university.edu',    N'+962791001004', 1, 1),
        (5, 6, 202320005, N'inactive.student@university.edu', N'+962791001005', 1, 1);
    SET IDENTITY_INSERT [dbo].[Student] OFF;

    -- ---- 2c. Courses -----------------------------------------------------
    -- 1-3=active core | 4=TEST404 INACTIVE | 5=TMP999 active, NO classes
    -- (safe delete + unique-code conflict test).
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

    -- ---- 2d. Classes -----------------------------------------------------
    -- 1-4=OPEN | 5=FULL 2/2 | 6=ONELEFT 2/3 | 7=INACTIVE archive | 8=EMPTY 0/20.
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

    -- ---- 2e. Registrations -----------------------------------------------
    -- Registered x8 | Pending x1 (Id 4) | Dropped x1 (Id 11, excluded from capacity).
    -- (Student 1, Class 1) = Id 1 (duplicate test). Student 5 has ZERO registrations.
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

    -- ---- 2f. Sync Class.CurrentCapacity (zero-safe, idempotent) ----------
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

    -- ---- 2g. Fix identity counters to the reseeded Ids -------------------
    DBCC CHECKIDENT ([dbo].[User],          RESEED, 7);
    DBCC CHECKIDENT ([dbo].[Student],       RESEED, 5);
    DBCC CHECKIDENT ([dbo].[Course],        RESEED, 5);
    DBCC CHECKIDENT ([dbo].[Class],         RESEED, 8);
    DBCC CHECKIDENT ([dbo].[Registrations], RESEED, 11);

    COMMIT TRANSACTION ResetSeedData;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION ResetSeedData;

    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[User]'),          'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[User]          OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Student]'),       'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Student]       OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Course]'),        'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Course]        OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Class]'),         'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Class]         OFF;
    IF OBJECTPROPERTY(OBJECT_ID(N'[dbo].[Registrations]'), 'TableHasIdentity') = 1 SET IDENTITY_INSERT [dbo].[Registrations] OFF;

    THROW;
END CATCH;
GO

-- ============================================================================
-- VERIFICATION (expected: 7 / 5 / 5 / 8 / 11)
-- ============================================================================

SELECT N'User'          AS [Table], COUNT(*) AS [Rows] FROM [dbo].[User]
UNION ALL SELECT N'Student',       COUNT(*) FROM [dbo].[Student]
UNION ALL SELECT N'Course',        COUNT(*) FROM [dbo].[Course]
UNION ALL SELECT N'Class',         COUNT(*) FROM [dbo].[Class]
UNION ALL SELECT N'Registrations', COUNT(*) FROM [dbo].[Registrations];
GO

SELECT [Id], [ClassName], [MaxCapacity], [CurrentCapacity],
       CASE WHEN [CurrentCapacity] >= [MaxCapacity] THEN N'FULL'
            WHEN [CurrentCapacity]  = [MaxCapacity] - 1 THEN N'ONE SEAT LEFT'
            ELSE N'OPEN' END AS [Status]
FROM [dbo].[Class]
ORDER BY [Id];
GO

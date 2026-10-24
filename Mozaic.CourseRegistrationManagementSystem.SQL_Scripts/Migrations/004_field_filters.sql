-- Field-level filtering for every list procedure.
-- Applies the same procedure bodies as backup.sql, but as CREATE OR ALTER
-- so it can be run against an existing database without dropping it.

CREATE OR ALTER PROCEDURE [dbo].[usp_GetCourses]
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

CREATE OR ALTER PROCEDURE [dbo].[usp_GetStudents]
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

CREATE OR ALTER PROCEDURE [dbo].[usp_GetClasses]
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

CREATE OR ALTER PROCEDURE [dbo].[usp_GetRegistrations]
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

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUsers]
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


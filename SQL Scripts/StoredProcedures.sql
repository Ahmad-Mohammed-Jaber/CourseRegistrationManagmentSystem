USE [CourseManagementDB]
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
IF OBJECT_ID('dbo.usp_GetCourseById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetCourseById];
IF OBJECT_ID('dbo.usp_GetClassesByCourseId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetClassesByCourseId];
IF OBJECT_ID('dbo.usp_GetClassById', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetClassById];
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

-- =============================================
-- CREATE ALL STORED PROCEDURES
-- =============================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- CLASS Stored Procedures
-- =============================================

-- Create Class (returns generated ID)
CREATE PROCEDURE [dbo].[usp_CreateClass]
    @CourseId INT,
    @ClassName NVARCHAR(50),
    @Instructor NVARCHAR(50),
    @MaxCapacity INT,
    @CurrentCapacity INT,
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @Schedule INT,
    @IsActive BIT,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Class (CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive)
    VALUES (@CourseId, @ClassName, @Instructor, @MaxCapacity, @CurrentCapacity, @StartDate, @EndDate, @Schedule, @IsActive);
    
    SET @NewId = SCOPE_IDENTITY();
END
GO

-- =============================================
-- COURSE Stored Procedures
-- =============================================

-- Create Course (returns generated ID)
CREATE PROCEDURE [dbo].[usp_CreateCourse]
    @CourseCode NVARCHAR(20),
    @CourseName NVARCHAR(100),
    @CreditHours DECIMAL(4, 2),
    @Description NVARCHAR(MAX),
    @IsActive BIT,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Course (CourseCode, CourseName, CreditHours, Description, IsActive)
    VALUES (@CourseCode, @CourseName, @CreditHours, @Description, @IsActive);
    
    SET @NewId = SCOPE_IDENTITY();
END
GO

-- =============================================
-- REGISTRATION Stored Procedures
-- =============================================

-- Create Registration (returns generated ID)
CREATE PROCEDURE [dbo].[usp_CreateRegistration]
    @StudentId INT,
    @ClassId INT,
    @RegistrationDate DATETIME2,
    @Status NVARCHAR(50),
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Registrations (StudentId, ClassId, RegistrationDate, Status)
    VALUES (@StudentId, @ClassId, @RegistrationDate, @Status);
    
    SET @NewId = SCOPE_IDENTITY();
END
GO

-- =============================================
-- STUDENT Stored Procedures
-- =============================================

-- Create Student (returns generated ID)
-- NOTE: Student has no FullName column (TablesV2 removed it; identity lives in [User]).
-- @FullName is accepted for DAL backward-compat and ignored.
CREATE PROCEDURE [dbo].[usp_CreateStudent]
    @UserId INT,
    @StudentNumber INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Student (UserId, StudentNumber, Email, Phone)
    VALUES (@UserId, @StudentNumber, @Email, @Phone);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- =============================================
-- USER Stored Procedures
-- =============================================

-- Create User (returns generated ID)
CREATE PROCEDURE [dbo].[usp_CreateUser]
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Role INT,
    @IsActive BIT,
    @CreatedBy INT = 0,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [dbo].[User] WHERE [UserName] = @UserName)
        THROW 50009, 'UserName already exists.', 1;

    INSERT INTO [User] (UserName, PasswordHash, FullName, Role, IsActive, CreatedBy, ModifiedBy)
    VALUES (@UserName, @PasswordHash, @FullName, @Role, @IsActive, @CreatedBy, @CreatedBy);

    SET @Id = SCOPE_IDENTITY();
END
GO

-- Delete Class
CREATE PROCEDURE [dbo].[usp_DeleteClass]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Class WHERE Id = @Id;
END
GO

-- Delete Course
CREATE PROCEDURE [dbo].[usp_DeleteCourse]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Course WHERE Id = @Id;
END
GO

-- Delete Registration
CREATE PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Registrations WHERE Id = @Id;
END
GO

-- Delete Student
CREATE PROCEDURE [dbo].[usp_DeleteStudent]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Student WHERE Id = @Id;
END
GO

-- Delete User
CREATE PROCEDURE [dbo].[usp_DeleteUser]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [User] WHERE Id = @Id;
END
GO

-- Get All Classes
CREATE PROCEDURE [dbo].[usp_GetAllClasses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Class;
END
GO

-- Get All Courses
CREATE PROCEDURE [dbo].[usp_GetAllCourses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Course;
END
GO

-- Get All Registrations
CREATE PROCEDURE [dbo].[usp_GetAllRegistrations]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Registrations;
END
GO

-- Get All Registrations Detailed
-- NOTE: co.Id aliased as CourseId to avoid duplicate "Id" column (r.Id).
CREATE PROCEDURE [dbo].[usp_GetAllRegistrationsDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        u.UserName,
        r.ClassId,
        c.ClassName,
        co.Id AS CourseId,
        co.CourseName,
        r.RegistrationDate,
        r.Status
    FROM Registrations r
    INNER JOIN Student s ON r.StudentId = s.Id
    INNER JOIN [User] u ON s.UserId = u.Id
    INNER JOIN Class c ON r.ClassId = c.Id
    INNER JOIN Course co ON c.CourseId = co.Id;
END
GO

-- Get All Students
CREATE PROCEDURE [dbo].[usp_GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.UserId, s.StudentNumber, s.Email, s.Phone,
           u.UserName AS UserName,
           u.FullName AS FullName,
           u.Role AS Role,
           u.IsActive AS IsActive
    FROM Student s
    JOIN [User] u ON s.UserId = u.Id;
END
GO

-- Get All Users
CREATE PROCEDURE [dbo].[usp_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM [User];
END
GO

-- Get Class by ID
CREATE PROCEDURE [dbo].[usp_GetClassById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Class 
    WHERE Id = @Id;
END
GO

-- Get Classes by Course ID
CREATE PROCEDURE [dbo].[usp_GetClassesByCourseId]
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Class 
    WHERE CourseId = @CourseId;
END
GO

-- Get Course by ID
CREATE PROCEDURE [dbo].[usp_GetCourseById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Course 
    WHERE Id = @Id;
END
GO

-- Get Registration by ID
CREATE PROCEDURE [dbo].[usp_GetRegistrationById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Registrations 
    WHERE Id = @Id;
END
GO

-- Get Registrations by Class ID
CREATE PROCEDURE [dbo].[usp_GetRegistrationsByClassId]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Registrations 
    WHERE ClassId = @ClassId;
END
GO

-- Get Registrations by Student ID
CREATE PROCEDURE [dbo].[usp_GetRegistrationsByStudentId]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Registrations 
    WHERE StudentId = @StudentId;
END
GO

-- Get Student by ID
CREATE PROCEDURE [dbo].[usp_GetStudentById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.UserId, s.StudentNumber, s.Email, s.Phone,
           u.UserName AS UserName,
           u.FullName AS FullName,
           u.Role AS Role,
           u.IsActive AS IsActive
    FROM Student s
    JOIN [User] u ON s.UserId = u.Id
    WHERE s.Id = @Id;
END
GO

-- Get Student by User ID
CREATE PROCEDURE [dbo].[usp_GetStudentByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.UserId, s.StudentNumber, s.Email, s.Phone,
           u.UserName AS UserName,
           u.FullName AS FullName,
           u.Role AS Role,
           u.IsActive AS IsActive
    FROM Student s
    JOIN [User] u ON s.UserId = u.Id
    WHERE s.UserId = @UserId;
END
GO

-- Get Student Registrations with Classes
CREATE PROCEDURE [dbo].[usp_GetStudentRegistrationsWithClasses]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        r.ClassId,
        r.RegistrationDate,
        r.Status,
        c.Id AS Class_Id,
        c.CourseId,
        c.ClassName,
        c.Instructor,
        c.MaxCapacity,
        c.CurrentCapacity,
        c.StartDate,
        c.EndDate,
        c.Schedule,
        c.IsActive
    FROM Registrations r
    INNER JOIN Class c ON r.ClassId = c.Id
    WHERE r.StudentId = @StudentId;
END
GO

-- Get User by ID
CREATE PROCEDURE [dbo].[usp_GetUserById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM [User] 
    WHERE Id = @Id;
END
GO

-- Get User by Username
CREATE PROCEDURE [dbo].[usp_GetUserByUserName]
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM [User] 
    WHERE UserName = @UserName;
END
GO

-- Check if Registration Exists (result column aliased for safe mapping)
CREATE PROCEDURE [dbo].[usp_RegistrationExists]
    @StudentId INT,
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS [Exists]
    FROM Registrations 
    WHERE StudentId = @StudentId AND ClassId = @ClassId;
END
GO

-- Search Classes
CREATE PROCEDURE [dbo].[usp_SearchClasses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Class 
    WHERE ClassName LIKE '%' + @regex + '%' OR Instructor LIKE '%' + @regex + '%';
END
GO

-- Search Courses
CREATE PROCEDURE [dbo].[usp_SearchCourses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Course 
    WHERE CourseCode LIKE '%' + @regex + '%' OR CourseName LIKE '%' + @regex + '%';
END
GO

-- Search Registrations
CREATE PROCEDURE [dbo].[usp_SearchRegistrations]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM Registrations 
    WHERE Status LIKE '%' + @regex + '%';
END
GO

-- Search Registrations Detailed
-- FIX: Student.FullName was dropped in TablesV2; filter on [User].FullName.
-- FIX: co.Id aliased as CourseId to avoid duplicate "Id" column.
CREATE PROCEDURE [dbo].[usp_SearchRegistrationsDetailed]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        u.UserName,
        r.ClassId,
        c.ClassName,
        co.Id AS CourseId,
        co.CourseName,
        r.RegistrationDate,
        r.Status
    FROM Registrations r
    INNER JOIN Student s ON r.StudentId = s.Id
    INNER JOIN [User] u ON s.UserId = u.Id
    INNER JOIN Class c ON r.ClassId = c.Id
    INNER JOIN Course co ON c.CourseId = co.Id
    WHERE u.UserName LIKE '%' + @regex + '%'
       OR u.FullName LIKE '%' + @regex + '%'
       OR c.ClassName LIKE '%' + @regex + '%'
       OR co.CourseName LIKE '%' + @regex + '%'
       OR r.Status LIKE '%' + @regex + '%';
END
GO

-- Search Students
CREATE PROCEDURE [dbo].[usp_SearchStudents]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.UserId, s.StudentNumber, s.Email, s.Phone,
           u.UserName AS UserName,
           u.FullName AS FullName,
           u.Role AS Role,
           u.IsActive AS IsActive
    FROM Student s 
    JOIN [User] u ON s.UserId = u.Id 
    WHERE u.FullName LIKE '%' + @regex + '%' OR s.Email LIKE '%' + @regex + '%';
END
GO

-- Search Users
CREATE PROCEDURE [dbo].[usp_SearchUsers]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive, CreatedOn, ModifiedOn, CreatedBy, ModifiedBy
    FROM [User] 
    WHERE UserName LIKE '%' + @regex + '%' OR FullName LIKE '%' + @regex + '%';
END
GO

-- Update Class
CREATE PROCEDURE [dbo].[usp_UpdateClass]
    @Id INT,
    @CourseId INT,
    @ClassName NVARCHAR(50),
    @Instructor NVARCHAR(50),
    @MaxCapacity INT,
    @CurrentCapacity INT,
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @Schedule INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Class 
    SET CourseId = @CourseId, 
        ClassName = @ClassName, 
        Instructor = @Instructor, 
        MaxCapacity = @MaxCapacity, 
        CurrentCapacity = @CurrentCapacity, 
        StartDate = @StartDate, 
        EndDate = @EndDate, 
        Schedule = @Schedule, 
        IsActive = @IsActive 
    WHERE Id = @Id;
END
GO

-- Update Course
CREATE PROCEDURE [dbo].[usp_UpdateCourse]
    @Id INT,
    @CourseCode NVARCHAR(20),
    @CourseName NVARCHAR(100),
    @CreditHours DECIMAL(4, 2),
    @Description NVARCHAR(MAX),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Course 
    SET CourseCode = @CourseCode, 
        CourseName = @CourseName, 
        CreditHours = @CreditHours, 
        Description = @Description, 
        IsActive = @IsActive 
    WHERE Id = @Id;
END
GO

-- Update Registration
CREATE PROCEDURE [dbo].[usp_UpdateRegistration]
    @Id INT,
    @StudentId INT,
    @ClassId INT,
    @RegistrationDate DATETIME2,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Registrations 
    SET StudentId = @StudentId, 
        ClassId = @ClassId, 
        RegistrationDate = @RegistrationDate, 
        Status = @Status 
    WHERE Id = @Id;
END
GO

-- Update Student
-- NOTE: @FullName accepted for DAL backward-compat; propagated to [User].FullName when non-NULL.
CREATE PROCEDURE [dbo].[usp_UpdateStudent]
    @Id INT,
    @UserId INT,
    @StudentNumber INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Student 
    SET UserId = @UserId, 
        StudentNumber = @StudentNumber, 
        Email = @Email, 
        Phone = @Phone 
    WHERE Id = @Id;

    IF @FullName IS NOT NULL
        UPDATE [User]
        SET [FullName] = @FullName
        WHERE [Id] = @UserId;
END
GO

-- Update User
CREATE PROCEDURE [dbo].[usp_UpdateUser]
    @Id INT,
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Role INT,
    @IsActive BIT,
    @ModifiedBy INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [dbo].[User]
               WHERE [UserName] = @UserName AND [Id] <> @Id)
        THROW 50009, 'UserName already exists on another user.', 1;

    UPDATE [User]
    SET UserName = @UserName,
        PasswordHash = @PasswordHash,
        FullName = @FullName,
        Role = @Role,
        IsActive = @IsActive,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = SYSDATETIMEOFFSET()
    WHERE Id = @Id;
END
GO

-- Get Student Profile by UserId (single auth session: student data fetched fresh from DB)
-- Canonical columns: StudentId + UserId (provider falls back to legacy Id = StudentId).
CREATE PROCEDURE [dbo].[usp_GetStudentProfileByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.Id AS StudentId,
        u.Id AS UserId,
        s.Id AS Id,
        u.UserName,
        u.FullName,
        u.Role,
        s.StudentNumber,
        s.Email,
        s.Phone
    FROM [User] u
    INNER JOIN [Student] s ON s.UserId = u.Id
    WHERE u.Id = @UserId;
END
GO
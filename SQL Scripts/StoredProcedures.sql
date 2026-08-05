USE [CourseManagementDB]
GO

-- =============================================
-- User Stored Procedures
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[usp_CreateUser]
    @Id UNIQUEIDENTIFIER,
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Role INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [User] (Id, UserName, PasswordHash, FullName, Role, IsActive)
    VALUES (@Id, @UserName, @PasswordHash, @FullName, @Role, @IsActive);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUserById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User];
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateUser]
    @Id UNIQUEIDENTIFIER,
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Role INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [User] 
    SET UserName = @UserName, 
        PasswordHash = @PasswordHash, 
        FullName = @FullName, 
        Role = @Role, 
        IsActive = @IsActive 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteUser]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [User] WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchUsers]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE UserName LIKE '%' + @regex + '%' OR FullName LIKE '%' + @regex + '%';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUserByUserName]
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE UserName = @UserName;
END
GO


-- =============================================
-- Student Stored Procedures
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[usp_CreateStudent]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @StudentNumber INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Student (Id, UserId, StudentNumber, FullName, Email, Phone)
    VALUES (@Id, @UserId, @StudentNumber, ISNULL(@FullName, ''), @Email, @Phone);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetStudentById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, StudentNumber, Email, Phone 
    FROM Student 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetStudentByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, StudentNumber, Email, Phone 
    FROM Student 
    WHERE UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, StudentNumber, Email, Phone 
    FROM Student;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateStudent]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @StudentNumber INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Student 
    SET UserId = @UserId, 
        StudentNumber = @StudentNumber, 
        FullName = ISNULL(@FullName, FullName),
        Email = @Email, 
        Phone = @Phone 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteStudent]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Student WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchStudents]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.UserId, s.StudentNumber, s.Email, s.Phone 
    FROM Student s 
    JOIN [User] u ON s.UserId = u.Id 
    WHERE u.FullName LIKE '%' + @regex + '%' OR s.Email LIKE '%' + @regex + '%';
END
GO


-- =============================================
-- Course Stored Procedures
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[usp_CreateCourse]
    @Id UNIQUEIDENTIFIER,
    @CourseCode NVARCHAR(6),
    @CourseName NVARCHAR(100),
    @CreditHours DECIMAL(4, 2),
    @Description NVARCHAR(MAX),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Course (Id, CourseCode, CourseName, CreditHours, Description, IsActive)
    VALUES (@Id, @CourseCode, @CourseName, @CreditHours, @Description, @IsActive);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetCourseById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllCourses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateCourse]
    @Id UNIQUEIDENTIFIER,
    @CourseCode NVARCHAR(6),
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

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteCourse]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Course WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchCourses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course 
    WHERE CourseCode LIKE '%' + @regex + '%' OR CourseName LIKE '%' + @regex + '%';
END
GO


-- =============================================
-- Class Stored Procedures
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[usp_CreateClass]
    @Id UNIQUEIDENTIFIER,
    @CourseId UNIQUEIDENTIFIER,
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
    INSERT INTO Class (Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive)
    VALUES (@Id, @CourseId, @ClassName, @Instructor, @MaxCapacity, @CurrentCapacity, @StartDate, @EndDate, @Schedule, @IsActive);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetClassById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetClassesByCourseId]
    @CourseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE CourseId = @CourseId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllClasses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, CurrentCapacity, MaxCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateClass]
    @Id UNIQUEIDENTIFIER,
    @CourseId UNIQUEIDENTIFIER,
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

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteClass]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Class WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchClasses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE ClassName LIKE '%' + @regex + '%' OR Instructor LIKE '%' + @regex + '%';
END
GO


-- =============================================
-- Registration Stored Procedures
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[usp_CreateRegistration]
    @Id UNIQUEIDENTIFIER,
    @StudentId UNIQUEIDENTIFIER,
    @ClassId UNIQUEIDENTIFIER,
    @RegistrationDate DATETIME2,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Registrations (Id, StudentId, ClassId, RegistrationDate, Status)
    VALUES (@Id, @StudentId, @ClassId, @RegistrationDate, @Status);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetRegistrationById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllRegistrations]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetRegistrationsByStudentId]
    @StudentId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE StudentId = @StudentId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetRegistrationsByClassId]
    @ClassId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE ClassId = @ClassId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateRegistration]
    @Id UNIQUEIDENTIFIER,
    @StudentId UNIQUEIDENTIFIER,
    @ClassId UNIQUEIDENTIFIER,
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

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Registrations WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchRegistrations]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE Status LIKE '%' + @regex + '%';
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_RegistrationExists]
    @StudentId UNIQUEIDENTIFIER,
    @ClassId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 
    FROM Registrations 
    WHERE StudentId = @StudentId AND ClassId = @ClassId;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetStudentRegistrationsWithClasses]
    @StudentId UNIQUEIDENTIFIER
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

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllRegistrationsDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        s.FullName,
        r.ClassId,
        c.ClassName,
        co.CourseName,
        r.RegistrationDate,
        r.Status
    FROM Registrations r
    INNER JOIN Student s ON r.StudentId = s.Id
    INNER JOIN Class c ON r.ClassId = c.Id
    INNER JOIN Course co ON c.CourseId = co.Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_SearchRegistrationsDetailed]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        s.FullName,
        r.ClassId,
        c.ClassName,
        co.CourseName,
        r.RegistrationDate,
        r.Status
    FROM Registrations r
    INNER JOIN Student s ON r.StudentId = s.Id
    INNER JOIN Class c ON r.ClassId = c.Id
    INNER JOIN Course co ON c.CourseId = co.Id
    WHERE s.FullName LIKE '%' + @regex + '%'
       OR c.ClassName LIKE '%' + @regex + '%'
       OR co.CourseName LIKE '%' + @regex + '%'
       OR r.Status LIKE '%' + @regex + '%';
END
GO

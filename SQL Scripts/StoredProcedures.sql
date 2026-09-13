USE [CourseManagementDB]
GO
/****** Object:  StoredProcedure [dbo].[usp_CreateClass]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- CLASS Stored Procedures
-- =============================================

-- Create Class (returns generated ID)
CREATE   PROCEDURE [dbo].[usp_CreateClass]
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
/****** Object:  StoredProcedure [dbo].[usp_CreateCourse]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- COURSE Stored Procedures
-- =============================================

-- Create Course (returns generated ID)
CREATE   PROCEDURE [dbo].[usp_CreateCourse]
    @CourseCode NVARCHAR(6),
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
/****** Object:  StoredProcedure [dbo].[usp_CreateRegistration]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- REGISTRATION Stored Procedures
-- =============================================

-- Create Registration (returns generated ID)
CREATE   PROCEDURE [dbo].[usp_CreateRegistration]
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
/****** Object:  StoredProcedure [dbo].[usp_CreateStudent]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- STUDENT Stored Procedures
-- =============================================

-- Create Student (returns generated ID)
CREATE   PROCEDURE [dbo].[usp_CreateStudent]
    @UserId INT,
    @StudentNumber INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Student (UserId, StudentNumber, FullName, Email, Phone)
    VALUES (@UserId, @StudentNumber, ISNULL(@FullName, ''), @Email, @Phone);
    
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CreateUser]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- USER Stored Procedures
-- =============================================

-- Create User (returns generated ID)
CREATE   PROCEDURE [dbo].[usp_CreateUser]
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Role INT,
    @IsActive BIT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [User] (UserName, PasswordHash, FullName, Role, IsActive)
    VALUES (@UserName, @PasswordHash, @FullName, @Role, @IsActive);
    
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteClass]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Class
CREATE   PROCEDURE [dbo].[usp_DeleteClass]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Class WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteCourse]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Course
CREATE   PROCEDURE [dbo].[usp_DeleteCourse]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Course WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteRegistration]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Registration
CREATE   PROCEDURE [dbo].[usp_DeleteRegistration]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Registrations WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteStudent]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Student
CREATE   PROCEDURE [dbo].[usp_DeleteStudent]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Student WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteUser]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete User
CREATE   PROCEDURE [dbo].[usp_DeleteUser]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [User] WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllClasses]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Classes
CREATE   PROCEDURE [dbo].[usp_GetAllClasses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, CurrentCapacity, MaxCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllCourses]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Courses
CREATE   PROCEDURE [dbo].[usp_GetAllCourses]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllRegistrations]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Registrations
CREATE   PROCEDURE [dbo].[usp_GetAllRegistrations]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetAllRegistrationsDetailed]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Registrations Detailed
CREATE   PROCEDURE [dbo].[usp_GetAllRegistrationsDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.Id,
        r.StudentId,
        u.UserName,
        r.ClassId,
        c.ClassName,
        co.Id,
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
/****** Object:  StoredProcedure [dbo].[usp_GetAllStudents]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Students
CREATE   PROCEDURE [dbo].[usp_GetAllStudents]
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
/****** Object:  StoredProcedure [dbo].[usp_GetAllUsers]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get All Users
CREATE   PROCEDURE [dbo].[usp_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User];
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetClassById]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Class by ID
CREATE   PROCEDURE [dbo].[usp_GetClassById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetClassesByCourseId]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Classes by Course ID
CREATE   PROCEDURE [dbo].[usp_GetClassesByCourseId]
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE CourseId = @CourseId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetCourseById]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Course by ID
CREATE   PROCEDURE [dbo].[usp_GetCourseById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetRegistrationById]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Registration by ID
CREATE   PROCEDURE [dbo].[usp_GetRegistrationById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetRegistrationsByClassId]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Registrations by Class ID
CREATE   PROCEDURE [dbo].[usp_GetRegistrationsByClassId]
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE ClassId = @ClassId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetRegistrationsByStudentId]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Registrations by Student ID
CREATE   PROCEDURE [dbo].[usp_GetRegistrationsByStudentId]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE StudentId = @StudentId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetStudentById]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Student by ID
CREATE   PROCEDURE [dbo].[usp_GetStudentById]
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
/****** Object:  StoredProcedure [dbo].[usp_GetStudentByUserId]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Student by User ID
CREATE   PROCEDURE [dbo].[usp_GetStudentByUserId]
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
/****** Object:  StoredProcedure [dbo].[usp_GetStudentRegistrationsWithClasses]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Student Registrations with Classes
CREATE   PROCEDURE [dbo].[usp_GetStudentRegistrationsWithClasses]
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
/****** Object:  StoredProcedure [dbo].[usp_GetUserById]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get User by ID
CREATE   PROCEDURE [dbo].[usp_GetUserById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUserByUserName]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get User by Username
CREATE   PROCEDURE [dbo].[usp_GetUserByUserName]
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE UserName = @UserName;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RegistrationExists]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Check if Registration Exists
CREATE   PROCEDURE [dbo].[usp_RegistrationExists]
    @StudentId INT,
    @ClassId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 
    FROM Registrations 
    WHERE StudentId = @StudentId AND ClassId = @ClassId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchClasses]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Classes
CREATE   PROCEDURE [dbo].[usp_SearchClasses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive 
    FROM Class 
    WHERE ClassName LIKE '%' + @regex + '%' OR Instructor LIKE '%' + @regex + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchCourses]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Courses
CREATE   PROCEDURE [dbo].[usp_SearchCourses]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive 
    FROM Course 
    WHERE CourseCode LIKE '%' + @regex + '%' OR CourseName LIKE '%' + @regex + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchRegistrations]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Registrations
CREATE   PROCEDURE [dbo].[usp_SearchRegistrations]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, StudentId, ClassId, RegistrationDate, Status 
    FROM Registrations 
    WHERE Status LIKE '%' + @regex + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchRegistrationsDetailed]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Registrations Detailed
CREATE   PROCEDURE [dbo].[usp_SearchRegistrationsDetailed]
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
        co.Id,
        co.CourseName,
        r.RegistrationDate,
        r.Status
    FROM Registrations r
    INNER JOIN Student s ON r.StudentId = s.Id
    INNER JOIN [User] u ON s.UserId = u.Id
    INNER JOIN Class c ON r.ClassId = c.Id
    INNER JOIN Course co ON c.CourseId = co.Id
    WHERE u.UserName LIKE '%' + @regex + '%'
       OR s.FullName LIKE '%' + @regex + '%'
       OR c.ClassName LIKE '%' + @regex + '%'
       OR co.CourseName LIKE '%' + @regex + '%'
       OR r.Status LIKE '%' + @regex + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchStudents]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Students
CREATE   PROCEDURE [dbo].[usp_SearchStudents]
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
/****** Object:  StoredProcedure [dbo].[usp_SearchUsers]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search Users
CREATE   PROCEDURE [dbo].[usp_SearchUsers]
    @regex NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, PasswordHash, FullName, Role, IsActive 
    FROM [User] 
    WHERE UserName LIKE '%' + @regex + '%' OR FullName LIKE '%' + @regex + '%';
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateClass]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Class
CREATE   PROCEDURE [dbo].[usp_UpdateClass]
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
/****** Object:  StoredProcedure [dbo].[usp_UpdateCourse]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Course
CREATE   PROCEDURE [dbo].[usp_UpdateCourse]
    @Id INT,
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
/****** Object:  StoredProcedure [dbo].[usp_UpdateRegistration]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Registration
CREATE   PROCEDURE [dbo].[usp_UpdateRegistration]
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
/****** Object:  StoredProcedure [dbo].[usp_UpdateStudent]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Student
CREATE   PROCEDURE [dbo].[usp_UpdateStudent]
    @Id INT,
    @UserId INT,
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
/****** Object:  StoredProcedure [dbo].[usp_UpdateUser]    Script Date: 8/9/2026 8:28:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update User
CREATE   PROCEDURE [dbo].[usp_UpdateUser]
    @Id INT,
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

USE [CourseManagementDB];
GO

-- NOTE: This file is superseded by StoredProceduresV2.sql (canonical).
-- Kept for reference; made idempotent and aligned to the canonical
-- column contracts (StudentId + UserId, CourseId alias).
IF OBJECT_ID('dbo.usp_GetStudentProfileByUserId', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetStudentProfileByUserId];
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
    FROM [dbo].[Student] s
    INNER JOIN [dbo].[User] u ON s.[UserId] = u.[Id]
    WHERE s.[UserId] = @UserId;
END
GO

IF OBJECT_ID('dbo.usp_GetUserByUserName', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_GetUserByUserName];
GO

CREATE PROCEDURE [dbo].[usp_GetUserByUserName]
    @UserName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        [Id],
        [UserName],
        [PasswordHash],
        [FullName],
        [Role],
        [IsActive]
    FROM [dbo].[User]
    WHERE [UserName] = @UserName;
END
GO

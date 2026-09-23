-- ============================================================================
-- LoggingProcedures.sql — Stored procedures for AppLogger (normal DAL flow)
--
-- Matches the normal database flow used by DAL/Providers/*.cs:
--   SqlConnection + SqlCommand with CommandType.StoredProcedure,
--   explicit parameters, ExecuteNonQuery (see Shared/Logging/MssqlExceptionSink.cs).
-- One insert procedure per log table (tables created by LoggingTables.sql).
--
-- Run order: 1) TablesV2.sql  2) StoredProceduresV2.sql
--            3) LoggingTables.sql  4) LoggingProcedures.sql
-- Safe to re-run (drops all logging procedures first).
-- ============================================================================
USE [CourseManagementDB];
GO

IF OBJECT_ID('dbo.usp_InsertDatabaseException', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_InsertDatabaseException];
IF OBJECT_ID('dbo.usp_InsertBusinessException', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_InsertBusinessException];
IF OBJECT_ID('dbo.usp_InsertViewError', 'P') IS NOT NULL DROP PROCEDURE [dbo].[usp_InsertViewError];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- usp_InsertDatabaseException — DAL / DatabaseException path
-- ----------------------------------------------------------------------------
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

-- ----------------------------------------------------------------------------
-- usp_InsertBusinessException — BL validation / business errors + warnings
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

-- ----------------------------------------------------------------------------
-- usp_InsertViewError — WinForms UI errors
-- ----------------------------------------------------------------------------
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

-- ============================================================================
-- LoggingTables.sql — Log tables in the SAME database as user data
--   (CourseManagementDB). Primary logging target for AppLogger.
--   Written via Shared/Logging/MssqlExceptionSink.cs (same flow as the
--   normal DAL providers: SqlConnection + stored procedure + ExecuteNonQuery,
--   procedures in LoggingProcedures.sql). No DDL in code: this script is the
--   ONLY place tables are created (no EnsureTable DDL in code,
--   matching TablesV2.sql / StoredProceduresV2.sql flow).
--   SQLite file (Shared/Logs/exceptions.db) remains as secondary/fallback
--   when SQL Server is unreachable (see Shared/Logging/FailoverSink.cs).
--
-- Column mapping (MssqlExceptionSink Emit -> procedure parameters):
--   TimeStamp -> @TimestampUtc (UTC), Level -> @Level, Operation -> @Operation,
--   UserId -> @UserId, ExceptionType -> @ExceptionType,
--   Exception.Message -> @Message, Exception.ToString() -> @StackTrace.
--
-- Tables: DatabaseExceptions, BusinessExceptions, ViewErrors
--
-- Run order: 1) TablesV2.sql  2) StoredProcedures.sql  3) LoggingTables.sql
--            4) LoggingProcedures.sql
-- Safe to re-run (idempotent: creates only missing tables).
-- ============================================================================
USE [CourseManagementDB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- 1. DatabaseExceptions (DAL / DatabaseException path)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[DatabaseExceptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[DatabaseExceptions] (
        [Id]            INT IDENTITY(1,1) NOT NULL,
        [TimestampUtc]  DATETIME2(7) NOT NULL,
        [Level]         NVARCHAR(10) NOT NULL,
        [Operation]     NVARCHAR(200) NOT NULL,
        [UserId]        INT NULL,
        [ExceptionType] NVARCHAR(500) NOT NULL,
        [Message]       NVARCHAR(MAX) NOT NULL,
        [StackTrace]    NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_DatabaseExceptions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 2. BusinessExceptions (BL validation / business errors + warnings)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[BusinessExceptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BusinessExceptions] (
        [Id]            INT IDENTITY(1,1) NOT NULL,
        [TimestampUtc]  DATETIME2(7) NOT NULL,
        [Level]         NVARCHAR(10) NOT NULL,
        [Operation]     NVARCHAR(200) NOT NULL,
        [UserId]        INT NULL,
        [ExceptionType] NVARCHAR(500) NOT NULL,
        [Message]       NVARCHAR(MAX) NOT NULL,
        [StackTrace]    NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_BusinessExceptions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 3. ViewErrors (WinForms UI errors)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[ViewErrors]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ViewErrors] (
        [Id]            INT IDENTITY(1,1) NOT NULL,
        [TimestampUtc]  DATETIME2(7) NOT NULL,
        [Level]         NVARCHAR(10) NOT NULL,
        [Operation]     NVARCHAR(200) NOT NULL,
        [UserId]        INT NULL,
        [ExceptionType] NVARCHAR(500) NOT NULL,
        [Message]       NVARCHAR(MAX) NOT NULL,
        [StackTrace]    NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ViewErrors] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 4. Helpful indexes (filter by time / user / operation)
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DatabaseExceptions_TimestampUtc' AND object_id = OBJECT_ID(N'[dbo].[DatabaseExceptions]'))
    CREATE NONCLUSTERED INDEX [IX_DatabaseExceptions_TimestampUtc] ON [dbo].[DatabaseExceptions] ([TimestampUtc] DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_BusinessExceptions_TimestampUtc' AND object_id = OBJECT_ID(N'[dbo].[BusinessExceptions]'))
    CREATE NONCLUSTERED INDEX [IX_BusinessExceptions_TimestampUtc] ON [dbo].[BusinessExceptions] ([TimestampUtc] DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ViewErrors_TimestampUtc' AND object_id = OBJECT_ID(N'[dbo].[ViewErrors]'))
    CREATE NONCLUSTERED INDEX [IX_ViewErrors_TimestampUtc] ON [dbo].[ViewErrors] ([TimestampUtc] DESC);
GO

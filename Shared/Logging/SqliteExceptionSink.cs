using Microsoft.Data.Sqlite;
using Serilog.Core;
using Serilog.Events;

namespace Shared.Logging;

public sealed class SqliteExceptionSink : ILogEventSink, IDisposable
{
    private readonly string _dbPath;
    private readonly string _table;
    private readonly object _sync = new();
    private bool _disposed;

    public SqliteExceptionSink(string dbPath, string table)
    {
        _dbPath = dbPath;
        _table = table;
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using (var pragma = connection.CreateCommand())
        {
            pragma.CommandText = "PRAGMA journal_mode=WAL;";
            pragma.ExecuteNonQuery();
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS ""{_table}"" (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TimestampUtc TEXT NOT NULL,
                Level VARCHAR(10) NOT NULL,
                Operation TEXT NOT NULL,
                UserId INTEGER NULL,
                ExceptionType TEXT NOT NULL,
                Message TEXT NOT NULL,
                StackTrace TEXT NULL
            );";
            cmd.ExecuteNonQuery();
        }
    }

    public void Emit(LogEvent logEvent)
    {
        try
        {
            logEvent.Properties.TryGetValue("Operation", out var operation);
            logEvent.Properties.TryGetValue("UserId", out var userId);
            var ex = logEvent.Exception;
            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                using var connection = new SqliteConnection($"Data Source={_dbPath}");
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $@"INSERT INTO ""{_table}""
                    (TimestampUtc, Level, Operation, UserId, ExceptionType, Message, StackTrace)
                    VALUES ($ts, $level, $op, $uid, $type, $msg, $stack);";
                cmd.Parameters.AddWithValue("$ts", logEvent.Timestamp.UtcDateTime.ToString("o"));
                cmd.Parameters.AddWithValue("$level", logEvent.Level.ToString());
                cmd.Parameters.AddWithValue("$op", ScalarText(operation) ?? string.Empty);
                long? id = ScalarLong(userId);
                cmd.Parameters.AddWithValue("$uid", id.HasValue ? (object)id.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("$type", ex?.GetType().FullName ?? logEvent.Level.ToString());
                cmd.Parameters.AddWithValue("$msg", ex?.Message ?? Unquote(logEvent.RenderMessage()));
                cmd.Parameters.AddWithValue("$stack", ex != null ? (object)ex.ToString() : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception emitEx)
        {
            System.Diagnostics.Debug.WriteLine($"SqliteExceptionSink emit failed: {emitEx.Message}");
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _disposed = true;
        }
    }

    private static string Unquote(string message)
    {
        if (message.Length >= 2 && message.StartsWith("\"") && message.EndsWith("\""))
        {
            return message.Substring(1, message.Length - 2);
        }

        return message;
    }

    private static string? ScalarText(LogEventPropertyValue? value)
    {
        if (value is ScalarValue scalar)
        {
            return scalar.Value?.ToString();
        }

        return value?.ToString();
    }

    private static long? ScalarLong(LogEventPropertyValue? value)
    {
        if (value is ScalarValue scalar && scalar.Value != null)
        {
            if (scalar.Value is long l)
            {
                return l;
            }

            if (scalar.Value is int i)
            {
                return i;
            }

            if (long.TryParse(scalar.Value.ToString(), out long parsed))
            {
                return parsed;
            }
        }

        return null;
    }
}

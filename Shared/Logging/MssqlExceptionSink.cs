using Microsoft.Data.SqlClient;
using Serilog.Core;
using Serilog.Events;
using System.Data;

namespace Shared.Logging;

public sealed class MssqlExceptionSink : ILogEventSink, IDisposable
{
    private readonly string _connectionString;
    private readonly string _procedure;
    private bool _disposed;

    public MssqlExceptionSink(string connectionString, string table)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
        _procedure = ResolveProcedure(table);
    }

    public void Emit(LogEvent logEvent)
    {
        if (_disposed)
        {
            return;
        }

        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = new SqlConnection(_connectionString);
            connection.Open();

            command = new SqlCommand(_procedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@TimestampUtc", SqlDbType.DateTime2).Value = logEvent.Timestamp.UtcDateTime;
            command.Parameters.Add("@Level", SqlDbType.NVarChar, 10).Value = logEvent.Level.ToString();
            command.Parameters.Add("@Operation", SqlDbType.NVarChar, 200).Value = ReadOperation(logEvent) ?? string.Empty;

            int? userId = ReadUserId(logEvent);
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId.HasValue ? userId.Value : DBNull.Value;

            command.Parameters.Add("@ExceptionType", SqlDbType.NVarChar, 500).Value = ReadExceptionType(logEvent) ?? string.Empty;
            command.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = ReadMessage(logEvent) ?? string.Empty;

            string? stackTrace = logEvent.Exception?.ToString();
            command.Parameters.Add("@StackTrace", SqlDbType.NVarChar, -1).Value = stackTrace != null ? (object)stackTrace : DBNull.Value;

            command.ExecuteNonQuery();
        }
        catch
        {
            // Let FailoverSink see the failure and route to SQLite.
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public void Dispose()
    {
        _disposed = true;
    }

    internal static string ResolveProcedure(string table)
    {
        return table switch
        {
            "DatabaseExceptions" => "usp_InsertDatabaseException",
            "BusinessExceptions" => "usp_InsertBusinessException",
            "ViewErrors" => "usp_InsertViewError",
            _ => throw new ArgumentException($"Unknown log table: {table}", nameof(table)),
        };
    }

    private static string? ReadOperation(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("Operation", out LogEventPropertyValue? value)
            && value is ScalarValue scalar)
        {
            return scalar.Value?.ToString();
        }

        return null;
    }

    private static string? ReadExceptionType(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("ExceptionType", out LogEventPropertyValue? value)
            && value is ScalarValue scalar
            && scalar.Value != null)
        {
            return scalar.Value.ToString();
        }

        return logEvent.Exception?.GetType().FullName ?? logEvent.Level.ToString();
    }

    private static int? ReadUserId(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("UserId", out LogEventPropertyValue? value)
            && value is ScalarValue scalar
            && scalar.Value != null)
        {
            if (scalar.Value is int i)
            {
                return i;
            }

            if (scalar.Value is long l)
            {
                return (int)l;
            }

            if (int.TryParse(scalar.Value.ToString(), out int parsed))
            {
                return parsed;
            }
        }

        return null;
    }

    private static string? ReadMessage(LogEvent logEvent)
    {
        if (logEvent.Exception != null)
        {
            return logEvent.Exception.Message;
        }

        string rendered = logEvent.RenderMessage();
        if (rendered.Length >= 2 && rendered.StartsWith("\"") && rendered.EndsWith("\""))
        {
            return rendered.Substring(1, rendered.Length - 2);
        }

        return rendered;
    }
}

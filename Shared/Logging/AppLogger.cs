using Serilog;
using Shared.Exceptions;
using Shared.Session;
using System.Runtime.CompilerServices;

namespace Shared.Logging;

public static class AppLogger
{
    private static readonly object _sync = new();
    private static bool _configured;
    private static ILogger? _database;
    private static ILogger? _business;
    private static ILogger? _view;

    public static string DbPath => ResolveDbPath();

    private static string ResolveDbPath()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);
        for (int i = 0; i < 8 && current != null; i++)
        {
            string candidate = Path.Combine(current.FullName, "Shared", "Logs", "exceptions.db");
            string slnx = Path.Combine(current.FullName, "CourseRegistrationManagmentSystem.slnx");
            if (File.Exists(slnx))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "Logs", "exceptions.db");
    }

    public static void Configure()
    {
        if (_configured)
        {
            return;
        }

        lock (_sync)
        {
            if (_configured)
            {
                return;
            }

            try
            {
                _database = new LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.Sink(new SqliteExceptionSink(DbPath, "DatabaseExceptions"))
                    .CreateLogger();
                _business = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Sink(new SqliteExceptionSink(DbPath, "BusinessExceptions"))
                    .CreateLogger();
                _view = new LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.Sink(new SqliteExceptionSink(DbPath, "ViewErrors"))
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AppLogger configure failed: {ex.Message}");
            }

            _configured = true;
        }
    }

    public static void LogCaught(Exception ex, [CallerMemberName] string operation = "")
    {
        try
        {
            Configure();
            int? userId = SessionManager.Current?.UserId;
            ILogger? log = ContainsDatabaseException(ex) ? _database : _business;
            if (log == null)
            {
                return;
            }

            var contextualized = log.ForContext("Operation", operation);
            if (userId.HasValue)
            {
                contextualized = contextualized.ForContext("UserId", userId.Value);
            }

            contextualized.Error(ex, "{ErrorMessage}", ex.Message);
        }
        catch
        {
        }
    }

    public static void LogViewError(Exception ex, [CallerMemberName] string operation = "")
    {
        try
        {
            Configure();
            int? userId = SessionManager.Current?.UserId;
            if (_view == null)
            {
                return;
            }

            var contextualized = _view.ForContext("Operation", operation);
            if (userId.HasValue)
            {
                contextualized = contextualized.ForContext("UserId", userId.Value);
            }

            contextualized.Error(ex, "{ErrorMessage}", ex.Message);
        }
        catch
        {
        }
    }

    public static void Warn(string operation, string message, int? userId = null)
    {
        try
        {
            Configure();
            userId ??= SessionManager.Current?.UserId;
            if (_business == null)
            {
                return;
            }

            var contextualized = _business.ForContext("Operation", operation);
            if (userId.HasValue)
            {
                contextualized = contextualized.ForContext("UserId", userId.Value);
            }

            contextualized.Warning("{WarningMessage}", message);
        }
        catch
        {
        }
    }

    public static void Close()
    {
        try
        {
            (_database as IDisposable)?.Dispose();
            (_business as IDisposable)?.Dispose();
            (_view as IDisposable)?.Dispose();
        }
        catch
        {
        }
    }

    private static bool ContainsDatabaseException(Exception ex)
    {
        Exception? current = ex;
        while (current != null)
        {
            if (current is DatabaseException)
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}

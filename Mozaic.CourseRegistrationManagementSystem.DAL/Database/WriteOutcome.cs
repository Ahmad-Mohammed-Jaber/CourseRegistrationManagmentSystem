using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Exceptions;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Database;

/// <summary>
/// Maps stored-procedure write outcomes to <see cref="Result{T}"/>.
/// Normal user-facing outcomes (duplicate/unique conflicts, 0 rows =
/// not-found) become <see cref="Result{T}"/> failures.
/// Fundamentally unexpected failures (missing SP, connection loss,
/// FK integrity violations, FK-guard THROWs, ...) become
/// <see cref="DatabaseException"/>.
/// </summary>
internal static class WriteOutcome
{
    // Business-rule THROWs raised by our SPs (unique/duplicate).
    private static readonly HashSet<int> ConflictNumbers = new()
    {
        50002, // CourseCode already exists
        50005, // already registered
        50007, // Student already exists for UserId
        50008, // StudentNumber already exists
        50009, // UserName already exists
        2601,  // unique index
        2627,  // unique constraint
    };

    public static Result<int> FromNewId(int newId, string entityName) =>
        newId > 0
            ? Result<int>.Ok(newId)
            : Result<int>.Fail(
                ValidationStatus.Conflict, entityName,
                $"Failed to create {entityName}: database reported no new id.");

    public static Result<int> FromRows(int rowsAffected, string entityName, int id) =>
        rowsAffected > 0
            ? Result<int>.Ok(rowsAffected)
            : Result<int>.Fail(
                ValidationStatus.NotFound, entityName,
                $"{entityName} with id {id} not found.");

    /// <summary>
    /// Returns null when the SqlException is NOT a normal business conflict
    /// (caller must throw <see cref="DatabaseException"/>); otherwise the
    /// user-facing failure result.
    /// </summary>
    public static Result<int>? TryBusinessConflict(SqlException ex, string entityName)
    {
        if (ConflictNumbers.Contains(ex.Number))
        {
            return Result<int>.Fail(ValidationStatus.Conflict, entityName, Clean(ex.Message));
        }

        var msg = ex.Message ?? string.Empty;
        if (msg.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            msg.Contains("already registered", StringComparison.OrdinalIgnoreCase) ||
            msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
        {
            return Result<int>.Fail(ValidationStatus.Conflict, entityName, Clean(ex.Message));
        }

        return null;
    }

    public static DatabaseException Unexpected(string context, Exception ex) =>
        new($"An error occured in {context}", ex);

    private static string Clean(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Database operation failed.";
        }

        // SqlException message contains procedure/line noise; keep first sentence-ish line.
        var firstLine = message.Split('\n')[0].Trim();
        return string.IsNullOrWhiteSpace(firstLine) ? "Database operation failed." : firstLine;
    }
}

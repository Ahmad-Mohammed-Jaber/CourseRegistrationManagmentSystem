using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Validation;

internal static class DbRequest
{
    public static RequestResult FromNewId(int newId, string entityName, int? entityId = null) =>
        newId > 0
            ? RequestResult.Success(1, entityId ?? newId)
            : RequestResult.Failure($"Failed to create {entityName}: database reported no new id.");

    public static RequestResult FromRows(int rowsAffected, string entityName, int id) =>
        rowsAffected > 0
            ? RequestResult.Success(rowsAffected, id)
            : RequestResult.NotFound($"{entityName} with id {id} not found.", id);

    public static RequestResult FromDbException(DatabaseException ex, string entityName)
    {
        var message = Flatten(ex);
        if (message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("unique", StringComparison.OrdinalIgnoreCase))
        {
            return RequestResult.Conflict(ExtractDbMessage(ex));
        }

        if (message.Contains("does not exist", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return RequestResult.NotFound(ExtractDbMessage(ex));
        }

        // FK / constraint violations surface as conflicts against the DB state.
        return RequestResult.Conflict(ExtractDbMessage(ex));
    }

    private static string Flatten(Exception ex)
    {
        var current = ex;
        var parts = new List<string>();
        while (current != null)
        {
            parts.Add(current.Message);
            current = current.InnerException;
        }

        return string.Join(" | ", parts);
    }

    private static string ExtractDbMessage(DatabaseException ex)
    {
        var inner = ex.InnerException;
        while (inner?.InnerException != null)
        {
            inner = inner.InnerException;
        }

        var message = inner?.Message ?? ex.Message;
        return string.IsNullOrWhiteSpace(message) ? "Database operation failed." : message.Trim();
    }
}

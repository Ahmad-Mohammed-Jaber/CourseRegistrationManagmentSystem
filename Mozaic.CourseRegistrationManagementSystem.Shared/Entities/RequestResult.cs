namespace Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

public enum RequestStatus
{
    Success,
    NotFound,
    Conflict,
    Failure,
}

public class RequestResult
{
    public RequestStatus Status { get; }
    public string? ErrorMessage { get; }
    public int RowsAffected { get; }
    public int? EntityId { get; }

    public bool IsSuccess => Status == RequestStatus.Success;

    private RequestResult(RequestStatus status, string? error, int rows, int? entityId)
    {
        Status = status;
        ErrorMessage = error;
        RowsAffected = rows;
        EntityId = entityId;
    }

    public static RequestResult Success(int rowsAffected = 1, int? entityId = null)
        => new(RequestStatus.Success, null, rowsAffected, entityId);

    public static RequestResult NotFound(string? message = null, int? entityId = null)
        => new(RequestStatus.NotFound, message ?? "Resource not found.", 0, entityId);

    public static RequestResult Conflict(string message, int? entityId = null)
        => new(RequestStatus.Conflict, message, 0, entityId);

    public static RequestResult Failure(string message, int? entityId = null)
        => new(RequestStatus.Failure, message, 0, entityId);
}
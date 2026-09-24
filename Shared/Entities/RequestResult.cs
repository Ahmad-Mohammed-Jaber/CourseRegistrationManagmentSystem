namespace Shared.Entities;

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

    public bool IsSuccess => Status == RequestStatus.Success;

    private RequestResult(RequestStatus status, string? error, int rows)
    {
        Status = status;
        ErrorMessage = error;
        RowsAffected = rows;
    }

    public static RequestResult Success(int rowsAffected = 1)
        => new(RequestStatus.Success, null, rowsAffected);

    public static RequestResult NotFound(string? message = null)
        => new(RequestStatus.NotFound, message ?? "Resource not found.", 0);

    public static RequestResult Conflict(string message)
        => new(RequestStatus.Conflict, message, 0);

    public static RequestResult Failure(string message)
        => new(RequestStatus.Failure, message, 0);
}
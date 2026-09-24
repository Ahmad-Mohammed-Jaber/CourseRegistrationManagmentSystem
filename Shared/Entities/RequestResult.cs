namespace Shared.Entities;

// Outcome of a service/database request (existence, uniqueness, authorization,
// business-rule and persistence checks). Field/input validation stays in
// ValidationResult; services return this type for write operations.
public enum RequestStatus
{
    Success,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}

public record RequestError(string Field, string Message);

public record RequestResult(RequestStatus Status, List<RequestError>? Errors)
{
    public bool IsSuccess => Status == RequestStatus.Success;

    public string Message => Errors is not null 
        ? (IsSuccess ? string.Empty : Status.ToString())
        : string.Join(Environment.NewLine, Errors.Select(e => $"• {e.Message}"));
    
    public static RequestResult Ok() =>
        new(RequestStatus.Success, null);

    public static RequestResult Fail(RequestStatus status, string field, string message) =>
        new(status, new List<RequestError> { new RequestError(field, message) });
}

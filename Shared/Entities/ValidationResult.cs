namespace Shared.Entities;

public enum ValidationStatus
{
    Success,
    Invalid,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict
}

public sealed record ValidationError(string Field, string Message);

public record ValidationResult(ValidationStatus Status, IReadOnlyList<ValidationError> Errors)
{
    public bool IsSuccess => Status == ValidationStatus.Success;

    public string Message => Errors.Count == 0
        ? (IsSuccess ? string.Empty : Status.ToString())
        : string.Join(Environment.NewLine, Errors.Select(e => $"• {e.Message}"));
}

public sealed record Result<T>(ValidationStatus Status, IReadOnlyList<ValidationError> Errors, T? Value) : ValidationResult(Status, Errors);

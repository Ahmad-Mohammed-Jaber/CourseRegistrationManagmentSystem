namespace Shared.Entities;

public class ValidationResult
{
    public string Message { get; set; }

    public bool Success { get; set; }

    public ValidationResult(string message, bool success)
    {
        Message = message;
        Success = success;
    }

    public ValidationResult(bool success)
    {
        Success = success; 
    }

    public ValidationResult()
    {
        
    }
}

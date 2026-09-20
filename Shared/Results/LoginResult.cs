namespace Shared.Results;

using Shared.Session;

public enum LoginStatus
{
    Success,
    Invalid 
}

public record LoginResult(
    LoginStatus LoginStatus,
    UserSession? UserSession = null);

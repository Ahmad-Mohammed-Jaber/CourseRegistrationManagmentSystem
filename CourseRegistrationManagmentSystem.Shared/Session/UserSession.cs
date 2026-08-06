namespace Shared.Session;

using Shared.Entities;

public record UserSession(int UserId, string UserName, string FullName, bool IsActive)
{
    public User.UserRoles Role { get; init; }

    public bool IsAdmin => Role == User.UserRoles.Admin;
}
namespace CourseRegistrationManagmentSystem.Shared.Session;
using CourseRegistrationManagmentSystem.Shared.Models;
public record UserSession(Guid UserId, string UserName, string FullName, bool IsActive)
{
    public User.UserRoles Role { get; init; }

    public bool IsAdmin => Role == User.UserRoles.Admin;
}

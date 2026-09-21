namespace Shared.Session;

using Shared.Entities;

public class UserSession
{
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public User.UserRoles Role { get; set; }

    public UserSession(int userId, string userName, string fullName, bool isActive)
    {
        UserId = userId;
        UserName = userName;
        FullName = fullName;
        IsActive = isActive;
    }
}

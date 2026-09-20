namespace Shared.Session;

using Shared.Entities;

/// <summary>
/// Single session for authentication & authorization only.
/// Student data is NOT stored here — fetch from DB via StudentService/StudentManager when needed.
/// </summary>
public record UserSession(
    int UserId,
    string UserName,
    string FullName,
    bool IsActive,
    User.UserRoles Role);

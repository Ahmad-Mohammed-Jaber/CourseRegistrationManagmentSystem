using UserEntity = Mozaic.CourseRegistrationManagementSystem.Shared.Entities.User;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Translates the string values posted by the filter dropdowns into the
/// nullable values understood by the service layer.
/// </summary>
internal static class FilterValues
{
    public static bool? ParseActive(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "active" => true,
            "inactive" => false,
            _ => null,
        };
    }

    public static UserEntity.UserRoles? ParseRole(string? role)
    {
        return role?.Trim().ToLowerInvariant() switch
        {
            "admin" => UserEntity.UserRoles.Admin,
            "student" => UserEntity.UserRoles.Student,
            _ => null,
        };
    }
}

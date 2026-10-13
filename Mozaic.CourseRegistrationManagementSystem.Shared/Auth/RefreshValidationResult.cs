using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Auth;

public sealed record RefreshValidationResult(bool Succeeded, User? User, RefreshToken? Record)
{
    public static RefreshValidationResult Success(User user, RefreshToken record) =>
        new(true, user, record);

    public static RefreshValidationResult Invalid() =>
        new(false, null, null);
}

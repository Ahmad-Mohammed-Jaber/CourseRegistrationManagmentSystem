using Shared.Entities;
using Shared.Session;

namespace BL.Validation;

public static class AccessValidator
{
    public static ValidationResult RequireLogin()
    {
        if (SessionManager.Current == null)
        {
            return new ValidationResult(
                ValidationStatus.Unauthorized,
                new[] { new ValidationError(string.Empty, "You must be logged in to perform this action.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireAdmin()
    {
        var login = RequireLogin();
        if (!login.IsSuccess)
        {
            return login;
        }

        if (SessionManager.Current!.Role != User.UserRoles.Admin)
        {
            return new ValidationResult(
                ValidationStatus.Forbidden,
                new[] { new ValidationError(string.Empty, "Administrative privileges are required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireRole(User.UserRoles role)
    {
        var login = RequireLogin();
        if (!login.IsSuccess)
        {
            return login;
        }

        if (SessionManager.Current!.Role != role)
        {
            return new ValidationResult(
                ValidationStatus.Forbidden,
                new[] { new ValidationError(string.Empty, $"Role '{role}' is required to perform this action.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireStudent()
    {
        return RequireRole(User.UserRoles.Student);
    }

    public static ValidationResult RequireOwnerOrAdmin(int ownerUserId)
    {
        var login = RequireLogin();
        if (!login.IsSuccess)
        {
            return login;
        }

        var current = SessionManager.Current!;
        if (current.Role != User.UserRoles.Admin && current.UserId != ownerUserId)
        {
            return new ValidationResult(
                ValidationStatus.Forbidden,
                new[] { new ValidationError(string.Empty, "You can only access your own data.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }
}

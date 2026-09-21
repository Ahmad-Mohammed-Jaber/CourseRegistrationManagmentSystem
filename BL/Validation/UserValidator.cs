using Shared.Entities;

namespace BL.Validation;

public static class UserValidator
{
    public static ValidationResult ValidateUserName(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(User.UserName), "Username is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(User.FullName), "Full name is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError("Password", "Password must be at least 6 characters long.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateUser(User? user)
    {
        if (user == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(User), "User is required.") });
        }

        var errors = new List<ValidationError>();
        Collect(ValidateUserName(user.UserName), errors);
        Collect(ValidateFullName(user.FullName), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult ValidateAdminRegistration(string? userName, string? fullName, string? password)
    {
        var errors = new List<ValidationError>();
        Collect(ValidateUserName(userName), errors);
        Collect(ValidateFullName(fullName), errors);
        Collect(ValidatePassword(password), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult RequireExists(User? user, int id)
    {
        if (user == null)
        {
            return new ValidationResult(
                ValidationStatus.NotFound,
                new[] { new ValidationError(nameof(User), $"User with id {id} not found.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireUniqueUserName(bool exists, string userName)
    {
        if (exists)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(User.UserName), $"A user with username '{userName}' already exists.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    internal static void Collect(ValidationResult result, List<ValidationError> errors)
    {
        if (!result.IsSuccess)
        {
            errors.AddRange(result.Errors);
        }
    }
}

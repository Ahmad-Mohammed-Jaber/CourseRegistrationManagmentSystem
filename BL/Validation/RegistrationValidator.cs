using Shared.Entities;

namespace BL.Validation;

public static class RegistrationValidator
{
    public static ValidationResult ValidateStudentId(int studentId)
    {
        if (studentId <= 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Registration.StudentId), "Student is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateClassId(int classId)
    {
        if (classId <= 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Registration.ClassId), "Class is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Registration.Status), "Status is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateRegistration(Registration? registration)
    {
        if (registration == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Registration), "Registration is required.") });
        }

        var errors = new List<ValidationError>();
        UserValidator.Collect(ValidateStudentId(registration.StudentId), errors);
        UserValidator.Collect(ValidateClassId(registration.ClassId), errors);
        UserValidator.Collect(ValidateStatus(registration.Status), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult RequireExists(Registration? registration, int id)
    {
        if (registration == null)
        {
            return new ValidationResult(
                ValidationStatus.NotFound,
                new[] { new ValidationError(nameof(Registration), $"Registration with id {id} not found.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateNotDuplicate(bool alreadyRegistered)
    {
        if (alreadyRegistered)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(Registration), "This student is already registered for the selected class.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateAlreadyRegistered(bool alreadyRegistered)
    {
        if (alreadyRegistered)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(Registration), "Class already registered.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateOwnership(int registrationStudentId, int currentStudentId)
    {
        if (registrationStudentId != currentStudentId)
        {
            return new ValidationResult(
                ValidationStatus.Forbidden,
                new[] { new ValidationError(nameof(Registration), "You do not have permission to drop this registration.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }
}

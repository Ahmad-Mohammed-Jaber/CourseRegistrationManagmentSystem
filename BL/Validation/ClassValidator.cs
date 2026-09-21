using Shared.Entities;

namespace BL.Validation;

public static class ClassValidator
{
    public static ValidationResult ValidateClassName(string? className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class.ClassName), "Class name is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCourseId(int courseId)
    {
        if (courseId <= 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class.CourseId), "Course is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateMaxCapacity(int maxCapacity)
    {
        if (maxCapacity <= 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class.MaxCapacity), "Max capacity must be greater than 0.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCurrentCapacity(int currentCapacity)
    {
        if (currentCapacity < 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class.CurrentCapacity), "Current capacity cannot be negative.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCapacityRange(int currentCapacity, int maxCapacity)
    {
        var errors = new List<ValidationError>();
        UserValidator.Collect(ValidateCurrentCapacity(currentCapacity), errors);
        UserValidator.Collect(ValidateMaxCapacity(maxCapacity), errors);
        if (errors.Count == 0 && currentCapacity > maxCapacity)
        {
            errors.Add(new ValidationError(nameof(Class.CurrentCapacity), "Current capacity cannot exceed max capacity."));
        }

        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate != default && endDate != default && endDate < startDate)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class.EndDate), "End date cannot be before start date.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateClass(Class? classEntity)
    {
        if (classEntity == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class), "Class is required.") });
        }

        var errors = new List<ValidationError>();
        UserValidator.Collect(ValidateClassName(classEntity.ClassName), errors);
        UserValidator.Collect(ValidateCourseId(classEntity.CourseId), errors);
        UserValidator.Collect(ValidateCapacityRange(classEntity.CurrentCapacity, classEntity.MaxCapacity), errors);
        UserValidator.Collect(ValidateDateRange(classEntity.StartDate, classEntity.EndDate), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult RequireExists(Class? classEntity, int id)
    {
        if (classEntity == null)
        {
            return new ValidationResult(
                ValidationStatus.NotFound,
                new[] { new ValidationError(nameof(Class), $"Class with id {id} not found.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateMaxCapacityNotBelowEnrollment(int maxCapacity, int currentEnrollment)
    {
        if (maxCapacity < currentEnrollment)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(
                    nameof(Class.MaxCapacity),
                    $"Max capacity ({maxCapacity}) cannot be less than current enrollment ({currentEnrollment}).") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateIsActive(Class? classEntity)
    {
        if (classEntity == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class), "Class is required.") });
        }

        if (!classEntity.IsActive)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(Class.IsActive), "This class is no longer active.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCapacityAvailable(Class? classEntity)
    {
        if (classEntity == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Class), "Class is required.") });
        }

        if (classEntity.CurrentCapacity >= classEntity.MaxCapacity)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(Class.MaxCapacity), "Class is full.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }
}

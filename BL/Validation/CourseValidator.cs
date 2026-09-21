using Shared.Entities;

namespace BL.Validation;

public static class CourseValidator
{
    public static ValidationResult ValidateCourseCode(string? courseCode)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Course.CourseCode), "Course code is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCourseName(string? courseName)
    {
        if (string.IsNullOrWhiteSpace(courseName))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Course.CourseName), "Course name is required.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCreditHours(double creditHours)
    {
        if (creditHours < 0)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Course.CreditHours), "Credit hours cannot be negative.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateCourse(Course? course)
    {
        if (course == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Course), "Course is required.") });
        }

        var errors = new List<ValidationError>();
        UserValidator.Collect(ValidateCourseCode(course.CourseCode), errors);
        UserValidator.Collect(ValidateCourseName(course.CourseName), errors);
        UserValidator.Collect(ValidateCreditHours(course.CreditHours), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult RequireExists(Course? course, int id)
    {
        if (course == null)
        {
            return new ValidationResult(
                ValidationStatus.NotFound,
                new[] { new ValidationError(nameof(Course), $"Course with id {id} not found.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireUniqueCourseCode(bool exists, string courseCode)
    {
        if (exists)
        {
            return new ValidationResult(
                ValidationStatus.Conflict,
                new[] { new ValidationError(nameof(Course.CourseCode), $"A course with code '{courseCode}' already exists.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }
}

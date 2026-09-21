
namespace BL.Validation;
using Shared.Entities;


 using System.Text.RegularExpressions;


public static class StudentValidator
{
    public static ValidationResult ValidateStudentNumber(int studentNumber)
    {
        var errors = new List<ValidationError>();
        CollectStudentNumberError(studentNumber, errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Student.Email), "Invalid email format.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidatePhone(string? phone)
    {
        // DB column is NVARCHAR(20); keep validation in sync.
        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?[\d\s\-()]{7,20}$"))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Student.Phone), "Invalid phone number format.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult ValidateStudent(Student? student)
    {
        if (student == null)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError(nameof(Student), "Student is required.") });
        }

        var errors = new List<ValidationError>();
        UserValidator.Collect(UserValidator.ValidateUserName(student.UserName), errors);
        UserValidator.Collect(UserValidator.ValidateFullName(student.FullName), errors);
        CollectStudentNumberError(student.StudentNumber, errors);
        UserValidator.Collect(ValidateEmail(student.Email), errors);
        UserValidator.Collect(ValidatePhone(student.Phone), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult ValidateStudentRegistration(
        string? userName, int studentNumber, string? fullName, string? email, string? phone, string? password)
    {
        var errors = new List<ValidationError>();
        UserValidator.Collect(UserValidator.ValidateUserName(userName), errors);
        UserValidator.Collect(UserValidator.ValidateFullName(fullName), errors);
        UserValidator.Collect(UserValidator.ValidatePassword(password), errors);
        CollectStudentNumberError(studentNumber, errors);
        UserValidator.Collect(ValidateEmail(email), errors);
        UserValidator.Collect(ValidatePhone(phone), errors);
        return new ValidationResult(
            errors.Count == 0 ? ValidationStatus.Success : ValidationStatus.Invalid,
            errors);
    }

    public static ValidationResult RequireExists(Student? student, int id)
    {
        if (student == null)
        {
            return new ValidationResult(
                ValidationStatus.NotFound,
                new[] { new ValidationError(nameof(Student), $"Student with id {id} not found.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    public static ValidationResult RequireProfileExists(Student? student)
    {
        if (student == null)
        {
            return new ValidationResult(
                ValidationStatus.Forbidden,
                new[] { new ValidationError(nameof(Student), "Student profile not found for current user. Please contact admin.") });
        }

        return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
    }

    internal static void CollectStudentNumberError(int studentNumber, List<ValidationError> errors)
    {
        string s = studentNumber.ToString();
        if (!Regex.IsMatch(s, @"^\d{9}$"))
        {
            errors.Add(new ValidationError(
                nameof(Student.StudentNumber),
                "Student number must be exactly 9 digits matching format YYYY S NNNN (e.g. 202210978)."));
            return;
        }

        if (!int.TryParse(s.Substring(0, 4), out int year))
        {
            errors.Add(new ValidationError(nameof(Student.StudentNumber), "Invalid year in student number."));
            return;
        }

        int currentYear = DateTime.Now.Year;
        if (year < 2000 || year > currentYear)
        {
            errors.Add(new ValidationError(
                nameof(Student.StudentNumber),
                $"Student number year must be between 2000 and {currentYear}."));
        }

        char semester = s[4];
        if (semester != '1' && semester != '2' && semester != '3')
        {
            errors.Add(new ValidationError(
                nameof(Student.StudentNumber),
                "Student number semester digit must be 1, 2, or 3."));
        }
    }
}

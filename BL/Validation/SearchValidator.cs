using Shared.Entities;
using System.Text.RegularExpressions;

namespace BL.Validation;

public static class SearchValidator
{
    public static ValidationResult ValidateSearchPattern(string? regex)
    {
        if (string.IsNullOrWhiteSpace(regex))
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError("Search", "Search pattern is required.") });
        }

        try
        {
            _ = new Regex(regex);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (ArgumentException)
        {
            return new ValidationResult(
                ValidationStatus.Invalid,
                new[] { new ValidationError("Search", "Invalid search pattern.") });
        }
    }
}

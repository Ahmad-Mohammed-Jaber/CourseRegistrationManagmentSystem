using System.Text.RegularExpressions;
using Shared.Session;
using Shared.Entities;

namespace BL.Validation;

public static class AccessValidator
{
    public static void RequireLogin()
    {
        if (SessionManager.Current == null)
            throw new UnauthorizedAccessException("You must be logged in to perform this action.");
    }

    public static void RequireAdmin()
    {
        RequireLogin();
        if (SessionManager.Current!.Role != User.UserRoles.Admin)
            throw new UnauthorizedAccessException("Administrative privileges are required.");
    }

    public static void RequireRole(User.UserRoles role)
    {
        RequireLogin();
        if (SessionManager.Current!.Role != role)
            throw new UnauthorizedAccessException($"Role '{role}' is required to perform this action.");
    }

    public static void RequireStudent()
    {
        RequireRole(User.UserRoles.Student);
    }

    // --- Obsolete bool-based helpers for backward compat (do not use in new code) ---
    [Obsolete("Use void RequireLogin() that throws. This bool version is for compat only.")]
    public static bool RequireLoginBool() => SessionManager.Current != null;

    [Obsolete("Use void RequireAdmin() that throws.")]
    public static bool RequireAdminBool() => SessionManager.Current != null && SessionManager.Current.Role == User.UserRoles.Admin;

    [Obsolete("Use void RequireRole() that throws.")]
    public static bool RequireRoleBool(User.UserRoles role) => SessionManager.Current != null && SessionManager.Current.Role == role;

    public static void ValidateStudentNumber(int studentNumber)
    {
        string s = studentNumber.ToString();
        if (!Regex.IsMatch(s, @"^\d{9}$"))
        {
            throw new ArgumentException("Student number must be exactly 9 digits matching format YYYY S NNNN (e.g. 202210978).");
        }
        if (!int.TryParse(s.Substring(0, 4), out int year))
        {
            throw new ArgumentException("Invalid year in student number.");
        }
        int currentYear = DateTime.Now.Year;
        if (year < 2000 || year > currentYear)
        {
            throw new ArgumentException($"Student number year must be between 2000 and {currentYear}.");
        }
        char semester = s[4];
        if (semester != '1' && semester != '2' && semester != '3')
        {
            throw new ArgumentException("Student number semester digit must be 1, 2, or 3.");
        }
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email format.");
        }
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?[\d\s\-()]{7,15}$"))
        {
            throw new ArgumentException("Invalid phone number format.");
        }
    }

    public static void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username is required.");
        }
    }

    public static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.");
        }
    }

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters long.");
        }
    }
}

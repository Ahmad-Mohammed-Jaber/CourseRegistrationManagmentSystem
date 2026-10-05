using System.Security.Claims;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp;

/// <summary>
/// Reads the current web user from the cookie claims.
/// Web equivalent of Shared SessionManager (which stays WinForms-only:
/// a static would leak across concurrent web requests).
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public const string FullNameClaimType = "FullName";

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : 0;
    }

    public static string GetFullName(this ClaimsPrincipal user) =>
        user.FindFirstValue(FullNameClaimType)
        ?? user.FindFirstValue(ClaimTypes.Name)
        ?? string.Empty;
}

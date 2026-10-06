using System.Security.Claims;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp;

public static class ClaimsPrincipalExtensions
{
    public const string FullNameClaimType = JwtTokenService.FullNameClaimType;

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

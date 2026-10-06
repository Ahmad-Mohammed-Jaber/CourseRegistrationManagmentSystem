namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Mozaic.CourseRegistration";

    public string Audience { get; init; } = "Mozaic.CourseRegistration.WebApp";

    public string SecretKey { get; init; } = string.Empty;

    public int ExpiryMinutes { get; init; } = 480;
}

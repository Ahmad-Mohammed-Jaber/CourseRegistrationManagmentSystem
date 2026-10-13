namespace Mozaic.CourseRegistrationManagementSystem.Shared.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Mozaic.CourseRegistration";

    public string Audience { get; init; } = "Mozaic.CourseRegistration.WebApp";

    public string SecretKey { get; init; } = "4a6f2b1d8e9c4021b3a57f68c3e214d9";

    public int ExpiryMinutes { get; init; } = 15;

    public int RefreshTokenExpiryDays { get; init; } = 7;
}

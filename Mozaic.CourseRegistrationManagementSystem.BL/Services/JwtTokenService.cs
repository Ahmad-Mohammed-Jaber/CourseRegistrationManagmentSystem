using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public class JwtTokenService
{
    public const string FullNameClaimType = "FullName";

    private static readonly JwtSecurityTokenHandler Handler = new();
    private static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(2);

    private readonly JwtOptions _options;
    private readonly byte[] _key;

    public JwtTokenService(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.SecretKey) || Encoding.UTF8.GetByteCount(options.SecretKey) < 32)
        {
            throw new ArgumentException("Jwt SecretKey must be at least 32 characters (256 bits).", nameof(options));
        }

        if (options.ExpiryMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Jwt ExpiryMinutes must be positive.");
        }

        _options = options;
        _key = Encoding.UTF8.GetBytes(options.SecretKey);
    }

    public string GenerateToken(UserSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        try
        {
            var now = DateTime.UtcNow;
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(_options.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                    new Claim(ClaimTypes.Name, session.UserName),
                    new Claim(FullNameClaimType, session.FullName),
                    new Claim(ClaimTypes.Role, session.Role.ToString()),
                }),
            };
            descriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));

            return Handler.WriteToken(Handler.CreateToken(descriptor));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while generating the authentication token.", ex);
        }
    }

    public ClaimsPrincipal? ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            return Handler.ValidateToken(token, BuildValidationParameters(), out _);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            return null;
        }
    }

    private TokenValidationParameters BuildValidationParameters() => new()
    {
        ValidateIssuer = true,
        ValidIssuer = _options.Issuer,
        ValidateAudience = true,
        ValidAudience = _options.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(_key),
        ClockSkew = DefaultClockSkew,
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role,
    };
}

using System.Security.Cryptography;
using System.Text;
using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Auth;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public class RefreshTokenService
{
    public const int TokenBytes = 64;

    private readonly RefreshTokenRepository _repository = new();
    private readonly JwtOptions _options;
    private readonly Func<int, Task<User?>> _userResolver;

    public RefreshTokenService(JwtOptions options, Func<int, Task<User?>>? userResolver = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        if (_options.RefreshTokenExpiryDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Jwt RefreshTokenExpiryDays must be positive.");
        }
        _userResolver = userResolver ?? (id =>
            new UserManager().GetAsync(id: id).ContinueWith(t => t.Result.Items.FirstOrDefault(), TaskContinuationOptions.ExecuteSynchronously));
    }

    public static string HashToken(string plaintextToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintextToken);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plaintextToken)));
    }

    public static string CreateOpaqueToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenBytes);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public DateTimeOffset RefreshExpiry(DateTimeOffset now) =>
        now.AddDays(_options.RefreshTokenExpiryDays);

    public async Task<(RefreshToken Record, string PlainTextToken)> IssueAsync(int userId, DateTimeOffset? expiresAt = null)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var plaintext = CreateOpaqueToken();
            var record = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashToken(plaintext),
                ExpiresAt = expiresAt ?? RefreshExpiry(now),
                CreatedAt = now,
            };
            await _repository.AddAsync(record);
            return (record, plaintext);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while issuing the refresh token.", ex);
        }
    }

    public async Task<RefreshValidationResult> ValidateAsync(string? presentedToken)
    {
        if (string.IsNullOrWhiteSpace(presentedToken))
        {
            return RefreshValidationResult.Invalid();
        }

        try
        {
            var stored = await _repository.GetByHashAsync(HashToken(presentedToken.Trim()));
            if (stored is null || stored.IsRevoked || stored.IsExpired(DateTimeOffset.UtcNow))
            {
                return RefreshValidationResult.Invalid();
            }

            var user = await _userResolver(stored.UserId);
            if (user is null || !user.IsActive)
            {
                return RefreshValidationResult.Invalid();
            }

            return RefreshValidationResult.Success(user, stored);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while validating the refresh token.", ex);
        }
    }

    public async Task<bool> RevokeAsync(string? presentedToken)
    {
        if (string.IsNullOrWhiteSpace(presentedToken))
        {
            return true;
        }

        try
        {
            await _repository.RevokeAsync(HashToken(presentedToken.Trim()));
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while revoking the refresh token.", ex);
        }
    }

    public async Task RevokeAllForUserAsync(int userId)
    {
        try
        {
            if (userId > 0)
            {
                await _repository.RevokeAllForUserAsync(userId);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while revoking refresh tokens.", ex);
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public sealed record RefreshRotationResult(
    bool Succeeded,
    bool ReuseDetected,
    int? UserId,
    string? NewToken,
    User? User)
{
    public static RefreshRotationResult Success(int userId, string newToken, User user) =>
        new(true, false, userId, newToken, user);

    public static RefreshRotationResult Invalid() =>
        new(false, false, null, null, null);

    public static RefreshRotationResult Reused(int userId) =>
        new(false, true, userId, null, null);
}

public class RefreshTokenService
{
    public const int TokenBytes = 64;

    private readonly IRefreshTokenStore _store;
    private readonly JwtOptions _options;
    private readonly Func<int, Task<User?>> _userResolver;

    public RefreshTokenService(IRefreshTokenStore store, JwtOptions options, Func<int, Task<User?>>? userResolver = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        if (_options.RefreshTokenExpiryDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Jwt RefreshTokenExpiryDays must be positive.");
        }
        if (_options.RefreshReuseGraceSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Jwt RefreshReuseGraceSeconds must be non-negative.");
        }
        _userResolver = userResolver ?? (id => new UserManager().GetByIdAsync(id));
    }

    public RefreshTokenService(JwtOptions options)
        : this(new SqlRefreshTokenStore(), options)
    {
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
                FamilyId = Guid.NewGuid(),
                ExpiresAt = expiresAt ?? RefreshExpiry(now),
                CreatedAt = now,
            };
            await _store.AddAsync(record);
            return (record, plaintext);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while issuing the refresh token.", ex);
        }
    }

    public async Task<RefreshRotationResult> RotateAsync(string? presentedToken)
    {
        if (string.IsNullOrWhiteSpace(presentedToken))
        {
            return RefreshRotationResult.Invalid();
        }

        try
        {
            var now = DateTimeOffset.UtcNow;
            var hash = HashToken(presentedToken.Trim());
            var stored = await _store.GetByHashAsync(hash);
            if (stored is null)
            {
                return RefreshRotationResult.Invalid();
            }

            if (stored.IsRevoked)
            {
                if (!string.IsNullOrEmpty(stored.ReplacedByHash))
                {
                    // Recently rotated: this may be a parallel request racing
                    // on the same token (two tabs) rather than theft. Advance
                    // the chain from the replacement inside the grace window.
                    if (stored.RevokedAt.HasValue
                        && stored.RevokedAt.Value >= now.AddSeconds(-_options.RefreshReuseGraceSeconds))
                    {
                        var successor = await _store.GetByHashAsync(stored.ReplacedByHash);
                        if (successor is not null
                            && successor.UserId == stored.UserId
                            && !successor.IsRevoked
                            && !successor.IsExpired(now))
                        {
                            stored = successor;
                        }
                        else
                        {
                            await _store.RevokeFamilyAsync(stored.FamilyId);
                            return RefreshRotationResult.Reused(stored.UserId);
                        }
                    }
                    else
                    {
                        // Replay of an already-rotated token: possible theft.
                        // Revoke the whole family to kill the attacker's session too.
                        await _store.RevokeFamilyAsync(stored.FamilyId);
                        return RefreshRotationResult.Reused(stored.UserId);
                    }
                }
                else
                {
                    return RefreshRotationResult.Invalid();
                }
            }

            if (stored.IsExpired(now))
            {
                return RefreshRotationResult.Invalid();
            }

            var user = await _userResolver(stored.UserId);
            if (user is null || !user.IsActive)
            {
                await _store.RevokeFamilyAsync(stored.FamilyId);
                return RefreshRotationResult.Invalid();
            }

            // Insert the replacement first so a failure here leaves the
            // presented token still valid for a retry.
            var plaintext = CreateOpaqueToken();
            var replacement = new RefreshToken
            {
                UserId = stored.UserId,
                TokenHash = HashToken(plaintext),
                FamilyId = stored.FamilyId,
                ExpiresAt = RefreshExpiry(now),
                CreatedAt = now,
            };
            await _store.AddAsync(replacement);
            await _store.MarkRotatedAsync(stored.Id, replacement.TokenHash);

            return RefreshRotationResult.Success(stored.UserId, plaintext, user);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while rotating the refresh token.", ex);
        }
    }

    public async Task RevokeAllForUserAsync(int userId)
    {
        try
        {
            if (userId > 0)
            {
                await _store.RevokeAllForUserAsync(userId);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while revoking refresh tokens.", ex);
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
            var stored = await _store.GetByHashAsync(HashToken(presentedToken.Trim()));
            if (stored is null)
            {
                return true;
            }

            await _store.RevokeFamilyAsync(stored.FamilyId);
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while revoking the refresh token.", ex);
        }
    }
}

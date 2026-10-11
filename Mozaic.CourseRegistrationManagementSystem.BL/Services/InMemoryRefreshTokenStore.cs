using System.Collections.Concurrent;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public sealed class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshToken> _byHash = new(StringComparer.Ordinal);
    private int _idSequence;

    public Task AddAsync(RefreshToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.Id = Interlocked.Increment(ref _idSequence);
        token.CreatedAt = token.CreatedAt == default ? DateTimeOffset.UtcNow : token.CreatedAt;
        if (!_byHash.TryAdd(token.TokenHash, token))
        {
            throw new InvalidOperationException("Refresh token hash already exists.");
        }
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        _byHash.TryGetValue(tokenHash ?? string.Empty, out var token);
        return Task.FromResult(token);
    }

    public Task MarkRotatedAsync(int id, string replacedByHash)
    {
        var row = _byHash.Values.FirstOrDefault(t => t.Id == id);
        if (row is not null && !row.IsRevoked)
        {
            row.RevokedAt = DateTimeOffset.UtcNow;
            row.ReplacedByHash = replacedByHash;
        }
        return Task.CompletedTask;
    }

    public Task RevokeFamilyAsync(Guid familyId)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var row in _byHash.Values.Where(t => t.FamilyId == familyId && !t.IsRevoked))
        {
            row.RevokedAt = now;
        }
        return Task.CompletedTask;
    }

    public Task RevokeAllForUserAsync(int userId)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var row in _byHash.Values.Where(t => t.UserId == userId && !t.IsRevoked))
        {
            row.RevokedAt = now;
        }
        return Task.CompletedTask;
    }
}

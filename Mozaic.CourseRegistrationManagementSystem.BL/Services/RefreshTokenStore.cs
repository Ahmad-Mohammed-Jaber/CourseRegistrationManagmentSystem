using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public interface IRefreshTokenStore
{
    Task AddAsync(RefreshToken token);

    Task<RefreshToken?> GetByHashAsync(string tokenHash);

    Task MarkRotatedAsync(int id, string replacedByHash);

    Task RevokeFamilyAsync(Guid familyId);

    Task RevokeAllForUserAsync(int userId);
}

public sealed class SqlRefreshTokenStore : IRefreshTokenStore
{
    private readonly RefreshTokenRepository _repository = new();

    public Task AddAsync(RefreshToken token) => _repository.AddAsync(token);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash) => _repository.GetByHashAsync(tokenHash);

    public Task MarkRotatedAsync(int id, string replacedByHash) => _repository.MarkRotatedAsync(id, replacedByHash);

    public Task RevokeFamilyAsync(Guid familyId) => _repository.RevokeFamilyAsync(familyId);

    public Task RevokeAllForUserAsync(int userId) => _repository.RevokeAllForUserAsync(userId);
}

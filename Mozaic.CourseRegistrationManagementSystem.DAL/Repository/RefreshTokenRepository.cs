using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class RefreshTokenRepository
{
    public Task AddAsync(RefreshToken entity) =>
        RefreshTokenDataProvider.AddAsync(entity);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash) =>
        RefreshTokenDataProvider.GetByHashAsync(tokenHash);

    public Task MarkRotatedAsync(int id, string replacedByHash) =>
        RefreshTokenDataProvider.MarkRotatedAsync(id, replacedByHash);

    public Task RevokeFamilyAsync(Guid familyId) =>
        RefreshTokenDataProvider.RevokeFamilyAsync(familyId);

    public Task RevokeAllForUserAsync(int userId) =>
        RefreshTokenDataProvider.RevokeAllForUserAsync(userId);
}

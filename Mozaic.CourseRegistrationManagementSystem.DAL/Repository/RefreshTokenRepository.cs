using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class RefreshTokenRepository
{
    public Task AddAsync(RefreshToken entity) =>
        RefreshTokenDataProvider.AddAsync(entity);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash) =>
        RefreshTokenDataProvider.GetByHashAsync(tokenHash);

    public Task RevokeAsync(string tokenHash) =>
        RefreshTokenDataProvider.RevokeAsync(tokenHash);

    public Task RevokeAllForUserAsync(int userId) =>
        RefreshTokenDataProvider.RevokeAllForUserAsync(userId);
}

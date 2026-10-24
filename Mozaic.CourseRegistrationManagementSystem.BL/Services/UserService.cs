using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class UserService
{
    /// <summary>
    /// The single read path for users. Every filter is optional; a null
    /// filter means "no restriction". Pass a page number to get one page
    /// plus the counter, or leave it out to get every matching row.
    /// </summary>
    public static async Task<UserResult> GetAsync(
        int? id = null,
        string? userName = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        try
        {
            PagedResult<User>.Normalize(ref pageNumber, ref pageSize);

            var term = string.IsNullOrWhiteSpace(search) ? null : search!.Trim();

            var userManager = new UserManager();
            var page = await userManager.GetAsync(id, userName, term, pageNumber, pageSize);

            // A page past the end of the result set comes back empty; fall back
            // to the last page so the view never shows "Page 9 of 3".
            if (page.Count == 0 && page.TotalPages > 0 && page.PageNumber > page.TotalPages)
            {
                page = await userManager.GetAsync(id, userName, term, page.TotalPages, pageSize);
            }

            return page;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving users.", ex);
        }
    }

    /// <summary>
    /// Fetches a single user by id, or null when no such row exists.
    /// </summary>
    public static async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            var userManager = new UserManager();
            return await userManager.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving user.", ex);
        }
    }

    public static async Task AddAsync(User user, int? actorId = null)
    {
        try
        {
            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var unique = await EnsureUniqueUserNameAsync(user.UserName);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            user.CreatedBy = effectiveActorId;
            user.ModifiedBy = effectiveActorId;

            var userManager = new UserManager();
            await userManager.AddAsync(user);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public static async Task UpdateAsync(User user, int? actorId = null)
    {
        try
        {
            var userManager = new UserManager();
            var existingUser = await userManager.GetByIdAsync(user.Id);
            var exists = UserValidator.RequireExists(existingUser, user.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var unique = await EnsureUniqueUserNameAsync(user.UserName, user.Id);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = existingUser!.PasswordHash;
            }

            user.CreatedBy = existingUser!.CreatedBy;
            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? existingUser.ModifiedBy;
            user.ModifiedBy = effectiveActorId;

            await userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var userManager = new UserManager();
            var existingUser = await userManager.GetByIdAsync(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await userManager.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    private static async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var userManager = new UserManager();
        var existingUser = (await userManager.GetAsync(userName: userName)).FirstOrDefault();
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }
}

using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using System.Text.RegularExpressions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class UserService
{
    public static User? GetById(int id)
    {
        try
        {
            var userManager = new UserManager();
            return userManager.GetById(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving user.", ex);
        }
    }

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

    public static List<User> GetAll()
    {
        try
        {
            var userManager = new UserManager();
            return userManager.GetAll();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving users.", ex);
        }
    }

    public static async Task<List<User>> GetAllAsync()
    {
        try
        {
            var userManager = new UserManager();
            return await userManager.GetAllAsync();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving users.", ex);
        }
    }

    public static void Add(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var unique = EnsureUniqueUserNameSync(user.UserName);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            var actorId = SessionManager.Current?.UserId ?? 0;
            user.CreatedBy = actorId;
            user.ModifiedBy = actorId;

            var userManager = new UserManager();
            userManager.Add(user);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public static async Task AddAsync(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

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

            var actorId = SessionManager.Current?.UserId ?? 0;
            user.CreatedBy = actorId;
            user.ModifiedBy = actorId;

            var userManager = new UserManager();
            await userManager.AddAsync(user);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public static void Update(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            var userManager = new UserManager();
            var existingUser = userManager.GetById(user.Id);
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

            var unique = EnsureUniqueUserNameSync(user.UserName, user.Id);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = existingUser!.PasswordHash;
            }

            user.CreatedBy = existingUser!.CreatedBy;
            user.ModifiedBy = SessionManager.Current?.UserId ?? existingUser.ModifiedBy;

            userManager.Update(user);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public static async Task UpdateAsync(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

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
            user.ModifiedBy = SessionManager.Current?.UserId ?? existingUser.ModifiedBy;

            await userManager.UpdateAsync(user);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public static void Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            var userManager = new UserManager();
            var existingUser = userManager.GetById(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            userManager.Delete(id);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            var userManager = new UserManager();
            var existingUser = await userManager.GetByIdAsync(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await userManager.DeleteAsync(id);
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    public static List<User> Search(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<User>();
            }

            var userManager = new UserManager();
            var users = userManager.GetAll();
            return users
                .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                               Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching users.", ex);
        }
    }

    public static async Task<List<User>> SearchAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<User>();
            }

            var userManager = new UserManager();
            var users = await userManager.GetAllAsync();
            return users
                .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                               Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching users.", ex);
        }
    }

    private static ValidationResult EnsureUniqueUserNameSync(string userName, int? excludeUserId = null)
    {
        var userManager = new UserManager();
        var existing = userManager.GetAll()
            .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        return UserValidator.RequireUniqueUserName(
            existing != null && existing.Id != (excludeUserId ?? 0), userName);
    }

    private static async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var userManager = new UserManager();
        var existingUser = await userManager.GetByUserNameAsync(userName);
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }
}

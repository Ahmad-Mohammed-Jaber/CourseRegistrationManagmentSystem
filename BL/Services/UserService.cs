using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Session;
using System.Text.RegularExpressions;
using Shared.Logging;

namespace BL.Services;

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

    public static ValidationResult Add(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = EnsureUniqueUserNameSync(user.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var actorId = SessionManager.Current?.UserId ?? 0;
            user.CreatedBy = actorId;
            user.ModifiedBy = actorId;

            var userManager = new UserManager();
            userManager.Add(user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public static async Task<ValidationResult> AddAsync(User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = await EnsureUniqueUserNameAsync(user.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var actorId = SessionManager.Current?.UserId ?? 0;
            user.CreatedBy = actorId;
            user.ModifiedBy = actorId;

            var userManager = new UserManager();
            await userManager.AddAsync(user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public static ValidationResult Update(int id, User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var userManager = new UserManager();
            var existingUser = userManager.GetById(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = EnsureUniqueUserNameSync(user.UserName, id);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = existingUser!.PasswordHash;
            }

            user.CreatedBy = existingUser!.CreatedBy;
            user.ModifiedBy = SessionManager.Current?.UserId ?? existingUser.ModifiedBy;

            userManager.Update(id, user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public static async Task<ValidationResult> UpdateAsync(int id, User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var userManager = new UserManager();
            var existingUser = await userManager.GetByIdAsync(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = UserValidator.ValidateUser(user);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = await EnsureUniqueUserNameAsync(user.UserName, id);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = existingUser!.PasswordHash;
            }

            user.CreatedBy = existingUser!.CreatedBy;
            user.ModifiedBy = SessionManager.Current?.UserId ?? existingUser.ModifiedBy;

            await userManager.UpdateAsync(id, user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public static ValidationResult Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var userManager = new UserManager();
            var existingUser = userManager.GetById(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            userManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    public static async Task<ValidationResult> DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var userManager = new UserManager();
            var existingUser = await userManager.GetByIdAsync(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await userManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
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

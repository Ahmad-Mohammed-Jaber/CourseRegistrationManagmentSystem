using BL.Interfaces;
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
   

    public static User?  GetById(int id)
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

    public async Task<Result<User?>> GetByIdAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<User?>(auth.Status, auth.Errors, default);
            }

            return new Result<User?>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _userManager.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving user.", ex);
        }
    }

    public Result<List<User>> GetAll()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<User>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<User>>(ValidationStatus.Success, Array.Empty<ValidationError>(), _userManager.GetAll());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving users.", ex);
        }
    }

    public async Task<Result<List<User>>> GetAllAsync()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<User>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<User>>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _userManager.GetAllAsync());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving users.", ex);
        }
    }

    public ValidationResult Add(User user)
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

            _userManager.Add(user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public async Task<ValidationResult> AddAsync(User user)
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

            await _userManager.AddAsync(user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding user.", ex);
        }
    }

    public ValidationResult Update(int id, User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingUser = _userManager.GetById(id);
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

            _userManager.Update(id, user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public async Task<ValidationResult> UpdateAsync(int id, User user)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingUser = await _userManager.GetByIdAsync(id);
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

            await _userManager.UpdateAsync(id, user);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating user.", ex);
        }
    }

    public ValidationResult Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingUser = _userManager.GetById(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            _userManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    public async Task<ValidationResult> DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingUser = await _userManager.GetByIdAsync(id);
            var exists = UserValidator.RequireExists(existingUser, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await _userManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting user.", ex);
        }
    }

    public Result<List<User>> Search(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<User>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<User>>(pattern.Status, pattern.Errors, default);
            }

            var users = _userManager.GetAll();
            return new Result<List<User>>(ValidationStatus.Success, Array.Empty<ValidationError>(), users
                .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                               Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching users.", ex);
        }
    }

    public async Task<Result<List<User>>> SearchAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<User>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<User>>(pattern.Status, pattern.Errors, default);
            }

            var users = await _userManager.GetAllAsync();
            return new Result<List<User>>(ValidationStatus.Success, Array.Empty<ValidationError>(), users
                .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                               Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching users.", ex);
        }
    }

    private ValidationResult EnsureUniqueUserNameSync(string userName, int? excludeUserId = null)
    {
        var existing = _userManager.GetAll()
            .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        return UserValidator.RequireUniqueUserName(
            existing != null && existing.Id != (excludeUserId ?? 0), userName);
    }

    private async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var existingUser = await _userManager.GetByUserNameAsync(userName);
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }
}

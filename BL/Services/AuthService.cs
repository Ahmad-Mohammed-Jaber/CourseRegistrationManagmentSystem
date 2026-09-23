using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Session;
using Shared.Logging;
using static BCrypt.Net.BCrypt;

namespace BL.Services;

public class AuthService
{
    private readonly UserManager _userManager = new UserManager();

    public async Task<Result<User>> LoginAsync(string userName, string password)
    {
        try
        {
            var userNameCheck = UserValidator.ValidateUserName(userName);
            if (!userNameCheck.IsSuccess)
            {
                return new Result<User>(userNameCheck.Status, userNameCheck.Errors, default);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new Result<User>(
                    ValidationStatus.Invalid,
                    new[] { new ValidationError("Password", "Password is required.") },
                    default);
            }

            User? userRes = await _userManager.GetByUserNameAsync(userName);

            if (userRes == null)
            {
                return new Result<User>(
                    ValidationStatus.Unauthorized,
                    new[] { new ValidationError(string.Empty, "Invalid username or password.") },
                    default);
            }

            bool validPassword = Verify(password, userRes.PasswordHash);

            if (!validPassword)
            {
                return new Result<UserSession>(
                    ValidationStatus.Unauthorized,
                    new[] { new ValidationError(string.Empty, "Invalid username or password.") },
                    default);
            }

            UserSession userSession = new UserSession(userRes.Id, userRes.UserName, userRes.FullName, userRes.IsActive)
            {
                Role = userRes.Role,
            };

            return new Result<UserSession>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                userSession);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying LoginAsync()", ex);
        }
    }

    public async Task<ValidationResult> RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        try
        {
            bool hasUsers = (await _userManager.GetAllAsync()).Any();
            if (hasUsers)
            {
                var auth = AccessValidator.RequireAdmin();
                if (!auth.IsSuccess)
                {
                    return auth;
                }
            }
            var fields = UserValidator.ValidateAdminRegistration(userName, fullName, password);
            if (!fields.IsSuccess)
            {
                return fields;
            }

            if (await _userManager.GetByUserNameAsync(userName) != null)
            {
                return new ValidationResult(
                    ValidationStatus.Conflict,
                    new[] { new ValidationError(nameof(User.UserName), $"A user with username '{userName}' already exists.") });
            }

            string passwordHash = HashPassword(password);

            var actorId = SessionManager.Current?.UserId ?? 0;

            await _userManager.AddAsync(new User
            {
                UserName = userName,
                FullName = fullName,
                IsActive = isActive,
                PasswordHash = passwordHash,
                Role = User.UserRoles.Admin,
                CreatedBy = actorId,
                ModifiedBy = actorId
            });
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterAdminAsync()", ex);
        }
    }

    public async Task<ValidationResult> RegisterStudentAsync(string userName, int studentNumber, string fullName, bool isActive, string email, string phone, string password)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var fields = StudentValidator.ValidateStudentRegistration(
                userName, studentNumber, fullName, email, phone, password);
            if (!fields.IsSuccess)
            {
                return fields;
            }

            if (await _userManager.GetByUserNameAsync(userName) != null)
            {
                return new ValidationResult(
                    ValidationStatus.Conflict,
                    new[] { new ValidationError(nameof(User.UserName), $"A user with username '{userName}' already exists.") });
            }

            string passwordHash = HashPassword(password);
            var actorId = SessionManager.Current?.UserId ?? 0;
            User user = new User
            {
                UserName = userName,
                FullName = fullName,
                IsActive = isActive,
                PasswordHash = passwordHash,
                Role = User.UserRoles.Student,
                CreatedBy = actorId,
                ModifiedBy = actorId,
            };

            await _userManager.AddAsync(user);

            var createdUser = await _userManager.GetByUserNameAsync(userName);
            if (createdUser == null)
            {
                throw new BusinessException("Failed to create student user.");
            }

            Student student = new Student
            {
                UserId = createdUser.Id,
                Email = email,
                Phone = phone,
                StudentNumber = studentNumber,
                CreatedBy = actorId,
                ModifiedBy = actorId,
            };

            var studentManager = new StudentManager();
            try
            {
                await studentManager.AddAsync(student);
            }
            catch
            {
                try
                {
                    await _userManager.DeleteAsync(createdUser.Id);
                }
                catch (Exception ex)
                {
                    AppLogger.LogCaught(ex);
                }

                throw;
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterStudentAsync()", ex);
        }
    }
}

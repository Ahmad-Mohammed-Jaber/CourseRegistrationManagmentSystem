using static BCrypt.Net.BCrypt;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public class AuthService
{
    private readonly UserManager _userManager = new UserManager();

    public async Task<Result<UserSession>> LoginAsync(string userName, string password)
    {
        try
        {
            var userNameCheck = UserValidator.ValidateUserName(userName);
            if (!userNameCheck.IsSuccess)
            {
                return new Result<UserSession>(userNameCheck.Status, userNameCheck.Errors, default);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new Result<UserSession>(
                    ValidationStatus.Invalid,
                    new[] { new ValidationError("Password", "Password is required.") },
                    default);
            }

            User? userRes = await _userManager.GetByUserNameAsync(userName);

            if (userRes == null)
            {
                return new Result<UserSession>(
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

    public async Task<Result<User>> RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        try
        {
            bool hasUsers = (await _userManager.GetAllAsync()).Any();
            if (hasUsers)
            {
                var auth = AccessValidator.RequireAdmin();
                if (!auth.IsSuccess)
                {
                    return Result<User>.From(auth);
                }
            }
            var fields = UserValidator.ValidateAdminRegistration(userName, fullName, password);
            if (!fields.IsSuccess)
            {
                return Result<User>.From(fields);
            }

            if (await _userManager.GetByUserNameAsync(userName) != null)
            {
                return new Result<User>(
                    ValidationStatus.Conflict,
                    new[] { new ValidationError(nameof(User.UserName), $"A user with username '{userName}' already exists.") },
                    default);
            }

            string passwordHash = HashPassword(password);

            var actorId = SessionManager.Current?.UserId ?? 0;

            var user = new User
            {
                UserName = userName,
                FullName = fullName,
                IsActive = isActive,
                PasswordHash = passwordHash,
                Role = User.UserRoles.Admin,
                CreatedBy = actorId,
                ModifiedBy = actorId
            };
            int newId = await _userManager.AddAsync(user);
            if (newId <= 0)
            {
                return Result<User>.Fail(ValidationStatus.Conflict, $"Failed to create user: database reported no new id.");
            }
            user.Id = newId;
            return Result<User>.Ok(user);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterAdminAsync()", ex);
        }
    }

    public async Task<Result<Student>> RegisterStudentAsync(string userName, int studentNumber, string fullName, bool isActive, string email, string phone, string password)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Student>.From(auth);
            }

            var fields = StudentValidator.ValidateStudentRegistration(
                userName, studentNumber, fullName, email, phone, password);
            if (!fields.IsSuccess)
            {
                return Result<Student>.From(fields);
            }

            if (await _userManager.GetByUserNameAsync(userName) != null)
            {
                return new Result<Student>(
                    ValidationStatus.Conflict,
                    new[] { new ValidationError(nameof(User.UserName), $"A user with username '{userName}' already exists.") },
                    default);
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

            int userId = await _userManager.AddAsync(user);
            var createdUser = userId > 0
                ? await _userManager.GetByIdAsync(userId) ?? await _userManager.GetByUserNameAsync(userName)
                : await _userManager.GetByUserNameAsync(userName);
            if (createdUser == null)
            {
                throw new BusinessException("Failed to create student user.");
            }

            Student student = new Student
            {
                UserId = createdUser.Id,
                UserName = userName,
                FullName = fullName,
                IsActive = isActive,
                Email = email,
                Phone = phone,
                StudentNumber = studentNumber,
                CreatedBy = actorId,
                ModifiedBy = actorId,
            };

            var studentManager = new StudentManager();
            try
            {
                int studentId = await studentManager.AddAsync(student);
                if (studentId <= 0)
                {
                    return Result<Student>.Fail(ValidationStatus.Conflict, $"Failed to create student: database reported no new id.");
                }
                student.Id = studentId;
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
            return Result<Student>.Ok(student);
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

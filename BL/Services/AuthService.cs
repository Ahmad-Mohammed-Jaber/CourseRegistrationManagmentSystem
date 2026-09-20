using static BCrypt.Net.BCrypt;
using Shared.Session;
using Shared.Entities;
using Shared.Exceptions;

using BL.Managers;
using BL.Validation;
using Shared.Results;

namespace BL.Services;

public class AuthService
{
    private readonly UserManager _userManager = new UserManager();

    public async Task<LoginResult> LoginAsync(string userName, string password)
    {
        try
        {
            User? userRes = await _userManager.GetByUserNameAsync(userName);

            if (userRes == null)
            {
                return new LoginResult(LoginStatus.Invalid);
            }

            bool validPassword = Verify(password, userRes.PasswordHash);

            if (!validPassword)
            {
                return new LoginResult(LoginStatus.Invalid);
            }

            // Single UserSession for auth/authz only — no student data embedded.
            // Student profile is fetched on-demand via StudentManager/StudentService.
            var userSession = new UserSession(
                userRes.Id,
                userRes.UserName,
                userRes.FullName,
                userRes.IsActive,
                userRes.Role);

            return new LoginResult(LoginStatus.Success, userSession);
        }
        catch (BussinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BussinessException("An error occured while trying LoginAsync()", ex);
        }
    }

    public async Task RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        // Allow first admin seeding when no users exist
        bool hasUsers = (await _userManager.GetAllAsync()).Any();
        if (hasUsers) AccessValidator.RequireAdmin();
        AccessValidator.ValidateUserName(userName);
        AccessValidator.ValidateFullName(fullName);
        AccessValidator.ValidatePassword(password);

        if (await _userManager.GetByUserNameAsync(userName) != null)
        {
            throw new InvalidOperationException($"A user with username '{userName}' already exists.");
        }

        string passwordHash = HashPassword(password);

        await _userManager.AddAsync(new User
        {
            UserName = userName,
            FullName = fullName,
            IsActive = isActive,
            PasswordHash = passwordHash,
            Role = User.UserRoles.Admin
        });
    }

    public async Task RegisterStudentAsync(string userName, int studentNumber, string fullName, bool isActive, string email, string phone, string password)
    {
        AccessValidator.RequireAdmin();

        AccessValidator.ValidateUserName(userName);
        AccessValidator.ValidateFullName(fullName);
        AccessValidator.ValidatePassword(password);
        AccessValidator.ValidateStudentNumber(studentNumber);
        AccessValidator.ValidateEmail(email);
        AccessValidator.ValidatePhone(phone);

        if (await _userManager.GetByUserNameAsync(userName) != null)
        {
            throw new InvalidOperationException($"A user with username '{userName}' already exists.");
        }

        string passwordHash = HashPassword(password);
        User user = new User
        {
            UserName = userName,
            FullName = fullName,
            IsActive = isActive,
            PasswordHash = passwordHash,
            Role = User.UserRoles.Student,
        };

        await _userManager.AddAsync(user);

        var createdUser = await _userManager.GetByUserNameAsync(userName);
        if (createdUser == null) throw new InvalidOperationException("Failed to create student user.");

        Student student = new Student
        {
            UserId = createdUser.Id,
            Email = email,
            Phone = phone,
            StudentNumber = studentNumber,
        };

        var studentManager = new StudentManager();
        await studentManager.AddAsync(student);
    }
}

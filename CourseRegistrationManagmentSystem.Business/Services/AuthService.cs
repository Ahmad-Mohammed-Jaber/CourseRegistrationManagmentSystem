using static BCrypt.Net.BCrypt;
using Shared.Session;
using Shared.Entities;
using Shared.Exceptions;

using BL.Managers;
using BL.Validation;

public class AuthService
{
    private readonly UserManager _userManager = new UserManager();
    private readonly StudentManager _studentManager = new StudentManager();

                // Return should be flag: (Success, Failed, etc.)
    public async Task<(UserSession?, StudentSession?)> LoginAsync(string userName, string password)
    {
        try
        {
            User? userRes = await _userManager.GetByUserNameAsync(userName);

            if (userRes == null)
            {

                throw new InvalidOperationException("User not found!");
            }

            // Verify Hash
            bool validPassword = Verify(password, userRes.PasswordHash);

            if (!validPassword)
            {
                throw new InvalidOperationException("Incorrect password!");
            }

            UserSession? userSession = new UserSession(userRes.Id, userRes.UserName, userRes.FullName, userRes.IsActive)
            {
                Role = userRes.Role,
            };

            if (userRes.Role == User.UserRoles.Admin)
            {
                return (userSession, null);
            }

            Student? studentRes = await _studentManager.GetByUserIdAsync(userRes.Id);

            StudentSession? studentSession = studentRes == null
                ? null
                : new StudentSession(studentRes.Id, studentRes.UserId, studentRes.FullName, studentRes.StudentNumber, studentRes.Email, studentRes.Phone);

            return (userSession, studentSession);
        }
        catch (Exception ex)
        {
            // Use serilog here to log exceptions
            throw new BussinessException("An error occured while trying LoginAsync()");
        }
    }

    public async Task RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        AccessValidator.RequireAdmin();
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

        string passwordHash = HashPassword(password);
        User user = new User
        {
            UserName = userName,
            FullName = userName,
            IsActive = isActive,
            PasswordHash = passwordHash,
            Role = User.UserRoles.Student,
        };

        await _userManager.AddAsync(user);

        user = await _userManager.GetByUserNameAsync(userName);

        Student student = new Student
        {
            UserId = user!.Id,
            UserName = userName,
            FullName = fullName,
            Email = email,
            IsActive = isActive,
            Role = User.UserRoles.Student,
            Phone = phone,
            StudentNumber = studentNumber,
            PasswordHash = passwordHash
        };

        await _studentManager.AddAsync(student);
    }
}
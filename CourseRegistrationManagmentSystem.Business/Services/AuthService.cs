using CourseRegistrationManagmentSystem.Business.Managers;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;
using CourseRegistrationManagmentSystem.Business.Validation;


using static BCrypt.Net.BCrypt;

public class AuthService
{
    private readonly UserManager _userManager = new UserManager();
    private readonly StudentManager _studentManager = new StudentManager();

    public async Task<(UserSession?, StudentSession?)> LoginAsync(string userName, string password)
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
            Id = Guid.NewGuid(),
            UserName = userName,
            FullName = fullName,
            IsActive = isActive,
            PasswordHash = passwordHash,
            Role = User.UserRoles.Student,
        };

        await _userManager.AddAsync(user);

        Student student = new Student
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
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

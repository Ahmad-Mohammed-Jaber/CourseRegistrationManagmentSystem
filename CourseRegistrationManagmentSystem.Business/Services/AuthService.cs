using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;
using CourseRegistrationManagmentSystem.Business.Validation;

using static BCrypt.Net.BCrypt;

public class AuthService
{
    private readonly UserRepository _userRepository = new UserRepository();
    private readonly StudentRepository _studentRepository = new StudentRepository();

    public async Task<(UserSession?, StudentSession?)> LoginAsync(string userName, string password)
    {
        User? userRes = await _userRepository.GetByUserNameAsync(userName);

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

        Student? studentRes = await _studentRepository.GetByUserIdAsync(userRes.Id);

        StudentSession? studentSession = studentRes == null
            ? null
            : new StudentSession(studentRes.Id, studentRes.UserId, studentRes.FullName, studentRes.StudentNumber, studentRes.Email, studentRes.Phone);

        return (userSession, studentSession);
    }

    public async Task RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        AccessValidator.RequireAdmin();

        string passwordHash = HashPassword(password);

        await _userRepository.AddAsync(new User
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
        // Depending on requirements, student registration might be open or admin-only.
        // If it's admin-only, uncomment the line below:
        // AccessValidator.RequireAdmin();

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

        await _userRepository.AddAsync(user);

        Student student = new Student
        {
            Id = Guid.NewGuid() ,
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

        await _studentRepository.AddAsync(student);
    }
}

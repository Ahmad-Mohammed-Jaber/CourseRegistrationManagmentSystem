
using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;

using static BCrypt.Net.BCrypt;

public class AuthService
{
    private readonly UserRepository _userRepository = new UserRepository();
    private readonly StudentRepository _studentRepository = new StudentRepository();

    public async Task<(UserSession?, StudentSession?)> LoginAsync(string userName, string password)
    {
        User? userRes = await _userRepository.GetByUserNameAsync(userName);

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

        StudentRepository studentRepository = new StudentRepository();

        Student? studentRes = await studentRepository.GetByUserIdAsync(userRes.Id);

        StudentSession? studentSession = new StudentSession(studentRes.Id, studentRes.UserId, studentRes.FullName, studentRes.StudentNumber, studentRes.Email, studentRes.Phone);

        return (userSession, studentSession);
    }

    public async Task RegisterAdminAsync(string userName, string fullName, bool isActive, string password)
    {
        if (SessionManager.UserSession!.IsAdmin)
        {
            throw new InvalidOperationException("Only admins can register new users");
        }

        UserRepository userRepository = new UserRepository();

        string passwordHash = HashPassword(password);

        await userRepository.AddAsync(new User
        {
            UserName = userName,
            FullName = fullName,
            IsActive = isActive,
            PasswordHash = passwordHash,
        });
    }

    public async Task RegisterStudentAsync(string userName, int studentNumber, string fullName, bool isActive, string email, string phone, string password)
    {

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

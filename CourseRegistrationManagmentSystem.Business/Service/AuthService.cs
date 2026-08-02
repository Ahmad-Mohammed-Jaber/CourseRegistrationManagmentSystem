
using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;

using static BCrypt.Net.BCrypt;

public class AuthService
{
    public async Task<(UserSession?, StudentSession?)> Login(string userName, string password)
    {
        UserRepository userRepository = new UserRepository();

        User? userRes = await userRepository.GetByUserNameAsync(userName);

        // Verify Hash
        bool validPassword = Verify(password, userRes.PasswordHash);

        if (!validPassword) return (null, null);

        UserSession? userSession = new UserSession(userRes.Id, userRes.UserName, userRes.FullName, userRes.IsActive)
        {
            Role = userRes.Role,
        };

        if (userRes.Role == User.UserRoles.Admin)
        {
            return (userSession, null);
        }

        StudentRepository studentRepository = new StudentRepository();

        Student? student = await studentRepository.GetByUserIdAsync(userRes.Id);
    }

    public async Task RegisterAdmin(string userName, string fullName, bool isActive, string password)
    {
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

    public async RegisterStudent(string userName, string fullName, bool isActive, , string password)
    {
        UserRepository userRepository = new UserRepository();
        StudentRepository studentRepository = new StudentRepository();

         
    }
}

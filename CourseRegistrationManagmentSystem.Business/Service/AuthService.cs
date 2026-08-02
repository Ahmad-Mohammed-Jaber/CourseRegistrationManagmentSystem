
using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;
using Microsoft.AspNetCore.Identity;

public class AuthService
{
    public async Task<UserSession> Login(string userName, string password)
    {
        UserRepository userRepository = new UserRepository();

        User? res = await userRepository.GetByUserNameAsync(userName);

        PasswordHasher
    }

}

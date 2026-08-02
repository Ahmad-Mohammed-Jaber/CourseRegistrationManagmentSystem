namespace CourseRegistrationManagmentSystem.Shared.Models;
public class User
{
    public enum UserRoles
    {
        Admin,
        Student
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserName { get; set; }

    public string PasswordHash { get; set; }

    public string FullName { get; set; }

    public UserRoles Role { get; set; } = UserRoles.Admin;

    public bool IsActive { get; set; }
}

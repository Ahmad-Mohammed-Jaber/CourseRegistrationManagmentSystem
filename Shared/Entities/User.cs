namespace Shared.Entities;

public class User
{
    public enum UserRoles
    {
        Admin,
        Student
    }

    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public UserRoles Role { get; set; } = UserRoles.Student;

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }
}

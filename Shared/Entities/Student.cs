namespace Shared.Entities;

public class Student
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StudentNumber { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }

    public string UserName { get; set; } = string.Empty;

    // Transient (not a DB column): comes from JOIN to [User].FullName.
    // TablesV2 removed Student.FullName; identity lives in [User].
    public string FullName { get; set; } = string.Empty;

    // Transient (not a DB column): carried only for user creation / enrichment.
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public User.UserRoles Role { get; set; } = User.UserRoles.Student;
}

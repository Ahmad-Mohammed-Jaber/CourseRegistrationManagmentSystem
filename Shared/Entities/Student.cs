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

  // Transient user fields — populated via join/enrichment, not stored in Students table
  public string UserName { get; set; } = string.Empty;

  public string FullName { get; set; } = string.Empty;

  public string PasswordHash { get; set; } = string.Empty;

  public bool IsActive { get; set; } = true;

  public User.UserRoles Role { get; set; } = User.UserRoles.Student;
}

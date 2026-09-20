namespace Shared.Dtos;

using Shared.Entities;

public class StudentDto 
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StudentNumber { get; init; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // User fields
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public User.UserRoles Role { get; set; } = User.UserRoles.Student;
}

namespace CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;

public class UserDto
{
    public Guid Id { get; init; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public User.UserRoles Role { get; set; }
    public bool IsActive { get; set; }
}

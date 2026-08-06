namespace Shared.Dtos;

using Shared.Entities;

public class UserDto
{
    public int Id { get; init; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public User.UserRoles Role { get; set; }
    public bool IsActive { get; set; }
}
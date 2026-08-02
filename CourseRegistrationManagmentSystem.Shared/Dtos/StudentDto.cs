namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public class StudentDto : UserDto
{
    public int StudentNumber { get; init; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}

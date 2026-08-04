namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public class RegistrationDto
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public string? StudentUserName { get; set; }
    public Guid ClassId { get; init; }
    public string? ClassName { get; set; }
    public string? CourseName { get; set; }
    public DateTime RegistrationDate { get; init; }
    public required string Status { get; set; }
}

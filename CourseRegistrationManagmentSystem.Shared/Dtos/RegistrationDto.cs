namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public class RegistrationDto
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public Guid ClassId { get; init; }
    public DateTime RegistrationDate { get; init; }
    public required string Status { get; set; }
}

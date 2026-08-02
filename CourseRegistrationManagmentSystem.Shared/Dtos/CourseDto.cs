namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public class CourseDto
{
    public Guid Id { get; init; }
    public required string CourseCode { get; set; }
    public required string CourseName { get; set; }
    public double CreditHours { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

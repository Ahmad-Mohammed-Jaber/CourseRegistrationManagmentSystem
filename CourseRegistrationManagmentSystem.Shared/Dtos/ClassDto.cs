namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public class ClassDto
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public required string ClassName { get; set; }
    public required string Instructor { get; set; }
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek Schedule { get; set; }
    public bool IsActive { get; set; }
}

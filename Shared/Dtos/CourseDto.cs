namespace Shared.Dtos;

public class CourseDto
{
    public int Id { get; init; }
    public required string CourseCode { get; set; }
    public required string CourseName { get; set; }
    public double CreditHours { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }
}
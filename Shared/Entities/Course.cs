namespace Shared.Entities;

public class Course
{
    public int Id { get; set; }

    public string CourseCode { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public double CreditHours { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }
}

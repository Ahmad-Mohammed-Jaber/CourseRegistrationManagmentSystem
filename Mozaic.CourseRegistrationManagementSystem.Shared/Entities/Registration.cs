namespace Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

public class Registration
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }

    // Transient (not DB columns): display names populated from joins for grid display.
    public string? StudentUserName { get; set; }

    public string? ClassName { get; set; }

    public string? CourseName { get; set; }
}

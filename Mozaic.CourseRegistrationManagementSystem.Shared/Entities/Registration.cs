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

    public Class Class { get; set; } = new();

    public Student Student { get; set; } = new();

    public Course Course { get; set; } = new();

    public string CreatedByName { get; set; } = string.Empty;

    public string ModifiedByName { get; set; } = string.Empty;
}

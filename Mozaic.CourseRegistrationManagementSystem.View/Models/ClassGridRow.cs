namespace Mozaic.CourseRegistrationManagementSystem.View.Models;

public sealed class ClassGridRow
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Schedule { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsRegistered { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int CreatedBy { get; set; }

    public int ModifiedBy { get; set; }
}

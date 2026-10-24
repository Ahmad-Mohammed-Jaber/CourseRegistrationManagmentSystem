namespace Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

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

    /// <summary>
    /// Display name of the creator, filled in by the read procedures when the
    /// audit actor row is joined. Not a database column.
    /// </summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the last editor, filled in by the read procedures when
    /// the audit actor row is joined. Not a database column.
    /// </summary>
    public string ModifiedByName { get; set; } = string.Empty;
}

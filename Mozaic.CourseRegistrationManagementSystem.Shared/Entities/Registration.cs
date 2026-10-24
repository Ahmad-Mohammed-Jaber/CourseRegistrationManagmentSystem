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

    /// <summary>
    /// The class this registration is for, joined in by the read procedures.
    /// </summary>
    public Class Class { get; set; } = new();

    /// <summary>
    /// The student this registration belongs to, joined in by the read procedures.
    /// </summary>
    public Student Student { get; set; } = new();

    /// <summary>
    /// The course the registered class belongs to, joined in by the read procedures.
    /// </summary>
    public Course Course { get; set; } = new();

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

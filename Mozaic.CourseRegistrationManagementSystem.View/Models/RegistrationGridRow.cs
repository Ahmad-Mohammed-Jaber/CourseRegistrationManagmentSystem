using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.View.Models;

public sealed class RegistrationGridRow
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int CreatedBy { get; set; }

    public int ModifiedBy { get; set; }
}

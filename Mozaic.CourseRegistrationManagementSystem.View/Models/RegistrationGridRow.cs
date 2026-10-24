using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.View.Models;

/// <summary>
/// Grid row for the registration list: the registration columns plus the
/// display names of the related student, class and course. Those names come
/// from joins, not from [dbo].[Registrations], so they live here rather than
/// on the entity.
/// </summary>
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

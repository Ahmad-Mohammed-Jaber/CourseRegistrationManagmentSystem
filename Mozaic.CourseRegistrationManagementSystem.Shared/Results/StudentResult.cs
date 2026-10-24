using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// The read result for students: the generic pagination envelope over Student rows.
/// </summary>
public sealed class StudentResult : PagedResult<Student>
{
}

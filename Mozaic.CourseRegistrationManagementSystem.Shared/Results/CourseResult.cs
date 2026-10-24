using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// The read result for courses: the generic pagination envelope over Course rows.
/// </summary>
public sealed class CourseResult : PagedResult<Course>
{
}

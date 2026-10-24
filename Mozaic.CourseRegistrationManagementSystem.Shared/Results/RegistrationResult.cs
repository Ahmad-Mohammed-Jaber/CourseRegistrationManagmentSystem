using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// The read result for registrations: the generic pagination envelope over
/// Registration rows. Each row carries the joined class, student and course.
/// </summary>
public sealed class RegistrationResult : PagedResult<Registration>
{
}

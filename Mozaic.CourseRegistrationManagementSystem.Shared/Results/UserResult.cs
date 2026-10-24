using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// The read result for users: the generic pagination envelope over User rows.
/// </summary>
public sealed class UserResult : PagedResult<User>
{
}

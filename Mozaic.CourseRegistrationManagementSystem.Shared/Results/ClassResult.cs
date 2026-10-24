using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

public sealed class ClassResult : PagedResult<Class>
{
    public const string StatusActive = "Active";

    public const string StatusInactive = "Inactive";

    public const string StatusAll = "All";

    public int OpenSeatsCount { get; set; }

    public int MyRegistrationsCount { get; set; }
}

using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// The read result for classes: the generic pagination envelope over Class
/// rows plus the counters only the student browse page shows.
/// </summary>
public sealed class ClassResult : PagedResult<Class>
{
    /// <summary>Only active classes.</summary>
    public const string StatusActive = "Active";

    /// <summary>Only soft-deleted classes.</summary>
    public const string StatusInactive = "Inactive";

    /// <summary>Every class, active or not.</summary>
    public const string StatusAll = "All";

    /// <summary>Open seats across the whole filtered set, not just this page.</summary>
    public int OpenSeatsCount { get; set; }

    /// <summary>Registrations the querying student holds, across all classes.</summary>
    public int MyRegistrationsCount { get; set; }
}

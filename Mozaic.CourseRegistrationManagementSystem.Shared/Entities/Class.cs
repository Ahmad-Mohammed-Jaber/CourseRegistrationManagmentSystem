using System.ComponentModel;
using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

public class Class
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DaysOfTheWeek Schedule { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }

    public bool IsRegistered { get; set; }

    public Course Course { get; set; }
}

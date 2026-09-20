using System.ComponentModel;

namespace Shared.Entities;

public class Class
{
    [Flags]
    public enum DaysOfWeek
    {
        None = 0,
        Sunday = 2,
        Monday = 4,
        Tuesday = 8,
        Wednesday = 16,
        Thursday = 32,
        Friday = 64,
        Saturday = 128,
    }

    public int Id { get; set; }

    public int CourseId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DaysOfWeek Schedule { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public int ModifiedBy { get; set; }

    public int CreatedBy { get; set; }
}

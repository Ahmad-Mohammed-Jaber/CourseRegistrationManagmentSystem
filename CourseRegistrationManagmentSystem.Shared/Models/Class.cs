namespace CourseRegistrationManagmentSystem.Shared.Models;

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

    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string ClassName { get; set; }

    public string Instructor { get; set; }

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DaysOfWeek Schedule { get; set; }

    public bool IsActive { get; set; }
}

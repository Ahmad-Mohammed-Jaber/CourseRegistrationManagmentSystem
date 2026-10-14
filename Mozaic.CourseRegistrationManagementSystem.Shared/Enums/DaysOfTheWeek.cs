namespace Mozaic.CourseRegistrationManagementSystem.Shared.Enums;

[Flags]
public enum DaysOfTheWeek
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

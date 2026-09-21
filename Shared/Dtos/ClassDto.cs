namespace Shared.Dtos;

using Shared.Entities;
using Shared.Helpers;

public class ClassDto
{
    public int Id { get; init; }

    public int CourseId { get; init; }

    public required string ClassName { get; set; }

    public required string Instructor { get; set; }

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Class.DaysOfWeek Schedule { get; set; }

    public string ScheduleString => ScheduleHelper.GetScheduleString(Schedule);

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }
}
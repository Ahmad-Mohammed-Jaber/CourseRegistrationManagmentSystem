using System.ComponentModel;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;

namespace Mozaic.CourseRegistrationManagementSystem.View.Models;

public sealed class ClassBrowseRow
{
    public int Id { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public string Schedule { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int CurrentCapacity { get; set; }

    public int MaxCapacity { get; set; }

    public bool IsRegistered { get; set; }

    [Browsable(false)]
    public bool IsFull => CurrentCapacity >= MaxCapacity;

    [Browsable(false)]
    public int SeatPercent => MaxCapacity <= 0
        ? 100
        : Math.Clamp((int)Math.Round(CurrentCapacity * 100.0 / MaxCapacity), 0, 100);

    public ClassBrowseRow() { }

    public ClassBrowseRow(Class entity)
    {
        Id = entity.Id;
        ClassName = entity.ClassName;
        CourseName = entity.Course?.CourseName ?? string.Empty;
        Instructor = entity.Instructor;
        Schedule = ScheduleHelper.GetScheduleString(entity.Schedule);
        StartDate = entity.StartDate;
        EndDate = entity.EndDate;
        CurrentCapacity = entity.CurrentCapacity;
        MaxCapacity = entity.MaxCapacity;
        IsRegistered = entity.IsRegistered;
    }
}

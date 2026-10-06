using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class BrowseClassViewModel
{
    public int ClassId { get; set; }

    [Display(Name = "Course")]
    public string CourseName { get; set; } = string.Empty;

    [Display(Name = "Class")]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "Instructor")]
    public string Instructor { get; set; } = string.Empty;

    [Display(Name = "Schedule")]
    public string Schedule { get; set; } = string.Empty;

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Seats")]
    public int CurrentCapacity { get; set; }

    public int MaxCapacity { get; set; }

    public bool IsFull => CurrentCapacity >= MaxCapacity;

    public bool IsRegistered { get; set; }

    /// <summary>Seat fill 0-100 for the kiaalap progress bar. Guards MaxCapacity of zero.</summary>
    public int SeatPercent => MaxCapacity <= 0
        ? 100
        : Math.Clamp((int)Math.Round(CurrentCapacity * 100.0 / MaxCapacity), 0, 100);
}

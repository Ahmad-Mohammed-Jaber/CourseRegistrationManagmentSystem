using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class ClassFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Course is required.")]
    [Display(Name = "Course")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Class name is required.")]
    [Display(Name = "Class Name")]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "Instructor")]
    public string Instructor { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Max capacity must be between 1 and 1000.")]
    [Display(Name = "Max Capacity")]
    public int MaxCapacity { get; set; } = 1;

    [Display(Name = "Current Capacity")]
    public int CurrentCapacity { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Sunday")]
    public bool Sunday { get; set; }

    [Display(Name = "Monday")]
    public bool Monday { get; set; }

    [Display(Name = "Tuesday")]
    public bool Tuesday { get; set; }

    [Display(Name = "Wednesday")]
    public bool Wednesday { get; set; }

    [Display(Name = "Thursday")]
    public bool Thursday { get; set; }

    [Display(Name = "Friday")]
    public bool Friday { get; set; }

    [Display(Name = "Saturday")]
    public bool Saturday { get; set; }

    public List<SelectListItem> Courses { get; set; } = new();

    public DaysOfTheWeek ToSchedule()
    {
        var days = DaysOfTheWeek.None;
        if (Sunday) days |= DaysOfTheWeek.Sunday;
        if (Monday) days |= DaysOfTheWeek.Monday;
        if (Tuesday) days |= DaysOfTheWeek.Tuesday;
        if (Wednesday) days |= DaysOfTheWeek.Wednesday;
        if (Thursday) days |= DaysOfTheWeek.Thursday;
        if (Friday) days |= DaysOfTheWeek.Friday;
        if (Saturday) days |= DaysOfTheWeek.Saturday;
        return days;
    }

    public void FromSchedule(DaysOfTheWeek schedule)
    {
        Sunday = schedule.HasFlag(DaysOfTheWeek.Sunday);
        Monday = schedule.HasFlag(DaysOfTheWeek.Monday);
        Tuesday = schedule.HasFlag(DaysOfTheWeek.Tuesday);
        Wednesday = schedule.HasFlag(DaysOfTheWeek.Wednesday);
        Thursday = schedule.HasFlag(DaysOfTheWeek.Thursday);
        Friday = schedule.HasFlag(DaysOfTheWeek.Friday);
        Saturday = schedule.HasFlag(DaysOfTheWeek.Saturday);
    }
}

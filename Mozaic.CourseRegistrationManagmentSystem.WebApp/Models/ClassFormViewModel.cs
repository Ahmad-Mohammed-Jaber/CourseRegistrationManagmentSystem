using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

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

    [Display(Name = "Max Capacity")]
    public int MaxCapacity { get; set; }

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

    public Class.DaysOfWeek ToSchedule()
    {
        var days = Class.DaysOfWeek.None;
        if (Sunday) days |= Class.DaysOfWeek.Sunday;
        if (Monday) days |= Class.DaysOfWeek.Monday;
        if (Tuesday) days |= Class.DaysOfWeek.Tuesday;
        if (Wednesday) days |= Class.DaysOfWeek.Wednesday;
        if (Thursday) days |= Class.DaysOfWeek.Thursday;
        if (Friday) days |= Class.DaysOfWeek.Friday;
        if (Saturday) days |= Class.DaysOfWeek.Saturday;
        return days;
    }

    public void FromSchedule(Class.DaysOfWeek schedule)
    {
        Sunday = schedule.HasFlag(Class.DaysOfWeek.Sunday);
        Monday = schedule.HasFlag(Class.DaysOfWeek.Monday);
        Tuesday = schedule.HasFlag(Class.DaysOfWeek.Tuesday);
        Wednesday = schedule.HasFlag(Class.DaysOfWeek.Wednesday);
        Thursday = schedule.HasFlag(Class.DaysOfWeek.Thursday);
        Friday = schedule.HasFlag(Class.DaysOfWeek.Friday);
        Saturday = schedule.HasFlag(Class.DaysOfWeek.Saturday);
    }
}

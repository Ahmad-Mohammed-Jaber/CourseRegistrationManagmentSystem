using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class MyRegistrationViewModel
{
    [Display(Name = "Registration ID")]
    public int RegistrationId { get; set; }

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

    [Display(Name = "Registered Date")]
    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

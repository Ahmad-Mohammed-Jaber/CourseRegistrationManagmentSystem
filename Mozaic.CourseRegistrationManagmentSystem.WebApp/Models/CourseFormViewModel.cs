using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class CourseFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course code is required.")]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course name is required.")]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; } = string.Empty;

    [Display(Name = "Credit Hours")]
    public double CreditHours { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

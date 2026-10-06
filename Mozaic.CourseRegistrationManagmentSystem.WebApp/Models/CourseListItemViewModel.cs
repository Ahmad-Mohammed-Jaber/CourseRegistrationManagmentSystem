using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class CourseListItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Display(Name = "Course Name")]
    public string CourseName { get; set; } = string.Empty;

    [Display(Name = "Credit Hours")]
    public double CreditHours { get; set; }

    [Display(Name = "Status")]
    public bool IsActive { get; set; }
}

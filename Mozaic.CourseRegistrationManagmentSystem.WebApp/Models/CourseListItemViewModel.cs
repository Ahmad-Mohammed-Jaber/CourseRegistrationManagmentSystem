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

    [Display(Name = "Created At")]
    public DateTimeOffset CreatedOn { get; set; }

    [Display(Name = "Created By")]
    public string CreatedByName { get; set; } = string.Empty;

    [Display(Name = "Edited At")]
    public DateTimeOffset ModifiedOn { get; set; }

    [Display(Name = "Edited By")]
    public string ModifiedByName { get; set; } = string.Empty;
}

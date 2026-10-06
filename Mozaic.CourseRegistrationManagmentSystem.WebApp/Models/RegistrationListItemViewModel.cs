using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class RegistrationListItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Student")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "Course")]
    public string CourseName { get; set; } = string.Empty;

    [Display(Name = "Class")]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "Registered Date")]
    public DateTime RegistrationDate { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;
}

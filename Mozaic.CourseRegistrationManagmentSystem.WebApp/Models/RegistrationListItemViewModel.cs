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

    [Display(Name = "Created At")]
    public DateTimeOffset CreatedOn { get; set; }

    [Display(Name = "Created By")]
    public string CreatedByName { get; set; } = string.Empty;

    [Display(Name = "Edited At")]
    public DateTimeOffset ModifiedOn { get; set; }

    [Display(Name = "Edited By")]
    public string ModifiedByName { get; set; } = string.Empty;
}

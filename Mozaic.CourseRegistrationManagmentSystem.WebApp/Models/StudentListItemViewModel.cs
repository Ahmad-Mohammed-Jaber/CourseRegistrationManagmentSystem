using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class StudentListItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "Student #")]
    public int StudentNumber { get; set; }

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

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

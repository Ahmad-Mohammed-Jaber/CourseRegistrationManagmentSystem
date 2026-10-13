using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class ClassListItemViewModel
{
    public int Id { get; set; }

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
    public string Seats { get; set; } = string.Empty;

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

using System.ComponentModel.DataAnnotations;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class EditStudentViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student number is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid student number.")]
    [Display(Name = "Student #")]
    public int StudentNumber { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}

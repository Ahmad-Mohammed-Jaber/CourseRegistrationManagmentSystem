using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public class RegistrationFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Student is required.")]
    [Display(Name = "Student")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Class is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Class is required.")]
    [Display(Name = "Class")]
    public int ClassId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Registered";

    [Display(Name = "Registration Date")]
    [DataType(DataType.DateTime)]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    public List<SelectListItem> Students { get; set; } = new();

    public List<SelectListItem> Classes { get; set; } = new();

    public List<SelectListItem> Statuses { get; set; } = new()
    {
        new SelectListItem("Registered", "Registered"),
        new SelectListItem("Pending", "Pending"),
        new SelectListItem("Confirmed", "Confirmed"),
        new SelectListItem("Dropped", "Dropped"),
    };
}

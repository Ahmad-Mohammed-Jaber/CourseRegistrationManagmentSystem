using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

/// <summary>
/// Minimal protected landing pages proving the auth middleware.
/// Mirrors the WinForms AdminDashboard / StudentDashboard split.
/// </summary>
[Authorize]
public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction(nameof(Admin));
        }

        return RedirectToAction(nameof(Student));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Admin() => View();

    [HttpGet]
    [Authorize(Roles = "Student")]
    public IActionResult Student() => View();
}

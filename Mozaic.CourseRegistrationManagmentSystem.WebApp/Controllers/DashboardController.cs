using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

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

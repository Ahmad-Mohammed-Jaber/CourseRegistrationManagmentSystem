using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.WebApp;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

/// <summary>
/// Student self-service registrations.
/// MVC-standard routes: GET /Registrations/Mine, POST /Registrations/Drop.
/// All links use tag helpers (asp-controller/asp-action) and all redirects
/// use RedirectToAction — no hard-coded paths.
/// </summary>
[Authorize(Roles = "Student")]
public class RegistrationsController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Mine()
    {
        var userId = User.GetUserId();
        if (userId <= 0)
        {
            return Challenge();
        }

        try
        {
            var student = await StudentService.GetByUserIdAsync(userId);
            if (student is null)
            {
                ViewData["InfoMessage"] = "No student profile was found for your account. Contact an administrator.";
                return View(new List<MyRegistrationViewModel>());
            }

            var rows = await StudentRegistrationService.GetRegistrationsForUserAsync(userId);

            var model = new List<MyRegistrationViewModel>(rows.Count);
            foreach (var (registration, @class) in rows)
            {
                var courseName = string.Empty;
                try
                {
                    var course = await CourseService.GetByIdAsync(@class.CourseId);
                    courseName = course?.CourseName ?? string.Empty;
                }
                catch (BusinessException)
                {
                    // Best-effort enrichment only; grid still renders without it.
                }

                model.Add(new MyRegistrationViewModel
                {
                    RegistrationId = registration.Id,
                    ClassId = @class.Id,
                    CourseName = courseName,
                    ClassName = @class.ClassName,
                    Instructor = @class.Instructor,
                    Schedule = @class.ScheduleDisplay,
                    StartDate = @class.StartDate,
                    EndDate = @class.EndDate,
                    RegistrationDate = registration.RegistrationDate,
                    Status = registration.Status,
                });
            }

            return View(model);
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<MyRegistrationViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Drop(int id)
    {
        var userId = User.GetUserId();
        if (userId <= 0)
        {
            return Challenge();
        }

        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a registration to drop.";
            return RedirectToAction(nameof(Mine));
        }

        try
        {
            await StudentRegistrationService.DropRegistrationForUserAsync(userId, id);
            TempData["SuccessMessage"] = "Class dropped successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Mine));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.WebApp;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

/// <summary>
/// Student self-service class catalog.
/// MVC-standard routes: GET /Classes/Browse, POST /Classes/Register.
/// All links use tag helpers (asp-controller/asp-action) and all redirects
/// use RedirectToAction — no hard-coded paths.
/// </summary>
[Authorize(Roles = "Student")]
public class ClassesController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Browse()
    {
        var userId = User.GetUserId();
        if (userId <= 0)
        {
            return Challenge();
        }

        try
        {
            var classes = (await ClassService.GetAllAsync())
                .Where(c => c.IsActive)
                .OrderBy(c => c.StartDate)
                .ToList();

            // Best effort: without a student profile the grid still renders;
            // Register then surfaces the BL "Student login required." error neatly.
            var registeredClassIds = new HashSet<int>();
            try
            {
                var rows = await StudentRegistrationService.GetRegistrationsForUserAsync(userId);
                foreach (var (registration, _) in rows)
                {
                    registeredClassIds.Add(registration.ClassId);
                }
            }
            catch (BusinessException)
            {
                // Fall through with an empty set; the grid still renders.
            }

            var model = new List<BrowseClassViewModel>(classes.Count);
            foreach (var @class in classes)
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

                model.Add(new BrowseClassViewModel
                {
                    ClassId = @class.Id,
                    CourseName = courseName,
                    ClassName = @class.ClassName,
                    Instructor = @class.Instructor,
                    Schedule = @class.ScheduleDisplay,
                    StartDate = @class.StartDate,
                    EndDate = @class.EndDate,
                    CurrentCapacity = @class.CurrentCapacity,
                    MaxCapacity = @class.MaxCapacity,
                    IsRegistered = registeredClassIds.Contains(@class.Id),
                });
            }

            ViewData["AvailableCount"] = classes.Count;
            ViewData["OpenSeatsCount"] = classes.Sum(c => Math.Max(0, c.MaxCapacity - c.CurrentCapacity));
            ViewData["MyRegistrationsCount"] = registeredClassIds.Count;

            return View(model);
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<BrowseClassViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var userId = User.GetUserId();
        if (userId <= 0)
        {
            return Challenge();
        }

        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a class to register for.";
            return RedirectToAction(nameof(Browse));
        }

        try
        {
            await StudentRegistrationService.RegisterClassForUserAsync(userId, id);
            TempData["SuccessMessage"] = "Registered successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Browse));
    }
}

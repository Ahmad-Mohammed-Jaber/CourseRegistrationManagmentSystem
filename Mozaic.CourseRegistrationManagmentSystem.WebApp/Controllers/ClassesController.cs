using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

public class ClassesController : Controller
{
    [HttpGet]
    [Authorize(Roles = "Student")]
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

            // map using automapper and extend model for reg

            var model = new List<BrowseClassViewModel>(classes.Count);
            foreach (var @class in classes)
            {
                // sp should return course name, is registered/ not per user, 
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
    [Authorize(Roles = "Student")]
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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string? q = null)
    {
        ViewData["Query"] = q ?? string.Empty;

        try
        {
            var classes = string.IsNullOrWhiteSpace(q)
                ? await ClassService.GetAllAsync()
                : await ClassService.SearchAsync(q);

            var model = new List<ClassListItemViewModel>(classes.Count);
            var userNames = await GetUserNamesAsync();
            foreach (var @class in classes.OrderBy(c => c.StartDate))
            {
                var courseName = string.Empty;
                try
                {
                    var course = await CourseService.GetByIdAsync(@class.CourseId);
                    courseName = course is null
                        ? string.Empty
                        : $"{course.CourseCode} {course.CourseName}".Trim();
                }
                catch (BusinessException)
                {
                    // Best-effort enrichment only; grid still renders without it.
                }

                model.Add(new ClassListItemViewModel
                {
                    Id = @class.Id,
                    CourseName = courseName,
                    ClassName = @class.ClassName,
                    Instructor = @class.Instructor,
                    Schedule = @class.ScheduleDisplay,
                    StartDate = @class.StartDate,
                    EndDate = @class.EndDate,
                    Seats = $"{@class.CurrentCapacity}/{@class.MaxCapacity}",
                    IsActive = @class.IsActive,
                    CreatedOn = @class.CreatedOn,
                    CreatedByName = ResolveUserName(userNames, @class.CreatedBy),
                    ModifiedOn = @class.ModifiedOn,
                    ModifiedByName = ResolveUserName(userNames, @class.ModifiedBy),
                });
            }

            return View(model);
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<ClassListItemViewModel>());
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new ClassFormViewModel();
        await PopulateCoursesAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCoursesAsync(model);
            return View(model);
        }

        try
        {
            await ClassService.AddAsync(new Class
            {
                CourseId = model.CourseId,
                ClassName = model.ClassName.Trim(),
                Instructor = model.Instructor.Trim(),
                MaxCapacity = model.MaxCapacity,
                CurrentCapacity = 0,
                Schedule = model.ToSchedule(),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
            }, User.GetUserId());

            TempData["SuccessMessage"] = $"Class '{model.ClassName.Trim()}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateCoursesAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a class to edit.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var @class = await ClassService.GetByIdAsync(id);
            if (@class is null)
            {
                TempData["ErrorMessage"] = $"Class with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ClassFormViewModel
            {
                Id = @class.Id,
                CourseId = @class.CourseId,
                ClassName = @class.ClassName,
                Instructor = @class.Instructor,
                MaxCapacity = @class.MaxCapacity,
                StartDate = @class.StartDate,
                EndDate = @class.EndDate,
                IsActive = @class.IsActive,
            };
            model.FromSchedule(@class.Schedule);
            await PopulateCoursesAsync(model);
            return View(model);
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClassFormViewModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            await PopulateCoursesAsync(model);
            return View(model);
        }

        try
        {
            var existing = await ClassService.GetByIdAsync(id);
            if (existing is null)
            {
                TempData["ErrorMessage"] = $"Class with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            await ClassService.UpdateAsync(new Class
            {
                Id = id,
                CourseId = model.CourseId,
                ClassName = model.ClassName.Trim(),
                Instructor = model.Instructor.Trim(),
                MaxCapacity = model.MaxCapacity,
                CurrentCapacity = existing.CurrentCapacity,
                Schedule = model.ToSchedule(),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
            }, User.GetUserId());

            TempData["SuccessMessage"] = $"Class '{model.ClassName.Trim()}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateCoursesAsync(model);
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a class to delete.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await ClassService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Class deleted successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static async Task PopulateCoursesAsync(ClassFormViewModel model)
    {
        try
        {
            var courses = await CourseService.GetAllAsync();
            model.Courses = courses
                .OrderBy(c => c.CourseCode)
                .Select(c => new SelectListItem($"{c.CourseCode} {c.CourseName}".Trim(), c.Id.ToString()))
                .ToList();
        }
        catch (BusinessException)
        {
            model.Courses = new List<SelectListItem>();
        }
    }

    private static async Task<Dictionary<int, string>> GetUserNamesAsync()
    {
        try
        {
            var users = await UserService.GetAllAsync();
            return users.ToDictionary(u => u.Id, u => u.UserName);
        }
        catch (BusinessException)
        {
            return new Dictionary<int, string>();
        }
    }

    private static string ResolveUserName(Dictionary<int, string> userNames, int userId)
    {
        if (userNames.TryGetValue(userId, out var name) && !string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        return userId <= 0 ? "—" : $"#{userId}";
    }
}

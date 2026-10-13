using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;
public class RegistrationsController : Controller
{
    [HttpGet]
    [Authorize(Roles = "Student")]
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
    [Authorize(Roles = "Student")]
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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string? q = null)
    {
        ViewData["Query"] = q ?? string.Empty;

        try
        {
            var rows = string.IsNullOrWhiteSpace(q)
                ? await RegistrationService.GetAllDetailedAsync()
                : await RegistrationService.SearchDetailedAsync(q);

            var userNames = await GetUserNamesAsync();

            return View(rows
                .OrderByDescending(r => r.Registration.RegistrationDate)
                .Select(r => new RegistrationListItemViewModel
                {
                    Id = r.Registration.Id,
                    StudentName = $"{r.Student.FullName} ({r.Student.UserName})",
                    CourseName = r.Course.CourseName,
                    ClassName = r.Class.ClassName,
                    RegistrationDate = r.Registration.RegistrationDate,
                    Status = r.Registration.Status,
                    CreatedOn = r.Registration.CreatedOn,
                    CreatedByName = ResolveUserName(userNames, r.Registration.CreatedBy),
                    ModifiedOn = r.Registration.ModifiedOn,
                    ModifiedByName = ResolveUserName(userNames, r.Registration.ModifiedBy),
                })
                .ToList());
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<RegistrationListItemViewModel>());
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new RegistrationFormViewModel();
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegistrationFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        try
        {
            await RegistrationService.AddAsync(new Registration
            {
                StudentId = model.StudentId,
                ClassId = model.ClassId,
                Status = model.Status,
                RegistrationDate = model.RegistrationDate,
            }, User.GetUserId());

            TempData["SuccessMessage"] = "Registration created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateOptionsAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a registration to edit.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var registration = await RegistrationService.GetByIdAsync(id);
            if (registration is null)
            {
                TempData["ErrorMessage"] = $"Registration with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new RegistrationFormViewModel
            {
                Id = registration.Id,
                StudentId = registration.StudentId,
                ClassId = registration.ClassId,
                Status = string.IsNullOrWhiteSpace(registration.Status) ? "Registered" : registration.Status,
                RegistrationDate = registration.RegistrationDate,
            };
            await PopulateOptionsAsync(model);
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
    public async Task<IActionResult> Edit(int id, RegistrationFormViewModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        try
        {
            var existing = await RegistrationService.GetByIdAsync(id);
            if (existing is null)
            {
                TempData["ErrorMessage"] = $"Registration with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            await RegistrationService.UpdateAsync(new Registration
            {
                Id = id,
                StudentId = model.StudentId,
                ClassId = model.ClassId,
                Status = model.Status,
                RegistrationDate = model.RegistrationDate,
            }, User.GetUserId());

            TempData["SuccessMessage"] = "Registration updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateOptionsAsync(model);
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
            TempData["ErrorMessage"] = "Please select a registration to delete.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await RegistrationService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Registration deleted successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static async Task PopulateOptionsAsync(RegistrationFormViewModel model)
    {
        try
        {
            var students = await StudentService.GetAllAsync();
            model.Students = students
                .OrderBy(s => s.FullName)
                .Select(s => new SelectListItem($"{s.FullName} ({s.UserName})", s.Id.ToString()))
                .ToList();
        }
        catch (BusinessException)
        {
            model.Students = new List<SelectListItem>();
        }

        try
        {
            var classes = await ClassService.GetAllAsync();
            model.Classes = classes
                .OrderBy(c => c.ClassName)
                .Select(c => new SelectListItem(c.ClassName, c.Id.ToString()))
                .ToList();
        }
        catch (BusinessException)
        {
            model.Classes = new List<SelectListItem>();
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

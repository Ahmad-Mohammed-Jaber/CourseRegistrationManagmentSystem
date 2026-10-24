using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

public class ClassesController : Controller
{
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Browse(int page = 1, int pageSize = 20)
    {
        var userId = User.GetUserId();
        if (userId <= 0)
        {
            return Challenge();
        }

        try
        {
            int? studentId = null;
            try
            {
                studentId = await StudentRegistrationService.GetStudentIdForUserAsync(userId);
            }
            catch (Exception)
            {
                // Rendering empty grid when empty stuff
            }

            var paged = await ClassService.GetAsync(userId: studentId, statusFilter: ClassResult.StatusActive, pageNumber: page, pageSize: pageSize);

            var model = BrowseClassesViewModel.FromResult(paged);

            return View(model);
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(BrowseClassesViewModel.Empty(page, pageSize));
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
    public async Task<IActionResult> Index(string? q = null, int page = 1, int pageSize = 20, string? status = null)
    {
        var statusFilter = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!;

        try
        {
            var result = await ClassService.GetAsync(search: q, statusFilter: statusFilter, pageNumber: page, pageSize: pageSize);

            return View(ClassListViewModel.FromResult(result, q, statusFilter));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(ClassListViewModel.Empty(page, pageSize, q, statusFilter));
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
                CurrentCapacity = @class.CurrentCapacity,
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

        try
        {
            var existing = await ClassService.GetByIdAsync(id);
            if (existing is null)
            {
                TempData["ErrorMessage"] = $"Class with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            // Current capacity is display-only; always show the stored enrollment.
            model.CurrentCapacity = existing.CurrentCapacity;

            if (!ModelState.IsValid)
            {
                await PopulateCoursesAsync(model);
                return View(model);
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
            var courses = await CourseService.GetAsync();
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
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class CoursesController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? q = null)
    {
        ViewData["Query"] = q ?? string.Empty;

        try
        {
            var courses = string.IsNullOrWhiteSpace(q)
                ? await CourseService.GetAllAsync()
                : await CourseService.SearchAsync(q);

            return View(courses
                .OrderBy(c => c.CourseCode)
                .Select(c => new CourseListItemViewModel
                {
                    Id = c.Id,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    CreditHours = c.CreditHours,
                    IsActive = c.IsActive,
                })
                .ToList());
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<CourseListItemViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create() => View(new CourseFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await CourseService.AddAsync(new Course
            {
                CourseCode = model.CourseCode.Trim(),
                CourseName = model.CourseName.Trim(),
                CreditHours = model.CreditHours,
                Description = model.Description?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
            });

            TempData["SuccessMessage"] = $"Course '{model.CourseCode.Trim()}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a course to edit.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var course = await CourseService.GetByIdAsync(id);
            if (course is null)
            {
                TempData["ErrorMessage"] = $"Course with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CourseFormViewModel
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                CreditHours = course.CreditHours,
                Description = course.Description,
                IsActive = course.IsActive,
            });
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var existing = await CourseService.GetByIdAsync(id);
            if (existing is null)
            {
                TempData["ErrorMessage"] = $"Course with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            await CourseService.UpdateAsync(new Course
            {
                Id = id,
                CourseCode = model.CourseCode.Trim(),
                CourseName = model.CourseName.Trim(),
                CreditHours = model.CreditHours,
                Description = model.Description?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
            });

            TempData["SuccessMessage"] = $"Course '{model.CourseCode.Trim()}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a course to delete.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await CourseService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Course deleted successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

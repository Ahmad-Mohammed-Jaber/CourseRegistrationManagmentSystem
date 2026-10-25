using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

public class ClassesController : Controller
{
    private readonly IMapper _mapper;

    public ClassesController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Browse(
        string? course = null,
        string? className = null,
        string? instructor = null,
        int page = 1,
        int pageSize = 20)
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

            var paged = await ClassService.GetAsync(
                userId: studentId,
                statusFilter: ClassResult.StatusActive,
                classNameFilter: className,
                instructorFilter: instructor,
                courseFilter: course,
                pageNumber: page,
                pageSize: pageSize);

            var model = BrowseClassesViewModel.FromResult(paged, _mapper, course, className, instructor, pageSize);

            return View(model);
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(BrowseClassesViewModel.Empty(page, pageSize, course, className, instructor));
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
    public async Task<IActionResult> Index(
        string? className = null,
        string? instructor = null,
        string? course = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        var statusFilter = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!;

        try
        {
            var result = await ClassService.GetAsync(
                statusFilter: statusFilter,
                classNameFilter: className,
                instructorFilter: instructor,
                courseFilter: course,
                pageNumber: page,
                pageSize: pageSize);

            return View(ClassListViewModel.FromResult(result, _mapper, className, instructor, course, statusFilter, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(ClassListViewModel.Empty(page, pageSize, className, instructor, course, statusFilter));
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
            await ClassService.AddAsync(_mapper.Map<Class>(model), User.GetUserId());

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

            var model = _mapper.Map<ClassFormViewModel>(@class);
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

            var entity = _mapper.Map<Class>(model);
            entity.Id = id;
            entity.CurrentCapacity = existing.CurrentCapacity;

            await ClassService.UpdateAsync(entity, User.GetUserId());

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

    private async Task PopulateCoursesAsync(ClassFormViewModel model)
    {
        try
        {
            var courses = await CourseService.GetAsync();
            model.Courses = _mapper.Map<List<SelectListItem>>(courses
                .OrderBy(c => c.CourseCode)
                .ToList());
        }
        catch (BusinessException)
        {
            model.Courses = new List<SelectListItem>();
        }
    }
}

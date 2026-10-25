using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class CoursesController : Controller
{
    private readonly IMapper _mapper;

    public CoursesController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? code = null,
        string? name = null,
        string? description = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var result = await CourseService.GetAsync(
                codeFilter: code,
                nameFilter: name,
                descriptionFilter: description,
                isActive: FilterValues.ParseActive(status),
                pageNumber: page,
                pageSize: pageSize);

            return View(CourseListViewModel.FromResult(result, _mapper, code, name, description, status, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(CourseListViewModel.Empty(page, pageSize, code, name, description, status));
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
            await CourseService.AddAsync(_mapper.Map<Course>(model), User.GetUserId());

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

            return View(_mapper.Map<CourseFormViewModel>(course));
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

            var entity = _mapper.Map<Course>(model);
            entity.Id = id;

            await CourseService.UpdateAsync(entity, User.GetUserId());

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

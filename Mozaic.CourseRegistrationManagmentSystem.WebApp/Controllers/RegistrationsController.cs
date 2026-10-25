using AutoMapper;
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
    private readonly IMapper _mapper;

    public RegistrationsController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Mine(
        string? course = null,
        string? className = null,
        string? status = null,
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
            var student = await StudentService.GetByUserIdAsync(userId);
            if (student is null)
            {
                ViewData["InfoMessage"] = "No student profile was found for your account. Contact an administrator.";
                return View(MyRegistrationsListViewModel.Empty(page, pageSize, course, className, status));
            }

            var rows = await RegistrationService.GetAsync(
                studentId: student.Id,
                courseFilter: course,
                classFilter: className,
                statusFilter: status,
                pageNumber: page,
                pageSize: pageSize);

            return View(MyRegistrationsListViewModel.FromResult(rows, _mapper, course, className, status, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(MyRegistrationsListViewModel.Empty(page, pageSize, course, className, status));
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
    public async Task<IActionResult> Index(
        string? student = null,
        string? course = null,
        string? className = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var result = await RegistrationService.GetAsync(
                studentFilter: student,
                courseFilter: course,
                classFilter: className,
                statusFilter: status,
                pageNumber: page,
                pageSize: pageSize);

            return View(RegistrationListViewModel.FromResult(result, _mapper, student, course, className, status, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(RegistrationListViewModel.Empty(page, pageSize, student, course, className, status));
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
            await RegistrationService.AddAsync(_mapper.Map<Registration>(model), User.GetUserId());

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

            var model = _mapper.Map<RegistrationFormViewModel>(registration);
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
            // No pre-read: UpdateAsync validates existence and throws BusinessException
            // when the row is missing, so the redundant GetByIdAsync (and its extra query)
            // is dropped. The "not found" message is now surfaced via the catch below.
            var entity = _mapper.Map<Registration>(model);
            entity.Id = id;

            await RegistrationService.UpdateAsync(entity, User.GetUserId());

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

    private async Task PopulateOptionsAsync(RegistrationFormViewModel model)
    {
        try
        {
            var students = await StudentService.GetAsync();
            model.Students = _mapper.Map<List<SelectListItem>>(students
                .OrderBy(s => s.FullName)
                .ToList());
        }
        catch (BusinessException)
        {
            model.Students = new List<SelectListItem>();
        }

        try
        {
            var classes = await ClassService.GetAsync();
            model.Classes = _mapper.Map<List<SelectListItem>>(classes
                .OrderBy(c => c.ClassName)
                .ToList());
        }
        catch (BusinessException)
        {
            model.Classes = new List<SelectListItem>();
        }
    }
}
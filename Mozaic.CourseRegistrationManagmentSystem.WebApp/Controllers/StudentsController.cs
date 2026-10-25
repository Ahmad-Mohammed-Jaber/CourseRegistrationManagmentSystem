using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;
using UserEntity = Mozaic.CourseRegistrationManagementSystem.Shared.Entities.User;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class StudentsController : Controller
{
    private readonly AuthService _authService;
    private readonly IMapper _mapper;

    public StudentsController(AuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? studentNumber = null,
        string? fullName = null,
        string? userName = null,
        string? email = null,
        string? phone = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var result = await StudentService.GetAsync(
                studentNumberFilter: studentNumber,
                fullNameFilter: fullName,
                userNameFilter: userName,
                emailFilter: email,
                phoneFilter: phone,
                isActive: FilterValues.ParseActive(status),
                pageNumber: page,
                pageSize: pageSize);

            return View(StudentListViewModel.FromResult(
                result, _mapper, studentNumber, fullName, userName, email, phone, status, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(StudentListViewModel.Empty(
                page, pageSize, studentNumber, fullName, userName, email, phone, status));
        }
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateStudentViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterStudentAsync(
            model.UserName.Trim(), model.StudentNumber, model.FullName.Trim(),
            model.IsActive, model.Email.Trim(), model.Phone.Trim(), model.Password, User.GetUserId());

        if (!result.IsSuccess || result.Value is null)
        {
            AddResultErrors(result.Errors, model);
            return View(model);
        }

        TempData["SuccessMessage"] = $"Student '{model.UserName.Trim()}' enrolled successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a student to edit.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var student = await StudentService.GetByIdAsync(id);
            if (student is null)
            {
                TempData["ErrorMessage"] = $"Student with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_mapper.Map<EditStudentViewModel>(student));
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditStudentViewModel model)
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
            var entity = _mapper.Map<Student>(model);
            entity.Id = id;
            entity.Role = UserEntity.UserRoles.Student;

            await StudentService.UpdateAsync(entity, User.GetUserId());

            TempData["SuccessMessage"] = $"Student '{model.UserName.Trim()}' updated successfully.";
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
            TempData["ErrorMessage"] = "Please select a student to delete.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await StudentService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Student and linked login account deleted successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void AddResultErrors(
        IReadOnlyList<ValidationError> errors, CreateStudentViewModel model)
    {
        var props = new[]
        {
            nameof(model.UserName), nameof(model.StudentNumber), nameof(model.FullName),
            nameof(model.Email), nameof(model.Phone), nameof(model.Password),
        };

        foreach (var error in errors)
        {
            var key = props.FirstOrDefault(p =>
                string.Equals(p, error.Field, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
            ModelState.AddModelError(key, error.Message);
        }

        if (errors.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Could not enroll the student.");
        }
    }
}
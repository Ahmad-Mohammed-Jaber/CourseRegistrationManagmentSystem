using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly AuthService _authService;

    public UsersController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? userName = null,
        string? fullName = null,
        string? role = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var result = await UserService.GetAsync(
                userNameFilter: userName,
                fullNameFilter: fullName,
                role: FilterValues.ParseRole(role),
                isActive: FilterValues.ParseActive(status),
                pageNumber: page,
                pageSize: pageSize);

            return View(UserListViewModel.FromResult(result, userName, fullName, role, status, pageSize));
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(UserListViewModel.Empty(page, pageSize, userName, fullName, role, status));
        }
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateUserViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterAdminAsync(
            model.UserName.Trim(), model.FullName.Trim(), model.IsActive, model.Password, User.GetUserId());

        if (!result.IsSuccess || result.Value is null)
        {
            AddResultErrors(result.Errors, model);
            return View(model);
        }

        TempData["SuccessMessage"] = $"User '{result.Value.UserName}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Please select a user to edit.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var user = await UserService.GetByIdAsync(id);
            if (user is null)
            {
                TempData["ErrorMessage"] = $"User with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
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
    public async Task<IActionResult> Edit(int id, EditUserViewModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (id == User.GetUserId() && !model.IsActive)
        {
            ModelState.AddModelError(nameof(model.IsActive), "You cannot deactivate your own account.");
            return View(model);
        }

        try
        {
            // No pre-read: Role is a non-editable field that UpdateAsync preserves
            // server-side (like PasswordHash when empty), so the controller no longer
            // loads the row just to copy Role back into the payload. Existence is
            // validated by UpdateAsync, which throws BusinessException when missing.
            await UserService.UpdateAsync(new User
            {
                Id = id,
                UserName = model.UserName.Trim(),
                FullName = model.FullName.Trim(),
                // Role intentionally omitted — not editable, preserved by UpdateAsync.
                IsActive = model.IsActive,
                PasswordHash = string.Empty, // Preserved by the service when empty.
            }, User.GetUserId());

            TempData["SuccessMessage"] = $"User '{model.UserName.Trim()}' updated successfully.";
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
            TempData["ErrorMessage"] = "Please select a user to delete.";
            return RedirectToAction(nameof(Index));
        }

        if (id == User.GetUserId())
        {
            TempData["ErrorMessage"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await UserService.DeleteAsync(id);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void AddResultErrors(
        IReadOnlyList<ValidationError> errors, CreateUserViewModel model)
    {
        foreach (var error in errors)
        {
            var key = string.IsNullOrWhiteSpace(error.Field) ? string.Empty
                : string.Equals(error.Field, nameof(model.UserName), StringComparison.OrdinalIgnoreCase) ? nameof(model.UserName)
                : string.Equals(error.Field, nameof(model.FullName), StringComparison.OrdinalIgnoreCase) ? nameof(model.FullName)
                : string.Equals(error.Field, "Password", StringComparison.OrdinalIgnoreCase) ? nameof(model.Password)
                : string.Empty;
            ModelState.AddModelError(key, error.Message);
        }

        if (errors.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Could not create the user.");
        }
    }
}
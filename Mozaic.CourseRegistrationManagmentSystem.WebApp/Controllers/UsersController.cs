using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.WebApp;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly AuthService _authService;

    public UsersController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q = null)
    {
        ViewData["Query"] = q ?? string.Empty;

        try
        {
            var users = string.IsNullOrWhiteSpace(q)
                ? await UserService.GetAllAsync()
                : await UserService.SearchAsync(q);

            return View(users
                .OrderBy(u => u.UserName)
                .Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                })
                .ToList());
        }
        catch (BusinessException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(new List<UserListItemViewModel>());
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
            model.UserName.Trim(), model.FullName.Trim(), model.IsActive, model.Password);

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
            var existing = await UserService.GetByIdAsync(id);
            if (existing is null)
            {
                TempData["ErrorMessage"] = $"User with id {id} was not found.";
                return RedirectToAction(nameof(Index));
            }

            await UserService.UpdateAsync(new User
            {
                Id = id,
                UserName = model.UserName.Trim(),
                FullName = model.FullName.Trim(),
                Role = existing.Role,
                IsActive = model.IsActive,
                PasswordHash = string.Empty, // Preserved by the service when empty.
            });

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

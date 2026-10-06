using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

public class AuthController : Controller
{
    private AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model.UserName.Trim(), model.Password);

        if (!result.IsSuccess || result.Value is null)
        {
            foreach (var error in result.Errors)
            {
                var key = string.IsNullOrWhiteSpace(error.Field) ||
                          string.Equals(error.Field, nameof(model.UserName), StringComparison.OrdinalIgnoreCase)
                    ? nameof(model.UserName)
                    : error.Field;
                ModelState.AddModelError(key, error.Message);
            }

            if (result.Errors.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        var session = result.Value;

        if (!session.IsActive)
        {
            ModelState.AddModelError(string.Empty, "This account is disabled. Contact an administrator.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.UserId.ToString()),
            new(ClaimTypes.Name, session.UserName),
            new(ClaimsPrincipalExtensions.FullNameClaimType, session.FullName),
            new(ClaimTypes.Role, session.Role.ToString()),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = model.RememberMe });

        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Login));

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        if (User.IsInRole(Mozaic.CourseRegistrationManagementSystem.Shared.Entities.User.UserRoles.Admin.ToString()))
        {
            return RedirectToAction("Admin", "Dashboard");
        }

        if (User.IsInRole(Mozaic.CourseRegistrationManagementSystem.Shared.Entities.User.UserRoles.Student.ToString()))
        {
            return RedirectToAction("Student", "Dashboard");
        }

        return RedirectToAction("Index", "Dashboard");
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.WebApp;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(AuthService authService, JwtTokenService jwtTokenService, JwtOptions jwtOptions)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions;
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

        // JWT is the auth credential; the HttpOnly cookie is only its browser transport.
        var token = _jwtTokenService.GenerateToken(session);
        var expires = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        AuthCookie.Issue(Request, Response, token, model.RememberMe, expires);

        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        // JWTs are stateless: forgetting the token logs the browser out.
        AuthCookie.Clear(Response);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Auth;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        AuthService authService,
        JwtTokenService jwtTokenService,
        RefreshTokenService refreshTokenService,
        JwtOptions jwtOptions)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
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
    [SkipAutoRefresh]
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

        // Short-lived access JWT + long-lived rotating opaque refresh token.
        // Both travel in HttpOnly cookies; only hashes touch the database.
        await IssueSessionAsync(session, model.RememberMe);

        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpPost]
    [AllowAnonymous]
    [SkipAutoRefresh]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Refresh(string? returnUrl = null)
    {
        Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var presented);
        RefreshRotationResult outcome;
        try
        {
            outcome = await _refreshTokenService.RotateAsync(presented);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            outcome = RefreshRotationResult.Invalid();
        }

        if (!outcome.Succeeded || outcome.User is null || outcome.NewToken is null)
        {
            AuthCookie.ClearAll(Response);
            if (WantsJson())
            {
                return Unauthorized(new { error = "Session expired. Please log in again." });
            }
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var session = new UserSession(
            outcome.User.Id,
            outcome.User.UserName,
            outcome.User.FullName,
            outcome.User.IsActive)
        {
            Role = outcome.User.Role,
        };
        await IssueSessionAsync(session, AuthCookie.IsPersistent(Request), outcome.NewToken);

        if (WantsJson())
        {
            return Ok(new { renewed = true });
        }
        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [AllowAnonymous]
    [SkipAutoRefresh]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Revoke this device chain's refresh family; logout stays idempotent
        // when the access token already expired or cookies are gone.
        try
        {
            Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var presented);
            await _refreshTokenService.RevokeAsync(presented);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
        }

        AuthCookie.ClearAll(Response);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Login));

    private async Task IssueSessionAsync(UserSession session, bool rememberMe, string? existingRefreshPlaintext = null)
    {
        var now = DateTimeOffset.UtcNow;
        var accessToken = _jwtTokenService.GenerateToken(session);
        var accessExpires = now.AddMinutes(_jwtOptions.ExpiryMinutes);

        string refreshPlaintext;
        DateTimeOffset refreshExpires;
        if (existingRefreshPlaintext is not null)
        {
            refreshPlaintext = existingRefreshPlaintext;
            refreshExpires = _refreshTokenService.RefreshExpiry(now);
        }
        else
        {
            var issued = await _refreshTokenService.IssueAsync(session.UserId);
            refreshPlaintext = issued.PlainTextToken;
            refreshExpires = issued.Record.ExpiresAt;
        }

        AuthCookie.IssuePair(Request, Response, accessToken, accessExpires, refreshPlaintext, refreshExpires, rememberMe);
        AuthCookie.IssueRemember(Request, Response, rememberMe);
    }

    private bool WantsJson() =>
        Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

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

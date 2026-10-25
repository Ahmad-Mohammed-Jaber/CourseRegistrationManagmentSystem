using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Auth;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Auth;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly AccessTokenService _accessTokenService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly IMapper _mapper;

    public AuthController(
        AuthService authService,
        AccessTokenService accessTokenService,
        RefreshTokenService refreshTokenService,
        JwtOptions jwtOptions,
        IMapper mapper)
    {
        _authService = authService;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    [SkipAutoRefresh]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        var outcome = await ValidateRefreshCookieAsync();
        if (outcome.Succeeded && outcome.User is not null && outcome.User.IsActive)
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

        await IssueSessionAsync(session, model.RememberMe);

        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpPost]
    [AllowAnonymous]
    [SkipAutoRefresh]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Refresh(string? returnUrl = null)
    {
        var outcome = await ValidateRefreshCookieAsync();

        if (!outcome.Succeeded || outcome.User is null || outcome.Record is null)
        {
            AuthCookie.ClearAll(Response);
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var session = BuildSession(outcome.User);
        await IssueSessionAsync(session, AuthCookie.IsPersistent(Request), outcome.PresentedToken, outcome.Record.ExpiresAt);

        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [AllowAnonymous]
    [SkipAutoRefresh]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
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

    private async Task IssueSessionAsync(UserSession session, bool rememberMe, string? existingRefreshPlaintext = null, DateTimeOffset? existingRefreshExpires = null)
    {
        var now = DateTimeOffset.UtcNow;
        var accessToken = _accessTokenService.GenerateToken(session);
        var accessExpires = now.AddMinutes(_jwtOptions.ExpiryMinutes);

        string refreshPlaintext;
        DateTimeOffset refreshExpires;
        if (existingRefreshPlaintext is not null)
        {
            // Silent refresh: same refresh token stays valid, only the access JWT is renewed.
            refreshPlaintext = existingRefreshPlaintext;
            refreshExpires = existingRefreshExpires ?? _refreshTokenService.RefreshExpiry(now);
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

    private sealed record RefreshCookieOutcome(
        bool Succeeded,
        bool Presented,
        string? PresentedToken,
        User? User,
        RefreshToken? Record);

    private async Task<RefreshCookieOutcome> ValidateRefreshCookieAsync()
    {
        Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var presented);
        if (string.IsNullOrWhiteSpace(presented))
        {
            return new RefreshCookieOutcome(false, false, null, null, null);
        }

        RefreshValidationResult outcome;
        try
        {
            outcome = await _refreshTokenService.ValidateAsync(presented);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            outcome = RefreshValidationResult.Invalid();
        }

        if (!outcome.Succeeded || outcome.User is null || outcome.Record is null)
        {
            return new RefreshCookieOutcome(false, true, presented, null, null);
        }

        return new RefreshCookieOutcome(true, true, presented, outcome.User, outcome.Record);
    }

    private UserSession BuildSession(User user) => _mapper.Map<UserSession>(user);

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

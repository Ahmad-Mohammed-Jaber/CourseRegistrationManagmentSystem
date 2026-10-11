using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Auth;

public sealed class AutoRefreshTokenFilter : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext httpContext,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        var endpoint = httpContext.GetEndpoint();
        if (!authorizeResult.Challenged
            || endpoint?.Metadata.GetMetadata<SkipAutoRefreshAttribute>() is not null
            || endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null
            || httpContext.User.Identity?.IsAuthenticated == true
            || !httpContext.Request.Cookies.TryGetValue(AuthCookie.RefreshName, out var presented)
            || string.IsNullOrWhiteSpace(presented))
        {
            await _default.HandleAsync(next, httpContext, policy, authorizeResult);
            return;
        }

        try
        {
            var services = httpContext.RequestServices;
            var refreshService = services.GetRequiredService<RefreshTokenService>();
            var jwtService = services.GetRequiredService<JwtTokenService>();
            var options = services.GetRequiredService<JwtOptions>();

            var outcome = await refreshService.RotateAsync(presented);
            if (!outcome.Succeeded || outcome.User is null || outcome.NewToken is null)
            {
                // Invalid, expired, reused (family revoked inside RotateAsync),
                // or disabled account: forget both cookies, back to login.
                AuthCookie.ClearAll(httpContext.Response);
                await _default.HandleAsync(next, httpContext, policy, authorizeResult);
                return;
            }

            var session = new UserSession(
                outcome.User.Id,
                outcome.User.UserName,
                outcome.User.FullName,
                outcome.User.IsActive)
            {
                Role = outcome.User.Role,
            };

            var now = DateTimeOffset.UtcNow;
            var accessToken = jwtService.GenerateToken(session);
            var persistent = AuthCookie.IsPersistent(httpContext.Request);
            AuthCookie.IssuePair(
                httpContext.Request,
                httpContext.Response,
                accessToken,
                now.AddMinutes(options.ExpiryMinutes),
                outcome.NewToken,
                refreshService.RefreshExpiry(now),
                persistent);
            AuthCookie.IssueRemember(httpContext.Request, httpContext.Response, persistent);

            // Retry the original request with fresh cookies. 307 preserves the
            // HTTP method + body, so POSTs (with their antiforgery tokens) replay.
            var retryTo = httpContext.Request.Path + httpContext.Request.QueryString;
            httpContext.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
            httpContext.Response.Headers.Location = retryTo;
            return;
        }
        catch (Exception ex)
        {
            // Fail closed (back to login) when the store is unreachable.
            AppLogger.LogCaught(ex);
            await _default.HandleAsync(next, httpContext, policy, authorizeResult);
            return;
        }
    }
}

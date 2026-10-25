using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Auth;
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
            var accessService = services.GetRequiredService<AccessTokenService>();
            var options = services.GetRequiredService<JwtOptions>();
            var mapper = services.GetRequiredService<IMapper>();

            var outcome = await refreshService.ValidateAsync(presented);
            if (!outcome.Succeeded || outcome.User is null || outcome.Record is null)
            {
                AuthCookie.ClearAll(httpContext.Response);
                await _default.HandleAsync(next, httpContext, policy, authorizeResult);
                return;
            }

            var session = mapper.Map<UserSession>(outcome.User);

            var now = DateTimeOffset.UtcNow;
            var accessToken = accessService.GenerateToken(session);
            var persistent = AuthCookie.IsPersistent(httpContext.Request);
            AuthCookie.IssueAccess(
                httpContext.Request,
                httpContext.Response,
                accessToken,
                persistent,
                now.AddMinutes(options.ExpiryMinutes));
            AuthCookie.IssueRemember(httpContext.Request, httpContext.Response, persistent);

            var retryTo = httpContext.Request.Path + httpContext.Request.QueryString;
            httpContext.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
            httpContext.Response.Headers.Location = retryTo;
            return;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            await _default.HandleAsync(next, httpContext, policy, authorizeResult);
        }
    }
}

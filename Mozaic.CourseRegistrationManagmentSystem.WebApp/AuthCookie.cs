namespace Mozaic.CourseRegistrationManagementSystem.WebApp;

public static class AuthCookie
{
    public const string Name = "Mozaic.Jwt";

    public const string RefreshName = "Mozaic.Refresh";

    public const string RememberName = "Mozaic.Remember";

    public static CookieOptions Build(bool secure, bool persistent, DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = secure,
        SameSite = SameSiteMode.Strict,
        IsEssential = true,
        Path = "/",
        Expires = persistent ? expires : null,
    };

    public static void Issue(HttpRequest request, HttpResponse response, string token, bool persistent, DateTimeOffset expires) =>
        IssueAccess(request, response, token, persistent, expires);

    public static void IssueAccess(HttpRequest request, HttpResponse response, string accessToken, bool persistent, DateTimeOffset accessExpires) =>
        response.Cookies.Append(Name, accessToken, Build(request.IsHttps, persistent, accessExpires));

    public static void IssueRefresh(HttpRequest request, HttpResponse response, string refreshToken, bool persistent, DateTimeOffset refreshExpires) =>
        response.Cookies.Append(RefreshName, refreshToken, Build(request.IsHttps, persistent, refreshExpires));

    public static void IssueRemember(HttpRequest request, HttpResponse response, bool persistent) =>
        response.Cookies.Append(
            RememberName,
            persistent ? "1" : "0",
            Build(request.IsHttps, persistent, DateTimeOffset.UtcNow.AddYears(1)));

    public static bool IsPersistent(HttpRequest request) =>
        request.Cookies.TryGetValue(RememberName, out var value) && value == "1";

    public static void IssuePair(
        HttpRequest request,
        HttpResponse response,
        string accessToken,
        DateTimeOffset accessExpires,
        string refreshToken,
        DateTimeOffset refreshExpires,
        bool persistent)
    {
        IssueAccess(request, response, accessToken, persistent, accessExpires);
        IssueRefresh(request, response, refreshToken, persistent, refreshExpires);
    }

    public static void Clear(HttpResponse response) => ClearAll(response);

    public static void ClearAll(HttpResponse response)
    {
        // Delete on "/" (current shape) and on legacy default paths
        // (cookies issued before Path="/" was set, e.g. Path=/Auth).
        foreach (var path in new[] { "/", "/Auth", "/Auth/Login" })
        {
            var options = new CookieOptions { Path = path };
            response.Cookies.Delete(Name, options);
            response.Cookies.Delete(RefreshName, options);
            response.Cookies.Delete(RememberName, options);
        }
    }
}

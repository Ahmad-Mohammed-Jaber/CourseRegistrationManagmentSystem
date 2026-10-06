namespace Mozaic.CourseRegistrationManagementSystem.WebApp;

public static class AuthCookie
{
    public const string Name = "Mozaic.Jwt";

    public static CookieOptions Build(bool secure, bool persistent, DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        // SameAsRequest semantics: require Secure only when the request itself is HTTPS,
        // so the http://localhost dev profile keeps working.
        Secure = secure,
        SameSite = SameSiteMode.Strict,
        IsEssential = true,
        Expires = persistent ? expires : null,
    };

    public static void Issue(HttpRequest request, HttpResponse response, string token, bool persistent, DateTimeOffset expires) =>
        response.Cookies.Append(Name, token, Build(request.IsHttps, persistent, expires));

    public static void Clear(HttpResponse response) => response.Cookies.Delete(Name);
}

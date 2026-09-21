
using BL.Services;
using Shared.Logging;

AppLogger.Configure();
try
{
    AuthService _authService = new AuthService();
    var result = await _authService.RegisterAdminAsync("admin", "admin", true, "P@ssw0rd");
    if (!result.IsSuccess)
    {
        AppLogger.Warn("ConsoleSeed", result.Message);
        Console.WriteLine($"Seed failed: {result.Message}");
        return 1;
    }

    Console.WriteLine("Seed succeeded.");
    return 0;
}
catch (Exception ex)
{
    AppLogger.LogCaught(ex);
    Console.WriteLine($"Seed crashed: {ex.Message}");
    return 2;
}
finally
{
    AppLogger.Close();
}

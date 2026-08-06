
using CourseRegistrationManagmentSystem.Shared.Session;

AuthService _authService = new AuthService();

await _authService.RegisterAdminAsync("admin", "admin", true, "P@ssw0rd");
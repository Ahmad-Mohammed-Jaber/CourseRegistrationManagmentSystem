using CourseRegistrationManagmentSystem.Shared.Session;

public static class SessionManager
{
    public static UserSession? UserSession { get; private set; };

    public static StudentSession? StudentSession { get; set; }

    public static bool IsLoggedIn => SessionManager.UserSession != null;

    public static void Login(UserSession userSession)
    {
        UserSession = userSession;
    }

    public static void Logout()

    {
        UserSession = null;
    }
}

namespace Shared.Session;

public static class SessionManager
{
    public static UserSession? Current { get; private set; }

    public static UserSession? UserSession => Current;

    public static bool IsLoggedIn => Current != null;

    public static void Login(UserSession userSession)
    {
        Current = userSession;
    }

    public static void Logout()
    {
        Current = null;
    }
}

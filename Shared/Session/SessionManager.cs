using Shared.Entities;

namespace Shared.Session;

public static class SessionManager
{
    public static UserSession? Current { get; private set; }

    public static UserSession? UserSession => Current;

    public static bool IsLoggedIn => Current != null;

    public static void Login(User userEntity)
    {
        Current = new UserSession(user;)
    }

    public static void Logout()
    {
        Current = null;
    }
}

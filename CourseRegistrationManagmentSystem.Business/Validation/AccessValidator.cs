using CourseRegistrationManagmentSystem.Shared.Session;
using CourseRegistrationManagmentSystem.Shared.Models;
using System;

namespace CourseRegistrationManagmentSystem.Business.Validation;

public static class AccessValidator
{
    public static void RequireLogin()
    {
        if (SessionManager.UserSession == null)
        {
            throw new InvalidOperationException("You must be logged in to perform this action.");
        }
    }

    public static void RequireAdmin()
    {
        RequireLogin();
        if (!SessionManager.UserSession!.IsAdmin)
        {
            throw new InvalidOperationException("Only admin users are allowed to use this service.");
        }
    }

    public static void RequireRole(User.UserRoles role)
    {
        RequireLogin();
        if (SessionManager.UserSession!.Role != role)
        {
            throw new InvalidOperationException($"Only users with the {role} role are allowed to perform this action.");
        }
    }
}

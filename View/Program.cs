using CourseRegistrationManagmentSystem.View;
using Shared.Entities;
using Shared.Session;

namespace View
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            bool keepRunning = true;
            while (keepRunning)
            {
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Single session with role determines dashboard
                    var session = SessionManager.Current;
                    if (session?.Role == User.UserRoles.Admin)
                    {
                        Application.Run(new AdminDashboard());
                    }
                    else
                    {
                        Application.Run(new StudentDashboard());
                    }
                    // If user logged out, loop shows login again; if app closed, keepRunning determined by DialogResult
                    // Logout already cleared session; if session is still logged in (user closed dashboard via X), clear it
                    if (SessionManager.IsLoggedIn) SessionManager.Logout();
                }
                else
                {
                    keepRunning = false;
                }
            }
        }

    }
}

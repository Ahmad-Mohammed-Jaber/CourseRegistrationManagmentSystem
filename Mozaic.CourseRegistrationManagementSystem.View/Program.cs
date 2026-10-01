using CourseRegistrationManagmentSystem.View;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using View;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            AppLogger.Configure();
            Application.ThreadException += (sender, e) =>
            {
                AppLogger.LogCaught(e.Exception);
                AppLogger.LogViewError(e.Exception, "ThreadException");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    AppLogger.LogCaught(ex);
                    AppLogger.LogViewError(ex, "UnhandledException");
                }
            };
            Application.ApplicationExit += (sender, e) => AppLogger.Close();

            try
            {
                Run();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                AppLogger.LogCaught(ex);
                AppLogger.LogViewError(ex);
                MessageBox.Show("An unexpected error occurred. The application will close.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                AppLogger.Close();
            }
        }

        static void Run()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    var session = SessionManager.Current;
                    if (session?.Role == User.UserRoles.Admin)
                    {
                        Application.Run(new AdminDashboard());
                    }
                    else
                    {
                        Application.Run(new StudentDashboard());
                    }
                    if (SessionManager.IsLoggedIn)
                    {
                        SessionManager.Logout();
                    }
                }
                else
                {
                    keepRunning = false;
                }
            }
        }

    }
}

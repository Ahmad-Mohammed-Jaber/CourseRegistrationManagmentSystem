namespace CourseRegistrationManagmentSystem.View
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
                    if (SessionManager.UserSession?.Role == CourseRegistrationManagmentSystem.Shared.Models.User.UserRoles.Admin)
                    {
                        Application.Run(new AdminDashboard());
                    }
                    else
                    {
                        Application.Run(new StudentDashboard());
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
using System;
using System.Windows.Forms;
using Shared.Session;
using BL.Services;
using View;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class StudentDashboard : Form
    {
        private readonly StudentService _studentService = new StudentService();

        public StudentDashboard()
        {
            InitializeComponent();

            Load += StudentDashboard_Load;
        }

        private async void StudentDashboard_Load(object sender, EventArgs e)
        {
            await LoadWelcomeTextAsync();

            // Fix welcome label overlapping with logout button
            lblWelcome.AutoSize = false;
            lblWelcome.Left = 20;
            lblWelcome.Top = 20;

            // Leave space for logout button
            lblWelcome.Width = this.ClientSize.Width - btnLogout.Width - 60;
            lblWelcome.Height = 35;

            lblWelcome.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblWelcome.BringToFront();
        }

        private async System.Threading.Tasks.Task LoadWelcomeTextAsync()
        {
            var session = SessionManager.Current;

            if (session == null)
            {
                lblWelcome.Text = "Welcome, Student";
                return;
            }

            // Base auth info from single UserSession (no student data cached)
            string baseText = $"Welcome, {session.FullName} ({session.UserName})";

            // Student specifics are fetched fresh from DB — authorization checked in BL (RequireStudent)
            try
            {
                var student = await _studentService.GetCurrentStudentAsync();
                if (student != null)
                {
                    lblWelcome.Text = $"{baseText} - Student #{student.StudentNumber}";
                    return;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Not a student or not authorized — fallback to base text
            }
            catch
            {
                // DB fetch failed — fallback to base text
            }

            lblWelcome.Text = baseText;
        }

        private void btnMyRegistrations_Click(object sender, EventArgs e)
        {
            using (var form = new MyRegistrationsForm())
            {
                form.ShowDialog();
            }
        }

        private void btnAvailableCourses_Click(object sender, EventArgs e)
        {
            using (var form = new BrowseClassesForm())
            {
                form.ShowDialog();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.Logout();
            Close();
        }
    }
}

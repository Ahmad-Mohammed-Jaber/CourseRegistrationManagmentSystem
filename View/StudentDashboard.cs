using System;
using System.Windows.Forms;
using Shared.Session;
using BL.Services;
using View;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class StudentDashboard : Form
    {
        public StudentDashboard()
        {
            InitializeComponent();

            Load += StudentDashboard_Load;
        }

        private async void StudentDashboard_Load(object sender, EventArgs e)
        {
            await LoadWelcomeTextAsync();

            lblWelcome.AutoSize = false;
            lblWelcome.Left = 20;
            lblWelcome.Top = 20;

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

            string baseText = $"Welcome, {session.FullName} ({session.UserName})";

            try
            {
                var profile = await StudentService.GetCurrentProfileAsync();
                if (profile != null)
                {
                    lblWelcome.Text = $"{baseText} - Student #{profile.StudentNumber}";
                    return;
                }
            }
            catch
            {
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

using System;
using System.Windows.Forms;
using CourseRegistrationManagmentSystem.Shared.Session;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class StudentDashboard : Form
    {
        public StudentDashboard()
        {
            InitializeComponent();

            Load += StudentDashboard_Load;
        }

        private void StudentDashboard_Load(object sender, EventArgs e)
        {
            LoadWelcomeText();

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

        private void LoadWelcomeText()
        {
            var session = SessionManager.StudentSession;

            if (session == null || string.IsNullOrWhiteSpace(session.UserName))
            {
                lblWelcome.Text = "Welcome, Student";
                return;
            }

            lblWelcome.Text = $"Welcome, {session.UserName} (Student)";
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
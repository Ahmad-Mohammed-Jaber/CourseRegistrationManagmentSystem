using System;
using System.Windows.Forms;
using Shared.Session;
using View;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();

            Load += AdminDashboard_Load;
        }

        private void AdminDashboard_Load(object? sender, EventArgs e)
        {
            LoadWelcomeText();

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

        private void LoadWelcomeText()
        {
            var session = SessionManager.UserSession;

            if (session == null || string.IsNullOrWhiteSpace(session.FullName))
            {
                lblWelcome.Text = "Welcome, Admin";
                return;
            }

            lblWelcome.Text = $"Welcome, {session.FullName} (Admin)";
        }

        private void btnUserManagement_Click(object sender, EventArgs e)
        {
            using (var form = new UserListForm())
            {
                form.ShowDialog();
            }
        }

        private void btnStudentManagement_Click(object sender, EventArgs e)
        {
            using (var form = new StudentListForm())
            {
                form.ShowDialog();
            }
        }

        private void btnCourseManagement_Click(object sender, EventArgs e)
        {
            using (var form = new CourseListForm())
            {
                form.ShowDialog();
            }
        }

        private void btnClassManagement_Click(object sender, EventArgs e)
        {
            using (var form = new ClassListForm())
            {
                form.ShowDialog();
            }
        }

        private void btnRegistrationManagement_Click(object sender, EventArgs e)
        {
            using (var form = new RegistrationListForm())
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
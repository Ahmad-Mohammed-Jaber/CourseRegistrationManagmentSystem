using System.Windows.Forms;
using CourseRegistrationManagmentSystem.Shared.Session;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
            lblWelcome.Text = $"Welcome, {SessionManager.UserSession?.FullName} (Admin)";
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
            this.Close();
        }
    }
}

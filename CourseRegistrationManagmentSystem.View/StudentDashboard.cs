using System.Windows.Forms;
using CourseRegistrationManagmentSystem.Shared.Session;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class StudentDashboard : Form
    {
        public StudentDashboard()
        {
            InitializeComponent();
            lblWelcome.Text = $"Welcome, {SessionManager.StudentSession?.UserName} (Student)";
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
            this.Close();
        }

        private void StudentDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}

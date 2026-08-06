using BL.Services;
using Shared.Session;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService = new AuthService();

        public LoginForm()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblErrorMessage.Text = "Please enter both username and password.";
                return;
            }

            try
            {
                lblErrorMessage.Text = "Authenticating...";
                var (userSession, studentSession) = await _authService.LoginAsync(username, password);

                if (userSession != null)
                {
                    SessionManager.Login(userSession);
                    SessionManager.StudentSession = studentSession;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
            catch (Exception)
            {
                lblErrorMessage.Text = "An unexpected error occurred. Please try again.";
            }
        }
    }
}

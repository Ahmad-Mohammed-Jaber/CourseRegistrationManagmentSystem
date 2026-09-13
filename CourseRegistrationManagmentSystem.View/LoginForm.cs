using BL.Services;
using Shared.Exceptions;
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
            catch (BussinessException ex)
            {
                lblErrorMessage.Text = "An error ";
            }
            catch (Exception)
            {
                // Must use Logger here, as exception wasnt logged before hand
                lblErrorMessage.Text = "An unexpected error occurred. Please try again.";
            }
        }
    }
}

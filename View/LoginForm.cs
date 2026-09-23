using BL.Services;
using Shared.Exceptions;
using Shared.Logging;
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
                var loginResult = await _authService.LoginAsync(username, password);
                if (!loginResult.IsSuccess)
                {
                    lblErrorMessage.Text = loginResult.Message;
                    return;
                }
                SessionManager.Login(loginResult.Value!);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (BusinessException ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                lblErrorMessage.Text = "An unexpected error occurred. Please try again.";
            }
        }
    }
}

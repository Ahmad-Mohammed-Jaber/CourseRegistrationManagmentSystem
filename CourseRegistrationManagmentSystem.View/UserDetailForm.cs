using CourseRegistrationManagmentSystem.Business.Services;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class UserDetailForm : Form
    {
        private readonly UserService _userService = new UserService();
        private readonly AuthService _authService = new AuthService();
        private UserDto? _user;
        private bool _isEditMode;

        private TextBox txtUsername;
        private TextBox txtFullName;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private TextBox txtPassword;
        private Label lblUsername;
        private Label lblFullName;
        private Label lblRole;
        private Label lblPassword;
        private Button btnSave;
        private Button btnCancel;

        public UserDetailForm(UserDto? user = null)
        {
            _user = user;
            _isEditMode = user != null;
            InitializeComponent();
            if (_isEditMode)
            {
                LoadUserData();
            }
            else
            {
                cmbRole.SelectedItem = User.UserRoles.Admin;
            }
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtFullName = new TextBox();
            this.cmbRole = new ComboBox();
            this.chkIsActive = new CheckBox();
            this.txtPassword = new TextBox();
            this.lblUsername = new Label();
            this.lblFullName = new Label();
            this.lblRole = new Label();
            this.lblPassword = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // lblUsername
            this.lblUsername.Text = "Username:";
            this.lblUsername.Location = new Point(20, 20);
            this.lblUsername.AutoSize = true;

            // txtUsername
            this.txtUsername.Location = new Point(120, 20);
            this.txtUsername.Size = new Size(200, 25);

            // lblFullName
            this.lblFullName.Text = "Full Name:";
            this.lblFullName.Location = new Point(20, 60);
            this.lblFullName.AutoSize = true;

            // txtFullName
            this.txtFullName.Location = new Point(120, 60);
            this.txtFullName.Size = new Size(200, 25);

            // lblRole
            this.lblRole.Text = "Role:";
            this.lblRole.Location = new Point(20, 100);
            this.lblRole.AutoSize = true;

            // cmbRole
            this.cmbRole.Location = new Point(120, 100);
            this.cmbRole.Size = new Size(200, 25);
            this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (User.UserRoles role in Enum.GetValues(typeof(User.UserRoles)))
            {
                this.cmbRole.Items.Add(role);
            }
            this.cmbRole.Enabled = false;

            // lblPassword
            this.lblPassword.Text = "Password:";
            this.lblPassword.Location = new Point(20, 140);
            this.lblPassword.AutoSize = true;

            // txtPassword
            this.txtPassword.Location = new Point(120, 140);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Enabled = !_isEditMode; // Only for new users in this simple version

            // chkIsActive
            this.chkIsActive.Text = "Is Active";
            this.chkIsActive.Location = new Point(120, 180);
            this.chkIsActive.AutoSize = true;

            // btnSave
            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 220);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            // btnCancel
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 220);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // UserDetailForm
            this.ClientSize = new Size(380, 300);
            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblFullName, txtFullName, lblRole, cmbRole, lblPassword, txtPassword, chkIsActive, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit User" : "Add User";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadUserData()
        {
            if (_user == null) return;
            txtUsername.Text = _user.UserName;
            txtFullName.Text = _user.FullName;
            cmbRole.SelectedItem = _user.Role;
            chkIsActive.Checked = _user.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Username and Full Name are required.");
                return;
            }

            var dto = new UserDto
            {
                Id = _isEditMode ? _user!.Id : Guid.NewGuid(),
                UserName = txtUsername.Text,
                FullName = txtFullName.Text,
                Role = _isEditMode ? _user!.Role : (User.UserRoles)cmbRole.SelectedItem!,
                IsActive = chkIsActive.Checked
            };

            try
            {
                if (_isEditMode)
                {
                    await _userService.UpdateAsync(dto.Id, dto.ToEntity());
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        await _authService.RegisterAdminAsync(
                            dto.UserName, dto.FullName, dto.IsActive, txtPassword.Text);
                    }
                    else
                    {
                        await _userService.AddAsync(dto.ToEntity());
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}");
            }
        }
    }
}

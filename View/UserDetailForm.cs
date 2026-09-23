using BL.Services;
using BL.Validation;
using Shared.Dtos;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;
using System.Windows.Forms;

namespace View
{
    public partial class UserDetailForm : Form
    {
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

            this.lblUsername.Text = "Username:";
            this.lblUsername.Location = new Point(20, 20);
            this.lblUsername.AutoSize = true;

            this.txtUsername.Location = new Point(120, 20);
            this.txtUsername.Size = new Size(200, 25);

            this.lblFullName.Text = "Full Name:";
            this.lblFullName.Location = new Point(20, 60);
            this.lblFullName.AutoSize = true;

            this.txtFullName.Location = new Point(120, 60);
            this.txtFullName.Size = new Size(200, 25);

            this.lblRole.Text = "Role:";
            this.lblRole.Location = new Point(20, 100);
            this.lblRole.AutoSize = true;

            this.cmbRole.Location = new Point(120, 100);
            this.cmbRole.Size = new Size(200, 25);
            this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (User.UserRoles role in Enum.GetValues(typeof(User.UserRoles)))
            {
                this.cmbRole.Items.Add(role);
            }
            this.cmbRole.Enabled = false;

            this.lblPassword.Text = "Password:";
            this.lblPassword.Location = new Point(20, 140);
            this.lblPassword.AutoSize = true;

            this.txtPassword.Location = new Point(120, 140);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Enabled = !_isEditMode;

            this.chkIsActive.Text = "Is Active";
            this.chkIsActive.Location = new Point(120, 180);
            this.chkIsActive.AutoSize = true;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 220);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 220);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 300);
            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblFullName, txtFullName, lblRole, cmbRole, lblPassword, txtPassword, chkIsActive, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit User" : "Add User";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadUserData()
        {
            if (_user == null)
            {
                return;
            }

            txtUsername.Text = _user.UserName;
            txtFullName.Text = _user.FullName;
            cmbRole.SelectedItem = _user.Role;
            chkIsActive.Checked = _user.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var dto = new UserDto
            {
                Id = _isEditMode ? _user!.Id : 0,
                UserName = txtUsername.Text,
                FullName = txtFullName.Text,
                Role = _isEditMode ? _user!.Role : (User.UserRoles)cmbRole.SelectedItem!,
                IsActive = chkIsActive.Checked
            };

            var validation = _isEditMode
                ? UserValidator.ValidateUser(dto.ToEntity())
                : string.IsNullOrWhiteSpace(txtPassword.Text)
                    ? UserValidator.ValidateUser(dto.ToEntity())
                    : UserValidator.ValidateAdminRegistration(dto.UserName, dto.FullName, txtPassword.Text);
            if (!validation.IsSuccess)
            {
                MessageBox.Show(validation.Message);
                return;
            }

            try
            {
                ValidationResult saveResult;
                if (_isEditMode)
                {
                    saveResult = await UserService.UpdateAsync(dto.Id, dto.ToEntity());
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        saveResult = await _authService.RegisterAdminAsync(
                            dto.UserName, dto.FullName, dto.IsActive, txtPassword.Text);
                    }
                    else
                    {
                        saveResult = await UserService.AddAsync(dto.ToEntity());
                    }
                }
                if (!saveResult.IsSuccess)
                {
                    MessageBox.Show($"Error saving user: {saveResult.Message}");
                    return;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error saving user due to an unexpected error.");
            }
        }
    }
}
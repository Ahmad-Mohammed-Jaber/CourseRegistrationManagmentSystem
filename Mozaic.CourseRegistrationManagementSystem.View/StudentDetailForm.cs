using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class StudentDetailForm : Form
    {
        private readonly AuthService _authService = new AuthService();

        private Student? _student;
        private bool _isEditMode;

        private TextBox txtNumber;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtPassword;

        private CheckBox chkActive;

        private Label lblNumber;
        private Label lblUsername;
        private Label lblFullName;
        private Label lblEmail;
        private Label lblPhone;
        private Label lblPassword;

        private Button btnSave;
        private Button btnCancel;

        public Student? SavedEntity { get; private set; }

        public StudentDetailForm(Student? student = null)
        {
            _student = student;
            _isEditMode = student != null;

            InitializeComponent();

            this.Text = _isEditMode ? "Edit Student" : "Add Student";

            if (_isEditMode)
            {
                LoadData();
            }
        }

        private void InitializeComponent()
        {
            txtNumber = new TextBox();
            txtUsername = new TextBox();
            txtFullName = new TextBox();
            txtEmail = new TextBox();
            txtPhone = new TextBox();
            txtPassword = new TextBox();

            chkActive = new CheckBox();

            lblNumber = new Label();
            lblUsername = new Label();
            lblFullName = new Label();
            lblEmail = new Label();
            lblPhone = new Label();
            lblPassword = new Label();

            btnSave = new Button();
            btnCancel = new Button();

            SuspendLayout();

            lblNumber.Text = "Student #:";
            lblNumber.Location = new Point(20, 20);
            lblNumber.AutoSize = true;

            txtNumber.Location = new Point(130, 20);
            txtNumber.Size = new Size(200, 25);

            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(20, 60);
            lblUsername.AutoSize = true;

            txtUsername.Location = new Point(130, 60);
            txtUsername.Size = new Size(200, 25);

            lblFullName.Text = "Full Name:";
            lblFullName.Location = new Point(20, 100);
            lblFullName.AutoSize = true;

            txtFullName.Location = new Point(130, 100);
            txtFullName.Size = new Size(200, 25);

            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(20, 140);
            lblEmail.AutoSize = true;

            txtEmail.Location = new Point(130, 140);
            txtEmail.Size = new Size(200, 25);

            lblPhone.Text = "Phone:";
            lblPhone.Location = new Point(20, 180);
            lblPhone.AutoSize = true;

            txtPhone.Location = new Point(130, 180);
            txtPhone.Size = new Size(200, 25);

            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(20, 220);
            lblPassword.AutoSize = true;

            txtPassword.Location = new Point(130, 220);
            txtPassword.Size = new Size(200, 25);

            txtPassword.PasswordChar = '*';

            chkActive.Text = "Is Active";
            chkActive.Location = new Point(130, 260);
            chkActive.AutoSize = true;

            btnSave.Text = "Save";
            btnSave.Location = new Point(130, 300);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += btnSave_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(220, 300);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += btnCancel_Click;


            Controls.AddRange(new Control[]
            {
                lblNumber, txtNumber,
                lblUsername, txtUsername,
                lblFullName, txtFullName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblPassword, txtPassword,
                chkActive,
                btnSave, btnCancel
            });

            ClientSize = new Size(380, 370);
            Text = "Add Student";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
            PerformLayout();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


        private void LoadData()
        {
            if (_student == null)
            {
                return;
            }

            txtNumber.Text = _student.StudentNumber.ToString();
            txtUsername.Text = _student.UserName;
            txtFullName.Text = _student.FullName;
            txtEmail.Text = _student.Email;
            txtPhone.Text = _student.Phone;
            chkActive.Checked = _student.IsActive;

            txtPassword.Text = "********";
            txtPassword.Enabled = false;
        }


        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtNumber.Text, out int studentNumber))
            {
                var numValidation = StudentValidator.ValidateStudent(new Student
                {
                    StudentNumber = 0,
                    UserName = txtUsername.Text,
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text
                });
                var errors = numValidation.Errors
                    .Where(err => err.Field != nameof(Student.StudentNumber))
                    .Prepend(new Shared.Entities.ValidationError(nameof(Student.StudentNumber), "Invalid student number."))
                    .ToArray();
                MessageBox.Show(string.Join(Environment.NewLine, errors.Select(err => $"• {err.Message}")));
                return;
            }

            Shared.Entities.ValidationResult validation;
            if (_isEditMode)
            {
                validation = StudentValidator.ValidateStudent(new Student
                {
                    StudentNumber = studentNumber,
                    UserName = txtUsername.Text,
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text
                });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Password is required.");
                    return;
                }
                validation = StudentValidator.ValidateStudentRegistration(
                    txtUsername.Text, studentNumber, txtFullName.Text,
                    txtEmail.Text, txtPhone.Text, txtPassword.Text);
            }
            if (!validation.IsSuccess)
            {
                MessageBox.Show(validation.Message);
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    var entity = new Student
                    {
                        Id = _student!.Id,
                        UserId = _student.UserId,
                        StudentNumber = studentNumber,
                        UserName = txtUsername.Text,
                        FullName = txtFullName.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,
                        IsActive = chkActive.Checked,
                        Role = User.UserRoles.Student
                    };

                    await StudentService.UpdateAsync(entity);
                    entity.CreatedOn = _student.CreatedOn;
                    entity.CreatedBy = _student.CreatedBy;
                    entity.ModifiedOn = DateTimeOffset.Now;
                    SavedEntity = entity;
                }
                else
                {
                    var regResult = await _authService.RegisterStudentAsync(
                        txtUsername.Text,
                        studentNumber,
                        txtFullName.Text,
                        chkActive.Checked,
                        txtEmail.Text,
                        txtPhone.Text,
                        txtPassword.Text
                    );

                    if (!regResult.IsSuccess || regResult.Value == null)
                    {
                        MessageBox.Show($"Error saving student: {regResult.Message}");
                        return;
                    }
                    var saved = regResult.Value;
                    saved.StudentNumber = studentNumber;
                    saved.Email = txtEmail.Text;
                    saved.Phone = txtPhone.Text;
                    saved.UserName = txtUsername.Text;
                    saved.FullName = txtFullName.Text;
                    saved.IsActive = chkActive.Checked;
                    saved.Role = User.UserRoles.Student;
                    if (_isEditMode && _student != null)
                    {
                        saved.CreatedOn = _student.CreatedOn;
                        saved.CreatedBy = _student.CreatedBy;
                    }
                    else
                    {
                        saved.CreatedOn = DateTimeOffset.Now;
                    }
                    saved.ModifiedOn = DateTimeOffset.Now;
                    SavedEntity = saved;
                }
                DialogResult = DialogResult.OK;
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error saving student due to an unexpected error.");
            }
        }
    }
}
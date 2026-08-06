using BL.Services;
using Shared.Dtos;
using Shared.Entities;
using System.Drawing;
using System.Windows.Forms;

namespace View
{
    public partial class StudentDetailForm : Form
    {
        private readonly StudentService _studentService = new StudentService();
        private readonly AuthService _authService = new AuthService();

        private StudentDto? _student;
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

        public StudentDetailForm(StudentDto? student = null)
        {
            _student = student;
            _isEditMode = student != null;

            InitializeComponent();

            if (_isEditMode)
                LoadData();
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

            int labelX = 20;
            int inputX = 130;
            int y = 20;
            int spacing = 40;

            void SetupLabel(Label label, string text, int top)
            {
                label.Text = text;
                label.Location = new Point(labelX, top);
                label.AutoSize = true;
            }

            void SetupTextBox(TextBox box, int top)
            {
                box.Location = new Point(inputX, top);
                box.Size = new Size(200, 25);
            }

            SetupLabel(lblNumber, "Student #:", y);
            SetupTextBox(txtNumber, y);

            y += spacing;
            SetupLabel(lblUsername, "Username:", y);
            SetupTextBox(txtUsername, y);

            y += spacing;
            SetupLabel(lblFullName, "Full Name:", y);
            SetupTextBox(txtFullName, y);

            y += spacing;
            SetupLabel(lblEmail, "Email:", y);
            SetupTextBox(txtEmail, y);

            y += spacing;
            SetupLabel(lblPhone, "Phone:", y);
            SetupTextBox(txtPhone, y);

            y += spacing;
            SetupLabel(lblPassword, "Password:", y);
            SetupTextBox(txtPassword, y);

            txtPassword.PasswordChar = '*';

            y += spacing;

            chkActive.Text = "Is Active";
            chkActive.Location = new Point(inputX, y);
            chkActive.AutoSize = true;

            y += 40;

            btnSave.Text = "Save";
            btnSave.Location = new Point(inputX, y);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += btnSave_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(inputX + 90, y);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;


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

            ClientSize = new Size(380, y + 70);
            Text = _isEditMode ? "Edit Student" : "Add Student";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
            PerformLayout();
        }


        private void LoadData()
        {
            if (_student == null)
                return;

            txtNumber.Text = _student.StudentNumber.ToString();
            txtUsername.Text = _student.UserName;
            txtFullName.Text = _student.FullName;
            txtEmail.Text = _student.Email;
            txtPhone.Text = _student.Phone;
            chkActive.Checked = _student.IsActive;

            // Password cannot be edited
            txtPassword.Text = "********";
            txtPassword.Enabled = false;
        }


        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Student Number, Username and Full Name are required.");
                return;
            }


            if (!int.TryParse(txtNumber.Text, out int studentNumber))
            {
                MessageBox.Show("Invalid student number.");
                return;
            }


            try
            {
                if (_isEditMode)
                {
                    var dto = new StudentDto
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

                    var entity = dto.ToEntity();
                    await _studentService.UpdateAsync(entity.Id, entity);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Password is required.");
                        return;
                    }

                    await _authService.RegisterStudentAsync(
                        txtUsername.Text,
                        studentNumber,
                        txtFullName.Text,
                        chkActive.Checked,
                        txtEmail.Text,
                        txtPhone.Text,
                        txtPassword.Text
                    );
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving student: {ex.Message}");
            }
        }
    }
}
using CourseRegistrationManagmentSystem.Business.Services.AdminServices;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class StudentDetailForm : Form
    {
        private readonly StudentService _studentService = new StudentService();
        private StudentDto? _student;
        private bool _isEditMode;

        private TextBox txtNumber;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private CheckBox chkActive;
        private Label lblNumber;
        private Label lblUsername;
        private Label lblFullName;
        private Label lblEmail;
        private Label lblPhone;
        private Button btnSave;
        private Button btnCancel;

        public StudentDetailForm(StudentDto? student = null)
        {
            _student = student;
            _isEditMode = student != null;
            InitializeComponent();
            if (_isEditMode) LoadData();
        }

        private void InitializeComponent()
        {
            this.txtNumber = new TextBox();
            this.txtUsername = new TextBox();
            this.txtFullName = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhone = new TextBox();
            this.chkActive = new CheckBox();
            this.lblNumber = new Label();
            this.lblUsername = new Label();
            this.lblFullName = new Label();
            this.lblEmail = new Label();
            this.lblPhone = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.lblNumber.Text = "Student #:";
            this.lblNumber.Location = new Point(20, 20);
            this.lblNumber.AutoSize = true;
            this.txtNumber.Location = new Point(120, 20);
            this.txtNumber.Size = new Size(200, 25);

            this.lblUsername.Text = "Username:";
            this.lblUsername.Location = new Point(20, 60);
            this.lblUsername.AutoSize = true;
            this.txtUsername.Location = new Point(120, 60);
            this.txtUsername.Size = new Size(200, 25);

            this.lblFullName.Text = "Full Name:";
            this.lblFullName.Location = new Point(20, 100);
            this.lblFullName.AutoSize = true;
            this.txtFullName.Location = new Point(120, 100);
            this.txtFullName.Size = new Size(200, 25);

            this.lblEmail.Text = "Email:";
            this.lblEmail.Location = new Point(20, 140);
            this.lblEmail.AutoSize = true;
            this.txtEmail.Location = new Point(120, 140);
            this.txtEmail.Size = new Size(200, 25);

            this.lblPhone.Text = "Phone:";
            this.lblPhone.Location = new Point(20, 180);
            this.lblPhone.AutoSize = true;
            this.txtPhone.Location = new Point(120, 180);
            this.txtPhone.Size = new Size(200, 25);

            this.chkActive.Text = "Is Active";
            this.chkActive.Location = new Point(120, 220);
            this.chkActive.AutoSize = true;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 260);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 260);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 320);
            this.Controls.AddRange(new Control[] { lblNumber, txtNumber, lblUsername, txtUsername, lblFullName, txtFullName, lblEmail, txtEmail, lblPhone, txtPhone, chkActive, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit Student" : "Add Student";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData()
        {
            if (_student == null) return;
            txtNumber.Text = _student.StudentNumber.ToString();
            txtUsername.Text = _student.UserName;
            txtFullName.Text = _student.FullName;
            txtEmail.Text = _student.Email;
            txtPhone.Text = _student.Phone;
            chkActive.Checked = _student.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Student Number, Username and Full Name are required.");
                return;
            }

            if (!int.TryParse(txtNumber.Text, out int studentNumber))
            {
                MessageBox.Show("Please enter a valid integer for the Student Number.");
                return;
            }

            var dto = new StudentDto
            {
                Id = _isEditMode ? _student!.Id : Guid.NewGuid(),
                StudentNumber = studentNumber,
                UserName = txtUsername.Text,
                FullName = txtFullName.Text,
                Email = txtEmail.Text,
                Phone = txtPhone.Text,
                IsActive = chkActive.Checked,
                Role = User.UserRoles.Student
            };

            try
            {
                if (_isEditMode) await _studentService.UpdateAsync(dto.Id, dto);
                else await _studentService.AddAsync(dto);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving student: {ex.Message}");
            }
        }
    }
}

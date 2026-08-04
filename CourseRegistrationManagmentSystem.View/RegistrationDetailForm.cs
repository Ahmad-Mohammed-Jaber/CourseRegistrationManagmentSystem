using CourseRegistrationManagmentSystem.Business.Services;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class RegistrationDetailForm : Form
    {
        private readonly RegistrationService _regService = new RegistrationService();
        private readonly StudentService _studentService = new StudentService();
        private readonly ClassService _classService = new ClassService();
        private RegistrationDto? _reg;
        private bool _isEditMode;

        private ComboBox cmbStudent;
        private ComboBox cmbClass;
        private ComboBox cmbStatus;
        private DateTimePicker dtRegDate;
        private Label lblStudent;
        private Label lblClass;
        private Label lblStatus;
        private Label lblDate;
        private Button btnSave;
        private Button btnCancel;

        public RegistrationDetailForm(RegistrationDto? reg = null)
        {
            _reg = reg;
            _isEditMode = reg != null;
            InitializeComponent();
            LoadStudents();
            LoadClasses();
            if (_isEditMode) LoadData();
        }

        private void InitializeComponent()
        {
            this.cmbStudent = new ComboBox();
            this.cmbClass = new ComboBox();
            this.cmbStatus = new ComboBox();
            this.dtRegDate = new DateTimePicker();
            this.lblStudent = new Label();
            this.lblClass = new Label();
            this.lblStatus = new Label();
            this.lblDate = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.lblStudent.Text = "Student:";
            this.lblStudent.Location = new Point(20, 20);
            this.lblStudent.AutoSize = true;
            this.cmbStudent.Location = new Point(120, 20);
            this.cmbStudent.Size = new Size(200, 25);
            this.cmbStudent.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblClass.Text = "Class:";
            this.lblClass.Location = new Point(20, 60);
            this.lblClass.AutoSize = true;
            this.cmbClass.Location = new Point(120, 60);
            this.cmbClass.Size = new Size(200, 25);
            this.cmbClass.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblStatus.Text = "Status:";
            this.lblStatus.Location = new Point(20, 100);
            this.lblStatus.AutoSize = true;
            this.cmbStatus.Location = new Point(120, 100);
            this.cmbStatus.Size = new Size(200, 25);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Pending", "Confirmed", "Cancelled" });

            this.lblDate.Text = "Date:";
            this.lblDate.Location = new Point(20, 140);
            this.lblDate.AutoSize = true;
            this.dtRegDate.Location = new Point(120, 140);
            this.dtRegDate.Size = new Size(200, 25);

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 180);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 180);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 250);
            this.Controls.AddRange(new Control[] { cmbStudent, cmbClass, cmbStatus, dtRegDate, lblStudent, lblClass, lblStatus, lblDate, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit Registration" : "Add Registration";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void LoadStudents()
        {
            var students = await _studentService.GetAllAsync();
            cmbStudent.DataSource = students;
            cmbStudent.DisplayMember = "FullName";
            cmbStudent.ValueMember = "Id";
        }

        private async void LoadClasses()
        {
            var classes = await _classService.GetAllAsync();
            cmbClass.DataSource = classes;
            cmbClass.DisplayMember = "ClassName";
            cmbClass.ValueMember = "Id";
        }

        private void LoadData()
        {
            if (_reg == null) return;
            cmbStudent.SelectedValue = _reg.StudentId;
            cmbClass.SelectedValue = _reg.ClassId;
            cmbStatus.SelectedItem = _reg.Status.ToString();
            dtRegDate.Value = _reg.RegistrationDate;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedValue == null || cmbClass.SelectedValue == null)
            {
                MessageBox.Show("Student and Class are required.");
                return;
            }

            var dto = new RegistrationDto
            {
                Id = _isEditMode ? _reg!.Id : Guid.NewGuid(),
                StudentId = (Guid)cmbStudent.SelectedValue!,
                ClassId = (Guid)cmbClass.SelectedValue!,
                Status = _reg?.Status ?? "", // Simplified
                RegistrationDate = dtRegDate.Value
            };

            try
            {
                if (_isEditMode) await _regService.UpdateAsync(dto.Id, dto);
                else await _regService.AddAsync(dto);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving registration: {ex.Message}");
            }
        }
    }
}

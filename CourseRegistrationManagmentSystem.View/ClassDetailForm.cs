using CourseRegistrationManagmentSystem.Business.Services.AdminServices;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Helpers;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class ClassDetailForm : Form
    {
        private readonly ClassService _classService = new ClassService();
        private readonly CourseService _courseService = new CourseService();
        private ClassDto? _class;
        private bool _isEditMode;

        private ComboBox cmbCourse;
        private TextBox txtName;
        private TextBox txtInstructor;
        private NumericUpDown numCapacity;
        private TextBox txtSchedule;
        private DateTimePicker dtStart;
        private DateTimePicker dtEnd;
        private CheckBox chkActive;
        private Label lblCourse;
        private Label lblName;
        private Label lblInstructor;
        private Label lblCapacity;
        private Label lblSchedule;
        private Label lblStart;
        private Label lblEnd;
        private Button btnSave;
        private Button btnCancel;

        public ClassDetailForm(ClassDto? cls = null)
        {
            _class = cls;
            _isEditMode = cls != null;
            InitializeComponent();
            LoadCourses();
            if (_isEditMode) LoadData();
        }

        private void InitializeComponent()
        {
            this.cmbCourse = new ComboBox();
            this.txtName = new TextBox();
            this.txtInstructor = new TextBox();
            this.numCapacity = new NumericUpDown();
            this.txtSchedule = new TextBox();
            this.dtStart = new DateTimePicker();
            this.dtEnd = new DateTimePicker();
            this.chkActive = new CheckBox();
            this.lblCourse = new Label();
            this.lblName = new Label();
            this.lblInstructor = new Label();
            this.lblCapacity = new Label();
            this.lblSchedule = new Label();
            this.lblStart = new Label();
            this.lblEnd = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.lblCourse.Text = "Course:";
            this.lblCourse.Location = new Point(20, 20);
            this.lblCourse.AutoSize = true;
            this.cmbCourse.Location = new Point(120, 20);
            this.cmbCourse.Size = new Size(200, 25);
            this.cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblName.Text = "Class Name:";
            this.lblName.Location = new Point(20, 60);
            this.lblName.AutoSize = true;
            this.txtName.Location = new Point(120, 60);
            this.txtName.Size = new Size(200, 25);

            this.lblInstructor.Text = "Instructor:";
            this.lblInstructor.Location = new Point(20, 100);
            this.lblInstructor.AutoSize = true;
            this.txtInstructor.Location = new Point(120, 100);
            this.txtInstructor.Size = new Size(200, 25);

            this.lblCapacity.Text = "Max Capacity:";
            this.lblCapacity.Location = new Point(20, 140);
            this.lblCapacity.AutoSize = true;
            this.numCapacity.Location = new Point(120, 140);
            this.numCapacity.Size = new Size(60, 25);

            this.lblSchedule.Text = "Schedule:";
            this.lblSchedule.Location = new Point(20, 180);
            this.lblSchedule.AutoSize = true;
            this.txtSchedule.Location = new Point(120, 180);
            this.txtSchedule.Size = new Size(200, 25);

            this.lblStart.Text = "Start Date:";
            this.lblStart.Location = new Point(20, 220);
            this.lblStart.AutoSize = true;
            this.dtStart.Location = new Point(120, 220);
            this.dtStart.Size = new Size(200, 25);

            this.lblEnd.Text = "End Date:";
            this.lblEnd.Location = new Point(20, 260);
            this.lblEnd.AutoSize = true;
            this.dtEnd.Location = new Point(120, 260);
            this.dtEnd.Size = new Size(200, 25);

            this.chkActive.Text = "Is Active";
            this.chkActive.Location = new Point(120, 300);
            this.chkActive.AutoSize = true;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 340);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 340);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 400);
            this.Controls.AddRange(new Control[] { cmbCourse, txtName, txtInstructor, numCapacity, txtSchedule, dtStart, dtEnd, chkActive, lblCourse, lblName, lblInstructor, lblCapacity, lblSchedule, lblStart, lblEnd, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit Class" : "Add Class";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void LoadCourses()
        {
            var courses = await _courseService.GetAllAsync();
            cmbCourse.DataSource = courses;
            cmbCourse.DisplayMember = "CourseName";
            cmbCourse.ValueMember = "Id";
        }

        private void LoadData()
        {
            if (_class == null) return;
            cmbCourse.SelectedValue = _class.CourseId;
            txtName.Text = _class.ClassName;
            txtInstructor.Text = _class.Instructor;
            numCapacity.Value = _class.MaxCapacity;
            txtSchedule.Text = ScheduleHelper.GetScheduleString(_class.Schedule);
            dtStart.Value = _class.StartDate;
            dtEnd.Value = _class.EndDate;
            chkActive.Checked = _class.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCourse.SelectedValue == null || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Course and Class Name are required.");
                return;
            }

            var dto = new ClassDto
            {
                Id = _isEditMode ? _class!.Id : Guid.NewGuid(),
                CourseId = (Guid)cmbCourse.SelectedValue!,
                ClassName = txtName.Text,
                Instructor = txtInstructor.Text,
                MaxCapacity = (int)numCapacity.Value,
                Schedule = ScheduleHelper.ParseScheduleString(txtSchedule.Text),
                StartDate = dtStart.Value,
                EndDate = dtEnd.Value,
                IsActive = chkActive.Checked
            };

            try
            {
                if (_isEditMode) await _classService.UpdateAsync(dto.Id, dto);
                else await _classService.AddAsync(dto);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving class: {ex.Message}");
            }
        }
    }
}

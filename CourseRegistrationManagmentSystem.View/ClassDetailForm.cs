using CourseRegistrationManagmentSystem.Business.Services;
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
        private FlowLayoutPanel pnlSchedule;
        private Dictionary<CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek, CheckBox> chkDays = new();
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
            this.pnlSchedule = new FlowLayoutPanel();
            this.pnlSchedule.Location = new Point(120, 180);
            this.pnlSchedule.Size = new Size(250, 60);
            this.pnlSchedule.FlowDirection = FlowDirection.LeftToRight;
            this.pnlSchedule.WrapContents = true;

            foreach (CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek day in Enum.GetValues(typeof(CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek)))
            {
                if (day == CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek.None) continue;
                var chk = new CheckBox { Text = day.ToString(), AutoSize = true };
                chkDays[day] = chk;
                this.pnlSchedule.Controls.Add(chk);
            }

            this.lblStart.Text = "Start Date:";
            this.lblStart.Location = new Point(20, 240);
            this.lblStart.AutoSize = true;
            this.dtStart.Location = new Point(120, 240);
            this.dtStart.Size = new Size(200, 25);

            this.lblEnd.Text = "End Date:";
            this.lblEnd.Location = new Point(20, 280);
            this.lblEnd.AutoSize = true;
            this.dtEnd.Location = new Point(120, 280);
            this.dtEnd.Size = new Size(200, 25);

            this.chkActive.Text = "Is Active";
            this.chkActive.Location = new Point(120, 320);
            this.chkActive.AutoSize = true;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 360);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 360);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 450);
            this.Controls.AddRange(new Control[] { cmbCourse, txtName, txtInstructor, numCapacity, pnlSchedule, dtStart, dtEnd, chkActive, lblCourse, lblName, lblInstructor, lblCapacity, lblSchedule, lblStart, lblEnd, btnSave, btnCancel });
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
            foreach (var entry in chkDays)
            {
                entry.Value.Checked = (_class.Schedule & entry.Key) == entry.Key;
            }
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
                Schedule = chkDays.Where(kvp => kvp.Value.Checked)
                                  .Aggregate(CourseRegistrationManagmentSystem.Shared.Models.Class.DaysOfWeek.None, (acc, kvp) => acc | kvp.Key),
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

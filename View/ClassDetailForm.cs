using Shared.Helpers;
using Shared.Dtos;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;
using System.Windows.Forms;
using BL.Services;
using BL.Validation;

namespace View
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
        private TableLayoutPanel pnlSchedule;
        private Dictionary<Class.DaysOfWeek, CheckBox> chkDays = new();
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
            if (_isEditMode)
            {
                LoadData();
            }
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
            this.pnlSchedule = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).BeginInit();
            this.SuspendLayout();

            this.ClientSize = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _isEditMode ? "Edit Class" : "Add Class";

            this.lblCourse.AutoSize = true;
            this.lblCourse.Location = new Point(20, 20);
            this.lblCourse.Text = "Course:";

            this.cmbCourse.Location = new Point(120, 18);
            this.cmbCourse.Size = new Size(250, 25);
            this.cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblName.AutoSize = true;
            this.lblName.Location = new Point(20, 60);
            this.lblName.Text = "Class Name:";

            this.txtName.Location = new Point(120, 58);
            this.txtName.Size = new Size(250, 25);

            this.lblInstructor.AutoSize = true;
            this.lblInstructor.Location = new Point(20, 100);
            this.lblInstructor.Text = "Instructor:";

            this.txtInstructor.Location = new Point(120, 98);
            this.txtInstructor.Size = new Size(250, 25);

            this.lblCapacity.AutoSize = true;
            this.lblCapacity.Location = new Point(20, 140);
            this.lblCapacity.Text = "Max Capacity:";

            this.numCapacity.Location = new Point(120, 138);
            this.numCapacity.Size = new Size(80, 25);
            this.numCapacity.Minimum = 0;
            this.numCapacity.Maximum = 1000;

            this.lblSchedule.AutoSize = true;
            this.lblSchedule.Location = new Point(20, 180);
            this.lblSchedule.Text = "Schedule:";

            this.pnlSchedule.Location = new Point(120, 180);
            this.pnlSchedule.Size = new Size(250, 120);
            this.pnlSchedule.ColumnCount = 2;
            this.pnlSchedule.RowCount = 4;
            this.pnlSchedule.ColumnStyles.Clear();
            this.pnlSchedule.RowStyles.Clear();
            this.pnlSchedule.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.pnlSchedule.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 4; i++)
            {
                this.pnlSchedule.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            }

            this.pnlSchedule.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;

            var orderedDays = new[]
            {
        Class.DaysOfWeek.Sunday,
        Class.DaysOfWeek.Monday,
        Class.DaysOfWeek.Tuesday,
        Class.DaysOfWeek.Wednesday,
        Class.DaysOfWeek.Thursday,
        Class.DaysOfWeek.Friday,
        Class.DaysOfWeek.Saturday
    };

            chkDays.Clear();

            for (int i = 0; i < orderedDays.Length; i++)
            {
                var chk = new CheckBox
                {
                    Text = orderedDays[i].ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                };

                chkDays.Add(orderedDays[i], chk);

                int row = i % 4;
                int col = i / 4;

                this.pnlSchedule.Controls.Add(chk, col, row);
            }

            this.lblStart.AutoSize = true;
            this.lblStart.Location = new Point(20, 320);
            this.lblStart.Text = "Start Date:";

            this.dtStart.Location = new Point(120, 318);
            this.dtStart.Size = new Size(250, 25);

            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new Point(20, 360);
            this.lblEnd.Text = "End Date:";

            this.dtEnd.Location = new Point(120, 358);
            this.dtEnd.Size = new Size(250, 25);

            this.chkActive.AutoSize = true;
            this.chkActive.Location = new Point(120, 400);
            this.chkActive.Text = "Is Active";

            this.btnSave.Location = new Point(120, 445);
            this.btnSave.Size = new Size(90, 35);
            this.btnSave.Text = "Save";
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Location = new Point(220, 445);
            this.btnCancel.Size = new Size(90, 35);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(this.lblCourse);
            this.Controls.Add(this.cmbCourse);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblInstructor);
            this.Controls.Add(this.txtInstructor);
            this.Controls.Add(this.lblCapacity);
            this.Controls.Add(this.numCapacity);
            this.Controls.Add(this.lblSchedule);
            this.Controls.Add(this.pnlSchedule);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.dtStart);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.dtEnd);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private async void LoadCourses()
        {
            var result = await _courseService.GetAllAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show($"Error loading courses: {result.Message}");
                return;
            }
            cmbCourse.DataSource = result.Value!;
            cmbCourse.DisplayMember = "CourseName";
            cmbCourse.ValueMember = "Id";
        }

        private void LoadData()
        {
            if (_class == null)
            {
                return;
            }

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
            var dto = new ClassDto
            {
                Id = _isEditMode ? _class!.Id : 0,
                CourseId = cmbCourse.SelectedValue is int cid ? cid : 0,
                ClassName = txtName.Text,
                Instructor = txtInstructor.Text,
                MaxCapacity = (int)numCapacity.Value,
                Schedule = chkDays.Where(kvp => kvp.Value.Checked)
                                  .Aggregate(Class.DaysOfWeek.None, (acc, kvp) => acc | kvp.Key),
                StartDate = dtStart.Value,
                EndDate = dtEnd.Value,
                IsActive = chkActive.Checked
            };

            var entity = dto.ToEntity();
            var validation = ClassValidator.ValidateClass(entity);
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
                    saveResult = await _classService.UpdateAsync(entity.Id, entity);
                }
                else
                {
                    saveResult = await _classService.AddAsync(entity);
                }

                if (!saveResult.IsSuccess)
                {
                    MessageBox.Show($"Error saving class: {saveResult.Message}");
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
                MessageBox.Show("Error saving class due to an unexpected error.");
            }
        }
    }
}
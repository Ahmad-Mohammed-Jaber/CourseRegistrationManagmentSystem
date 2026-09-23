using BL.Services;
using BL.Validation;
using Shared.Dtos;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;
using System.Windows.Forms;

namespace View
{
    public partial class CourseDetailForm : Form
    {
        private CourseDto? _course;
        private bool _isEditMode;

        private TextBox txtCode;
        private TextBox txtName;
        private NumericUpDown numCredits;
        private TextBox txtDesc;
        private CheckBox chkActive;
        private Label lblCode;
        private Label lblName;
        private Label lblCredits;
        private Label lblDesc;
        private Button btnSave;
        private Button btnCancel;

        public CourseDetailForm(CourseDto? course = null)
        {
            _course = course;
            _isEditMode = course != null;
            InitializeComponent();
            if (_isEditMode)
            {
                LoadData();
            }
        }

        private void InitializeComponent()
        {
            this.txtCode = new TextBox();
            this.txtName = new TextBox();
            this.numCredits = new NumericUpDown();
            this.txtDesc = new TextBox();
            this.chkActive = new CheckBox();
            this.lblCode = new Label();
            this.lblName = new Label();
            this.lblCredits = new Label();
            this.lblDesc = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.lblCode.Text = "Course Code:";
            this.lblCode.Location = new Point(20, 20);
            this.lblCode.AutoSize = true;
            this.txtCode.Location = new Point(120, 20);
            this.txtCode.Size = new Size(200, 25);

            this.lblName.Text = "Course Name:";
            this.lblName.Location = new Point(20, 60);
            this.lblName.AutoSize = true;
            this.txtName.Location = new Point(120, 60);
            this.txtName.Size = new Size(200, 25);

            this.lblCredits.Text = "Credit Hours:";
            this.lblCredits.Location = new Point(20, 100);
            this.lblCredits.AutoSize = true;
            this.numCredits.Location = new Point(120, 100);
            this.numCredits.Size = new Size(60, 25);

            this.lblDesc.Text = "Description:";
            this.lblDesc.Location = new Point(20, 140);
            this.lblDesc.AutoSize = true;
            this.txtDesc.Location = new Point(120, 140);
            this.txtDesc.Size = new Size(200, 60);
            this.txtDesc.Multiline = true;

            this.chkActive.Text = "Is Active";
            this.chkActive.Location = new Point(120, 210);
            this.chkActive.AutoSize = true;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(120, 250);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(210, 250);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new Size(380, 320);
            this.Controls.AddRange(new Control[] { lblCode, txtCode, lblName, txtName, lblCredits, numCredits, lblDesc, txtDesc, chkActive, btnSave, btnCancel });
            this.Text = _isEditMode ? "Edit Course" : "Add Course";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData()
        {
            if (_course == null)
            {
                return;
            }

            txtCode.Text = _course.CourseCode;
            txtName.Text = _course.CourseName;
            numCredits.Value = (decimal)_course.CreditHours;
            txtDesc.Text = _course.Description;
            chkActive.Checked = _course.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var dto = new CourseDto
            {
                Id = _isEditMode ? _course!.Id : 0,
                CourseCode = txtCode.Text,
                CourseName = txtName.Text,
                CreditHours = (int)numCredits.Value,
                Description = txtDesc.Text,
                IsActive = chkActive.Checked
            };

            var entity = dto.ToEntity();
            var validation = CourseValidator.ValidateCourse(entity);
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
                    saveResult = await CourseService.UpdateAsync(entity.Id, entity);
                }
                else
                {
                    saveResult = await CourseService.AddAsync(entity);
                }

                if (!saveResult.IsSuccess)
                {
                    MessageBox.Show($"Error saving course: {saveResult.Message}");
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
                MessageBox.Show("Error saving course due to an unexpected error.");
            }
        }
    }
}
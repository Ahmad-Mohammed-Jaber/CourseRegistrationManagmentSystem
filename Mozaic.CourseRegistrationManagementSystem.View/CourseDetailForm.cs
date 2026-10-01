using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class CourseDetailForm : Form
    {
        private Course? _course;
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

        public Course? SavedEntity { get; private set; }

        public CourseDetailForm(Course? course = null)
        {
            _course = course;
            _isEditMode = course != null;
            InitializeComponent();

            // Moved out of InitializeComponent so designer doesn't break
            this.Text = _isEditMode ? "Edit Course" : "Add Course";

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
            this.btnCancel.Click += btnCancel_Click;   // fixed: no lambda

            this.ClientSize = new Size(380, 320);
            this.Controls.AddRange(new Control[] { lblCode, txtCode, lblName, txtName, lblCredits, numCredits, lblDesc, txtDesc, chkActive, btnSave, btnCancel });
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
            var entity = new Course
            {
                Id = _isEditMode ? _course!.Id : 0,
                CourseCode = txtCode.Text,
                CourseName = txtName.Text,
                CreditHours = (int)numCredits.Value,
                Description = txtDesc.Text,
                IsActive = chkActive.Checked
            };

            var validation = CourseValidator.ValidateCourse(entity);
            if (!validation.IsSuccess)
            {
                MessageBox.Show(validation.Message);
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    await CourseService.UpdateAsync(entity);
                }
                else
                {
                    await CourseService.AddAsync(entity);
                }

                entity.CreatedOn = _isEditMode && _course != null ? _course.CreatedOn : DateTimeOffset.Now;
                entity.ModifiedOn = DateTimeOffset.Now;
                if (_isEditMode && _course != null)
                {
                    entity.CreatedBy = _course.CreatedBy;
                    entity.ModifiedBy = _course.ModifiedBy;
                }
                SavedEntity = entity;
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

        // New method for cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}2
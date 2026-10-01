using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;
using Mozaic.CourseRegistrationManagementSystem.Shared.Dtos;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class RegistrationDetailForm : Form
    {
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

        public RegistrationDto? SavedDto { get; private set; }


        public RegistrationDetailForm(RegistrationDto? reg = null)
        {
            _reg = reg;
            _isEditMode = reg != null;

            InitializeComponent();

            LoadDataAsync();
        }


        private void InitializeComponent()
        {
            cmbStudent = new ComboBox();
            cmbClass = new ComboBox();
            cmbStatus = new ComboBox();
            dtRegDate = new DateTimePicker();

            lblStudent = new Label();
            lblClass = new Label();
            lblStatus = new Label();
            lblDate = new Label();

            btnSave = new Button();
            btnCancel = new Button();


            SuspendLayout();


            lblStudent.Text = "Student:";
            lblStudent.Location = new Point(20, 20);
            lblStudent.AutoSize = true;

            cmbStudent.Location = new Point(120, 20);
            cmbStudent.Size = new Size(200, 25);
            cmbStudent.DropDownStyle = ComboBoxStyle.DropDownList;


            lblClass.Text = "Class:";
            lblClass.Location = new Point(20, 60);
            lblClass.AutoSize = true;

            cmbClass.Location = new Point(120, 60);
            cmbClass.Size = new Size(200, 25);
            cmbClass.DropDownStyle = ComboBoxStyle.DropDownList;


            lblStatus.Text = "Status:";
            lblStatus.Location = new Point(20, 100);
            lblStatus.AutoSize = true;


            cmbStatus.Location = new Point(120, 100);
            cmbStatus.Size = new Size(200, 25);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbStatus.Items.AddRange(new string[]
            {
                "Registered",
                "Pending",
                "Confirmed",
                "Cancelled"
            });

            cmbStatus.SelectedItem = "Registered";


            lblDate.Text = "Date:";
            lblDate.Location = new Point(20, 140);
            lblDate.AutoSize = true;

            dtRegDate.Location = new Point(120, 140);
            dtRegDate.Size = new Size(200, 25);


            btnSave.Text = "Save";
            btnSave.Location = new Point(120, 180);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += btnSave_Click;


            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(210, 180);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
            };


            ClientSize = new Size(380, 250);

            Controls.AddRange(new Control[]
            {
                cmbStudent,
                cmbClass,
                cmbStatus,
                dtRegDate,

                lblStudent,
                lblClass,
                lblStatus,
                lblDate,

                btnSave,
                btnCancel
            });


            Text = _isEditMode
                ? "Edit Registration"
                : "Add Registration";

            StartPosition = FormStartPosition.CenterParent;


            ResumeLayout(false);
            PerformLayout();
        }



        private async void LoadDataAsync()
        {
            var students = await StudentService.GetAllAsync();

            cmbStudent.DataSource = students;
            cmbStudent.DisplayMember = "FullName";
            cmbStudent.ValueMember = "Id";


            var classes = await ClassService.GetAllAsync();

            cmbClass.DataSource = classes;
            cmbClass.DisplayMember = "ClassName";
            cmbClass.ValueMember = "Id";


            if (_isEditMode)
            {
                LoadExistingRegistration();
            }
        }



        private void LoadExistingRegistration()
        {
            if (_reg == null)
            {
                return;
            }

            cmbStudent.SelectedValue = _reg.StudentId;

            cmbClass.SelectedValue = _reg.ClassId;


            if (!string.IsNullOrWhiteSpace(_reg.Status))
            {
                cmbStatus.SelectedItem = _reg.Status;
            }
            else
            {
                cmbStatus.SelectedItem = "Registered";
            }


            dtRegDate.Value = _reg.RegistrationDate;
        }



        private async void btnSave_Click(object sender, EventArgs e)
        {
            var dto = new RegistrationDto
            {
                Id = _isEditMode
                    ? _reg!.Id
                    : 0,

                StudentId = cmbStudent.SelectedValue is int sid ? sid : 0,

                ClassId = cmbClass.SelectedValue is int cid ? cid : 0,

                Status = cmbStatus.SelectedItem?.ToString()
                         ?? "Registered",

                RegistrationDate = dtRegDate.Value
            };

            var entity = dto.ToEntity();
            var validation = RegistrationValidator.ValidateRegistration(entity);
            if (!validation.IsSuccess)
            {
                MessageBox.Show(validation.Message);
                return;
            }

            try
            {
                Result<Registration> saveResult;
                if (_isEditMode)
                {
                    saveResult = await RegistrationService.UpdateAsync(entity.Id, entity);
                }
                else
                {
                    saveResult = await RegistrationService.AddAsync(entity);
                }

                if (!saveResult.IsSuccess || saveResult.Value == null)
                {
                    MessageBox.Show($"Error saving registration: {saveResult.Message}");
                    return;
                }

                var saved = saveResult.Value;
                string? studentName = (cmbStudent.SelectedItem as Student)?.UserName
                    ?? (cmbStudent.SelectedItem as Student)?.FullName
                    ?? _reg?.StudentUserName;
                string? className = (cmbClass.SelectedItem as Class)?.ClassName ?? _reg?.ClassName;
                string? courseName = _reg?.CourseName;
                try
                {
                    if (cmbClass.SelectedItem is Class selectedClass)
                    {
                        var course = await CourseService.GetByIdAsync(selectedClass.CourseId);
                        if (course != null)
                        {
                            courseName = course.CourseName;
                        }
                    }
                }
                catch
                {
                    // Display-name lookup is best-effort; grid still updates without it.
                }
                SavedDto = new RegistrationDto
                {
                    Id = saved.Id,
                    StudentId = saved.StudentId,
                    StudentUserName = studentName,
                    ClassId = saved.ClassId,
                    ClassName = className,
                    CourseName = courseName,
                    RegistrationDate = dtRegDate.Value,
                    Status = cmbStatus.SelectedItem?.ToString() ?? "Registered",
                    CreatedOn = _isEditMode && _reg != null ? _reg.CreatedOn : DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    CreatedBy = _isEditMode && _reg != null ? _reg.CreatedBy : saved.CreatedBy,
                    ModifiedBy = saved.ModifiedBy
                };

                DialogResult = DialogResult.OK;
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error saving registration due to an unexpected error.");
            }
        }
    }
}
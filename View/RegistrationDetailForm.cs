using BL.Services;
using BL.Validation;
using Shared.Dtos;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;
using System.Windows.Forms;

namespace View
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
            var studentsResult = await _studentService.GetAllAsync();
            if (!studentsResult.IsSuccess)
            {
                MessageBox.Show($"Error loading students: {studentsResult.Message}");
                return;
            }
            var students = studentsResult.Value!;

            cmbStudent.DataSource = students;
            cmbStudent.DisplayMember = "FullName";
            cmbStudent.ValueMember = "Id";


            var classesResult = await _classService.GetAllAsync();
            if (!classesResult.IsSuccess)
            {
                MessageBox.Show($"Error loading classes: {classesResult.Message}");
                return;
            }
            var classes = classesResult.Value!;

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
                ValidationResult saveResult;
                if (_isEditMode)
                {
                    saveResult = await _regService.UpdateAsync(entity.Id, entity);
                }
                else
                {
                    saveResult = await _regService.AddAsync(entity);
                }

                if (!saveResult.IsSuccess)
                {
                    MessageBox.Show($"Error saving registration: {saveResult.Message}");
                    return;
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
                MessageBox.Show("Error saving registration due to an unexpected error.");
            }
        }
    }
}
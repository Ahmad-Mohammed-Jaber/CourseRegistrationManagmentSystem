using System.Drawing;
using System.Windows.Forms;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.View.Models;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class BrowseClassesForm : Form
    {
        private DataGridView dgvClasses;
        private Button btnRegister;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private TextBox txtCourseFilter;
        private TextBox txtClassNameFilter;
        private TextBox txtInstructorFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Panel topPanel;
        private List<Class> _classes = new();
        private BrowseClassesViewModel _model = new();
        private const int PageSize = 20;
        private string? _course;
        private string? _className;
        private string? _instructor;

        public BrowseClassesForm()
        {
            InitializeComponent();
            _ = LoadAvailableClassesAsync();
        }

        private void InitializeComponent()
        {
            dgvClasses = new DataGridView();
            btnRegister = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            lblPage = new Label();
            txtCourseFilter = new TextBox();
            txtClassNameFilter = new TextBox();
            txtInstructorFilter = new TextBox();
            btnFilter = new Button();
            btnClearFilters = new Button();
            topPanel = new Panel();

            SuspendLayout();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 105;

            btnRegister.Location = new Point(10, 10);
            btnRegister.Size = new Size(140, 30);
            btnRegister.Text = "Register for Class";
            btnRegister.Click += btnRegister_Click;

            btnPrev.Location = new Point(160, 10);
            btnPrev.Size = new Size(80, 30);
            btnPrev.Text = "< Prev";
            btnPrev.Click += btnPrev_Click;

            btnNext.Location = new Point(250, 10);
            btnNext.Size = new Size(80, 30);
            btnNext.Text = "Next >";
            btnNext.Click += btnNext_Click;

            lblPage.Location = new Point(340, 10);
            lblPage.Size = new Size(200, 30);
            lblPage.TextAlign = ContentAlignment.MiddleLeft;

            var lblCourse = new Label { Text = "Course", Location = new Point(10, 48), Size = new Size(180, 18) };
            txtCourseFilter.Location = new Point(10, 68);
            txtCourseFilter.Size = new Size(180, 25);

            var lblClassName = new Label { Text = "Class Name", Location = new Point(200, 48), Size = new Size(160, 18) };
            txtClassNameFilter.Location = new Point(200, 68);
            txtClassNameFilter.Size = new Size(160, 25);

            var lblInstructor = new Label { Text = "Instructor", Location = new Point(370, 48), Size = new Size(160, 18) };
            txtInstructorFilter.Location = new Point(370, 68);
            txtInstructorFilter.Size = new Size(160, 25);

            btnFilter.Location = new Point(540, 66);
            btnFilter.Size = new Size(70, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(620, 66);
            btnClearFilters.Size = new Size(70, 27);
            btnClearFilters.Text = "Clear";
            btnClearFilters.Click += btnClearFilters_Click;

            topPanel.Controls.Add(btnRegister);
            topPanel.Controls.Add(btnPrev);
            topPanel.Controls.Add(btnNext);
            topPanel.Controls.Add(lblPage);
            topPanel.Controls.Add(lblCourse);
            topPanel.Controls.Add(txtCourseFilter);
            topPanel.Controls.Add(lblClassName);
            topPanel.Controls.Add(txtClassNameFilter);
            topPanel.Controls.Add(lblInstructor);
            topPanel.Controls.Add(txtInstructorFilter);
            topPanel.Controls.Add(btnFilter);
            topPanel.Controls.Add(btnClearFilters);

            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.ReadOnly = true;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClasses.ColumnHeadersHeight = 35;
            dgvClasses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvClasses.DataBindingComplete += dgvClasses_DataBindingComplete;

            ClientSize = new Size(1500, 1000);
            Controls.Add(dgvClasses);
            Controls.Add(topPanel);
            Text = "Available Classes";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }
        private void dgvClasses_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvClasses.Columns.Count == 0)
            {
                return;
            }

            if (dgvClasses.Columns["Id"] != null)
            {
                dgvClasses.Columns["Id"].HeaderText = "Class ID";
            }

            if (dgvClasses.Columns["ClassName"] != null)
            {
                dgvClasses.Columns["ClassName"].HeaderText = "Class";
            }

            if (dgvClasses.Columns["CourseName"] != null)
            {
                dgvClasses.Columns["CourseName"].HeaderText = "Course";
            }

            if (dgvClasses.Columns["Instructor"] != null)
            {
                dgvClasses.Columns["Instructor"].HeaderText = "Instructor";
            }

            if (dgvClasses.Columns["IsRegistered"] != null)
            {
                dgvClasses.Columns["IsRegistered"].HeaderText = "Registered";
            }

            if (dgvClasses.Columns["Schedule"] != null)
            {
                dgvClasses.Columns["Schedule"].HeaderText = "Schedule";
            }

            if (dgvClasses.Columns["CurrentCapacity"] != null)
            {
                dgvClasses.Columns["CurrentCapacity"].HeaderText = "Current Capacity";
            }

            if (dgvClasses.Columns["MaxCapacity"] != null)
            {
                dgvClasses.Columns["MaxCapacity"].HeaderText = "Max Capacity";
            }

            int index = 0;

            if (dgvClasses.Columns["Id"] != null)
            {
                dgvClasses.Columns["Id"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["ClassName"] != null)
            {
                dgvClasses.Columns["ClassName"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["CourseName"] != null)
            {
                dgvClasses.Columns["CourseName"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["Instructor"] != null)
            {
                dgvClasses.Columns["Instructor"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["IsRegistered"] != null)
            {
                dgvClasses.Columns["IsRegistered"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["Schedule"] != null)
            {
                dgvClasses.Columns["Schedule"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["CurrentCapacity"] != null)
            {
                dgvClasses.Columns["CurrentCapacity"].DisplayIndex = index++;
            }

            if (dgvClasses.Columns["MaxCapacity"] != null)
            {
                dgvClasses.Columns["MaxCapacity"].DisplayIndex = index++;
            }

            foreach (DataGridViewColumn column in dgvClasses.Columns)
            {
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }
        }
        private async Task LoadAvailableClassesAsync()
        {
            try
            {
                // usp_GetClasses joins Registrations on StudentId, so pass
                // the Student.Id (not the User.Id) for the IsRegistered flag.
                int? studentId = null;
                try
                {
                    var userId = SessionManager.Current?.UserId ?? 0;
                    if (userId > 0)
                    {
                        studentId = await StudentRegistrationService.GetStudentIdForUserAsync(userId);
                    }
                }
                catch (Exception)
                {
                    // Best-effort only; the grid still renders with IsRegistered unset.
                }

                var paged = await ClassService.GetAsync(
                    userId: studentId,
                    statusFilter: ClassResult.StatusActive,
                    classNameFilter: _className,
                    instructorFilter: _instructor,
                    courseFilter: _course,
                    pageNumber: _model.Page <= 0 ? 1 : _model.Page,
                    pageSize: PageSize);
                _model = BrowseClassesViewModel.FromResult(paged);
                _classes = paged.ToList();
                RefreshGrid();
                UpdatePager();
            }
            catch (BusinessException)
            {
                MessageBox.Show("Could not load classes. Please try again.");
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error loading classes due to an unexpected error.");
            }
        }

        private void RefreshGrid()
        {
            dgvClasses.DataSource = null;
            dgvClasses.DataSource = _model.Rows.ToList();
        }

        private void UpdatePager()
        {
            lblPage.Text = $"Page {_model.Page} of {_model.TotalPages}";
            btnPrev.Enabled = _model.HasPrevious;
            btnNext.Enabled = _model.HasNext;
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_model.HasPrevious)
            {
                _model.Page--;
                await LoadAvailableClassesAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_model.HasNext)
            {
                _model.Page++;
                await LoadAvailableClassesAsync();
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            _course = Clean(txtCourseFilter.Text);
            _className = Clean(txtClassNameFilter.Text);
            _instructor = Clean(txtInstructorFilter.Text);
            _model.Page = 1;
            await LoadAvailableClassesAsync();
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtCourseFilter.Clear();
            txtClassNameFilter.Clear();
            txtInstructorFilter.Clear();
            _course = null;
            _className = null;
            _instructor = null;
            _model.Page = 1;
            await LoadAvailableClassesAsync();
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassBrowseRow gridRow
                && _classes.FirstOrDefault(c => c.Id == gridRow.Id) is Class cls)
            {
                try
                {
                    await StudentRegistrationService.RegisterClass(cls.Id);
                    var cached = _classes.FirstOrDefault(c => c.Id == cls.Id);
                    if (cached != null)
                    {
                        cached.CurrentCapacity++;
                        cached.IsRegistered = true;
                    }
                    var row = _model.Rows.FirstOrDefault(r => r.Id == cls.Id);
                    if (row != null)
                    {
                        row.CurrentCapacity++;
                        row.IsRegistered = true;
                    }
                    RefreshGrid();
                    MessageBox.Show("Registered successfully!");
                }
                catch (BusinessException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    AppLogger.LogViewError(ex);
                    MessageBox.Show("Registration failed due to an unexpected error.");
                }
            }
            else
            {
                MessageBox.Show("Please select a class to register.");
            }
        }
    }
}
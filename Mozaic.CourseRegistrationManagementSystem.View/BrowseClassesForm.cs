using System.Drawing;
using System.Windows.Forms;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
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
        private Panel topPanel;
        private List<Class> _classes = new();
        private BrowseClassesViewModel _model = new();
        private const int PageSize = 20;

        public BrowseClassesForm()
        {
            InitializeComponent();
            LoadAvailableClasses();
        }

        private void InitializeComponent()
        {
            dgvClasses = new DataGridView();
            btnRegister = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            lblPage = new Label();
            topPanel = new Panel();

            SuspendLayout();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

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

            topPanel.Controls.Add(btnRegister);
            topPanel.Controls.Add(btnPrev);
            topPanel.Controls.Add(btnNext);
            topPanel.Controls.Add(lblPage);

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
        private async void LoadAvailableClasses()
        {
            try
            {
                // usp_GetAllClasses joins Registrations on StudentId, so pass
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

                var paged = await ClassService.GetAllAsync(studentId, _model.Page <= 0 ? 1 : _model.Page, PageSize);
                _model = BrowseClassesViewModel.FromResult(paged);
                _classes = paged.Items.ToList();
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

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_model.HasPrevious)
            {
                _model.Page--;
                LoadAvailableClasses();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_model.HasNext)
            {
                _model.Page++;
                LoadAvailableClasses();
            }
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
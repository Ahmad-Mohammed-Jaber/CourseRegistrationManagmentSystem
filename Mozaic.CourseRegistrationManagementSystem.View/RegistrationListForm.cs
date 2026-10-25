using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.View.Mappings;
using Mozaic.CourseRegistrationManagementSystem.View.Models;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class RegistrationListForm : Form
    {
        private DataGridView dgvRegs;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtStudentFilter;
        private TextBox txtCourseFilter;
        private TextBox txtClassNameFilter;
        private ComboBox cmbStatusFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private Panel topPanel;
        private List<Registration> _regs = new();
        private List<RegistrationGridRow> _rows = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _student;
        private string? _course;
        private string? _className;
        private string? _status = "All";

        public RegistrationListForm()
        {
            InitializeComponent();
            _ = LoadRegistrationsAsync();
        }

        private void InitializeComponent()
        {
            dgvRegs = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnBack = new Button();
            txtStudentFilter = new TextBox();
            txtCourseFilter = new TextBox();
            txtClassNameFilter = new TextBox();
            cmbStatusFilter = new ComboBox();
            btnFilter = new Button();
            btnClearFilters = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            lblPage = new Label();
            topPanel = new Panel();

            SuspendLayout();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 105;

            btnBack.Location = new Point(10, 10);
            btnBack.Size = new Size(80, 30);
            btnBack.Text = "Back";
            btnBack.Click += btnBack_Click;

            btnAdd.Location = new Point(100, 10);
            btnAdd.Size = new Size(130, 30);
            btnAdd.Text = "Add Registration";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(240, 10);
            btnEdit.Size = new Size(130, 30);
            btnEdit.Text = "Edit Registration";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(380, 10);
            btnDelete.Size = new Size(140, 30);
            btnDelete.Text = "Delete Registration";
            btnDelete.Click += btnDelete_Click;

            btnPrev.Location = new Point(540, 10);
            btnPrev.Size = new Size(70, 30);
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;

            lblPage.Location = new Point(615, 17);
            lblPage.Size = new Size(150, 20);
            lblPage.TextAlign = ContentAlignment.MiddleCenter;
            lblPage.Text = "Page 1 of 1";

            btnNext.Location = new Point(770, 10);
            btnNext.Size = new Size(70, 30);
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;

            var lblStudent = new Label { Text = "Student", Location = new Point(10, 48), Size = new Size(180, 18) };
            txtStudentFilter.Location = new Point(10, 68);
            txtStudentFilter.Size = new Size(180, 25);

            var lblCourse = new Label { Text = "Course", Location = new Point(200, 48), Size = new Size(160, 18) };
            txtCourseFilter.Location = new Point(200, 68);
            txtCourseFilter.Size = new Size(160, 25);

            var lblClass = new Label { Text = "Class", Location = new Point(370, 48), Size = new Size(160, 18) };
            txtClassNameFilter.Location = new Point(370, 68);
            txtClassNameFilter.Size = new Size(160, 25);

            var lblStatus = new Label { Text = "Status", Location = new Point(540, 48), Size = new Size(120, 18) };
            cmbStatusFilter.Location = new Point(540, 68);
            cmbStatusFilter.Size = new Size(120, 25);
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Registered", "Pending", "Dropped" });
            cmbStatusFilter.SelectedIndex = 0;

            btnFilter.Location = new Point(670, 66);
            btnFilter.Size = new Size(70, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(750, 66);
            btnClearFilters.Size = new Size(70, 27);
            btnClearFilters.Text = "Clear";
            btnClearFilters.Click += btnClearFilters_Click;

            topPanel.Controls.AddRange(new Control[]
            {
                btnBack,
                btnAdd,
                btnEdit,
                btnDelete,
                btnPrev,
                lblPage,
                btnNext,
                lblStudent,
                txtStudentFilter,
                lblCourse,
                txtCourseFilter,
                lblClass,
                txtClassNameFilter,
                lblStatus,
                cmbStatusFilter,
                btnFilter,
                btnClearFilters
            });

            dgvRegs.Dock = DockStyle.Fill;
            dgvRegs.ReadOnly = true;
            dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegs.MultiSelect = false;
            dgvRegs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvRegs);
            Controls.Add(topPanel);
            Name = "RegistrationListForm";
            Text = "Manage Registrations";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async Task LoadRegistrationsAsync(
            int page = 1,
            string? student = null,
            string? course = null,
            string? className = null,
            string? status = null,
            int? selectId = null)
        {
            try
            {
                var result = await RegistrationService.GetAsync(
                    studentFilter: student,
                    courseFilter: course,
                    classFilter: className,
                    statusFilter: status,
                    pageNumber: page,
                    pageSize: PageSize);
                _regs = result.ToList();
                _rows = MapRows(_regs);
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);
                _student = student;
                _course = course;
                _className = className;
                _status = string.IsNullOrWhiteSpace(status) ? "All" : status;
                RefreshGrid();
                UpdatePager();
                if (selectId.HasValue)
                {
                    SelectRowById(selectId.Value);
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error loading registrations due to an unexpected error.");
            }
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static List<RegistrationGridRow> MapRows(IEnumerable<Registration> registrations)
        {
            return ViewMapper.Instance.Map<List<RegistrationGridRow>>(registrations.ToList());
        }

        private void UpdatePager()
        {
            lblPage.Text = $"Page {_page} of {_totalPages}";
            btnPrev.Enabled = _page > 1;
            btnNext.Enabled = _page < _totalPages;
        }

        private void RefreshGrid()
        {
            dgvRegs.DataSource = null;
            dgvRegs.DataSource = _rows;
            ConfigureColumns();
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvRegs.Rows)
            {
                if (row.DataBoundItem is RegistrationGridRow gridRow && gridRow.Id == id)
                {
                    row.Selected = true;
                    dgvRegs.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadRegistrationsAsync(
                1,
                Clean(txtStudentFilter.Text),
                Clean(txtCourseFilter.Text),
                Clean(txtClassNameFilter.Text),
                cmbStatusFilter.SelectedItem?.ToString());
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtStudentFilter.Clear();
            txtCourseFilter.Clear();
            txtClassNameFilter.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            await LoadRegistrationsAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            await LoadRegistrationsAsync(_page - 1, _student, _course, _className, _status);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await LoadRegistrationsAsync(_page + 1, _student, _course, _className, _status);
        }

        private void ConfigureColumns()
        {
            if (dgvRegs.Columns["Id"] != null)
            {
                dgvRegs.Columns["Id"].Visible = true;
            }

            if (dgvRegs.Columns["StudentId"] != null)
            {
                dgvRegs.Columns["StudentId"].Visible = true;
            }

            if (dgvRegs.Columns["ClassId"] != null)
            {
                dgvRegs.Columns["ClassId"].Visible = true;
            }

            if (dgvRegs.Columns["StudentName"] != null)
            {
                dgvRegs.Columns["StudentName"].HeaderText = "Student";
            }

            if (dgvRegs.Columns["ClassName"] != null)
            {
                dgvRegs.Columns["ClassName"].HeaderText = "Class";
            }

            if (dgvRegs.Columns["CourseName"] != null)
            {
                dgvRegs.Columns["CourseName"].HeaderText = "Course";
            }

            if (dgvRegs.Columns["RegistrationDate"] != null)
            {
                dgvRegs.Columns["RegistrationDate"].HeaderText = "Registered Date";
            }

            if (dgvRegs.Columns["Status"] != null)
            {
                dgvRegs.Columns["Status"].HeaderText = "Status";
            }

            if (dgvRegs.Columns["CreatedOn"] != null)
            {
                dgvRegs.Columns["CreatedOn"].Visible = true;
                dgvRegs.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvRegs.Columns["ModifiedOn"] != null)
            {
                dgvRegs.Columns["ModifiedOn"].Visible = true;
                dgvRegs.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvRegs.Columns["CreatedBy"] != null)
            {
                dgvRegs.Columns["CreatedBy"].Visible = true;
                dgvRegs.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvRegs.Columns["ModifiedBy"] != null)
            {
                dgvRegs.Columns["ModifiedBy"].Visible = true;
                dgvRegs.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new RegistrationDetailForm();

            if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
            {
                await LoadRegistrationsAsync(_page, _student, _course, _className, _status, form.SavedEntity.Id);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationGridRow gridRow
                && _regs.FirstOrDefault(r => r.Id == gridRow.Id) is Registration reg)
            {
                using var form = new RegistrationDetailForm(reg);

                if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                {
                    await LoadRegistrationsAsync(_page, _student, _course, _className, _status, form.SavedEntity.Id);
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationGridRow gridRow
                && _regs.FirstOrDefault(r => r.Id == gridRow.Id) is Registration reg)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete registration {reg.Id}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await RegistrationService.DeleteAsync(reg.Id);
                        await LoadRegistrationsAsync(_page, _student, _course, _className, _status);
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting registration due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to delete.");
            }
        }
    }
}
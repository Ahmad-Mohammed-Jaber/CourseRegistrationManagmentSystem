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
    public partial class ClassListForm : Form
    {
        private DataGridView dgvClasses;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtClassNameFilter;
        private TextBox txtInstructorFilter;
        private TextBox txtCourseFilter;
        private ComboBox cmbStatusFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private Panel topPanel;
        private List<Class> _classes = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _className;
        private string? _instructor;
        private string? _course;
        private string? _status = "Active";

        public ClassListForm()
        {
            InitializeComponent();
            _ = LoadClassesAsync();
        }

        private void InitializeComponent()
        {
            dgvClasses = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnBack = new Button();
            txtClassNameFilter = new TextBox();
            txtInstructorFilter = new TextBox();
            txtCourseFilter = new TextBox();
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
            btnAdd.Size = new Size(100, 30);
            btnAdd.Text = "Add Class";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(210, 10);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Text = "Edit Class";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(320, 10);
            btnDelete.Size = new Size(110, 30);
            btnDelete.Text = "Delete Class";
            btnDelete.Click += btnDelete_Click;

            btnPrev.Location = new Point(450, 10);
            btnPrev.Size = new Size(70, 30);
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;

            lblPage.Location = new Point(525, 17);
            lblPage.Size = new Size(150, 20);
            lblPage.TextAlign = ContentAlignment.MiddleCenter;
            lblPage.Text = "Page 1 of 1";

            btnNext.Location = new Point(680, 10);
            btnNext.Size = new Size(70, 30);
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;

            var lblClassName = new Label { Text = "Class Name", Location = new Point(10, 48), Size = new Size(160, 18) };
            txtClassNameFilter.Location = new Point(10, 68);
            txtClassNameFilter.Size = new Size(160, 25);

            var lblInstructor = new Label { Text = "Instructor", Location = new Point(180, 48), Size = new Size(180, 18) };
            txtInstructorFilter.Location = new Point(180, 68);
            txtInstructorFilter.Size = new Size(180, 25);

            var lblCourse = new Label { Text = "Course", Location = new Point(380, 48), Size = new Size(160, 18) };
            txtCourseFilter.Location = new Point(380, 68);
            txtCourseFilter.Size = new Size(160, 25);

            var lblStatus = new Label { Text = "Status", Location = new Point(560, 48), Size = new Size(110, 18) };
            cmbStatusFilter.Location = new Point(560, 68);
            cmbStatusFilter.Size = new Size(110, 25);
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Items.AddRange(new object[] { "Active", "Inactive", "All" });
            cmbStatusFilter.SelectedIndex = 0;

            btnFilter.Location = new Point(690, 66);
            btnFilter.Size = new Size(70, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(770, 66);
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
                lblClassName,
                txtClassNameFilter,
                lblInstructor,
                txtInstructorFilter,
                lblCourse,
                txtCourseFilter,
                lblStatus,
                cmbStatusFilter,
                btnFilter,
                btnClearFilters
            });

            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.ReadOnly = true;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvClasses);
            Controls.Add(topPanel);
            Name = "ClassListForm";
            Text = "Manage Classes";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async Task LoadClassesAsync(
            int page = 1,
            string? className = null,
            string? instructor = null,
            string? course = null,
            string? status = null,
            int? selectId = null)
        {
            try
            {
                var result = await ClassService.GetAsync(
                    statusFilter: string.IsNullOrWhiteSpace(status) ? "Active" : status,
                    classNameFilter: className,
                    instructorFilter: instructor,
                    courseFilter: course,
                    pageNumber: page,
                    pageSize: PageSize);
                _classes = result.ToList();
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);
                _className = className;
                _instructor = instructor;
                _course = course;
                _status = string.IsNullOrWhiteSpace(status) ? "Active" : status;
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
                MessageBox.Show("Error loading classes due to an unexpected error.");
            }
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private void UpdatePager()
        {
            lblPage.Text = $"Page {_page} of {_totalPages}";
            btnPrev.Enabled = _page > 1;
            btnNext.Enabled = _page < _totalPages;
        }

        private void RefreshGrid()
        {
            dgvClasses.DataSource = null;
            dgvClasses.DataSource = ViewMapper.Instance.Map<List<ClassGridRow>>(_classes);
            ConfigureColumns();
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvClasses.Rows)
            {
                if (row.DataBoundItem is ClassGridRow gridRow && gridRow.Id == id)
                {
                    row.Selected = true;
                    dgvClasses.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private void ConfigureColumns()
        {
            if (dgvClasses.Columns["Schedule"] != null)
            {
                dgvClasses.Columns["Schedule"].HeaderText = "Schedule";
            }

            if (dgvClasses.Columns["CurrentCapacity"] != null && dgvClasses.Columns["MaxCapacity"] != null)
            {
                int currentIdx = dgvClasses.Columns["CurrentCapacity"].DisplayIndex;
                int maxIdx = dgvClasses.Columns["MaxCapacity"].DisplayIndex;
                dgvClasses.Columns["CurrentCapacity"].DisplayIndex = maxIdx;
                dgvClasses.Columns["MaxCapacity"].DisplayIndex = currentIdx;
            }

            if (dgvClasses.Columns["CreatedOn"] != null)
            {
                dgvClasses.Columns["CreatedOn"].Visible = true;
                dgvClasses.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvClasses.Columns["ModifiedOn"] != null)
            {
                dgvClasses.Columns["ModifiedOn"].Visible = true;
                dgvClasses.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvClasses.Columns["CreatedBy"] != null)
            {
                dgvClasses.Columns["CreatedBy"].Visible = true;
                dgvClasses.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvClasses.Columns["ModifiedBy"] != null)
            {
                dgvClasses.Columns["ModifiedBy"].Visible = true;
                dgvClasses.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadClassesAsync(
                1,
                Clean(txtClassNameFilter.Text),
                Clean(txtInstructorFilter.Text),
                Clean(txtCourseFilter.Text),
                cmbStatusFilter.SelectedItem?.ToString());
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtClassNameFilter.Clear();
            txtInstructorFilter.Clear();
            txtCourseFilter.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            await LoadClassesAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            await LoadClassesAsync(_page - 1, _className, _instructor, _course, _status);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await LoadClassesAsync(_page + 1, _className, _instructor, _course, _status);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new ClassDetailForm();

            if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
            {
                await LoadClassesAsync(_page, _className, _instructor, _course, _status, form.SavedEntity.Id);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassGridRow gridRow
                && _classes.FirstOrDefault(c => c.Id == gridRow.Id) is Class cls)
            {
                using var form = new ClassDetailForm(cls);

                if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                {
                    await LoadClassesAsync(_page, _className, _instructor, _course, _status, form.SavedEntity.Id);
                }
            }
            else
            {
                MessageBox.Show("Please select a class to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassGridRow gridRow
                && _classes.FirstOrDefault(c => c.Id == gridRow.Id) is Class cls)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete class {cls.ClassName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await ClassService.DeleteAsync(cls.Id);
                        await LoadClassesAsync(_page, _className, _instructor, _course, _status);
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting class due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a class to delete.");
            }
        }
    }
}
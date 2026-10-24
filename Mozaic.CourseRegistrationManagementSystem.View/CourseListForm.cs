using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class CourseListForm : Form
    {

        private DataGridView dgvCourses;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtCodeFilter;
        private TextBox txtNameFilter;
        private ComboBox cmbStatusFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private Panel topPanel;
        private List<Course> _courses = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _code;
        private string? _name;
        private string? _status = "All";

        public CourseListForm()
        {
            InitializeComponent();
            _ = LoadCoursesAsync();
        }

        private void InitializeComponent()
        {
            dgvCourses = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnBack = new Button();
            txtCodeFilter = new TextBox();
            txtNameFilter = new TextBox();
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
            btnAdd.Size = new Size(110, 30);
            btnAdd.Text = "Add Course";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(220, 10);
            btnEdit.Size = new Size(110, 30);
            btnEdit.Text = "Edit Course";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(340, 10);
            btnDelete.Size = new Size(120, 30);
            btnDelete.Text = "Delete Course";
            btnDelete.Click += btnDelete_Click;

            btnPrev.Location = new Point(480, 10);
            btnPrev.Size = new Size(70, 30);
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;

            lblPage.Location = new Point(555, 17);
            lblPage.Size = new Size(150, 20);
            lblPage.TextAlign = ContentAlignment.MiddleCenter;
            lblPage.Text = "Page 1 of 1";

            btnNext.Location = new Point(710, 10);
            btnNext.Size = new Size(70, 30);
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;

            var lblCode = new Label
            {
                Text = "Course Code",
                Location = new Point(10, 48),
                Size = new Size(120, 18)
            };
            txtCodeFilter.Location = new Point(10, 68);
            txtCodeFilter.Size = new Size(120, 25);

            var lblName = new Label
            {
                Text = "Course Name",
                Location = new Point(140, 48),
                Size = new Size(180, 18)
            };
            txtNameFilter.Location = new Point(140, 68);
            txtNameFilter.Size = new Size(180, 25);

            var lblStatus = new Label
            {
                Text = "Status",
                Location = new Point(330, 48),
                Size = new Size(120, 18)
            };
            cmbStatusFilter.Location = new Point(330, 68);
            cmbStatusFilter.Size = new Size(120, 25);
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            cmbStatusFilter.SelectedIndex = 0;

            btnFilter.Location = new Point(460, 66);
            btnFilter.Size = new Size(80, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(550, 66);
            btnClearFilters.Size = new Size(80, 27);
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
                lblCode,
                txtCodeFilter,
                lblName,
                txtNameFilter,
                lblStatus,
                cmbStatusFilter,
                btnFilter,
                btnClearFilters
            });

            dgvCourses.Dock = DockStyle.Fill;
            dgvCourses.ReadOnly = true;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.MultiSelect = false;
            dgvCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvCourses);
            Controls.Add(topPanel);
            Name = "CourseListForm";
            Text = "Manage Courses";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async Task LoadCoursesAsync(
            int page = 1,
            string? code = null,
            string? name = null,
            string? status = null,
            int? selectId = null)
        {
            try
            {
                var result = await CourseService.GetAsync(
                    codeFilter: code,
                    nameFilter: name,
                    isActive: ParseStatus(status),
                    pageNumber: page,
                    pageSize: PageSize);
                _courses = result.ToList();
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);
                _code = code;
                _name = name;
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
                MessageBox.Show("Error loading courses due to an unexpected error.");
            }
        }

        private static bool? ParseStatus(string? status)
        {
            return status?.Trim().ToLowerInvariant() switch
            {
                "active" => true,
                "inactive" => false,
                _ => null,
            };
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
            dgvCourses.DataSource = null;
            dgvCourses.DataSource = _courses;
            ConfigureColumns();
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvCourses.Rows)
            {
                if (row.DataBoundItem is Course entity && entity.Id == id)
                {
                    row.Selected = true;
                    dgvCourses.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private void ConfigureColumns()
        {
            if (dgvCourses.Columns["CreatedOn"] != null)
            {
                dgvCourses.Columns["CreatedOn"].Visible = true;
                dgvCourses.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvCourses.Columns["ModifiedOn"] != null)
            {
                dgvCourses.Columns["ModifiedOn"].Visible = true;
                dgvCourses.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvCourses.Columns["CreatedBy"] != null)
            {
                dgvCourses.Columns["CreatedBy"].Visible = true;
                dgvCourses.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvCourses.Columns["ModifiedBy"] != null)
            {
                dgvCourses.Columns["ModifiedBy"].Visible = true;
                dgvCourses.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadCoursesAsync(
                1,
                Clean(txtCodeFilter.Text),
                Clean(txtNameFilter.Text),
                cmbStatusFilter.SelectedItem?.ToString());
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtCodeFilter.Clear();
            txtNameFilter.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            await LoadCoursesAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            await LoadCoursesAsync(_page - 1, _code, _name, _status);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await LoadCoursesAsync(_page + 1, _code, _name, _status);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new CourseDetailForm();

            if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
            {
                await LoadCoursesAsync(_page, _code, _name, _status, form.SavedEntity.Id);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCourses.CurrentRow?.DataBoundItem is Course course)
            {
                using var form = new CourseDetailForm(course);

                if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                {
                    await LoadCoursesAsync(_page, _code, _name, _status, form.SavedEntity.Id);
                }
            }
            else
            {
                MessageBox.Show("Please select a course to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCourses.CurrentRow?.DataBoundItem is Course course)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete course {course.CourseName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await CourseService.DeleteAsync(course.Id);
                        await LoadCoursesAsync(_page, _code, _name, _status);
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting course due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a course to delete.");
            }
        }
    }
}
using BL.Services;
using CourseRegistrationManagmentSystem.View;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace View
{
    public partial class CourseListForm : Form
    {
        private readonly CourseService _courseService = new CourseService();

        private DataGridView dgvCourses;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;

        public CourseListForm()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void InitializeComponent()
        {
            dgvCourses = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnBack = new Button();
            txtSearch = new TextBox();
            btnSearch = new Button();
            topPanel = new Panel();

            SuspendLayout();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            btnBack.Location = new Point(10, 10);
            btnBack.Size = new Size(80, 30);
            btnBack.Text = "Back";
            btnBack.Click += (s, e) => Close();

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

            txtSearch.Location = new Point(480, 12);
            txtSearch.Size = new Size(150, 25);

            btnSearch.Location = new Point(640, 10);
            btnSearch.Size = new Size(70, 30);
            btnSearch.Text = "Search";
            btnSearch.Click += btnSearch_Click;

            topPanel.Controls.AddRange(new Control[]
            {
                btnBack,
                btnAdd,
                btnEdit,
                btnDelete,
                txtSearch,
                btnSearch
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

        private async void LoadCourses()
        {
            try
            {
                dgvCourses.DataSource = null;
                var loadResult = await _courseService.GetAllAsync();
                if (!loadResult.IsSuccess)
                {
                    MessageBox.Show($"Error loading courses: {loadResult.Message}");
                    return;
                }
                dgvCourses.DataSource = loadResult.Value!.Select(c => c.ToDto()).ToList();
                ConfigureColumns();
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

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchResult = await _courseService.SearchAsync(txtSearch.Text);
                    if (!searchResult.IsSuccess)
                    {
                        MessageBox.Show($"Error searching courses: {searchResult.Message}");
                        return;
                    }
                    dgvCourses.DataSource = searchResult.Value!.Select(c => c.ToDto()).ToList();
                    ConfigureColumns();
                }
                else
                {
                    LoadCourses();
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error searching courses due to an unexpected error.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new CourseDetailForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadCourses();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCourses.CurrentRow?.DataBoundItem is CourseDto course)
            {
                using var form = new CourseDetailForm(course);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCourses();
                }
            }
            else
            {
                MessageBox.Show("Please select a course to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCourses.CurrentRow?.DataBoundItem is CourseDto course)
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
                        var deleteResult = await _courseService.DeleteAsync(course.Id);
                        if (!deleteResult.IsSuccess)
                        {
                            MessageBox.Show($"Error deleting course: {deleteResult.Message}");
                            return;
                        }
                        LoadCourses();
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
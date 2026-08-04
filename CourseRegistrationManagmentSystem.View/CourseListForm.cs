using CourseRegistrationManagmentSystem.Business.Services;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Drawing;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
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

            // topPanel
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            // btnBack
            btnBack.Location = new Point(10, 10);
            btnBack.Size = new Size(80, 30);
            btnBack.Text = "Back";
            btnBack.Click += (s, e) => Close();

            // btnAdd
            btnAdd.Location = new Point(100, 10);
            btnAdd.Size = new Size(110, 30);
            btnAdd.Text = "Add Course";
            btnAdd.Click += btnAdd_Click;

            // btnEdit
            btnEdit.Location = new Point(220, 10);
            btnEdit.Size = new Size(110, 30);
            btnEdit.Text = "Edit Course";
            btnEdit.Click += btnEdit_Click;

            // btnDelete
            btnDelete.Location = new Point(340, 10);
            btnDelete.Size = new Size(120, 30);
            btnDelete.Text = "Delete Course";
            btnDelete.Click += btnDelete_Click;

            // txtSearch
            txtSearch.Location = new Point(480, 12);
            txtSearch.Size = new Size(150, 25);

            // btnSearch
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

            // dgvCourses
            dgvCourses.Dock = DockStyle.Fill;
            dgvCourses.ReadOnly = true;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.MultiSelect = false;
            dgvCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // CourseListForm
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
            dgvCourses.DataSource = null;
            dgvCourses.DataSource = await _courseService.GetAllAsync();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dgvCourses.DataSource = await _courseService.SearchAsync(txtSearch.Text);
            }
            else
            {
                LoadCourses();
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
                    await _courseService.DeleteAsync(course.Id);
                    LoadCourses();
                }
            }
            else
            {
                MessageBox.Show("Please select a course to delete.");
            }
        }
    }
}
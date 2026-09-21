using BL.Services;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace View
{
    public partial class StudentListForm : Form
    {
        private readonly StudentService _studentService = new StudentService();

        private DataGridView dgvStudents;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;

        public StudentListForm()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void InitializeComponent()
        {
            dgvStudents = new DataGridView();
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
            btnAdd.Size = new Size(100, 30);
            btnAdd.Text = "Add Student";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(210, 10);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Text = "Edit Student";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(320, 10);
            btnDelete.Size = new Size(110, 30);
            btnDelete.Text = "Delete Student";
            btnDelete.Click += btnDelete_Click;

            txtSearch.Location = new Point(450, 12);
            txtSearch.Size = new Size(150, 25);

            btnSearch.Location = new Point(610, 10);
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

            dgvStudents.Dock = DockStyle.Fill;
            dgvStudents.ReadOnly = true;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.MultiSelect = false;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.ColumnHeadersHeight = 35;
            dgvStudents.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvStudents.DataBindingComplete += dgvStudents_DataBindingComplete;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvStudents);
            Controls.Add(topPanel);
            Name = "StudentListForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Manage Students";

            ResumeLayout(false);
        }

        private void dgvStudents_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvStudents.Columns.Count == 0)
            {
                return;
            }

            if (dgvStudents.Columns.Contains("Id"))
            {
                dgvStudents.Columns["Id"].DisplayIndex = 0;
            }

            if (dgvStudents.Columns.Contains("UserName"))
            {
                dgvStudents.Columns["UserName"].DisplayIndex = 1;
            }

            if (dgvStudents.Columns.Contains("FullName"))
            {
                dgvStudents.Columns["FullName"].DisplayIndex = 2;
            }

            if (dgvStudents.Columns.Contains("Email"))
            {
                dgvStudents.Columns["Email"].DisplayIndex = 3;
            }

            if (dgvStudents.Columns.Contains("Phone"))
            {
                dgvStudents.Columns["Phone"].DisplayIndex = 4;
            }

            if (dgvStudents.Columns.Contains("Role"))
            {
                dgvStudents.Columns["Role"].DisplayIndex = 5;
            }

            if (dgvStudents.Columns.Contains("IsActive"))
            {
                dgvStudents.Columns["IsActive"].DisplayIndex = 6;
            }

            if (dgvStudents.Columns.Contains("CreatedOn"))
            {
                dgvStudents.Columns["CreatedOn"].Visible = true;
                dgvStudents.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvStudents.Columns.Contains("ModifiedOn"))
            {
                dgvStudents.Columns["ModifiedOn"].Visible = true;
                dgvStudents.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvStudents.Columns.Contains("CreatedBy"))
            {
                dgvStudents.Columns["CreatedBy"].Visible = true;
                dgvStudents.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvStudents.Columns.Contains("ModifiedBy"))
            {
                dgvStudents.Columns["ModifiedBy"].Visible = true;
                dgvStudents.Columns["ModifiedBy"].HeaderText = "Modified By";
            }

            foreach (DataGridViewColumn column in dgvStudents.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private async void LoadStudents()
        {
            try
            {
                dgvStudents.DataSource = null;
                var loadResult = await _studentService.GetAllAsync();
                if (!loadResult.IsSuccess)
                {
                    MessageBox.Show($"Error loading students: {loadResult.Message}");
                    return;
                }
                dgvStudents.DataSource = loadResult.Value!.Select(s => s.ToDto()).ToList();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error loading students due to an unexpected error.");
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchResult = await _studentService.SearchAsync(txtSearch.Text);
                    if (!searchResult.IsSuccess)
                    {
                        MessageBox.Show($"Error searching students: {searchResult.Message}");
                        return;
                    }
                    dgvStudents.DataSource = searchResult.Value!.Select(s => s.ToDto()).ToList();
                }
                else
                {
                    LoadStudents();
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error searching students due to an unexpected error.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new StudentDetailForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadStudents();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow?.DataBoundItem is StudentDto student)
            {
                using var form = new StudentDetailForm(student);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStudents();
                }
            }
            else
            {
                MessageBox.Show("Please select a student to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow?.DataBoundItem is StudentDto student)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete student {student.FullName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var deleteResult = await _studentService.DeleteAsync(student.Id);
                        if (!deleteResult.IsSuccess)
                        {
                            MessageBox.Show($"Error deleting student: {deleteResult.Message}");
                            return;
                        }
                        LoadStudents();
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting student due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.");
            }
        }
    }
}
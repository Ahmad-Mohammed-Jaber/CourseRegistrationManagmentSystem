using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class StudentListForm : Form
    {
        private DataGridView dgvStudents;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtStudentNumberFilter;
        private TextBox txtFullNameFilter;
        private TextBox txtUserNameFilter;
        private TextBox txtEmailFilter;
        private TextBox txtPhoneFilter;
        private ComboBox cmbStatusFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private Panel topPanel;
        private List<Student> _students = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _studentNumber;
        private string? _fullName;
        private string? _userName;
        private string? _email;
        private string? _phone;
        private string? _status = "All";
        private readonly List<Label> _filterLabels = new();

        public StudentListForm()
        {
            InitializeComponent();
            _ = LoadStudentsAsync();
        }

        private void InitializeComponent()
        {
            dgvStudents = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnBack = new Button();
            txtStudentNumberFilter = new TextBox();
            txtFullNameFilter = new TextBox();
            txtUserNameFilter = new TextBox();
            txtEmailFilter = new TextBox();
            txtPhoneFilter = new TextBox();
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

            AddFilterLabel("Student Number", 10);
            txtStudentNumberFilter.Location = new Point(10, 68);
            txtStudentNumberFilter.Size = new Size(110, 25);

            AddFilterLabel("Full Name", 130);
            txtFullNameFilter.Location = new Point(130, 68);
            txtFullNameFilter.Size = new Size(160, 25);

            AddFilterLabel("Username", 300);
            txtUserNameFilter.Location = new Point(300, 68);
            txtUserNameFilter.Size = new Size(140, 25);

            AddFilterLabel("Email", 450);
            txtEmailFilter.Location = new Point(450, 68);
            txtEmailFilter.Size = new Size(180, 25);

            AddFilterLabel("Phone", 640);
            txtPhoneFilter.Location = new Point(640, 68);
            txtPhoneFilter.Size = new Size(130, 25);

            AddFilterLabel("Status", 780);
            cmbStatusFilter.Location = new Point(780, 68);
            cmbStatusFilter.Size = new Size(100, 25);
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            cmbStatusFilter.SelectedIndex = 0;

            btnFilter.Location = new Point(890, 66);
            btnFilter.Size = new Size(70, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(970, 66);
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
                txtStudentNumberFilter,
                txtFullNameFilter,
                txtUserNameFilter,
                txtEmailFilter,
                txtPhoneFilter,
                cmbStatusFilter,
                btnFilter,
                btnClearFilters
            });
            topPanel.Controls.AddRange(_filterLabels.ToArray());

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

        private void AddFilterLabel(string text, int x)
        {
            _filterLabels.Add(new Label
            {
                Text = text,
                Location = new Point(x, 48),
                Size = new Size(160, 18)
            });
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvStudents_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvStudents.Columns.Count == 0)
            {
                return;
            }

            if (dgvStudents.Columns.Contains("PasswordHash"))
            {
                dgvStudents.Columns["PasswordHash"].Visible = false;
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

        private async Task LoadStudentsAsync(
            int page = 1,
            string? studentNumber = null,
            string? fullName = null,
            string? userName = null,
            string? email = null,
            string? phone = null,
            string? status = null,
            int? selectId = null)
        {
            try
            {
                var result = await StudentService.GetAsync(
                    studentNumberFilter: studentNumber,
                    fullNameFilter: fullName,
                    userNameFilter: userName,
                    emailFilter: email,
                    phoneFilter: phone,
                    isActive: ParseStatus(status),
                    pageNumber: page,
                    pageSize: PageSize);
                _students = result.ToList();
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);
                _studentNumber = studentNumber;
                _fullName = fullName;
                _userName = userName;
                _email = email;
                _phone = phone;
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
                MessageBox.Show("Error loading students due to an unexpected error.");
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
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = _students;
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                if (row.DataBoundItem is Student entity && entity.Id == id)
                {
                    row.Selected = true;
                    dgvStudents.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadStudentsAsync(
                1,
                Clean(txtStudentNumberFilter.Text),
                Clean(txtFullNameFilter.Text),
                Clean(txtUserNameFilter.Text),
                Clean(txtEmailFilter.Text),
                Clean(txtPhoneFilter.Text),
                cmbStatusFilter.SelectedItem?.ToString());
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtStudentNumberFilter.Clear();
            txtFullNameFilter.Clear();
            txtUserNameFilter.Clear();
            txtEmailFilter.Clear();
            txtPhoneFilter.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            await LoadStudentsAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            await LoadStudentsAsync(_page - 1, _studentNumber, _fullName, _userName, _email, _phone, _status);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await LoadStudentsAsync(_page + 1, _studentNumber, _fullName, _userName, _email, _phone, _status);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new StudentDetailForm();

            if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
            {
                await LoadStudentsAsync(
                    _page, _studentNumber, _fullName, _userName, _email, _phone, _status, form.SavedEntity.Id);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow?.DataBoundItem is Student student)
            {
                using var form = new StudentDetailForm(student);

                if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                {
                    await LoadStudentsAsync(
                        _page, _studentNumber, _fullName, _userName, _email, _phone, _status, form.SavedEntity.Id);
                }
            }
            else
            {
                MessageBox.Show("Please select a student to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow?.DataBoundItem is Student student)
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
                        await StudentService.DeleteAsync(student.Id);
                        await LoadStudentsAsync(_page, _studentNumber, _fullName, _userName, _email, _phone, _status);
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
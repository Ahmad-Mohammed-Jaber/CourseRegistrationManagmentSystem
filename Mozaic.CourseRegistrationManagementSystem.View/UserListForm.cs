using System.Windows.Forms;
using System.Windows;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.BL.Services;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class UserListForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private DataGridView dgvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtUserNameFilter;
        private TextBox txtFullNameFilter;
        private ComboBox cmbRoleFilter;
        private ComboBox cmbStatusFilter;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private Panel topPanel;
        private List<User> _users = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _userName;
        private string? _fullName;
        private string? _role = "All";
        private string? _status = "All";

        public UserListForm()
        {
            InitializeComponent();
            _ = LoadUsersAsync();
        }

        private void InitializeComponent()
        {
            this.dgvUsers = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnBack = new Button();
            this.txtUserNameFilter = new TextBox();
            this.txtFullNameFilter = new TextBox();
            this.cmbRoleFilter = new ComboBox();
            this.cmbStatusFilter = new ComboBox();
            this.btnFilter = new Button();
            this.btnClearFilters = new Button();
            this.btnPrev = new Button();
            this.btnNext = new Button();
            this.lblPage = new Label();
            this.topPanel = new Panel();
            this.SuspendLayout();

            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 105;

            this.btnBack.Location = new Point(10, 10);
            this.btnBack.Size = new Size(80, 30);
            this.btnBack.Text = "Back";
            this.btnBack.Click += btnBack_Click;

            this.btnAdd.Location = new Point(100, 10);
            this.btnAdd.Size = new Size(100, 30);
            this.btnAdd.Text = "Add User";
            this.btnAdd.Click += btnAdd_Click;

            this.btnEdit.Location = new Point(210, 10);
            this.btnEdit.Size = new Size(100, 30);
            this.btnEdit.Text = "Edit User";
            this.btnEdit.Click += btnEdit_Click;

            this.btnDelete.Location = new Point(320, 10);
            this.btnDelete.Size = new Size(100, 30);
            this.btnDelete.Text = "Delete User";
            this.btnDelete.Click += btnDelete_Click;

            this.btnPrev.Location = new Point(450, 10);
            this.btnPrev.Size = new Size(70, 30);
            this.btnPrev.Text = "Prev";
            this.btnPrev.Click += btnPrev_Click;

            this.lblPage.Location = new Point(525, 17);
            this.lblPage.Size = new Size(150, 20);
            this.lblPage.TextAlign = ContentAlignment.MiddleCenter;
            this.lblPage.Text = "Page 1 of 1";

            this.btnNext.Location = new Point(680, 10);
            this.btnNext.Size = new Size(70, 30);
            this.btnNext.Text = "Next";
            this.btnNext.Click += btnNext_Click;

            var lblUserName = new Label { Text = "Username", Location = new Point(10, 48), Size = new Size(150, 18) };
            this.txtUserNameFilter.Location = new Point(10, 68);
            this.txtUserNameFilter.Size = new Size(150, 25);

            var lblFullName = new Label { Text = "Full Name", Location = new Point(180, 48), Size = new Size(180, 18) };
            this.txtFullNameFilter.Location = new Point(180, 68);
            this.txtFullNameFilter.Size = new Size(180, 25);

            var lblRole = new Label { Text = "Role", Location = new Point(380, 48), Size = new Size(120, 18) };
            this.cmbRoleFilter.Location = new Point(380, 68);
            this.cmbRoleFilter.Size = new Size(120, 25);
            this.cmbRoleFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbRoleFilter.Items.AddRange(new object[] { "All", "Admin", "Student" });
            this.cmbRoleFilter.SelectedIndex = 0;

            var lblStatus = new Label { Text = "Status", Location = new Point(520, 48), Size = new Size(110, 18) };
            this.cmbStatusFilter.Location = new Point(520, 68);
            this.cmbStatusFilter.Size = new Size(110, 25);
            this.cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatusFilter.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            this.cmbStatusFilter.SelectedIndex = 0;

            this.btnFilter.Location = new Point(650, 66);
            this.btnFilter.Size = new Size(70, 27);
            this.btnFilter.Text = "Filter";
            this.btnFilter.Click += btnFilter_Click;

            this.btnClearFilters.Location = new Point(730, 66);
            this.btnClearFilters.Size = new Size(70, 27);
            this.btnClearFilters.Text = "Clear";
            this.btnClearFilters.Click += btnClearFilters_Click;

            this.topPanel.Controls.AddRange(new Control[]
            {
                btnBack, btnAdd, btnEdit, btnDelete,
                btnPrev, lblPage, btnNext,
                lblUserName, txtUserNameFilter,
                lblFullName, txtFullNameFilter,
                lblRole, cmbRoleFilter,
                lblStatus, cmbStatusFilter,
                btnFilter, btnClearFilters
            });

            this.dgvUsers.Dock = DockStyle.Fill;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.MultiSelect = false;
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.topPanel);
            this.Name = "UserListForm";
            this.Text = "Manage Users";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async Task LoadUsersAsync(
            int page = 1,
            string? userName = null,
            string? fullName = null,
            string? role = null,
            string? status = null,
            int? selectId = null)
        {
            try
            {
                var result = await UserService.GetAsync(
                    userNameFilter: userName,
                    fullNameFilter: fullName,
                    role: ParseRole(role),
                    isActive: ParseStatus(status),
                    pageNumber: page,
                    pageSize: PageSize);
                _users = result.ToList();
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);
                _userName = userName;
                _fullName = fullName;
                _role = string.IsNullOrWhiteSpace(role) ? "All" : role;
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
                MessageBox.Show("Error loading users due to an unexpected error.");
            }
        }

        private static User.UserRoles? ParseRole(string? role)
        {
            return role?.Trim().ToLowerInvariant() switch
            {
                "admin" => User.UserRoles.Admin,
                "student" => User.UserRoles.Student,
                _ => null,
            };
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
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = _users;
            ConfigureColumns();
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.DataBoundItem is User entity && entity.Id == id)
                {
                    row.Selected = true;
                    dgvUsers.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private void ConfigureColumns()
        {
            if (dgvUsers.Columns["PasswordHash"] != null)
            {
                dgvUsers.Columns["PasswordHash"].Visible = false;
            }

            if (dgvUsers.Columns["CreatedOn"] != null)
            {
                dgvUsers.Columns["CreatedOn"].Visible = true;
                dgvUsers.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvUsers.Columns["ModifiedOn"] != null)
            {
                dgvUsers.Columns["ModifiedOn"].Visible = true;
                dgvUsers.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvUsers.Columns["CreatedBy"] != null)
            {
                dgvUsers.Columns["CreatedBy"].Visible = true;
                dgvUsers.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvUsers.Columns["ModifiedBy"] != null)
            {
                dgvUsers.Columns["ModifiedBy"].Visible = true;
                dgvUsers.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync(
                1,
                Clean(txtUserNameFilter.Text),
                Clean(txtFullNameFilter.Text),
                cmbRoleFilter.SelectedItem?.ToString(),
                cmbStatusFilter.SelectedItem?.ToString());
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtUserNameFilter.Clear();
            txtFullNameFilter.Clear();
            cmbRoleFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            await LoadUsersAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync(_page - 1, _userName, _fullName, _role, _status);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync(_page + 1, _userName, _fullName, _role, _status);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new UserDetailForm())
            {
                if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                {
                    await LoadUsersAsync(_page, _userName, _fullName, _role, _status, form.SavedEntity.Id);
                }
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is User user)
            {
                using (var form = new UserDetailForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK && form.SavedEntity != null)
                    {
                        await LoadUsersAsync(_page, _userName, _fullName, _role, _status, form.SavedEntity.Id);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.");
            }
        }
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is User user)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete user {user.UserName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await UserService.DeleteAsync(user.Id);
                        await LoadUsersAsync(_page, _userName, _fullName, _role, _status);
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting user due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }
    }
}

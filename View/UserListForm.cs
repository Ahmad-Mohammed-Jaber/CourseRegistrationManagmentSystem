using System.Windows.Forms;
using System.Windows;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logging;
using BL.Services;

namespace View
{
    public partial class UserListForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private DataGridView dgvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;

        public UserListForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.dgvUsers = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnBack = new Button();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.topPanel = new Panel();
            this.SuspendLayout();

            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 50;

            this.btnBack.Location = new Point(10, 10);
            this.btnBack.Size = new Size(80, 30);
            this.btnBack.Text = "Back";
            this.btnBack.Click += (s, e) => this.Close();

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

            this.txtSearch.Location = new Point(430, 10);
            this.txtSearch.Size = new Size(120, 25);

            this.btnSearch.Location = new Point(560, 10);
            this.btnSearch.Size = new Size(70, 30);
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += btnSearch_Click;

            this.topPanel.Controls.AddRange(new Control[] { btnBack, btnAdd, btnEdit, btnDelete, txtSearch, btnSearch });

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

        private async void LoadUsers()
        {
            try
            {
                dgvUsers.DataSource = null;
                var users = await UserService.GetAllAsync();
                dgvUsers.DataSource = users.Select(u => u.ToDto()).ToList();
                ConfigureColumns();
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

        private void ConfigureColumns()
        {
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

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var users = await UserService.SearchAsync(txtSearch.Text);
                    dgvUsers.DataSource = users.Select(u => u.ToDto()).ToList();
                    ConfigureColumns();
                }
                else
                {
                    LoadUsers();
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error searching users due to an unexpected error.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new UserDetailForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is UserDto user)
            {
                using (var form = new UserDetailForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsers();
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
            if (dgvUsers.CurrentRow?.DataBoundItem is UserDto user)
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
                        var deleteResult = await UserService.DeleteAsync(user.Id);
                        if (!deleteResult.IsSuccess)
                        {
                            MessageBox.Show($"Error deleting user: {deleteResult.Message}");
                            return;
                        }
                        LoadUsers();
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

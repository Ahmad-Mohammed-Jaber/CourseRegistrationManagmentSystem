using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Windows.Forms;
using System.Windows;
using CourseRegistrationManagmentSystem.Business.Services;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class UserListForm : Form
    {
        private readonly UserService _userService = new UserService();
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

            // topPanel
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 50;

            // btnBack
            this.btnBack.Location = new Point(10, 10);
            this.btnBack.Size = new Size(80, 30);
            this.btnBack.Text = "Back";
            this.btnBack.Click += (s, e) => this.Close();

            // btnAdd
            this.btnAdd.Location = new Point(100, 10);
            this.btnAdd.Size = new Size(100, 30);
            this.btnAdd.Text = "Add User";
            this.btnAdd.Click += btnAdd_Click;

            // btnEdit
            this.btnEdit.Location = new Point(210, 10);
            this.btnEdit.Size = new Size(100, 30);
            this.btnEdit.Text = "Edit User";
            this.btnEdit.Click += btnEdit_Click;

            // btnDelete
            this.btnDelete.Location = new Point(320, 10);
            this.btnDelete.Size = new Size(100, 30);
            this.btnDelete.Text = "Delete User";
            this.btnDelete.Click += btnDelete_Click;

            // txtSearch
            this.txtSearch.Location = new Point(430, 10);
            this.txtSearch.Size = new Size(120, 25);

            // btnSearch
            this.btnSearch.Location = new Point(560, 10);
            this.btnSearch.Size = new Size(70, 30);
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += btnSearch_Click;

            this.topPanel.Controls.AddRange(new Control[] { btnBack, btnAdd, btnEdit, btnDelete, txtSearch, btnSearch });

            // dgvUsers
            this.dgvUsers.Dock = DockStyle.Fill;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.MultiSelect = false;
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // UserListForm
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
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = await _userService.GetAllAsync();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dgvUsers.DataSource = await _userService.SearchAsync(txtSearch.Text);
            }
            else
            {
                LoadUsers();
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
                // Windows Forms MessageBox returns DialogResult
                var result = MessageBox.Show(
                    $"Are you sure you want to delete user {user.UserName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question // Optional: adds a warning icon
                );

                // Use DialogResult, not MessageBoxResult
                if (result == DialogResult.Yes)
                {
                    await _userService.DeleteAsync(user.Id);
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }
    }
}

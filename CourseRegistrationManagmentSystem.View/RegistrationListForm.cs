using CourseRegistrationManagmentSystem.Business.Services;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Drawing;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class RegistrationListForm : Form
    {
        private readonly RegistrationService _regService = new RegistrationService();

        private DataGridView dgvRegs;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;

        public RegistrationListForm()
        {
            InitializeComponent();
            LoadRegistrations();
        }

        private void InitializeComponent()
        {
            dgvRegs = new DataGridView();
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
            btnAdd.Size = new Size(130, 30);
            btnAdd.Text = "Add Registration";
            btnAdd.Click += btnAdd_Click;

            // btnEdit
            btnEdit.Location = new Point(240, 10);
            btnEdit.Size = new Size(130, 30);
            btnEdit.Text = "Edit Registration";
            btnEdit.Click += btnEdit_Click;

            // btnDelete
            btnDelete.Location = new Point(380, 10);
            btnDelete.Size = new Size(140, 30);
            btnDelete.Text = "Delete Registration";
            btnDelete.Click += btnDelete_Click;

            // txtSearch
            txtSearch.Location = new Point(530, 12);
            txtSearch.Size = new Size(150, 25);

            // btnSearch
            btnSearch.Location = new Point(690, 10);
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

            // dgvRegs
            dgvRegs.Dock = DockStyle.Fill;
            dgvRegs.ReadOnly = true;
            dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegs.MultiSelect = false;
            dgvRegs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // RegistrationListForm
            ClientSize = new Size(900, 500);
            Controls.Add(dgvRegs);
            Controls.Add(topPanel);
            Name = "RegistrationListForm";
            Text = "Manage Registrations";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private async void LoadRegistrations()
        {
            dgvRegs.DataSource = null;
            dgvRegs.DataSource = await _regService.GetAllAsync();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dgvRegs.DataSource = await _regService.SearchAsync(txtSearch.Text);
            }
            else
            {
                LoadRegistrations();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new RegistrationDetailForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadRegistrations();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationDto reg)
            {
                using var form = new RegistrationDetailForm(reg);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadRegistrations();
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationDto reg)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete registration {reg.Id}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _regService.DeleteAsync(reg.Id);
                    LoadRegistrations();
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to delete.");
            }
        }
    }
}
using CourseRegistrationManagmentSystem.Business.Services.AdminServices;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Drawing;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class ClassListForm : Form
    {
        private readonly ClassService _classService = new ClassService();

        private DataGridView dgvClasses;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;

        public ClassListForm()
        {
            InitializeComponent();
            LoadClasses();
        }

        private void InitializeComponent()
        {
            dgvClasses = new DataGridView();
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
            btnAdd.Size = new Size(100, 30);
            btnAdd.Text = "Add Class";
            btnAdd.Click += btnAdd_Click;

            // btnEdit
            btnEdit.Location = new Point(210, 10);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Text = "Edit Class";
            btnEdit.Click += btnEdit_Click;

            // btnDelete
            btnDelete.Location = new Point(320, 10);
            btnDelete.Size = new Size(110, 30);
            btnDelete.Text = "Delete Class";
            btnDelete.Click += btnDelete_Click;

            // txtSearch
            txtSearch.Location = new Point(450, 12);
            txtSearch.Size = new Size(150, 25);

            // btnSearch
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

            // dgvClasses
            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.ReadOnly = true;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ClassListForm
            ClientSize = new Size(900, 500);
            Controls.Add(dgvClasses);
            Controls.Add(topPanel);
            Name = "ClassListForm";
            Text = "Manage Classes";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private async void LoadClasses()
        {
            dgvClasses.DataSource = null;
            dgvClasses.DataSource = await _classService.GetAllAsync();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dgvClasses.DataSource = await _classService.SearchAsync(txtSearch.Text);
            }
            else
            {
                LoadClasses();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new ClassDetailForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadClasses();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassDto cls)
            {
                using var form = new ClassDetailForm(cls);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClasses();
                }
            }
            else
            {
                MessageBox.Show("Please select a class to edit.");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassDto cls)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete class {cls.ClassName}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _classService.DeleteAsync(cls.Id);
                    LoadClasses();
                }
            }
            else
            {
                MessageBox.Show("Please select a class to delete.");
            }
        }
    }
}
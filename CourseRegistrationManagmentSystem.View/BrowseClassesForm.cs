using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Business.Services;
using System.Drawing;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class BrowseClassesForm : Form
    {
        private readonly StudentRegistrationService _regService = new();
        private readonly ClassService _classService = new();

        private DataGridView dgvClasses;
        private Button btnRegister;
        private Panel topPanel;

        public BrowseClassesForm()
        {
            InitializeComponent();
            LoadAvailableClasses();
        }

        private void InitializeComponent()
        {
            dgvClasses = new DataGridView();
            btnRegister = new Button();
            topPanel = new Panel();

            SuspendLayout();

            // topPanel
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            // btnRegister
            btnRegister.Location = new Point(10, 10);
            btnRegister.Size = new Size(140, 30);
            btnRegister.Text = "Register for Class";
            btnRegister.Click += btnRegister_Click;

            topPanel.Controls.Add(btnRegister);

            // dgvClasses
            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.ReadOnly = true;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClasses.ColumnHeadersHeight = 35;
            dgvClasses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvClasses.DataBindingComplete += dgvClasses_DataBindingComplete;

            // BrowseClassesForm
            ClientSize = new Size(850, 1000);
            Controls.Add(dgvClasses);
            Controls.Add(topPanel);
            Text = "Available Classes";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void dgvClasses_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvClasses.Columns.Count == 0)
                return;

            if (dgvClasses.Columns.Contains("Id"))
                dgvClasses.Columns["Id"].DisplayIndex = 0;

            if (dgvClasses.Columns.Contains("ClassName"))
                dgvClasses.Columns["ClassName"].DisplayIndex = 1;

            if (dgvClasses.Columns.Contains("CourseName"))
                dgvClasses.Columns["CourseName"].DisplayIndex = 2;

            if (dgvClasses.Columns.Contains("InstructorName"))
                dgvClasses.Columns["InstructorName"].DisplayIndex = 3;

            if (dgvClasses.Columns.Contains("Schedule"))
                dgvClasses.Columns["Schedule"].DisplayIndex = 4;

            foreach (DataGridViewColumn column in dgvClasses.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private async void LoadAvailableClasses()
        {
            dgvClasses.DataSource = null;
            dgvClasses.DataSource = await _classService.GetAllAsync();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassDto cls)
            {
                try
                {
                    await _regService.RegisterClass(cls.Id);
                    MessageBox.Show("Registered successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Registration failed: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a class to register.");
            }
        }
    }
}
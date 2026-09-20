using System.Drawing;
using System.Windows.Forms;
using Shared.Dtos;
using BL.Services;

namespace View
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
            ClientSize = new Size(1500, 1000);
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


            // Headers
            if (dgvClasses.Columns["Id"] != null)
                dgvClasses.Columns["Id"].HeaderText = "Class ID";

            if (dgvClasses.Columns["ClassName"] != null)
                dgvClasses.Columns["ClassName"].HeaderText = "Class";

            if (dgvClasses.Columns["CourseName"] != null)
                dgvClasses.Columns["CourseName"].HeaderText = "Course";

            if (dgvClasses.Columns["InstructorName"] != null)
                dgvClasses.Columns["InstructorName"].HeaderText = "Instructor";

            if (dgvClasses.Columns["Schedule"] != null)
                dgvClasses.Columns["Schedule"].HeaderText = "Schedule";

            if (dgvClasses.Columns["CurrentCapacity"] != null)
                dgvClasses.Columns["CurrentCapacity"].HeaderText = "Current Capacity";

            if (dgvClasses.Columns["MaxCapacity"] != null)
                dgvClasses.Columns["MaxCapacity"].HeaderText = "Max Capacity";


            // Ordering
            int index = 0;

            if (dgvClasses.Columns["Id"] != null)
                dgvClasses.Columns["Id"].DisplayIndex = index++;

            if (dgvClasses.Columns["ClassName"] != null)
                dgvClasses.Columns["ClassName"].DisplayIndex = index++;

            if (dgvClasses.Columns["CourseName"] != null)
                dgvClasses.Columns["CourseName"].DisplayIndex = index++;

            if (dgvClasses.Columns["InstructorName"] != null)
                dgvClasses.Columns["InstructorName"].DisplayIndex = index++;

            if (dgvClasses.Columns["Schedule"] != null)
                dgvClasses.Columns["Schedule"].DisplayIndex = index++;

            if (dgvClasses.Columns["CurrentCapacity"] != null)
                dgvClasses.Columns["CurrentCapacity"].DisplayIndex = index++;

            if (dgvClasses.Columns["MaxCapacity"] != null)
                dgvClasses.Columns["MaxCapacity"].DisplayIndex = index++;


            foreach (DataGridViewColumn column in dgvClasses.Columns)
            {
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }
        }
private async void LoadAvailableClasses()
        {
            dgvClasses.DataSource = null;
            var classes = await _classService.GetAllAsync();
            dgvClasses.DataSource = classes.Select(c => c.ToDto()).ToList();
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
using CourseRegistrationManagmentSystem.Business.Services;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Drawing;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class MyRegistrationsForm : Form
    {
        private readonly StudentRegistrationService _regService = new StudentRegistrationService();

        private DataGridView dgvRegs;
        private Button btnDrop;
        private Panel topPanel;

        public MyRegistrationsForm()
        {
            InitializeComponent();
            _ = LoadMyRegistrationsAsync();
        }

        private void InitializeComponent()
        {
            dgvRegs = new DataGridView();
            btnDrop = new Button();
            topPanel = new Panel();

            SuspendLayout();

            // topPanel
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            // btnDrop
            btnDrop.Location = new Point(10, 10);
            btnDrop.Size = new Size(110, 30);
            btnDrop.Text = "Drop Class";
            btnDrop.Click += btnDrop_Click;

            topPanel.Controls.Add(btnDrop);

            // dgvRegs
            dgvRegs.Dock = DockStyle.Fill;
            dgvRegs.ReadOnly = true;
            dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegs.MultiSelect = false;
            dgvRegs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRegs.ColumnHeadersHeight = 35;
            dgvRegs.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvRegs.DataBindingComplete += dgvRegs_DataBindingComplete;

            // MyRegistrationsForm
            ClientSize = new Size(850, 500);
            Controls.Add(dgvRegs);
            Controls.Add(topPanel);
            Text = "My Registrations";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void dgvRegs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvRegs.Columns.Count == 0)
                return;

            // Adjust based on RegistrationDetailsDto properties
            if (dgvRegs.Columns.Contains("Id"))
                dgvRegs.Columns["Id"].DisplayIndex = 0;

            if (dgvRegs.Columns.Contains("CourseName"))
                dgvRegs.Columns["CourseName"].DisplayIndex = 1;

            if (dgvRegs.Columns.Contains("ClassName"))
                dgvRegs.Columns["ClassName"].DisplayIndex = 2;

            if (dgvRegs.Columns.Contains("InstructorName"))
                dgvRegs.Columns["InstructorName"].DisplayIndex = 3;

            if (dgvRegs.Columns.Contains("Schedule"))
                dgvRegs.Columns["Schedule"].DisplayIndex = 4;

            foreach (DataGridViewColumn column in dgvRegs.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private async Task LoadMyRegistrationsAsync()
        {
            dgvRegs.DataSource = null;

            List<RegistrationDetailsDto> registrations =
                await _regService.GetRegistrationsAsync();

            dgvRegs.DataSource = registrations;
        }

        private async void btnDrop_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationDetailsDto reg)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to drop this class?",
                    "Confirm Drop",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _regService.DropRegistration(reg.RegistrationId);
                    await LoadMyRegistrationsAsync();
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to drop.");
            }
        }
    }
}

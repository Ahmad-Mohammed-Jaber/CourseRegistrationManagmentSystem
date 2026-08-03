using CourseRegistrationManagmentSystem.Business.Services.StudentServices;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Session;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class MyRegistrationsForm : Form
    {
        private readonly RegistrationService _regService = new RegistrationService();
        private DataGridView dgvRegs;
        private Button btnDrop;

        public MyRegistrationsForm()
        {
            InitializeComponent();
            LoadMyRegistrations();
        }

        private void InitializeComponent()
        {
            this.dgvRegs = new DataGridView();
            this.btnDrop = new Button();
            this.SuspendLayout();

            this.dgvRegs.Location = new Point(12, 12);
            this.dgvRegs.Size = new Size(560, 300);
            this.dgvRegs.ReadOnly = true;
            this.dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRegs.MultiSelect = false;

            this.btnDrop.Location = new Point(12, 320);
            this.btnDrop.Size = new Size(100, 30);
            this.btnDrop.Text = "Drop Class";
            this.btnDrop.Click += btnDrop_Click;

            this.ClientSize = new Size(585, 360);
            this.Controls.AddRange(new Control[] { dgvRegs, btnDrop });
            this.Text = "My Registrations";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void LoadMyRegistrations()
        {
            // Note: RegistrationService for students doesn't have a GetAll for the student.
            // In a real app, we'd add a method to fetch registrations by student ID.
            // For now, we'll simulate by filtering all (if we had access) or just show a message.
            MessageBox.Show("Fetching my registrations... (Backend needs a GetByStudentId method)");
            dgvRegs.DataSource = null;
        }

        private async void btnDrop_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationDto reg)
            {
                if (MessageBox.Show($"Drop this class?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _regService.DropRegistration(reg.Id);
                    LoadMyRegistrations();
                }
            }
        }
    }
}

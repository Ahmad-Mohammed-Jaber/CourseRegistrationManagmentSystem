using CourseRegistrationManagmentSystem.Business.Services.StudentServices;
using CourseRegistrationManagmentSystem.Business.Services.AdminServices;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem.View
{
    public partial class BrowseClassesForm : Form
    {
        private readonly Business.Services.StudentServices.RegistrationService _regService = new ();
        private readonly ClassService _classService = new ClassService();
        private DataGridView dgvClasses;
        private Button btnRegister;

        public BrowseClassesForm()
        {
            InitializeComponent();
            LoadAvailableClasses();
        }

        private void InitializeComponent()
        {
            this.dgvClasses = new DataGridView();
            this.btnRegister = new Button();
            this.SuspendLayout();

            this.dgvClasses.Location = new Point(12, 12);
            this.dgvClasses.Size = new Size(560, 300);
            this.dgvClasses.ReadOnly = true;
            this.dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvClasses.MultiSelect = false;

            this.btnRegister.Location = new Point(12, 320);
            this.btnRegister.Size = new Size(120, 30);
            this.btnRegister.Text = "Register for Class";
            this.btnRegister.Click += btnRegister_Click;

            this.ClientSize = new Size(585, 360);
            this.Controls.AddRange(new Control[] { dgvClasses, btnRegister });
            this.Text = "Available Classes";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
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
        }
    }
}

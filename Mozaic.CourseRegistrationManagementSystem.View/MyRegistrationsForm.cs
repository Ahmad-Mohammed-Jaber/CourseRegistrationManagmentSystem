using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class MyRegistrationsForm : Form
    {
        private DataGridView dgvRegs;
        private Button btnDrop;
        private Panel topPanel;
        private List<(Registration Registration, Class Class)> _data = new();

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

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            btnDrop.Location = new Point(10, 10);
            btnDrop.Size = new Size(110, 30);
            btnDrop.Text = "Drop Class";
            btnDrop.Click += btnDrop_Click;

            topPanel.Controls.Add(btnDrop);

            dgvRegs.Dock = DockStyle.Fill;
            dgvRegs.ReadOnly = true;
            dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegs.MultiSelect = false;
            dgvRegs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvRegs.ColumnHeadersHeight = 35;
            dgvRegs.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgvRegs.DataBindingComplete += dgvRegs_DataBindingComplete;

            ClientSize = new Size(1500, 1000);
            Controls.Add(dgvRegs);
            Controls.Add(topPanel);

            Name = "MyRegistrationsForm";
            Text = "My Registrations";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }


        private async Task LoadMyRegistrationsAsync()
        {
            try
            {
                _data = await StudentRegistrationService.GetRegistrationsAsync();

                RefreshGrid();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error loading registrations due to an unexpected error.");
            }
        }

        private void RefreshGrid()
        {
            dgvRegs.DataSource = null;
            dgvRegs.DataSource = _data
                .Select(x => new
                {
                    RegistrationId = x.Registration.Id,
                    ClassName = x.Class.ClassName,
                    Instructor = x.Class.Instructor,
                    Schedule = x.Class.ScheduleDisplay,
                    StartDate = x.Class.StartDate,
                    EndDate = x.Class.EndDate,
                    RegistrationDate = x.Registration.RegistrationDate,
                    Status = x.Registration.Status
                })
                .ToList();
        }


        private void dgvRegs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ConfigureColumns();
        }


        private void ConfigureColumns()
        {
            if (dgvRegs.Columns.Count == 0)
            {
                return;
            }

            if (dgvRegs.Columns["RegistrationId"] != null)
            {
                dgvRegs.Columns["RegistrationId"].HeaderText = "Registration ID";
            }

            if (dgvRegs.Columns["CourseName"] != null)
            {
                dgvRegs.Columns["CourseName"].HeaderText = "Course";
            }

            if (dgvRegs.Columns["ClassName"] != null)
            {
                dgvRegs.Columns["ClassName"].HeaderText = "Class";
            }

            if (dgvRegs.Columns["Instructor"] != null)
            {
                dgvRegs.Columns["Instructor"].HeaderText = "Instructor";
            }

            if (dgvRegs.Columns["InstructorName"] != null)
            {
                dgvRegs.Columns["InstructorName"].HeaderText = "Instructor";
            }

            if (dgvRegs.Columns["Schedule"] != null)
            {
                dgvRegs.Columns["Schedule"].HeaderText = "Schedule";
            }

            if (dgvRegs.Columns["RegistrationDate"] != null)
            {
                dgvRegs.Columns["RegistrationDate"].HeaderText = "Registered Date";
            }

            if (dgvRegs.Columns["Status"] != null)
            {
                dgvRegs.Columns["Status"].HeaderText = "Status";
            }

            int index = 0;

            if (dgvRegs.Columns["RegistrationId"] != null)
            {
                dgvRegs.Columns["RegistrationId"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["CourseName"] != null)
            {
                dgvRegs.Columns["CourseName"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["ClassName"] != null)
            {
                dgvRegs.Columns["ClassName"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["Instructor"] != null)
            {
                dgvRegs.Columns["Instructor"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["InstructorName"] != null)
            {
                dgvRegs.Columns["InstructorName"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["Schedule"] != null)
            {
                dgvRegs.Columns["Schedule"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["RegistrationDate"] != null)
            {
                dgvRegs.Columns["RegistrationDate"].DisplayIndex = index++;
            }

            if (dgvRegs.Columns["Status"] != null)
            {
                dgvRegs.Columns["Status"].DisplayIndex = index++;
            }

            foreach (DataGridViewColumn column in dgvRegs.Columns)
            {
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }
        }


        private async void btnDrop_Click(object sender, EventArgs e)
        {
            var item = dgvRegs.CurrentRow?.DataBoundItem;
            if (item == null)
            {
                MessageBox.Show("Please select a registration to drop.");
                return;
            }

            var idProp = item.GetType().GetProperty("RegistrationId");
            if (idProp == null || idProp.GetValue(item) is not int registrationId)
            {
                MessageBox.Show("Please select a registration to drop.");
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to drop this class?",
                "Confirm Drop",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);


            if (result == DialogResult.Yes)
            {
                try
                {
                    await StudentRegistrationService.DropRegistration(registrationId);
                    _data.RemoveAll(r => r.Registration.Id == registrationId);
                    RefreshGrid();
                }
                catch (BusinessException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    AppLogger.LogViewError(ex);
                    MessageBox.Show("Drop failed due to an unexpected error.");
                }
            }
        }
    }
}
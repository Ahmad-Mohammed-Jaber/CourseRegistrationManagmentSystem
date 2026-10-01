using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Dtos;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class RegistrationListForm : Form
    {
        private DataGridView dgvRegs;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private TextBox txtSearch;
        private Button btnSearch;
        private Panel topPanel;
        private List<RegistrationDto> _regs = new();

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

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            btnBack.Location = new Point(10, 10);
            btnBack.Size = new Size(80, 30);
            btnBack.Text = "Back";
            btnBack.Click += (s, e) => Close();

            btnAdd.Location = new Point(100, 10);
            btnAdd.Size = new Size(130, 30);
            btnAdd.Text = "Add Registration";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(240, 10);
            btnEdit.Size = new Size(130, 30);
            btnEdit.Text = "Edit Registration";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(380, 10);
            btnDelete.Size = new Size(140, 30);
            btnDelete.Text = "Delete Registration";
            btnDelete.Click += btnDelete_Click;

            txtSearch.Location = new Point(530, 12);
            txtSearch.Size = new Size(150, 25);

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

            dgvRegs.Dock = DockStyle.Fill;
            dgvRegs.ReadOnly = true;
            dgvRegs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRegs.MultiSelect = false;
            dgvRegs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvRegs);
            Controls.Add(topPanel);
            Name = "RegistrationListForm";
            Text = "Manage Registrations";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private async void LoadRegistrations()
        {
            try
            {
                var details = await RegistrationService.GetAllDetailedAsync();
                _regs = details
                    .Select(x => new RegistrationDto
                    {
                        Id = x.Registration.Id,
                        StudentId = x.Registration.StudentId,
                        StudentUserName = x.Student.UserName,
                        ClassId = x.Registration.ClassId,
                        ClassName = x.Class.ClassName,
                        CourseName = x.Course.CourseName,
                        RegistrationDate = x.Registration.RegistrationDate,
                        Status = x.Registration.Status,
                        CreatedOn = x.Registration.CreatedOn,
                        ModifiedOn = x.Registration.ModifiedOn,
                        CreatedBy = x.Registration.CreatedBy,
                        ModifiedBy = x.Registration.ModifiedBy
                    })
                    .ToList();

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
            dgvRegs.DataSource = _regs;
            ConfigureColumns();
        }

        private void SelectRowById(int id)
        {
            foreach (DataGridViewRow row in dgvRegs.Rows)
            {
                if (row.DataBoundItem is RegistrationDto dto && dto.Id == id)
                {
                    row.Selected = true;
                    dgvRegs.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var details = await RegistrationService.SearchDetailedAsync(txtSearch.Text);
                    _regs = details
                        .Select(x => new RegistrationDto
                        {
                            Id = x.Registration.Id,
                            StudentId = x.Registration.StudentId,
                            StudentUserName = x.Student.UserName,
                            ClassId = x.Registration.ClassId,
                            ClassName = x.Class.ClassName,
                            CourseName = x.Course.CourseName,
                            RegistrationDate = x.Registration.RegistrationDate,
                            Status = x.Registration.Status,
                            CreatedOn = x.Registration.CreatedOn,
                            ModifiedOn = x.Registration.ModifiedOn,
                            CreatedBy = x.Registration.CreatedBy,
                            ModifiedBy = x.Registration.ModifiedBy
                        })
                        .ToList();

                    RefreshGrid();
                }
                else
                {
                    LoadRegistrations();
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error searching registrations due to an unexpected error.");
            }
        }

        private void ConfigureColumns()
        {
            if (dgvRegs.Columns["Id"] != null)
            {
                dgvRegs.Columns["Id"].Visible = true;
            }

            if (dgvRegs.Columns["StudentId"] != null)
            {
                dgvRegs.Columns["StudentId"].Visible = true;
            }

            if (dgvRegs.Columns["ClassId"] != null)
            {
                dgvRegs.Columns["ClassId"].Visible = true;
            }

            if (dgvRegs.Columns["StudentName"] != null)
            {
                dgvRegs.Columns["StudentName"].HeaderText = "Student";
            }

            if (dgvRegs.Columns["ClassName"] != null)
            {
                dgvRegs.Columns["ClassName"].HeaderText = "Class";
            }

            if (dgvRegs.Columns["CourseName"] != null)
            {
                dgvRegs.Columns["CourseName"].HeaderText = "Course";
            }

            if (dgvRegs.Columns["RegistrationDate"] != null)
            {
                dgvRegs.Columns["RegistrationDate"].HeaderText = "Registered Date";
            }

            if (dgvRegs.Columns["Status"] != null)
            {
                dgvRegs.Columns["Status"].HeaderText = "Status";
            }

            if (dgvRegs.Columns["CreatedOn"] != null)
            {
                dgvRegs.Columns["CreatedOn"].Visible = true;
                dgvRegs.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvRegs.Columns["ModifiedOn"] != null)
            {
                dgvRegs.Columns["ModifiedOn"].Visible = true;
                dgvRegs.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvRegs.Columns["CreatedBy"] != null)
            {
                dgvRegs.Columns["CreatedBy"].Visible = true;
                dgvRegs.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvRegs.Columns["ModifiedBy"] != null)
            {
                dgvRegs.Columns["ModifiedBy"].Visible = true;
                dgvRegs.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new RegistrationDetailForm();

            if (form.ShowDialog() == DialogResult.OK && form.SavedDto != null)
            {
                _regs.Add(form.SavedDto);
                RefreshGrid();
                SelectRowById(form.SavedDto.Id);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRegs.CurrentRow?.DataBoundItem is RegistrationDto reg)
            {
                using var form = new RegistrationDetailForm(reg);

                if (form.ShowDialog() == DialogResult.OK && form.SavedDto != null)
                {
                    int index = _regs.FindIndex(r => r.Id == reg.Id);
                    if (index >= 0)
                    {
                        _regs[index] = form.SavedDto;
                    }
                    RefreshGrid();
                    SelectRowById(reg.Id);
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
                    try
                    {
                        var deleteResult = await RegistrationService.DeleteAsync(reg.Id);
                        if (!deleteResult.IsSuccess)
                        {
                            MessageBox.Show($"Error deleting registration: {deleteResult.Message}");
                            return;
                        }
                        _regs.RemoveAll(r => r.Id == reg.Id);
                        RefreshGrid();
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting registration due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to delete.");
            }
        }
    }
}
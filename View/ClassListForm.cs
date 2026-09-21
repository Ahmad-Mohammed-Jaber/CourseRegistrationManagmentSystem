using BL.Services;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace View
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
            _ = LoadClassesAsync();
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

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            btnBack.Location = new Point(10, 10);
            btnBack.Size = new Size(80, 30);
            btnBack.Text = "Back";
            btnBack.Click += (s, e) => Close();

            btnAdd.Location = new Point(100, 10);
            btnAdd.Size = new Size(100, 30);
            btnAdd.Text = "Add Class";
            btnAdd.Click += btnAdd_Click;

            btnEdit.Location = new Point(210, 10);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Text = "Edit Class";
            btnEdit.Click += btnEdit_Click;

            btnDelete.Location = new Point(320, 10);
            btnDelete.Size = new Size(110, 30);
            btnDelete.Text = "Delete Class";
            btnDelete.Click += btnDelete_Click;

            txtSearch.Location = new Point(450, 12);
            txtSearch.Size = new Size(150, 25);

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

            dgvClasses.Dock = DockStyle.Fill;
            dgvClasses.ReadOnly = true;
            dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClasses.MultiSelect = false;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ClientSize = new Size(1500, 800);
            Controls.Add(dgvClasses);
            Controls.Add(topPanel);
            Name = "ClassListForm";
            Text = "Manage Classes";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                dgvClasses.DataSource = null;
                var result = await _classService.GetAllAsync();
                if (!result.IsSuccess)
                {
                    MessageBox.Show($"Error loading classes: {result.Message}");
                    return;
                }
                dgvClasses.DataSource = result.Value!.Select(c => c.ToDto()).ToList();
                ConfigureColumns();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error loading classes due to an unexpected error.");
            }
        }

        private void ConfigureColumns()
        {
            if (dgvClasses.Columns["Schedule"] != null)
            {
                dgvClasses.Columns["Schedule"].Visible = false;
            }

            if (dgvClasses.Columns["ScheduleString"] != null)
            {
                dgvClasses.Columns["ScheduleString"].HeaderText = "Schedule";
            }

            if (dgvClasses.Columns["CurrentCapacity"] != null && dgvClasses.Columns["MaxCapacity"] != null)
            {
                int currentIdx = dgvClasses.Columns["CurrentCapacity"].DisplayIndex;
                int maxIdx = dgvClasses.Columns["MaxCapacity"].DisplayIndex;
                dgvClasses.Columns["CurrentCapacity"].DisplayIndex = maxIdx;
                dgvClasses.Columns["MaxCapacity"].DisplayIndex = currentIdx;
            }

            if (dgvClasses.Columns["CreatedOn"] != null)
            {
                dgvClasses.Columns["CreatedOn"].Visible = true;
                dgvClasses.Columns["CreatedOn"].HeaderText = "Created On";
            }

            if (dgvClasses.Columns["ModifiedOn"] != null)
            {
                dgvClasses.Columns["ModifiedOn"].Visible = true;
                dgvClasses.Columns["ModifiedOn"].HeaderText = "Modified On";
            }

            if (dgvClasses.Columns["CreatedBy"] != null)
            {
                dgvClasses.Columns["CreatedBy"].Visible = true;
                dgvClasses.Columns["CreatedBy"].HeaderText = "Created By";
            }

            if (dgvClasses.Columns["ModifiedBy"] != null)
            {
                dgvClasses.Columns["ModifiedBy"].Visible = true;
                dgvClasses.Columns["ModifiedBy"].HeaderText = "Modified By";
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    dgvClasses.DataSource = null;
                    var result = await _classService.SearchAsync(txtSearch.Text);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show($"Error searching classes: {result.Message}");
                        return;
                    }
                    dgvClasses.DataSource = result.Value!.Select(c => c.ToDto()).ToList();
                    ConfigureColumns();
                }
                else
                {
                    await LoadClassesAsync();
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.LogViewError(ex);
                MessageBox.Show("Error searching classes due to an unexpected error.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new ClassDetailForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = LoadClassesAsync();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow?.DataBoundItem is ClassDto cls)
            {
                using var form = new ClassDetailForm(cls);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    _ = LoadClassesAsync();
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
                    try
                    {
                        var deleteResult = await _classService.DeleteAsync(cls.Id);
                        if (!deleteResult.IsSuccess)
                        {
                            MessageBox.Show($"Error deleting class: {deleteResult.Message}");
                            return;
                        }
                        await LoadClassesAsync();
                    }
                    catch (BusinessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogViewError(ex);
                        MessageBox.Show("Error deleting class due to an unexpected error.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a class to delete.");
            }
        }
    }
}
using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.View.Mappings;
using Mozaic.CourseRegistrationManagementSystem.View.Models;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaic.CourseRegistrationManagementSystem.View
{
    public partial class MyRegistrationsForm : Form
    {
        private DataGridView dgvRegs;
        private Button btnDrop;
        private Button btnFilter;
        private Button btnClearFilters;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPage;
        private TextBox txtCourseFilter;
        private TextBox txtClassNameFilter;
        private ComboBox cmbStatusFilter;
        private Panel topPanel;
        private List<Registration> _data = new();
        private const int PageSize = 20;
        private int _page = 1;
        private int _totalPages = 1;
        private string? _course;
        private string? _className;
        private string? _status = "All";

        public MyRegistrationsForm()
        {
            InitializeComponent();
            _ = LoadMyRegistrationsAsync();
        }

        private void InitializeComponent()
        {
            dgvRegs = new DataGridView();
            btnDrop = new Button();
            btnFilter = new Button();
            btnClearFilters = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            lblPage = new Label();
            txtCourseFilter = new TextBox();
            txtClassNameFilter = new TextBox();
            cmbStatusFilter = new ComboBox();
            topPanel = new Panel();

            SuspendLayout();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 105;

            btnDrop.Location = new Point(10, 10);
            btnDrop.Size = new Size(110, 30);
            btnDrop.Text = "Drop Class";
            btnDrop.Click += btnDrop_Click;

            btnPrev.Location = new Point(140, 10);
            btnPrev.Size = new Size(70, 30);
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;

            lblPage.Location = new Point(215, 17);
            lblPage.Size = new Size(150, 20);
            lblPage.TextAlign = ContentAlignment.MiddleCenter;
            lblPage.Text = "Page 1 of 1";

            btnNext.Location = new Point(370, 10);
            btnNext.Size = new Size(70, 30);
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;

            var lblCourse = new Label { Text = "Course", Location = new Point(10, 48), Size = new Size(180, 18) };
            txtCourseFilter.Location = new Point(10, 68);
            txtCourseFilter.Size = new Size(180, 25);

            var lblClassName = new Label { Text = "Class Name", Location = new Point(200, 48), Size = new Size(160, 18) };
            txtClassNameFilter.Location = new Point(200, 68);
            txtClassNameFilter.Size = new Size(160, 25);

            var lblStatus = new Label { Text = "Status", Location = new Point(370, 48), Size = new Size(120, 18) };
            cmbStatusFilter.Location = new Point(370, 68);
            cmbStatusFilter.Size = new Size(120, 25);
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Registered", "Pending", "Dropped" });
            cmbStatusFilter.SelectedIndex = 0;

            btnFilter.Location = new Point(500, 66);
            btnFilter.Size = new Size(70, 27);
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;

            btnClearFilters.Location = new Point(580, 66);
            btnClearFilters.Size = new Size(70, 27);
            btnClearFilters.Text = "Clear";
            btnClearFilters.Click += btnClearFilters_Click;

            topPanel.Controls.Add(btnDrop);
            topPanel.Controls.Add(btnPrev);
            topPanel.Controls.Add(lblPage);
            topPanel.Controls.Add(btnNext);
            topPanel.Controls.Add(lblCourse);
            topPanel.Controls.Add(txtCourseFilter);
            topPanel.Controls.Add(lblClassName);
            topPanel.Controls.Add(txtClassNameFilter);
            topPanel.Controls.Add(lblStatus);
            topPanel.Controls.Add(cmbStatusFilter);
            topPanel.Controls.Add(btnFilter);
            topPanel.Controls.Add(btnClearFilters);

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
                var result = await StudentRegistrationService.GetRegistrationsAsync(
                    courseFilter: _course,
                    classFilter: _className,
                    statusFilter: _status,
                    pageNumber: _page,
                    pageSize: PageSize);

                _data = result.ToList();
                _page = result.PageNumber;
                _totalPages = Math.Max(1, result.TotalPages);

                RefreshGrid();
                UpdatePager();
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

        private void UpdatePager()
        {
            lblPage.Text = $"Page {_page} of {_totalPages}";
            btnPrev.Enabled = _page > 1;
            btnNext.Enabled = _page < _totalPages;
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            _course = Clean(txtCourseFilter.Text);
            _className = Clean(txtClassNameFilter.Text);
            _status = cmbStatusFilter.SelectedItem?.ToString() ?? "All";
            _page = 1;
            await LoadMyRegistrationsAsync();
        }

        private async void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtCourseFilter.Clear();
            txtClassNameFilter.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            _course = null;
            _className = null;
            _status = "All";
            _page = 1;
            await LoadMyRegistrationsAsync();
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                await LoadMyRegistrationsAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_page < _totalPages)
            {
                _page++;
                await LoadMyRegistrationsAsync();
            }
        }

        private void RefreshGrid()
        {
            dgvRegs.DataSource = null;
            dgvRegs.DataSource = ViewMapper.Instance.Map<List<MyRegistrationGridRow>>(_data);
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
                    await LoadMyRegistrationsAsync();
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
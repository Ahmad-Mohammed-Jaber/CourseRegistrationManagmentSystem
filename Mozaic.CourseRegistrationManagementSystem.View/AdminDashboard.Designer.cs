namespace CourseRegistrationManagmentSystem.View
{
    partial class AdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblWelcome = new Label();
            btnUserManagement = new Button();
            btnStudentManagement = new Button();
            btnCourseManagement = new Button();
            btnClassManagement = new Button();
            btnRegistrationManagement = new Button();
            btnLogout = new Button();
            SuspendLayout();
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(0, 37);
            lblWelcome.TabIndex = 6;
            btnUserManagement.Location = new Point(50, 70);
            btnUserManagement.Name = "btnUserManagement";
            btnUserManagement.Size = new Size(200, 40);
            btnUserManagement.TabIndex = 5;
            btnUserManagement.Text = "User Management";
            btnUserManagement.Click += btnUserManagement_Click;
            btnStudentManagement.Location = new Point(50, 120);
            btnStudentManagement.Name = "btnStudentManagement";
            btnStudentManagement.Size = new Size(200, 40);
            btnStudentManagement.TabIndex = 4;
            btnStudentManagement.Text = "Student Management";
            btnStudentManagement.Click += btnStudentManagement_Click;
            btnCourseManagement.Location = new Point(50, 170);
            btnCourseManagement.Name = "btnCourseManagement";
            btnCourseManagement.Size = new Size(200, 40);
            btnCourseManagement.TabIndex = 3;
            btnCourseManagement.Text = "Course Management";
            btnCourseManagement.Click += btnCourseManagement_Click;
            btnClassManagement.Location = new Point(50, 220);
            btnClassManagement.Name = "btnClassManagement";
            btnClassManagement.Size = new Size(200, 40);
            btnClassManagement.TabIndex = 2;
            btnClassManagement.Text = "Class Management";
            btnClassManagement.Click += btnClassManagement_Click;
            btnRegistrationManagement.Location = new Point(50, 270);
            btnRegistrationManagement.Name = "btnRegistrationManagement";
            btnRegistrationManagement.Size = new Size(200, 40);
            btnRegistrationManagement.TabIndex = 1;
            btnRegistrationManagement.Text = "Registration Management";
            btnRegistrationManagement.Click += btnRegistrationManagement_Click;
            btnLogout.Location = new Point(300, 20);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(80, 30);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;
            ClientSize = new Size(400, 350);
            Controls.Add(btnLogout);
            Controls.Add(btnRegistrationManagement);
            Controls.Add(btnClassManagement);
            Controls.Add(btnCourseManagement);
            Controls.Add(btnStudentManagement);
            Controls.Add(btnUserManagement);
            Controls.Add(lblWelcome);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AdminDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Admin Dashboard";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnUserManagement;
        private System.Windows.Forms.Button btnStudentManagement;
        private System.Windows.Forms.Button btnCourseManagement;
        private System.Windows.Forms.Button btnClassManagement;
        private System.Windows.Forms.Button btnRegistrationManagement;
        private System.Windows.Forms.Button btnLogout;
    }
}

namespace CourseRegistrationManagmentSystem.View
{
    partial class StudentDashboard
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
            btnMyRegistrations = new Button();
            btnAvailableCourses = new Button();
            btnLogout = new Button();
            SuspendLayout();
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(0, 37);
            lblWelcome.TabIndex = 3;
            btnMyRegistrations.Location = new Point(50, 70);
            btnMyRegistrations.Name = "btnMyRegistrations";
            btnMyRegistrations.Size = new Size(200, 40);
            btnMyRegistrations.TabIndex = 2;
            btnMyRegistrations.Text = "My Registrations";
            btnMyRegistrations.Click += btnMyRegistrations_Click;
            btnAvailableCourses.Location = new Point(50, 120);
            btnAvailableCourses.Name = "btnAvailableCourses";
            btnAvailableCourses.Size = new Size(200, 40);
            btnAvailableCourses.TabIndex = 1;
            btnAvailableCourses.Text = "Available Courses";
            btnAvailableCourses.Click += btnAvailableCourses_Click;
            btnLogout.Location = new Point(300, 20);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(80, 30);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;
            ClientSize = new Size(400, 200);
            Controls.Add(btnLogout);
            Controls.Add(btnAvailableCourses);
            Controls.Add(btnMyRegistrations);
            Controls.Add(lblWelcome);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "StudentDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Student Dashboard";
            Load += StudentDashboard_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnMyRegistrations;
        private System.Windows.Forms.Button btnAvailableCourses;
        private System.Windows.Forms.Button btnLogout;
    }
}

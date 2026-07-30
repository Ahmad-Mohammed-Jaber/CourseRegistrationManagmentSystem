using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseRegistrationManagmentSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var popupForm = new Form();
            popupForm.Text = "I Don't wanna do your dirty work no moreee!";
            popupForm.ShowDialog(this);

            popupForm.Text = "Imma fool to do dirty work go yeahhh";
            popupForm.ShowDialog(this);


        }
    }
}

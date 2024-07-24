using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpenseReport
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;
            guna2TextBox1.Left = (this.ClientSize.Width - guna2TextBox1.Width) / 2;
            guna2TextBox2.Left = (this.ClientSize.Width - guna2TextBox2.Width) / 2;
            guna2Button1.Left = (this.ClientSize.Width - guna2Button1.Width) / 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            var userId = guna2TextBox1.Text;
            var password = guna2TextBox2.Text;

            Debug.WriteLine("User ID: " + userId);
            Debug.WriteLine("Password: " + password);
        }
    }
}

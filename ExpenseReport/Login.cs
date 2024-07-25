using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ExpenseReport
{
    public partial class Login : Form
    {
        private const string userId = "admin";
        private const string password = "admin";

        public Login()
        {
            InitializeComponent();

            // Listen for enter key on password field
            guna2TextBox2.KeyDown += (sndr, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    guna2Button1.PerformClick();
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            var userId = guna2TextBox1.Text;
            var password = guna2TextBox2.Text;

            if (userId == Login.userId && password == Login.password)
            {
                // @TODO: Open the main form
            }
            else
            {
                MessageBox.Show("Invalid credentials");
            }
        }
    }
}

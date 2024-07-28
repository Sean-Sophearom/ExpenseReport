using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExpenseReport.UserControls;

namespace ExpenseReport
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AddUserControl(new UC_Report());
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            AddUserControl(new UC_Report());
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            AddUserControl(new UC_Employee());
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            AddUserControl(new UC_Expense());
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            AddUserControl(new UC_Income());
        }

        private void AddUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            guna2Panel3.Controls.Clear();
            guna2Panel3.Controls.Add(userControl);
            userControl.BringToFront();
        }
    }
}

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

namespace ExpenseReport.UserControls
{
    public partial class UC_Employee : UserControl
    {
        private readonly Database db = Database.Instance;
        public UC_Employee()
        {
            InitializeComponent();
        }

        private void UC_Employee_Load(object sender, EventArgs e)
        {
            CBboxDepartment.DataSource = db.GetDepartments();
            CBboxDepartment.DisplayMember = "Value";
            CBboxDepartment.ValueMember = "Key";
            CBboxDepartment.SelectedIndex = -1;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(CBboxDepartment.SelectedValue);
            Debug.WriteLine(CBboxDepartment.Text);
        }
    }
}

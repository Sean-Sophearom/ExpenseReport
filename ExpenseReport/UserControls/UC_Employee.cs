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
        private string editId;
        public UC_Employee()
        {
            InitializeComponent();
        }

        private void UC_Employee_Load(object sender, EventArgs e)
        {
            PopulateComboBox();
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            guna2DataGridView1.DataSource = db.GetEmployees();
        }

        private void PopulateComboBox()
        {
            CBboxDepartment.DataSource = db.GetCBBoxOptions("TBDepartment");
            CBboxDepartment.DisplayMember = "Value";
            CBboxDepartment.ValueMember = "Key";
            CBboxDepartment.SelectedIndex = -1;

            CBboxManager.DataSource = db.GetCBBoxOptions("TBEmployee");
            CBboxManager.DisplayMember = "Value";
            CBboxManager.ValueMember = "Key";
            CBboxManager.SelectedIndex = -1;
        }

        private void ResetInputFields()
        {
            TextboxName.Text = "";
            TextboxPhone.Text = "";
            TextboxEmail.Text = "";
            TextboxPosition.Text = "";
            PopulateComboBox();
        }

        private void InsertBtn_Click(object sender, EventArgs e)
        {
            var name = TextboxName.Text;
            var phone = TextboxPhone.Text;
            var email = TextboxEmail.Text;
            var position = TextboxPosition.Text;
            var department = CBboxDepartment.SelectedValue?.ToString();
            var manager = CBboxManager.SelectedValue?.ToString() ?? "-";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(position) || department == null)
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            var result = db.InsertEmployee(name, phone, email, position, department, manager);
            PopulateDataGrid();
            ResetInputFields();

            if (result) MessageBox.Show("Employee record inserted successfully");
            else MessageBox.Show("An error occurred while inserting the record! Please try again.");

        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            var cells = guna2DataGridView1.SelectedRows[0].Cells;
            editId = cells[0].Value.ToString();

            var departmentOptions = db.GetCBBoxOptions("TBDepartment");
            var managerOptions = db.GetCBBoxOptions("TBEmployee").Where(x => x.Key != int.Parse(editId)).ToList();

            TextboxName.Text = cells[1].Value.ToString();
            TextboxPhone.Text = cells[2].Value.ToString();
            TextboxEmail.Text = cells[3].Value.ToString();
            TextboxPosition.Text = cells[4].Value.ToString();
            CBboxManager.DataSource = managerOptions;
            CBboxDepartment.SelectedIndex = departmentOptions.FindIndex(x => x.Value == cells[5].Value.ToString());
            CBboxManager.SelectedIndex = managerOptions.FindIndex(x => x.Value == cells[6].Value.ToString());

            EditBtn.Enabled = false;
            DeleteBtn.Enabled = false;
            UpdateBtn.Visible = true;
            CancelBtn.Visible = true;
            InsertBtn.Visible = false;
            ActionHeader.Text = "Edit Employee Record";
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            var id = guna2DataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            var result = MessageBox.Show($"Are you sure you want to delete this employee record? (Id: {id})", "Delete Record", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                bool success = db.DeleteEmployee(id);
                PopulateDataGrid();
                if (success) MessageBox.Show("Employee record deleted successfully");
                else MessageBox.Show("An error occurred while deleting the record! Please try again.");
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {

            var name = TextboxName.Text;
            var phone = TextboxPhone.Text;
            var email = TextboxEmail.Text;
            var position = TextboxPosition.Text;
            var department = CBboxDepartment.SelectedValue.ToString();
            var manager = CBboxManager.SelectedValue?.ToString() ?? "-";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(position) || department == null)
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            var result = db.UpdateEmployee(editId, name, phone, email, position, department, manager);

            PopulateDataGrid();
            ResetInputFields();

            if (result) MessageBox.Show("Employee record updated successfully");
            else MessageBox.Show("An error occurred while updating the record! Please try again.");

            EditBtn.Enabled = true;
            DeleteBtn.Enabled = true;
            UpdateBtn.Visible = false;
            CancelBtn.Visible = false;
            InsertBtn.Visible = true;
            ActionHeader.Text = "Insert New Employee";
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            PopulateDataGrid();
            ResetInputFields();

            EditBtn.Enabled = true;
            DeleteBtn.Enabled = true;
            UpdateBtn.Visible = false;
            CancelBtn.Visible = false;
            InsertBtn.Visible = true;
            ActionHeader.Text = "Insert New Employee";
        }
    }
}

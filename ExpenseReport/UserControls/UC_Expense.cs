using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpenseReport.UserControls
{
    public partial class UC_Expense : UserControl
    {
        private readonly Database db = Database.Instance;
        private string editId;
        private List<KeyValuePair<int, string>> employeeOptions;
        private List<KeyValuePair<int, string>> currencyOptions;
        private List<string> typeOptions;
        public UC_Expense()
        {
            InitializeComponent();
        }

        private void UC_Expense_Load(object sender, EventArgs e)
        {
            typeOptions = db.GetTransactionTypes("Expense");
            employeeOptions = db.GetCBBoxOptions("TBEmployee");
            currencyOptions = db.GetCBBoxOptions("TBCurrency");

            PopulateComboBox();
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            guna2DataGridView1.DataSource = db.GetTransactions("TBExpense");
        }

        private void PopulateComboBox()
        {
            CBboxType.DataSource = typeOptions;
            CBboxType.SelectedIndex = -1;

            CBboxEmployee.DataSource = employeeOptions;
            CBboxEmployee.DisplayMember = "Value";
            CBboxEmployee.ValueMember = "Key";
            CBboxEmployee.SelectedIndex = -1;

            CBboxCurrency.DataSource = currencyOptions;
            CBboxCurrency.DisplayMember = "Value";
            CBboxCurrency.ValueMember = "Key";
            CBboxCurrency.SelectedIndex = -1;
        }

        private void ResetInputFields()
        {
            DatetimePicker.Value = DateTime.Now;
            TextboxAmount.Text = "";
            TextboxDescription.Text = "";

            for (var i = 0; i < 2; i++)
            {
                CBboxType.SelectedIndex = -1;
                CBboxEmployee.SelectedIndex = -1;
                CBboxCurrency.SelectedIndex = -1;
            }
        }

        private void InsertBtn_Click(object sender, EventArgs e)
        {
            var date = DatetimePicker.Value.ToString("yyyy-MM-dd");
            var type = CBboxType.SelectedValue?.ToString();
            var currency = CBboxCurrency.SelectedValue?.ToString();
            var amount = TextboxAmount.Text;
            var employee = CBboxEmployee.SelectedValue?.ToString();
            var description = string.IsNullOrEmpty(TextboxDescription.Text) ? "-" : TextboxDescription.Text;

            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(currency))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            var result = db.InsertTransaction("TBExpense", date, type, amount, currency, employee, description);
            PopulateDataGrid();
            ResetInputFields();

            if (result) MessageBox.Show("Expense record inserted successfully");
            else MessageBox.Show("An error occurred while inserting the record! Please try again.");

        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            var cells = guna2DataGridView1.SelectedRows[0].Cells;
            editId = cells[0].Value.ToString();

            DatetimePicker.Text = cells[1].Value.ToString();
            CBboxType.Text = cells[2].Value.ToString();
            CBboxCurrency.Text = cells[3].Value.ToString();
            TextboxAmount.Text = cells[4].Value.ToString();
            CBboxEmployee.Text = cells[5].Value.ToString();
            TextboxDescription.Text = cells[6].Value.ToString();

            EditBtn.Enabled = false;
            DeleteBtn.Enabled = false;
            UpdateBtn.Visible = true;
            CancelBtn.Visible = true;
            InsertBtn.Visible = false;
            ActionHeader.Text = "      Edit Expense Record";
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            var code = guna2DataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            var result = MessageBox.Show($"Are you sure you want to delete this expense record? (Code: {code})", "Delete Record", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                bool success = db.DeleteTransaction("TBExpense", code);
                PopulateDataGrid();
                if (success) MessageBox.Show("Expense record deleted successfully");
                else MessageBox.Show("An error occurred while deleting the record! Please try again.");
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            var date = DatetimePicker.Value.ToString("yyyy-MM-dd");
            var type = CBboxType.SelectedValue?.ToString();
            var currency = CBboxCurrency.SelectedValue?.ToString();
            var amount = TextboxAmount.Text;
            var employee = CBboxEmployee.SelectedValue?.ToString();
            var description = string.IsNullOrEmpty(TextboxDescription.Text) ? "-" : TextboxDescription.Text;

            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(currency))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            var result = db.UpdateTransaction("TBExpense", editId, date, type, amount, currency, employee, description);

            PopulateDataGrid();
            ResetInputFields();

            if (result) MessageBox.Show("Expense record updated successfully");
            else MessageBox.Show("An error occurred while updating the record! Please try again.");

            EditBtn.Enabled = true;
            DeleteBtn.Enabled = true;
            UpdateBtn.Visible = false;
            CancelBtn.Visible = false;
            InsertBtn.Visible = true;
            ActionHeader.Text = "Insert New Expense Record";
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
            ActionHeader.Text = "Insert New Expense Record";
        }

        private void TextboxAmount_TextChanged(object sender, EventArgs e)
        {
            var text = TextboxAmount.Text;
            var regex = new Regex(@"^\d+(\.\d*)?$");
            var valid = regex.IsMatch(text) || text.Length == 0;
            if (!valid)
            {
                TextboxAmount.Text = text.Length <= 1 ? "" : text.Substring(0, text.Length - 1);
                MessageBox.Show("Please enter a valid amount");
            }

        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            db.ExportToExcel(db.GetTransactions("TBExpense"), "Expenses");
        }
    }
}

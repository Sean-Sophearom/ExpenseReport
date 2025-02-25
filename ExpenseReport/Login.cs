﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
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

            if (Program.isDevMode || (userId == Login.userId && password == Login.password))
            {
                var mainForm = new MainForm();
                mainForm.Location = this.Location;
                mainForm.StartPosition = FormStartPosition.Manual;
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            else if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your credentials", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Invalid credentials", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

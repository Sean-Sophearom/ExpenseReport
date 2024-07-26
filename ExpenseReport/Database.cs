using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;

namespace ExpenseReport
{
    internal class Database
    {
        private const string dbName = "expenseReport.db";
        private const string dbPath = "Data Source=" + dbName;

        // Singleton pattern
        private static readonly Database instance = new Database();
        public static Database Instance { get => instance; }
        private Database()
        {
        }

        public List<KeyValuePair<int, string>> GetDepartments()
        {
            // Get all departments from the database
            var departments = new List<KeyValuePair<int, string>>();
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "SELECT Id, Name FROM TBDepartment";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                }
                conn.Close();
            }
            return departments;
        }

        public void CreateDatabaseAndTables()
        {
            if (!File.Exists(dbName))
            {
                SQLiteConnection.CreateFile(dbName);
                Debug.WriteLine("Database File created");
            }

            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = @"
                    CREATE TABLE IF NOT EXISTS TBDepartment (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        ManagerId INTEGER
                    );

                    CREATE TABLE IF NOT EXISTS TBEmployee (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Phone TEXT,
                        Email TEXT,
                        Position TEXT,
                        DepartmentId INTEGER,
                        ManagerId INTEGER,
                        FOREIGN KEY(DepartmentId) REFERENCES TBDepartment(Id) ON DELETE CASCADE,
                        FOREIGN KEY(ManagerId) REFERENCES TBEmployee(Id) ON DELETE SET NULL
                    );  

                    -- ALTER TABLE TBDepartment ADD FOREIGN KEY(ManagerId) REFERENCES TBEmployee(Id);

                    CREATE TABLE IF NOT EXISTS TBCurrency (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT, 
                        Code TEXT,
                        Symbol TEXT,
                        Value REAL
                    );

                    CREATE TABLE IF NOT EXISTS TBExpense (
                        Code INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Amount REAL,
                        Type TEXT,
                        Description TEXT,
                        CurrencyId INTEGER,
                        EmployeeId INTEGER,
                        FOREIGN KEY(CurrencyId) REFERENCES TBCurrency(Id) ON DELETE CASCADE,
                        FOREIGN KEY(EmployeeId) REFERENCES TBEmployee(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS TBIncome (
                        Code INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Amount REAL,
                        Type TEXT,
                        Description TEXT,
                        CurrencyId INTEGER,
                        EmployeeId INTEGER,
                        FOREIGN KEY(CurrencyId) REFERENCES TBCurrency(Id) ON DELETE CASCADE,
                        FOREIGN KEY(EmployeeId) REFERENCES TBEmployee(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS TBTransactionType (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Type TEXT
                    );
                ";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void PopulateDatabaseTables()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string selectQuery = @"
                    WITH 
                        departmentCount as (SELECT COUNT(*) AS COUNT FROM TBDepartment),
                        employeeCount as (SELECT COUNT(*) AS COUNT FROM TBEmployee),
                        currencyCount as (SELECT COUNT(*) AS COUNT FROM TBCurrency),
                        transactionTypeCount as (SELECT COUNT(*) AS COUNT FROM TBTransactionType),
                        expenseCount as (SELECT COUNT(*) AS COUNT FROM TBExpense),                
                        incomeCount as (SELECT COUNT(*) AS COUNT FROM TBIncome)
                    SELECT 
                        (SELECT count FROM departmentCount) +
                        (SELECT count FROM employeeCount) +
                        (SELECT count FROM currencyCount) +
                        (SELECT count FROM transactionTypeCount) +
                        (SELECT count FROM expenseCount) +
                        (SELECT count FROM incomeCount) AS totalCount;
                ";
                string insertQuery = @"
                    INSERT INTO TBDepartment (Name, ManagerId) VALUES 
                        ('Software Development', NULL),
                        ('Quality Assurance (QA)', NULL),
                        ('Human Resources (HR)', NULL),
                        ('IT Support', NULL),
                        ('Sales and Marketing', NULL),
                        ('Finance and Accounting', NULL),
                        ('Customer Support', NULL);

                    INSERT INTO TBCurrency (Name, Code, Symbol, Value) VALUES 
                        ('US Dollar', 'USD', '$', 1.0),
                        ('Khmer Riel', 'KHR', '៛', 4100.0);

                    INSERT INTO TBTransactionType (Name, Type) VALUES 
                        ('Software Sales', 'Income'),
                        ('Consulting Services', 'Income'),
                        ('Subscription Fees', 'Income'),
                        ('Maintenance and Support Contracts', 'Income'),
                        ('Custom Software Development', 'Income'),
                        ('Training Services', 'Income'),
                        ('Reselling Hardware and Software', 'Income'),
                        ('Advertising Revenue', 'Income'),
                        ('Affiliate Marketing', 'Income'),
                        ('Managed IT Services', 'Income'),
                        ('Salaries and Wages', 'Expense'),
                        ('Rent and Utilities', 'Expense'),
                        ('Software Licenses and Subscriptions', 'Expense'),
                        ('Hardware and Equipment Purchases', 'Expense'),
                        ('Internet and Telecommunications', 'Expense'),
                        ('Training and Professional Development', 'Expense'),
                        ('Marketing and Advertising', 'Expense'),
                        ('Office Supplies', 'Expense'),
                        ('Travel and Accommodation', 'Expense'),
                        ('Insurance', 'Expense');
                    
                ";
                var cmd = new SQLiteCommand(selectQuery, conn);
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    cmd = new SQLiteCommand(insertQuery, conn);
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("Database populated");
                }
                conn.Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace ExpenseReport
{
    internal class Database
    {
        private const string dbName = "expenseReport.db";
        private const string dbPath = "Data Source=" + dbName;
        private const bool dropDb = true;

        // Singleton pattern
        private static readonly Database instance = new Database();
        public static Database Instance { get => instance; }
        private Database()
        {
        }

        //Get Employees
        public DataTable GetEmployees()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "SELECT * FROM ViewEmployeeList ORDER BY Id DESC";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                adapter.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public bool InsertEmployee(string name, string phone, string email, string position, string departmentId, string managerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "INSERT INTO TBEmployee (Name, Phone, Email, Position, DepartmentId, ManagerId) VALUES (@Name, @Phone, @Email, @Position, @DepartmentId, @ManagerId)";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                cmd.Parameters.AddWithValue("@ManagerId", managerId);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public bool UpdateEmployee(string id, string name, string phone, string email, string position, string departmentId, string managerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "UPDATE TBEmployee SET Name = @Name, Phone = @Phone, Email = @Email, Position = @Position, DepartmentId = @DepartmentId, ManagerId = @ManagerId WHERE Id = @Id";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                cmd.Parameters.AddWithValue("@ManagerId", managerId);
                cmd.Parameters.AddWithValue("@Id", id);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public bool DeleteEmployee(string id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "DELETE FROM TBEmployee WHERE Id = @Id";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public List<KeyValuePair<int, string>> GetCBBoxOptions(string table)
        {
            var options = new List<KeyValuePair<int, string>>();
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = $"SELECT Id, Name FROM {table} ORDER BY NAME ASC";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    options.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                }
                conn.Close();
            }
            return options;
        }

        public List<KeyValuePair<int, string>> GetTransactionTypes(string type)
        {
            var options = GetCBBoxOptions("TBTransactionType");
            options = options.FindAll(x => x.Value.Contains(type));
            return options;
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

                // Drop database tables
                if (dropDb)
                {
                    var dropQuery = @"
                        DROP TABLE IF EXISTS TBDepartment;
                        DROP TABLE IF EXISTS TBEmployee;
                        DROP TABLE IF EXISTS TBCurrency;
                        DROP TABLE IF EXISTS TBExpense;
                        DROP TABLE IF EXISTS TBIncome;
                        DROP TABLE IF EXISTS TBTransactionType;
                    ";
                    SQLiteCommand dropCmd = new SQLiteCommand(dropQuery, conn);
                    dropCmd.ExecuteNonQuery();
                }

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

                    CREATE VIEW IF NOT EXISTS ViewEmployeeList AS
                        SELECT EMP.Id Id,
                               EMP.Name Name,
                               EMP.Phone Phone,
                               EMP.Email Email,
                               EMP.Position Position,
                               DEP.Name Department,
                               COALESCE(MGR.Name, '-') Manager
                          FROM TBEmployee EMP
                               LEFT JOIN
                               TBDepartment DEP ON EMP.DepartmentId = DEP.Id
                               LEFT JOIN
                               TBEmployee MGR ON EMP.ManagerId = MGR.Id;

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
                        ('Software Development', 1),
                        ('Quality Assurance (QA)', 7),
                        ('Human Resources (HR)', 6),
                        ('IT Support', 5),
                        ('Sales and Marketing', 4),
                        ('Finance and Accounting', 3),
                        ('Customer Support', 2);

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
                    
                    INSERT INTO TBEmployee (Name, Phone, Email, Position, DepartmentId, ManagerId) VALUES
                        ('Bob Smith', '555-5678', 'bob.smith@example.com', 'CEO', 1, NULL);

                    INSERT INTO TBEmployee (Name, Phone, Email, Position, DepartmentId, ManagerId) VALUES
                        ('Paul Red', '555-3457', 'paul.red@example.com', 'Customer Support Lead', 7, 1),
                        ('Leo King', '555-9012', 'leo.king@example.com', 'Finance Manager', 6, 1),
                        ('Jack White', '555-7890', 'jack.white@example.com', 'Sales Manager', 5, 1),
                        ('Hank Green', '555-5679', 'hank.green@example.com', 'IT Support Manager', 4, 1),
                        ('Frank Miller', '555-3456', 'frank.miller@example.com', 'HR Manager', 3, 1),
                        ('Diana Prince', '555-4321', 'diana.prince@example.com', 'QA Lead', 2, 1);

                    INSERT INTO TBEmployee (Name, Phone, Email, Position, DepartmentId, ManagerId) VALUES
                        ('Oscar Blue', '555-2346', 'oscar.blue@example.com', 'Customer Support Agent', 7, 2),
                        ('Nina Chen', '555-1235', 'nina.chen@example.com', 'Senior Accountant', 6, 3),
                        ('Mia Wong', '555-0123', 'mia.wong@example.com', 'Accountant', 6, 3),
                        ('Karen Black', '555-8901', 'karen.black@example.com', 'Marketing Specialist', 5, 4),
                        ('Ivy Lee', '555-6789', 'ivy.lee@example.com', 'Sales Representative', 5, 4),
                        ('Grace Hopper', '555-4567', 'grace.hopper@example.com', 'IT Support Specialist', 4, 5),
                        ('Eve Adams', '555-2345', 'eve.adams@example.com', 'HR Specialist', 3, 6),
                        ('Charlie Brown', '555-8765', 'charlie.brown@example.com', 'QA Engineer', 2, 7),
                        ('Alice Johnson', '555-1234', 'alice.johnson@example.com', 'Developer', 1, 1);
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

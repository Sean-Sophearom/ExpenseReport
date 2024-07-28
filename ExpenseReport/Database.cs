using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;
using OfficeOpenXml;

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

        public void ExportToExcel(DataTable dt, string sheetName = "Sheet1", string fileName = "")

        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = sheetName;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.FileName = fileName;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
                        ws.Cells["A1"].LoadFromDataTable(dt, true);
                        pck.SaveAs(new FileInfo(filePath));
                        try
                        {
                            Process.Start(filePath);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Data exported successfully", "Export To Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

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

        public DataTable GetTransactions(string table)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = table == "TBIncome"
                    ? "SELECT * FROM ViewIncomeList ORDER BY Code DESC"
                    : "SELECT * FROM ViewExpenseList ORDER BY Code DESC";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                adapter.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public bool InsertTransaction(string table, string date, string type, string amount, string currencyId, string employeeId, string description)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = $"INSERT INTO {table} (Date, Type, Amount, CurrencyId, EmployeeId, Description) VALUES (@Date, @Type, @Amount, @CurrencyId, @EmployeeId, @Description)";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Description", description);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public bool UpdateTransaction(string table, string id, string date, string type, string amount, string currencyId, string employeeId, string description)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = $"UPDATE {table} SET Date = @Date, Type = @Type, Amount = @Amount, CurrencyId = @CurrencyId, EmployeeId = @EmployeeId, Description = @Description WHERE Code = @Id";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Id", id);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public bool DeleteTransaction(string table, string id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = $"DELETE FROM {table} WHERE Code = @Id";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return result == 1;
            }
        }

        public List<KeyValuePair<int, string>> GetCBBoxOptions(string table, string filter = "")
        {
            var options = new List<KeyValuePair<int, string>>();
            using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = $"SELECT Id, Name FROM {table} {filter} ORDER BY NAME ASC";
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

        public List<string> GetTransactionTypes(string type)
        {
            var options = GetCBBoxOptions("TBTransactionType", $"where Type = '{type}'");
            return options.ConvertAll(x => x.Value);
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
                if (dropDb)
                {
                    var dropQuery = @"
                        DROP TABLE IF EXISTS TBDepartment;
                        DROP TABLE IF EXISTS TBEmployee;
                        DROP TABLE IF EXISTS TBCurrency;
                        DROP TABLE IF EXISTS TBExpense;
                        DROP TABLE IF EXISTS TBIncome;
                        DROP TABLE IF EXISTS TBTransactionType;
                        DROP VIEW IF EXISTS ViewEmployeeList;   
                        DROP VIEW IF EXISTS ViewIncomeList;
                        DROP VIEW IF EXISTS ViewExpenseList;
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
                        FullName TEXT,
                        Name TEXT, 
                        Value REAL
                    );

                    CREATE TABLE IF NOT EXISTS TBExpense (
                        Code INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Type TEXT,
                        CurrencyId INTEGER,
                        Amount REAL,
                        EmployeeId INTEGER,
                        Description TEXT,
                        FOREIGN KEY(CurrencyId) REFERENCES TBCurrency(Id) ON DELETE CASCADE,
                        FOREIGN KEY(EmployeeId) REFERENCES TBEmployee(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS TBIncome (
                        Code INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Type TEXT,
                        CurrencyId INTEGER,
                        Amount REAL,
                        EmployeeId INTEGER,
                        Description TEXT,
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
                
                    CREATE VIEW IF NOT EXISTS ViewIncomeList AS  
                        SELECT 
                            INC.Code Code,
                            INC.Date Date,
                            INC.Type Type,
                            CUR.Name Currency,
                            INC.Amount Amount,
                            COALESCE(EMP.Name,'-') Employee,
                            INC.Description Description
                        FROM TBIncome INC
                        LEFT JOIN TBCurrency CUR ON INC.CurrencyId = CUR.Id
                        LEFT JOIN TBEmployee EMP ON INC.EmployeeId = EMP.Id;

                    CREATE VIEW IF NOT EXISTS ViewExpenseList AS
                        SELECT 
                            EXP.Code Code,
                            EXP.Date Date,
                            EXP.Type Type,
                            CUR.Name Currency,
                            EXP.Amount Amount,
                            COALESCE(EMP.Name,'-') Employee,
                            EXP.Description Description
                        FROM TBExpense EXP
                        LEFT JOIN TBCurrency CUR ON EXP.CurrencyId = CUR.Id
                        LEFT JOIN TBEmployee EMP ON EXP.EmployeeId = EMP.Id;
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

                    INSERT INTO TBCurrency (FullName, Name, Value) VALUES 
                        ('US Dollar', 'USD$', 1.0),
                        ('Khmer Riel', 'KHR៛', 4100.0);

                    INSERT INTO TBTransactionType (Name, Type) VALUES 
                        ('Software Sales', 'Income'),
                        ('Consulting Services', 'Income'),
                        ('Subscription Fees', 'Income'),
                        ('Maintenance and Support', 'Income'),
                        ('Custom Software', 'Income'),
                        ('Training Services', 'Income'),
                        ('Reselling Hardware', 'Income'),
                        ('Advertising Revenue', 'Income'),
                        ('Affiliate Marketing', 'Income'),
                        ('Managed IT Services', 'Income'),
                        ('Other', 'Income'),
                        ('Other', 'Expense'),
                        ('Salaries and Wages', 'Expense'),
                        ('Rent and Utilities', 'Expense'),
                        ('Software Licenses', 'Expense'),
                        ('Hardware and Equipment', 'Expense'),
                        ('Internet Fees', 'Expense'),
                        ('Training and Development', 'Expense'),
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

                    INSERT INTO TBIncome (Date, Type, Amount, CurrencyId, EmployeeId, Description) VALUES
                        ('2024-01-15', 'Software Sales', 1500.00, 1, 1, 'Monthly sales revenue'),
                        ('2024-02-20', 'Consulting Services', 2200.00, 1, 2, 'Consulting fees for project X'),
                        ('2024-03-25', 'Subscription Fees', 2800.00, 1, 3, 'Annual subscription fee'),
                        ('2024-04-10', 'Maintenance and Support', 1300.00, 1, 4, 'Support contract renewal'),
                        ('2024-05-18', 'Custom Software', 3000.00, 1, 5, 'Custom software development for client Y'),
                        ('2024-06-05', 'Training Services', 1800.00, 1, 6, 'Employee training services'),
                        ('2024-07-12', 'Reselling Hardware', 2500.00, 1, 7, 'Revenue from reselling hardware'),
                        ('2024-01-22', 'Advertising Revenue', 2000.00, 1, 8, 'Online advertising revenue'),
                        ('2024-02-14', 'Affiliate Marketing', 2700.00, 1, 9, 'Revenue from affiliate marketing'),
                        ('2024-03-29', 'Managed IT Services', 1900.00, 1, 10, 'Managed IT services for client Z'),
                        ('2024-04-16', 'Software Sales', 5000000.00, 2, 11, 'Large software sales contract'),
                        ('2024-05-23', 'Consulting Services', 6000000.00, 2, 12, 'Consulting services for major client'),
                        ('2024-06-30', 'Subscription Fees', 7000000.00, 2, 13, 'Corporate subscription fee'),
                        ('2024-07-04', 'Maintenance and Support', 8000000.00, 2, 14, 'Multi-year support contract'),
                        ('2024-01-09', 'Custom Software', 9000000.00, 2, 15, 'Custom software for government project'),
                        ('2024-02-26', 'Training Services', 10000000.00, 2, 16, 'Large scale training services');

                    INSERT INTO TBExpense (Date, Type, Amount, CurrencyId, EmployeeId, Description) VALUES
                        ('2024-01-03', 'Salaries and Wages', 2500.00, 1, 1, 'Monthly salary payment'),
                        ('2024-02-11', 'Rent and Utilities', 1800.00, 1, 2, 'Office rent and utilities'),
                        ('2024-03-08', 'Software Licenses', 2300.00, 1, 3, 'Software subscription fee'),
                        ('2024-04-05', 'Hardware and Equipment', 2900.00, 1, 4, 'New hardware purchase'),
                        ('2024-05-17', 'Internet Fees', 1200.00, 1, 5, 'Monthly internet and phone bill'),
                        ('2024-06-13', 'Training and Development', 1700.00, 1, 6, 'Employee training program'),
                        ('2024-07-21', 'Marketing and Advertising', 2600.00, 1, 7, 'Marketing campaign expenses'),
                        ('2024-01-07', 'Office Supplies', 900.00, 1, 8, 'Office supplies purchase'),
                        ('2024-02-19', 'Travel and Accommodation', 3000.00, 1, 9, 'Business travel expenses'),
                        ('2024-03-24', 'Insurance', 1500.00, 1, 10, 'Company insurance premium'),
                        ('2024-04-01', 'Salaries and Wages', 8000000.00, 2, 11, 'Quarterly salary payment'),
                        ('2024-05-11', 'Rent and Utilities', 9000000.00, 2, 12, 'Annual office rent and utilities'),
                        ('2024-06-18', 'Software Licenses', 10000000.00, 2, 13, 'Corporate software license fee'),
                        ('2024-07-14', 'Hardware and Equipment', 11000000.00, 2, 14, 'Bulk hardware purchase'),
                        ('2024-01-26', 'Internet Fees', 12000000.00, 2, 15, 'Yearly internet and phone bill'),
                        ('2024-02-08', 'Training and Development', 7000000.00, 2, 16, 'Professional development program');
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

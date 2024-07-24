using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

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

        public void CreateDatabase()
        {
            if (!File.Exists(dbName))
            {
                SQLiteConnection.CreateFile(dbName);
            }

            /*using (SQLiteConnection conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string query = "SELECT 1 + 1 as TESTING_INTEGERRR";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                var result = cmd.ExecuteScalar();
                Debug.WriteLine(result);
                conn.Close();
            }*/
        }
    }
}

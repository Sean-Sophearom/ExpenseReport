using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpenseReport
{
    internal static class Program
    {
        public const bool isDevMode = false;
        public const bool dropDb = false;
        private static readonly Database db = Database.Instance;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            db.CreateDatabaseAndTables();
            db.PopulateDatabaseTables();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}

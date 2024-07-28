using Guna.Charts.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpenseReport.UserControls
{
    public partial class UC_Report : UserControl
    {
        private readonly Database db = Database.Instance;
        public UC_Report()
        {
            InitializeComponent();
        }

        private async void UC_Report_Load(object sender, EventArgs e)
        {
            PopulateLineChart();

            await Task.Delay(1000);

            PopulatePieChartInc();

            await Task.Delay(1000);

            PopulatePieChartExp();
        }



        private void PopulateLineChart()
        {
            lineChart.Visible = true;
            lineChart.YAxes.GridLines.Display = false;

            var incomeByMonth = db.GetReport("Income", "Month");
            var expenseByMonth = db.GetReport("Expense", "Month");

            var dsIncByMonth = new GunaLineDataset
            {
                Label = "Income",
                PointRadius = 7,
                PointStyle = PointStyle.Circle,
                PointFillColors = new ColorCollection(new Color[] { Color.LightBlue }),
                PointBorderColors = new ColorCollection(new Color[] { Color.LightBlue }),
            };
            incomeByMonth.ForEach(x => dsIncByMonth.DataPoints.Add(x.Key, x.Value));

            var dsExpByMonth = new GunaLineDataset
            {
                Label = "Expense",
                PointRadius = 7,
                PointStyle = PointStyle.Circle,
                PointFillColors = new ColorCollection(new Color[] { Color.LightCoral }),
                PointBorderColors = new ColorCollection(new Color[] { Color.LightCoral }),
                FillColor = Color.LightCoral,
                BorderColor = Color.LightCoral
            };
            expenseByMonth.ForEach(x => dsExpByMonth.DataPoints.Add(x.Key, x.Value));

            lineChart.Datasets.Add(dsIncByMonth);
            lineChart.Datasets.Add(dsExpByMonth);
            lineChart.Update();
        }

        private void PopulatePieChartInc()
        {
            pieChartInc.Visible = true;
            pieChartInc.Legend.Position = LegendPosition.Right;
            pieChartInc.XAxes.Display = false;
            pieChartInc.YAxes.Display = false;

            var incomeByType = db.GetReport("Income", "Type");
            var shadesOfBlue = new Color[] { Color.LightSkyBlue, Color.CornflowerBlue, Color.RoyalBlue, Color.DarkBlue };
            var dsIncByType = new GunaPieDataset
            {
                FillColors = new ColorCollection(shadesOfBlue),
            };
            incomeByType.GetRange(0, 4).ForEach(x => dsIncByType.DataPoints.Add(x.Key, x.Value));

            pieChartInc.Datasets.Add(dsIncByType);
            pieChartInc.Update();
        }

        private void PopulatePieChartExp()
        {
            pieChartExp.Visible = true;
            pieChartExp.Legend.Position = LegendPosition.Right;
            pieChartExp.XAxes.Display = false;
            pieChartExp.YAxes.Display = false;

            var expenseByType = db.GetReport("Expense", "Type");
            var shadesOfRed = new Color[] { Color.LightCoral, Color.IndianRed, Color.Firebrick, Color.DarkRed };
            var dsExpByType = new GunaPieDataset
            {
                FillColors = new ColorCollection(shadesOfRed),
            };
            expenseByType.GetRange(0, 4).ForEach(x => dsExpByType.DataPoints.Add(x.Key, x.Value));

            pieChartExp.Datasets.Add(dsExpByType);
            pieChartExp.Update();
        }
    }
}

using System;
using System.Data;
using quanlycitieu.DAL;

namespace quanlycitieu.BLL
{
    public class StatisticBLL
    {
        private StatisticDAL statisticDAL = new StatisticDAL();

        /// <summary>
        /// Thống kê chi tiết từng ngày trong tháng (từ ngày 1 đến ngày cuối tháng)
        /// </summary>
        public DataTable GetDailyStatistics(int userId, int month, int year)
        {
            DataTable dtRaw = statisticDAL.GetDailyTransactionsInMonth(userId, month, year);

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Day", typeof(int));
            dtResult.Columns.Add("DayLabel", typeof(string));
            dtResult.Columns.Add("Income", typeof(decimal));
            dtResult.Columns.Add("Expense", typeof(decimal));

            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                decimal inc = 0;
                decimal exp = 0;

                foreach (DataRow row in dtRaw.Rows)
                {
                    if (Convert.ToInt32(row["Day"]) == day)
                    {
                        string type = row["Type"]?.ToString() ?? "";
                        decimal amt = Convert.ToDecimal(row["Amount"]);
                        if (type == "Thu Nhập") inc += amt;
                        else if (type == "Chi Tiêu") exp += amt;
                    }
                }

                dtResult.Rows.Add(day, $"{day:D2}/{month:D2}", inc, exp);
            }

            return dtResult;
        }

        /// <summary>
        /// Thống kê 12 tháng trong năm (từ Tháng 1 đến Tháng 12)
        /// </summary>
        public DataTable GetYearlyStatistics(int userId, int year)
        {
            DataTable dtRaw = statisticDAL.GetMonthlyTransactionsInYear(userId, year);

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Month", typeof(int));
            dtResult.Columns.Add("MonthLabel", typeof(string));
            dtResult.Columns.Add("Income", typeof(decimal));
            dtResult.Columns.Add("Expense", typeof(decimal));

            for (int m = 1; m <= 12; m++)
            {
                decimal inc = 0;
                decimal exp = 0;

                foreach (DataRow row in dtRaw.Rows)
                {
                    if (Convert.ToInt32(row["Month"]) == m)
                    {
                        string type = row["Type"]?.ToString() ?? "";
                        decimal amt = Convert.ToDecimal(row["Amount"]);
                        if (type == "Thu Nhập") inc += amt;
                        else if (type == "Chi Tiêu") exp += amt;
                    }
                }

                dtResult.Rows.Add(m, $"Tháng {m}", inc, exp);
            }

            return dtResult;
        }

        /// <summary>
        /// Tỷ lệ chi tiêu theo danh mục kèm phần trăm
        /// </summary>
        public DataTable GetCategoryExpenseShare(int userId, int month, int year)
        {
            DataTable dtRaw = statisticDAL.GetCategoryExpenseShareInMonth(userId, month, year);

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("CategoryName", typeof(string));
            dtResult.Columns.Add("Amount", typeof(decimal));
            dtResult.Columns.Add("Percentage", typeof(double));

            decimal totalExpense = 0;
            foreach (DataRow row in dtRaw.Rows)
            {
                totalExpense += Convert.ToDecimal(row["Amount"]);
            }

            foreach (DataRow row in dtRaw.Rows)
            {
                string cat = row["CategoryName"]?.ToString() ?? "";
                decimal amt = Convert.ToDecimal(row["Amount"]);
                double pct = totalExpense > 0 ? (double)Math.Round((amt / totalExpense) * 100m, 1) : 0;
                dtResult.Rows.Add(cat, amt, pct);
            }

            return dtResult;
        }
    }
}

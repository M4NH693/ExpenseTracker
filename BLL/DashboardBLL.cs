using System;
using System.Data;
using quanlycitieu.DAL;

namespace quanlycitieu.BLL
{
    public class DashboardBLL
    {
        private DashboardDAL dashboardDAL = new DashboardDAL();

        public (decimal TotalIncome, decimal TotalExpense, decimal Balance) GetDashboardSummary(int userId, int month, int year)
        {
            var (income, expense) = dashboardDAL.GetMonthlyTotals(userId, month, year);
            decimal balance = income - expense;
            return (income, expense, balance);
        }

        public DataTable GetMonthlyTransactions(int userId, int month, int year)
        {
            return dashboardDAL.GetMonthlyTransactions(userId, month, year);
        }

        public DataTable GetExpensesByCategoryInMonth(int userId, int month, int year)
        {
            return dashboardDAL.GetExpensesByCategoryInMonth(userId, month, year);
        }
    }
}

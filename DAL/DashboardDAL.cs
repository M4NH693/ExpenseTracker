using System;
using System.Data;
using Npgsql;

namespace quanlycitieu.DAL
{
    public class DashboardDAL
    {
        public (decimal TotalIncome, decimal TotalExpense) GetMonthlyTotals(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN Type = 'Thu Nhập' THEN ABS(Amount) ELSE 0 END), 0) as TotalIncome,
                    COALESCE(SUM(CASE WHEN Type = 'Chi Tiêu' THEN ABS(Amount) ELSE 0 END), 0) as TotalExpense
                FROM Transactions
                WHERE UserId = @uid 
                  AND EXTRACT(MONTH FROM Date) = @m 
                  AND EXTRACT(YEAR FROM Date) = @y";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            DataTable dt = DbConnection.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                decimal inc = Convert.ToDecimal(dt.Rows[0]["TotalIncome"]);
                decimal exp = Convert.ToDecimal(dt.Rows[0]["TotalExpense"]);
                return (inc, exp);
            }
            return (0m, 0m);
        }

        public DataTable GetMonthlyTransactions(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    t.Id as ""Mã"",
                    t.Type as ""Loại"",
                    c.Name as ""Danh Mục"",
                    ABS(t.Amount) as ""Số Tiền"",
                    t.Date as ""Thời Gian"",
                    t.Description as ""Ghi Chú""
                FROM Transactions t
                JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @uid
                  AND EXTRACT(MONTH FROM t.Date) = @m
                  AND EXTRACT(YEAR FROM t.Date) = @y
                ORDER BY t.Date DESC, t.Id DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetExpensesByCategoryInMonth(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    c.Name as ""Name"",
                    t.Type as ""Type"",
                    COALESCE(SUM(ABS(t.Amount)), 0) as ""Total""
                FROM Transactions t
                JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @uid
                  AND EXTRACT(MONTH FROM t.Date) = @m
                  AND EXTRACT(YEAR FROM t.Date) = @y
                GROUP BY c.Name, t.Type
                HAVING SUM(ABS(t.Amount)) > 0
                ORDER BY t.Type DESC, SUM(ABS(t.Amount)) DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }
    }
}

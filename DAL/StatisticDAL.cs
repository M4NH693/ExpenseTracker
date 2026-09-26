using System;
using System.Data;
using Npgsql;

namespace quanlycitieu.DAL
{
    public class StatisticDAL
    {
        public DataTable GetDailyTransactionsInMonth(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    EXTRACT(DAY FROM t.Date)::int as Day,
                    t.Type,
                    COALESCE(SUM(ABS(t.Amount)), 0) as Amount
                FROM Transactions t
                WHERE t.UserId = @uid
                  AND EXTRACT(MONTH FROM t.Date) = @m
                  AND EXTRACT(YEAR FROM t.Date) = @y
                GROUP BY EXTRACT(DAY FROM t.Date), t.Type
                ORDER BY Day ASC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetMonthlyTransactionsInYear(int userId, int year)
        {
            string query = @"
                SELECT 
                    EXTRACT(MONTH FROM t.Date)::int as Month,
                    t.Type,
                    COALESCE(SUM(ABS(t.Amount)), 0) as Amount
                FROM Transactions t
                WHERE t.UserId = @uid
                  AND EXTRACT(YEAR FROM t.Date) = @y
                GROUP BY EXTRACT(MONTH FROM t.Date), t.Type
                ORDER BY Month ASC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetCategoryExpenseShareInMonth(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    c.Name as CategoryName,
                    COALESCE(SUM(ABS(t.Amount)), 0) as Amount
                FROM Transactions t
                JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @uid
                  AND t.Type = 'Chi Tiêu'
                  AND EXTRACT(MONTH FROM t.Date) = @m
                  AND EXTRACT(YEAR FROM t.Date) = @y
                GROUP BY c.Name
                HAVING SUM(ABS(t.Amount)) > 0
                ORDER BY SUM(ABS(t.Amount)) DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }
    }
}

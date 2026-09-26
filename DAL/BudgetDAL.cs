using System;
using System.Data;
using Npgsql;
using quanlycitieu.DTO;

namespace quanlycitieu.DAL
{
    public class BudgetDAL
    {
        public bool UpsertBudget(BudgetDTO budget)
        {
            string query = @"
                INSERT INTO Budgets (UserId, CategoryId, AmountLimit, Month, Year)
                VALUES (@uid, @catId, @amt, @m, @y)
                ON CONFLICT (UserId, CategoryId, Month, Year)
                DO UPDATE SET AmountLimit = EXCLUDED.AmountLimit";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", budget.UserId),
                new NpgsqlParameter("@catId", budget.CategoryId),
                new NpgsqlParameter("@amt", budget.AmountLimit),
                new NpgsqlParameter("@m", budget.Month),
                new NpgsqlParameter("@y", budget.Year)
            };

            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteBudget(int userId, int budgetId)
        {
            string query = "DELETE FROM Budgets WHERE Id = @id AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", budgetId),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public DataTable GetBudgetsWithSpent(int userId, int month, int year)
        {
            string query = @"
                SELECT 
                    b.Id,
                    b.CategoryId,
                    c.Name as ""DanhMuc"",
                    b.AmountLimit as ""HanMuc"",
                    COALESCE(spent.Total, 0) as ""DaChi"",
                    (b.AmountLimit - COALESCE(spent.Total, 0)) as ""ConLai"",
                    CASE 
                        WHEN b.AmountLimit > 0 THEN ROUND((COALESCE(spent.Total, 0) / b.AmountLimit) * 100, 1)
                        ELSE 0 
                    END as ""PhanTram"",
                    b.Month,
                    b.Year
                FROM Budgets b
                JOIN Categories c ON b.CategoryId = c.Id
                LEFT JOIN (
                    SELECT CategoryId, SUM(ABS(Amount)) as Total
                    FROM Transactions
                    WHERE UserId = @uid 
                      AND Type = 'Chi Tiêu'
                      AND EXTRACT(MONTH FROM Date) = @m
                      AND EXTRACT(YEAR FROM Date) = @y
                    GROUP BY CategoryId
                ) spent ON b.CategoryId = spent.CategoryId
                WHERE b.UserId = @uid AND b.Month = @m AND b.Year = @y
                ORDER BY b.Id DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            return DbConnection.ExecuteQuery(query, parameters);
        }

        public decimal? GetBudgetLimit(int userId, int categoryId, int month, int year)
        {
            string query = @"
                SELECT AmountLimit 
                FROM Budgets 
                WHERE UserId = @uid AND CategoryId = @catId AND Month = @m AND Year = @y";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@catId", categoryId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            var result = DbConnection.ExecuteScalar(query, parameters);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDecimal(result);
            }
            return null;
        }

        public decimal GetTotalSpentForCategoryInMonth(int userId, int categoryId, int month, int year)
        {
            string query = @"
                SELECT COALESCE(SUM(ABS(Amount)), 0)
                FROM Transactions
                WHERE UserId = @uid 
                  AND CategoryId = @catId 
                  AND Type = 'Chi Tiêu'
                  AND EXTRACT(MONTH FROM Date) = @m
                  AND EXTRACT(YEAR FROM Date) = @y";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", userId),
                new NpgsqlParameter("@catId", categoryId),
                new NpgsqlParameter("@m", month),
                new NpgsqlParameter("@y", year)
            };

            var result = DbConnection.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public string GetCategoryName(int categoryId)
        {
            string query = "SELECT Name FROM Categories WHERE Id = @id";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@id", categoryId) };
            var result = DbConnection.ExecuteScalar(query, parameters);
            return result?.ToString() ?? "Danh mục";
        }
    }
}

using System;
using System.Data;
using Npgsql;
using quanlycitieu.DTO;

namespace quanlycitieu.DAL
{
    public class TransactionDAL
    {
        public bool InsertTransaction(TransactionDTO transaction)
        {
            string query = @"INSERT INTO Transactions (UserId, Type, CategoryId, Amount, Date, Description) 
                             VALUES (@uid, @type, @catId, @amt, @date, @desc)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", transaction.UserId),
                new NpgsqlParameter("@type", transaction.Type),
                new NpgsqlParameter("@catId", transaction.CategoryId),
                new NpgsqlParameter("@amt", transaction.Amount),
                new NpgsqlParameter("@date", transaction.Date),
                new NpgsqlParameter("@desc", (object?)transaction.Description ?? DBNull.Value)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateTransaction(TransactionDTO transaction)
        {
            // Chỉ cho phép sửa giao dịch thuộc chính user đó
            string query = @"UPDATE Transactions 
                             SET Amount = @amt, Date = @date, Description = @desc 
                             WHERE Id = @id AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@amt", transaction.Amount),
                new NpgsqlParameter("@date", transaction.Date),
                new NpgsqlParameter("@desc", (object?)transaction.Description ?? DBNull.Value),
                new NpgsqlParameter("@id", transaction.Id),
                new NpgsqlParameter("@uid", transaction.UserId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteTransaction(int userId, int id)
        {
            // Chỉ cho phép xóa giao dịch thuộc chính user đó
            string query = "DELETE FROM Transactions WHERE Id = @id AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public DataTable GetTransactionsByFilter(int userId, int filterTypeIndex, string searchText)
        {
            // Xây dựng query an toàn, dùng parameterized query chống SQL Injection
            string typeFilter = "";
            if (filterTypeIndex == 1) typeFilter = " AND t.Type = 'Thu Nhập'";
            if (filterTypeIndex == 2) typeFilter = " AND t.Type = 'Chi Tiêu'";

            string searchFilter = "";
            if (!string.IsNullOrWhiteSpace(searchText))
                searchFilter = " AND (t.Description ILIKE @search OR c.Name ILIKE @search)";

            string query = $@"
                SELECT t.Id as ""Mã"", t.Type as ""Loại"", t.Date as ""Thời Gian"", 
                       t.Description as ""GhiChu"", ABS(t.Amount) as ""SoTien"", 
                       c.Name as ""DanhMuc""
                FROM Transactions t 
                JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @uid{typeFilter}{searchFilter} 
                ORDER BY t.Date DESC";

            var paramList = new System.Collections.Generic.List<NpgsqlParameter> {
                new NpgsqlParameter("@uid", userId)
            };
            if (!string.IsNullOrWhiteSpace(searchText))
                paramList.Add(new NpgsqlParameter("@search", $"%{searchText}%"));

            return DbConnection.ExecuteQuery(query, paramList.ToArray());
        }

        public decimal GetTotalAmountByType(int userId, string type)
        {
            string query = "SELECT COALESCE(SUM(Amount), 0) FROM Transactions WHERE Type = @type AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@type", type),
                new NpgsqlParameter("@uid", userId)
            };
            var result = DbConnection.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public DataTable GetRecentTransactions(int userId, int limit = 5)
        {
            string query = $@"
                SELECT t.Type as ""Loại"", c.Name as ""Danh Mục"", ABS(t.Amount) as ""Số Tiền""
                FROM Transactions t 
                JOIN Categories c ON t.CategoryId = c.Id 
                WHERE t.UserId = @uid
                ORDER BY t.Date DESC LIMIT {limit}";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@uid", userId) };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetExpensesByCategory(int userId)
        {
            string query = @"
                SELECT c.Name, t.Type, COALESCE(SUM(ABS(t.Amount)), 0) as Total 
                FROM Transactions t
                JOIN Categories c ON c.Id = t.CategoryId
                WHERE t.UserId = @uid
                GROUP BY c.Name, t.Type";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@uid", userId) };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetDailyTransactions(int userId)
        {
            string query = @"
                SELECT DATE_TRUNC('day', t.Date) as Date, t.Type, ABS(SUM(t.Amount)) as Amount
                FROM Transactions t
                WHERE t.UserId = @uid
                GROUP BY DATE_TRUNC('day', t.Date), t.Type
                ORDER BY DATE_TRUNC('day', t.Date) ASC";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@uid", userId) };
            return DbConnection.ExecuteQuery(query, parameters);
        }
    }
}

using System.Data;
using quanlycitieu.DAL;
using quanlycitieu.DTO;
using System;

namespace quanlycitieu.BLL
{
    public class TransactionBLL
    {
        private TransactionDAL transactionDAL = new TransactionDAL();

        public void AddTransaction(TransactionDTO t)
        {
            if (t.Amount <= 0) throw new Exception("Vui lòng nhập số tiền hợp lệ!");
            if (t.CategoryId <= 0) throw new Exception("Vui lòng chọn danh mục!");

            if (t.Type == "Chi Tiêu" && t.Amount > 0)
            {
                t.Amount = -t.Amount;
            }
            transactionDAL.InsertTransaction(t);
        }

        public void UpdateTransaction(TransactionDTO t)
        {
            if (t.Amount <= 0) throw new Exception("Số tiền không hợp lệ!");
            if (t.Type == "Chi Tiêu" && t.Amount > 0) t.Amount = -t.Amount;

            if (!transactionDAL.UpdateTransaction(t))
                throw new Exception("Không thể cập nhật giao dịch này!");
        }

        public void DeleteTransaction(int userId, int id)
        {
            if (!transactionDAL.DeleteTransaction(userId, id))
                throw new Exception("Không thể xóa giao dịch này!");
        }

        public DataTable GetTransactionsByFilter(int userId, int filterTypeIndex, string searchText)
        {
            return transactionDAL.GetTransactionsByFilter(userId, filterTypeIndex, searchText);
        }

        public DataTable GetRecentTransactions(int userId, int limit = 5)
        {
            return transactionDAL.GetRecentTransactions(userId, limit);
        }

        public decimal GetTotalIncome(int userId)
        {
            return transactionDAL.GetTotalAmountByType(userId, "Thu Nhập");
        }

        public decimal GetTotalExpense(int userId)
        {
            decimal expense = transactionDAL.GetTotalAmountByType(userId, "Chi Tiêu");
            return Math.Abs(expense);
        }

        public DataTable GetExpensesByCategory(int userId)
        {
            return transactionDAL.GetExpensesByCategory(userId);
        }

        public DataTable GetDailyTransactions(int userId)
        {
            return transactionDAL.GetDailyTransactions(userId);
        }
    }
}

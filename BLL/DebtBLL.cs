using System;
using System.Data;
using quanlycitieu.DAL;
using quanlycitieu.DTO;

namespace quanlycitieu.BLL
{
    public class DebtBLL
    {
        private DebtDAL debtDAL = new DebtDAL();

        public void AddDebt(int userId, int debtType, string personName, decimal amount, DateTime startDate, DateTime dueDate, string note)
        {
            if (debtType != 1 && debtType != 2)
                throw new Exception("Vui lòng chọn loại giao dịch (Cho vay hoặc Đi vay)!");

            if (string.IsNullOrWhiteSpace(personName))
                throw new Exception("Vui lòng nhập tên người vay / cho vay!");

            if (amount <= 0)
                throw new Exception("Số tiền vay / nợ phải lớn hơn 0đ!");

            if (dueDate.Date < startDate.Date)
                throw new Exception("Hạn trả không thể trước ngày bắt đầu vay!");

            var debt = new DebtDTO
            {
                UserId = userId,
                DebtType = debtType,
                PersonName = personName.Trim(),
                Amount = amount,
                StartDate = startDate.Date,
                DueDate = dueDate.Date,
                Status = 0, // Mặc định là Chưa trả
                Note = note?.Trim() ?? ""
            };

            if (!debtDAL.InsertDebt(debt))
            {
                throw new Exception("Không thể tạo khoản vay/nợ!");
            }
        }

        public void UpdateDebt(int userId, int debtId, int debtType, string personName, decimal amount, DateTime startDate, DateTime dueDate, string note)
        {
            if (debtId <= 0)
                throw new Exception("Khoản nợ không hợp lệ!");

            if (string.IsNullOrWhiteSpace(personName))
                throw new Exception("Vui lòng nhập tên người vay / cho vay!");

            if (amount <= 0)
                throw new Exception("Số tiền phải lớn hơn 0đ!");

            if (dueDate.Date < startDate.Date)
                throw new Exception("Hạn trả không thể trước ngày bắt đầu vay!");

            var debt = new DebtDTO
            {
                Id = debtId,
                UserId = userId,
                DebtType = debtType,
                PersonName = personName.Trim(),
                Amount = amount,
                StartDate = startDate.Date,
                DueDate = dueDate.Date,
                Note = note?.Trim() ?? ""
            };

            if (!debtDAL.UpdateDebt(debt))
            {
                throw new Exception("Không thể cập nhật khoản nợ (chỉ có thể sửa khi chưa tất toán)!");
            }
        }

        public void DeleteDebt(int userId, int debtId)
        {
            if (debtId <= 0) return;
            if (!debtDAL.DeleteDebt(userId, debtId))
            {
                throw new Exception("Không thể xóa khoản nợ này!");
            }
        }

        public DataTable GetDebts(int userId, int? debtTypeFilter, int? statusFilter, string searchText)
        {
            return debtDAL.GetDebts(userId, debtTypeFilter, statusFilter, searchText);
        }

        public DebtDTO? GetDebtById(int userId, int debtId)
        {
            return debtDAL.GetDebtById(userId, debtId);
        }

        /// <summary>
        /// Thực hiện tất toán khoản nợ và tự động sinh giao dịch đối ứng vào Transactions
        /// </summary>
        public string SettleDebt(int userId, int debtId)
        {
            if (debtId <= 0)
                throw new Exception("Khoản nợ không hợp lệ!");

            bool success = debtDAL.SettleDebtWithTransaction(userId, debtId, out string msg);
            if (!success)
            {
                throw new Exception(msg);
            }
            return msg;
        }
    }
}

using System;
using System.Data;
using quanlycitieu.DAL;
using quanlycitieu.DTO;

namespace quanlycitieu.BLL
{
    public class BudgetBLL
    {
        private BudgetDAL budgetDAL = new BudgetDAL();

        public void SetBudget(int userId, int categoryId, decimal amountLimit, int month, int year)
        {
            if (categoryId <= 0)
                throw new Exception("Vui lòng chọn danh mục chi tiêu hợp lệ!");

            if (amountLimit <= 0)
                throw new Exception("Hạn mức ngân sách phải lớn hơn 0đ!");

            if (month < 1 || month > 12)
                throw new Exception("Tháng không hợp lệ!");

            if (year < 2000 || year > 2100)
                throw new Exception("Năm không hợp lệ!");

            var budget = new BudgetDTO
            {
                UserId = userId,
                CategoryId = categoryId,
                AmountLimit = amountLimit,
                Month = month,
                Year = year
            };

            if (!budgetDAL.UpsertBudget(budget))
            {
                throw new Exception("Không thể lưu hạn mức ngân sách!");
            }
        }

        public void DeleteBudget(int userId, int budgetId)
        {
            if (budgetId <= 0) return;
            if (!budgetDAL.DeleteBudget(userId, budgetId))
            {
                throw new Exception("Không thể xóa hạn mức ngân sách này!");
            }
        }

        public DataTable GetBudgets(int userId, int month, int year)
        {
            return budgetDAL.GetBudgetsWithSpent(userId, month, year);
        }

        /// <summary>
        /// Kiểm tra hạn mức chi tiêu của danh mục khi thêm giao dịch chi tiêu mới
        /// Trả về cấp độ cảnh báo: None (dưới 80%), Yellow (80% - 100%), Red (vượt 100%)
        /// </summary>
        public BudgetAlertResult CheckBudgetAlert(int userId, int categoryId, decimal newExpenseAmount, DateTime transactionDate)
        {
            int month = transactionDate.Month;
            int year = transactionDate.Year;

            decimal? limit = budgetDAL.GetBudgetLimit(userId, categoryId, month, year);
            if (!limit.HasValue || limit.Value <= 0)
            {
                // Danh mục này chưa được thiết lập hạn mức
                return new BudgetAlertResult { Level = BudgetAlertLevel.None };
            }

            decimal currentSpent = budgetDAL.GetTotalSpentForCategoryInMonth(userId, categoryId, month, year);
            decimal newTotalSpent = currentSpent + newExpenseAmount;
            decimal budgetLimit = limit.Value;
            double percentUsed = (double)(newTotalSpent / budgetLimit) * 100;
            string categoryName = budgetDAL.GetCategoryName(categoryId);

            var result = new BudgetAlertResult
            {
                CurrentSpent = currentSpent,
                NewTotalSpent = newTotalSpent,
                AmountLimit = budgetLimit,
                PercentUsed = percentUsed
            };

            if (newTotalSpent > budgetLimit)
            {
                // Cảnh báo Đỏ (Vượt quá 100% hạn mức)
                decimal overAmount = newTotalSpent - budgetLimit;
                result.Level = BudgetAlertLevel.Red;
                result.Message = 
                    $"⚠️ [CẢNH BÁO ĐỎ - VƯỢT HẠN MỨC NGÂN SÁCH]\n\n" +
                    $"• Danh mục: {categoryName}\n" +
                    $"• Hạn mức tháng {month}/{year}: {budgetLimit:N0} đ\n" +
                    $"• Đã chi trước đó: {currentSpent:N0} đ\n" +
                    $"• Khoản chi lần này: {newExpenseAmount:N0} đ\n" +
                    $"----------------------------------------\n" +
                    $"➔ Tổng chi sẽ là: {newTotalSpent:N0} đ ({percentUsed:F1}% hạn mức)\n" +
                    $"➔ Vượt quá hạn mức: +{overAmount:N0} đ\n\n" +
                    $"Bạn có chắc chắn VẪN MUỐN LƯU giao dịch này không?";
            }
            else if (newTotalSpent >= budgetLimit * 0.8m)
            {
                // Cảnh báo Vàng (Từ 80% đến 100% hạn mức)
                decimal remaining = budgetLimit - newTotalSpent;
                result.Level = BudgetAlertLevel.Yellow;
                result.Message = 
                    $"⚡ [CẢNH BÁO VÀNG - SẮP CHẠM HẠN MỨC]\n\n" +
                    $"• Danh mục: {categoryName}\n" +
                    $"• Hạn mức tháng {month}/{year}: {budgetLimit:N0} đ\n" +
                    $"• Đã chi (bao gồm lần này): {newTotalSpent:N0} đ ({percentUsed:F1}% hạn mức)\n" +
                    $"• Số tiền còn lại trong ngân sách: {remaining:N0} đ\n\n" +
                    $"Hãy cân nhắc các khoản chi tiêu tiếp theo cho danh mục này!";
            }
            else
            {
                result.Level = BudgetAlertLevel.None;
            }

            return result;
        }
    }
}

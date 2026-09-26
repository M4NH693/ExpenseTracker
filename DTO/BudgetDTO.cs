using System;

namespace quanlycitieu.DTO
{
    public class BudgetDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal AmountLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        // Các thuộc tính tính toán hỗ trợ hiển thị
        public decimal ActualSpent { get; set; }
        public decimal Remaining => AmountLimit - ActualSpent;
        public double PercentUsed => AmountLimit > 0 ? (double)(ActualSpent / AmountLimit) * 100 : 0;
    }

    public enum BudgetAlertLevel
    {
        None,    // Dưới 80%
        Yellow,  // Từ 80% đến 100%
        Red      // Vượt 100%
    }

    public class BudgetAlertResult
    {
        public BudgetAlertLevel Level { get; set; } = BudgetAlertLevel.None;
        public string Message { get; set; } = "";
        public decimal CurrentSpent { get; set; }
        public decimal NewTotalSpent { get; set; }
        public decimal AmountLimit { get; set; }
        public double PercentUsed { get; set; }
    }
}

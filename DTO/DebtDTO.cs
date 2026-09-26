using System;

namespace quanlycitieu.DTO
{
    public class DebtDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DebtType { get; set; } // 1: Cho vay (Lend), 2: Đi vay (Borrow)
        public string PersonName { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; } // 0: Chưa trả, 1: Đã tất toán
        public string Note { get; set; } = "";
        public int? TransactionId { get; set; }

        // Thuộc tính tiện ích hiển thị
        public string DebtTypeName => DebtType == 1 ? "Cho vay" : "Đi vay";
        public string StatusName => Status == 1 ? "Đã tất toán" : "Chưa trả";
        public bool IsOverdue => Status == 0 && DueDate.Date < DateTime.Today;
    }
}

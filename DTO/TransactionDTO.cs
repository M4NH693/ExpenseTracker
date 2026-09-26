using System;

namespace quanlycitieu.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        
        // Extended property for UI Display (JOIN with Categories)
        public string CategoryName { get; set; }
    }
}

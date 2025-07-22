
namespace Cashicart.Common.DTOs
{
    public class SalesReportDto
    {
        public Guid TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

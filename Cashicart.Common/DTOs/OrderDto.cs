
namespace Cashicart.Common.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}

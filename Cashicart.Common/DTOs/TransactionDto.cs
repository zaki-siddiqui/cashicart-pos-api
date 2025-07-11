using Cashicart.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.DTOs
{
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TransactionItemDto> Items { get; set; }
    }

    public class TransactionItemDto
    {
        public Guid TransactionItemId { get; set; }
        public Guid TransactionId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

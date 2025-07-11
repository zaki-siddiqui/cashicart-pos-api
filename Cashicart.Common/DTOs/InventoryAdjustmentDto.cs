using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.DTOs
{
    public class InventoryAdjustmentDto
    {
        public Guid InventoryAdjustmentId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.DTOs
{
    public class SalesReportDto
    {
        public Guid TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        //public decimal TotalSales { get; set; }
        //public int TransactionCount { get; set; }
        //public List<TopProductDto> TopProducts { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.DTOs
{
    public class TopProductDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

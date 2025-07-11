using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Common.DTOs
{
    public class AuditLogDto
    {
        public Guid AuditLogId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

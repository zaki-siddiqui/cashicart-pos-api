using Cashicart.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid AuditLogId { get; private set; }
        public Guid UserId { get; private set; }
        public string Action { get; private set; }
        public string EntityName { get; private set; }
        public Guid EntityId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public AuditLog(Guid userId, string action, string entityName, Guid entityId)
        {
            if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(entityName))
                throw new DomainException("Action and entity name are required for audit log.");
            AuditLogId = Guid.NewGuid();
            UserId = userId;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

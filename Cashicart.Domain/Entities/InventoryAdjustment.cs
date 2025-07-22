using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class InventoryAdjustment
    {
        [Key]
        public Guid InventoryAdjustmentId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public InventoryAdjustment(Guid productId, int quantity, string reason, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Adjustment reason is required.");
            InventoryAdjustmentId = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
            Reason = reason;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

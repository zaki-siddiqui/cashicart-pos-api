using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal TotalAmount { get; private set; } // Reverted to private set
        public List<OrderItem> Items { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        // Parameterless constructor for EF Core
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public Order(Guid customerId, decimal totalAmount, List<OrderItem> items, OrderStatus status)
        {
            if (totalAmount < 0) throw new DomainException("Total amount cannot be negative.");
            if (items == null || !items.Any()) throw new DomainException("Order must have at least one item.");
            OrderId = Guid.NewGuid();
            CustomerId = customerId;
            TotalAmount = totalAmount;
            Items = items ?? new List<OrderItem>();
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }

        // Method to recalculate TotalAmount internally
        public void RecalculateTotalAmount()
        {
            TotalAmount = Items.Sum(i => i.Quantity * i.UnitPrice);
        }

        public void CompleteOrder()
        {
            if (Status != OrderStatus.Pending) throw new DomainException("Order is not in pending state.");
            Status = OrderStatus.Completed;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }

    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice <= 0) throw new DomainException("Unit price must be greater than zero.");
            OrderItemId = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
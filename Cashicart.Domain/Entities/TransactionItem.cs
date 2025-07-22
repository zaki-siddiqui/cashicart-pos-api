using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class TransactionItem
    {
        [Key]
        public Guid TransactionItemId { get; private set; }
        public Guid TransactionId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public TransactionItem(Guid transactionId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice <= 0) throw new DomainException("Unit price must be greater than zero.");
            TransactionItemId = Guid.NewGuid();
            TransactionId = transactionId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}

using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class Receipt
    {
        [Key]
        public Guid ReceiptId { get; private set; } = Guid.NewGuid();
        public string ReceiptNumber { get; private set; }
        public Guid TransactionId { get; private set; }
        public Guid? CustomerId { get; private set; }
        public string CustomerName { get; private set; } = "Walk-in Customer";
        public string? CustomerEmail { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal DiscountAmount { get; private set; } = 0;
        public decimal TotalAmount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public decimal ChangeAmount { get; private set; } = 0;
        public PaymentMethod PaymentMethod { get; private set; }
        public string? PaymentReference { get; private set; } // Card last 4 digits, check number, etc.
        public List<ReceiptItem> Items { get; private set; } = new List<ReceiptItem>();
        public string? Notes { get; private set; }
        public bool IsEmailSent { get; private set; } = false;
        public bool IsPrinted { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public Guid CreatedByUserId { get; private set; }

        // Navigation properties
        public virtual Transaction Transaction { get; private set; }
        public virtual Customer? Customer { get; private set; }
        public virtual User CreatedByUser { get; private set; }

        public Receipt()
        {
            // Parameterless constructor for EF Core
        }

        public Receipt(string receiptNumber, Guid transactionId, Guid createdByUserId,
                      decimal subTotal, decimal taxAmount, decimal totalAmount, 
                      decimal amountPaid, PaymentMethod paymentMethod,
                      List<ReceiptItem> items, Guid? customerId = null, 
                      string customerName = "Walk-in Customer", string? customerEmail = null,
                      decimal discountAmount = 0, decimal changeAmount = 0,
                      string? paymentReference = null, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                throw new DomainException("Receipt number is required.");
            if (items == null || !items.Any())
                throw new DomainException("Receipt must have at least one item.");
            if (subTotal < 0) throw new DomainException("Subtotal cannot be negative.");
            if (taxAmount < 0) throw new DomainException("Tax amount cannot be negative.");
            if (totalAmount < 0) throw new DomainException("Total amount cannot be negative.");
            if (amountPaid < 0) throw new DomainException("Amount paid cannot be negative.");

            ReceiptNumber = receiptNumber;
            TransactionId = transactionId;
            CreatedByUserId = createdByUserId;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            SubTotal = subTotal;
            TaxAmount = taxAmount;
            DiscountAmount = discountAmount;
            TotalAmount = totalAmount;
            AmountPaid = amountPaid;
            ChangeAmount = changeAmount;
            PaymentMethod = paymentMethod;
            PaymentReference = paymentReference;
            Items = items;
            Notes = notes;
        }

        public void MarkAsEmailSent()
        {
            IsEmailSent = true;
        }

        public void MarkAsPrinted()
        {
            IsPrinted = true;
        }

        public static string GenerateReceiptNumber(DateTime date, int sequenceNumber)
        {
            return $"R{date:yyyyMMdd}-{sequenceNumber:D4}";
        }
    }

    public class ReceiptItem
    {
        [Key]
        public Guid ReceiptItemId { get; private set; } = Guid.NewGuid();
        public Guid ReceiptId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string? ProductSKU { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal LineTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal DiscountAmount { get; private set; } = 0;

        public ReceiptItem()
        {
            // Parameterless constructor for EF Core
        }

        public ReceiptItem(Guid receiptId, Guid productId, string productName, 
                          int quantity, decimal unitPrice, decimal taxAmount = 0, 
                          decimal discountAmount = 0, string? productSKU = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name is required.");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice < 0) throw new DomainException("Unit price cannot be negative.");

            ReceiptId = receiptId;
            ProductId = productId;
            ProductName = productName;
            ProductSKU = productSKU;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TaxAmount = taxAmount;
            DiscountAmount = discountAmount;
            LineTotal = (quantity * unitPrice) - discountAmount + taxAmount;
        }
    }

    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        MobilePayment = 4,
        Check = 5,
        GiftCard = 6,
        StoreCredit = 7
    }
}
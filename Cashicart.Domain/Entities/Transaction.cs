using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

public class Transaction
{
    [Key]
    public Guid TransactionId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public List<TransactionItem> Items { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;


    public Transaction()
    {
        // Parameterless constructor for EF Core
        Items = new List<TransactionItem>();
    }

    public Transaction(Guid userId, decimal totalAmount, List<TransactionItem> items, TransactionStatus status, DateTime createdAt)
    {
        if (totalAmount < 0) throw new DomainException("Total amount cannot be negative.");
        if (items == null || !items.Any()) throw new DomainException("Transaction must have at least one item.");
        TransactionId = Guid.NewGuid();
        UserId = userId;
        TotalAmount = totalAmount;
        Status = status;
        CreatedAt = createdAt;
        Items = items ?? new List<TransactionItem>();
    }

    // New constructor for testing
    public Transaction(Guid transactionId, Guid userId, decimal totalAmount, List<TransactionItem> items, TransactionStatus status, DateTime createdAt)
    {
        if (totalAmount < 0) throw new DomainException("Total amount cannot be negative.");
        if (items == null || !items.Any()) throw new DomainException("Transaction must have at least one item.");
        TransactionId = transactionId;
        UserId = userId;
        TotalAmount = totalAmount;
        Status = status;
        CreatedAt = createdAt;
        Items = items ?? new List<TransactionItem>();
    }

    // Factory method to initialize with minimal data for seeding
    public static Transaction CreateForSeeding(Guid id, Guid userId, decimal totalAmount, TransactionStatus status, DateTime createdAt)
    {
        var transaction = new Transaction
        {
            TransactionId = id,
            UserId = userId,
            TotalAmount = totalAmount,
            Status = status,
            CreatedAt = createdAt
        };
        return transaction;
    }

    public void CompleteTransaction()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Transaction is not in pending state.");
        Status = TransactionStatus.Completed;
    }

    public void CancelTransaction()
    {
        if (Status != TransactionStatus.Completed)
            throw new DomainException("Only completed transactions can be cancelled.");
        Status = TransactionStatus.Cancelled;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled
}
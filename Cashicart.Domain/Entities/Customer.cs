using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public Customer(string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Customer name is required.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) throw new DomainException("Invalid email address.");
            CustomerId = Guid.NewGuid();
            Name = name;
            Email = email;
            Phone = phone;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Customer name is required.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) throw new DomainException("Invalid email address.");
            Name = name;
            Email = email;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

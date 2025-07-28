using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class Business
    {
        [Key]
        public Guid BusinessId { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Address { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? ZipCode { get; private set; }
        public string Country { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Website { get; private set; }
        public string? TaxId { get; private set; } // For tax reporting
        public string Currency { get; private set; } = "USD";
        public string TimeZone { get; private set; } = "UTC";
        public BusinessType Type { get; private set; }
        public SubscriptionPlan Plan { get; private set; } = SubscriptionPlan.Trial;
        public DateTime? SubscriptionExpiresAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        // Navigation properties
        public virtual ICollection<User> Users { get; private set; } = new List<User>();
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();
        public virtual ICollection<Customer> Customers { get; private set; } = new List<Customer>();

        public Business()
        {
            // Parameterless constructor for EF Core
        }

        public Business(string name, string country, BusinessType type, 
                       string? description = null, string? address = null, 
                       string? city = null, string? state = null, 
                       string? zipCode = null, string? phone = null, 
                       string? email = null, string? taxId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Business name is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country is required.");

            Name = name;
            Description = description;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            Phone = phone;
            Email = email;
            TaxId = taxId;
            Type = type;

            // Set currency based on country
            Currency = country.ToUpper() switch
            {
                "US" or "USA" => "USD",
                "UK" or "GB" => "GBP",
                "CA" => "CAD",
                "AU" => "AUD",
                _ => "USD"
            };

            // Set trial subscription
            Plan = SubscriptionPlan.Trial;
            SubscriptionExpiresAt = DateTime.UtcNow.AddDays(30);
        }

        public void UpdateBusinessInfo(string name, string? description = null, 
                                     string? address = null, string? city = null, 
                                     string? state = null, string? zipCode = null, 
                                     string? phone = null, string? email = null, 
                                     string? website = null, string? taxId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Business name is required.");

            Name = name;
            Description = description;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            Phone = phone;
            Email = email;
            Website = website;
            TaxId = taxId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSubscription(SubscriptionPlan plan, DateTime expiresAt)
        {
            Plan = plan;
            SubscriptionExpiresAt = expiresAt;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsSubscriptionActive => 
            IsActive && SubscriptionExpiresAt.HasValue && SubscriptionExpiresAt > DateTime.UtcNow;

        public bool IsTrialExpired => 
            Plan == SubscriptionPlan.Trial && SubscriptionExpiresAt.HasValue && SubscriptionExpiresAt < DateTime.UtcNow;
    }

    public enum BusinessType
    {
        Retail = 1,
        Restaurant = 2,
        Service = 3,
        Wholesale = 4,
        Other = 5
    }

    public enum SubscriptionPlan
    {
        Trial = 0,
        Basic = 1,      // $29/month - Up to 1000 transactions
        Professional = 2, // $79/month - Up to 5000 transactions
        Enterprise = 3   // $199/month - Unlimited transactions
    }
}
using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class BusinessSettings
    {
        [Key]
        public Guid BusinessSettingsId { get; private set; } = Guid.NewGuid();
        public string BusinessName { get; private set; }
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
        public decimal DefaultTaxRate { get; private set; } = 0.0m;
        public BusinessType Type { get; private set; }
        
        // Receipt Settings
        public string? ReceiptHeader { get; private set; }
        public string? ReceiptFooter { get; private set; }
        public bool PrintReceiptAutomatically { get; private set; } = true;
        public bool EmailReceiptsByDefault { get; private set; } = false;
        
        // Software Settings
        public string? LicenseKey { get; private set; }
        public DateTime? LicenseExpiresAt { get; private set; }
        public bool AutoBackup { get; private set; } = true;
        public int BackupRetentionDays { get; private set; } = 30;
        
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public BusinessSettings()
        {
            // Parameterless constructor for EF Core
        }

        public BusinessSettings(string businessName, string country, BusinessType type, 
                               string? description = null, string? address = null, 
                               string? city = null, string? state = null, 
                               string? zipCode = null, string? phone = null, 
                               string? email = null, string? taxId = null)
        {
            if (string.IsNullOrWhiteSpace(businessName))
                throw new DomainException("Business name is required.");
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("Country is required.");

            BusinessName = businessName;
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

            // Set default tax rate based on country
            DefaultTaxRate = country.ToUpper() switch
            {
                "US" or "USA" => 0.0875m, // Average US sales tax
                "UK" or "GB" => 0.20m,     // UK VAT
                "CA" => 0.13m,             // Average Canadian tax
                _ => 0.0m
            };
        }

        public void UpdateBusinessInfo(string businessName, string? description = null, 
                                     string? address = null, string? city = null, 
                                     string? state = null, string? zipCode = null, 
                                     string? phone = null, string? email = null, 
                                     string? website = null, string? taxId = null)
        {
            if (string.IsNullOrWhiteSpace(businessName))
                throw new DomainException("Business name is required.");

            BusinessName = businessName;
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

        public void UpdateTaxRate(decimal taxRate)
        {
            if (taxRate < 0 || taxRate > 1)
                throw new DomainException("Tax rate must be between 0 and 1.");
            DefaultTaxRate = taxRate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateReceiptSettings(string? header, string? footer, 
                                        bool printAutomatically, bool emailByDefault)
        {
            ReceiptHeader = header;
            ReceiptFooter = footer;
            PrintReceiptAutomatically = printAutomatically;
            EmailReceiptsByDefault = emailByDefault;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLicense(string licenseKey, DateTime? expiresAt)
        {
            LicenseKey = licenseKey;
            LicenseExpiresAt = expiresAt;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateBackupSettings(bool autoBackup, int retentionDays)
        {
            if (retentionDays < 1)
                throw new DomainException("Backup retention days must be at least 1.");
            AutoBackup = autoBackup;
            BackupRetentionDays = retentionDays;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsLicenseValid => 
            !string.IsNullOrEmpty(LicenseKey) && 
            (!LicenseExpiresAt.HasValue || LicenseExpiresAt > DateTime.UtcNow);
    }

    public enum BusinessType
    {
        Retail = 1,
        Restaurant = 2,
        Service = 3,
        Wholesale = 4,
        Other = 5
    }
}
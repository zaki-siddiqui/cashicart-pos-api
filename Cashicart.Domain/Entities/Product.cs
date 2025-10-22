using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;


namespace Cashicart.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string SKU { get; private set; }
        public decimal Price { get; private set; }
        public decimal? CostPrice { get; private set; } // For profit margin calculations
        public int StockQuantity { get; private set; }
        public int? LowStockThreshold { get; private set; } = 10; // Alert when stock is low
        public Guid CategoryId { get; private set; }
        public bool IsTaxable { get; private set; } = true;
        public decimal TaxRate { get; private set; } = 0.0m; // Default tax rate for this product
        public string? Barcode { get; private set; } // For barcode scanning
        public ProductStatus Status { get; private set; } = ProductStatus.Active;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public string? Description { get; private set; }
        
        // Navigation properties
        public virtual Category Category { get; private set; }
        public virtual ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
        public virtual ICollection<ProductTranslation> Translations { get; private set; } = new List<ProductTranslation>();



        public Product()
        {
            // Parameterless constructor for EF Core
        }

        public Product(string name, string sku, decimal price, int stockQuantity, 
                      Guid categoryId, string? description = null, 
                      decimal? costPrice = null, string? barcode = null, 
                      bool isTaxable = true, decimal taxRate = 0.0m, 
                      int? lowStockThreshold = 10)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("SKU is required.");
            if (price < 0) throw new DomainException("Price cannot be negative.");
            if (stockQuantity < 0) throw new DomainException("Stock quantity cannot be negative.");
            if (costPrice.HasValue && costPrice < 0) throw new DomainException("Cost price cannot be negative.");
            if (taxRate < 0 || taxRate > 1) throw new DomainException("Tax rate must be between 0 and 1.");

            ProductId = Guid.NewGuid();
            Name = name;
            SKU = sku;
            Price = price;
            CostPrice = costPrice;
            StockQuantity = stockQuantity;
            LowStockThreshold = lowStockThreshold;
            CategoryId = categoryId;
            Description = description;
            Barcode = barcode;
            IsTaxable = isTaxable;
            TaxRate = taxRate;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string sku, decimal? price, int? stockQuantity, Guid categoryId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("SKU is required.");
            if (price.HasValue && price <= 0) throw new DomainException("Price must be greater than zero.");
            if (stockQuantity.HasValue && stockQuantity < 0) throw new DomainException("Stock quantity cannot be negative.");

            Name = name;
            SKU = sku;
            if (price.HasValue) Price = price.Value;
            if (stockQuantity.HasValue) StockQuantity = stockQuantity.Value;
            CategoryId = categoryId;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        

        public void UpdateStock(int quantity)
        {
            if (quantity < 0 && StockQuantity < Math.Abs(quantity))
                throw new DomainException("Insufficient stock for product.");
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal price)
        {
            if (price <= 0) throw new DomainException("Price must be greater than zero.");
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string? description)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCostPrice(decimal? costPrice)
        {
            if (costPrice.HasValue && costPrice < 0) 
                throw new DomainException("Cost price cannot be negative.");
            CostPrice = costPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateBarcode(string? barcode)
        {
            Barcode = barcode;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTaxSettings(bool isTaxable, decimal taxRate)
        {
            if (taxRate < 0 || taxRate > 1) 
                throw new DomainException("Tax rate must be between 0 and 1.");
            IsTaxable = isTaxable;
            TaxRate = taxRate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(ProductStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLowStockThreshold(int? threshold)
        {
            if (threshold.HasValue && threshold < 0)
                throw new DomainException("Low stock threshold cannot be negative.");
            LowStockThreshold = threshold;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsLowStock => LowStockThreshold.HasValue && StockQuantity <= LowStockThreshold;
        public decimal? ProfitMargin => CostPrice.HasValue ? (Price - CostPrice.Value) / Price : null;
        public decimal CalculateTax(decimal quantity = 1) => IsTaxable ? Price * quantity * TaxRate : 0;
    }

    public enum ProductStatus
    {
        Active = 1,
        Inactive = 2,
        Discontinued = 3
    }
}
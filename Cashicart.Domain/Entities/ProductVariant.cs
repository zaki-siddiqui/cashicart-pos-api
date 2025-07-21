using Cashicart.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Cashicart.Domain.Entities
{
    public class ProductVariant
    {
        [Key]
        public Guid VariantId { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; }
        public string SKU { get; private set; }
        public string? Size { get; private set; }
        public string? Color { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public virtual Product Product { get; private set; }

        private ProductVariant()
        {
            // Parameterless constructor for EF Core
        }

        public ProductVariant(Guid productId, string sku, decimal price, int stockQuantity, string? size = null, string? color = null)
        {
            ProductId = productId;
            SKU = sku;
            Price = price;
            StockQuantity = stockQuantity;
            Size = size;
            Color = color;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateVariant(string sku, decimal price, int stockQuantity, string? size = null, string? color = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new DomainException("Variant SKU is required.");
            if (price <= 0)
                throw new DomainException("Variant price must be greater than zero.");
            if (stockQuantity < 0)
                throw new DomainException("Variant stock cannot be negative.");

            SKU = sku;
            Price = price;
            StockQuantity = stockQuantity;
            Size = size;
            Color = color;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal price)
        {
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

using Cashicart.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string SKU { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public Guid CategoryId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public string? Description { get; private set; }
        public virtual Category Category { get; private set; }
        public virtual ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
        public virtual ICollection<ProductTranslation> Translations { get; private set; } = new List<ProductTranslation>();



        public Product()
        {
            // Parameterless constructor for EF Core
        }

        //public Product(string name, string sku, decimal price, int stockQuantity, Guid categoryId)
        //{
        //    if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
        //    if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("SKU is required.");
        //    //if (price <= 0) throw new DomainException("Price must be greater than zero.");
        //    if (price < 0) throw new DomainException("Price cannot be negative.");
        //    if (stockQuantity < 0) throw new DomainException("Stock quantity cannot be negative.");
        //    ProductId = Guid.NewGuid();
        //    Name = name;
        //    SKU = sku;
        //    Price = price;
        //    StockQuantity = stockQuantity;
        //    CategoryId = categoryId;
        //    CreatedAt = DateTime.UtcNow;
        //    UpdatedAt = DateTime.UtcNow;
        //}

        public Product(string name, string sku, decimal price, int stockQuantity, Guid categoryId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainException("SKU is required.");
            if (price < 0) throw new DomainException("Price cannot be negative.");
            if (stockQuantity < 0) throw new DomainException("Stock quantity cannot be negative.");

            ProductId = Guid.NewGuid();
            Name = name;
            SKU = sku;
            Price = price;
            StockQuantity = stockQuantity;
            CategoryId = categoryId;
            Description = description;
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
    }
}
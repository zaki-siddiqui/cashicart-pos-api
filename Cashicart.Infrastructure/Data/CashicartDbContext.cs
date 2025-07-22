using Microsoft.EntityFrameworkCore;
using Cashicart.Domain.Entities;

namespace Cashicart.Infrastructure.Data
{
    public class CashicartDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public CashicartDbContext(DbContextOptions<CashicartDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Disable concurrency check globally
            modelBuilder.Entity<Product>().Property(p => p.ProductId).IsConcurrencyToken(false);
            modelBuilder.Entity<ProductTranslation>().Property(t => t.TranslationId).IsConcurrencyToken(false);


            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(v => v.SKU)
                .IsUnique();

            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Translations)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductTranslation>()
                .HasIndex(t => new { t.ProductId, t.Language })
                .IsUnique();

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<InventoryAdjustment>().HasQueryFilter(i => !i.IsDeleted);
        }

        public async Task CommitAsync() => await SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Cashicart.Domain.Entities;
using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Cashicart.Infrastructure.Data;
public class CashicartDbContext : DbContext, IUnitOfWork
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


    public CashicartDbContext(DbContextOptions<CashicartDbContext> options) : base(options) { }

    public IRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(this, new Logger<GenericRepository<T>>(new LoggerFactory()));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name);

        modelBuilder.Entity<Transaction>()
            .HasMany(t => t.Items)
            .WithOne()
            .HasForeignKey(ti => ti.TransactionId);
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Name);

        modelBuilder.Entity<InventoryAdjustment>()
            .HasIndex(ia => ia.ProductId);
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CustomerId);
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CreatedAt);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Transaction>()
            .Property(p => p.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<TransactionItem>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(p => p.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(p => p.UnitPrice)
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
            .Property(p => p.Description)
            .HasMaxLength(5000);

        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<Transaction>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<InventoryAdjustment>().HasQueryFilter(o => !o.IsDeleted);

        // Seed Data
        //modelBuilder.Entity<Product>().HasData(
        //    new Product("Laptop", "LAP001", 999.99m, 10, Guid.Parse("11111111-1111-1111-1111-111111111111")),
        //    new Product("Phone", "PHN001", 499.99m, 20, Guid.Parse("22222222-2222-2222-2222-222222222222"))
        //);
        //modelBuilder.Entity<Customer>().HasData(
        //    new Customer("John Doe", "john@example.com", "123-456-7890")
        //);
        //modelBuilder.Entity<Transaction>().HasData(
        //    Transaction.CreateForSeeding(
        //        Guid.Parse("99999999-9999-9999-9999-999999999999"),
        //        Guid.Parse("66666666-6666-6666-6666-666666666666"),
        //        1499.98m,
        //        TransactionStatus.Completed,
        //        DateTime.UtcNow.AddDays(-1)
        //    )
        //);
        //modelBuilder.Entity<TransactionItem>().HasData(
        //    new TransactionItem(Guid.Parse("99999999-9999-9999-9999-999999999999"), Guid.Parse("77777777-7777-7777-7777-777777777777"), 1, 999.99m),
        //    new TransactionItem(Guid.Parse("99999999-9999-9999-9999-999999999999"), Guid.Parse("88888888-8888-8888-8888-888888888888"), 1, 499.99m)
        //);
        //modelBuilder.Entity<InventoryAdjustment>().HasData(
        //    new InventoryAdjustment(Guid.Parse("77777777-7777-7777-7777-777777777777"), 5, "Restock", Guid.Parse("66666666-6666-6666-6666-666666666666"))
        //);
        //modelBuilder.Entity<AuditLog>().HasData(
        //    new AuditLog(Guid.Parse("66666666-6666-6666-6666-666666666666"), "CreateProduct", "Product", Guid.Parse("77777777-7777-7777-7777-777777777777"))
        //);
    }

    public async Task CommitAsync() => await SaveChangesAsync();
}
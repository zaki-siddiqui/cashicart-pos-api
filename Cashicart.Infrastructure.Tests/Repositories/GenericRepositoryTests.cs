using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Infrastructure.Data;
using Cashicart.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cashicart.Infrastructure.Tests.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly CashicartDbContext _context;
        private readonly IRepository<Product> _repository;

        public GenericRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<CashicartDbContext>()
                .UseSqlite(connection)
                .Options;
            _context = new CashicartDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new GenericRepository<Product>(_context, new Logger<GenericRepository<Product>>(new LoggerFactory()));
        }

        [Fact]
        public async Task Given_ExistingId_When_GetByIdAsync_Then_ReturnsProduct()
        {
            // Arrange
            var product = new Product("Test", "TST001", 100m, 10, Guid.NewGuid());
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            Assert.Equal(product.ProductId, result.ProductId);
        }

        [Fact]
        public async Task Given_NonExistingId_When_GetByIdAsync_Then_ThrowsNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _repository.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task Given_ValidEntity_When_AddAsync_Then_AddsToContext()
        {
            // Arrange
            var product = new Product("Test", "TST002", 200m, 20, Guid.NewGuid());

            // Act
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Contains(_context.Products, p => p.ProductId == product.ProductId);
        }

        [Fact]
        public async Task Given_ValidEntity_When_UpdateAsync_Then_UpdatesContext()
        {
            // Arrange
            var product = new Product("Test", "TST003", 300m, 30, Guid.NewGuid());
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            product.UpdatePrice(350m);

            // Act
            await _repository.UpdateAsync(product);
            await _context.SaveChangesAsync();

            // Assert
            var updatedProduct = await _repository.GetByIdAsync(product.ProductId);
            Assert.Equal(350m, updatedProduct.Price);
        }
    }
}
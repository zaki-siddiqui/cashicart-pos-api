using Cashicart.Application.Features.Products.Commands;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cashicart.Application.Tests
{
    public class UnitTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;
        private readonly CreateProductCommandHandler _handler;

        public UnitTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
            var productRepositoryMock = new Mock<IRepository<Product>>();
            productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.GetRepository<Product>()).Returns(productRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _handler = new CreateProductCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object); // Two parameters
        }

        [Fact]
        public async Task CreateProductCommandHandler_ValidInput_ReturnsGuid()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                SKU = "TST001",
                Price = 100m,
                StockQuantity = 10,
                CategoryId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _unitOfWorkMock.Verify(u => u.GetRepository<Product>().AddAsync(It.IsAny<Product>()), Times.Once());
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once());
        }
    }
}
using Cashicart.Application.Features.Products.Queries;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;


namespace Cashicart.Application.Tests.Features.Products
{
    public class GetProductQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<GetProductQueryHandler>> _loggerMock;
        private readonly GetProductQueryHandler _handler;

        public GetProductQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<GetProductQueryHandler>>();
            _handler = new GetProductQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ReturnsProduct()
        {
            // Arrange
            var query = new GetProductQuery { ProductId = Guid.NewGuid() };
            var product = new Product("Test", "TST001", 100m, 10, Guid.NewGuid());
            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(r => r.GetByIdAsync(query.ProductId)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.GetRepository<Product>()).Returns(productRepoMock.Object);

            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching product with ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(product.ProductId, result.ProductId);
            _loggerMock.Verify();
        }
    }
}

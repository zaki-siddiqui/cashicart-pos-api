using Cashicart.Application.Features.Inventory.Queries;
using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Tests.Features.Inventory
{
    public class GetInventoryAdjustmentQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<GetInventoryAdjustmentQueryHandler>> _loggerMock;
        private readonly GetInventoryAdjustmentQueryHandler _handler;

        public GetInventoryAdjustmentQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<GetInventoryAdjustmentQueryHandler>>();
            _handler = new GetInventoryAdjustmentQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ReturnsAdjustment()
        {
            // Arrange
            var query = new GetInventoryAdjustmentQuery { InventoryAdjustmentId = Guid.NewGuid() };
            var adjustment = new InventoryAdjustment(Guid.NewGuid(), 5, "Restock", Guid.NewGuid());
            var adjustmentRepoMock = new Mock<IRepository<InventoryAdjustment>>();
            adjustmentRepoMock.Setup(r => r.GetByIdAsync(query.InventoryAdjustmentId)).ReturnsAsync(adjustment);
            _unitOfWorkMock.Setup(u => u.GetRepository<InventoryAdjustment>()).Returns(adjustmentRepoMock.Object);

            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching inventory adjustment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(adjustment.InventoryAdjustmentId, result.InventoryAdjustmentId);
            _loggerMock.Verify();
        }
    }
}

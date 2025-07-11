using Cashicart.Application.Features.Inventory.Commands;
using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Tests.Features.Inventory
{
    public class AdjustInventoryCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AdjustInventoryCommandHandler>> _loggerMock;
        private readonly AdjustInventoryCommandHandler _handler;

        public AdjustInventoryCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AdjustInventoryCommandHandler>>();
            _handler = new AdjustInventoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsAdjustmentId()
        {
            // Arrange
            var command = new AdjustInventoryCommand { ProductId = Guid.NewGuid(), Quantity = 5, Reason = "Restock", UserId = Guid.NewGuid() };
            var adjustmentId = Guid.NewGuid(); // Expected ID for verification
            var adjustment = new InventoryAdjustment(command.ProductId, command.Quantity, command.Reason, command.UserId); // ID set internally
            var productMock = new Mock<Product>("Test", "TST001", 100m, 10, Guid.NewGuid());
            var productRepoMock = new Mock<IRepository<Product>>();
            productRepoMock.Setup(r => r.GetByIdAsync(command.ProductId)).ReturnsAsync(productMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Product>()).Returns(productRepoMock.Object);

            var adjustmentRepoMock = new Mock<IRepository<InventoryAdjustment>>();
            adjustmentRepoMock.Setup(r => r.AddAsync(It.IsAny<InventoryAdjustment>())).Returns(Task.CompletedTask); // No ID assignment here
            _unitOfWorkMock.Setup(u => u.GetRepository<InventoryAdjustment>()).Returns(adjustmentRepoMock.Object);

            var auditLogRepoMock = new Mock<IRepository<AuditLog>>();
            _unitOfWorkMock.Setup(u => u.GetRepository<AuditLog>()).Returns(auditLogRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adjusting inventory")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result); // Verify a valid ID is returned
            _loggerMock.Verify();
        }
    }
}

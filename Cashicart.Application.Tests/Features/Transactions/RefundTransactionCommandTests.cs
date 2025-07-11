using Cashicart.Application.Features.Transactions.Commands;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using Cashicart.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Tests.Features.Transactions
{
    public class RefundTransactionCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<ILogger<RefundTransactionCommandHandler>> _loggerMock;
        private readonly RefundTransactionCommandHandler _handler;

        public RefundTransactionCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _loggerMock = new Mock<ILogger<RefundTransactionCommandHandler>>();
            _handler = new RefundTransactionCommandHandler(_unitOfWorkMock.Object, _paymentServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_PerformsRefund()
        {
            // Arrange
            var command = new RefundTransactionCommand { TransactionId = Guid.NewGuid(), Amount = 100m, UserId = Guid.NewGuid() };
            var transactionItem = new TransactionItem(command.TransactionId, Guid.NewGuid(), 1, 50m); // 4 arguments
            var transaction = new Transaction(command.UserId, 100m, new List<TransactionItem> { transactionItem }, TransactionStatus.Completed, DateTime.UtcNow);
            var transactionRepoMock = new Mock<IRepository<Transaction>>();
            transactionRepoMock.Setup(r => r.GetByIdAsync(command.TransactionId)).ReturnsAsync(transaction);
            _unitOfWorkMock.Setup(u => u.GetRepository<Transaction>()).Returns(transactionRepoMock.Object);

            var auditLogRepoMock = new Mock<IRepository<AuditLog>>();
            _unitOfWorkMock.Setup(u => u.GetRepository<AuditLog>()).Returns(auditLogRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _paymentServiceMock.Setup(p => p.RefundPayment(command.TransactionId, command.Amount)).Returns(Task.CompletedTask);

            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transaction")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _loggerMock.Verify();
        }
    }
}

using Cashicart.Application.Features.Reports.Queries;
using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cashicart.Application.Tests.Features.Reports
{
    public class GetSalesReportQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<GetSalesReportQueryHandler>> _loggerMock;
        private readonly GetSalesReportQueryHandler _handler;

        public GetSalesReportQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<GetSalesReportQueryHandler>>();
            _handler = new GetSalesReportQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidDates_ReturnsReport()
        {
            // Arrange
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<CashicartDbContext>()
                .UseSqlite(connection)
                .Options;
            using var context = new CashicartDbContext(options);
            context.Database.EnsureCreated(); // Uses seeded data

            var query = new GetSalesReportQuery { StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow };
            var transactionRepoMock = new Mock<IRepository<Transaction>>();
            transactionRepoMock.Setup(r => r.GetAll()).Returns(context.Transactions.Include(t => t.Items).AsQueryable());
            _unitOfWorkMock.Setup(u => u.GetRepository<Transaction>()).Returns(transactionRepoMock.Object);

            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching sales report")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result); // One transaction seeded
            _loggerMock.Verify();
        }
    }
}
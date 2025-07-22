using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Transactions.Queries
{
    public class GetTransactionQuery : IRequest<TransactionDto>
    {
        public Guid TransactionId { get; set; }
    }

    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTransactionQueryHandler> _logger;

        public GetTransactionQueryHandler(IUnitOfWork unitOfWork, ILogger<GetTransactionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TransactionDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching transaction with ID {TransactionId}", request.TransactionId);
            var transaction = await _unitOfWork.GetRepository<Transaction>().GetByIdAsync(request.TransactionId);
            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                UserId = transaction.UserId,
                TotalAmount = transaction.TotalAmount,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt,
                Items = transaction.Items.Select(i => new TransactionItemDto
                {
                    TransactionItemId = i.TransactionItemId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}

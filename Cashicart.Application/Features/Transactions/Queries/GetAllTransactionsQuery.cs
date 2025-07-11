using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Features.Transactions.Queries
{
    public class GetAllTransactionsQuery : IRequest<List<TransactionDto>>
    {
    }

    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTransactionsQueryHandler> _logger;

        public GetAllTransactionsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllTransactionsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all transactions");
            var transactions = await _unitOfWork.GetRepository<Transaction>().GetAll().ToListAsync();
            return transactions.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                TotalAmount = t.TotalAmount,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                Items = t.Items.Select(i => new TransactionItemDto
                {
                    TransactionItemId = i.TransactionItemId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }
    }
}

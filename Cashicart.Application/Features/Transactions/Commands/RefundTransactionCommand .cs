using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using Cashicart.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Transactions.Commands;

public class RefundTransactionCommand : IRequest
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
}

public class RefundTransactionCommandHandler : IRequestHandler<RefundTransactionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<RefundTransactionCommandHandler> _logger;

    public RefundTransactionCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        ILogger<RefundTransactionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(RefundTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.GetRepository<Transaction>().GetByIdAsync(request.TransactionId);
        if (transaction.Status != TransactionStatus.Completed)
            throw new DomainException("Only completed transactions can be refunded.");

        await _paymentService.RefundPayment(transaction.TransactionId, request.Amount);
        transaction.CancelTransaction();
        await _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);

        var auditLog = new AuditLog(request.UserId, "RefundTransaction", nameof(Transaction), transaction.TransactionId);
        await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Transaction {TransactionId} refunded", request.TransactionId);
    }
}
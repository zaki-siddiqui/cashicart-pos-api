using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Inventory.Commands;

public class AdjustInventoryCommand : IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; }
    public Guid UserId { get; set; }
}

public class AdjustInventoryCommandHandler : IRequestHandler<AdjustInventoryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdjustInventoryCommandHandler> _logger;

    public AdjustInventoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AdjustInventoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adjusting inventory for product {ProductId}", request.ProductId);
        var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(request.ProductId);
        product.UpdateStock(request.Quantity);
        var adjustment = new InventoryAdjustment(request.ProductId, request.Quantity, request.Reason, request.UserId);
        await _unitOfWork.GetRepository<InventoryAdjustment>().AddAsync(adjustment);
        await _unitOfWork.GetRepository<Product>().UpdateAsync(product);

        var auditLog = new AuditLog(request.UserId, "AdjustInventory", nameof(InventoryAdjustment), adjustment.InventoryAdjustmentId);
        await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
        await _unitOfWork.CommitAsync();

        return adjustment.InventoryAdjustmentId;
    }
}
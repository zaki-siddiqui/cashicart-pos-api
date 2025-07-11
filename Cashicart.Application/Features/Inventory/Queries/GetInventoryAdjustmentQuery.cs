using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Inventory.Queries;

public class GetInventoryAdjustmentQuery : IRequest<InventoryAdjustmentDto>
{
    public Guid InventoryAdjustmentId { get; set; }
}

public class GetInventoryAdjustmentQueryHandler : IRequestHandler<GetInventoryAdjustmentQuery, InventoryAdjustmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInventoryAdjustmentQueryHandler> _logger;

    public GetInventoryAdjustmentQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInventoryAdjustmentQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<InventoryAdjustmentDto> Handle(GetInventoryAdjustmentQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching inventory adjustment with ID {InventoryAdjustmentId}", request.InventoryAdjustmentId);
        var adjustment = await _unitOfWork.GetRepository<InventoryAdjustment>().GetByIdAsync(request.InventoryAdjustmentId);
        return new InventoryAdjustmentDto
        {
            InventoryAdjustmentId = adjustment.InventoryAdjustmentId,
            ProductId = adjustment.ProductId,
            Quantity = adjustment.Quantity,
            Reason = adjustment.Reason,
            UserId = adjustment.UserId,
            CreatedAt = adjustment.CreatedAt
        };
    }
}
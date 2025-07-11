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

namespace Cashicart.Application.Features.Inventory.Queries
{
    public class GetAllInventoryAdjustmentsQuery : IRequest<List<InventoryAdjustmentDto>>
    {
    }

    public class GetAllInventoryAdjustmentsQueryHandler : IRequestHandler<GetAllInventoryAdjustmentsQuery, List<InventoryAdjustmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllInventoryAdjustmentsQueryHandler> _logger;

        public GetAllInventoryAdjustmentsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllInventoryAdjustmentsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<InventoryAdjustmentDto>> Handle(GetAllInventoryAdjustmentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all inventory adjustments");
            var adjustments = await _unitOfWork.GetRepository<InventoryAdjustment>().GetAll().ToListAsync();
            return adjustments.Select(a => new InventoryAdjustmentDto
            {
                InventoryAdjustmentId = a.InventoryAdjustmentId,
                ProductId = a.ProductId,
                Quantity = a.Quantity,
                Reason = a.Reason,
                UserId = a.UserId,
                CreatedAt = a.CreatedAt
            }).ToList();
        }
    }
}

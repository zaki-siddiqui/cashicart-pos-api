using Cashicart.Common.DTOs;
using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Orders.Queries
{
    public class GetOrderQuery : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
    }

    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetOrderQueryHandler> _logger;

        public GetOrderQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrderQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching order with ID {OrderId}", request.OrderId);
            var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(request.OrderId);
            if (order == null) throw new NotFoundException($"Order with ID {request.OrderId} not found.");
            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}

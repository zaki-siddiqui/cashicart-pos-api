using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Orders.Commands
{
    public class UpdateOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating order with ID {OrderId}", request.OrderId);
            var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(request.OrderId);
            if (request.Items != null)
            {
                order.Items.Clear();
                var products = await _unitOfWork.GetRepository<Product>().GetAllAsync(); // Use new async method
                order.Items.AddRange(request.Items.Select(i =>
                {
                    var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                    if (product == null) throw new NotFoundException($"Product {i.ProductId} not found.");
                    return new OrderItem(order.OrderId, i.ProductId, i.Quantity, i.UnitPrice > 0 ? i.UnitPrice : product.Price);
                }).ToList());
                order.RecalculateTotalAmount();
            }
            await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
            await _unitOfWork.CommitAsync();
        }
    }
}
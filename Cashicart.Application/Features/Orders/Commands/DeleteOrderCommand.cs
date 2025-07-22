using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Orders.Commands
{
    public class DeleteOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
    }

    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order with ID {OrderId}", request.OrderId);
            var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(request.OrderId);
            _unitOfWork.GetRepository<Order>().Remove(order);
            await _unitOfWork.CommitAsync();
        }
    }
}

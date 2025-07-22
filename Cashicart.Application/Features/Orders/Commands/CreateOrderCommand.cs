using Cashicart.Common.Constants;
using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using Cashicart.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage(ErrorMessages.CategoryIdRequired);
            RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.")
                .Must(items => items.All(i => i.Quantity > 0)).WithMessage("Quantity must be greater than zero.");
        }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly IPaymentService _paymentService;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger,
            IValidator<CreateOrderCommand> validator, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);
            var products = await _unitOfWork.GetRepository<Product>().GetAll().ToListAsync();
            var orderItems = request.Items.Select(i =>
            {
                var product = products.FirstOrDefault(p => p.ProductId == i.ProductId);
                if (product == null) throw new NotFoundException($"Product {i.ProductId} not found.");
                if (product.StockQuantity < i.Quantity) throw new DomainException("Insufficient stock.");
                return new OrderItem(Guid.NewGuid(), i.ProductId, i.Quantity, product.Price);
            }).ToList();
            decimal totalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice);

            //var paymentIntentId = await _paymentService.ProcessPayment(totalAmount, "pm_card_visa"); // Replace with actual payment method

            var order = new Order(request.CustomerId, totalAmount, orderItems, OrderStatus.Pending);
            await _unitOfWork.GetRepository<Order>().AddAsync(order);

            var auditLog = new AuditLog(Guid.Empty, "CreateOrder", nameof(Order), order.OrderId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.CommitAsync();

            return order.OrderId;
        }
    }
}

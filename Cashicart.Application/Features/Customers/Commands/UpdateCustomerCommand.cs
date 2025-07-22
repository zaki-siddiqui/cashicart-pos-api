using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommand : IRequest
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;

        public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating customer with ID {CustomerId}", request.CustomerId);
            var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(request.CustomerId);
            customer.UpdateDetails(request.Name, request.Email, request.Phone);
            await _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);

            var auditLog = new AuditLog(Guid.Empty, "UpdateCustomer", nameof(Customer), customer.CustomerId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.CommitAsync();
        }
    }
}

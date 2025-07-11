using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating customer with email {Email}", request.Email);
            var customer = new Customer(request.Name, request.Email, request.Phone);
            await _unitOfWork.GetRepository<Customer>().AddAsync(customer);

            var auditLog = new AuditLog(Guid.Empty, "CreateCustomer", nameof(Customer), customer.CustomerId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.CommitAsync();

            return customer.CustomerId;
        }
    }
}

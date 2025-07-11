using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Features.Customers.Queries
{
    public class GetCustomerQuery : IRequest<CustomerDto>
    {
        public Guid CustomerId { get; set; }
    }

    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCustomerQueryHandler> _logger;

        public GetCustomerQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCustomerQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching customer with ID {CustomerId}", request.CustomerId);
            var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(request.CustomerId);
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }
    }
}

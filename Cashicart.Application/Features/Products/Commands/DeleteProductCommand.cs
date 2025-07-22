using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public Guid ProductId { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting DeleteProduct for ID: {ProductId}", request.ProductId);

            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion, ID: {ProductId}", request.ProductId);
                throw new DomainException("Product not found.");
            }

            _unitOfWork.GetRepository<Product>().Remove(product);
            await _unitOfWork.CommitAsync();

            var auditLog = new AuditLog(Guid.Empty, "DeleteProduct", nameof(Product), product.ProductId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Product soft-deleted with ID: {ProductId}", request.ProductId);
        }
    }
}

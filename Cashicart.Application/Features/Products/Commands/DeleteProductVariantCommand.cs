using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Products.Commands
{
    public class DeleteProductVariantCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
    }

    public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductVariantCommandHandler> _logger;

        public DeleteProductVariantCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductVariantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete variant {VariantId} from product {ProductId}", request.VariantId, request.ProductId);

            var product = await _unitOfWork.GetRepository<Product>()
                .GetAll()
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", request.ProductId);
                throw new DomainException("Product not found.");
            }

            var variantRepo = _unitOfWork.GetRepository<ProductVariant>();
            var variant = product.Variants.FirstOrDefault(v => v.VariantId == request.VariantId);
            if (variant == null)
            {
                _logger.LogWarning("Variant not found: {VariantId}", request.VariantId);
                throw new DomainException("Variant not found.");
            }

            await variantRepo.Remove(variant);

            _logger.LogInformation("Variant {VariantId} deleted", request.VariantId);

            await _unitOfWork.CommitAsync();

        }
    }
}

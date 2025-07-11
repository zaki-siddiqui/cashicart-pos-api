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
            _logger.LogInformation("Deleting variant {VariantId} for product {ProductId}", request.VariantId, request.ProductId);

            var variantRepo = _unitOfWork.GetRepository<ProductVariant>();
            var variant = await variantRepo.GetAll()
                .FirstOrDefaultAsync(v => v.VariantId == request.VariantId && v.ProductId == request.ProductId, cancellationToken);

            if (variant == null)
                throw new Exception("Variant not found.");

            await variantRepo.Remove(variant);
            await _unitOfWork.CommitAsync();
        }
    }
}

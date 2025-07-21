using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
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
    public class UpdateProductVariantCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
        public string SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductVariantCommandHandler> _logger;

        public UpdateProductVariantCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductVariantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating variant {VariantId} for product {ProductId}", request.VariantId, request.ProductId);

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
            var variant = await variantRepo.GetAll()
                .FirstOrDefaultAsync(v => v.VariantId == request.VariantId && v.ProductId == request.ProductId, cancellationToken);

            if (variant == null)
            {
                _logger.LogWarning("Variant not found: {VariantId}", request.VariantId);
                throw new DomainException("Variant not found.");
            }

            // Validation
            if (string.IsNullOrWhiteSpace(request.SKU))
                throw new DomainException("SKU is required.");

            if (request.Price <= 0)
                throw new DomainException("Price must be greater than 0.");

            if (request.StockQuantity < 0)
                throw new DomainException("Stock quantity cannot be negative.");

            var isDuplicateSKU = await variantRepo.GetAll()
                .AnyAsync(v => v.SKU == request.SKU && v.VariantId != request.VariantId, cancellationToken);

            if (isDuplicateSKU)
                throw new DomainException("SKU already exists for another variant.");

            // Update properties
            variant.GetType().GetProperty("SKU")?.SetValue(variant, request.SKU);
            variant.GetType().GetProperty("Size")?.SetValue(variant, request.Size);
            variant.GetType().GetProperty("Color")?.SetValue(variant, request.Color);
            variant.UpdatePrice(request.Price);
            variant.UpdateStock(request.StockQuantity - variant.StockQuantity); // adjust relative

            await variantRepo.UpdateAsync(variant);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Variant {VariantId} updated successfully", request.VariantId);
        }
    }
}

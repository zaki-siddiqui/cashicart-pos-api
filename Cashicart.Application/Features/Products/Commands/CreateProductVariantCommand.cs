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
    public class CreateProductVariantCommand : IRequest<Guid>
    {
        public Guid ProductId { get; set; }
        public string SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductVariantCommandHandler> _logger;

        public CreateProductVariantCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductVariantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating variant with SKU {SKU} for product {ProductId}", request.SKU, request.ProductId);

            var product = await _unitOfWork.GetRepository<Product>().GetAll()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && !p.IsDeleted, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product not found or deleted: {ProductId}", request.ProductId);
                throw new DomainException("Product not found or deleted.");
            }

            if (product.Category == null || product.Category.IsDeleted)
            {
                _logger.LogWarning("Invalid or deleted category for Product ID {ProductId}", request.ProductId);
                throw new DomainException("Associated category is missing or deleted.");
            }

            // Validation
            if (string.IsNullOrWhiteSpace(request.SKU))
                throw new DomainException("SKU is required.");

            if (request.Price <= 0)
                throw new DomainException("Price must be greater than 0.");

            if (request.StockQuantity < 0)
                throw new DomainException("Stock quantity cannot be negative.");

            // Enforce unique SKU
            var existing = await _unitOfWork.GetRepository<ProductVariant>().GetAll()
                .AnyAsync(v => v.SKU == request.SKU, cancellationToken);
            if (existing)
            {
                _logger.LogWarning("Duplicate SKU '{SKU}' for product {ProductId}", request.SKU, request.ProductId);
                throw new DomainException("SKU already exists for another variant.");
            }

            var variant = new ProductVariant(
                request.ProductId,
                request.SKU,
                request.Price,
                request.StockQuantity,
                request.Size,
                request.Color
            );

            await _unitOfWork.GetRepository<ProductVariant>().AddAsync(variant);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Variant created with SKU {SKU}", request.SKU);

            return variant.VariantId;
        }


        //public async Task<Guid> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Creating variant for product {ProductId}", request.ProductId);

        //    var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(request.ProductId);
        //    if (product == null) throw new Exception("Product not found");

        //    var variant = new ProductVariant(
        //        request.ProductId,
        //        request.SKU,
        //        request.Price,
        //        request.StockQuantity,
        //        request.Size,
        //        request.Color
        //    );

        //    await _unitOfWork.GetRepository<ProductVariant>().AddAsync(variant);
        //    await _unitOfWork.CommitAsync();

        //    return variant.VariantId;
        //}
    }
}

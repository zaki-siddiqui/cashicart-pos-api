using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Cashicart.Domain.Exceptions;
using Cashicart.Common.DTOs;

namespace Cashicart.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public List<ProductTranslationDto> Translations { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting CreateProduct for SKU: {SKU}", request.SKU);

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("Invalid category ID {CategoryId} for product creation", request.CategoryId);
                throw new DomainException("Invalid or deleted category.");
            }


            var product = new Product(
                request.Name,
                request.SKU,
                request.Price,
                request.StockQuantity,
                request.CategoryId,
                request.Description
            );

            // Add translations
            foreach (var t in request.Translations)
            {
                _logger.LogInformation("Adding translation for lang: {Lang}, name: {Name}", t.Language, t.Name);
                product.Translations.Add(new ProductTranslation(product.ProductId, t.Language, t.Name, t.Description));
            }

            // Add variants
            foreach (var v in request.Variants)
            {
                if (string.IsNullOrWhiteSpace(v.SKU)) throw new DomainException("Variant SKU is required.");
                if (v.Price <= 0) throw new DomainException("Variant price must be greater than zero.");
                if (v.StockQuantity < 0) throw new DomainException("Variant stock cannot be negative.");

                product.Variants.Add(new ProductVariant(product.ProductId, v.SKU, v.Price, v.StockQuantity, v.Size, v.Color));
            }

            await _unitOfWork.GetRepository<Product>().AddAsync(product);

            var auditLog = new AuditLog(Guid.Empty, "CreateProduct", nameof(Product), product.ProductId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Product created successfully with ID: {ProductId}", product.ProductId);

            return product.ProductId;
        }
    }
}
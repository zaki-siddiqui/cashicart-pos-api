using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Common.Options;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Cashicart.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public List<ProductTranslationDto> Translations { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly LocalizationOptions _localizationOptions;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger, IOptions<LocalizationOptions> localizationOptions)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizationOptions = localizationOptions.Value;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating product {ProductId}", request.ProductId);

            var productRepo = _unitOfWork.GetRepository<Product>();

            // Load product WITH translations
            var product = await productRepo.GetAll()
                .Include(p => p.Translations)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product not found {ProductId}", request.ProductId);
                throw new DomainException("Product not found.");
            }

            // Validate category
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
                throw new DomainException("Invalid or deleted category.");

            // Update main product
            product.Update(request.Name, request.SKU, request.Price, request.StockQuantity, request.CategoryId, request.Description);

            await productRepo.UpdateAsync(product);

            var requestedLanguages = request.Translations
               .Select(t => t.Language.Trim().ToLower())
               .ToList();

            var toRemove = product.Translations
                .Where(x => !requestedLanguages.Contains(x.Language))
                .ToList();

            foreach (var removeItem in toRemove)
            {
                //await _unitOfWork.GetRepository<ProductTranslation>().Remove(removeItem);
                _logger.LogInformation("Removed translation for {Lang}", removeItem.Language);
                product.Translations.Remove(removeItem);
            }

            // Handle translations
            foreach (var t in request.Translations)
            {
                var lang = t.Language.Trim().ToLower();

                if (!_localizationOptions.SupportedLanguages.Contains(lang))
                    throw new DomainException($"Language '{lang}' is not supported.");

                var existing = product.Translations.FirstOrDefault(x => x.Language == lang);

                if (existing != null)
                {
                    // EF is tracking this, so just modify fields
                    existing.UpdateTranslation(t.Name, t.Description);
                    _logger.LogInformation("Updated translation for {Lang}", lang);
                }
                else
                {
                    //// Use DbContext to explicitly add new entity
                    //await _unitOfWork.GetRepository<ProductTranslation>()
                    //    .AddAsync(new ProductTranslation(product.ProductId, lang, t.Name, t.Description));

                    var newTranslation = new ProductTranslation(product.ProductId, lang, t.Name, t.Description);
                    await _unitOfWork.GetRepository<ProductTranslation>().AddAsync(newTranslation);
                    //// product.Translations.Add(newTranslation);
                    _logger.LogInformation("Added new translation for {Lang}", lang);
                }
            }

            // Handle Variants
            var requestedVariantIds = request.Variants.Where(v => v.VariantId.HasValue).Select(v => v.VariantId.Value).ToList();

            // Remove variants not in request
            var variantsToRemove = product.Variants.Where(v => !requestedVariantIds.Contains(v.VariantId)).ToList();
            foreach (var remove in variantsToRemove)
            {
                _logger.LogInformation("Removing variant {VariantId}", remove.VariantId);
                product.Variants.Remove(remove);
            }

            foreach (var v in request.Variants)
            {
                if (string.IsNullOrWhiteSpace(v.SKU)) throw new DomainException("Variant SKU is required.");
                if (v.Price <= 0) throw new DomainException("Variant price must be greater than zero.");
                if (v.StockQuantity < 0) throw new DomainException("Variant stock cannot be negative.");

                if (v.VariantId.HasValue)
                {
                    var existingVariant = product.Variants.FirstOrDefault(ev => ev.VariantId == v.VariantId.Value);
                    if (existingVariant != null)
                    {
                        existingVariant.UpdateVariant(v.SKU, v.Price, v.StockQuantity, v.Size, v.Color);
                        _logger.LogInformation("Updated variant {VariantId}", existingVariant.VariantId);
                    }
                }
                else
                {
                    ////product.Variants.Add(new ProductVariant(product.ProductId, v.SKU, v.Price, v.StockQuantity, v.Size, v.Color));
                    await _unitOfWork.GetRepository<ProductVariant>().AddAsync(new ProductVariant(product.ProductId, v.SKU, v.Price, v.StockQuantity, v.Size, v.Color));
                    _logger.LogInformation("Added new variant {SKU}", v.SKU);
                }
            }

            // Do NOT call UpdateAsync() on product or translations
            var auditLog = new AuditLog(Guid.Empty, "UpdateProduct", nameof(Product), product.ProductId);
                await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);

                try
                {
                    await _unitOfWork.CommitAsync();
                    _logger.LogInformation("Product {ProductId} updated successfully", request.ProductId);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency conflict while updating product {ProductId}", request.ProductId);
                    throw new DomainException("The product was updated or deleted by another process. Please refresh and try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while updating product {ProductId}", request.ProductId);
                    throw new DomainException("An unexpected error occurred while updating the product.");
                }
        }
    }
}





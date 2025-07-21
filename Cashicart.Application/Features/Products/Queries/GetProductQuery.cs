using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Products.Queries
{
    public class GetProductQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public string? Language { get; set; } = "en";
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetProductQueryHandler> _logger;

        public GetProductQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching product with ID {ProductId}", request.ProductId);

            var product = await _unitOfWork.GetRepository<Product>().GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && !p.IsDeleted, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", request.ProductId);
                throw new Exception("Product not found.");
            }


            // Determine requested language
            string lang = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.ToLower();
            var translation = product.Translations.FirstOrDefault(t => t.Language == lang)
                           ?? product.Translations.FirstOrDefault(t => t.Language == "en"); // fallback

            if (translation == null)
            {
                _logger.LogWarning("No translation found for ProductId {ProductId} in lang {Lang}", request.ProductId, lang);
            }

            return new ProductDto
            {
                ProductId = product.ProductId,
                //Name = product.Name,
                Name = translation?.Name ?? product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                //Description = product.Description,
                Description = translation?.Description ?? product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrls = product.Images?.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).ToList() ?? new(),
                ThumbnailUrls = product.Images?.OrderByDescending(i => i.IsPrimary).Select(i => i.ThumbnailUrl).ToList() ?? new(),
                Variants = product.Variants?.Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    SKU = v.SKU,
                    Size = v.Size,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new()
            };
        }
    }
}

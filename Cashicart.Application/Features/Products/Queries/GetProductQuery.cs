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
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && !p.IsDeleted, cancellationToken);

            if (product == null)
                throw new Exception("Product not found");

            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Description = product.Description,
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

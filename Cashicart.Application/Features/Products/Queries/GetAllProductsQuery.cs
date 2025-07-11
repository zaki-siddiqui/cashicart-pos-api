using Cashicart.Common.DTOs;
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

namespace Cashicart.Application.Features.Products.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? NameFilter { get; set; }
        public Guid? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; } // e.g. "price", "name", "stock"
        public string? SortOrder { get; set; } = "asc"; // "asc" or "desc"
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching products with filters");

            var query = _unitOfWork.GetRepository<Product>()
                .GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(p => p.Name.Contains(request.NameFilter));

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            query = request.SortBy?.ToLower() switch
            {
                "price" => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "name" => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "stock" => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.StockQuantity)
                    : query.OrderBy(p => p.StockQuantity),

                _ => query.OrderBy(p => p.Name)
            };

            var products = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                ImageUrls = p.Images?
                    .OrderByDescending(i => i.IsPrimary)
                    .Select(i => i.ImageUrl)
                    .ToList() ?? new List<string>(),

                ThumbnailUrls = p.Images?
                    .OrderByDescending(i => i.IsPrimary)
                    .Select(i => i.ThumbnailUrl)
                    .ToList() ?? new List<string>(),
                Variants = p.Variants?.Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    SKU = v.SKU,
                    Size = v.Size,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new()

                //ImageUrls = p.Images?
                //    .OrderByDescending(i => i.IsPrimary)
                //    .Select(i => i.ImageUrl)
                //    .ToList() ?? new List<string>()
            }).ToList();
        }

        //public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        //{
        //    //_logger.LogInformation("Fetching all products with filter {NameFilter}", request.NameFilter);

        //    _logger.LogInformation("Fetching all products with filters: Name={Name}, Category={CategoryId}", request.NameFilter, request.CategoryId);


        //    //var query = _unitOfWork.GetRepository<Product>().GetAll().Include(p => p.Category);
        //    var query = _unitOfWork.GetRepository<Product>().GetAll().Include(p => p.Category).AsQueryable();

        //    if (!string.IsNullOrEmpty(request.NameFilter))
        //        query = query.Where(p => p.Name.Contains(request.NameFilter));

        //    if (request.CategoryId.HasValue)
        //        query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        //    if (request.MinPrice.HasValue)
        //        query = query.Where(p => p.Price >= request.MinPrice.Value);

        //    if (request.MaxPrice.HasValue)
        //        query = query.Where(p => p.Price <= request.MaxPrice.Value);

        //    // Sorting
        //    query = request.SortBy?.ToLower() switch
        //    {
        //        "price" => request.SortOrder?.ToLower() == "desc"
        //                    ? query.OrderByDescending(p => p.Price)
        //                    : query.OrderBy(p => p.Price),

        //        "name" => request.SortOrder?.ToLower() == "desc"
        //                    ? query.OrderByDescending(p => p.Name)
        //                    : query.OrderBy(p => p.Name),

        //        "stock" => request.SortOrder?.ToLower() == "desc"
        //                    ? query.OrderByDescending(p => p.StockQuantity)
        //                    : query.OrderBy(p => p.StockQuantity),

        //        _ => query.OrderBy(p => p.Name) // Default
        //    };


        //    var products = await query
        //        .Where(p => !p.IsDeleted)
        //        .Skip((request.Page - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .ToListAsync(cancellationToken);

        //    return products.Select(p => new ProductDto
        //    {
        //        ProductId = p.ProductId,
        //        Name = p.Name,
        //        SKU = p.SKU,
        //        Price = p.Price,
        //        StockQuantity = p.StockQuantity,
        //        CategoryId = p.CategoryId,
        //        CategoryName = p.Category?.Name,
        //        ImageUrls = p.Images?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
        //    }).ToList();
        //}

    }
}
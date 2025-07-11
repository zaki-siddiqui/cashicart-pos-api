using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Updating product with ID {ProductId}", request.ProductId);

        //    var repo = _unitOfWork.GetRepository<Product>();
        //    var product = await repo.GetByIdAsync(request.ProductId);
        //    if (product == null) throw new Exception("Product not found");

        //    product.Update(request.Name, request.SKU, request.Price, request.StockQuantity, request.CategoryId);

        //    await repo.UpdateAsync(product);

        //    var auditLog = new AuditLog(Guid.Empty, "UpdateProduct", nameof(Product), product.ProductId);
        //    await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);

        //    await _unitOfWork.CommitAsync();
        //}

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating product with ID {ProductId}", request.ProductId);

            var productRepo = _unitOfWork.GetRepository<Product>();
            var categoryRepo = _unitOfWork.GetRepository<Category>();

            var product = await productRepo.GetByIdAsync(request.ProductId);
            if (product == null) throw new Exception("Product not found");

            var category = await categoryRepo.GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
                throw new Exception($"Category '{request.CategoryId}' does not exist or has been deleted.");
            //throw new Exception("Invalid or deleted category.");

            product.Update(request.Name, request.SKU, request.Price, request.StockQuantity, request.CategoryId, request.Description);

            await productRepo.UpdateAsync(product);

            var auditLog = new AuditLog(Guid.Empty, "UpdateProduct", nameof(Product), product.ProductId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.CommitAsync();
        }

        //public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Updating product with ID {ProductId}", request.ProductId);
        //    var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(request.ProductId);
        //    if (request.Price.HasValue) product.UpdatePrice(request.Price.Value);
        //    if (request.StockQuantity.HasValue) product.UpdateStock(request.StockQuantity.Value);
        //    await _unitOfWork.GetRepository<Product>().UpdateAsync(product);

        //    var auditLog = new AuditLog(Guid.Empty, "UpdateProduct", nameof(Product), product.ProductId);
        //    await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
        //    await _unitOfWork.CommitAsync();
        //}
    }
}

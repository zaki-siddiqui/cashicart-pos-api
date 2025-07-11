using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using FluentValidation;

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
            _logger.LogInformation("Creating product: {Name}", request.Name);

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
                throw new Exception($"Category '{request.CategoryId}' does not exist or has been deleted.");
            //throw new Exception("Invalid or deleted category.");

            var product = new Product(
                request.Name,
                request.SKU,
                request.Price,
                request.StockQuantity,
                request.CategoryId,
                request.Description
            );

            await _unitOfWork.GetRepository<Product>().AddAsync(product);

            var auditLog = new AuditLog(Guid.Empty, "CreateProduct", nameof(Product), product.ProductId);
            await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);

            await _unitOfWork.CommitAsync();

            return product.ProductId;
        }

        //public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var product = new Product(request.Name, request.SKU, request.Price, request.StockQuantity, request.CategoryId);
        //        var repository = _unitOfWork.GetRepository<Product>();
        //        await repository.AddAsync(product);
        //        await _unitOfWork.CommitAsync();
        //        _logger.LogInformation("Product created with ID {ProductId}", product.ProductId);
        //        return product.ProductId;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to create product with command {@Command}", request);
        //        throw; // Re-throw to trigger the 500 in the controller
        //    }
        //}
    }
}
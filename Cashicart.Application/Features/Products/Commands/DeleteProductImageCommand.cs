//using Cashicart.Common.Interfaces;
//using Cashicart.Domain.Entities;
//using MediatR;
//using Microsoft.AspNetCore.Hosting;

//namespace Cashicart.Application.Features.Products.Commands
//{
//    public class DeleteProductImageCommand : IRequest
//    {
//        public Guid ProductId { get; set; }
//        public Guid ImageId { get; set; }
//    }

//    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IWebHostEnvironment _env;

//        public DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env)
//        {
//            _unitOfWork = unitOfWork;
//            _env = env;
//        }

//        public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
//        {
//            var repo = _unitOfWork.GetRepository<ProductImage>();
//            var image = await repo.GetByIdAsync(request.ImageId);
//            if (image == null || image.ProductId != request.ProductId)
//                throw new Exception("Image not found or does not belong to product.");

//            // Delete file from disk
//            var fullPath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
//            if (File.Exists(fullPath))
//                File.Delete(fullPath);

//            await repo.Remove(image);
//            await _unitOfWork.CommitAsync();
//        }
//    }
//}

using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Products.Commands
{
    public class DeleteProductImageCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid ImageId { get; set; }
    }

    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteProductImageCommandHandler> _logger;

        public DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env, ILogger<DeleteProductImageCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _logger = logger;
        }

        public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting image {ImageId} from product {ProductId}", request.ImageId, request.ProductId);

            var repo = _unitOfWork.GetRepository<ProductImage>();

            var image = await repo.GetByIdAsync(request.ImageId);
            if (image == null || image.ProductId != request.ProductId)
            {
                _logger.LogWarning("Image not found with image {imageId}", request.ImageId);
                throw new DomainException("Image not found or does not belong to the product.");
            }
                

            // Prevent deleting last image
            var totalImages = await repo.GetAll()
                .CountAsync(i => i.ProductId == request.ProductId, cancellationToken);

            if (totalImages <= 1)
            {
                _logger.LogWarning("Cannot delete the only image for product {ProductId}", request.ProductId);
                throw new DomainException("At least one image is required per product. You cannot delete the last image.");
            }

            bool isPrimary = image.IsPrimary;

            // Delete main image file
            var imagePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            // Delete thumbnail file
            if (!string.IsNullOrWhiteSpace(image.ThumbnailUrl))
            {
                var thumbPath = Path.Combine(_env.WebRootPath, image.ThumbnailUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(thumbPath))
                    File.Delete(thumbPath);
            }

            await repo.Remove(image);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Image {ImageId} deleted successfully");

            // Auto-promote another image if deleted one was primary
            if (isPrimary)
            {
                var otherImages = await repo.GetAll()
                    .Where(i => i.ProductId == request.ProductId)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync(cancellationToken);

                var newPrimary = otherImages.FirstOrDefault();
                if (newPrimary != null)
                {
                    newPrimary.SetPrimary();
                    await repo.UpdateAsync(newPrimary);
                    await _unitOfWork.CommitAsync();
                }
            }
        }
    }
}


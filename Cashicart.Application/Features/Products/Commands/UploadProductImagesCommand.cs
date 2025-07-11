using Cashicart.Common.Interfaces;
using Cashicart.Common.Options;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Cashicart.Application.Features.Products.Commands
{
    public class UploadProductImagesCommand : IRequest<List<string>>
    {
        public Guid ProductId { get; set; }
        public List<IFormFile> Files { get; set; }
    }

    public class UploadProductImagesCommandHandler : IRequestHandler<UploadProductImagesCommand, List<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UploadProductImagesCommandHandler> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly ImageUploadOptions _imageOptions;

        public UploadProductImagesCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UploadProductImagesCommandHandler> logger,
            IWebHostEnvironment env,
            IOptions<ImageUploadOptions> imageOptions)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _env = env;
            _imageOptions = imageOptions.Value;
        }

        public async Task<List<string>> Handle(UploadProductImagesCommand request, CancellationToken cancellationToken)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var productRepo = _unitOfWork.GetRepository<Product>();
            var imageRepo = _unitOfWork.GetRepository<ProductImage>();

            var product = await productRepo.GetAll()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);

            if (product == null)
                throw new Exception("Product not found");

            if (request.Files == null || !request.Files.Any())
                throw new Exception("No files uploaded.");

            // Validate file count
            if (request.Files.Count + product.Images.Count > 5)
                throw new Exception($"You can upload up to 5 images per product. You already have {product.Images.Count}.");

            // Validate all files before saving
            var invalidFiles = new List<string>();

            foreach (var file in request.Files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    invalidFiles.Add($"'{file.FileName}' has unsupported extension '{ext}'");
                    continue;
                }

                if (file.Length > _imageOptions.MaxImageSizeBytes)
                {
                    invalidFiles.Add($"'{file.FileName}' exceeds {_imageOptions.MaxImageSizeBytes / 1024 / 1024}MB size limit");
                }
            }

            if (invalidFiles.Any())
                throw new Exception("Upload failed. Issues found with the following files:\n" + string.Join("\n", invalidFiles));

            // Validate total size
            long existingSize = product.Images
                .Select(i => Path.Combine(_env.WebRootPath ?? "wwwroot", i.ImageUrl.TrimStart('/')))
                .Where(File.Exists)
                .Sum(path => new FileInfo(path).Length);

            long newUploadSize = request.Files.Sum(f => f.Length);
            if (existingSize + newUploadSize > _imageOptions.MaxTotalSizeBytes)
                throw new Exception($"Total image size limit exceeded. Max allowed: {_imageOptions.MaxTotalSizeBytes / 1024 / 1024}MB.");

            var uploadedUrls = new List<string>();

            foreach (var file in request.Files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsPath, fileName);
                var thumbnailPath = Path.Combine(uploadsPath, $"thumb-{fileName}");

                using var image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken);

                // Resize
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(_imageOptions.ResizeMaxWidth, _imageOptions.ResizeMaxHeight)
                }));
                await image.SaveAsync(filePath, new JpegEncoder(), cancellationToken);

                // Thumbnail
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Crop,
                    Size = new Size(_imageOptions.ThumbnailSize, _imageOptions.ThumbnailSize)
                }));
                await image.SaveAsync(thumbnailPath, new JpegEncoder(), cancellationToken);

                var imageUrl = $"/uploads/products/{fileName}";
                var thumbUrl = $"/uploads/products/thumb-{fileName}";

                var record = new ProductImage(request.ProductId, imageUrl);
                record.SetThumbnail(thumbUrl);

                await imageRepo.AddAsync(record);
                uploadedUrls.Add(imageUrl);
            }

            await _unitOfWork.CommitAsync();
            return uploadedUrls;
        }
    }
}

using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Products.Commands
{
    public class SetPrimaryProductImageCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid ImageId { get; set; }
    }

    public class SetPrimaryProductImageCommandHandler : IRequestHandler<SetPrimaryProductImageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetPrimaryProductImageCommandHandler> _logger;

        public SetPrimaryProductImageCommandHandler(IUnitOfWork unitOfWork, ILogger<SetPrimaryProductImageCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting primary image {ImageId} for product {ProductId}", request.ImageId, request.ProductId);

            var repo = _unitOfWork.GetRepository<ProductImage>();
            var images = await repo.GetAll().Where(img => img.ProductId == request.ProductId).ToListAsync();

            if (images == null)
            {
                _logger.LogWarning("Image not found: {ImageId}", request.ImageId);
                throw new DomainException("Image not found.");
            }

            foreach (var img in images)
            {
                if (img.ProductImageId == request.ImageId)
                    img.SetPrimary();
                else
                    img.UnsetPrimary();

                await repo.UpdateAsync(img);
            }

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Image {ImageId} set as primary for product {ProductId}", request.ImageId, request.ProductId);
        }
    }
}

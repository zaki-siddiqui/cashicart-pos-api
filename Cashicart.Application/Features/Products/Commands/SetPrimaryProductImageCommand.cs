using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SetPrimaryProductImageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ProductImage>();
            var images = await repo.GetAll().Where(img => img.ProductId == request.ProductId).ToListAsync();

            foreach (var img in images)
            {
                if (img.ProductImageId == request.ImageId)
                    img.SetPrimary();
                else
                    img.UnsetPrimary();

                await repo.UpdateAsync(img);
            }

            await _unitOfWork.CommitAsync();
        }
    }
}

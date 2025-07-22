using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest
    {
        public Guid CategoryId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var category = await repo.GetByIdAsync(request.CategoryId);
            if (category == null) throw new NotFoundException("Category not found");

            category.SoftDelete();
            await repo.UpdateAsync(category);
            await _unitOfWork.CommitAsync();
        }
    }
}

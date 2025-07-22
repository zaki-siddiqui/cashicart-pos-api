using Cashicart.Common.Exceptions;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var category = await repo.GetByIdAsync(request.CategoryId);
            if (category == null) throw new NotFoundException("Category not found");

            category.UpdateName(request.Name);
            await repo.UpdateAsync(category);
            await _unitOfWork.CommitAsync();
        }
    }
}

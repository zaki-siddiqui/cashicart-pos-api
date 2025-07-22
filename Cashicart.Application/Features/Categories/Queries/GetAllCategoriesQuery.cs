using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAll()
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}

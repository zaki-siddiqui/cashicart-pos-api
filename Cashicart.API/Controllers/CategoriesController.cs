using Asp.Versioning;
using Cashicart.Application.Features.Categories.Commands;
using Cashicart.Application.Features.Categories.Queries;
using Cashicart.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashicart.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Manager")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, ApiResponse<Guid>.SuccessResponse(id, "Category created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            command.CategoryId = id;
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Category updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCategoryCommand { CategoryId = id });
            return Ok(ApiResponse<string>.SuccessResponse(null, "Category deleted successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(ApiResponse<object>.SuccessResponse(result, "Categories fetched successfully."));
        }
    }
}

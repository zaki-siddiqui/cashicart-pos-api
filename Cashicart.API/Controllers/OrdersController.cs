using Cashicart.Application.Features.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Asp.Versioning;
using Cashicart.Common.Responses;
using Cashicart.Application.Features.Orders.Queries; // Added to use ValidationException

namespace Cashicart.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Manager")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var orderId = await _mediator.Send(command);
            return Ok(ApiResponse<Guid>.SuccessResponse(orderId, "Order created successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _mediator.Send(new GetOrderQuery { OrderId = id });
            return Ok(ApiResponse<object>.SuccessResponse(order, "Order fetched successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderCommand command)
        {
            command.OrderId = id;
            await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Order updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            await _mediator.Send(new DeleteOrderCommand { OrderId = id });
            return Ok(ApiResponse<object>.SuccessResponse(null, "Order deleted successfully."));
        }
    }
}
using Cashicart.Application.Features.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Asp.Versioning; // Added to use ValidationException

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
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                var orderId = await _mediator.Send(command);
                _logger.LogInformation("Order created with ID {OrderId}", orderId);
                return CreatedAtAction(nameof(GetOrder), new { id = orderId }, new { OrderId = orderId });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for order creation");
                return BadRequest(new { Errors = ex.Errors.Select(e => e.ErrorMessage) }); // Corrected
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred while creating the order.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            try
            {
                // Placeholder: Implement GetOrderQuery
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while fetching the order.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderCommand command)
        {
            try
            {
                command.OrderId = id;
                await _mediator.Send(command);
                _logger.LogInformation("Order updated with ID {OrderId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while updating the order.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteOrderCommand { OrderId = id });
                _logger.LogInformation("Order deleted with ID {OrderId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while deleting the order.");
            }
        }
    }
}
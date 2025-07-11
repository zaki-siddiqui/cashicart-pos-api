using Asp.Versioning;
using Cashicart.Application.Features.Inventory.Commands;
using Cashicart.Application.Features.Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashicart.API.Controllers;

//[Route("api/[controller]")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryCommand command)
    {
        try
        {
            var adjustmentId = await _mediator.Send(command);
            _logger.LogInformation("Inventory adjustment created with ID {InventoryAdjustmentId}", adjustmentId);
            return CreatedAtAction(nameof(GetAdjustment), new { id = adjustmentId }, new { InventoryAdjustmentId = adjustmentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting inventory");
            return StatusCode(500, "An error occurred while adjusting inventory.");
        }
    }

    [HttpGet("adjustments/{id}")]
    //[HttpGet("{id}")]
    public async Task<IActionResult> GetAdjustment(Guid id)
    {
        try
        {
            var adjustment = await _mediator.Send(new GetInventoryAdjustmentQuery { InventoryAdjustmentId = id });
            return Ok(adjustment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching inventory adjustment with ID {InventoryAdjustmentId}", id);
            return StatusCode(500, "An error occurred while fetching the inventory adjustment.");
        }
    }


    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAllInventoryAdjustments()
    {
        try
        {
            var adjustments = await _mediator.Send(new GetAllInventoryAdjustmentsQuery());
            return Ok(adjustments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all inventory adjustments");
            return StatusCode(500, "An error occurred while fetching inventory adjustments.");
        }
    }

}
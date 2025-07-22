using Asp.Versioning;
using Cashicart.Application.Features.Inventory.Commands;
using Cashicart.Application.Features.Inventory.Queries;
using Cashicart.Common.Responses;
using MediatR;
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

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse(null, "Inventory adjusted successfully."));
    }



    [HttpGet("adjustment/{adjustmentId}")]
    public async Task<IActionResult> GetAdjustment(Guid adjustmentId)
    {
        var result = await _mediator.Send(new GetInventoryAdjustmentQuery { InventoryAdjustmentId = adjustmentId });
        return Ok(ApiResponse<object>.SuccessResponse(result, "Inventory adjustment fetched."));
    }

    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAllInventoryAdjustments()
    {
        var adjustments = await _mediator.Send(new GetAllInventoryAdjustmentsQuery());
        return Ok(ApiResponse<object>.SuccessResponse(adjustments, "Successfully fetched Inventory adjustment."));
    }

}
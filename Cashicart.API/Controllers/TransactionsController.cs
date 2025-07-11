using Asp.Versioning;
using Cashicart.Application.Features.Transactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashicart.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundTransaction([FromBody] RefundTransactionCommand command)
    {
        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Transaction {TransactionId} refunded", command.TransactionId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding transaction");
            return StatusCode(500, "An error occurred while refunding the transaction.");
        }
    }
}
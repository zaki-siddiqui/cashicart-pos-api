using Asp.Versioning;
using Cashicart.Application.Features.Transactions.Commands;
using Cashicart.Application.Features.Transactions.Queries;
using Cashicart.Common.Responses;
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

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundTransaction([FromBody] RefundTransactionCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse(null, "Transaction refunded successfully."));
    }

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetTransaction(Guid id)
    //{
    //    var result = await _mediator.Send(new GetTransactionQuery { TransactionId = id });
    //    return Ok(ApiResponse<object>.SuccessResponse(result, "Transaction fetched successfully."));
    //}

    //[HttpPost("create")]
    //public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
    //{
    //    var transactionId = await _mediator.Send(command);
    //    return Ok(ApiResponse<Guid>.SuccessResponse(transactionId, "Transaction created successfully."));
    //}
}
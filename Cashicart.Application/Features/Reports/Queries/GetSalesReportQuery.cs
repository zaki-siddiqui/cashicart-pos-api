using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cashicart.Application.Features.Reports.Queries;

public class GetSalesReportQuery : IRequest<List<SalesReportDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, List<SalesReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSalesReportQueryHandler> _logger;

    public GetSalesReportQueryHandler(IUnitOfWork unitOfWork, ILogger<GetSalesReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching sales report for period {StartDate} to {EndDate}", request.StartDate, request.EndDate);
        var transactions = await _unitOfWork.GetRepository<Transaction>().GetAll()
            .Where(t => (!request.StartDate.HasValue || t.CreatedAt >= request.StartDate) &&
                       (!request.EndDate.HasValue || t.CreatedAt <= request.EndDate))
            .ToListAsync();

        var report = transactions.Select(t => new SalesReportDto
        {
            TransactionId = t.TransactionId,
            TotalAmount = t.TotalAmount,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt
        }).ToList();

        return report;
    }
}
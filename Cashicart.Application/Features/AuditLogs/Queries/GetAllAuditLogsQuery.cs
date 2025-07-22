using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Cashicart.Application.Features.AuditLogs.Queries
{
    public class GetAllAuditLogsQuery : IRequest<List<AuditLogDto>>
    {
    }

    public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllAuditLogsQueryHandler> _logger;

        public GetAllAuditLogsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllAuditLogsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<AuditLogDto>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all audit logs");
            var logs = await _unitOfWork.GetRepository<AuditLog>().GetAll().ToListAsync();
            return logs.Select(log => new AuditLogDto
            {
                AuditLogId = log.AuditLogId,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                CreatedAt = log.CreatedAt
            }).ToList();
        }
    }
}

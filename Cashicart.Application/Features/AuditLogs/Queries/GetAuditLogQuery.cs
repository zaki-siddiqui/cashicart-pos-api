using Cashicart.Common.DTOs;
using Cashicart.Common.Interfaces;
using Cashicart.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Application.Features.AuditLogs.Queries
{
    public class GetAuditLogQuery : IRequest<AuditLogDto>
    {
        public Guid AuditLogId { get; set; }
    }

    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, AuditLogDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAuditLogQueryHandler> _logger;

        public GetAuditLogQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAuditLogQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuditLogDto> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching audit log with ID {AuditLogId}", request.AuditLogId);
            var log = await _unitOfWork.GetRepository<AuditLog>().GetByIdAsync(request.AuditLogId);
            return new AuditLogDto
            {
                AuditLogId = log.AuditLogId,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                CreatedAt = log.CreatedAt
            };
        }
    }
}

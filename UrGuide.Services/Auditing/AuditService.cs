using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Auditing
{
    /// <summary>
    /// Enhanced audit logging service for structured audit events
    /// </summary>
    public class AuditService : IAuditService
    {
        private const string SystemUserId = "system";
        private const int MaxUserIdLength = 600;
        private const int MaxReferenceIdLength = 500;
        private const int MaxDetailsLength = 4000;
        private const int MaxCategoryLength = 100;
        private const int MaxIpAddressLength = 45;
        private const int MaxUserAgentLength = 500;

        private readonly UrGuideContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(UrGuideContext context, ILogger<AuditService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task LogAsync(
            EventCodes eventCode,
            string userId,
            string referenceId = null,
            string details = null,
            string category = null,
            AuditSeverity severity = AuditSeverity.Info,
            string ipAddress = null,
            string userAgent = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var auditEvent = new AuditEvent
                {
                    UserId = Truncate(userId ?? SystemUserId, MaxUserIdLength),
                    EventCode = eventCode,
                    ReferenceId = Truncate(referenceId, MaxReferenceIdLength),
                    Details = Truncate(details, MaxDetailsLength),
                    Category = Truncate(category ?? GetDefaultCategory(eventCode), MaxCategoryLength),
                    Severity = severity,
                    IpAddress = Truncate(ipAddress, MaxIpAddressLength),
                    UserAgent = Truncate(userAgent, MaxUserAgentLength)
                };

                _context.AuditEvents.Add(auditEvent);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Audit [{Severity}] {EventCode} by {UserId}: {Details}",
                    auditEvent.Severity, auditEvent.EventCode, auditEvent.UserId, auditEvent.Details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit event {EventCode} for user {UserId}", eventCode, userId ?? SystemUserId);
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (value == null) return null;
            return value.Length > maxLength ? value[..(maxLength - 3)] + "..." : value;
        }

        private static string GetDefaultCategory(EventCodes eventCode)
        {
            var code = (int)eventCode;
            return code switch
            {
                >= 1000 and < 2000 => "Authentication",
                >= 2000 and < 3000 => "Account",
                >= 3000 and < 4000 => "Content",
                >= 4000 and < 5000 => "AccountManagement",
                >= 5000 and < 6000 => "Moderation",
                >= 6000 and < 7000 => "Financial",
                >= 7000 and < 8000 => "Settings",
                >= 10000 => "System",
                _ => "General"
            };
        }
    }
}

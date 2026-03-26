using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data.Entities.Event;

namespace UrGuide.Services.Contracts
{
    /// <summary>
    /// Service interface for structured audit logging
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log an audit event with enhanced details
        /// </summary>
        Task LogAsync(
            EventCodes eventCode,
            string userId,
            string referenceId = null,
            string details = null,
            string category = null,
            AuditSeverity severity = AuditSeverity.Info,
            string ipAddress = null,
            string userAgent = null,
            CancellationToken cancellationToken = default);
    }
}

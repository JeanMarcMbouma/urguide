using MediatR;
using UrGuide.Data.Entities.Event;

namespace UrGuide.Services.Auditing.Abstraction
{
    public abstract class BaseAuditCommand : IRequest
    {

        protected BaseAuditCommand(string userId, string? referenceId = null)
        {
            System.ArgumentException.ThrowIfNullOrEmpty(userId);
            UserId = userId;
            ReferenceId = referenceId;
        }

        public string UserId { get; }
        public string? ReferenceId { get; }
        public abstract EventCodes EventCode { get; }
    }
}

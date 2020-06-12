using MediatR;
using UrGuide.Data.Entities.Event;

namespace UrGuide.Services.Auditing.Abstraction
{
    public abstract class BaseAuditCommand : IRequest
    {
        public string UserId { get; set; }
        public string ReferenceId { get; set; }
        public abstract EventCodes EventCode { get; }
    }
}

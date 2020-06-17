using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class CatalogCreatedCommand : BaseAuditCommand
    {
        public CatalogCreatedCommand(string userId, string catalogId)
        {
            UserId = userId ?? throw new System.ArgumentNullException(nameof(userId));
            ReferenceId = catalogId ?? throw new System.ArgumentNullException(nameof(catalogId));
        }
        public override EventCodes EventCode => EventCodes.CreateCalalog;
    }
    class CatalogCreatedCommandHandler : BaseAuditEventCommandHandler<CatalogCreatedCommand>
    {
        public CatalogCreatedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

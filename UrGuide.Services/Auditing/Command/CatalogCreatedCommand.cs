using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class CatalogCreatedCommand(string userId, string catalogId) : BaseAuditCommand(userId, referenceId: catalogId)
    {
        public override EventCodes EventCode => EventCodes.CreateCalalog;
    }
    class CatalogCreatedCommandHandler(UrGuideContext context) : BaseAuditEventCommandHandler<CatalogCreatedCommand>(context)
    {
    }
}

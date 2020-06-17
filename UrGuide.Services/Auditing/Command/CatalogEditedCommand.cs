using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class CatalogEditedCommand : BaseAuditCommand
    {
        public CatalogEditedCommand(string userId, string catalogId)
        {
            UserId = userId ?? throw new System.ArgumentNullException(nameof(userId));
            ReferenceId = catalogId ?? throw new System.ArgumentNullException(nameof(catalogId));
        }
        public override EventCodes EventCode => EventCodes.EditCatalog;
    }
    class CatalogEditedCommandHandler : BaseAuditEventCommandHandler<CatalogEditedCommand>
    {
        public CatalogEditedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

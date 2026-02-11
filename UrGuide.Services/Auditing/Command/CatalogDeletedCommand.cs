using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class CatalogDeletedCommand : BaseAuditCommand
    {
        public CatalogDeletedCommand(string userId, string catalogId) : base(userId, referenceId: catalogId)
        {
            ArgumentException.ThrowIfNullOrEmpty(catalogId, nameof(catalogId));
        }
        public override EventCodes EventCode => EventCodes.DeleteCatalog;
    }
    class CatalogDeletedCommandHandler : BaseAuditEventCommandHandler<CatalogDeletedCommand>
    {
        public CatalogDeletedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

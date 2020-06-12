using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class UserDeleteAccountCommand : BaseAuditCommand
    {
        public UserDeleteAccountCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public override EventCodes EventCode => EventCodes.DeleteAccount;
    }

    class UserDeleteAccountCommandHandler : BaseAuditEventCommandHandler<UserDeleteAccountCommand>
    {
        public UserDeleteAccountCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

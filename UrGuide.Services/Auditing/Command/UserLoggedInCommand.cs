using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class UserLoggedInCommand : BaseAuditCommand
    {
        public UserLoggedInCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public override EventCodes EventCode => EventCodes.Login;
    }

    class UserLoggedInCommandHandler : BaseAuditEventCommandHandler<UserLoggedInCommand>
    {
        public UserLoggedInCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

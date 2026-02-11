using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class UserLoggedInCommand(string userId) : BaseAuditCommand(userId)
    {
        public override EventCodes EventCode => EventCodes.Login;
    }

    class UserLoggedInCommandHandler : BaseAuditEventCommandHandler<UserLoggedInCommand>
    {
        public UserLoggedInCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

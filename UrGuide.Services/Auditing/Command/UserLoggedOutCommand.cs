using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    public class UserLoggedOutCommand(string userId) : BaseAuditCommand(userId)
    {
        public override EventCodes EventCode => EventCodes.Logout;
    }



    class UserLoggedOutCommandHandler : BaseAuditEventCommandHandler<UserLoggedOutCommand>
    {
        public UserLoggedOutCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

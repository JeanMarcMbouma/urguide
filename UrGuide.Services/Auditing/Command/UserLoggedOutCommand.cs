using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    public class UserLoggedOutCommand : BaseAuditCommand
    {
        public UserLoggedOutCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public override EventCodes EventCode => EventCodes.Logout;
    }



    class UserLoggedOutCommandHandler : BaseAuditEventCommandHandler<UserLoggedOutCommand>
    {
        public UserLoggedOutCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}

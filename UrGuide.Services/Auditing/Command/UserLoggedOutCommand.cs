using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;

namespace UrGuide.Services.Auditing.Command
{
    public class UserLoggedOutCommand : IRequest
    {
        public UserLoggedOutCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
        public string UserId { get; }
    }



    class UserLoggedOutCommandHandler : IRequestHandler<UserLoggedOutCommand>
    {
        public UserLoggedOutCommandHandler(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UrGuideContext Context { get; }

        public Task<Unit> Handle(UserLoggedOutCommand request, CancellationToken cancellationToken)
        {
            Context.AuditEvents.Add(new Data.Entities.Event.AuditEvent
            {
                UserId = request.UserId,
                EventCode = Data.Entities.Event.EventCodes.Logout
            });
            return Unit.Task;
        }
    }
}

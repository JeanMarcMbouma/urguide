using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;

namespace UrGuide.Services.Auditing.Command
{
    class UserLoggedInCommand : IRequest
    {
        public UserLoggedInCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
        public string UserId { get; }
    }

    class UserLoggedInCommandHandler : IRequestHandler<UserLoggedInCommand>
    {
        public UserLoggedInCommandHandler(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UrGuideContext Context { get; }

        public Task<Unit> Handle(UserLoggedInCommand request, CancellationToken cancellationToken)
        {
            Context.AuditEvents.Add(new Data.Entities.Event.AuditEvent
            {
                UserId = request.UserId,
                EventCode = Data.Entities.Event.EventCodes.Login
            });
            return Unit.Task;
        }
    }
}

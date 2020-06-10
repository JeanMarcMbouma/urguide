using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;

namespace UrGuide.Services.Auditing.Command
{
    class UserDeleteAccountCommand : IRequest
    {
        public UserDeleteAccountCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
        public string UserId { get; }
    }

    class UserDeleteAccountCommandHandler : IRequestHandler<UserDeleteAccountCommand>
    {
        public UserDeleteAccountCommandHandler(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UrGuideContext Context { get; }

        public Task<Unit> Handle(UserDeleteAccountCommand request, CancellationToken cancellationToken)
        {
            Context.AuditEvents.Add(new Data.Entities.Event.AuditEvent
            {
                UserId = request.UserId,
                EventCode = Data.Entities.Event.EventCodes.DeleteAccount
            });
            return Unit.Task;
        }
    }
}

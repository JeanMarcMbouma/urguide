using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;

namespace UrGuide.Services.Auditing.Abstraction
{
    abstract class BaseAuditEventCommandHandler<T> : IRequestHandler<T> where T :  BaseAuditCommand, IRequest
    {
        protected UrGuideContext Context { get; }

        public BaseAuditEventCommandHandler(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual Task<Unit> HandleInternal(T request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(T request, CancellationToken cancellationToken)
        {
            Context.AuditEvents.Add(new Data.Entities.Event.AuditEvent
            {
                UserId = request.UserId,
                EventCode = request.EventCode,
                ReferenceId = request.ReferenceId
            });
            return Unit.Task;
        }
    }
}

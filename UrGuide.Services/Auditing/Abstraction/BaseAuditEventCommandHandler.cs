using BbQ.Cqrs;
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

        protected virtual Task HandleInternal(T request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Handle(T request, CancellationToken cancellationToken)
        {
            Context.AuditEvents.Add(new Data.Entities.Event.AuditEvent
            {
                UserId = request.UserId,
                EventCode = request.EventCode,
                ReferenceId = request.ReferenceId
            });
            await HandleInternal(request, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

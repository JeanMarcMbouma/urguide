using System;
using System.Linq;
using BbQ.Outcome;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Model;
using UrGuide.Model.Auditing;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Auditing
{
    public class UserActivityService : IUserActivityService
    {
        public UserActivityService(IUserContext userContext,
                                   UrGuideContext context)
        {
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserContext UserContext { get; }
        public UrGuideContext Context { get; }

        public async Task<Outcome<PagedList<ActivityModel>>> GetUserActivityAsync(PaginationParameters pagination, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PagedList<ActivityModel>>().WithErrors(ErrorMessages.NotAuthenticated);
            var items = await PagedList.Of(Context.AuditEvents.Where(x => x.UserId == UserContext.UserId).OrderByDescending(x => x.Created),
                pagination.PageNumber, a => AuditEventMapper.ToActivityModel(a), cancellationToken);
            return Result.Of(items);
        }
    }
}

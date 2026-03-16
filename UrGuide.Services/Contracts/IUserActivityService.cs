using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Auditing;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IUserActivityService
    {
        Task<Outcome<PagedList<ActivityModel>>> GetUserActivityAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    }
}

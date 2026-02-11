using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Search;

namespace UrGuide.Services.Contracts
{
    public interface ISearchAnalyticsService
    {
        Task<Result<bool>> TrackSearchAsync(
            string query,
            string userId,
            long resultsCount,
            long timeTakenMs,
            SearchFilters filters,
            string searchType,
            string? ipAddress,
            string userAgent,
            CancellationToken cancellationToken = default);
    }
}

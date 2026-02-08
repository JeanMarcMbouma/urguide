using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Search;
using UrGuide.Model.Results;
using UrGuide.Model.Search;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Search
{
    public class SearchAnalyticsService : ISearchAnalyticsService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<SearchAnalyticsService> _logger;

        public SearchAnalyticsService(UrGuideContext context, ILogger<SearchAnalyticsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> TrackSearchAsync(
            string query,
            string userId,
            long resultsCount,
            long timeTakenMs,
            SearchFilters filters,
            string searchType,
            string ipAddress,
            string userAgent,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var analytics = new SearchAnalytics
                {
                    Id = Guid.NewGuid().ToString(),
                    Query = query,
                    UserId = userId,
                    SearchedAt = DateTime.UtcNow,
                    ResultsCount = resultsCount,
                    TimeTakenMs = timeTakenMs,
                    Filters = filters != null ? JsonSerializer.Serialize(filters) : null,
                    SearchType = searchType,
                    HasResults = resultsCount > 0,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.SearchAnalytics.Add(analytics);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tracked search analytics for query: {Query}, results: {ResultsCount}", query, resultsCount);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking search analytics for query: {Query}", query);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }
    }
}

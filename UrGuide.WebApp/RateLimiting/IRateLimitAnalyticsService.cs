using System;
using System.Threading.Tasks;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Service for tracking rate limit analytics
    /// </summary>
    public interface IRateLimitAnalyticsService
    {
        /// <summary>
        /// Record a rate limit hit (request counted against limit)
        /// </summary>
        Task RecordHitAsync(string? userId, string endpoint, RateLimitTier tier, int currentCount, int limit);

        /// <summary>
        /// Record a rate limit violation (request blocked due to exceeding limit)
        /// </summary>
        Task RecordViolationAsync(string? userId, string endpoint, RateLimitTier tier);

        /// <summary>
        /// Get rate limit statistics for a user
        /// </summary>
        Task<RateLimitStatistics> GetStatisticsAsync(string? userId, DateTime? from = null, DateTime? to = null);
    }

    /// <summary>
    /// Rate limit statistics for analytics
    /// </summary>
    public class RateLimitStatistics
    {
        public int TotalRequests { get; set; }
        public int TotalViolations { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}

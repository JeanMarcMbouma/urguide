using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// In-memory implementation of rate limit analytics service
    /// For production, this should be replaced with a persistent storage implementation
    /// </summary>
    public class RateLimitAnalyticsService : IRateLimitAnalyticsService
    {
        private readonly ILogger<RateLimitAnalyticsService> _logger;
        private readonly ConcurrentDictionary<string, ConcurrentBag<RateLimitEvent>> _events = new();

        public RateLimitAnalyticsService(ILogger<RateLimitAnalyticsService> logger)
        {
            _logger = logger;
        }

        public Task RecordHitAsync(string userId, string endpoint, RateLimitTier tier, int currentCount, int limit)
        {
            var eventData = new RateLimitEvent
            {
                UserId = userId ?? "anonymous",
                Endpoint = endpoint,
                Tier = tier,
                Timestamp = DateTime.UtcNow,
                IsViolation = false,
                CurrentCount = currentCount,
                Limit = limit
            };

            var key = GetKey(userId);
            var events = _events.GetOrAdd(key, _ => new ConcurrentBag<RateLimitEvent>());
            events.Add(eventData);

            // Only log violations and warnings, not every hit
            if (currentCount > limit * 0.8) // Log when approaching limit
            {
                _logger.LogInformation("Rate limit approaching: User={UserId}, Endpoint={Endpoint}, Tier={Tier}, Count={CurrentCount}/{Limit}",
                    userId ?? "anonymous", endpoint, tier, currentCount, limit);
            }

            // Clean up old events (keep last 24 hours)
            CleanupOldEvents(key);

            return Task.CompletedTask;
        }

        public Task RecordViolationAsync(string userId, string endpoint, RateLimitTier tier)
        {
            var eventData = new RateLimitEvent
            {
                UserId = userId ?? "anonymous",
                Endpoint = endpoint,
                Tier = tier,
                Timestamp = DateTime.UtcNow,
                IsViolation = true
            };

            var key = GetKey(userId);
            var events = _events.GetOrAdd(key, _ => new ConcurrentBag<RateLimitEvent>());
            events.Add(eventData);

            _logger.LogWarning("Rate limit violation: User={UserId}, Endpoint={Endpoint}, Tier={Tier}",
                userId ?? "anonymous", endpoint, tier);

            // Clean up old events
            CleanupOldEvents(key);

            return Task.CompletedTask;
        }

        public Task<RateLimitStatistics> GetStatisticsAsync(string userId, DateTime? from = null, DateTime? to = null)
        {
            var key = GetKey(userId);
            var fromDate = from ?? DateTime.UtcNow.AddDays(-1);
            var toDate = to ?? DateTime.UtcNow;

            if (_events.TryGetValue(key, out var events))
            {
                var filteredEvents = events
                    .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
                    .ToList();

                var stats = new RateLimitStatistics
                {
                    TotalRequests = filteredEvents.Count,
                    TotalViolations = filteredEvents.Count(e => e.IsViolation),
                    PeriodStart = fromDate,
                    PeriodEnd = toDate
                };

                return Task.FromResult(stats);
            }

            return Task.FromResult(new RateLimitStatistics
            {
                TotalRequests = 0,
                TotalViolations = 0,
                PeriodStart = fromDate,
                PeriodEnd = toDate
            });
        }

        private string GetKey(string userId)
        {
            return userId ?? "anonymous";
        }

        private void CleanupOldEvents(string key)
        {
            if (_events.TryGetValue(key, out var events))
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-24);
                var recentEvents = events.Where(e => e.Timestamp > cutoffTime).ToList();
                
                if (recentEvents.Count < events.Count)
                {
                    var newBag = new ConcurrentBag<RateLimitEvent>(recentEvents);
                    _events.TryUpdate(key, newBag, events);
                }
            }
        }

        private class RateLimitEvent
        {
            public string UserId { get; set; }
            public string Endpoint { get; set; }
            public RateLimitTier Tier { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsViolation { get; set; }
            public int CurrentCount { get; set; }
            public int Limit { get; set; }
        }
    }
}

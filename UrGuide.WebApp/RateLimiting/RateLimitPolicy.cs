using System;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Represents a rate limit policy with configurable limits
    /// </summary>
    public class RateLimitPolicy
    {
        /// <summary>
        /// The tier this policy applies to
        /// </summary>
        public RateLimitTier Tier { get; set; }

        /// <summary>
        /// Maximum number of requests allowed
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Time window for the limit (e.g., "1m", "1h", "1d")
        /// </summary>
        public string Period { get; set; } = "1m";

        /// <summary>
        /// Gets the period as a TimeSpan
        /// </summary>
        public TimeSpan GetPeriodTimeSpan()
        {
            if (string.IsNullOrEmpty(Period))
                return TimeSpan.FromMinutes(1);

            var value = int.Parse(Period.Substring(0, Period.Length - 1));
            var unit = Period[Period.Length - 1];

            return unit switch
            {
                's' => TimeSpan.FromSeconds(value),
                'm' => TimeSpan.FromMinutes(value),
                'h' => TimeSpan.FromHours(value),
                'd' => TimeSpan.FromDays(value),
                _ => TimeSpan.FromMinutes(value)
            };
        }
    }
}

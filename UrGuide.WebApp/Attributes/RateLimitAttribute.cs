using System;

namespace UrGuide.WebApp.Attributes
{
    /// <summary>
    /// Attribute to apply custom rate limits to specific endpoints
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RateLimitAttribute : Attribute
    {
        /// <summary>
        /// Maximum number of requests allowed
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Time period (e.g., "1s", "1m", "1h", "1d")
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Specific tier this limit applies to (optional)
        /// </summary>
        public string? Tier { get; set; }

        public RateLimitAttribute(int limit, string period)
        {
            Limit = limit;
            Period = period;
        }
    }
}

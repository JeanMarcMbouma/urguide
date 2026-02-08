using System.Collections.Generic;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Configuration options for tiered rate limiting
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// Enable or disable rate limiting
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Global rate limit policies by tier
        /// </summary>
        public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();

        /// <summary>
        /// Endpoint-specific rate limit overrides
        /// Key: endpoint pattern (e.g., "POST:/api/tours")
        /// Value: Dictionary of tier -> policy
        /// </summary>
        public Dictionary<string, Dictionary<string, RateLimitPolicy>> EndpointPolicies { get; set; } = new();

        /// <summary>
        /// IP addresses or user IDs exempt from rate limiting
        /// </summary>
        public List<string> Exemptions { get; set; } = new();

        /// <summary>
        /// Enable rate limit analytics tracking
        /// </summary>
        public bool EnableAnalytics { get; set; } = true;
    }
}

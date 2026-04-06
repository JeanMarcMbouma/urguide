using System;

namespace UrGuide.WebApp.Caching
{
    /// <summary>
    /// Options for the Redis cache service, bound from the "Redis" configuration section.
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// Redis connection string. Supports both single-node and Sentinel formats.
        /// Example: "localhost:6379" or "sentinel-host:26379,sentinel-host2:26379,serviceName=mymaster"
        /// </summary>
        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>
        /// Optional key prefix applied to every cache key to avoid collisions in shared Redis instances.
        /// </summary>
        public string KeyPrefix { get; set; } = "urguide";

        /// <summary>
        /// Default expiry applied when a caller does not specify one.
        /// </summary>
        public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Redis Sentinel service name (e.g. "mymaster"). Leave empty when Sentinel is not used.
        /// </summary>
        public string SentinelServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Comma-separated Sentinel endpoints (e.g. "sentinel1:26379,sentinel2:26379").
        /// Required when <see cref="SentinelServiceName"/> is set.
        /// </summary>
        public string SentinelEndpoints { get; set; } = string.Empty;

        /// <summary>Redis database index (0-15).</summary>
        public int Database { get; set; } = 0;

        /// <summary>When true, Redis administrative commands (SCAN, DEBUG, etc.) are enabled on this connection.</summary>
        public bool AllowAdmin { get; set; } = false;
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Caching
{
    /// <summary>
    /// Redis-backed implementation of <see cref="ICacheService"/> using the cache-aside pattern.
    /// All cache misses are logged so callers can monitor hit/miss ratios in structured logs.
    /// When Redis is unavailable, <see cref="IConnectionMultiplexer"/> will be null and
    /// tag-based invalidation is skipped; all other operations continue via <see cref="IDistributedCache"/>.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer? _multiplexer;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly RedisOptions _options;

        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public RedisCacheService(
            IDistributedCache distributedCache,
            ILogger<RedisCacheService> logger,
            IOptions<RedisOptions> options,
            IServiceProvider services)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            // GetService<T> returns null (rather than throwing) when IConnectionMultiplexer is not
            // registered — this happens in the Redis-unavailable fallback scenario.
            _multiplexer = services.GetService<IConnectionMultiplexer>();
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = await _distributedCache.GetAsync(BuildKey(key), cancellationToken);
                if (bytes == null)
                {
                    _logger.LogDebug("Cache miss for key {Key}", key);
                    return default;
                }

                _logger.LogDebug("Cache hit for key {Key}", key);
                return JsonSerializer.Deserialize<T>(bytes, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for key {Key}; treating as cache miss", key);
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var effectiveExpiry = expiry ?? _options.DefaultExpiry;
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
                var entryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = effectiveExpiry
                };
                await _distributedCache.SetAsync(BuildKey(key), bytes, entryOptions, cancellationToken);
                _logger.LogDebug("Cached key {Key} with expiry {Expiry}", key, effectiveExpiry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SET failed for key {Key}; continuing without cache", key);
            }
        }

        /// <inheritdoc/>
        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var cached = await GetAsync<T>(key, cancellationToken);
            if (cached is not null)
                return cached;

            var value = await factory();
            if (value is not null)
                await SetAsync(key, value, expiry, cancellationToken);

            return value;
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(BuildKey(key), cancellationToken);
                _logger.LogDebug("Removed cache key {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            if (_multiplexer is null)
            {
                _logger.LogDebug("Tag invalidation skipped for tag {Tag}: no Redis connection available", tag);
                return;
            }

            try
            {
                var db = _multiplexer.GetDatabase();
                var prefix = BuildKey(tag);
                var keysToDelete = new List<RedisKey>();

                // Iterate all connected primary endpoints — handles standalone, Sentinel, and cluster.
                // Replicas are skipped because they only hold read-only copies.
                foreach (var endPoint in _multiplexer.GetEndPoints())
                {
                    var server = _multiplexer.GetServer(endPoint);
                    if (!server.IsConnected || server.IsReplica)
                        continue;

                    // SCAN avoids blocking the server; honours cancellation per-iteration.
                    await foreach (var key in server.KeysAsync(pattern: $"{prefix}*")
                        .WithCancellation(cancellationToken))
                    {
                        keysToDelete.Add(key);
                    }
                }

                if (keysToDelete.Count > 0)
                {
                    // Batch delete minimises round-trips
                    await db.KeyDeleteAsync(keysToDelete.ToArray());
                    _logger.LogDebug("Invalidated {Count} cache keys for tag {Tag}", keysToDelete.Count, tag);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis tag invalidation failed for tag {Tag}", tag);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            if (_multiplexer is null)
                return false;

            try
            {
                var db = _multiplexer.GetDatabase();
                await db.PingAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string BuildKey(string key) =>
            string.IsNullOrEmpty(_options.KeyPrefix) ? key : $"{_options.KeyPrefix}:{key}";
    }
}

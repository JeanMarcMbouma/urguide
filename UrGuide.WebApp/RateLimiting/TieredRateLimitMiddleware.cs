using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UrGuide.WebApp.Attributes;
using UrGuide.WebApp.Caching;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Middleware for tiered rate limiting based on user authentication and subscription level.
    /// When Redis is available, uses atomic INCR/EXPIRE operations for distributed accuracy.
    /// Falls back to in-process IMemoryCache when Redis is unavailable.
    /// </summary>
    public class TieredRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TieredRateLimitMiddleware> _logger;
        private readonly RateLimitOptions _options;
        private readonly IMemoryCache _memoryCache;
        private readonly IRateLimitAnalyticsService _analytics;
        // Resolved lazily via IServiceProvider so the middleware starts safely even when
        // IConnectionMultiplexer is not registered (Redis fallback scenario).
        private readonly IConnectionMultiplexer? _redis;

        public TieredRateLimitMiddleware(
            RequestDelegate next,
            ILogger<TieredRateLimitMiddleware> logger,
            IOptions<RateLimitOptions> options,
            IMemoryCache memoryCache,
            IRateLimitAnalyticsService analytics,
            IServiceProvider services)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _memoryCache = memoryCache;
            _analytics = analytics;
            // GetService<T> returns null (rather than throwing) when T is not registered.
            _redis = services.GetService<IConnectionMultiplexer>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip if rate limiting is disabled
            if (!_options.Enabled)
            {
                await _next(context);
                return;
            }

            // Check for exempt attribute on endpoint
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            if (endpoint.Metadata?.GetMetadata<RateLimitExemptAttribute>() != null)
            {
                _logger.LogDebug("Endpoint is exempt from rate limiting");
                await _next(context);
                return;
            }

            // Determine user tier
            var tier = GetUserTier(context);
            var userId = GetUserId(context);
            var ipAddress = GetClientIpAddress(context);

            // Check if user/IP is exempt
            if (IsExempt(userId, ipAddress))
            {
                _logger.LogDebug("User {UserId} or IP {IpAddress} is exempt from rate limiting", userId, ipAddress);
                await _next(context);
                return;
            }

            // Get the applicable policy - use route pattern to avoid per-ID buckets
            var routePatternText = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;
            var endpointKey = !string.IsNullOrEmpty(routePatternText)
                ? $"{context.Request.Method}:{routePatternText}"
                : $"{context.Request.Method}:{context.Request.Path}";
            var policy = GetApplicablePolicy(endpoint, endpointKey, tier);

            if (policy == null)
            {
                _logger.LogDebug("No rate limit policy found for tier {Tier}", tier);
                await _next(context);
                return;
            }

            // Check rate limit using Redis when available, fall back to memory cache
            var identifier = userId ?? ipAddress ?? "unknown";
            var periodTimeSpan = policy.GetPeriodTimeSpan();

            int currentCount;
            DateTimeOffset resetTime;

            if (_redis != null && _redis.IsConnected)
            {
                (currentCount, resetTime) = await IncrementRedisCounterAsync(tier, identifier, endpointKey, periodTimeSpan);
            }
            else
            {
                (currentCount, resetTime) = IncrementMemoryCounter(tier, identifier, endpointKey, periodTimeSpan);
            }

            // Track analytics
            if (_options.EnableAnalytics)
            {
                await _analytics.RecordHitAsync(userId, endpointKey, tier, currentCount, policy.Limit);
            }

            // Check if limit exceeded
            if (currentCount > policy.Limit)
            {
                // Track violation
                if (_options.EnableAnalytics)
                {
                    await _analytics.RecordViolationAsync(userId, endpointKey, tier);
                }

                _logger.LogWarning(
                    "Rate limit exceeded for {Identifier} on tier {Tier}, endpoint {Endpoint}. Count: {Count}/{Limit}",
                    identifier, tier, endpointKey, currentCount, policy.Limit);

                AddRateLimitHeaders(context, policy, currentCount, resetTime);

                // Return 429 Too Many Requests
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Rate limit exceeded",
                    message = "Too many requests. Please try again later.",
                    retryAfter = GetRetryAfter(resetTime)
                });
                return;
            }

            AddRateLimitHeaders(context, policy, currentCount, resetTime);

            // Continue to next middleware
            await _next(context);
        }

        // ── Redis counter (atomic INCR + EXPIRE) ──────────────────────────────

        private async Task<(int count, DateTimeOffset resetTime)> IncrementRedisCounterAsync(
            RateLimitTier tier, string identifier, string endpointKey, TimeSpan period)
        {
            try
            {
                var db = _redis!.GetDatabase();
                var counterKey = CacheKeys.RateLimit(tier.ToString(), identifier, endpointKey);

                // Atomically increment; set TTL on first request in window
                var count = await db.StringIncrementAsync(counterKey);
                if (count == 1)
                    await db.KeyExpireAsync(counterKey, period);

                // Derive reset time from remaining TTL
                var ttl = await db.KeyTimeToLiveAsync(counterKey);
                var resetTime = ttl.HasValue && ttl.Value > TimeSpan.Zero
                    ? DateTimeOffset.UtcNow.Add(ttl.Value)
                    : DateTimeOffset.UtcNow.Add(period);

                return ((int)count, resetTime);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis rate-limit counter failed; falling back to memory cache");
                return IncrementMemoryCounter(tier, identifier, endpointKey, period);
            }
        }

        // ── In-memory counter (single-node fallback) ──────────────────────────

        private (int count, DateTimeOffset resetTime) IncrementMemoryCounter(
            RateLimitTier tier, string identifier, string endpointKey, TimeSpan period)
        {
            var cacheKey = CacheKeys.RateLimit(tier.ToString(), identifier, endpointKey);
            var lockKey  = $"{cacheKey}:lock";
            var resetKey = CacheKeys.RateLimitReset(tier.ToString(), identifier, endpointKey);

            var lockObj = _memoryCache.GetOrCreate(lockKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = period;
                return new object();
            })!;

            int currentCount;
            DateTimeOffset resetTime;

            lock (lockObj)
            {
                if (!_memoryCache.TryGetValue(resetKey, out resetTime))
                {
                    resetTime = DateTimeOffset.UtcNow.Add(period);
                    _memoryCache.Set(resetKey, resetTime, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = resetTime
                    });
                }

                if (DateTimeOffset.UtcNow >= resetTime)
                {
                    currentCount = 1;
                    resetTime = DateTimeOffset.UtcNow.Add(period);
                    _memoryCache.Set(resetKey, resetTime, new MemoryCacheEntryOptions { AbsoluteExpiration = resetTime });
                    _memoryCache.Set(cacheKey, currentCount, new MemoryCacheEntryOptions { AbsoluteExpiration = resetTime });
                }
                else
                {
                    currentCount = _memoryCache.GetOrCreate(cacheKey, entry =>
                    {
                        entry.AbsoluteExpiration = resetTime;
                        return 0;
                    });
                    currentCount++;
                    _memoryCache.Set(cacheKey, currentCount, new MemoryCacheEntryOptions { AbsoluteExpiration = resetTime });
                }
            }

            return (currentCount, resetTime);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private RateLimitTier GetUserTier(HttpContext context)
        {
            var user = context.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
                return RateLimitTier.Anonymous;

            // Check if user is premium
            var isPremiumClaim = user.FindFirst("IsPremium")?.Value;
            if (isPremiumClaim == "True" || isPremiumClaim == "true")
                return RateLimitTier.Premium;

            // Check for premium role
            if (user.IsInRole("Premium"))
                return RateLimitTier.Premium;

            // Default to authenticated
            return RateLimitTier.Authenticated;
        }

        private string? GetUserId(HttpContext context) =>
            context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

        private string? GetClientIpAddress(HttpContext context) =>
            // Rely on RemoteIpAddress, which will be populated correctly when
            // ForwardedHeadersMiddleware is configured in the application
            context.Connection.RemoteIpAddress?.ToString();

        private bool IsExempt(string? userId, string? ipAddress)
        {
            if (_options.Exemptions == null || !_options.Exemptions.Any())
                return false;

            return (!string.IsNullOrEmpty(userId) && _options.Exemptions.Contains(userId))
                || (!string.IsNullOrEmpty(ipAddress) && _options.Exemptions.Contains(ipAddress));
        }

        private RateLimitPolicy? GetApplicablePolicy(Endpoint endpoint, string endpointKey, RateLimitTier tier)
        {
            // Check for custom attributes on endpoint (supports multiple with tier matching)
            var customAttributes = endpoint?.Metadata?.GetOrderedMetadata<RateLimitAttribute>();
            if (customAttributes != null && customAttributes.Any())
            {
                var matchingAttribute = customAttributes.FirstOrDefault(a =>
                    !string.IsNullOrEmpty(a.Tier) &&
                    Enum.TryParse<RateLimitTier>(a.Tier, out var specifiedTier) &&
                    specifiedTier == tier);

                var applicableAttribute = matchingAttribute ?? customAttributes.FirstOrDefault(a =>
                    string.IsNullOrEmpty(a.Tier));

                if (applicableAttribute != null)
                {
                    return new RateLimitPolicy
                    {
                        Tier = tier,
                        Limit = applicableAttribute.Limit,
                        Period = applicableAttribute.Period
                    };
                }
            }

            // Check for endpoint-specific policy in configuration
            if (_options.EndpointPolicies.TryGetValue(endpointKey, out var endpointPolicies))
            {
                var tierKey = tier.ToString();
                if (endpointPolicies.TryGetValue(tierKey, out var policy))
                    return policy;
            }

            // Fall back to global policy for the tier
            var globalTierKey = tier.ToString();
            if (_options.Policies.TryGetValue(globalTierKey, out var globalPolicy))
                return globalPolicy;

            return null;
        }

        private void AddRateLimitHeaders(HttpContext context, RateLimitPolicy policy, int currentCount, DateTimeOffset resetTime)
        {
            var remaining = Math.Max(0, policy.Limit - currentCount);
            context.Response.Headers["X-RateLimit-Limit"]     = policy.Limit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"]     = resetTime.ToUnixTimeSeconds().ToString();
            context.Response.Headers["X-RateLimit-Tier"]      = policy.Tier.ToString();
        }

        private int GetRetryAfter(DateTimeOffset resetTime)
        {
            var remaining = resetTime - DateTimeOffset.UtcNow;
            return remaining <= TimeSpan.Zero ? 0 : Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UrGuide.WebApp.Attributes;

namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Middleware for tiered rate limiting based on user authentication and subscription level
    /// </summary>
    public class TieredRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TieredRateLimitMiddleware> _logger;
        private readonly RateLimitOptions _options;
        private readonly IMemoryCache _cache;
        private readonly IRateLimitAnalyticsService _analytics;

        public TieredRateLimitMiddleware(
            RequestDelegate next,
            ILogger<TieredRateLimitMiddleware> logger,
            IOptions<RateLimitOptions> options,
            IMemoryCache cache,
            IRateLimitAnalyticsService analytics)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _cache = cache;
            _analytics = analytics;
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
            if (endpoint?.Metadata?.GetMetadata<RateLimitExemptAttribute>() != null)
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

            // Check rate limit
            var identifier = userId ?? ipAddress ?? "unknown";
            var cacheKey = $"ratelimit:{tier}:{identifier}:{endpointKey}";
            var lockKey = $"{cacheKey}:lock";
            var resetKey = $"{cacheKey}:reset";
            var periodTimeSpan = policy.GetPeriodTimeSpan();

            // Use a lock object with same expiration as counter to prevent memory leak
            var lockObj = _cache.GetOrCreate(lockKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = periodTimeSpan;
                return new object();
            });

            int currentCount;
            DateTimeOffset resetTime;
            lock (lockObj)
            {
                // Get or create reset time for this window
                if (!_cache.TryGetValue(resetKey, out resetTime))
                {
                    resetTime = DateTimeOffset.UtcNow.Add(periodTimeSpan);
                    _cache.Set(resetKey, resetTime, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = resetTime
                    });
                }

                // Check if window has expired
                if (DateTimeOffset.UtcNow >= resetTime)
                {
                    // Reset the window
                    currentCount = 1;
                    resetTime = DateTimeOffset.UtcNow.Add(periodTimeSpan);
                    _cache.Set(resetKey, resetTime, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = resetTime
                    });
                    _cache.Set(cacheKey, currentCount, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = resetTime
                    });
                }
                else
                {
                    // Increment counter without changing expiration
                    currentCount = _cache.GetOrCreate(cacheKey, entry =>
                    {
                        entry.AbsoluteExpiration = resetTime;
                        return 0;
                    });
                    currentCount++;
                    _cache.Set(cacheKey, currentCount, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = resetTime
                    });
                }
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

                _logger.LogWarning("Rate limit exceeded for {Identifier} on tier {Tier}, endpoint {Endpoint}. Count: {Count}/{Limit}",
                    identifier, tier, endpointKey, currentCount, policy.Limit);

                // Add rate limit headers
                AddRateLimitHeaders(context, policy, currentCount, resetTime);

                // Return 429 Too Many Requests
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Rate limit exceeded",
                    message = $"Too many requests. Please try again later.",
                    retryAfter = GetRetryAfter(resetTime)
                });
                return;
            }

            // Add rate limit headers
            AddRateLimitHeaders(context, policy, currentCount, resetTime);

            // Continue to next middleware
            await _next(context);
        }

        private RateLimitTier GetUserTier(HttpContext context)
        {
            var user = context.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return RateLimitTier.Anonymous;
            }

            // Check if user is premium
            // Look for IsPremium claim or check user attributes
            var isPremiumClaim = user.FindFirst("IsPremium")?.Value;
            if (isPremiumClaim == "True" || isPremiumClaim == "true")
            {
                return RateLimitTier.Premium;
            }

            // Check for premium role
            if (user.IsInRole("Premium"))
            {
                return RateLimitTier.Premium;
            }

            // Default to authenticated
            return RateLimitTier.Authenticated;
        }

        private string GetUserId(HttpContext context)
        {
            return context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Rely on RemoteIpAddress, which will be populated correctly when
            // ForwardedHeadersMiddleware is configured in the application
            return context.Connection.RemoteIpAddress?.ToString();
        }

        private bool IsExempt(string userId, string ipAddress)
        {
            if (_options.Exemptions == null || !_options.Exemptions.Any())
            {
                return false;
            }

            return _options.Exemptions.Contains(userId) || _options.Exemptions.Contains(ipAddress);
        }

        private RateLimitPolicy GetApplicablePolicy(Endpoint endpoint, string endpointKey, RateLimitTier tier)
        {
            // Check for custom attributes on endpoint (supports multiple with tier matching)
            var customAttributes = endpoint?.Metadata?.GetOrderedMetadata<RateLimitAttribute>();
            if (customAttributes != null && customAttributes.Any())
            {
                // First, try to find an attribute whose Tier matches the current tier
                var matchingAttribute = customAttributes.FirstOrDefault(a =>
                    !string.IsNullOrEmpty(a.Tier) &&
                    Enum.TryParse<RateLimitTier>(a.Tier, out var specifiedTier) &&
                    specifiedTier == tier);

                // If no exact tier match, fall back to an attribute without a specified Tier
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
                {
                    return policy;
                }
            }

            // Fall back to global policy for the tier
            var globalTierKey = tier.ToString();
            if (_options.Policies.TryGetValue(globalTierKey, out var globalPolicy))
            {
                return globalPolicy;
            }

            return null;
        }

        private void AddRateLimitHeaders(HttpContext context, RateLimitPolicy policy, int currentCount, DateTimeOffset resetTime)
        {
            var remaining = Math.Max(0, policy.Limit - currentCount);

            context.Response.Headers["X-RateLimit-Limit"] = policy.Limit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = resetTime.ToUnixTimeSeconds().ToString();
            context.Response.Headers["X-RateLimit-Tier"] = policy.Tier.ToString();
        }

        private int GetRetryAfter(DateTimeOffset resetTime)
        {
            var now = DateTimeOffset.UtcNow;
            var remaining = resetTime - now;

            if (remaining <= TimeSpan.Zero)
            {
                return 0;
            }

            // Return the number of whole seconds remaining, rounded up (defensive max to handle edge cases)
            return Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));
        }
    }
}

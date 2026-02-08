using Microsoft.AspNetCore.Http;
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

            // Get the applicable policy
            var endpointKey = $"{context.Request.Method}:{context.Request.Path}";
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
            var periodTimeSpan = policy.GetPeriodTimeSpan();

            var currentCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = periodTimeSpan;
                return 0;
            });

            currentCount++;

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
                AddRateLimitHeaders(context, policy, currentCount, periodTimeSpan);

                // Return 429 Too Many Requests
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Rate limit exceeded",
                    message = $"Too many requests. Please try again later.",
                    retryAfter = GetRetryAfter(cacheKey, periodTimeSpan)
                });
                return;
            }

            // Update counter
            _cache.Set(cacheKey, currentCount, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = periodTimeSpan
            });

            // Add rate limit headers
            AddRateLimitHeaders(context, policy, currentCount, periodTimeSpan);

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
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            
            // Check for forwarded IP (when behind a proxy)
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
            }

            return ipAddress;
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
            // Check for custom attribute on endpoint
            var customAttribute = endpoint?.Metadata?.GetMetadata<RateLimitAttribute>();
            if (customAttribute != null)
            {
                // If attribute specifies a tier and it doesn't match, skip
                if (!string.IsNullOrEmpty(customAttribute.Tier) && 
                    Enum.TryParse<RateLimitTier>(customAttribute.Tier, out var specifiedTier) && 
                    specifiedTier != tier)
                {
                    // Fall through to check other policies
                }
                else
                {
                    return new RateLimitPolicy
                    {
                        Tier = tier,
                        Limit = customAttribute.Limit,
                        Period = customAttribute.Period
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

        private void AddRateLimitHeaders(HttpContext context, RateLimitPolicy policy, int currentCount, TimeSpan periodTimeSpan)
        {
            var remaining = Math.Max(0, policy.Limit - currentCount);
            var resetTime = DateTimeOffset.UtcNow.Add(periodTimeSpan).ToUnixTimeSeconds();

            context.Response.Headers["X-RateLimit-Limit"] = policy.Limit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = resetTime.ToString();
            context.Response.Headers["X-RateLimit-Tier"] = policy.Tier.ToString();
        }

        private int GetRetryAfter(string cacheKey, TimeSpan periodTimeSpan)
        {
            // Return the number of seconds until the rate limit resets
            return (int)periodTimeSpan.TotalSeconds;
        }
    }
}

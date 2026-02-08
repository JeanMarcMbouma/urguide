using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UrGuide.WebApp.Attributes;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.RateLimiting;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Controller for rate limit analytics and monitoring
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class RateLimitController : ControllerBase
    {
        private readonly IRateLimitAnalyticsService _analyticsService;

        public RateLimitController(IRateLimitAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        /// <summary>
        /// Get rate limit statistics for the current user
        /// </summary>
        /// <param name="from">Start date (optional)</param>
        /// <param name="to">End date (optional)</param>
        /// <returns>Rate limit statistics</returns>
        [HttpGet("stats")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(RateLimitStatistics))]
        public async Task<IActionResult> GetStatistics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ErrorEnvelop.Create(new[] { "User ID not found in claims" }));
            }

            var stats = await _analyticsService.GetStatisticsAsync(userId, from, to);
            return Ok(stats);
        }

        /// <summary>
        /// Health check endpoint exempt from rate limiting
        /// </summary>
        /// <returns>OK response</returns>
        [HttpGet("health")]
        [AllowAnonymous]
        [RateLimitExempt("Health check endpoint")]
        [ProducesResponseType(200)]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                message = "Rate limiting system is operational"
            });
        }

        /// <summary>
        /// Test endpoint with custom rate limit
        /// </summary>
        /// <returns>OK response</returns>
        [HttpGet("test")]
        [AllowAnonymous]
        [RateLimit(3, "1m")] // Custom: 3 requests per minute
        [ProducesResponseType(200)]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "This endpoint has a custom rate limit of 3 requests per minute",
                timestamp = DateTime.UtcNow
            });
        }
    }
}

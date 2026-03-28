using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Model.Recommendations;
using UrGuide.Services.Recommendations;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(IRecommendationService recommendationService, ILogger<RecommendationController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        /// <summary>
        /// Get personalized tour recommendations for the authenticated user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRecommendations([FromQuery] int count = 10, [FromQuery] double? lat = null, [FromQuery] double? lng = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (count < 1 || count > 50)
                {
                    return BadRequest(new { error = "Count must be between 1 and 50" });
                }

                if (lat.HasValue && (lat.Value < -90 || lat.Value > 90))
                {
                    return BadRequest(new { error = "Latitude must be between -90 and 90" });
                }

                if (lng.HasValue && (lng.Value < -180 || lng.Value > 180))
                {
                    return BadRequest(new { error = "Longitude must be between -180 and 180" });
                }

                var recommendations = await _recommendationService.GetRecommendationsAsync(userId, count, lat, lng);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendations");
                return StatusCode(500, new { error = "An error occurred while getting recommendations" });
            }
        }

        /// <summary>
        /// Get popular tours (no authentication required)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularTours([FromQuery] int count = 10, [FromQuery] double? lat = null, [FromQuery] double? lng = null)
        {
            try
            {
                if (count < 1 || count > 50)
                {
                    return BadRequest(new { error = "Count must be between 1 and 50" });
                }

                if (lat.HasValue && (lat.Value < -90 || lat.Value > 90))
                {
                    return BadRequest(new { error = "Latitude must be between -90 and 90" });
                }

                if (lng.HasValue && (lng.Value < -180 || lng.Value > 180))
                {
                    return BadRequest(new { error = "Longitude must be between -180 and 180" });
                }

                var tours = await _recommendationService.GetPopularToursAsync(count, lat, lng);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular tours");
                return StatusCode(500, new { error = "An error occurred while getting popular tours" });
            }
        }

        /// <summary>
        /// Set user preferences for recommendations
        /// </summary>
        [HttpPut("preferences")]
        public async Task<IActionResult> SetPreferences([FromBody] SetPreferencesRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (request?.Preferences == null || request.Preferences.Count == 0)
                {
                    return BadRequest(new { error = "At least one preference is required" });
                }

                if (request.Preferences.Count > 20)
                {
                    return BadRequest(new { error = "Maximum 20 preferences allowed" });
                }

                var result = await _recommendationService.SetUserPreferencesAsync(userId, request);
                if (!result)
                {
                    return BadRequest(new { error = "Invalid preferences. Valid types: category, location, price_range, duration, language" });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting preferences");
                return StatusCode(500, new { error = "An error occurred while setting preferences" });
            }
        }

        /// <summary>
        /// Get user preferences
        /// </summary>
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var preferences = await _recommendationService.GetUserPreferencesAsync(userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting preferences");
                return StatusCode(500, new { error = "An error occurred while getting preferences" });
            }
        }

        /// <summary>
        /// Record a user interaction with a tour
        /// </summary>
        [HttpPost("interactions")]
        public async Task<IActionResult> RecordInteraction([FromBody] RecordInteractionRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(request?.TourId))
                {
                    return BadRequest(new { error = "TourId is required" });
                }

                if (request.Type < 0 || request.Type > 4)
                {
                    return BadRequest(new { error = "Type must be between 0 (Viewed) and 4 (Shared)" });
                }

                var result = await _recommendationService.RecordInteractionAsync(userId, request);
                if (!result)
                {
                    return BadRequest(new { error = "Invalid interaction type" });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording interaction");
                return StatusCode(500, new { error = "An error occurred while recording the interaction" });
            }
        }

        /// <summary>
        /// Provide feedback on a recommendation
        /// </summary>
        [HttpPost("feedback")]
        public async Task<IActionResult> ProvideFeedback([FromBody] RecommendationFeedbackRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(request?.RecommendationId))
                {
                    return BadRequest(new { error = "RecommendationId is required" });
                }

                var result = await _recommendationService.ProvideFeedbackAsync(userId, request);
                if (!result)
                {
                    return NotFound(new { error = "Recommendation not found" });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error providing feedback");
                return StatusCode(500, new { error = "An error occurred while providing feedback" });
            }
        }

        /// <summary>
        /// Get recommendation statistics (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _recommendationService.GetRecommendationStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendation stats");
                return StatusCode(500, new { error = "An error occurred while getting recommendation stats" });
            }
        }
    }
}

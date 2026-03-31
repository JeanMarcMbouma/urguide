using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Data.Entities.Recommendations;
using UrGuide.Model.Recommendations;
using UrGuide.Services.Recommendations;
using UrGuide.WebApp.Resources;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RecommendationController(IRecommendationService recommendationService, ILogger<RecommendationController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _recommendationService = recommendationService;
            _logger = logger;
            _localizer = localizer;
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
                    return BadRequest(new { error = _localizer["Recommendation_CountInvalid"].Value });
                }

                if (lat.HasValue && (lat.Value < -90 || lat.Value > 90))
                {
                    return BadRequest(new { error = _localizer["Recommendation_LatitudeInvalid"].Value });
                }

                if (lng.HasValue && (lng.Value < -180 || lng.Value > 180))
                {
                    return BadRequest(new { error = _localizer["Recommendation_LongitudeInvalid"].Value });
                }

                var recommendations = await _recommendationService.GetRecommendationsAsync(userId, count, lat, lng);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendations");
                return StatusCode(500, new { error = _localizer["Recommendation_GetError"].Value });
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
                    return BadRequest(new { error = _localizer["Recommendation_CountInvalid"].Value });
                }

                if (lat.HasValue && (lat.Value < -90 || lat.Value > 90))
                {
                    return BadRequest(new { error = _localizer["Recommendation_LatitudeInvalid"].Value });
                }

                if (lng.HasValue && (lng.Value < -180 || lng.Value > 180))
                {
                    return BadRequest(new { error = _localizer["Recommendation_LongitudeInvalid"].Value });
                }

                var tours = await _recommendationService.GetPopularToursAsync(count, lat, lng);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular tours");
                return StatusCode(500, new { error = _localizer["Recommendation_PopularToursError"].Value });
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
                    return BadRequest(new { error = _localizer["Recommendation_PreferenceRequired"].Value });
                }

                if (request.Preferences.Count > 20)
                {
                    return BadRequest(new { error = _localizer["Recommendation_PreferenceMaximum"].Value });
                }

                var result = await _recommendationService.SetUserPreferencesAsync(userId, request);
                if (!result)
                {
                    var validTypes = string.Join(", ", RecommendationService.ValidPreferenceTypes);
                    return BadRequest(new { error = string.Format(_localizer["Recommendation_PreferenceInvalid"].Value, validTypes) });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting preferences");
                return StatusCode(500, new { error = _localizer["Recommendation_SetPreferencesError"].Value });
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
                return StatusCode(500, new { error = _localizer["Recommendation_GetPreferencesError"].Value });
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
                    return BadRequest(new { error = _localizer["Recommendation_TourIdRequired"].Value });
                }

                if (!Enum.IsDefined(typeof(InteractionType), request.Type))
                {
                    var allowedTypes = string.Join(", ", Enum.GetNames(typeof(InteractionType)));
                    return BadRequest(new { error = string.Format(_localizer["Recommendation_InteractionTypeInvalid"].Value, allowedTypes) });
                }

                var result = await _recommendationService.RecordInteractionAsync(userId, request);
                if (!result)
                {
                    return BadRequest(new { error = _localizer["Recommendation_InteractionTypeUnknown"].Value });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording interaction");
                return StatusCode(500, new { error = _localizer["Recommendation_RecordInteractionError"].Value });
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
                    return BadRequest(new { error = _localizer["Recommendation_FeedbackIdRequired"].Value });
                }

                var result = await _recommendationService.ProvideFeedbackAsync(userId, request);
                if (!result)
                {
                    return NotFound(new { error = _localizer["Recommendation_NotFound"].Value });
                }

                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error providing feedback");
                return StatusCode(500, new { error = _localizer["Recommendation_FeedbackError"].Value });
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
                return StatusCode(500, new { error = _localizer["Recommendation_StatsError"].Value });
            }
        }
    }
}

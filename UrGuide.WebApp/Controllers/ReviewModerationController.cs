using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Reviews;
using UrGuide.Services.Reviews;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reviews")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class ReviewModerationController : ControllerBase
    {
        private readonly IReviewModerationService _moderationService;
        private readonly ILogger<ReviewModerationController> _logger;

        public ReviewModerationController(
            IReviewModerationService moderationService,
            ILogger<ReviewModerationController> logger)
        {
            _moderationService = moderationService;
            _logger = logger;
        }

        /// <summary>
        /// Flag a review for moderation
        /// </summary>
        [HttpPost("{reviewId}/flag")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> FlagReview(string reviewId, [FromBody] FlagReviewRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _moderationService.FlagReviewAsync(userId, reviewId, request);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flagging review {ReviewId}", reviewId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get the moderation queue (Admin only)
        /// </summary>
        [HttpGet("moderation/queue")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(ModerationQueueItem[]))]
        public async Task<IActionResult> GetModerationQueue(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string status = null)
        {
            try
            {
                var result = await _moderationService.GetModerationQueueAsync(page, pageSize, status);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation queue");
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Take a moderation action on a review (Admin only)
        /// </summary>
        [HttpPost("moderation/{reviewId}/action")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> ModerateReview(string reviewId, [FromBody] ReviewModerationResult action)
        {
            try
            {
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();

                action.ReviewId = reviewId;
                var result = await _moderationService.ModerateReviewAsync(adminId, reviewId, action);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating review {ReviewId}", reviewId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get moderation statistics (Admin only)
        /// </summary>
        [HttpGet("moderation/stats")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(ModerationStatsDto))]
        public async Task<IActionResult> GetModerationStats()
        {
            try
            {
                var result = await _moderationService.GetModerationStatsAsync();
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation stats");
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Submit an appeal for a moderated review
        /// </summary>
        [HttpPost("{reviewId}/appeal")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> SubmitAppeal(string reviewId, [FromBody] ReviewAppealRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                request.ReviewId = reviewId;
                var result = await _moderationService.SubmitAppealAsync(userId, request);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting appeal for review {ReviewId}", reviewId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }
    }
}

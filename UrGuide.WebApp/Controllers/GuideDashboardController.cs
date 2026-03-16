using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.Services.Payments;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/guide")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class GuideDashboardController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IPayoutService _payoutService;
        private readonly ITourRequestService _tourRequestService;
        private readonly ILogger<GuideDashboardController> _logger;

        public GuideDashboardController(
            IFeedbackService feedbackService,
            IPayoutService payoutService,
            ITourRequestService tourRequestService,
            ILogger<GuideDashboardController> logger)
        {
            _feedbackService = feedbackService;
            _payoutService = payoutService;
            _tourRequestService = tourRequestService;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard summary stats for the authenticated guide
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Get feedback/reviews for the guide (page 1, large size for stats)
                var pagination = new PaginationParameters { PageNumber = 1 };
                var feedbackResult = await _feedbackService.GetUserFeedback(userId, pagination, cancellationToken);

                decimal averageRating = 0;
                int reviewCount = 0;
                if (!feedbackResult.IsError && feedbackResult.Value != null)
                {
                    reviewCount = feedbackResult.Value.ItemsCount;
                    if (reviewCount > 0)
                    {
                        decimal totalRating = 0;
                        foreach (var fb in feedbackResult.Value.Items)
                            totalRating += fb.Rating;
                        averageRating = totalRating / feedbackResult.Value.Items.Count;
                    }
                }

                // Get available balance
                decimal availableBalance = 0;
                try
                {
                    availableBalance = await _payoutService.GetGuideAvailableBalanceAsync(userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not retrieve payout balance for guide {UserId}", userId);
                }

                // Get open tour requests count (guides see open requests)
                var requestPagination = new SearchParameters { PageNumber = 1 };
                var requestsResult = await _tourRequestService.GetTourRequestsAsync(requestPagination, cancellationToken);
                int openRequests = requestsResult.IsError ? 0 : (requestsResult.Value?.ItemsCount ?? 0);

                var dashboard = new
                {
                    availableBalance,
                    averageRating = Math.Round(averageRating, 1),
                    reviewCount,
                    openTourRequests = openRequests,
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guide dashboard");
                return StatusCode(500, new { error = "An error occurred while retrieving dashboard data" });
            }
        }
    }
}

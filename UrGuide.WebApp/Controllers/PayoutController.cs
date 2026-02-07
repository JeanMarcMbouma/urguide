using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Model.Payments;
using UrGuide.Services.Payments;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayoutController : ControllerBase
    {
        private readonly IPayoutService _payoutService;
        private readonly ILogger<PayoutController> _logger;

        public PayoutController(IPayoutService payoutService, ILogger<PayoutController> logger)
        {
            _payoutService = payoutService;
            _logger = logger;
        }

        /// <summary>
        /// Create a payout request for a guide
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayout([FromBody] CreatePayoutRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Validate that the requesting user is the guide
                if (request.GuideId != userId)
                {
                    return Forbid();
                }

                var payout = await _payoutService.CreatePayoutAsync(request.GuideId, request);
                return Ok(payout);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid payout request");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Payout operation failed");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payout");
                return StatusCode(500, new { error = "An error occurred while creating the payout" });
            }
        }

        /// <summary>
        /// Get payout details
        /// </summary>
        [HttpGet("{payoutId}")]
        public async Task<IActionResult> GetPayout(string payoutId)
        {
            try
            {
                var payout = await _payoutService.GetPayoutAsync(payoutId);
                return Ok(payout);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payout");
                return StatusCode(500, new { error = "An error occurred while retrieving the payout" });
            }
        }

        /// <summary>
        /// Get guide's payout history
        /// </summary>
        [HttpGet("guide/{guideId}")]
        public async Task<IActionResult> GetGuidePayouts(string guideId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Validate that the requesting user is the guide
                if (guideId != userId)
                {
                    return Forbid();
                }

                var payouts = await _payoutService.GetGuidePayoutsAsync(guideId, page, pageSize);
                return Ok(payouts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guide payouts");
                return StatusCode(500, new { error = "An error occurred while retrieving payouts" });
            }
        }

        /// <summary>
        /// Get guide's available balance for payout
        /// </summary>
        [HttpGet("guide/{guideId}/balance")]
        public async Task<IActionResult> GetGuideBalance(string guideId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Validate that the requesting user is the guide
                if (guideId != userId)
                {
                    return Forbid();
                }

                var balance = await _payoutService.GetGuideAvailableBalanceAsync(guideId);
                return Ok(new { guideId, availableBalance = balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guide balance");
                return StatusCode(500, new { error = "An error occurred while retrieving balance" });
            }
        }

        /// <summary>
        /// Process a payout (admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{payoutId}/process")]
        public async Task<IActionResult> ProcessPayout(string payoutId)
        {
            try
            {
                var success = await _payoutService.ProcessPayoutAsync(payoutId);
                if (!success)
                {
                    return NotFound(new { error = "Payout not found or failed to process" });
                }

                return Ok(new { message = "Payout processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payout");
                return StatusCode(500, new { error = "An error occurred while processing the payout" });
            }
        }
    }
}

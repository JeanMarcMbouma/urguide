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
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;
        private readonly ILogger<RefundController> _logger;

        public RefundController(IRefundService refundService, ILogger<RefundController> logger)
        {
            _refundService = refundService;
            _logger = logger;
        }

        /// <summary>
        /// Request a refund for a payment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRefund([FromBody] CreateRefundRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var refund = await _refundService.CreateRefundAsync(userId, request);
                return Ok(refund);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid refund request");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Refund operation failed");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refund");
                return StatusCode(500, new { error = "An error occurred while creating the refund" });
            }
        }

        /// <summary>
        /// Get refund details
        /// </summary>
        [HttpGet("{refundId}")]
        public async Task<IActionResult> GetRefund(string refundId)
        {
            try
            {
                var refund = await _refundService.GetRefundAsync(refundId);
                return Ok(refund);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refund");
                return StatusCode(500, new { error = "An error occurred while retrieving the refund" });
            }
        }

        /// <summary>
        /// Get refunds for a payment
        /// </summary>
        [HttpGet("payment/{paymentId}")]
        public async Task<IActionResult> GetPaymentRefunds(string paymentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var refunds = await _refundService.GetPaymentRefundsAsync(paymentId, page, pageSize);
                return Ok(refunds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment refunds");
                return StatusCode(500, new { error = "An error occurred while retrieving refunds" });
            }
        }

        /// <summary>
        /// Process a refund (admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{refundId}/process")]
        public async Task<IActionResult> ProcessRefund(string refundId)
        {
            try
            {
                var success = await _refundService.ProcessRefundAsync(refundId);
                if (!success)
                {
                    return NotFound(new { error = "Refund not found or failed to process" });
                }

                return Ok(new { message = "Refund processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                return StatusCode(500, new { error = "An error occurred while processing the refund" });
            }
        }
    }
}

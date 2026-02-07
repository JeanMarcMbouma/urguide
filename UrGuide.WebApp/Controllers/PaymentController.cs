using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Model.Payments;
using UrGuide.Services.Payments;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a payment for a tour booking
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var payment = await _paymentService.CreatePaymentAsync(userId, request);
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid payment request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return StatusCode(500, new { error = "An error occurred while creating the payment" });
            }
        }

        /// <summary>
        /// Get payment details
        /// </summary>
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(string paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentAsync(paymentId);
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment");
                return StatusCode(500, new { error = "An error occurred while retrieving the payment" });
            }
        }

        /// <summary>
        /// Get user's transaction history
        /// </summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var transactions = await _paymentService.GetUserTransactionHistoryAsync(userId, page, pageSize);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction history");
                return StatusCode(500, new { error = "An error occurred while retrieving transaction history" });
            }
        }

        /// <summary>
        /// Confirm a payment
        /// </summary>
        [HttpPost("{paymentId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(string paymentId)
        {
            try
            {
                var success = await _paymentService.ConfirmPaymentAsync(paymentId);
                if (!success)
                {
                    return NotFound(new { error = "Payment not found" });
                }

                return Ok(new { message = "Payment confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment");
                return StatusCode(500, new { error = "An error occurred while confirming the payment" });
            }
        }

        /// <summary>
        /// Cancel a payment
        /// </summary>
        [HttpPost("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(string paymentId)
        {
            try
            {
                var success = await _paymentService.CancelPaymentAsync(paymentId);
                if (!success)
                {
                    return NotFound(new { error = "Payment not found" });
                }

                return Ok(new { message = "Payment cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment");
                return StatusCode(500, new { error = "An error occurred while cancelling the payment" });
            }
        }
    }
}

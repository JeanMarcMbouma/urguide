using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Model.Payments;
using UrGuide.Services.Payments;
using UrGuide.WebApp.Resources;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _paymentService = paymentService;
            _logger = logger;
            _localizer = localizer;
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
                return StatusCode(500, new { error = _localizer["Payment_CreateError"].Value });
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
                return StatusCode(500, new { error = _localizer["Payment_RetrieveError"].Value });
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
                return StatusCode(500, new { error = _localizer["Payment_HistoryError"].Value });
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
                    return NotFound(new { error = _localizer["Payment_NotFound"].Value });
                }

                return Ok(new { message = _localizer["Payment_ConfirmSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment");
                return StatusCode(500, new { error = _localizer["Payment_ConfirmError"].Value });
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
                    return NotFound(new { error = _localizer["Payment_NotFound"].Value });
                }

                return Ok(new { message = _localizer["Payment_CancelSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment");
                return StatusCode(500, new { error = _localizer["Payment_CancelError"].Value });
            }
        }
    }
}

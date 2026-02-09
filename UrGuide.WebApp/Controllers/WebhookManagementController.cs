using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Webhooks;
using UrGuide.Services.Webhooks;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Webhook management endpoints for external integrations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/webhook-management")]
    public class WebhookManagementController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookManagementController> _logger;

        public WebhookManagementController(IWebhookService webhookService, ILogger<WebhookManagementController> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new webhook
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterWebhook([FromBody] RegisterWebhookRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var webhook = await _webhookService.RegisterWebhookAsync(userId, request);
                return Ok(webhook);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid webhook registration request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering webhook");
                return StatusCode(500, new { error = "An error occurred while registering the webhook" });
            }
        }

        /// <summary>
        /// Get all webhooks for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWebhooks()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var webhooks = await _webhookService.GetUserWebhooksAsync(userId);
                return Ok(webhooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving webhooks");
                return StatusCode(500, new { error = "An error occurred while retrieving webhooks" });
            }
        }

        /// <summary>
        /// Get a specific webhook by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWebhook(string id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var webhook = await _webhookService.GetWebhookAsync(id, userId);
                return Ok(webhook);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving webhook");
                return StatusCode(500, new { error = "An error occurred while retrieving the webhook" });
            }
        }

        /// <summary>
        /// Update a webhook
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWebhook(string id, [FromBody] UpdateWebhookRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var webhook = await _webhookService.UpdateWebhookAsync(id, userId, request);
                return Ok(webhook);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid webhook update request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating webhook");
                return StatusCode(500, new { error = "An error occurred while updating the webhook" });
            }
        }

        /// <summary>
        /// Delete a webhook
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWebhook(string id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var deleted = await _webhookService.DeleteWebhookAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { error = "Webhook not found" });
                }

                return Ok(new { message = "Webhook deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting webhook");
                return StatusCode(500, new { error = "An error occurred while deleting the webhook" });
            }
        }

        /// <summary>
        /// Get webhook delivery history
        /// </summary>
        [HttpGet("{id}/deliveries")]
        public async Task<IActionResult> GetWebhookDeliveries(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var deliveries = await _webhookService.GetWebhookDeliveriesAsync(id, userId, page, pageSize);
                return Ok(deliveries);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving webhook deliveries");
                return StatusCode(500, new { error = "An error occurred while retrieving webhook deliveries" });
            }
        }

        /// <summary>
        /// Test a webhook by sending a sample payload
        /// </summary>
        [HttpPost("test")]
        public async Task<IActionResult> TestWebhook([FromBody] TestWebhookRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var success = await _webhookService.TestWebhookAsync(request.WebhookId, userId, request);
                if (success)
                {
                    return Ok(new { message = "Test webhook delivered successfully" });
                }
                else
                {
                    return BadRequest(new { error = "Test webhook delivery failed" });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing webhook");
                return StatusCode(500, new { error = "An error occurred while testing the webhook" });
            }
        }
    }
}

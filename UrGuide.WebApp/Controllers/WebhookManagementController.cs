using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Webhooks;
using UrGuide.Services.Webhooks;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/webhook-management")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class WebhookManagementController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookManagementController> _logger;

        public WebhookManagementController(IWebhookService webhookService, ILogger<WebhookManagementController> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(WebhookCreatedResponse))]
        public async Task<IActionResult> RegisterWebhook([FromBody] RegisterWebhookRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.RegisterWebhookAsync(userId, request);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<WebhookResponse>))]
        public async Task<IActionResult> GetWebhooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.GetUserWebhooksAsync(userId);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(WebhookResponse))]
        public async Task<IActionResult> GetWebhook(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.GetWebhookAsync(id, userId);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(WebhookResponse))]
        public async Task<IActionResult> UpdateWebhook(string id, [FromBody] UpdateWebhookRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.UpdateWebhookAsync(id, userId, request);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteWebhook(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.DeleteWebhookAsync(id, userId);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("{id}/deliveries")]
        [ProducesResponseType(200, Type = typeof(List<WebhookDeliveryResponse>))]
        public async Task<IActionResult> GetWebhookDeliveries(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var result = await _webhookService.GetWebhookDeliveriesAsync(id, userId, page, pageSize);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("test")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> TestWebhook([FromBody] TestWebhookRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _webhookService.TestWebhookAsync(request.WebhookId, userId, request);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }
    }
}

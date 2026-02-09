using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using UrGuide.Data;
using UrGuide.Data.Entities.Webhooks;
using UrGuide.Model.Webhooks;

namespace UrGuide.Services.Webhooks
{
    public class WebhookService : IWebhookService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<WebhookService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookService(
            UrGuideContext context,
            ILogger<WebhookService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WebhookResponse> RegisterWebhookAsync(string userId, RegisterWebhookRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                throw new ArgumentException("Webhook URL is required");

            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) || 
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Invalid webhook URL");

            if (request.Events == null || !request.Events.Any())
                throw new ArgumentException("At least one event must be subscribed");

            var secret = GenerateSecret();
            var webhook = new WebhookSubscription
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Url = request.Url,
                Secret = secret,
                IsActive = true,
                Description = request.Description ?? string.Empty,
                Events = request.Events.Distinct().ToList(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SuccessCount = 0,
                FailureCount = 0
            };

            _context.WebhookSubscriptions.Add(webhook);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Webhook registered for user {userId}: {webhook.Id}");

            return MapToResponse(webhook);
        }

        public async Task<List<WebhookResponse>> GetUserWebhooksAsync(string userId)
        {
            var webhooks = await _context.WebhookSubscriptions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return webhooks.Select(MapToResponse).ToList();
        }

        public async Task<WebhookResponse> GetWebhookAsync(string webhookId, string userId)
        {
            var webhook = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);

            if (webhook == null)
                throw new ArgumentException("Webhook not found");

            return MapToResponse(webhook);
        }

        public async Task<WebhookResponse> UpdateWebhookAsync(string webhookId, string userId, UpdateWebhookRequest request)
        {
            var webhook = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);

            if (webhook == null)
                throw new ArgumentException("Webhook not found");

            if (!string.IsNullOrWhiteSpace(request.Url))
            {
                if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Invalid webhook URL");
                webhook.Url = request.Url;
            }

            if (request.Description != null)
                webhook.Description = request.Description;

            if (request.Events != null && request.Events.Any())
                webhook.Events = request.Events.Distinct().ToList();

            if (request.IsActive.HasValue)
                webhook.IsActive = request.IsActive.Value;

            webhook.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Webhook updated: {webhookId}");

            return MapToResponse(webhook);
        }

        public async Task<bool> DeleteWebhookAsync(string webhookId, string userId)
        {
            var webhook = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);

            if (webhook == null)
                return false;

            _context.WebhookSubscriptions.Remove(webhook);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Webhook deleted: {webhookId}");

            return true;
        }

        public async Task<List<WebhookDeliveryResponse>> GetWebhookDeliveriesAsync(string webhookId, string userId, int page = 1, int pageSize = 20)
        {
            // Verify webhook belongs to user
            var webhook = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);

            if (webhook == null)
                throw new ArgumentException("Webhook not found");

            var deliveries = await _context.WebhookDeliveries
                .Where(d => d.WebhookSubscriptionId == webhookId)
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new WebhookDeliveryResponse
                {
                    Id = d.Id,
                    WebhookSubscriptionId = d.WebhookSubscriptionId,
                    Event = d.Event,
                    Payload = d.Payload,
                    Status = d.Status,
                    AttemptCount = d.AttemptCount,
                    MaxAttempts = d.MaxAttempts,
                    CreatedAt = d.CreatedAt,
                    DeliveredAt = d.DeliveredAt,
                    NextRetryAt = d.NextRetryAt,
                    ResponseStatusCode = d.ResponseStatusCode,
                    ResponseBody = d.ResponseBody,
                    ErrorMessage = d.ErrorMessage
                })
                .ToListAsync();

            return deliveries;
        }

        public async Task<bool> TestWebhookAsync(string webhookId, string userId, TestWebhookRequest request)
        {
            var webhook = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);

            if (webhook == null)
                throw new ArgumentException("Webhook not found");

            var payload = new WebhookPayload
            {
                EventId = Guid.NewGuid().ToString(),
                Event = request.Event,
                Timestamp = DateTime.UtcNow,
                Data = request.SamplePayload ?? new { test = true, message = "This is a test webhook delivery" }
            };

            var delivery = await CreateDeliveryAsync(webhook, payload);
            await DeliverWebhookAsync(delivery);

            return delivery.Status == WebhookDeliveryStatus.Delivered;
        }

        public async Task PublishEventAsync(WebhookEvent eventType, object data)
        {
            var subscriptions = await _context.WebhookSubscriptions
                .Where(w => w.IsActive && w.Events.Contains(eventType))
                .ToListAsync();

            if (!subscriptions.Any())
            {
                _logger.LogDebug($"No active webhooks found for event {eventType}");
                return;
            }

            var payload = new WebhookPayload
            {
                EventId = Guid.NewGuid().ToString(),
                Event = eventType,
                Timestamp = DateTime.UtcNow,
                Data = data
            };

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var delivery = await CreateDeliveryAsync(subscription, payload);
                    // Fire and forget - actual delivery happens in background
                    _ = Task.Run(async () => await DeliverWebhookAsync(delivery));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating delivery for webhook {subscription.Id}");
                }
            }
        }

        private async Task<WebhookDelivery> CreateDeliveryAsync(WebhookSubscription subscription, WebhookPayload payload)
        {
            var payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            var signature = GenerateSignature(payloadJson, subscription.Secret);

            var delivery = new WebhookDelivery
            {
                Id = Guid.NewGuid().ToString(),
                WebhookSubscriptionId = subscription.Id,
                Event = payload.Event,
                Payload = payloadJson,
                Signature = signature,
                Status = WebhookDeliveryStatus.Pending,
                AttemptCount = 0,
                MaxAttempts = 5,
                CreatedAt = DateTime.UtcNow
            };

            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return delivery;
        }

        private async Task DeliverWebhookAsync(WebhookDelivery delivery)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(w => w.Id == delivery.WebhookSubscriptionId);

            if (subscription == null || !subscription.IsActive)
            {
                delivery.Status = WebhookDeliveryStatus.Failed;
                delivery.ErrorMessage = "Webhook subscription not found or inactive";
                await _context.SaveChangesAsync();
                return;
            }

            for (int attempt = 1; attempt <= delivery.MaxAttempts; attempt++)
            {
                delivery.AttemptCount = attempt;
                delivery.Status = attempt > 1 ? WebhookDeliveryStatus.Retrying : WebhookDeliveryStatus.Pending;

                try
                {
                    using var httpClient = _httpClientFactory.CreateClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(30);

                    var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url)
                    {
                        Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add("X-Webhook-Signature", delivery.Signature);
                    request.Headers.Add("X-Webhook-Event", delivery.Event.ToString());
                    request.Headers.Add("X-Webhook-Delivery", delivery.Id);

                    var response = await httpClient.SendAsync(request);
                    delivery.ResponseStatusCode = (int)response.StatusCode;
                    delivery.ResponseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        delivery.Status = WebhookDeliveryStatus.Delivered;
                        delivery.DeliveredAt = DateTime.UtcNow;
                        subscription.SuccessCount++;
                        subscription.LastTriggeredAt = DateTime.UtcNow;

                        _logger.LogInformation($"Webhook delivered successfully: {delivery.Id}");
                        break;
                    }
                    else
                    {
                        delivery.ErrorMessage = $"HTTP {delivery.ResponseStatusCode}: {delivery.ResponseBody}";
                        _logger.LogWarning($"Webhook delivery failed with status {delivery.ResponseStatusCode}: {delivery.Id}");
                    }
                }
                catch (Exception ex)
                {
                    delivery.ErrorMessage = ex.Message;
                    _logger.LogError(ex, $"Error delivering webhook: {delivery.Id}");
                }

                if (delivery.Status != WebhookDeliveryStatus.Delivered && attempt < delivery.MaxAttempts)
                {
                    // Exponential backoff: 5s, 15s, 45s, 135s
                    var delaySeconds = Math.Pow(3, attempt) * 5;
                    delivery.NextRetryAt = DateTime.UtcNow.AddSeconds(delaySeconds);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Scheduling retry {attempt + 1}/{delivery.MaxAttempts} in {delaySeconds}s for webhook: {delivery.Id}");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            if (delivery.Status != WebhookDeliveryStatus.Delivered)
            {
                delivery.Status = WebhookDeliveryStatus.MaxRetriesReached;
                subscription.FailureCount++;
                _logger.LogWarning($"Webhook delivery failed after {delivery.MaxAttempts} attempts: {delivery.Id}");
            }

            await _context.SaveChangesAsync();
        }

        private string GenerateSecret()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        private string GenerateSignature(string payload, string secret)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var payloadBytes = encoding.GetBytes(payload);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(payloadBytes);
                return "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private WebhookResponse MapToResponse(WebhookSubscription webhook)
        {
            return new WebhookResponse
            {
                Id = webhook.Id,
                Url = webhook.Url,
                Secret = webhook.Secret,
                IsActive = webhook.IsActive,
                Description = webhook.Description,
                Events = webhook.Events,
                CreatedAt = webhook.CreatedAt,
                UpdatedAt = webhook.UpdatedAt,
                LastTriggeredAt = webhook.LastTriggeredAt,
                SuccessCount = webhook.SuccessCount,
                FailureCount = webhook.FailureCount
            };
        }
    }
}

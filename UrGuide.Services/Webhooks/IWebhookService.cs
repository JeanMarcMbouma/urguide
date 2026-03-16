using System.Collections.Generic;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Webhooks;

namespace UrGuide.Services.Webhooks
{
    public interface IWebhookService
    {
        Task<Outcome<WebhookCreatedResponse>> RegisterWebhookAsync(string userId, RegisterWebhookRequest request);
        Task<Outcome<List<WebhookResponse>>> GetUserWebhooksAsync(string userId);
        Task<Outcome<WebhookResponse>> GetWebhookAsync(string webhookId, string userId);
        Task<Outcome<WebhookResponse>> UpdateWebhookAsync(string webhookId, string userId, UpdateWebhookRequest request);
        Task<Outcome<bool>> DeleteWebhookAsync(string webhookId, string userId);
        Task<Outcome<List<WebhookDeliveryResponse>>> GetWebhookDeliveriesAsync(string webhookId, string userId, int page = 1, int pageSize = 20);
        Task<Outcome<bool>> TestWebhookAsync(string webhookId, string userId, TestWebhookRequest request);
        Task PublishEventAsync(WebhookEvent eventType, object data);
    }
}

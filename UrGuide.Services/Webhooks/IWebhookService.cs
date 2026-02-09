using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Model.Webhooks;

namespace UrGuide.Services.Webhooks
{
    public interface IWebhookService
    {
        Task<WebhookResponse> RegisterWebhookAsync(string userId, RegisterWebhookRequest request);
        Task<List<WebhookResponse>> GetUserWebhooksAsync(string userId);
        Task<WebhookResponse> GetWebhookAsync(string webhookId, string userId);
        Task<WebhookResponse> UpdateWebhookAsync(string webhookId, string userId, UpdateWebhookRequest request);
        Task<bool> DeleteWebhookAsync(string webhookId, string userId);
        Task<List<WebhookDeliveryResponse>> GetWebhookDeliveriesAsync(string webhookId, string userId, int page = 1, int pageSize = 20);
        Task<bool> TestWebhookAsync(string webhookId, string userId, TestWebhookRequest request);
        Task PublishEventAsync(WebhookEvent eventType, object data);
    }
}

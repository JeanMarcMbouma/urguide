using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Webhooks;

namespace UrGuide.Services.Webhooks
{
    public interface IWebhookService
    {
        Task<Result<WebhookResponse>> RegisterWebhookAsync(string userId, RegisterWebhookRequest request);
        Task<Result<List<WebhookResponse>>> GetUserWebhooksAsync(string userId);
        Task<Result<WebhookResponse>> GetWebhookAsync(string webhookId, string userId);
        Task<Result<WebhookResponse>> UpdateWebhookAsync(string webhookId, string userId, UpdateWebhookRequest request);
        Task<Result<bool>> DeleteWebhookAsync(string webhookId, string userId);
        Task<Result<List<WebhookDeliveryResponse>>> GetWebhookDeliveriesAsync(string webhookId, string userId, int page = 1, int pageSize = 20);
        Task<Result<bool>> TestWebhookAsync(string webhookId, string userId, TestWebhookRequest request);
        Task PublishEventAsync(WebhookEvent eventType, object data);
    }
}

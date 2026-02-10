using System;
using UrGuide.Model.Webhooks;

namespace UrGuide.Data.Entities.Webhooks
{
    /// <summary>
    /// Represents a webhook delivery attempt with history and logs
    /// </summary>
    public class WebhookDelivery
    {
        public string Id { get; set; }
        public string WebhookSubscriptionId { get; set; }
        public virtual WebhookSubscription WebhookSubscription { get; set; }
        public WebhookEvent Event { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }
        public WebhookDeliveryStatus Status { get; set; }
        public int AttemptCount { get; set; }
        public int MaxAttempts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? NextRetryAt { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorMessage { get; set; }
    }
}


using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.Webhooks;

namespace UrGuide.Data.Entities.Webhooks
{
    /// <summary>
    /// Represents a registered webhook subscription
    /// </summary>
    public class WebhookSubscription
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string Url { get; set; }
        public string Secret { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public List<WebhookEvent> Events { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }
}


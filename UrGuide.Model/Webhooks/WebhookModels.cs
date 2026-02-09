using System;
using System.Collections.Generic;

namespace UrGuide.Model.Webhooks
{
    /// <summary>
    /// Available webhook events
    /// </summary>
    public enum WebhookEvent
    {
        // Payment events
        PaymentCreated = 1000,
        PaymentSucceeded = 1001,
        PaymentFailed = 1002,
        PaymentCancelled = 1003,
        PaymentRefunded = 1004,
        
        // Booking events
        BookingCreated = 2000,
        BookingConfirmed = 2001,
        BookingCancelled = 2002,
        BookingCompleted = 2003,
        
        // Tour events
        TourCreated = 3000,
        TourUpdated = 3001,
        TourDeleted = 3002,
        
        // User events
        UserRegistered = 4000,
        UserUpdated = 4001,
        
        // Review events
        ReviewCreated = 5000,
        ReviewUpdated = 5001
    }
    
    public enum WebhookDeliveryStatus
    {
        Pending = 0,
        Delivered = 1,
        Failed = 2,
        Retrying = 3,
        MaxRetriesReached = 4
    }

    public class RegisterWebhookRequest
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public List<WebhookEvent> Events { get; set; }
    }

    public class UpdateWebhookRequest
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public List<WebhookEvent> Events { get; set; }
        public bool? IsActive { get; set; }
    }

    public class WebhookResponse
    {
        public string Id { get; set; }
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

    public class WebhookDeliveryResponse
    {
        public string Id { get; set; }
        public string WebhookSubscriptionId { get; set; }
        public WebhookEvent Event { get; set; }
        public string Payload { get; set; }
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

    public class TestWebhookRequest
    {
        public string WebhookId { get; set; }
        public WebhookEvent Event { get; set; }
        public object SamplePayload { get; set; }
    }

    public class WebhookPayload
    {
        public string EventId { get; set; }
        public WebhookEvent Event { get; set; }
        public DateTime Timestamp { get; set; }
        public object Data { get; set; }
    }
}


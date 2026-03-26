using System;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Data.Entities.PushNotifications;

/// <summary>
/// Tracks push notification delivery attempts for auditing and analytics.
/// </summary>
public class PushNotificationLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public virtual User User { get; set; }
    public string DeviceRegistrationId { get; set; } = string.Empty;
    public virtual DeviceRegistration DeviceRegistration { get; set; }
    public DevicePlatform Platform { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Optional template identifier for future template support (Issue #163).
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;
}

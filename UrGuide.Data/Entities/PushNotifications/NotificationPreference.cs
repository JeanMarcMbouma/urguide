using System;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.PushNotifications;

/// <summary>
/// User preferences for push notification opt-in/opt-out management.
/// </summary>
public class NotificationPreference
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public virtual User User { get; set; }
    public bool PushEnabled { get; set; } = true;
    public bool TourUpdatesEnabled { get; set; } = true;
    public bool BookingAlertsEnabled { get; set; } = true;
    public bool ChatMessagesEnabled { get; set; } = true;
    public bool PromotionalEnabled { get; set; }
    public bool SystemAlertsEnabled { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

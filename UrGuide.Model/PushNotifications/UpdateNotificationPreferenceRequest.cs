namespace UrGuide.Model.PushNotifications;

/// <summary>
/// Request model for updating notification preferences.
/// </summary>
public class UpdateNotificationPreferenceRequest
{
    public bool PushEnabled { get; set; } = true;
    public bool TourUpdatesEnabled { get; set; } = true;
    public bool BookingAlertsEnabled { get; set; } = true;
    public bool ChatMessagesEnabled { get; set; } = true;
    public bool PromotionalEnabled { get; set; } = false;
    public bool SystemAlertsEnabled { get; set; } = true;
}

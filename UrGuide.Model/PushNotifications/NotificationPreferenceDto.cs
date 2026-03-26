namespace UrGuide.Model.PushNotifications;

/// <summary>
/// DTO for user notification preferences (opt-in/opt-out management).
/// </summary>
public class NotificationPreferenceDto
{
    public string UserId { get; set; } = string.Empty;
    public bool PushEnabled { get; set; } = true;
    public bool TourUpdatesEnabled { get; set; } = true;
    public bool BookingAlertsEnabled { get; set; } = true;
    public bool ChatMessagesEnabled { get; set; } = true;
    public bool PromotionalEnabled { get; set; } = false;
    public bool SystemAlertsEnabled { get; set; } = true;
}

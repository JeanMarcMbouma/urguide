namespace UrGuide.Model.PushNotifications;

/// <summary>
/// Result DTO for push notification delivery tracking.
/// </summary>
public class PushNotificationResultDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DeliveryStatus Status { get; set; }
    public string ErrorMessage { get; set; }
    public string SentAt { get; set; } = string.Empty;
    public string DeliveredAt { get; set; }
}

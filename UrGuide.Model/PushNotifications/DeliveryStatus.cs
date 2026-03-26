namespace UrGuide.Model.PushNotifications;

/// <summary>
/// Status of a push notification delivery attempt.
/// </summary>
public enum DeliveryStatus
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Failed = 3,
    Expired = 4
}

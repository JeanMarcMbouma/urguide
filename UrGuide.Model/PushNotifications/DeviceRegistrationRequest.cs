namespace UrGuide.Model.PushNotifications;

/// <summary>
/// Request model for registering a device for push notifications.
/// </summary>
public class DeviceRegistrationRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string DeviceName { get; set; }
    public string AppVersion { get; set; }
}

namespace UrGuide.Model.PushNotifications;

/// <summary>
/// DTO representing a registered device.
/// </summary>
public class DeviceRegistrationDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string DeviceName { get; set; }
    public string AppVersion { get; set; }
    public bool IsActive { get; set; }
    public string RegisteredAt { get; set; } = string.Empty;
    public string LastUsedAt { get; set; }
}

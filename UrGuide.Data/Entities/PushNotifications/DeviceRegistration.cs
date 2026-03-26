using System;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Data.Entities.PushNotifications;

/// <summary>
/// Represents a registered device for push notifications.
/// </summary>
public class DeviceRegistration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public virtual User User { get; set; }
    public string DeviceToken { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
}

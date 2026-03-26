using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.PushNotifications;

/// <summary>
/// Interface for platform-specific push notification providers (APNs, FCM).
/// </summary>
public interface IPushNotificationProvider
{
    DevicePlatform Platform { get; }
    Task<PushNotificationDeliveryResult> SendAsync(string deviceToken, string title, string body, string imageUrl, string actionUrl, Dictionary<string, string> data, CancellationToken cancellationToken);
}

/// <summary>
/// Internal result from a push notification delivery attempt.
/// </summary>
public class PushNotificationDeliveryResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DeliveryStatus Status { get; set; }
}

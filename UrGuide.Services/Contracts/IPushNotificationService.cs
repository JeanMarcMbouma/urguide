using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.Contracts;

/// <summary>
/// Service contract for managing push notifications, device registrations,
/// and notification preferences.
/// </summary>
public interface IPushNotificationService
{
    Task<Outcome<DeviceRegistrationDto>> RegisterDeviceAsync(DeviceRegistrationRequest request, CancellationToken ct);
    Task<Outcome<bool>> UnregisterDeviceAsync(string deviceId, CancellationToken ct);
    Task<Outcome<List<DeviceRegistrationDto>>> GetUserDevicesAsync(CancellationToken ct);
    Task<Outcome<List<PushNotificationResultDto>>> SendPushNotificationAsync(SendPushNotificationRequest request, CancellationToken ct);
    Task<Outcome<PushNotificationResultDto>> GetDeliveryStatusAsync(string notificationId, CancellationToken ct);
    Task<Outcome<NotificationPreferenceDto>> GetNotificationPreferencesAsync(CancellationToken ct);
    Task<Outcome<NotificationPreferenceDto>> UpdateNotificationPreferencesAsync(UpdateNotificationPreferenceRequest request, CancellationToken ct);
}

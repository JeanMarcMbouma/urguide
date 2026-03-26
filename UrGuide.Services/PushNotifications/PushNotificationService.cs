using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BbQ.Outcome;
using UrGuide.Data;
using UrGuide.Model.PushNotifications;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.PushNotifications;

class PushNotificationService : IPushNotificationService
{
    public PushNotificationService(
        UrGuideContext context,
        IValidator<DeviceRegistrationRequest> validator,
        ILogger<PushNotificationService> logger,
        IUserContext userContext,
        IEnumerable<IPushNotificationProvider> providers)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        Providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public UrGuideContext Context { get; }
    public IValidator<DeviceRegistrationRequest> Validator { get; }
    public ILogger<PushNotificationService> Logger { get; }
    public IUserContext UserContext { get; }
    public IEnumerable<IPushNotificationProvider> Providers { get; }

    public async Task<Outcome<DeviceRegistrationDto>> RegisterDeviceAsync(DeviceRegistrationRequest request, CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<DeviceRegistrationDto>().WithErrors(ErrorMessages.NotAuthenticated);

        var validationResult = await Validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Of<DeviceRegistrationDto>().WithErrors(
                validationResult.Errors.Select(e => e.ErrorMessage).ToArray());

        // Check if the device token already exists for this user and update it
        var existing = await Context.DeviceRegistrations
            .FirstOrDefaultAsync(d => d.UserId == UserContext.UserId && d.DeviceToken == request.DeviceToken, ct);

        if (existing != null)
        {
            existing.Platform = request.Platform;
            existing.DeviceName = request.DeviceName;
            existing.AppVersion = request.AppVersion;
            existing.IsActive = true;
            existing.LastUsedAt = DateTime.UtcNow;
        }
        else
        {
            existing = new Data.Entities.PushNotifications.DeviceRegistration
            {
                UserId = UserContext.UserId,
                DeviceToken = request.DeviceToken,
                Platform = request.Platform,
                DeviceName = request.DeviceName,
                AppVersion = request.AppVersion,
                IsActive = true,
                RegisteredAt = DateTime.UtcNow
            };
            Context.DeviceRegistrations.Add(existing);
        }

        await Context.SaveChangesAsync(ct);
        Logger.LogInformation("Device registered for user {UserId}, platform {Platform}", UserContext.UserId, request.Platform);

        return Result.Of(MapToDeviceDto(existing));
    }

    public async Task<Outcome<bool>> UnregisterDeviceAsync(string deviceId, CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

        var device = await Context.DeviceRegistrations
            .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == UserContext.UserId, ct);

        if (device == null)
            return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

        device.IsActive = false;
        await Context.SaveChangesAsync(ct);
        Logger.LogInformation("Device {DeviceId} unregistered for user {UserId}", deviceId, UserContext.UserId);

        return Result.Of(true);
    }

    public async Task<Outcome<List<DeviceRegistrationDto>>> GetUserDevicesAsync(CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<List<DeviceRegistrationDto>>().WithErrors(ErrorMessages.NotAuthenticated);

        var devices = await Context.DeviceRegistrations
            .Where(d => d.UserId == UserContext.UserId && d.IsActive)
            .ToListAsync(ct);

        return Result.Of(devices.Select(MapToDeviceDto).ToList());
    }

    public async Task<Outcome<List<PushNotificationResultDto>>> SendPushNotificationAsync(SendPushNotificationRequest request, CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<List<PushNotificationResultDto>>().WithErrors(ErrorMessages.NotAuthenticated);

        if (string.IsNullOrEmpty(request.UserId))
            return Result.Of<List<PushNotificationResultDto>>().WithErrors("UserId is required.");

        if (string.IsNullOrEmpty(request.Title) && string.IsNullOrEmpty(request.Body))
            return Result.Of<List<PushNotificationResultDto>>().WithErrors("Title or Body is required.");

        // Check notification preferences
        var preferences = await Context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (preferences != null)
        {
            if (!preferences.PushEnabled)
            {
                Logger.LogInformation("Push notifications disabled for user {UserId}", request.UserId);
                return Result.Of(new List<PushNotificationResultDto>());
            }

            // Category-specific preference checks
            if (!string.IsNullOrEmpty(request.Category))
            {
                var categoryLower = request.Category.ToLowerInvariant();
                var categoryDisabled = categoryLower switch
                {
                    "tour_updates" or "tourupdates" => !preferences.TourUpdatesEnabled,
                    "booking_alerts" or "bookingalerts" => !preferences.BookingAlertsEnabled,
                    "chat_messages" or "chatmessages" or "chat" => !preferences.ChatMessagesEnabled,
                    "promotional" or "promotions" => !preferences.PromotionalEnabled,
                    "system" or "system_alerts" => !preferences.SystemAlertsEnabled,
                    _ => false
                };

                if (categoryDisabled)
                {
                    Logger.LogInformation("{Category} notifications disabled for user {UserId}", request.Category, request.UserId);
                    return Result.Of(new List<PushNotificationResultDto>());
                }
            }
        }

        var devices = await Context.DeviceRegistrations
            .Where(d => d.UserId == request.UserId && d.IsActive)
            .ToListAsync(ct);

        if (devices.Count == 0)
        {
            Logger.LogInformation("No active devices found for user {UserId}", request.UserId);
            return Result.Of(new List<PushNotificationResultDto>());
        }

        var results = new List<PushNotificationResultDto>();
        var notificationRecords = new List<(Data.Entities.PushNotifications.PushNotificationLog Log, Data.Entities.PushNotifications.DeviceRegistration Device)>();

        foreach (var device in devices)
        {
            var provider = Providers.FirstOrDefault(p => p.Platform == device.Platform);
            if (provider == null)
            {
                Logger.LogWarning("No push notification provider found for platform {Platform}", device.Platform);
                continue;
            }

            var log = new Data.Entities.PushNotifications.PushNotificationLog
            {
                UserId = request.UserId,
                DeviceRegistrationId = device.Id,
                Platform = device.Platform,
                Title = request.Title ?? string.Empty,
                Body = request.Body ?? string.Empty,
                Status = DeliveryStatus.Pending,
                SentAt = DateTime.UtcNow,
                TemplateId = request.TemplateId ?? string.Empty
            };
            Context.PushNotificationLogs.Add(log);
            notificationRecords.Add((log, device));

            var deliveryResult = await provider.SendAsync(
                device.DeviceToken,
                request.Title,
                request.Body,
                request.ImageUrl,
                request.ActionUrl,
                request.Data,
                ct);

            log.Status = deliveryResult.Status;
            log.ErrorMessage = deliveryResult.ErrorMessage ?? string.Empty;
            if (deliveryResult.Status == DeliveryStatus.Delivered)
                log.DeliveredAt = DateTime.UtcNow;
        }

        await Context.SaveChangesAsync(ct);

        foreach (var (log, device) in notificationRecords)
        {
            results.Add(MapToResultDto(log, device));
        }

        return Result.Of(results);
    }

    public async Task<Outcome<PushNotificationResultDto>> GetDeliveryStatusAsync(string notificationId, CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<PushNotificationResultDto>().WithErrors(ErrorMessages.NotAuthenticated);

        var log = await Context.PushNotificationLogs
            .Include(l => l.DeviceRegistration)
            .FirstOrDefaultAsync(
                l => l.Id == notificationId && l.DeviceRegistration.UserId == UserContext.UserId,
                ct);

        if (log == null)
            return Result.Of<PushNotificationResultDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

        return Result.Of(MapToResultDto(log, log.DeviceRegistration));
    }

    public async Task<Outcome<NotificationPreferenceDto>> GetNotificationPreferencesAsync(CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<NotificationPreferenceDto>().WithErrors(ErrorMessages.NotAuthenticated);

        var preference = await Context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == UserContext.UserId, ct);

        if (preference == null)
        {
            // Return defaults when no preferences have been saved yet
            return Result.Of(new NotificationPreferenceDto
            {
                UserId = UserContext.UserId,
                PushEnabled = true,
                TourUpdatesEnabled = true,
                BookingAlertsEnabled = true,
                ChatMessagesEnabled = true,
                PromotionalEnabled = false,
                SystemAlertsEnabled = true
            });
        }

        return Result.Of(MapToPreferenceDto(preference));
    }

    public async Task<Outcome<NotificationPreferenceDto>> UpdateNotificationPreferencesAsync(UpdateNotificationPreferenceRequest request, CancellationToken ct)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<NotificationPreferenceDto>().WithErrors(ErrorMessages.NotAuthenticated);

        var preference = await Context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == UserContext.UserId, ct);

        if (preference == null)
        {
            preference = new Data.Entities.PushNotifications.NotificationPreference
            {
                UserId = UserContext.UserId
            };
            Context.NotificationPreferences.Add(preference);
        }

        preference.PushEnabled = request.PushEnabled;
        preference.TourUpdatesEnabled = request.TourUpdatesEnabled;
        preference.BookingAlertsEnabled = request.BookingAlertsEnabled;
        preference.ChatMessagesEnabled = request.ChatMessagesEnabled;
        preference.PromotionalEnabled = request.PromotionalEnabled;
        preference.SystemAlertsEnabled = request.SystemAlertsEnabled;
        preference.UpdatedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync(ct);
        Logger.LogInformation("Notification preferences updated for user {UserId}", UserContext.UserId);

        return Result.Of(MapToPreferenceDto(preference));
    }

    private static DeviceRegistrationDto MapToDeviceDto(Data.Entities.PushNotifications.DeviceRegistration entity)
    {
        return new DeviceRegistrationDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            DeviceToken = entity.DeviceToken,
            Platform = entity.Platform,
            DeviceName = entity.DeviceName,
            AppVersion = entity.AppVersion,
            IsActive = entity.IsActive,
            RegisteredAt = entity.RegisteredAt.ToString("O"),
            LastUsedAt = entity.LastUsedAt?.ToString("O")
        };
    }

    private static PushNotificationResultDto MapToResultDto(
        Data.Entities.PushNotifications.PushNotificationLog log,
        Data.Entities.PushNotifications.DeviceRegistration device)
    {
        return new PushNotificationResultDto
        {
            Id = log.Id,
            UserId = log.UserId,
            DeviceId = device?.Id ?? string.Empty,
            Platform = log.Platform,
            Title = log.Title,
            Body = log.Body,
            Status = log.Status,
            ErrorMessage = log.ErrorMessage,
            SentAt = log.SentAt.ToString("O"),
            DeliveredAt = log.DeliveredAt?.ToString("O")
        };
    }

    private static NotificationPreferenceDto MapToPreferenceDto(Data.Entities.PushNotifications.NotificationPreference entity)
    {
        return new NotificationPreferenceDto
        {
            UserId = entity.UserId,
            PushEnabled = entity.PushEnabled,
            TourUpdatesEnabled = entity.TourUpdatesEnabled,
            BookingAlertsEnabled = entity.BookingAlertsEnabled,
            ChatMessagesEnabled = entity.ChatMessagesEnabled,
            PromotionalEnabled = entity.PromotionalEnabled,
            SystemAlertsEnabled = entity.SystemAlertsEnabled
        };
    }
}

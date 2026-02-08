using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Services.Contracts;
using UrGuide.Model.Users;

namespace UrGuide.WebApp.MessageQueue.Services;

/// <summary>
/// Notification service decorator that publishes notification messages to the message queue
/// </summary>
public class QueuedNotificationService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<QueuedNotificationService> _logger;

    public QueuedNotificationService(IPublishEndpoint publishEndpoint, ILogger<QueuedNotificationService> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task NotifyAsync(CreateNotification createNotification)
    {
        try
        {
            _logger.LogInformation("Queuing notification for user {UserId}", createNotification.UserId);

            var message = new SendNotificationMessage
            {
                UserId = createNotification.UserId,
                AuthorId = createNotification.AuthorId,
                Content = createNotification.Content,
                ReferenceLink = createNotification.ReferenceLink,
                IsSystem = createNotification.IsSystem,
                QueuedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(message);
            
            _logger.LogInformation("Notification queued successfully for user {UserId}", createNotification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue notification for user {UserId}", createNotification.UserId);
            throw;
        }
    }

    public Task SystemNotifyAsync(string userId, string content, string? referenceLink)
    {
        // Note: This matches UrGuide.Services.Constants.SystemUserId
        // Using hardcoded value here since Constants class is internal
        const string SystemUserId = "00000000-0000-0000-0000-000000000000";
        
        return NotifyAsync(new CreateNotification
        {
            AuthorId = SystemUserId,
            Content = content,
            ReferenceLink = referenceLink,
            IsSystem = true,
            UserId = userId
        });
    }
}

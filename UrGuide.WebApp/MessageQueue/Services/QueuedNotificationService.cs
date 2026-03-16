using System;
using BbQ.Outcome;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Services.Contracts;
using UrGuide.Model.Users;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;

namespace UrGuide.WebApp.MessageQueue.Services;

/// <summary>
/// Notification service decorator that publishes notification messages to the message queue
/// Implements IUserNotificationService and delegates read operations to the synchronous service
/// </summary>
public class QueuedNotificationService : IUserNotificationService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUserNotificationService _synchronousNotificationService;
    private readonly ILogger<QueuedNotificationService> _logger;

    public QueuedNotificationService(
        IPublishEndpoint publishEndpoint,
        IUserNotificationService synchronousNotificationService,
        ILogger<QueuedNotificationService> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _synchronousNotificationService = synchronousNotificationService ?? throw new ArgumentNullException(nameof(synchronousNotificationService));
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

    // Read operations are delegated to the synchronous service
    // These operations don't benefit from queuing and need immediate results

    public Task<Outcome<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken)
    {
        return _synchronousNotificationService.MarkAsReadAsync(notificationId, cancellationToken);
    }

    public Task<Outcome<Notification>> GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
    {
        return _synchronousNotificationService.GetNotificationAsync(notificationId, cancellationToken);
    }

    public Task<Outcome<PagedList<Notification>>> GetUnreadAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        return _synchronousNotificationService.GetUnreadAsync(pagination, cancellationToken);
    }

    public Task<Outcome<PagedList<Notification>>> GetAllAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        return _synchronousNotificationService.GetAllAsync(pagination, cancellationToken);
    }
}

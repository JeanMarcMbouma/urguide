using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Shared.Contracts;
using UrGuide.Model.Messages;

namespace UrGuide.WebApp.MessageQueue.Services;

/// <summary>
/// Queue-based email service that publishes messages to the message queue
/// </summary>
public class QueuedEmailService : IEmailService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<QueuedEmailService> _logger;

    public QueuedEmailService(IPublishEndpoint publishEndpoint, ILogger<QueuedEmailService> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendAsync(SendDirectMessageCommand message)
    {
        try
        {
            _logger.LogInformation("Queuing email message for {To}", message.To);

            var emailMessage = new SendEmailMessage
            {
                To = message.To,
                ToName = message.ToName,
                Subject = message.Subject,
                Content = message.Content,
                Link = message.Link,
                LinkText = message.LinkText,
                QueuedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(emailMessage);
            
            _logger.LogInformation("Email message queued successfully for {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue email message for {To}", message.To);
            throw;
        }
    }
}

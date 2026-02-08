using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.WebApp.Services;

namespace UrGuide.WebApp.MessageQueue.Consumers;

/// <summary>
/// Consumer for processing email messages from the queue
/// Uses concrete EmailService to avoid circular dependency with QueuedEmailService
/// </summary>
public class SendEmailConsumer : IConsumer<SendEmailMessage>
{
    private readonly EmailService _emailService;
    private readonly ILogger<SendEmailConsumer> _logger;

    public SendEmailConsumer(EmailService emailService, ILogger<SendEmailConsumer> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Processing email message for {To} with subject: {Subject}", 
                message.To, message.Subject);

            await _emailService.SendAsync(new Model.Messages.SendDirectMessageCommand
            {
                To = message.To,
                ToName = message.ToName,
                Subject = message.Subject,
                Content = message.Content,
                Link = message.Link,
                LinkText = message.LinkText
            });

            _logger.LogInformation("Successfully sent email to {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw; // This will trigger the retry policy
        }
    }
}

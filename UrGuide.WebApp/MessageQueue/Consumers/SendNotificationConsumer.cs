using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Data;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.MessageQueue.Consumers;

/// <summary>
/// Consumer for processing notification messages from the queue
/// </summary>
public class SendNotificationConsumer : IConsumer<SendNotificationMessage>
{
    private readonly UrGuideContext _context;
    private readonly IInstantMessagingService _instantMessaging;
    private readonly IMapper _mapper;
    private readonly ILogger<SendNotificationConsumer> _logger;

    public SendNotificationConsumer(
        UrGuideContext context,
        IInstantMessagingService instantMessaging,
        IMapper mapper,
        ILogger<SendNotificationConsumer> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _instantMessaging = instantMessaging ?? throw new ArgumentNullException(nameof(instantMessaging));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<SendNotificationMessage> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Processing notification for user {UserId}", message.UserId);

            var user = await _context.Users.FindAsync(new[] { message.UserId });
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", message.UserId);
                return;
            }

            var sender = await _context.Users.FindAsync(new[] { message.AuthorId });
            
            var notification = new UrGuide.Data.Entities.Users.Notification
            {
                Content = message.Content,
                ReferenceLink = message.ReferenceLink,
                IsSystem = message.IsSystem,
                Created = DateTime.UtcNow,
                Sender = sender,
                Read = false
            };
            
            user.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send real-time notification via SignalR
            _ = _instantMessaging.Send(user.Id, _mapper.Map<Model.Users.Notification>(notification))
                .ConfigureAwait(false);

            _logger.LogInformation("Successfully sent notification to user {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", message.UserId);
            throw; // This will trigger the retry policy
        }
    }
}

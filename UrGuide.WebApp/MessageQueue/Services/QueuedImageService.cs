using System;
using System.Collections.Generic;
using MassTransit;
using Microsoft.Extensions.Logging;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Services.Contracts;
using UrGuide.Model.Shared;

namespace UrGuide.WebApp.MessageQueue.Services;

/// <summary>
/// Queue-based image service that publishes image processing messages to the message queue
/// </summary>
public class QueuedImageService : IImageService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<QueuedImageService> _logger;

    public QueuedImageService(IPublishEndpoint publishEndpoint, ILogger<QueuedImageService> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void SaveImage(UrGuide.Data.Entities.Shared.Image imageFile)
    {
        try
        {
            _logger.LogInformation("Queuing image processing for image {ImageId}", imageFile.Id);

            var message = new ProcessImageMessage
            {
                ImageId = imageFile.Id,
                Base64Image = string.Empty, // Image data will be loaded from database in consumer
                ProcessingType = ImageProcessingType.PostImage,
                QueuedAt = DateTime.UtcNow
            };

            // Fire and forget - we don't await this because:
            // 1. SaveImage is a void method in the IImageService interface (non-breaking change requirement)
            // 2. The message is published to RabbitMQ which has its own persistence and retry mechanisms
            // 3. MassTransit handles the publish operation asynchronously in the background
            // 4. Failures will be retried by RabbitMQ according to the configured retry policy
            _ = _publishEndpoint.Publish(message);
            
            _logger.LogInformation("Image processing queued successfully for {ImageId}", imageFile.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue image processing for {ImageId}", imageFile.Id);
            throw;
        }
    }

    public string SaveAvatar(string userId, ImageFileModel? imageFile = null)
    {
        try
        {
            _logger.LogInformation("Queuing avatar processing for user {UserId}", userId);

            var message = new ProcessImageMessage
            {
                ImageId = Guid.NewGuid().ToString(),
                Base64Image = imageFile?.ImageBase64 ?? string.Empty,
                ProcessingType = ImageProcessingType.Avatar,
                UserId = userId,
                QueuedAt = DateTime.UtcNow
            };

            // Fire and forget - we don't await this because:
            // 1. SaveAvatar returns a string (non-breaking change requirement)
            // 2. The message is published to RabbitMQ which has its own persistence
            // 3. We return a placeholder URL that will be updated once processing completes
            _ = _publishEndpoint.Publish(message);
            
            _logger.LogInformation("Avatar processing queued successfully for user {UserId}", userId);
            
            // Return a placeholder URL that will be updated once processing completes
            return $"/images/processing/{userId}.png";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue avatar processing for user {UserId}", userId);
            throw;
        }
    }

    public void DeleteImage(UrGuide.Data.Entities.Shared.Image image)
    {
        // Image deletion is not queued as it needs to be immediate
        _logger.LogWarning("DeleteImage called on QueuedImageService - not implemented for async processing");
        throw new NotImplementedException("Image deletion should use the synchronous ImageService");
    }

    public void DeleteImages(ICollection<UrGuide.Data.Entities.Shared.Image> images)
    {
        // Image deletion is not queued as it needs to be immediate
        _logger.LogWarning("DeleteImages called on QueuedImageService - not implemented for async processing");
        throw new NotImplementedException("Image deletion should use the synchronous ImageService");
    }
}

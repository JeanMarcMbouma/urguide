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
/// Deletion operations are not queued and will throw NotSupportedException
/// </summary>
public class QueuedImageService : IImageService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<QueuedImageService> _logger;

    public QueuedImageService(
        IPublishEndpoint publishEndpoint,
        ILogger<QueuedImageService> logger)
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

            // Synchronously publish to ensure message is queued before method returns
            // This avoids potential message loss on fire-and-forget
            var publishTask = _publishEndpoint.Publish(message);
            publishTask.Wait(TimeSpan.FromSeconds(5));
            
            if (!publishTask.IsCompleted)
            {
                _logger.LogWarning("Publish operation timed out for image {ImageId}", imageFile.Id);
            }
            
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

            // Synchronously publish and wait for a short timeout
            var publishTask = _publishEndpoint.Publish(message);
            var completed = publishTask.Wait(TimeSpan.FromSeconds(5));
            
            if (!completed)
            {
                _logger.LogWarning("Publish operation timed out for avatar processing for user {UserId}", userId);
            }
            
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
        // Image deletion is not supported in queued mode - it must be immediate
        // Code paths that delete images should check if queued services are enabled
        // and use synchronous deletion if needed
        _logger.LogError("DeleteImage called on QueuedImageService for image {ImageId} - operation not supported in queued mode", image?.Id);
        throw new NotSupportedException(
            "Image deletion is not supported in queued mode. " +
            "Use synchronous ImageService for immediate deletion operations.");
    }

    public void DeleteImages(ICollection<UrGuide.Data.Entities.Shared.Image> images)
    {
        // Image deletion is not supported in queued mode - it must be immediate
        // Code paths that delete images should check if queued services are enabled
        // and use synchronous deletion if needed
        _logger.LogError("DeleteImages called on QueuedImageService for {Count} images - operation not supported in queued mode", images?.Count ?? 0);
        throw new NotSupportedException(
            "Image deletion is not supported in queued mode. " +
            "Use synchronous ImageService for immediate deletion operations.");
    }
}

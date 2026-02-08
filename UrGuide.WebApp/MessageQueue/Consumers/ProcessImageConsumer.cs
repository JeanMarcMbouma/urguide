using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UrGuide.WebApp.MessageQueue.Messages;
using UrGuide.Services.Contracts;
using UrGuide.Data;

namespace UrGuide.WebApp.MessageQueue.Consumers;

/// <summary>
/// Consumer for processing image processing messages from the queue
/// </summary>
public class ProcessImageConsumer : IConsumer<ProcessImageMessage>
{
    private readonly IImageService _imageService;
    private readonly ILogger<ProcessImageConsumer> _logger;
    private readonly UrGuideContext _context;

    public ProcessImageConsumer(
        IImageService imageService, 
        ILogger<ProcessImageConsumer> logger,
        UrGuideContext context)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Consume(ConsumeContext<ProcessImageMessage> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Processing image {ImageId} of type {ProcessingType}", 
                message.ImageId, message.ProcessingType);

            switch (message.ProcessingType)
            {
                case ImageProcessingType.PostImage:
                case ImageProcessingType.CatalogImage:
                    // For post images, they should be processed directly by the ImageService
                    // when they are created, not asynchronously.
                    // This path would be used if we want to defer processing
                    _logger.LogInformation("Post image processing deferred for {ImageId}", message.ImageId);
                    break;

                case ImageProcessingType.Avatar:
                    if (!string.IsNullOrEmpty(message.UserId) && !string.IsNullOrEmpty(message.Base64Image))
                    {
                        var imageUrl = _imageService.SaveAvatar(message.UserId, 
                            new Model.Shared.ImageFileModel { ImageBase64 = message.Base64Image });
                        
                        // NOTE: The avatar URL is generated but not automatically persisted to database.
                        // The User entity in this codebase doesn't have a direct ProfilePictureUrl property.
                        // Avatar images are stored on disk and referenced via the ImageUrl pattern.
                        // Full implementation would require:
                        // 1. Adding a ProfilePictureUrl property to User entity, OR
                        // 2. Using generic attributes to store the URL, OR
                        // 3. Creating a separate UserProfile table
                        
                        _logger.LogInformation("Successfully processed avatar for user {UserId}, URL: {ImageUrl}", 
                            message.UserId, imageUrl);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process image {ImageId}", message.ImageId);
            throw; // This will trigger the retry policy
        }
    }
}

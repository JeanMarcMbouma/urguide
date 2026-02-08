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
                        
                        // Update the user's ProfileImage in the database
                        var user = await _context.Users.FindAsync(new[] { message.UserId });
                        if (user != null)
                        {
                            if (user.ProfileImage == null)
                            {
                                user.ProfileImage = new UrGuide.Data.Entities.Users.Image
                                {
                                    ImageUrl = imageUrl
                                };
                            }
                            else
                            {
                                user.ProfileImage.ImageUrl = imageUrl;
                            }
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Successfully processed and persisted avatar for user {UserId}, URL: {ImageUrl}", 
                                message.UserId, imageUrl);
                        }
                        else
                        {
                            _logger.LogWarning("User {UserId} not found, avatar URL not persisted", message.UserId);
                        }
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

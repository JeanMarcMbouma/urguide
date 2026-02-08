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
                    // For post and catalog images, processing is expected to happen synchronously
                    // when they are created. If messages of these types reach this consumer, it
                    // indicates a misconfiguration or missing implementation of asynchronous
                    // processing for these types.
                    _logger.LogError(
                        "Unsupported image processing type {ProcessingType} for image {ImageId} in {Consumer}. " +
                        "Post and catalog images must be processed synchronously or have explicit async handling implemented.",
                        message.ProcessingType,
                        message.ImageId,
                        nameof(ProcessImageConsumer));
                    throw new NotSupportedException(
                        $"Processing type '{message.ProcessingType}' is not supported by {nameof(ProcessImageConsumer)}.");

                case ImageProcessingType.Avatar:
                    if (string.IsNullOrEmpty(message.UserId) || string.IsNullOrEmpty(message.Base64Image))
                    {
                        _logger.LogWarning(
                            "Invalid avatar processing message for ImageId {ImageId}: missing required fields (UserIdMissing={UserIdMissing}, Base64ImageMissing={Base64ImageMissing}). UserId: {UserId}",
                            message.ImageId,
                            string.IsNullOrEmpty(message.UserId),
                            string.IsNullOrEmpty(message.Base64Image),
                            message.UserId);
                        // Throw to move message to error queue for investigation
                        throw new InvalidOperationException(
                            $"Invalid avatar processing message: UserId and Base64Image are required. ImageId: {message.ImageId}");
                    }
                    
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
                        _logger.LogWarning(
                            "Invalid avatar processing message for ImageId {ImageId}: missing required fields (UserIdMissing={UserIdMissing}, Base64ImageMissing={Base64ImageMissing}). UserId: {UserId}",
                            message.ImageId,
                            string.IsNullOrEmpty(message.UserId),
                            string.IsNullOrEmpty(message.Base64Image),
                            message.UserId);
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

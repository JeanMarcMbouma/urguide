using System;

namespace UrGuide.WebApp.MessageQueue.Messages;

/// <summary>
/// Message contract for asynchronous image processing
/// </summary>
public record ProcessImageMessage
{
    public string ImageId { get; init; } = string.Empty;
    public string Base64Image { get; init; } = string.Empty;
    public ImageProcessingType ProcessingType { get; init; }
    public string? UserId { get; init; }
    public DateTime QueuedAt { get; init; } = DateTime.UtcNow;
}

public enum ImageProcessingType
{
    PostImage,
    Avatar,
    CatalogImage
}

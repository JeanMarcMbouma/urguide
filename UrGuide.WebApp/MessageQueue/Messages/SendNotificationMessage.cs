using System;

namespace UrGuide.WebApp.MessageQueue.Messages;

/// <summary>
/// Message contract for asynchronous notification dispatch
/// </summary>
public record SendNotificationMessage
{
    public string UserId { get; init; } = string.Empty;
    public string AuthorId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? ReferenceLink { get; init; }
    public bool IsSystem { get; init; }
    public DateTime QueuedAt { get; init; } = DateTime.UtcNow;
}

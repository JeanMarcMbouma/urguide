using System;

namespace UrGuide.WebApp.MessageQueue.Messages;

/// <summary>
/// Message contract for asynchronous email sending
/// </summary>
public record SendEmailMessage
{
    public string To { get; init; } = string.Empty;
    public string ToName { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Link { get; init; }
    public string? LinkText { get; init; }
    public DateTime QueuedAt { get; init; } = DateTime.UtcNow;
}

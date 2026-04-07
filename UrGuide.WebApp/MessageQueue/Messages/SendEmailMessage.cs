using System;
using System.Collections.Generic;

namespace UrGuide.WebApp.MessageQueue.Messages;

/// <summary>
/// Message contract for asynchronous email sending.
/// When <see cref="TemplateName"/> is set the consumer will resolve the named
/// admin-managed template and render it before delivery.
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

    /// <summary>
    /// BCP 47 language tag (e.g. "en", "fr"). Defaults to "en".
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Name of the admin-managed email template. When supplied the consumer
    /// renders the template with <see cref="TemplateVariables"/> before sending.
    /// </summary>
    public string? TemplateName { get; init; }

    /// <summary>
    /// Key/value pairs substituted into the template using {{Key}} syntax.
    /// </summary>
    public Dictionary<string, string>? TemplateVariables { get; init; }
}

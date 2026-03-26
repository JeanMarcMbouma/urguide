using System.Collections.Generic;

namespace UrGuide.Model.PushNotifications;

/// <summary>
/// Request model for sending a push notification.
/// Designed to be extensible for future template support (Issue #163).
/// </summary>
public class SendPushNotificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ImageUrl { get; set; }
    public string ActionUrl { get; set; }
    public string Category { get; set; }
    public Dictionary<string, string> Data { get; set; }

    /// <summary>
    /// Optional template ID for future template support (Issue #163).
    /// When set, Title and Body may be overridden by the template engine.
    /// </summary>
    public string TemplateId { get; set; }

    /// <summary>
    /// Template variables for future template support (Issue #163).
    /// </summary>
    public Dictionary<string, string> TemplateVariables { get; set; }
}

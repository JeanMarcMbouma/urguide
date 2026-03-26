using System;

namespace UrGuide.Data.Entities.PushNotifications;

/// <summary>
/// Reusable push notification template with multi-language and A/B testing support.
/// Variable placeholders use the {{variable_name}} syntax.
/// </summary>
public class NotificationTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Unique logical name for the template (e.g. "booking_confirmed").
    /// Multiple records can share the same Name for different languages/versions.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Notification category (e.g. tour_updates, booking_alerts, promotional).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// ISO 639-1 language code (e.g. "en", "fr", "es", "de", "ar").
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Monotonically increasing version number.
    /// Incremented each time the template content is edited.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Title template string. Supports {{variable_name}} placeholders.
    /// </summary>
    public string TitleTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Body template string. Supports {{variable_name}} placeholders.
    /// </summary>
    public string BodyTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Optional static image URL to include with the notification.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Optional deep-link / action URL opened when the notification is tapped.
    /// </summary>
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is the active (current) version for its Name + Language combination.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// A/B testing variant label (e.g. "A", "B", or empty for non-variant).
    /// </summary>
    public string VariantGroup { get; set; } = string.Empty;

    /// <summary>
    /// UserId of the admin who created this template.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

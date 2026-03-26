using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.PushNotifications;

public class CreateNotificationTemplateRequest
{
    /// <summary>
    /// Logical name used to look up the template (e.g. "booking_confirmed").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Notification category (e.g. tour_updates, booking_alerts, promotional).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// ISO 639-1 language code (defaults to "en").
    /// </summary>
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Title template. Use {{variable_name}} for variable substitution.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string TitleTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Body template. Use {{variable_name}} for variable substitution.
    /// </summary>
    [Required]
    [MaxLength(4000)]
    public string BodyTemplate { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// A/B testing variant label (e.g. "A", "B").
    /// </summary>
    [MaxLength(50)]
    public string VariantGroup { get; set; } = string.Empty;
}

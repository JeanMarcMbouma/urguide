using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.PushNotifications;

public class UpdateNotificationTemplateRequest
{
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
    /// Whether this template is the active version.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// A/B testing variant label (e.g. "A", "B").
    /// </summary>
    [MaxLength(50)]
    public string VariantGroup { get; set; } = string.Empty;
}

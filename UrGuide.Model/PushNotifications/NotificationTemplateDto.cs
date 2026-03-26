namespace UrGuide.Model.PushNotifications;

public class NotificationTemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int Version { get; set; } = 1;
    public string TitleTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string VariantGroup { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}

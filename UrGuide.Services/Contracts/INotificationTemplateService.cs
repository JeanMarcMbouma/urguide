using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.Contracts;

/// <summary>
/// Service contract for managing reusable push notification templates.
/// </summary>
public interface INotificationTemplateService
{
    /// <summary>
    /// Returns all templates, optionally filtered by category and/or language.
    /// </summary>
    Task<Outcome<List<NotificationTemplateDto>>> GetTemplatesAsync(
        string? category = null,
        string? language = null,
        CancellationToken ct = default);

    /// <summary>
    /// Returns a specific template by its database ID.
    /// </summary>
    Task<Outcome<NotificationTemplateDto>> GetTemplateByIdAsync(
        string id,
        CancellationToken ct = default);

    /// <summary>
    /// Returns the active template for a given name and language.
    /// Falls back to "en" when no record exists for the requested language.
    /// </summary>
    Task<Outcome<NotificationTemplateDto>> GetTemplateByNameAsync(
        string name,
        string language = "en",
        CancellationToken ct = default);

    /// <summary>
    /// Creates a new template. The version is set to 1 for the first entry of a
    /// name+language combination, and auto-incremented for subsequent ones.
    /// </summary>
    Task<Outcome<NotificationTemplateDto>> CreateTemplateAsync(
        CreateNotificationTemplateRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing template. Increments the version number and persists
    /// the previous version (marked inactive) for history.
    /// </summary>
    Task<Outcome<NotificationTemplateDto>> UpdateTemplateAsync(
        string id,
        UpdateNotificationTemplateRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Soft-deletes a template by marking it inactive.
    /// </summary>
    Task<Outcome<bool>> DeleteTemplateAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Resolves all {{variable_name}} placeholders in a template using the
    /// supplied variables dictionary. Returns the rendered (title, body) pair.
    /// </summary>
    (string title, string body) RenderTemplate(
        NotificationTemplateDto template,
        Dictionary<string, string>? variables);
}

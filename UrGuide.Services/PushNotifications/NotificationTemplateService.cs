using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Model.PushNotifications;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.PushNotifications;

class NotificationTemplateService : INotificationTemplateService
{
    // Matches {{variable_name}} placeholders (letters, digits and underscores).
    private static readonly Regex PlaceholderRegex =
        new(@"\{\{([a-zA-Z0-9_]+)\}\}", RegexOptions.Compiled);

    public NotificationTemplateService(
        UrGuideContext context,
        IUserContext userContext,
        ILogger<NotificationTemplateService> logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public UrGuideContext Context { get; }
    public IUserContext UserContext { get; }
    public ILogger<NotificationTemplateService> Logger { get; }

    public async Task<Outcome<List<NotificationTemplateDto>>> GetTemplatesAsync(
        string? category = null, string? language = null, CancellationToken ct = default)
    {
        var query = Context.NotificationTemplates.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        if (!string.IsNullOrEmpty(language))
            query = query.Where(t => t.Language == language);

        var templates = await query
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Language)
            .ThenByDescending(t => t.Version)
            .ToListAsync(ct);

        return Result.Of(templates.Select(MapToDto).ToList());
    }

    public async Task<Outcome<NotificationTemplateDto>> GetTemplateByIdAsync(
        string id, CancellationToken ct = default)
    {
        var template = await Context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (template == null)
            return Result.Of<NotificationTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

        return Result.Of(MapToDto(template));
    }

    public async Task<Outcome<NotificationTemplateDto>> GetTemplateByNameAsync(
        string name, string language = "en", CancellationToken ct = default)
    {
        // Try exact language match first
        var template = await Context.NotificationTemplates
            .Where(t => t.Name == name && t.Language == language && t.IsActive)
            .OrderByDescending(t => t.Version)
            .FirstOrDefaultAsync(ct);

        // Fall back to English if the requested language is not available
        if (template == null && language != "en")
        {
            template = await Context.NotificationTemplates
                .Where(t => t.Name == name && t.Language == "en" && t.IsActive)
                .OrderByDescending(t => t.Version)
                .FirstOrDefaultAsync(ct);
        }

        if (template == null)
            return Result.Of<NotificationTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

        return Result.Of(MapToDto(template));
    }

    public async Task<Outcome<NotificationTemplateDto>> CreateTemplateAsync(
        CreateNotificationTemplateRequest request, CancellationToken ct = default)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<NotificationTemplateDto>().WithErrors(ErrorMessages.NotAuthenticated);

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Of<NotificationTemplateDto>().WithErrors("Name is required.");

        if (string.IsNullOrWhiteSpace(request.TitleTemplate))
            return Result.Of<NotificationTemplateDto>().WithErrors("TitleTemplate is required.");

        if (string.IsNullOrWhiteSpace(request.BodyTemplate))
            return Result.Of<NotificationTemplateDto>().WithErrors("BodyTemplate is required.");

        // Determine next version number for this name + language combination
        var latestVersion = await Context.NotificationTemplates
            .Where(t => t.Name == request.Name && t.Language == (request.Language ?? "en"))
            .MaxAsync(t => (int?)t.Version, ct) ?? 0;

        var template = new Data.Entities.PushNotifications.NotificationTemplate
        {
            Name = request.Name,
            Category = request.Category ?? string.Empty,
            Language = string.IsNullOrEmpty(request.Language) ? "en" : request.Language,
            Version = latestVersion + 1,
            TitleTemplate = request.TitleTemplate,
            BodyTemplate = request.BodyTemplate,
            ImageUrl = request.ImageUrl ?? string.Empty,
            ActionUrl = request.ActionUrl ?? string.Empty,
            IsActive = true,
            VariantGroup = request.VariantGroup ?? string.Empty,
            CreatedBy = UserContext.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.NotificationTemplates.Add(template);
        await Context.SaveChangesAsync(ct);

        Logger.LogInformation(
            "Notification template '{Name}' (lang={Language}, v{Version}) created by {UserId}",
            template.Name, template.Language, template.Version, UserContext.UserId);

        return Result.Of(MapToDto(template));
    }

    public async Task<Outcome<NotificationTemplateDto>> UpdateTemplateAsync(
        string id, UpdateNotificationTemplateRequest request, CancellationToken ct = default)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of<NotificationTemplateDto>().WithErrors(ErrorMessages.NotAuthenticated);

        var existing = await Context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (existing == null)
            return Result.Of<NotificationTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

        if (string.IsNullOrWhiteSpace(request.TitleTemplate))
            return Result.Of<NotificationTemplateDto>().WithErrors("TitleTemplate is required.");

        if (string.IsNullOrWhiteSpace(request.BodyTemplate))
            return Result.Of<NotificationTemplateDto>().WithErrors("BodyTemplate is required.");

        // Mark old version inactive before creating the new one
        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;

        var newTemplate = new Data.Entities.PushNotifications.NotificationTemplate
        {
            Name = existing.Name,
            Category = existing.Category,
            Language = existing.Language,
            Version = existing.Version + 1,
            TitleTemplate = request.TitleTemplate,
            BodyTemplate = request.BodyTemplate,
            ImageUrl = request.ImageUrl ?? string.Empty,
            ActionUrl = request.ActionUrl ?? string.Empty,
            IsActive = request.IsActive,
            VariantGroup = request.VariantGroup ?? string.Empty,
            CreatedBy = existing.CreatedBy,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        Context.NotificationTemplates.Add(newTemplate);
        await Context.SaveChangesAsync(ct);

        Logger.LogInformation(
            "Notification template '{Name}' (lang={Language}) updated to v{Version} by {UserId}",
            newTemplate.Name, newTemplate.Language, newTemplate.Version, UserContext.UserId);

        return Result.Of(MapToDto(newTemplate));
    }

    public async Task<Outcome<bool>> DeleteTemplateAsync(string id, CancellationToken ct = default)
    {
        if (!UserContext.IsAuthenticated)
            return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

        var template = await Context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (template == null)
            return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

        template.IsActive = false;
        template.UpdatedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync(ct);

        Logger.LogInformation(
            "Notification template '{Name}' (id={Id}) deactivated by {UserId}",
            template.Name, id, UserContext.UserId);

        return Result.Of(true);
    }

    public (string title, string body) RenderTemplate(
        NotificationTemplateDto template,
        Dictionary<string, string>? variables)
    {
        if (variables == null || variables.Count == 0)
            return (template.TitleTemplate, template.BodyTemplate);

        var title = PlaceholderRegex.Replace(template.TitleTemplate, m =>
            variables.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);

        var body = PlaceholderRegex.Replace(template.BodyTemplate, m =>
            variables.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);

        return (title, body);
    }

    private static NotificationTemplateDto MapToDto(
        Data.Entities.PushNotifications.NotificationTemplate entity)
    {
        return new NotificationTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = entity.Category,
            Language = entity.Language,
            Version = entity.Version,
            TitleTemplate = entity.TitleTemplate,
            BodyTemplate = entity.BodyTemplate,
            ImageUrl = entity.ImageUrl,
            ActionUrl = entity.ActionUrl,
            IsActive = entity.IsActive,
            VariantGroup = entity.VariantGroup,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt.ToString("O"),
            UpdatedAt = entity.UpdatedAt.ToString("O")
        };
    }
}

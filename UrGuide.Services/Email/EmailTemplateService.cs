using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Email;
using UrGuide.Model.Email;
using UrGuide.Model.Results;

namespace UrGuide.Services.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(UrGuideContext context, ILogger<EmailTemplateService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Outcome<EmailTemplateDto>> CreateTemplateAsync(string userId, CreateEmailTemplateRequest request)
        {
            try
            {
                var entity = new EmailTemplate
                {
                    TemplateId = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Subject = request.Subject,
                    HtmlBody = request.HtmlBody,
                    PlainTextBody = request.PlainTextBody,
                    Category = request.Category,
                    Language = request.Language ?? "en",
                    Version = 1,
                    IsActive = true,
                    IsDefault = false,
                    VariablesJson = JsonSerializer.Serialize(request.Variables ?? new List<string>()),
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var initialVersion = new EmailTemplateVersion
                {
                    VersionId = Guid.NewGuid().ToString(),
                    TemplateId = entity.TemplateId,
                    VersionNumber = 1,
                    Subject = request.Subject,
                    HtmlBody = request.HtmlBody,
                    PlainTextBody = request.PlainTextBody,
                    ChangeSummary = "Initial version",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmailTemplates.Add(entity);
                _context.EmailTemplateVersions.Add(initialVersion);
                await _context.SaveChangesAsync();

                return Result.Of(MapToDto(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating email template");
                return Result.Of<EmailTemplateDto>().WithErrors("Failed to create email template");
            }
        }

        public async Task<Outcome<EmailTemplateDto>> UpdateTemplateAsync(string userId, string templateId, UpdateEmailTemplateRequest request)
        {
            try
            {
                var entity = await _context.EmailTemplates.FindAsync(templateId);
                if (entity == null)
                {
                    return Result.Of<EmailTemplateDto>().WithErrors("Email template not found");
                }

                entity.Subject = request.Subject ?? entity.Subject;
                entity.HtmlBody = request.HtmlBody ?? entity.HtmlBody;
                entity.PlainTextBody = request.PlainTextBody ?? entity.PlainTextBody;
                entity.Version += 1;
                entity.UpdatedAt = DateTime.UtcNow;

                var version = new EmailTemplateVersion
                {
                    VersionId = Guid.NewGuid().ToString(),
                    TemplateId = templateId,
                    VersionNumber = entity.Version,
                    Subject = entity.Subject,
                    HtmlBody = entity.HtmlBody,
                    PlainTextBody = entity.PlainTextBody,
                    ChangeSummary = request.ChangeSummary ?? "Updated template",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmailTemplateVersions.Add(version);
                await _context.SaveChangesAsync();

                return Result.Of(MapToDto(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email template {TemplateId}", templateId);
                return Result.Of<EmailTemplateDto>().WithErrors("Failed to update email template");
            }
        }

        public async Task<Outcome<EmailTemplateDto>> GetTemplateAsync(string templateId)
        {
            try
            {
                var entity = await _context.EmailTemplates.FindAsync(templateId);
                if (entity == null)
                {
                    return Result.Of<EmailTemplateDto>().WithErrors("Email template not found");
                }

                return Result.Of(MapToDto(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email template {TemplateId}", templateId);
                return Result.Of<EmailTemplateDto>().WithErrors("Failed to retrieve email template");
            }
        }

        public async Task<Outcome<EmailTemplateListResponse>> GetTemplatesAsync(int page, int pageSize, string category = null, string language = null)
        {
            try
            {
                var query = _context.EmailTemplates.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(t => t.Category == category);
                }

                if (!string.IsNullOrEmpty(language))
                {
                    query = query.Where(t => t.Language == language);
                }

                var totalCount = await query.CountAsync();

                var templates = await query
                    .OrderByDescending(t => t.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new EmailTemplateListItem
                    {
                        TemplateId = t.TemplateId,
                        Name = t.Name,
                        Category = t.Category,
                        Language = t.Language,
                        Version = t.Version,
                        IsActive = t.IsActive,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return Result.Of(new EmailTemplateListResponse
                {
                    Templates = templates,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing email templates");
                return Result.Of<EmailTemplateListResponse>().WithErrors("Failed to list email templates");
            }
        }

        public async Task<Outcome<EmailPreviewResult>> PreviewTemplateAsync(EmailPreviewRequest request)
        {
            try
            {
                var entity = await _context.EmailTemplates.FindAsync(request.TemplateId);
                if (entity == null)
                {
                    return Result.Of<EmailPreviewResult>().WithErrors("Email template not found");
                }

                var result = SubstituteVariables(entity, request.Variables);
                return Result.Of(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing email template {TemplateId}", request.TemplateId);
                return Result.Of<EmailPreviewResult>().WithErrors("Failed to preview email template");
            }
        }

        public async Task<Outcome<EmailPreviewResult>> RenderEmailAsync(string templateName, string language, Dictionary<string, string> variables)
        {
            try
            {
                var entity = await _context.EmailTemplates
                    .Where(t => t.Name == templateName && t.Language == language && t.IsActive)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    // Fallback to default language
                    entity = await _context.EmailTemplates
                        .Where(t => t.Name == templateName && t.Language == "en" && t.IsActive)
                        .FirstOrDefaultAsync();
                }

                if (entity == null)
                {
                    return Result.Of<EmailPreviewResult>().WithErrors($"Email template '{templateName}' not found");
                }

                var result = SubstituteVariables(entity, variables);
                return Result.Of(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering email template {TemplateName}", templateName);
                return Result.Of<EmailPreviewResult>().WithErrors("Failed to render email template");
            }
        }

        public async Task<Outcome<List<EmailTemplateVersionDto>>> GetTemplateVersionsAsync(string templateId)
        {
            try
            {
                var template = await _context.EmailTemplates.FindAsync(templateId);
                if (template == null)
                {
                    return Result.Of<List<EmailTemplateVersionDto>>().WithErrors("Email template not found");
                }

                var versions = await _context.EmailTemplateVersions
                    .Where(v => v.TemplateId == templateId)
                    .OrderByDescending(v => v.VersionNumber)
                    .Select(v => new EmailTemplateVersionDto
                    {
                        VersionId = v.VersionId,
                        TemplateId = v.TemplateId,
                        VersionNumber = v.VersionNumber,
                        Subject = v.Subject,
                        HtmlBody = v.HtmlBody,
                        PlainTextBody = v.PlainTextBody,
                        ChangeSummary = v.ChangeSummary,
                        CreatedBy = v.CreatedBy,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync();

                return Result.Of(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving versions for template {TemplateId}", templateId);
                return Result.Of<List<EmailTemplateVersionDto>>().WithErrors("Failed to retrieve template versions");
            }
        }

        public async Task<Outcome<bool>> DeactivateTemplateAsync(string templateId)
        {
            try
            {
                var entity = await _context.EmailTemplates.FindAsync(templateId);
                if (entity == null)
                {
                    return Result.Of(false).WithErrors("Email template not found");
                }

                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating email template {TemplateId}", templateId);
                return Result.Of(false).WithErrors("Failed to deactivate email template");
            }
        }

        private static EmailPreviewResult SubstituteVariables(EmailTemplate template, Dictionary<string, string> variables)
        {
            var subject = template.Subject;
            var htmlBody = template.HtmlBody;
            var plainTextBody = template.PlainTextBody;

            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    var placeholder = "{{" + variable.Key + "}}";
                    subject = subject.Replace(placeholder, variable.Value);
                    htmlBody = htmlBody.Replace(placeholder, variable.Value);
                    if (plainTextBody != null)
                    {
                        plainTextBody = plainTextBody.Replace(placeholder, variable.Value);
                    }
                }
            }

            return new EmailPreviewResult
            {
                Subject = subject,
                HtmlBody = htmlBody,
                PlainTextBody = plainTextBody
            };
        }

        private static EmailTemplateDto MapToDto(EmailTemplate entity)
        {
            var variables = new List<string>();
            if (!string.IsNullOrEmpty(entity.VariablesJson))
            {
                try
                {
                    variables = JsonSerializer.Deserialize<List<string>>(entity.VariablesJson) ?? new List<string>();
                }
                catch
                {
                    // If deserialization fails, return empty list
                }
            }

            return new EmailTemplateDto
            {
                TemplateId = entity.TemplateId,
                Name = entity.Name,
                Subject = entity.Subject,
                HtmlBody = entity.HtmlBody,
                PlainTextBody = entity.PlainTextBody,
                Category = entity.Category,
                Language = entity.Language,
                Version = entity.Version,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault,
                Variables = variables,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}

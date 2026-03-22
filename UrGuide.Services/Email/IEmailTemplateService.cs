using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Email;

namespace UrGuide.Services.Email
{
    public interface IEmailTemplateService
    {
        Task<Outcome<EmailTemplateDto>> CreateTemplateAsync(string userId, CreateEmailTemplateRequest request);
        Task<Outcome<EmailTemplateDto>> UpdateTemplateAsync(string userId, string templateId, UpdateEmailTemplateRequest request);
        Task<Outcome<EmailTemplateDto>> GetTemplateAsync(string templateId);
        Task<Outcome<EmailTemplateListResponse>> GetTemplatesAsync(int page, int pageSize, string category = null, string language = null);
        Task<Outcome<EmailPreviewResult>> PreviewTemplateAsync(EmailPreviewRequest request);
        Task<Outcome<EmailPreviewResult>> RenderEmailAsync(string templateName, string language, Dictionary<string, string> variables);
        Task<Outcome<List<EmailTemplateVersionDto>>> GetTemplateVersionsAsync(string templateId);
        Task<Outcome<bool>> DeactivateTemplateAsync(string templateId);
    }
}

using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model.Templates;

namespace UrGuide.Services.Templates
{
    public interface ITourTemplateService
    {
        Task<Outcome<TourTemplateDto>> CreateTemplateAsync(string guideId, CreateTourTemplateRequest request, CancellationToken cancellationToken);
        Task<Outcome<TourTemplateDto>> UpdateTemplateAsync(string guideId, string templateId, UpdateTourTemplateRequest request, CancellationToken cancellationToken);
        Task<Outcome<bool>> DeleteTemplateAsync(string guideId, string templateId, CancellationToken cancellationToken);
        Task<Outcome<TourTemplateDto>> GetTemplateAsync(string templateId, CancellationToken cancellationToken);
        Task<Outcome<PagedList<TourTemplateListItem>>> GetGuideTemplatesAsync(string guideId, int page, int pageSize, string category, CancellationToken cancellationToken);
        Task<Outcome<TourTemplateDto>> CreateTourFromTemplateAsync(string guideId, string templateId, CancellationToken cancellationToken);
        Task<Outcome<bool>> IncrementUsageCountAsync(string templateId, CancellationToken cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BbQ.Outcome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Model.Templates;
using UrGuide.Services.Abstraction;
using UrGuide.Shared.Contracts;
using DataTourTemplate = UrGuide.Data.Entities.Tour.TourTemplate;

namespace UrGuide.Services.Templates
{
    public class TourTemplateService : BaseService, ITourTemplateService
    {
        public TourTemplateService(UrGuideContext context,
                                   IUserContext userContext,
                                   ILogger<TourTemplateService> logger) : base(context, userContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILogger<TourTemplateService> Logger { get; }

        public async Task<Outcome<TourTemplateDto>> CreateTemplateAsync(string guideId, CreateTourTemplateRequest request, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var template = new DataTourTemplate
            {
                GuideId = guideId,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                BasePrice = request.BasePrice,
                CurrencyCode = request.CurrencyCode,
                DefaultDurationMinutes = request.DefaultDurationMinutes,
                DefaultMaxParticipants = request.DefaultMaxParticipants,
                DefaultMeetingPoint = request.DefaultMeetingPoint,
                ItineraryJson = JsonSerializer.Serialize(request.Itinerary ?? new List<ItineraryItem>()),
                IncludedItemsJson = JsonSerializer.Serialize(request.IncludedItems ?? new List<string>()),
                ExcludedItemsJson = JsonSerializer.Serialize(request.ExcludedItems ?? new List<string>()),
                IsActive = true,
                UsageCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Context.Set<DataTourTemplate>().Add(template);
            await Context.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Tour template {TemplateId} created by guide {GuideId}", template.TemplateId, guideId);

            return Result.Of(MapToDto(template));
        }

        public async Task<Outcome<TourTemplateDto>> UpdateTemplateAsync(string guideId, string templateId, UpdateTourTemplateRequest request, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var template = await Context.Set<DataTourTemplate>()
                .FirstOrDefaultAsync(t => t.TemplateId == templateId, cancellationToken);

            if (template == null)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (template.GuideId != guideId)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotAuthorized);

            if (request.Name != null)
                template.Name = request.Name;
            if (request.Description != null)
                template.Description = request.Description;
            if (request.Category != null)
                template.Category = request.Category;
            if (request.BasePrice.HasValue)
                template.BasePrice = request.BasePrice.Value;
            if (request.CurrencyCode != null)
                template.CurrencyCode = request.CurrencyCode;
            if (request.DefaultDurationMinutes.HasValue)
                template.DefaultDurationMinutes = request.DefaultDurationMinutes.Value;
            if (request.DefaultMaxParticipants.HasValue)
                template.DefaultMaxParticipants = request.DefaultMaxParticipants.Value;
            if (request.DefaultMeetingPoint != null)
                template.DefaultMeetingPoint = request.DefaultMeetingPoint;
            if (request.Itinerary != null)
                template.ItineraryJson = JsonSerializer.Serialize(request.Itinerary);
            if (request.IncludedItems != null)
                template.IncludedItemsJson = JsonSerializer.Serialize(request.IncludedItems);
            if (request.ExcludedItems != null)
                template.ExcludedItemsJson = JsonSerializer.Serialize(request.ExcludedItems);
            if (request.IsActive.HasValue)
                template.IsActive = request.IsActive.Value;

            template.UpdatedAt = DateTime.UtcNow;

            await Context.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Tour template {TemplateId} updated by guide {GuideId}", templateId, guideId);

            return Result.Of(MapToDto(template));
        }

        public async Task<Outcome<bool>> DeleteTemplateAsync(string guideId, string templateId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var template = await Context.Set<DataTourTemplate>()
                .FirstOrDefaultAsync(t => t.TemplateId == templateId, cancellationToken);

            if (template == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (template.GuideId != guideId)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthorized);

            Context.Set<DataTourTemplate>().Remove(template);
            await Context.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Tour template {TemplateId} deleted by guide {GuideId}", templateId, guideId);

            return Result.Of(true);
        }

        public async Task<Outcome<TourTemplateDto>> GetTemplateAsync(string templateId, CancellationToken cancellationToken)
        {
            var template = await Context.Set<DataTourTemplate>()
                .FirstOrDefaultAsync(t => t.TemplateId == templateId, cancellationToken);

            if (template == null)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            return Result.Of(MapToDto(template));
        }

        public async Task<Outcome<PagedList<TourTemplateListItem>>> GetGuideTemplatesAsync(string guideId, int page, int pageSize, string category, CancellationToken cancellationToken)
        {
            var query = Context.Set<DataTourTemplate>()
                .Where(t => t.GuideId == guideId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(t => t.Category == category);

            query = query.OrderByDescending(t => t.CreatedAt);

            var pagedData = await PagedList.Of(query, page, cancellationToken);
            var result = pagedData.To(t => MapToListItem(t));

            return Result.Of(result);
        }

        public async Task<Outcome<TourTemplateDto>> CreateTourFromTemplateAsync(string guideId, string templateId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var template = await Context.Set<DataTourTemplate>()
                .FirstOrDefaultAsync(t => t.TemplateId == templateId, cancellationToken);

            if (template == null)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (template.GuideId != guideId)
                return Result.Of<TourTemplateDto>().WithErrors(ErrorMessages.NotAuthorized);

            template.UsageCount++;
            template.UpdatedAt = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Tour created from template {TemplateId} by guide {GuideId}. Usage count: {UsageCount}",
                templateId, guideId, template.UsageCount);

            return Result.Of(MapToDto(template));
        }

        public async Task<Outcome<bool>> IncrementUsageCountAsync(string templateId, CancellationToken cancellationToken)
        {
            var template = await Context.Set<DataTourTemplate>()
                .FirstOrDefaultAsync(t => t.TemplateId == templateId, cancellationToken);

            if (template == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

            template.UsageCount++;
            template.UpdatedAt = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);

            return Result.Of(true);
        }

        private static TourTemplateDto MapToDto(DataTourTemplate template)
        {
            return new TourTemplateDto
            {
                TemplateId = template.TemplateId,
                GuideId = template.GuideId,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                BasePrice = template.BasePrice,
                CurrencyCode = template.CurrencyCode,
                DefaultDurationMinutes = template.DefaultDurationMinutes,
                DefaultMaxParticipants = template.DefaultMaxParticipants,
                DefaultMeetingPoint = template.DefaultMeetingPoint,
                Itinerary = DeserializeOrDefault<List<ItineraryItem>>(template.ItineraryJson, new List<ItineraryItem>()),
                IncludedItems = DeserializeOrDefault<List<string>>(template.IncludedItemsJson, new List<string>()),
                ExcludedItems = DeserializeOrDefault<List<string>>(template.ExcludedItemsJson, new List<string>()),
                IsActive = template.IsActive,
                UsageCount = template.UsageCount,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }

        private static TourTemplateListItem MapToListItem(DataTourTemplate template)
        {
            return new TourTemplateListItem
            {
                TemplateId = template.TemplateId,
                Name = template.Name,
                Category = template.Category,
                BasePrice = template.BasePrice,
                CurrencyCode = template.CurrencyCode,
                UsageCount = template.UsageCount,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt
            };
        }

        private static T DeserializeOrDefault<T>(string json, T defaultValue)
        {
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            try
            {
                return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
            }
            catch (JsonException)
            {
                return defaultValue;
            }
        }
    }
}

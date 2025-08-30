using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Tour;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using DataTourRequest = UrGuide.Data.Entities.Tour.TourRequest;
using DataTourRequestStatus = UrGuide.Data.Entities.Tour.TourRequestStatus;

namespace UrGuide.Services.Tour
{
    class TourRequestService : BaseService, ITourRequestService
    {
        public TourRequestService(UrGuideContext context,
                                  IUserContext userContext,
                                  IMapper mapper,
                                  ILogger<TourRequestService> logger,
                                  IUserNotificationService notificationService) : base(context, userContext)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public IMapper Mapper { get; }
        public ILogger<TourRequestService> Logger { get; }
        public IUserNotificationService NotificationService { get; }

        public async Task<Result<TourRequestModel>> CreateTourRequestAsync(CreateTourRequestModel model, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<TourRequestModel>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            // Validate that the region exists and allows tour requests
            var region = await Context.Set<Data.Entities.Regions.Region>()
                .Include(r => r.Flags)
                .FirstOrDefaultAsync(r => r.RegionId == model.RegionId, cancellationToken);

            if (region == null)
                return Result.Of<TourRequestModel>().WithErrors("Region not found");

            if (region.Flags != null && !region.Flags.CanRaiseTourRequests)
                return Result.Of<TourRequestModel>().WithErrors("Tour requests are not allowed in this region");

            var tourRequest = new DataTourRequest
            {
                Title = model.Title,
                Description = model.Description,
                PreferredDate = model.PreferredDate,
                MaxParticipants = model.MaxParticipants,
                MaxBudget = model.MaxBudget,
                Tags = model.Tags ?? string.Empty,
                RequesterId = UserContext.UserId,
                RegionId = model.RegionId,
                Status = DataTourRequestStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Context.Set<DataTourRequest>().Add(tourRequest);
            await Context.SaveChangesAsync(cancellationToken);

            // Notify guides in the region
            await NotifyGuidesInRegionAsync(tourRequest, cancellationToken);

            var result = Mapper.Map<TourRequestModel>(tourRequest);
            return Result.Of(result);
        }

        private async Task NotifyGuidesInRegionAsync(DataTourRequest tourRequest, CancellationToken cancellationToken)
        {
            try
            {
                // Find guides (authors) in the same region
                var guidesInRegion = await Context.Users
                    .Where(u => u.Attributes.Any(a => a.Name == "UserType" && a.Value == "Guide"))
                    .Where(u => u.Attributes.Any(a => a.Name == "RegionId" && a.Value == tourRequest.RegionId))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);

                var notificationContent = $"New tour request: '{tourRequest.Title}' in your region. Budget: ${tourRequest.MaxBudget:F2}";
                var referenceLink = $"/tour-requests/{tourRequest.TourRequestId}";

                foreach (var guideId in guidesInRegion)
                {
                    await NotificationService.SystemNotifyAsync(guideId, notificationContent, referenceLink);
                }

                Logger.LogInformation($"Notified {guidesInRegion.Count} guides about tour request {tourRequest.TourRequestId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to notify guides about tour request {tourRequest.TourRequestId}");
            }
        }

        public async Task<Result<TourRequestModel>> GetTourRequestByIdAsync(string tourRequestId, CancellationToken cancellationToken)
        {
            var tourRequest = await Context.Set<DataTourRequest>()
                .Include(tr => tr.Requester)
                .Include(tr => tr.Region)
                .FirstOrDefaultAsync(tr => tr.TourRequestId == tourRequestId, cancellationToken);

            if (tourRequest == null)
                return Result.Of<TourRequestModel>().WithErrors(ErrorMessages.NotFoundEntityForKey);

            var result = Mapper.Map<TourRequestModel>(tourRequest);
            return Result.Of(result);
        }

        public async Task<Result<PagedList<TourRequestModel>>> GetTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            var query = Context.Set<DataTourRequest>()
                .Include(tr => tr.Requester)
                .Include(tr => tr.Region)
                .Where(tr => tr.Status == DataTourRequestStatus.Open)
                .OrderByDescending(tr => tr.CreatedAt);

            var pagedData = await PagedList.Of(query, pagination.PageNumber, cancellationToken);
            var result = pagedData.To(tr => Mapper.Map<TourRequestModel>(tr));

            return Result.Of(result);
        }

        public async Task<Result<PagedList<TourRequestModel>>> GetMyTourRequestsAsync(SearchParameters pagination, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<PagedList<TourRequestModel>>().WithErrors(ErrorMessages.NotAuthenticated);

            var query = Context.Set<DataTourRequest>()
                .Include(tr => tr.Requester)
                .Include(tr => tr.Region)
                .Where(tr => tr.RequesterId == UserContext.UserId)
                .OrderByDescending(tr => tr.CreatedAt);

            var pagedData = await PagedList.Of(query, pagination.PageNumber, cancellationToken);
            var result = pagedData.To(tr => Mapper.Map<TourRequestModel>(tr));

            return Result.Of(result);
        }

        public async Task<Result<PagedList<TourRequestModel>>> GetTourRequestsByRegionAsync(string regionId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            var query = Context.Set<DataTourRequest>()
                .Include(tr => tr.Requester)
                .Include(tr => tr.Region)
                .Where(tr => tr.RegionId == regionId && tr.Status == DataTourRequestStatus.Open)
                .OrderByDescending(tr => tr.CreatedAt);

            var pagedData = await PagedList.Of(query, pagination.PageNumber, cancellationToken);
            var result = pagedData.To(tr => Mapper.Map<TourRequestModel>(tr));

            return Result.Of(result);
        }

        public async Task<Result<bool>> CancelTourRequestAsync(string tourRequestId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            var tourRequest = await Context.Set<DataTourRequest>()
                .FirstOrDefaultAsync(tr => tr.TourRequestId == tourRequestId && tr.RequesterId == UserContext.UserId, cancellationToken);

            if (tourRequest == null)
                return Result.Of(false).WithErrors(ErrorMessages.NotFoundEntityForKey);

            if (tourRequest.Status != DataTourRequestStatus.Open)
                return Result.Of(false).WithErrors("Only open tour requests can be cancelled");

            tourRequest.Status = DataTourRequestStatus.Cancelled;
            tourRequest.UpdatedAt = DateTime.UtcNow;

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }
    }
}
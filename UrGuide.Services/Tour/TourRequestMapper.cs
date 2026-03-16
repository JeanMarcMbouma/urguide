using UrGuide.Model.Tour;

namespace UrGuide.Services.Tour
{
    public static class TourRequestMapper
    {
        public static TourRequestModel ToTourRequestModel(Data.Entities.Tour.TourRequest source)
        {
            return new TourRequestModel
            {
                TourRequestId = source.TourRequestId,
                Title = source.Title,
                Description = source.Description,
                PreferredDate = source.PreferredDate,
                MaxParticipants = source.MaxParticipants,
                MaxBudget = source.MaxBudget,
                Tags = source.Tags,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                Status = (TourRequestStatus)source.Status,
                RequesterId = source.RequesterId,
                RequesterName = source.Requester != null ? $"{source.Requester.FirstName} {source.Requester.LastName}" : string.Empty,
                RegionId = source.RegionId,
                RegionName = source.Region != null ? source.Region.Name : string.Empty
            };
        }
    }
}

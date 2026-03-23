using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Model.Recommendations;

namespace UrGuide.Services.Recommendations
{
    public interface IRecommendationService
    {
        Task<List<TourRecommendationDto>> GetRecommendationsAsync(string userId, int count = 10, double? latitude = null, double? longitude = null);
        Task<List<TourRecommendationDto>> GetPopularToursAsync(int count = 10, double? latitude = null, double? longitude = null);
        Task<bool> SetUserPreferencesAsync(string userId, SetPreferencesRequest request);
        Task<List<UserPreferenceDto>> GetUserPreferencesAsync(string userId);
        Task<bool> RecordInteractionAsync(string userId, RecordInteractionRequest request);
        Task<bool> ProvideFeedbackAsync(string userId, RecommendationFeedbackRequest request);
        Task<RecommendationStatsDto> GetRecommendationStatsAsync();
    }
}

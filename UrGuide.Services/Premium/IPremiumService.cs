using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Premium;

namespace UrGuide.Services.Premium
{
    public interface IPremiumService
    {
        // Subscription Plans
        Task<Outcome<SubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanRequest request);
        Task<Outcome<List<SubscriptionPlanDto>>> GetAllPlansAsync();
        Task<Outcome<SubscriptionPlanDto>> GetPlanAsync(string planId);

        // Guide Subscriptions
        Task<Outcome<GuideSubscriptionDto>> SubscribeAsync(string guideId, SubscribeRequest request);
        Task<Outcome<GuideSubscriptionDto>> GetGuideSubscriptionAsync(string guideId);
        Task<Outcome<bool>> CancelSubscriptionAsync(string guideId);

        // Visibility Boosts
        Task<Outcome<VisibilityBoostDto>> CreateBoostAsync(string guideId, CreateVisibilityBoostRequest request);
        Task<Outcome<List<VisibilityBoostDto>>> GetActiveBoostsAsync(string guideId);

        // Advertisements
        Task<Outcome<AdvertisementDto>> CreateAdvertisementAsync(string advertiserId, CreateAdvertisementRequest request);
        Task<Outcome<AdvertisementDto>> GetAdvertisementAsync(string adId, string advertiserId);
        Task<Outcome<List<AdvertisementDto>>> GetAdvertiserAdsAsync(string advertiserId, int page = 1, int pageSize = 20);
        Task<Outcome<AdvertisementDto>> UpdateAdvertisementAsync(string adId, string advertiserId, UpdateAdvertisementRequest request);
        Task<Outcome<AdPerformanceDto>> GetAdPerformanceAsync(string adId, string advertiserId);
        Task<Outcome<bool>> RecordImpressionAsync(string adId);
        Task<Outcome<bool>> RecordClickAsync(string adId);
    }
}

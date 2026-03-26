using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Premium;
using UrGuide.Model.Premium;
using UrGuide.Model.Results;

namespace UrGuide.Services.Premium
{
    public class PremiumService : IPremiumService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<PremiumService> _logger;

        private static readonly Dictionary<BoostType, decimal> BoostCosts = new()
        {
            { BoostType.SearchRanking, 9.99m },
            { BoostType.FeaturedListing, 19.99m },
            { BoostType.TopResult, 29.99m },
            { BoostType.HighlightedProfile, 14.99m }
        };

        public PremiumService(UrGuideContext context, ILogger<PremiumService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Subscription Plans
        public async Task<Outcome<SubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanRequest request)
        {
            try
            {
                var plan = new SubscriptionPlan
                {
                    SubscriptionPlanId = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Description = request.Description,
                    Tier = (PlanTier)request.Tier,
                    BillingCycle = (BillingCycle)request.BillingCycle,
                    Price = request.Price,
                    PlatformFeePercentage = request.PlatformFeePercentage,
                    SearchRankingBoost = request.SearchRankingBoost,
                    MaxGroupSize = request.MaxGroupSize,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SubscriptionPlans.Add(plan);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created subscription plan {PlanName}", request.Name);
                return Result.Of(MapToPlanDto(plan));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return Result.Of<SubscriptionPlanDto>().WithErrors("Failed to create subscription plan");
            }
        }

        public async Task<Outcome<List<SubscriptionPlanDto>>> GetAllPlansAsync()
        {
            try
            {
                var plans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Tier)
                    .ThenBy(p => p.BillingCycle)
                    .ToListAsync();

                return Result.Of(plans.Select(MapToPlanDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans");
                return Result.Of<List<SubscriptionPlanDto>>().WithErrors("Failed to retrieve subscription plans");
            }
        }

        public async Task<Outcome<SubscriptionPlanDto>> GetPlanAsync(string planId)
        {
            try
            {
                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.SubscriptionPlanId == planId);

                if (plan == null)
                    return Result.Of<SubscriptionPlanDto>().WithErrors("Subscription plan not found");

                return Result.Of(MapToPlanDto(plan));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan {PlanId}", planId);
                return Result.Of<SubscriptionPlanDto>().WithErrors("Failed to retrieve subscription plan");
            }
        }

        // Guide Subscriptions
        public async Task<Outcome<GuideSubscriptionDto>> SubscribeAsync(string guideId, SubscribeRequest request)
        {
            try
            {
                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.SubscriptionPlanId == request.SubscriptionPlanId && p.IsActive);

                if (plan == null)
                    return Result.Of<GuideSubscriptionDto>().WithErrors("Subscription plan not found or inactive");

                var existingActive = await _context.GuideSubscriptions
                    .FirstOrDefaultAsync(s => s.GuideId == guideId && s.Status == GuideSubscriptionStatus.Active);

                if (existingActive != null)
                {
                    existingActive.Status = GuideSubscriptionStatus.Cancelled;
                    existingActive.UpdatedAt = DateTime.UtcNow;
                }

                var endDate = plan.BillingCycle switch
                {
                    BillingCycle.Monthly => DateTime.UtcNow.AddMonths(1),
                    BillingCycle.Quarterly => DateTime.UtcNow.AddMonths(3),
                    BillingCycle.Yearly => DateTime.UtcNow.AddYears(1),
                    _ => DateTime.UtcNow.AddMonths(1)
                };

                var subscription = new GuideSubscription
                {
                    GuideSubscriptionId = Guid.NewGuid().ToString(),
                    GuideId = guideId,
                    SubscriptionPlanId = request.SubscriptionPlanId,
                    Status = GuideSubscriptionStatus.Active,
                    StartDate = DateTime.UtcNow,
                    EndDate = endDate,
                    AutoRenew = request.AutoRenew,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.GuideSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Guide {GuideId} subscribed to plan {PlanName}", guideId, plan.Name);

                return Result.Of(MapToSubscriptionDto(subscription, plan));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing guide {GuideId}", guideId);
                return Result.Of<GuideSubscriptionDto>().WithErrors("Failed to create subscription");
            }
        }

        public async Task<Outcome<GuideSubscriptionDto>> GetGuideSubscriptionAsync(string guideId)
        {
            try
            {
                var subscription = await _context.GuideSubscriptions
                    .Include(s => s.SubscriptionPlan)
                    .FirstOrDefaultAsync(s => s.GuideId == guideId && s.Status == GuideSubscriptionStatus.Active);

                if (subscription == null)
                    return Result.Of<GuideSubscriptionDto>().WithErrors("No active subscription found");

                return Result.Of(MapToSubscriptionDto(subscription, subscription.SubscriptionPlan));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription for guide {GuideId}", guideId);
                return Result.Of<GuideSubscriptionDto>().WithErrors("Failed to retrieve subscription");
            }
        }

        public async Task<Outcome<bool>> CancelSubscriptionAsync(string guideId)
        {
            try
            {
                var subscription = await _context.GuideSubscriptions
                    .FirstOrDefaultAsync(s => s.GuideId == guideId && s.Status == GuideSubscriptionStatus.Active);

                if (subscription == null)
                    return Result.Of(false).WithErrors("No active subscription found");

                subscription.Status = GuideSubscriptionStatus.Cancelled;
                subscription.AutoRenew = false;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cancelled subscription for guide {GuideId}", guideId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription for guide {GuideId}", guideId);
                return Result.Of(false).WithErrors("Failed to cancel subscription");
            }
        }

        // Visibility Boosts
        public async Task<Outcome<VisibilityBoostDto>> CreateBoostAsync(string guideId, CreateVisibilityBoostRequest request)
        {
            try
            {
                var boostType = (BoostType)request.BoostType;
                var dailyCost = BoostCosts.GetValueOrDefault(boostType, 9.99m);
                var totalCost = dailyCost * request.DurationDays;

                var boost = new VisibilityBoost
                {
                    VisibilityBoostId = Guid.NewGuid().ToString(),
                    GuideId = guideId,
                    TourId = request.TourId,
                    BoostType = boostType,
                    Status = BoostStatus.Active,
                    BoostMultiplier = request.BoostMultiplier,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(request.DurationDays),
                    Cost = totalCost,
                    CreatedAt = DateTime.UtcNow
                };

                _context.VisibilityBoosts.Add(boost);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created visibility boost for guide {GuideId}", guideId);
                return Result.Of(MapToBoostDto(boost));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating visibility boost for guide {GuideId}", guideId);
                return Result.Of<VisibilityBoostDto>().WithErrors("Failed to create visibility boost");
            }
        }

        public async Task<Outcome<List<VisibilityBoostDto>>> GetActiveBoostsAsync(string guideId)
        {
            try
            {
                var boosts = await _context.VisibilityBoosts
                    .Where(b => b.GuideId == guideId && b.Status == BoostStatus.Active && b.EndDate > DateTime.UtcNow)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return Result.Of(boosts.Select(MapToBoostDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting boosts for guide {GuideId}", guideId);
                return Result.Of<List<VisibilityBoostDto>>().WithErrors("Failed to retrieve visibility boosts");
            }
        }

        // Advertisements
        public async Task<Outcome<AdvertisementDto>> CreateAdvertisementAsync(string advertiserId, CreateAdvertisementRequest request)
        {
            try
            {
                var ad = new Advertisement
                {
                    AdvertisementId = Guid.NewGuid().ToString(),
                    AdvertiserId = advertiserId,
                    Title = request.Title,
                    Content = request.Content,
                    ImageUrl = request.ImageUrl,
                    TargetUrl = request.TargetUrl,
                    TargetAudience = (AdTargetAudience)request.TargetAudience,
                    TargetRegionId = request.TargetRegionId,
                    Status = AdStatus.Draft,
                    Budget = request.Budget,
                    SpentAmount = 0,
                    Impressions = 0,
                    Clicks = 0,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Advertisements.Add(ad);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created advertisement {AdId} for advertiser {AdvertiserId}", ad.AdvertisementId, advertiserId);
                return Result.Of(MapToAdDto(ad));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating advertisement for advertiser {AdvertiserId}", advertiserId);
                return Result.Of<AdvertisementDto>().WithErrors("Failed to create advertisement");
            }
        }

        public async Task<Outcome<AdvertisementDto>> GetAdvertisementAsync(string adId)
        {
            try
            {
                var ad = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == adId);

                if (ad == null)
                    return Result.Of<AdvertisementDto>().WithErrors("Advertisement not found");

                return Result.Of(MapToAdDto(ad));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting advertisement {AdId}", adId);
                return Result.Of<AdvertisementDto>().WithErrors("Failed to retrieve advertisement");
            }
        }

        public async Task<Outcome<List<AdvertisementDto>>> GetAdvertiserAdsAsync(string advertiserId, int page = 1, int pageSize = 20)
        {
            try
            {
                var ads = await _context.Advertisements
                    .Where(a => a.AdvertiserId == advertiserId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Result.Of(ads.Select(MapToAdDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ads for advertiser {AdvertiserId}", advertiserId);
                return Result.Of<List<AdvertisementDto>>().WithErrors("Failed to retrieve advertisements");
            }
        }

        public async Task<Outcome<AdvertisementDto>> UpdateAdvertisementAsync(string adId, string advertiserId, UpdateAdvertisementRequest request)
        {
            try
            {
                var ad = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == adId);

                if (ad == null)
                    return Result.Of<AdvertisementDto>().WithErrors("Advertisement not found");

                if (ad.AdvertiserId != advertiserId)
                    return Result.Of<AdvertisementDto>().WithErrors("Advertisement not found");

                if (!string.IsNullOrEmpty(request.Title))
                    ad.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Content))
                    ad.Content = request.Content;

                if (!string.IsNullOrEmpty(request.ImageUrl))
                    ad.ImageUrl = request.ImageUrl;

                if (request.Status.HasValue)
                    ad.Status = (AdStatus)request.Status.Value;

                if (request.AdditionalBudget.HasValue)
                    ad.Budget += request.AdditionalBudget.Value;

                ad.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated advertisement {AdId}", adId);
                return Result.Of(MapToAdDto(ad));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating advertisement {AdId}", adId);
                return Result.Of<AdvertisementDto>().WithErrors("Failed to update advertisement");
            }
        }

        public async Task<Outcome<AdPerformanceDto>> GetAdPerformanceAsync(string adId)
        {
            try
            {
                var ad = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == adId);

                if (ad == null)
                    return Result.Of<AdPerformanceDto>().WithErrors("Advertisement not found");

                var ctr = ad.Impressions > 0 ? (decimal)ad.Clicks / ad.Impressions * 100 : 0;
                var cpc = ad.Clicks > 0 ? ad.SpentAmount / ad.Clicks : 0;

                return Result.Of(new AdPerformanceDto
                {
                    AdvertisementId = ad.AdvertisementId,
                    Title = ad.Title,
                    Impressions = ad.Impressions,
                    Clicks = ad.Clicks,
                    ClickThroughRate = Math.Round(ctr, 2),
                    CostPerClick = Math.Round(cpc, 2),
                    SpentAmount = ad.SpentAmount,
                    RemainingBudget = ad.Budget - ad.SpentAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ad performance for {AdId}", adId);
                return Result.Of<AdPerformanceDto>().WithErrors("Failed to retrieve ad performance");
            }
        }

        public async Task<Outcome<bool>> RecordImpressionAsync(string adId)
        {
            try
            {
                var ad = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == adId);

                if (ad == null || ad.Status != AdStatus.Active)
                    return Result.Of(false);

                ad.Impressions++;
                await _context.SaveChangesAsync();
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording impression for ad {AdId}", adId);
                return Result.Of(false);
            }
        }

        public async Task<Outcome<bool>> RecordClickAsync(string adId)
        {
            try
            {
                var ad = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == adId);

                if (ad == null || ad.Status != AdStatus.Active)
                    return Result.Of(false);

                ad.Clicks++;

                var costPerClick = ad.Budget > 0 && ad.Impressions > 0
                    ? ad.Budget / (ad.Impressions * 10m)
                    : 0.01m;
                ad.SpentAmount += costPerClick;

                if (ad.SpentAmount >= ad.Budget)
                    ad.Status = AdStatus.Paused;

                await _context.SaveChangesAsync();
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording click for ad {AdId}", adId);
                return Result.Of(false);
            }
        }

        // Helper methods
        private static SubscriptionPlanDto MapToPlanDto(SubscriptionPlan plan)
        {
            return new SubscriptionPlanDto
            {
                SubscriptionPlanId = plan.SubscriptionPlanId,
                Name = plan.Name,
                Description = plan.Description,
                Tier = (int)plan.Tier,
                TierName = plan.Tier.ToString(),
                BillingCycle = (int)plan.BillingCycle,
                BillingCycleName = plan.BillingCycle.ToString(),
                Price = plan.Price,
                PlatformFeePercentage = plan.PlatformFeePercentage,
                SearchRankingBoost = plan.SearchRankingBoost,
                MaxGroupSize = plan.MaxGroupSize,
                IsActive = plan.IsActive
            };
        }

        private static GuideSubscriptionDto MapToSubscriptionDto(GuideSubscription sub, SubscriptionPlan plan)
        {
            return new GuideSubscriptionDto
            {
                GuideSubscriptionId = sub.GuideSubscriptionId,
                GuideId = sub.GuideId,
                PlanName = plan?.Name,
                PlanTier = plan?.Tier.ToString(),
                Status = (int)sub.Status,
                StatusName = sub.Status.ToString(),
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                AutoRenew = sub.AutoRenew,
                Price = plan?.Price ?? 0,
                BillingCycle = plan?.BillingCycle.ToString()
            };
        }

        private static VisibilityBoostDto MapToBoostDto(VisibilityBoost boost)
        {
            return new VisibilityBoostDto
            {
                VisibilityBoostId = boost.VisibilityBoostId,
                GuideId = boost.GuideId,
                TourId = boost.TourId,
                BoostType = (int)boost.BoostType,
                BoostTypeName = boost.BoostType.ToString(),
                Status = (int)boost.Status,
                BoostMultiplier = boost.BoostMultiplier,
                StartDate = boost.StartDate,
                EndDate = boost.EndDate,
                Cost = boost.Cost
            };
        }

        private static AdvertisementDto MapToAdDto(Advertisement ad)
        {
            return new AdvertisementDto
            {
                AdvertisementId = ad.AdvertisementId,
                AdvertiserId = ad.AdvertiserId,
                Title = ad.Title,
                Content = ad.Content,
                ImageUrl = ad.ImageUrl,
                TargetUrl = ad.TargetUrl,
                TargetAudience = (int)ad.TargetAudience,
                TargetAudienceName = ad.TargetAudience.ToString(),
                Status = (int)ad.Status,
                StatusName = ad.Status.ToString(),
                Budget = ad.Budget,
                SpentAmount = ad.SpentAmount,
                Impressions = ad.Impressions,
                Clicks = ad.Clicks,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate
            };
        }
    }
}

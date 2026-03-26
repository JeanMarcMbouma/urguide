using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Premium
{
    // Subscription Plans
    public class SubscriptionPlanDto
    {
        public string SubscriptionPlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Tier { get; set; }
        public string TierName { get; set; }
        public int BillingCycle { get; set; }
        public string BillingCycleName { get; set; }
        public decimal Price { get; set; }
        public decimal PlatformFeePercentage { get; set; }
        public int SearchRankingBoost { get; set; }
        public int MaxGroupSize { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubscriptionPlanRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 1)]
        public int Tier { get; set; }

        [Required]
        [Range(0, 2)]
        public int BillingCycle { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal PlatformFeePercentage { get; set; }

        [Range(0, 1000)]
        public int SearchRankingBoost { get; set; }

        [Range(1, 10000)]
        public int MaxGroupSize { get; set; } = 3;
    }

    // Guide Subscriptions
    public class GuideSubscriptionDto
    {
        public string GuideSubscriptionId { get; set; }
        public string GuideId { get; set; }
        public string PlanName { get; set; }
        public string PlanTier { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; }
    }

    public class SubscribeRequest
    {
        [Required]
        public string SubscriptionPlanId { get; set; }

        public bool AutoRenew { get; set; } = true;
    }

    // Visibility Boosts
    public class VisibilityBoostDto
    {
        public string VisibilityBoostId { get; set; }
        public string GuideId { get; set; }
        public string TourId { get; set; }
        public int BoostType { get; set; }
        public string BoostTypeName { get; set; }
        public int Status { get; set; }
        public int BoostMultiplier { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Cost { get; set; }
    }

    public class CreateVisibilityBoostRequest
    {
        public string TourId { get; set; }

        [Required]
        [Range(0, 3)]
        public int BoostType { get; set; }

        [Required]
        [Range(1, 100)]
        public int DurationDays { get; set; }

        [Range(1, 10)]
        public int BoostMultiplier { get; set; } = 2;
    }

    // Advertisements
    public class AdvertisementDto
    {
        public string AdvertisementId { get; set; }
        public string AdvertiserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public int TargetAudience { get; set; }
        public string TargetAudienceName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public decimal Budget { get; set; }
        public decimal SpentAmount { get; set; }
        public int Impressions { get; set; }
        public int Clicks { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateAdvertisementRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(2000)]
        public string ImageUrl { get; set; }

        [StringLength(2000)]
        public string TargetUrl { get; set; }

        [Required]
        [Range(0, 4)]
        public int TargetAudience { get; set; }

        public string TargetRegionId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Budget { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class UpdateAdvertisementRequest
    {
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(2000)]
        public string ImageUrl { get; set; }

        [Range(0, 4)]
        public int? Status { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? AdditionalBudget { get; set; }
    }

    // Ad Performance
    public class AdPerformanceDto
    {
        public string AdvertisementId { get; set; }
        public string Title { get; set; }
        public int Impressions { get; set; }
        public int Clicks { get; set; }
        public decimal ClickThroughRate { get; set; }
        public decimal CostPerClick { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingBudget { get; set; }
    }
}

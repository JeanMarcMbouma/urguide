using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Premium
{
    public enum BillingCycle
    {
        Monthly = 0,
        Quarterly = 1,
        Yearly = 2
    }

    public enum PlanTier
    {
        Basic = 0,
        Premium = 1
    }

    public class SubscriptionPlan
    {
        public string SubscriptionPlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PlanTier Tier { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public decimal Price { get; set; }
        public decimal PlatformFeePercentage { get; set; }
        public int SearchRankingBoost { get; set; }
        public int MaxGroupSize { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<GuideSubscription> GuideSubscriptions { get; set; } = new List<GuideSubscription>();
    }
}

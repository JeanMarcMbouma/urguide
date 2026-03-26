using System;

namespace UrGuide.Data.Entities.Premium
{
    public enum GuideSubscriptionStatus
    {
        Active = 0,
        Expired = 1,
        Cancelled = 2,
        PastDue = 3,
        Trial = 4
    }

    public class GuideSubscription
    {
        public string GuideSubscriptionId { get; set; }
        public string GuideId { get; set; }
        public string SubscriptionPlanId { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public GuideSubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public string StripeSubscriptionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

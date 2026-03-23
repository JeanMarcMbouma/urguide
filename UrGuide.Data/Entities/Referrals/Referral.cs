using System;

namespace UrGuide.Data.Entities.Referrals
{
    public class Referral
    {
        public string Id { get; set; }
        public string ReferralCodeId { get; set; }
        public string ReferrerId { get; set; }
        public string ReferredUserId { get; set; }
        public ReferralStatus Status { get; set; }
        public decimal RewardAmount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? RewardedAt { get; set; }
        public virtual ReferralCode ReferralCode { get; set; }
    }

    public enum ReferralStatus
    {
        Pending = 0,
        Completed = 1,
        Rewarded = 2,
        Expired = 3
    }
}

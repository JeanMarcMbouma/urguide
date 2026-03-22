using System;
using UrGuide.Data.Entities.Referrals;

namespace UrGuide.Model.Referrals
{
    public class ReferralDto
    {
        public string Id { get; set; }
        public string ReferredUserId { get; set; }
        public ReferralStatus Status { get; set; }
        public decimal RewardAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

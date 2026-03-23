using System;

namespace UrGuide.Model.Referrals
{
    public class ReferralDto
    {
        public string Id { get; set; }
        public string ReferredUserId { get; set; }
        /// <summary>
        /// 0 = Pending, 1 = Completed, 2 = Rewarded, 3 = Expired
        /// </summary>
        public int Status { get; set; }
        public decimal RewardAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

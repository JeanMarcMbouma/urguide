using System.Collections.Generic;

namespace UrGuide.Model.Referrals
{
    public class ReferralDashboardDto
    {
        public string Code { get; set; }
        public int TotalReferrals { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal PendingRewards { get; set; }
        public List<ReferralDto> RecentReferrals { get; set; } = new List<ReferralDto>();
    }
}

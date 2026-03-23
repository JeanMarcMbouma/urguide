using System;

namespace UrGuide.Model.Referrals
{
    public class ReferralCodeDto
    {
        public string Code { get; set; }
        /// <summary>
        /// 0 = User, 1 = Guide
        /// </summary>
        public int Type { get; set; }
        public int TotalReferrals { get; set; }
        public decimal TotalEarnings { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

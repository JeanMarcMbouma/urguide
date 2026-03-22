using System;
using UrGuide.Data.Entities.Referrals;

namespace UrGuide.Model.Referrals
{
    public class ReferralCodeDto
    {
        public string Code { get; set; }
        public ReferralCodeType Type { get; set; }
        public int TotalReferrals { get; set; }
        public decimal TotalEarnings { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

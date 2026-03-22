using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Referrals
{
    public class ReferralCode
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public ReferralCodeType Type { get; set; }
        public int TotalReferrals { get; set; } = 0;
        public decimal TotalEarnings { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();
    }

    public enum ReferralCodeType
    {
        User = 0,
        Guide = 1
    }
}

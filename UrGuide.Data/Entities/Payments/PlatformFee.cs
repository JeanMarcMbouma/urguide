using System;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Entities.Payments
{
    public class PlatformFee
    {
        public string FeeId { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public Membership MembershipTier { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Calculates platform fee based on membership tier
        /// </summary>
        public static decimal CalculateFee(decimal amount, Membership membership)
        {
            var percentage = membership switch
            {
                Membership.Basic => 0.02m, // 2%
                Membership.Premium => 0.05m, // 5%
                Membership.Platinum => 0.05m, // 5%
                _ => 0.02m
            };
            return Math.Round(amount * percentage, 2);
        }
    }
}

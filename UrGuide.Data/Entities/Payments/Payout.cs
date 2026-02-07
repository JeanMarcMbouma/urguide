using System;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Payments
{
    public class Payout
    {
        public string PayoutId { get; set; }
        public string GuideId { get; set; }
        public virtual Author Guide { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public PayoutStatus Status { get; set; }
        public string StripePayoutId { get; set; }
        public string StripeAccountId { get; set; }
        public string Description { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FailureReason { get; set; }
    }

    public enum PayoutStatus
    {
        Pending = 0,
        Processing = 1,
        Paid = 2,
        Failed = 3,
        Cancelled = 4
    }
}

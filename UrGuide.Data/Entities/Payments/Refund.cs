using System;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Payments
{
    public class Refund
    {
        public string RefundId { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public RefundStatus Status { get; set; }
        public string StripeRefundId { get; set; }
        public string Reason { get; set; }
        public string RequestedBy { get; set; }
        public virtual User RequestedByUser { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FailureReason { get; set; }
    }

    public enum RefundStatus
    {
        Pending = 0,
        Processing = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4
    }
}

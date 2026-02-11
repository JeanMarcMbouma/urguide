using System;

namespace UrGuide.Data.Entities.Payments
{
    public class PaymentTransaction
    {
        public string TransactionId { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public string StripeTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Metadata { get; set; }
    }

    public enum TransactionType
    {
        Payment = 0,
        Refund = 1,
        Payout = 2,
        PlatformFee = 3,
        Adjustment = 4
    }
}

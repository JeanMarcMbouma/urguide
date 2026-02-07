using System;
using UrGuide.Data.Entities.Regions;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Payments
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string BookingId { get; set; }
        public virtual Tour.Booking Booking { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public virtual Currency Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string StripeCustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Description { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public decimal GuidePayout { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Metadata { get; set; }
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5,
        PartiallyRefunded = 6
    }

    public enum PaymentMethod
    {
        Card = 0,
        Wallet = 1,
        BankTransfer = 2
    }
}

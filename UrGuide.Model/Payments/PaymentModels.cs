using System;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Payments
{
    public class CreatePaymentRequest
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string PaymentMethodId { get; set; }
    }

    public class PaymentResponse
    {
        public string PaymentId { get; set; }
        public string ClientSecret { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public decimal GuidePayout { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentDetailsResponse
    {
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public string BookingId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public decimal GuidePayout { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

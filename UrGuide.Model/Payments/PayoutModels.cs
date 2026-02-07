using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Payments
{
    public class CreatePayoutRequest
    {
        [Required]
        public string GuideId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class PayoutResponse
    {
        public string PayoutId { get; set; }
        public string GuideId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string FailureReason { get; set; }
    }

    public class PayoutListResponse
    {
        public List<PayoutResponse> Payouts { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

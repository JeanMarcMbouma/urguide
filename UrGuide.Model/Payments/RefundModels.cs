using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Payments
{
    public class CreateRefundRequest
    {
        [Required]
        public string PaymentId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Amount { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Reason { get; set; }
    }

    public class RefundResponse
    {
        public string RefundId { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string FailureReason { get; set; }
    }

    public class RefundListResponse
    {
        public List<RefundResponse> Refunds { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

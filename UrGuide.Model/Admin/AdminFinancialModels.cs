using System;
using System.Collections.Generic;

namespace UrGuide.Model.Admin
{
    public class AdminTransactionListResponse
    {
        public List<AdminTransactionItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminTransactionItem
    {
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string BookingId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public decimal GuidePayout { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminPayoutListResponse
    {
        public List<AdminPayoutItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminPayoutItem
    {
        public string PayoutId { get; set; }
        public string GuideId { get; set; }
        public string GuideName { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string FailureReason { get; set; }
    }

    public class AdminRefundListResponse
    {
        public List<AdminRefundItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminRefundItem
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
    }

    public class FinancialFilterParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}

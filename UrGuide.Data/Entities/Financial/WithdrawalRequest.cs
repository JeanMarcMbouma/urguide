using System;

namespace UrGuide.Data.Entities.Financial
{
    public enum WithdrawalStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }

    public class WithdrawalRequest
    {
        public string WithdrawalRequestId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountHolderName { get; set; }
        public WithdrawalStatus Status { get; set; }
        public string TransactionReference { get; set; }
        public string FailureReason { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

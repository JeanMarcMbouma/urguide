using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Financial
{
    // Coin Wallet
    public class CoinWalletDto
    {
        public string CoinWalletId { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalEarned { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CoinTransactionDto
    {
        public string CoinTransactionId { get; set; }
        public decimal Amount { get; set; }
        public int TransactionType { get; set; }
        public string Description { get; set; }
        public string ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddCoinsRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class SpendCoinsRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string ReferenceId { get; set; }
    }

    // Withdrawal
    public class WithdrawalRequestDto
    {
        public string WithdrawalRequestId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public int Status { get; set; }
        public string TransactionReference { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CreateWithdrawalRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; }

        [Required]
        [StringLength(200)]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        public string RoutingNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string AccountHolderName { get; set; }
    }

    // Payout Schedule
    public class PayoutScheduleDto
    {
        public string PayoutScheduleId { get; set; }
        public string GuideId { get; set; }
        public int Frequency { get; set; }
        public decimal MinimumAmount { get; set; }
        public DateTime NextPayoutDate { get; set; }
        public DateTime? LastPayoutDate { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePayoutScheduleRequest
    {
        [Required]
        [Range(0, 3, ErrorMessage = "Frequency must be 0 (Weekly), 1 (BiWeekly), 2 (Monthly), or 3 (OnDemand)")]
        public int Frequency { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Minimum amount must be greater than 0")]
        public decimal MinimumAmount { get; set; } = 50.00m;
    }

    public class UpdatePayoutScheduleRequest
    {
        [Range(0, 3)]
        public int? Frequency { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? MinimumAmount { get; set; }

        public bool? Pause { get; set; }
    }

    // Financial Reporting
    public class FinancialReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalPayouts { get; set; }
        public decimal TotalPlatformFees { get; set; }
        public decimal TotalRefunds { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalActiveSubscriptions { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public List<RevenueByPeriodDto> RevenueByPeriod { get; set; } = new List<RevenueByPeriodDto>();
    }

    public class RevenueByPeriodDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Revenue { get; set; }
        public decimal Payouts { get; set; }
        public decimal PlatformFees { get; set; }
        public int TransactionCount { get; set; }
    }

    public class FinancialReportRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string GroupBy { get; set; } = "month";
    }
}

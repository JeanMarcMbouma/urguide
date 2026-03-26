using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Financial;
using UrGuide.Model.Financial;
using UrGuide.Model.Results;

namespace UrGuide.Services.Financial
{
    public class FinancialService : IFinancialService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<FinancialService> _logger;

        public FinancialService(UrGuideContext context, ILogger<FinancialService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Coin Wallet
        public async Task<Outcome<CoinWalletDto>> GetWalletAsync(string userId)
        {
            try
            {
                var wallet = await _context.CoinWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                {
                    wallet = CreateNewWallet(userId);
                    _context.CoinWallets.Add(wallet);
                    await _context.SaveChangesAsync();
                }

                return Result.Of(MapToWalletDto(wallet));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet for user {UserId}", userId);
                return Result.Of<CoinWalletDto>().WithErrors("Failed to retrieve wallet");
            }
        }

        public async Task<Outcome<CoinWalletDto>> AddCoinsAsync(string userId, AddCoinsRequest request)
        {
            try
            {
                var wallet = await _context.CoinWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                {
                    wallet = CreateNewWallet(userId);
                    _context.CoinWallets.Add(wallet);
                }

                wallet.Balance += request.Amount;
                wallet.TotalEarned += request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                var transaction = new CoinTransaction
                {
                    CoinTransactionId = Guid.NewGuid().ToString(),
                    CoinWalletId = wallet.CoinWalletId,
                    Amount = request.Amount,
                    TransactionType = CoinTransactionType.Purchase,
                    Description = request.Description ?? "Coins purchased",
                    CreatedAt = DateTime.UtcNow
                };
                _context.CoinTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Added {Amount} coins to wallet for user {UserId}", request.Amount, userId);
                return Result.Of(MapToWalletDto(wallet));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding coins for user {UserId}", userId);
                return Result.Of<CoinWalletDto>().WithErrors("Failed to add coins");
            }
        }

        public async Task<Outcome<CoinWalletDto>> SpendCoinsAsync(string userId, SpendCoinsRequest request)
        {
            try
            {
                var wallet = await _context.CoinWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                    return Result.Of<CoinWalletDto>().WithErrors("Wallet not found");

                if (wallet.Balance < request.Amount)
                    return Result.Of<CoinWalletDto>().WithErrors("Insufficient balance");

                wallet.Balance -= request.Amount;
                wallet.TotalSpent += request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                var transaction = new CoinTransaction
                {
                    CoinTransactionId = Guid.NewGuid().ToString(),
                    CoinWalletId = wallet.CoinWalletId,
                    Amount = -request.Amount,
                    TransactionType = CoinTransactionType.TourPayment,
                    Description = request.Description ?? "Coins spent",
                    ReferenceId = request.ReferenceId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CoinTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Spent {Amount} coins from wallet for user {UserId}", request.Amount, userId);
                return Result.Of(MapToWalletDto(wallet));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error spending coins for user {UserId}", userId);
                return Result.Of<CoinWalletDto>().WithErrors("Failed to spend coins");
            }
        }

        public async Task<Outcome<List<CoinTransactionDto>>> GetTransactionsAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var wallet = await _context.CoinWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                    return Result.Of(new List<CoinTransactionDto>());

                var transactions = await _context.CoinTransactions
                    .Where(t => t.CoinWalletId == wallet.CoinWalletId)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new CoinTransactionDto
                    {
                        CoinTransactionId = t.CoinTransactionId,
                        Amount = t.Amount,
                        TransactionType = (int)t.TransactionType,
                        Description = t.Description,
                        ReferenceId = t.ReferenceId,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return Result.Of(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions for user {UserId}", userId);
                return Result.Of<List<CoinTransactionDto>>().WithErrors("Failed to retrieve transactions");
            }
        }

        // Withdrawals
        public async Task<Outcome<WithdrawalRequestDto>> CreateWithdrawalAsync(string userId, CreateWithdrawalRequest request)
        {
            try
            {
                var withdrawal = new WithdrawalRequest
                {
                    WithdrawalRequestId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Amount = request.Amount,
                    CurrencyCode = request.CurrencyCode,
                    BankName = request.BankName,
                    AccountNumber = request.AccountNumber,
                    RoutingNumber = request.RoutingNumber,
                    AccountHolderName = request.AccountHolderName,
                    Status = WithdrawalStatus.Pending,
                    RequestedAt = DateTime.UtcNow
                };

                _context.WithdrawalRequests.Add(withdrawal);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created withdrawal request {WithdrawalId} for user {UserId}", withdrawal.WithdrawalRequestId, userId);
                return Result.Of(MapToWithdrawalDto(withdrawal));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating withdrawal for user {UserId}", userId);
                return Result.Of<WithdrawalRequestDto>().WithErrors("Failed to create withdrawal request");
            }
        }

        public async Task<Outcome<WithdrawalRequestDto>> GetWithdrawalAsync(string withdrawalId)
        {
            try
            {
                var withdrawal = await _context.WithdrawalRequests
                    .FirstOrDefaultAsync(w => w.WithdrawalRequestId == withdrawalId);

                if (withdrawal == null)
                    return Result.Of<WithdrawalRequestDto>().WithErrors("Withdrawal request not found");

                return Result.Of(MapToWithdrawalDto(withdrawal));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting withdrawal {WithdrawalId}", withdrawalId);
                return Result.Of<WithdrawalRequestDto>().WithErrors("Failed to retrieve withdrawal request");
            }
        }

        public async Task<Outcome<List<WithdrawalRequestDto>>> GetUserWithdrawalsAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var withdrawals = await _context.WithdrawalRequests
                    .Where(w => w.UserId == userId)
                    .OrderByDescending(w => w.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(w => MapToWithdrawalDto(w))
                    .ToListAsync();

                return Result.Of(withdrawals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting withdrawals for user {UserId}", userId);
                return Result.Of<List<WithdrawalRequestDto>>().WithErrors("Failed to retrieve withdrawal requests");
            }
        }

        public async Task<Outcome<WithdrawalRequestDto>> ProcessWithdrawalAsync(string withdrawalId)
        {
            try
            {
                var withdrawal = await _context.WithdrawalRequests
                    .FirstOrDefaultAsync(w => w.WithdrawalRequestId == withdrawalId);

                if (withdrawal == null)
                    return Result.Of<WithdrawalRequestDto>().WithErrors("Withdrawal request not found");

                if (withdrawal.Status != WithdrawalStatus.Pending)
                    return Result.Of<WithdrawalRequestDto>().WithErrors("Withdrawal request is not in pending status");

                withdrawal.Status = WithdrawalStatus.Processing;
                withdrawal.ProcessedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Processing withdrawal {WithdrawalId}", withdrawalId);
                return Result.Of(MapToWithdrawalDto(withdrawal));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal {WithdrawalId}", withdrawalId);
                return Result.Of<WithdrawalRequestDto>().WithErrors("Failed to process withdrawal request");
            }
        }

        public async Task<Outcome<bool>> CancelWithdrawalAsync(string withdrawalId)
        {
            try
            {
                var withdrawal = await _context.WithdrawalRequests
                    .FirstOrDefaultAsync(w => w.WithdrawalRequestId == withdrawalId);

                if (withdrawal == null)
                    return Result.Of(false).WithErrors("Withdrawal request not found");

                if (withdrawal.Status != WithdrawalStatus.Pending)
                    return Result.Of(false).WithErrors("Only pending withdrawals can be cancelled");

                withdrawal.Status = WithdrawalStatus.Cancelled;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cancelled withdrawal {WithdrawalId}", withdrawalId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling withdrawal {WithdrawalId}", withdrawalId);
                return Result.Of(false).WithErrors("Failed to cancel withdrawal request");
            }
        }

        // Payout Schedules
        public async Task<Outcome<PayoutScheduleDto>> CreatePayoutScheduleAsync(string guideId, CreatePayoutScheduleRequest request)
        {
            try
            {
                var existing = await _context.PayoutSchedules
                    .FirstOrDefaultAsync(p => p.GuideId == guideId && p.Status == PayoutScheduleStatus.Active);

                if (existing != null)
                    return Result.Of<PayoutScheduleDto>().WithErrors("An active payout schedule already exists");

                var frequency = (PayoutFrequency)request.Frequency;
                var nextDate = CalculateNextPayoutDate(frequency);

                var schedule = new PayoutSchedule
                {
                    PayoutScheduleId = Guid.NewGuid().ToString(),
                    GuideId = guideId,
                    Frequency = frequency,
                    MinimumAmount = request.MinimumAmount,
                    NextPayoutDate = nextDate,
                    Status = PayoutScheduleStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PayoutSchedules.Add(schedule);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created payout schedule for guide {GuideId}", guideId);
                return Result.Of(MapToScheduleDto(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payout schedule for guide {GuideId}", guideId);
                return Result.Of<PayoutScheduleDto>().WithErrors("Failed to create payout schedule");
            }
        }

        public async Task<Outcome<PayoutScheduleDto>> GetPayoutScheduleAsync(string guideId)
        {
            try
            {
                var schedule = await _context.PayoutSchedules
                    .FirstOrDefaultAsync(p => p.GuideId == guideId && p.Status != PayoutScheduleStatus.Cancelled);

                if (schedule == null)
                    return Result.Of<PayoutScheduleDto>().WithErrors("No payout schedule found");

                return Result.Of(MapToScheduleDto(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payout schedule for guide {GuideId}", guideId);
                return Result.Of<PayoutScheduleDto>().WithErrors("Failed to retrieve payout schedule");
            }
        }

        public async Task<Outcome<PayoutScheduleDto>> UpdatePayoutScheduleAsync(string guideId, UpdatePayoutScheduleRequest request)
        {
            try
            {
                var schedule = await _context.PayoutSchedules
                    .FirstOrDefaultAsync(p => p.GuideId == guideId && p.Status != PayoutScheduleStatus.Cancelled);

                if (schedule == null)
                    return Result.Of<PayoutScheduleDto>().WithErrors("No payout schedule found");

                if (request.Frequency.HasValue)
                {
                    schedule.Frequency = (PayoutFrequency)request.Frequency.Value;
                    schedule.NextPayoutDate = CalculateNextPayoutDate(schedule.Frequency);
                }

                if (request.MinimumAmount.HasValue)
                    schedule.MinimumAmount = request.MinimumAmount.Value;

                if (request.Pause.HasValue)
                    schedule.Status = request.Pause.Value ? PayoutScheduleStatus.Paused : PayoutScheduleStatus.Active;

                schedule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated payout schedule for guide {GuideId}", guideId);
                return Result.Of(MapToScheduleDto(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payout schedule for guide {GuideId}", guideId);
                return Result.Of<PayoutScheduleDto>().WithErrors("Failed to update payout schedule");
            }
        }

        // Financial Reporting
        public async Task<Outcome<FinancialReportDto>> GenerateFinancialReportAsync(FinancialReportRequest request)
        {
            try
            {
                var payments = await _context.Payments
                    .Where(p => p.CreatedAt >= request.StartDate && p.CreatedAt <= request.EndDate)
                    .ToListAsync();

                var payouts = await _context.Payouts
                    .Where(p => p.CreatedAt >= request.StartDate && p.CreatedAt <= request.EndDate)
                    .ToListAsync();

                var platformFees = await _context.PlatformFees
                    .Where(f => f.CreatedAt >= request.StartDate && f.CreatedAt <= request.EndDate)
                    .ToListAsync();

                var refunds = await _context.Refunds
                    .Where(r => r.CreatedAt >= request.StartDate && r.CreatedAt <= request.EndDate)
                    .ToListAsync();

                var totalRevenue = payments.Sum(p => p.Amount);
                var totalPayouts = payouts.Sum(p => p.Amount);
                var totalPlatformFees = platformFees.Sum(f => f.Amount);
                var totalRefunds = refunds.Sum(r => r.Amount);

                var report = new FinancialReportDto
                {
                    TotalRevenue = totalRevenue,
                    TotalPayouts = totalPayouts,
                    TotalPlatformFees = totalPlatformFees,
                    TotalRefunds = totalRefunds,
                    TotalTransactions = payments.Count,
                    AverageTransactionValue = payments.Count > 0 ? totalRevenue / payments.Count : 0,
                    ReportStartDate = request.StartDate,
                    ReportEndDate = request.EndDate,
                    RevenueByPeriod = GenerateRevenueByPeriod(payments, payouts, platformFees, request)
                };

                _logger.LogInformation("Generated financial report from {Start} to {End}", request.StartDate, request.EndDate);
                return Result.Of(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating financial report");
                return Result.Of<FinancialReportDto>().WithErrors("Failed to generate financial report");
            }
        }

        // Helper methods
        private static CoinWallet CreateNewWallet(string userId)
        {
            return new CoinWallet
            {
                CoinWalletId = Guid.NewGuid().ToString(),
                UserId = userId,
                Balance = 0,
                TotalEarned = 0,
                TotalSpent = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static DateTime CalculateNextPayoutDate(PayoutFrequency frequency)
        {
            return frequency switch
            {
                PayoutFrequency.Weekly => DateTime.UtcNow.AddDays(7),
                PayoutFrequency.BiWeekly => DateTime.UtcNow.AddDays(14),
                PayoutFrequency.Monthly => DateTime.UtcNow.AddMonths(1),
                PayoutFrequency.OnDemand => DateTime.MaxValue,
                _ => DateTime.UtcNow.AddMonths(1)
            };
        }

        private static List<RevenueByPeriodDto> GenerateRevenueByPeriod(
            List<UrGuide.Data.Entities.Payments.Payment> payments,
            List<UrGuide.Data.Entities.Payments.Payout> payouts,
            List<UrGuide.Data.Entities.Payments.PlatformFee> fees,
            FinancialReportRequest request)
        {
            var result = new List<RevenueByPeriodDto>();
            var current = request.StartDate;

            while (current < request.EndDate)
            {
                var periodEnd = request.GroupBy?.ToLowerInvariant() switch
                {
                    "week" => current.AddDays(7),
                    "day" => current.AddDays(1),
                    "year" => current.AddYears(1),
                    _ => current.AddMonths(1)
                };

                if (periodEnd > request.EndDate)
                    periodEnd = request.EndDate;

                var periodPayments = payments.Where(p => p.CreatedAt >= current && p.CreatedAt < periodEnd).ToList();
                var periodPayouts = payouts.Where(p => p.CreatedAt >= current && p.CreatedAt < periodEnd).ToList();
                var periodFees = fees.Where(f => f.CreatedAt >= current && f.CreatedAt < periodEnd).ToList();

                result.Add(new RevenueByPeriodDto
                {
                    PeriodStart = current,
                    PeriodEnd = periodEnd,
                    Revenue = periodPayments.Sum(p => p.Amount),
                    Payouts = periodPayouts.Sum(p => p.Amount),
                    PlatformFees = periodFees.Sum(f => f.Amount),
                    TransactionCount = periodPayments.Count
                });

                current = periodEnd;
            }

            return result;
        }

        private static CoinWalletDto MapToWalletDto(CoinWallet wallet)
        {
            return new CoinWalletDto
            {
                CoinWalletId = wallet.CoinWalletId,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                TotalEarned = wallet.TotalEarned,
                TotalSpent = wallet.TotalSpent,
                CreatedAt = wallet.CreatedAt
            };
        }

        private static WithdrawalRequestDto MapToWithdrawalDto(WithdrawalRequest withdrawal)
        {
            return new WithdrawalRequestDto
            {
                WithdrawalRequestId = withdrawal.WithdrawalRequestId,
                Amount = withdrawal.Amount,
                CurrencyCode = withdrawal.CurrencyCode,
                BankName = withdrawal.BankName,
                AccountHolderName = withdrawal.AccountHolderName,
                Status = (int)withdrawal.Status,
                TransactionReference = withdrawal.TransactionReference,
                RequestedAt = withdrawal.RequestedAt,
                ProcessedAt = withdrawal.ProcessedAt,
                CompletedAt = withdrawal.CompletedAt
            };
        }

        private static PayoutScheduleDto MapToScheduleDto(PayoutSchedule schedule)
        {
            return new PayoutScheduleDto
            {
                PayoutScheduleId = schedule.PayoutScheduleId,
                GuideId = schedule.GuideId,
                Frequency = (int)schedule.Frequency,
                MinimumAmount = schedule.MinimumAmount,
                NextPayoutDate = schedule.NextPayoutDate,
                LastPayoutDate = schedule.LastPayoutDate,
                Status = (int)schedule.Status,
                CreatedAt = schedule.CreatedAt
            };
        }
    }
}

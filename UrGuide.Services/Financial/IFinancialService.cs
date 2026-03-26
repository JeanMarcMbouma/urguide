using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Financial;

namespace UrGuide.Services.Financial
{
    public interface IFinancialService
    {
        // Coin Wallet
        Task<Outcome<CoinWalletDto>> GetWalletAsync(string userId);
        Task<Outcome<CoinWalletDto>> AddCoinsAsync(string userId, AddCoinsRequest request);
        Task<Outcome<CoinWalletDto>> SpendCoinsAsync(string userId, SpendCoinsRequest request);
        Task<Outcome<List<CoinTransactionDto>>> GetTransactionsAsync(string userId, int page = 1, int pageSize = 20);

        // Withdrawals
        Task<Outcome<WithdrawalRequestDto>> CreateWithdrawalAsync(string userId, CreateWithdrawalRequest request);
        Task<Outcome<WithdrawalRequestDto>> GetWithdrawalAsync(string withdrawalId, string userId);
        Task<Outcome<List<WithdrawalRequestDto>>> GetUserWithdrawalsAsync(string userId, int page = 1, int pageSize = 20);
        Task<Outcome<WithdrawalRequestDto>> ProcessWithdrawalAsync(string withdrawalId);
        Task<Outcome<WithdrawalRequestDto>> CompleteWithdrawalAsync(string withdrawalId, string transactionReference);
        Task<Outcome<WithdrawalRequestDto>> FailWithdrawalAsync(string withdrawalId, string failureReason);
        Task<Outcome<bool>> CancelWithdrawalAsync(string withdrawalId, string userId);

        // Payout Schedules
        Task<Outcome<PayoutScheduleDto>> CreatePayoutScheduleAsync(string guideId, CreatePayoutScheduleRequest request);
        Task<Outcome<PayoutScheduleDto>> GetPayoutScheduleAsync(string guideId);
        Task<Outcome<PayoutScheduleDto>> UpdatePayoutScheduleAsync(string guideId, UpdatePayoutScheduleRequest request);

        // Financial Reporting
        Task<Outcome<FinancialReportDto>> GenerateFinancialReportAsync(FinancialReportRequest request);
    }
}

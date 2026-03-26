using System.Collections.Generic;
using BbQ.Outcome;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Admin;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    /// <summary>
    /// Service interface for admin user management operations
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Get paginated list of all users with detailed information
        /// </summary>
        Task<Outcome<PagedList<AdminUserInfo>>> GetAllUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get detailed user information including roles and activity
        /// </summary>
        Task<Outcome<AdminUserInfo>> GetUserDetailAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Suspend a user account (lockout)
        /// </summary>
        Task<Outcome<bool>> SuspendUserAsync(string userId, int durationDays, CancellationToken cancellationToken);
        
        /// <summary>
        /// Activate a suspended user account
        /// </summary>
        Task<Outcome<bool>> ActivateUserAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Delete a user account (admin action)
        /// </summary>
        Task<Outcome<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Update user roles
        /// </summary>
        Task<Outcome<bool>> UpdateUserRolesAsync(UpdateUserRolesModel model, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get user activity history
        /// </summary>
        Task<Outcome<PagedList<UserActivityModel>>> GetUserActivityAsync(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get all available roles in the system
        /// </summary>
        Task<Outcome<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get pending guides awaiting verification
        /// </summary>
        Task<Outcome<PagedList<PendingGuideVerification>>> GetPendingGuidesAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get detailed guide verification information
        /// </summary>
        Task<Outcome<GuideVerificationDetail>> GetGuideVerificationDetailAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Approve or reject guide verification
        /// </summary>
        Task<Outcome<bool>> ProcessGuideVerificationAsync(GuideVerificationDecisionModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Get pending tour posts awaiting moderation
        /// </summary>
        Task<Outcome<PagedList<PendingTourModeration>>> GetPendingToursAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get detailed tour moderation information
        /// </summary>
        Task<Outcome<TourModerationDetail>> GetTourModerationDetailAsync(string postId, CancellationToken cancellationToken);

        /// <summary>
        /// Approve or reject tour post
        /// </summary>
        Task<Outcome<bool>> ProcessTourModerationAsync(TourModerationDecisionModel model, CancellationToken cancellationToken);

        // ── Financial Monitoring ──────────────────────────────────────────────

        /// <summary>
        /// Get paginated list of all payment transactions with optional filtering
        /// </summary>
        Task<Outcome<AdminTransactionListResponse>> GetAllTransactionsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all guide payout requests with optional filtering
        /// </summary>
        Task<Outcome<AdminPayoutListResponse>> GetAllPayoutsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all refund requests with optional filtering
        /// </summary>
        Task<Outcome<AdminRefundListResponse>> GetAllRefundsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        // ── System Monitoring ─────────────────────────────────────────────────

        /// <summary>
        /// Get system health status (database, storage, external services)
        /// </summary>
        Task<Outcome<SystemHealthStatus>> GetSystemHealthAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all audit log events with optional filtering
        /// </summary>
        Task<Outcome<AdminAuditLogResponse>> GetAllAuditLogsAsync(AuditLogFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all registered webhook subscriptions
        /// </summary>
        Task<Outcome<AdminWebhookListResponse>> GetAllWebhooksAsync(PaginationParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get current platform settings / feature toggles
        /// </summary>
        Task<Outcome<PlatformSettings>> GetPlatformSettingsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Update platform settings / feature toggles
        /// </summary>
        Task<Outcome<bool>> UpdatePlatformSettingsAsync(PlatformSettings settings, CancellationToken cancellationToken);

        // ── Account Freeze / Temporary Suspension ─────────────────────────────

        /// <summary>
        /// Freeze a user account with reason and optional duration
        /// </summary>
        Task<Outcome<AccountFreezeInfo>> FreezeAccountAsync(AccountFreezeRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Unfreeze a previously frozen user account
        /// </summary>
        Task<Outcome<bool>> UnfreezeAccountAsync(AccountUnfreezeRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Get freeze history for a specific user
        /// </summary>
        Task<Outcome<AccountFreezeHistoryResponse>> GetFreezeHistoryAsync(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get all currently frozen accounts
        /// </summary>
        Task<Outcome<AccountFreezeHistoryResponse>> GetFrozenAccountsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
    }
}

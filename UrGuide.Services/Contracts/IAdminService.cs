using System.Collections.Generic;
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
        Task<Result<PagedList<AdminUserInfo>>> GetAllUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get detailed user information including roles and activity
        /// </summary>
        Task<Result<AdminUserInfo>> GetUserDetailAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Suspend a user account (lockout)
        /// </summary>
        Task<Result<bool>> SuspendUserAsync(string userId, int durationDays, CancellationToken cancellationToken);
        
        /// <summary>
        /// Activate a suspended user account
        /// </summary>
        Task<Result<bool>> ActivateUserAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Delete a user account (admin action)
        /// </summary>
        Task<Result<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Update user roles
        /// </summary>
        Task<Result<bool>> UpdateUserRolesAsync(UpdateUserRolesModel model, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get user activity history
        /// </summary>
        Task<Result<PagedList<UserActivityModel>>> GetUserActivityAsync(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get all available roles in the system
        /// </summary>
        Task<Result<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get pending guides awaiting verification
        /// </summary>
        Task<Result<PagedList<PendingGuideVerification>>> GetPendingGuidesAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get detailed guide verification information
        /// </summary>
        Task<Result<GuideVerificationDetail>> GetGuideVerificationDetailAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Approve or reject guide verification
        /// </summary>
        Task<Result<bool>> ProcessGuideVerificationAsync(GuideVerificationDecisionModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Get pending tour posts awaiting moderation
        /// </summary>
        Task<Result<PagedList<PendingTourModeration>>> GetPendingToursAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get detailed tour moderation information
        /// </summary>
        Task<Result<TourModerationDetail>> GetTourModerationDetailAsync(string postId, CancellationToken cancellationToken);

        /// <summary>
        /// Approve or reject tour post
        /// </summary>
        Task<Result<bool>> ProcessTourModerationAsync(TourModerationDecisionModel model, CancellationToken cancellationToken);

        // ── Financial Monitoring ──────────────────────────────────────────────

        /// <summary>
        /// Get paginated list of all payment transactions with optional filtering
        /// </summary>
        Task<Result<AdminTransactionListResponse>> GetAllTransactionsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all guide payout requests with optional filtering
        /// </summary>
        Task<Result<AdminPayoutListResponse>> GetAllPayoutsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all refund requests with optional filtering
        /// </summary>
        Task<Result<AdminRefundListResponse>> GetAllRefundsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken);

        // ── System Monitoring ─────────────────────────────────────────────────

        /// <summary>
        /// Get system health status (database, storage, external services)
        /// </summary>
        Task<Result<SystemHealthStatus>> GetSystemHealthAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all audit log events with optional filtering
        /// </summary>
        Task<Result<AdminAuditLogResponse>> GetAllAuditLogsAsync(AuditLogFilterParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get paginated list of all registered webhook subscriptions
        /// </summary>
        Task<Result<AdminWebhookListResponse>> GetAllWebhooksAsync(PaginationParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get current platform settings / feature toggles
        /// </summary>
        Task<Result<PlatformSettings>> GetPlatformSettingsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Update platform settings / feature toggles
        /// </summary>
        Task<Result<bool>> UpdatePlatformSettingsAsync(PlatformSettings settings, CancellationToken cancellationToken);
    }
}

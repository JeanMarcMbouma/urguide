using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Admin;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Controller for admin user management operations (Admin only)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
        }

        /// <summary>
        /// Get paginated list of all users with filtering
        /// </summary>
        /// <param name="searchParameters">Search and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of users with admin details</returns>
        [HttpGet("users")]
        [ProducesResponseType(200, Type = typeof(PagedList<AdminUserInfo>))]
        public async Task<IActionResult> GetAllUsers([FromQuery] SearchParameters searchParameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllUsersAsync(searchParameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get detailed information for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed user information</returns>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(200, Type = typeof(AdminUserInfo))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserDetail(string userId, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetUserDetailAsync(userId, cancellationToken);

            if (result.IsError)
                return NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Suspend a user account (lockout)
        /// </summary>
        /// <param name="userId">User ID to suspend</param>
        /// <param name="durationDays">Duration of suspension in days (default: 30)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpPost("users/{userId}/suspend")]
        [ProducesResponseType(200)]
        
        public async Task<IActionResult> SuspendUser(string userId, [FromQuery] int durationDays = 30, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.SuspendUserAsync(userId, durationDays, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(new { message = $"User suspended for {durationDays} days", userId });
        }

        /// <summary>
        /// Activate a suspended user account
        /// </summary>
        /// <param name="userId">User ID to activate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpPost("users/{userId}/activate")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ActivateUser(string userId, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.ActivateUserAsync(userId, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(new { message = "User activated successfully", userId });
        }

        /// <summary>
        /// Delete a user account (permanent action)
        /// </summary>
        /// <param name="userId">User ID to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.DeleteUserAsync(userId, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(new { message = "User deleted successfully", userId });
        }

        /// <summary>
        /// Update user roles
        /// </summary>
        /// <param name="model">Update user roles model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpPut("users/roles")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesModel model, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.UpdateUserRolesAsync(model, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(new { message = "User roles updated successfully", model.UserId, roles = model.Roles });
        }

        /// <summary>
        /// Get user activity history
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="paginationParameters">Pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of user activities</returns>
        [HttpGet("users/{userId}/activity")]
        [ProducesResponseType(200, Type = typeof(PagedList<UserActivityModel>))]
        public async Task<IActionResult> GetUserActivity(string userId, [FromQuery] PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetUserActivityAsync(userId, paginationParameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all available roles in the system
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of role names</returns>
        [HttpGet("roles")]
        [ProducesResponseType(200, Type = typeof(System.Collections.Generic.List<string>))]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllRolesAsync(cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get pending guides awaiting verification
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of pending guides</returns>
        [HttpGet("guides/pending")]
        [ProducesResponseType(200, Type = typeof(PagedList<PendingGuideVerification>))]
        public async Task<IActionResult> GetPendingGuides([FromQuery] PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetPendingGuidesAsync(paginationParameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get detailed guide verification information
        /// </summary>
        /// <param name="userId">User ID of the guide</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed guide verification info</returns>
        [HttpGet("guides/{userId}/verification")]
        [ProducesResponseType(200, Type = typeof(GuideVerificationDetail))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGuideVerificationDetail(string userId, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetGuideVerificationDetailAsync(userId, cancellationToken);

            if (result.IsError)
                return NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Approve or reject guide verification
        /// </summary>
        /// <param name="model">Guide verification decision</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpPost("guides/verification")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ProcessGuideVerification([FromBody] GuideVerificationDecisionModel model, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.ProcessGuideVerificationAsync(model, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            var action = model.Approve ? "approved" : "rejected";
            return Ok(new { message = $"Guide verification {action}", userId = model.UserId });
        }

        /// <summary>
        /// Get pending tour posts awaiting moderation
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of pending tours</returns>
        [HttpGet("tours/pending")]
        [ProducesResponseType(200, Type = typeof(PagedList<PendingTourModeration>))]
        public async Task<IActionResult> GetPendingTours([FromQuery] PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetPendingToursAsync(paginationParameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get detailed tour moderation information
        /// </summary>
        /// <param name="postId">Post ID of the tour</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed tour moderation info</returns>
        [HttpGet("tours/{postId}/moderation")]
        [ProducesResponseType(200, Type = typeof(TourModerationDetail))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTourModerationDetail(string postId, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetTourModerationDetailAsync(postId, cancellationToken);

            if (result.IsError)
                return NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Approve or reject tour post
        /// </summary>
        /// <param name="model">Tour moderation decision</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        [HttpPost("tours/moderation")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ProcessTourModeration([FromBody] TourModerationDecisionModel model, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.ProcessTourModerationAsync(model, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            var action = model.Approve ? "approved" : "rejected";
            return Ok(new { message = $"Tour post {action}", postId = model.PostId });
        }

        // ── Financial Monitoring ──────────────────────────────────────────────

        /// <summary>
        /// Get all payment transactions with optional date and status filtering
        /// </summary>
        [HttpGet("financial/transactions")]
        [ProducesResponseType(200, Type = typeof(AdminTransactionListResponse))]
        public async Task<IActionResult> GetAllTransactions([FromQuery] FinancialFilterParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllTransactionsAsync(parameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all guide payout requests with optional date and status filtering
        /// </summary>
        [HttpGet("financial/payouts")]
        [ProducesResponseType(200, Type = typeof(AdminPayoutListResponse))]
        public async Task<IActionResult> GetAllPayouts([FromQuery] FinancialFilterParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllPayoutsAsync(parameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all refund requests with optional date and status filtering
        /// </summary>
        [HttpGet("financial/refunds")]
        [ProducesResponseType(200, Type = typeof(AdminRefundListResponse))]
        public async Task<IActionResult> GetAllRefunds([FromQuery] FinancialFilterParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllRefundsAsync(parameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        // ── System Monitoring ─────────────────────────────────────────────────

        /// <summary>
        /// Get system health status for all platform services
        /// </summary>
        [HttpGet("system/health")]
        [ProducesResponseType(200, Type = typeof(SystemHealthStatus))]
        public async Task<IActionResult> GetSystemHealth(CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetSystemHealthAsync(cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all platform audit log events with optional filtering
        /// </summary>
        [HttpGet("system/audit-logs")]
        [ProducesResponseType(200, Type = typeof(AdminAuditLogResponse))]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogFilterParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllAuditLogsAsync(parameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all registered webhook subscriptions
        /// </summary>
        [HttpGet("system/webhooks")]
        [ProducesResponseType(200, Type = typeof(AdminWebhookListResponse))]
        public async Task<IActionResult> GetAllWebhooks([FromQuery] PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetAllWebhooksAsync(paginationParameters, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Get current platform settings and feature toggles
        /// </summary>
        [HttpGet("system/settings")]
        [ProducesResponseType(200, Type = typeof(PlatformSettings))]
        public async Task<IActionResult> GetPlatformSettings(CancellationToken cancellationToken = default)
        {
            var result = await _adminService.GetPlatformSettingsAsync(cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(result.Value);
        }

        /// <summary>
        /// Update platform settings and feature toggles
        /// </summary>
        [HttpPut("system/settings")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdatePlatformSettings([FromBody] PlatformSettings settings, CancellationToken cancellationToken = default)
        {
            var result = await _adminService.UpdatePlatformSettingsAsync(settings, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            return Ok(new { message = "Platform settings updated successfully" });
        }
    }
}

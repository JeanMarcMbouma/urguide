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
    }
}

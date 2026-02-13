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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
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

            if (result.HasError)
                return NotFound(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
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

            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
        }
    }
}

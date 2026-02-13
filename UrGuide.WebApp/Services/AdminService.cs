using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Data;
using UrGuide.Model;
using UrGuide.Model.Admin;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// Admin service for user management operations
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UrGuideContext _context;
        private readonly IUserContext _userContext;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            UserManager<UrGuideUser> userManager,
            RoleManager<IdentityRole> roleManager,
            UrGuideContext context,
            IUserContext userContext,
            ILogger<AdminService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PagedList<AdminUserInfo>>> GetAllUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var query = _userManager.Users.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchParameters.Term))
                {
                    var searchTerm = searchParameters.Term.ToLower();
                    query = query.Where(u =>
                        u.Email!.ToLower().Contains(searchTerm) ||
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm));
                }

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply pagination - using default page size of 10
                const int pageSize = 10;
                var users = await query
                    .OrderByDescending(u => u.Id)
                    .Skip((searchParameters.PageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var adminUserInfoList = new List<AdminUserInfo>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var postCount = await _context.Posts.CountAsync(p => p.User.Id == user.Id, cancellationToken);
                    var tourCount = await _context.TourRequests.CountAsync(t => t.RequesterId == user.Id, cancellationToken);

                    adminUserInfoList.Add(new AdminUserInfo
                    {
                        Id = user.Id,
                        Email = user.Email ?? "",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsGuide = user.IsGuide,
                        EmailConfirmed = user.EmailConfirmed,
                        TwoFactorEnabled = user.TwoFactorEnabled,
                        LockoutEnabled = user.LockoutEnabled,
                        LockoutEnd = user.LockoutEnd,
                        AccessFailedCount = user.AccessFailedCount,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles.ToArray(),
                        PostCount = postCount,
                        TourCount = tourCount
                    });
                }

                var pagedList = PagedList.Of(adminUserInfoList, searchParameters.PageNumber);
                return Result.Of(pagedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return Result.Of<PagedList<AdminUserInfo>>().WithErrors("Failed to retrieve users");
            }
        }

        public async Task<Result<AdminUserInfo>> GetUserDetailAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Of<AdminUserInfo>().WithErrors("User not found");

                var roles = await _userManager.GetRolesAsync(user);
                var postCount = await _context.Posts.CountAsync(p => p.User.Id == userId, cancellationToken);
                var tourCount = await _context.TourRequests.CountAsync(t => t.RequesterId == userId, cancellationToken);

                var adminUserInfo = new AdminUserInfo
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsGuide = user.IsGuide,
                    EmailConfirmed = user.EmailConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToArray(),
                    PostCount = postCount,
                    TourCount = tourCount
                };

                return Result.Of(adminUserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user detail for {UserId}", userId);
                return Result.Of<AdminUserInfo>().WithErrors("Failed to retrieve user details");
            }
        }

        public async Task<Result<bool>> SuspendUserAsync(string userId, int durationDays, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Of(false).WithErrors("User not found");

                // Set lockout end date
                var lockoutEnd = DateTimeOffset.UtcNow.AddDays(durationDays);
                var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to suspend user {UserId}: {Errors}", userId, errors);
                    return Result.Of(false).WithErrors($"Failed to suspend user: {errors}");
                }

                // Enable lockout if not already enabled
                if (!user.LockoutEnabled)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                }

                _logger.LogInformation("User {UserId} suspended by admin {AdminId} for {Days} days", 
                    userId, _userContext.UserId, durationDays);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending user {UserId}", userId);
                return Result.Of(false).WithErrors("Failed to suspend user");
            }
        }

        public async Task<Result<bool>> ActivateUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Of(false).WithErrors("User not found");

                // Remove lockout
                var result = await _userManager.SetLockoutEndDateAsync(user, null);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to activate user {UserId}: {Errors}", userId, errors);
                    return Result.Of(false).WithErrors($"Failed to activate user: {errors}");
                }

                // Reset access failed count
                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("User {UserId} activated by admin {AdminId}", userId, _userContext.UserId);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return Result.Of(false).WithErrors("Failed to activate user");
            }
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Of(false).WithErrors("User not found");

                // Prevent deleting yourself
                if (user.Id == _userContext.UserId)
                    return Result.Of(false).WithErrors("Cannot delete your own account");

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to delete user {UserId}: {Errors}", userId, errors);
                    return Result.Of(false).WithErrors($"Failed to delete user: {errors}");
                }

                _logger.LogInformation("User {UserId} deleted by admin {AdminId}", userId, _userContext.UserId);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return Result.Of(false).WithErrors("Failed to delete user");
            }
        }

        public async Task<Result<bool>> UpdateUserRolesAsync(UpdateUserRolesModel model, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return Result.Of(false).WithErrors("User not found");

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove from all current roles
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("Failed to remove roles from user {UserId}: {Errors}", model.UserId, errors);
                        return Result.Of(false).WithErrors($"Failed to update roles: {errors}");
                    }
                }

                // Add to new roles
                if (model.Roles.Any())
                {
                    // Verify all roles exist
                    foreach (var role in model.Roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                            return Result.Of(false).WithErrors($"Role '{role}' does not exist");
                    }

                    var addResult = await _userManager.AddToRolesAsync(user, model.Roles);
                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("Failed to add roles to user {UserId}: {Errors}", model.UserId, errors);
                        return Result.Of(false).WithErrors($"Failed to update roles: {errors}");
                    }
                }

                _logger.LogInformation("User {UserId} roles updated by admin {AdminId}. New roles: {Roles}", 
                    model.UserId, _userContext.UserId, string.Join(", ", model.Roles));

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles for user {UserId}", model.UserId);
                return Result.Of(false).WithErrors("Failed to update user roles");
            }
        }

        public async Task<Result<PagedList<UserActivityModel>>> GetUserActivityAsync(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            try
            {
                // Query audit events for the user
                var query = _context.AuditEvents
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.Created);

                var totalCount = await query.CountAsync(cancellationToken);

                const int pageSize = 10;
                var activities = await query
                    .Skip((paginationParameters.PageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new UserActivityModel
                    {
                        UserId = a.UserId,
                        ActionType = a.EventCode.ToString(),
                        Description = $"Event: {a.EventCode} - Ref: {a.ReferenceId}",
                        Timestamp = a.Created,
                        IpAddress = "" // Not available in current AuditEvent model
                    })
                    .ToListAsync(cancellationToken);

                var pagedList = PagedList.Of(activities, paginationParameters.PageNumber);
                return Result.Of(pagedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user activity for {UserId}", userId);
                return Result.Of<PagedList<UserActivityModel>>().WithErrors("Failed to retrieve user activity");
            }
        }

        public async Task<Result<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToListAsync(cancellationToken);

                return Result.Of(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return Result.Of<List<string>>().WithErrors("Failed to retrieve roles");
            }
        }
    }
}

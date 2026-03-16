using Microsoft.AspNetCore.Identity;
using BbQ.Outcome;
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
        private readonly AdminPlatformSettings _platformSettings;

        public AdminService(
            UserManager<UrGuideUser> userManager,
            RoleManager<IdentityRole> roleManager,
            UrGuideContext context,
            IUserContext userContext,
            ILogger<AdminService> logger,
            AdminPlatformSettings platformSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _platformSettings = platformSettings ?? throw new ArgumentNullException(nameof(platformSettings));
        }

        public async Task<Outcome<PagedList<AdminUserInfo>>> GetAllUsersAsync(SearchParameters searchParameters, CancellationToken cancellationToken)
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

        public async Task<Outcome<AdminUserInfo>> GetUserDetailAsync(string userId, CancellationToken cancellationToken)
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

        public async Task<Outcome<bool>> SuspendUserAsync(string userId, int durationDays, CancellationToken cancellationToken)
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

        public async Task<Outcome<bool>> ActivateUserAsync(string userId, CancellationToken cancellationToken)
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

        public async Task<Outcome<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken)
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

        public async Task<Outcome<bool>> UpdateUserRolesAsync(UpdateUserRolesModel model, CancellationToken cancellationToken)
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

        public async Task<Outcome<PagedList<UserActivityModel>>> GetUserActivityAsync(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
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

        public async Task<Outcome<List<string>>> GetAllRolesAsync(CancellationToken cancellationToken)
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

        public async Task<Outcome<PagedList<PendingGuideVerification>>> GetPendingGuidesAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            try
            {
                // Get all guides (users with IsGuide = true)
                var query = _userManager.Users.Where(u => u.IsGuide);

                var totalCount = await query.CountAsync(cancellationToken);

                const int pageSize = 10;
                var guides = await query
                    .OrderByDescending(u => u.Id)
                    .Skip((paginationParameters.PageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var pendingGuides = new List<PendingGuideVerification>();

                foreach (var guide in guides)
                {
                    var user = await _context.Users
                        .Include(u => u.Attributes)
                        .Include(u => u.ProfileImage)
                        .FirstOrDefaultAsync(u => u.Id == guide.Id, cancellationToken);
                    
                    var postCount = await _context.Posts.CountAsync(p => p.User.Id == guide.Id, cancellationToken);
                    
                    // Extract attributes
                    var attributes = user?.Attributes?.ToList() ?? new List<UrGuide.Core.Attributes.GenericAttribute>();
                    var descriptionAttr = attributes.FirstOrDefault(a => a.Name == "Description");
                    var cityAttr = attributes.FirstOrDefault(a => a.Name == "City");
                    var countryAttr = attributes.FirstOrDefault(a => a.Name == "Country");
                    var addressAttr = attributes.FirstOrDefault(a => a.Name == "Address");
                    var genderAttr = attributes.FirstOrDefault(a => a.Name == "Gender");
                    var dobAttr = attributes.FirstOrDefault(a => a.Name == "DateOfBirth");

                    var submission = await _context.GuideVerificationSubmissions
                        .FirstOrDefaultAsync(s => s.GuideId == guide.Id, cancellationToken);

                    pendingGuides.Add(new PendingGuideVerification
                    {
                        UserId = guide.Id,
                        Email = guide.Email ?? "",
                        FullName = $"{guide.FirstName} {guide.LastName}",
                        PhoneNumber = guide.PhoneNumber,
                        ProfileImage = user?.ProfileImage?.ImageUrl ?? "",
                        Description = descriptionAttr?.Value ?? "",
                        Address = addressAttr?.Value ?? "",
                        City = cityAttr?.Value ?? "",
                        Country = countryAttr?.Value ?? "",
                        Gender = genderAttr?.Value ?? "",
                        DateOfBirth = dobAttr != null && DateTime.TryParse(dobAttr.Value, out var dob) ? dob : null,
                        RegisteredAt = user?.CreatedAt ?? DateTime.UtcNow,
                        Status = submission != null ? (GuideVerificationStatus)submission.Status : GuideVerificationStatus.Pending,
                        TourCount = postCount,
                        Documents = new string[] { }
                    });
                }

                var pagedList = PagedList.Of(pendingGuides, paginationParameters.PageNumber);
                return Result.Of(pagedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending guides");
                return Result.Of<PagedList<PendingGuideVerification>>().WithErrors("Failed to retrieve pending guides");
            }
        }

        public async Task<Outcome<GuideVerificationDetail>> GetGuideVerificationDetailAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var guide = await _userManager.FindByIdAsync(userId);
                if (guide == null || !guide.IsGuide)
                    return Result.Of<GuideVerificationDetail>().WithErrors("Guide not found");

                var user = await _context.Users
                    .Include(u => u.Attributes)
                    .Include(u => u.ProfileImage)
                    .Include(u => u.Feedback)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
                    
                var postCount = await _context.Posts.CountAsync(p => p.User.Id == userId, cancellationToken);

                // Extract attributes
                var attributes = user?.Attributes?.ToList() ?? new List<UrGuide.Core.Attributes.GenericAttribute>();
                var descriptionAttr = attributes.FirstOrDefault(a => a.Name == "Description");
                var cityAttr = attributes.FirstOrDefault(a => a.Name == "City");
                var countryAttr = attributes.FirstOrDefault(a => a.Name == "Country");
                var addressAttr = attributes.FirstOrDefault(a => a.Name == "Address");
                var genderAttr = attributes.FirstOrDefault(a => a.Name == "Gender");
                var dobAttr = attributes.FirstOrDefault(a => a.Name == "DateOfBirth");

                // Calculate average rating from feedback
                var feedbacks = user?.Feedback?.ToList() ?? new List<UrGuide.Data.Shared.Feedback>();
                var avgRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
                var reviewCount = feedbacks.Count;

                var submission = await _context.GuideVerificationSubmissions
                    .Include(s => s.Documents)
                    .FirstOrDefaultAsync(s => s.GuideId == userId, cancellationToken);

                var submissionDocuments = submission?.Documents.Select(d => new GuideDocument
                {
                    DocumentId = d.Id,
                    DocumentType = d.DocumentType,
                    DocumentUrl = d.FileName,
                    UploadedAt = d.UploadedAt,
                    Verified = d.Status == 1,
                }).ToArray() ?? new GuideDocument[] { };

                var detail = new GuideVerificationDetail
                {
                    UserId = userId,
                    Email = guide.Email ?? "",
                    FullName = $"{guide.FirstName} {guide.LastName}",
                    PhoneNumber = guide.PhoneNumber,
                    ProfileImage = user?.ProfileImage?.ImageUrl ?? "",
                    Description = descriptionAttr?.Value ?? "",
                    Address = addressAttr?.Value ?? "",
                    City = cityAttr?.Value ?? "",
                    Country = countryAttr?.Value ?? "",
                    Gender = genderAttr?.Value ?? "",
                    DateOfBirth = dobAttr != null && DateTime.TryParse(dobAttr.Value, out var dob) ? dob : null,
                    RegisteredAt = user?.CreatedAt ?? DateTime.UtcNow,
                    Status = submission != null ? (GuideVerificationStatus)submission.Status : GuideVerificationStatus.Pending,
                    TourCount = postCount,
                    AverageRating = (decimal)avgRating,
                    ReviewCount = reviewCount,
                    Checklist = new VerificationChecklist
                    {
                        ProfileComplete = !string.IsNullOrEmpty(descriptionAttr?.Value),
                        IdentityDocumentProvided = submissionDocuments.Length > 0,
                        ContactVerified = guide.EmailConfirmed && !string.IsNullOrEmpty(guide.PhoneNumber),
                        BackgroundCheckPassed = false,
                        ProfileDescriptionAdequate = descriptionAttr?.Value?.Length >= 100
                    },
                    Documents = submissionDocuments
                };

                return Result.Of(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guide verification detail for {UserId}", userId);
                return Result.Of<GuideVerificationDetail>().WithErrors("Failed to retrieve guide details");
            }
        }

        public async Task<Outcome<bool>> ProcessGuideVerificationAsync(GuideVerificationDecisionModel model, CancellationToken cancellationToken)
        {
            try
            {
                var guide = await _userManager.FindByIdAsync(model.UserId);
                if (guide == null || !guide.IsGuide)
                    return Result.Of(false).WithErrors("Guide not found");

                // In a full implementation, you would:
                // 1. Update verification status in a separate table
                // 2. Send notification to the guide
                // 3. Log the admin action

                if (model.Approve)
                {
                    // Add "VerifiedGuide" role if it exists
                    if (await _roleManager.RoleExistsAsync("VerifiedGuide"))
                    {
                        await _userManager.AddToRoleAsync(guide, "VerifiedGuide");
                    }

                    // Persist approval to verification submission
                    var submission = await _context.GuideVerificationSubmissions
                        .FirstOrDefaultAsync(s => s.GuideId == model.UserId, cancellationToken);
                    var now = DateTime.UtcNow;
                    if (submission != null)
                    {
                        submission.Status = (int)Model.Admin.GuideVerificationStatus.Approved;
                        submission.ReviewedAt = now;
                        submission.ReviewedByAdminId = _userContext.UserId;
                        submission.AdminNotes = model.AdminNotes;
                        submission.UpdatedAt = now;
                    }
                    else
                    {
                        await _context.GuideVerificationSubmissions.AddAsync(new UrGuide.Data.Entities.Users.GuideVerificationSubmission
                        {
                            Id = Guid.NewGuid().ToString(),
                            GuideId = model.UserId,
                            Status = (int)Model.Admin.GuideVerificationStatus.Approved,
                            SubmittedAt = now,
                            ReviewedAt = now,
                            ReviewedByAdminId = _userContext.UserId,
                            AdminNotes = model.AdminNotes,
                            CreatedAt = now,
                            UpdatedAt = now,
                        }, cancellationToken);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Guide {UserId} approved by admin {AdminId}", model.UserId, _userContext.UserId);
                }
                else
                {
                    // Persist rejection to verification submission
                    var submission = await _context.GuideVerificationSubmissions
                        .FirstOrDefaultAsync(s => s.GuideId == model.UserId, cancellationToken);
                    var now = DateTime.UtcNow;
                    if (submission != null)
                    {
                        submission.Status = (int)Model.Admin.GuideVerificationStatus.Rejected;
                        submission.ReviewedAt = now;
                        submission.ReviewedByAdminId = _userContext.UserId;
                        submission.RejectionReason = model.Reason;
                        submission.AdminNotes = model.AdminNotes;
                        submission.UpdatedAt = now;
                    }
                    else
                    {
                        await _context.GuideVerificationSubmissions.AddAsync(new UrGuide.Data.Entities.Users.GuideVerificationSubmission
                        {
                            Id = Guid.NewGuid().ToString(),
                            GuideId = model.UserId,
                            Status = (int)Model.Admin.GuideVerificationStatus.Rejected,
                            SubmittedAt = now,
                            ReviewedAt = now,
                            ReviewedByAdminId = _userContext.UserId,
                            RejectionReason = model.Reason,
                            AdminNotes = model.AdminNotes,
                            CreatedAt = now,
                            UpdatedAt = now,
                        }, cancellationToken);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Guide {UserId} rejected by admin {AdminId}. Reason: {Reason}",
                        model.UserId, _userContext.UserId, model.Reason);
                }

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing guide verification for {UserId}", model.UserId);
                return Result.Of(false).WithErrors("Failed to process guide verification");
            }
        }

        public async Task<Outcome<PagedList<PendingTourModeration>>> GetPendingToursAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            try
            {
                // Get all posts for moderation
                var query = _context.Posts.Include(p => p.User).AsQueryable();

                var totalCount = await query.CountAsync(cancellationToken);

                const int pageSize = 10;
                var posts = await query
                    .OrderByDescending(p => p.DateOfPublication)
                    .Skip((paginationParameters.PageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var pendingTours = posts.Select(post => new PendingTourModeration
                {
                    PostId = post.Id,
                    Title = post.Text ?? "Untitled",
                    Description = post.Description ?? "",
                    GuideId = post.User?.Id ?? "",
                    GuideName = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Unknown",
                    CreatedAt = post.DateOfPublication,
                    StartDate = post.StartDate,
                    EndDate = post.EndDate,
                    Location = post.GeoLocation ?? "",
                    Cost = decimal.TryParse(post.Cost, out var cost) ? cost : 0,
                    Status = TourModerationStatus.PendingReview,
                    Tags = post.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { },
                    Images = new string[] { }, // No direct image array
                    ReportCount = 0 // No reports table currently
                }).ToList();

                var pagedList = PagedList.Of(pendingTours, paginationParameters.PageNumber);
                return Result.Of(pagedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending tours");
                return Result.Of<PagedList<PendingTourModeration>>().WithErrors("Failed to retrieve pending tours");
            }
        }

        public async Task<Outcome<TourModerationDetail>> GetTourModerationDetailAsync(string postId, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Itineraries)
                    .Include(p => p.Reservations)
                    .Include(p => p.BidHistories)
                    .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

                if (post == null)
                    return Result.Of<TourModerationDetail>().WithErrors("Tour post not found");

                var bidCount = post.BidHistories?.Count ?? 0;

                var detail = new TourModerationDetail
                {
                    PostId = post.Id,
                    Title = post.Text ?? "Untitled",
                    Description = post.Description ?? "",
                    GuideId = post.User?.Id ?? "",
                    GuideName = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Unknown",
                    GuideEmail = post.User?.Email ?? "",
                    CreatedAt = post.DateOfPublication,
                    StartDate = post.StartDate,
                    EndDate = post.EndDate,
                    Location = post.GeoLocation ?? "",
                    Cost = decimal.TryParse(post.Cost, out var cost) ? cost : 0,
                    Status = TourModerationStatus.PendingReview,
                    Tags = post.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { },
                    Images = new string[] { },
                    BidCount = bidCount,
                    ReservationCount = post.Reservations?.Count ?? 0,
                    ReportCount = 0,
                    Itinerary = post.Itineraries?.Select(i => i.Description ?? "").ToArray() ?? new string[] { },
                    Violations = new ContentViolation[] { } // No violations table currently
                };

                return Result.Of(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour moderation detail for {PostId}", postId);
                return Result.Of<TourModerationDetail>().WithErrors("Failed to retrieve tour details");
            }
        }

        public async Task<Outcome<bool>> ProcessTourModerationAsync(TourModerationDecisionModel model, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == model.PostId, cancellationToken);

                if (post == null)
                    return Result.Of(false).WithErrors("Tour post not found");

                // In a full implementation, you would:
                // 1. Update moderation status in a separate table or add a status field to Post
                // 2. Send notification to the guide if requested
                // 3. Log the admin action
                // 4. If rejected, optionally hide/delete the post

                if (model.Approve)
                {
                    _logger.LogInformation("Tour post {PostId} approved by admin {AdminId}", model.PostId, _userContext.UserId);
                }
                else
                {
                    _logger.LogInformation("Tour post {PostId} rejected by admin {AdminId}. Reason: {Reason}", 
                        model.PostId, _userContext.UserId, model.Reason);
                    
                    // Optionally, you could delete or hide the post here
                    // For now, we'll just log it
                }

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tour moderation for {PostId}", model.PostId);
                return Result.Of(false).WithErrors("Failed to process tour moderation");
            }
        }

        // ── Financial Monitoring ──────────────────────────────────────────────

        public async Task<Outcome<AdminTransactionListResponse>> GetAllTransactionsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Payments
                    .Include(p => p.User)
                    .AsQueryable();

                if (parameters.StartDate.HasValue)
                    query = query.Where(p => p.CreatedAt >= parameters.StartDate.Value);

                if (parameters.EndDate.HasValue)
                    query = query.Where(p => p.CreatedAt <= parameters.EndDate.Value);

                if (!string.IsNullOrWhiteSpace(parameters.Status) &&
                    Enum.TryParse<UrGuide.Data.Entities.Payments.PaymentStatus>(parameters.Status, true, out var status))
                    query = query.Where(p => p.Status == status);

                var totalCount = await query.CountAsync(cancellationToken);

                var pageSize = parameters.PageSize > 0 ? parameters.PageSize : 20;
                var page = Math.Max(1, parameters.PageNumber);
                var items = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new AdminTransactionItem
                    {
                        PaymentId = p.PaymentId,
                        UserId = p.UserId,
                        UserEmail = p.User != null ? p.User.Email : string.Empty,
                        BookingId = p.BookingId,
                        Amount = p.Amount,
                        CurrencyCode = p.CurrencyCode,
                        Status = p.Status.ToString(),
                        PaymentMethod = p.PaymentMethod.ToString(),
                        Description = p.Description,
                        PlatformFeeAmount = p.PlatformFeeAmount,
                        GuidePayout = p.GuidePayout,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

                return Result.Of(new AdminTransactionListResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin transaction list");
                return Result.Of<AdminTransactionListResponse>().WithErrors("Failed to retrieve transactions");
            }
        }

        public async Task<Outcome<AdminPayoutListResponse>> GetAllPayoutsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Payouts
                    .Include(p => p.Guide)
                    .AsQueryable();

                if (parameters.StartDate.HasValue)
                    query = query.Where(p => p.RequestedAt >= parameters.StartDate.Value);

                if (parameters.EndDate.HasValue)
                    query = query.Where(p => p.RequestedAt <= parameters.EndDate.Value);

                if (!string.IsNullOrWhiteSpace(parameters.Status) &&
                    Enum.TryParse<UrGuide.Data.Entities.Payments.PayoutStatus>(parameters.Status, true, out var status))
                    query = query.Where(p => p.Status == status);

                var totalCount = await query.CountAsync(cancellationToken);

                var pageSize = parameters.PageSize > 0 ? parameters.PageSize : 20;
                var page = Math.Max(1, parameters.PageNumber);
                var items = await query
                    .OrderByDescending(p => p.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new AdminPayoutItem
                    {
                        PayoutId = p.PayoutId,
                        GuideId = p.GuideId,
                        GuideName = p.Guide != null && p.Guide.ProfileInfo != null ? p.Guide.ProfileInfo.FirstName : string.Empty,
                        Amount = p.Amount,
                        CurrencyCode = p.CurrencyCode,
                        Status = p.Status.ToString(),
                        RequestedAt = p.RequestedAt,
                        ProcessedAt = p.ProcessedAt,
                        FailureReason = p.FailureReason
                    })
                    .ToListAsync(cancellationToken);

                return Result.Of(new AdminPayoutListResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin payout list");
                return Result.Of<AdminPayoutListResponse>().WithErrors("Failed to retrieve payouts");
            }
        }

        public async Task<Outcome<AdminRefundListResponse>> GetAllRefundsAsync(FinancialFilterParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Refunds.AsQueryable();

                if (parameters.StartDate.HasValue)
                    query = query.Where(r => r.RequestedAt >= parameters.StartDate.Value);

                if (parameters.EndDate.HasValue)
                    query = query.Where(r => r.RequestedAt <= parameters.EndDate.Value);

                if (!string.IsNullOrWhiteSpace(parameters.Status) &&
                    Enum.TryParse<UrGuide.Data.Entities.Payments.RefundStatus>(parameters.Status, true, out var status))
                    query = query.Where(r => r.Status == status);

                var totalCount = await query.CountAsync(cancellationToken);

                var pageSize = parameters.PageSize > 0 ? parameters.PageSize : 20;
                var page = Math.Max(1, parameters.PageNumber);
                var items = await query
                    .OrderByDescending(r => r.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new AdminRefundItem
                    {
                        RefundId = r.RefundId,
                        PaymentId = r.PaymentId,
                        Amount = r.Amount,
                        CurrencyCode = r.CurrencyCode,
                        Status = r.Status.ToString(),
                        Reason = r.Reason,
                        RequestedBy = r.RequestedBy,
                        RequestedAt = r.RequestedAt,
                        ProcessedAt = r.ProcessedAt
                    })
                    .ToListAsync(cancellationToken);

                return Result.Of(new AdminRefundListResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin refund list");
                return Result.Of<AdminRefundListResponse>().WithErrors("Failed to retrieve refunds");
            }
        }

        // ── System Monitoring ─────────────────────────────────────────────────

        public async Task<Outcome<SystemHealthStatus>> GetSystemHealthAsync(CancellationToken cancellationToken)
        {
            var services = new System.Collections.Generic.List<ServiceHealthItem>();

            // Check database connectivity
            var dbService = new ServiceHealthItem { ServiceName = "Database" };
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                sw.Stop();
                if (canConnect)
                {
                    dbService.Status = "Healthy";
                    dbService.Message = "Connected";
                }
                else
                {
                    dbService.Status = "Unhealthy";
                    dbService.Message = "Cannot connect to database";
                }
                dbService.ResponseTimeMs = sw.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                dbService.Status = "Unhealthy";
                dbService.Message = "Database connection check failed";
                dbService.ResponseTimeMs = -1;
            }
            services.Add(dbService);

            // Check user store (Identity)
            var identityService = new ServiceHealthItem { ServiceName = "Identity" };
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                _ = await _userManager.Users.AnyAsync(cancellationToken);
                sw.Stop();
                identityService.Status = "Healthy";
                identityService.Message = "User store accessible";
                identityService.ResponseTimeMs = sw.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity health check failed");
                identityService.Status = "Unhealthy";
                identityService.Message = "Identity store check failed";
                identityService.ResponseTimeMs = -1;
            }
            services.Add(identityService);

            string overallStatus;
            if (services.Count == 0)
                overallStatus = "Unknown";
            else if (services.Exists(s => s.Status == "Unhealthy"))
                overallStatus = "Unhealthy";
            else if (services.TrueForAll(s => s.Status == "Healthy"))
                overallStatus = "Healthy";
            else
                overallStatus = "Degraded";

            return Result.Of(new SystemHealthStatus
            {
                OverallStatus = overallStatus,
                CheckedAt = DateTime.UtcNow,
                Services = services
            });
        }

        public async Task<Outcome<AdminAuditLogResponse>> GetAllAuditLogsAsync(AuditLogFilterParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.AuditEvents.AsQueryable();

                if (!string.IsNullOrWhiteSpace(parameters.UserId))
                    query = query.Where(e => e.UserId == parameters.UserId);

                if (parameters.StartDate.HasValue)
                    query = query.Where(e => e.Created >= parameters.StartDate.Value);

                if (parameters.EndDate.HasValue)
                    query = query.Where(e => e.Created <= parameters.EndDate.Value);

                if (!string.IsNullOrWhiteSpace(parameters.EventCode) &&
                    Enum.TryParse<UrGuide.Data.Entities.Event.EventCodes>(parameters.EventCode, true, out var eventCode))
                    query = query.Where(e => e.EventCode == eventCode);

                var totalCount = await query.CountAsync(cancellationToken);

                var pageSize = parameters.PageSize > 0 ? parameters.PageSize : 50;
                var page = Math.Max(1, parameters.PageNumber);

                // Fetch audit events and join with user email
                var events = await query
                    .OrderByDescending(e => e.Created)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var userIds = events.Select(e => e.UserId).Distinct().ToList();
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Email })
                    .ToListAsync(cancellationToken);
                var userEmailMap = users.ToDictionary(u => u.Id, u => u.Email ?? string.Empty);

                var items = events.Select(e => new AdminAuditLogItem
                {
                    Id = e.Id,
                    EventCode = e.EventCode.ToString(),
                    UserId = e.UserId,
                    UserEmail = e.UserId != null && userEmailMap.TryGetValue(e.UserId, out var email) ? email : string.Empty,
                    ReferenceId = e.ReferenceId,
                    Created = e.Created
                }).ToList();

                return Result.Of(new AdminAuditLogResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return Result.Of<AdminAuditLogResponse>().WithErrors("Failed to retrieve audit logs");
            }
        }

        public async Task<Outcome<AdminWebhookListResponse>> GetAllWebhooksAsync(PaginationParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.WebhookSubscriptions
                    .Include(w => w.User)
                    .AsQueryable();

                var totalCount = await query.CountAsync(cancellationToken);

                const int pageSize = 20;
                var page = Math.Max(1, parameters.PageNumber);
                var items = await query
                    .OrderByDescending(w => w.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(w => new AdminWebhookItem
                    {
                        Id = w.Id,
                        UserId = w.UserId,
                        UserEmail = w.User != null ? w.User.Email : string.Empty,
                        Url = w.Url,
                        IsActive = w.IsActive,
                        Description = w.Description,
                        SuccessCount = w.SuccessCount,
                        FailureCount = w.FailureCount,
                        CreatedAt = w.CreatedAt,
                        LastTriggeredAt = w.LastTriggeredAt
                    })
                    .ToListAsync(cancellationToken);

                return Result.Of(new AdminWebhookListResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving webhooks for admin");
                return Result.Of<AdminWebhookListResponse>().WithErrors("Failed to retrieve webhooks");
            }
        }

        public Task<Outcome<PlatformSettings>> GetPlatformSettingsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Of(_platformSettings.Get()));
        }

        public Task<Outcome<bool>> UpdatePlatformSettingsAsync(PlatformSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                var errors = new System.Collections.Generic.List<string>();

                if (settings.PlatformFeePercentage < 0 || settings.PlatformFeePercentage > 100)
                    errors.Add("PlatformFeePercentage must be between 0 and 100.");
                if (settings.MaxImagesPerPost <= 0)
                    errors.Add("MaxImagesPerPost must be greater than 0.");
                if (settings.MinBookingDaysAdvance < 0)
                    errors.Add("MinBookingDaysAdvance must be 0 or greater.");

                if (errors.Count > 0)
                    return Task.FromResult(Result.Of(false).WithErrors(errors.ToArray()));

                _platformSettings.Update(settings);
                _logger.LogInformation("Platform settings updated by admin {AdminId}", _userContext.UserId);
                return Task.FromResult(Result.Of(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating platform settings");
                return Task.FromResult(Result.Of(false).WithErrors("Failed to update platform settings"));
            }
        }
    }
}

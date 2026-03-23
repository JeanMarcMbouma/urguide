using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Tour;
using UrGuide.Model.Results;
using UrGuide.Model.Reviews;

namespace UrGuide.Services.Reviews
{
    public partial class ReviewModerationService : IReviewModerationService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<ReviewModerationService> _logger;

        [GeneratedRegex(@"https?://\S+", RegexOptions.IgnoreCase)]
        private static partial Regex UrlPattern();

        public ReviewModerationService(UrGuideContext context, ILogger<ReviewModerationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Outcome<bool>> FlagReviewAsync(string userId, string reviewId, FlagReviewRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Reason))
                    return Result.Of(false).WithErrors("Reason is required");

                var review = await _context.Set<Review>()
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

                if (review == null)
                    return Result.Of(false).WithErrors("Review not found");

                var existingFlag = await _context.ReviewFlags
                    .FirstOrDefaultAsync(f => f.ReviewId == reviewId && f.FlaggedBy == userId);

                if (existingFlag != null)
                    return Result.Of(false).WithErrors("You have already flagged this review");

                var flag = new ReviewFlag
                {
                    ReviewFlagId = Guid.NewGuid().ToString(),
                    ReviewId = reviewId,
                    FlaggedBy = userId,
                    Reason = request.Reason,
                    Description = request.Description ?? string.Empty,
                    Status = ReviewFlagStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReviewFlags.Add(flag);

                // Auto-flag the review if it has multiple flags (including the one just added)
                var flagCount = await _context.ReviewFlags.CountAsync(f => f.ReviewId == reviewId);
                if (flagCount + 1 >= 3 && review.ModerationStatus == ReviewModerationStatus.Approved)
                {
                    review.ModerationStatus = ReviewModerationStatus.FlaggedForReview;
                    review.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Review {ReviewId} flagged by user {UserId}", reviewId, userId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flagging review {ReviewId}", reviewId);
                return Result.Of(false).WithErrors("An error occurred while flagging the review");
            }
        }

        public async Task<Outcome<List<ModerationQueueItem>>> GetModerationQueueAsync(int page, int pageSize, string statusFilter)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _context.Set<Review>().AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(statusFilter) &&
                    Enum.TryParse<ReviewModerationStatus>(statusFilter, true, out var status))
                {
                    query = query.Where(r => r.ModerationStatus == status);
                }
                else
                {
                    query = query.Where(r =>
                        r.ModerationStatus == ReviewModerationStatus.Pending ||
                        r.ModerationStatus == ReviewModerationStatus.FlaggedForReview);
                }

                var items = await query
                    .OrderByDescending(r => r.Flags.Count)
                    .ThenByDescending(r => r.SpamScore)
                    .ThenBy(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new ModerationQueueItem
                    {
                        ReviewId = r.ReviewId,
                        ReviewText = r.Text,
                        Rating = r.Rating,
                        AuthorName = r.Author != null && r.Author.ProfileInfo != null
                            ? r.Author.ProfileInfo.FirstName
                            : string.Empty,
                        CreatedAt = r.CreatedAt,
                        ModerationStatus = (int)r.ModerationStatus,
                        FlagCount = r.Flags.Count,
                        SpamScore = r.SpamScore,
                        IsSpam = r.IsSpam
                    })
                    .ToListAsync();

                return Result.Of(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation queue");
                return Result.Of<List<ModerationQueueItem>>().WithErrors("An error occurred while retrieving the moderation queue");
            }
        }

        public async Task<Outcome<bool>> ModerateReviewAsync(string adminId, string reviewId, ReviewModerationResult action)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(action.Reason))
                    return Result.Of(false).WithErrors("Reason is required");

                var review = await _context.Set<Review>()
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

                if (review == null)
                    return Result.Of(false).WithErrors("Review not found");

                var previousContent = review.Text;
                var actionType = (ModerationActionType)action.ActionType;

                switch (actionType)
                {
                    case ModerationActionType.Approved:
                        review.ModerationStatus = ReviewModerationStatus.Approved;
                        break;
                    case ModerationActionType.Rejected:
                        review.ModerationStatus = ReviewModerationStatus.Rejected;
                        break;
                    case ModerationActionType.FlaggedForReview:
                        review.ModerationStatus = ReviewModerationStatus.FlaggedForReview;
                        break;
                    case ModerationActionType.Removed:
                        review.ModerationStatus = ReviewModerationStatus.Removed;
                        break;
                    case ModerationActionType.Restored:
                        review.ModerationStatus = ReviewModerationStatus.Approved;
                        break;
                }

                review.UpdatedAt = DateTime.UtcNow;

                var moderationAction = new ReviewModerationAction
                {
                    ActionId = Guid.NewGuid().ToString(),
                    ReviewId = reviewId,
                    ActionType = actionType,
                    PerformedBy = adminId,
                    Reason = action.Reason,
                    PreviousContent = previousContent,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReviewModerationActions.Add(moderationAction);

                // Resolve related flags
                var pendingFlags = await _context.ReviewFlags
                    .Where(f => f.ReviewId == reviewId && f.Status != ReviewFlagStatus.Resolved)
                    .ToListAsync();

                foreach (var flag in pendingFlags)
                {
                    flag.Status = ReviewFlagStatus.Resolved;
                    flag.ResolvedAt = DateTime.UtcNow;
                    flag.ResolvedBy = adminId;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Review {ReviewId} moderated by admin {AdminId} with action {ActionType}",
                    reviewId, adminId, actionType);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating review {ReviewId}", reviewId);
                return Result.Of(false).WithErrors("An error occurred while moderating the review");
            }
        }

        public async Task<Outcome<ModerationStatsDto>> GetModerationStatsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                var pendingCount = await _context.Set<Review>()
                    .CountAsync(r => r.ModerationStatus == ReviewModerationStatus.Pending);

                var flaggedCount = await _context.Set<Review>()
                    .CountAsync(r => r.ModerationStatus == ReviewModerationStatus.FlaggedForReview);

                var resolvedTodayCount = await _context.ReviewModerationActions
                    .CountAsync(a => a.CreatedAt >= today);

                var spamDetectedCount = await _context.Set<Review>()
                    .CountAsync(r => r.IsSpam);

                var stats = new ModerationStatsDto
                {
                    PendingCount = pendingCount,
                    FlaggedCount = flaggedCount,
                    ResolvedTodayCount = resolvedTodayCount,
                    SpamDetectedCount = spamDetectedCount
                };

                return Result.Of(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moderation stats");
                return Result.Of<ModerationStatsDto>().WithErrors("An error occurred while retrieving moderation stats");
            }
        }

        public Task<Outcome<decimal>> CheckForSpamAsync(string reviewText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewText))
                    return Task.FromResult(Result.Of(0m));

                decimal score = 0;

                // Check for excessive caps (more than 50% uppercase)
                var letters = reviewText.Where(char.IsLetter).ToList();
                if (letters.Count > 0)
                {
                    var uppercaseRatio = (decimal)letters.Count(char.IsUpper) / letters.Count;
                    if (uppercaseRatio > 0.5m)
                        score += 25;
                }

                // Check for URLs
                if (UrlPattern().IsMatch(reviewText))
                    score += 20;

                // Check for repeated words
                var words = reviewText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 3)
                {
                    var distinctRatio = (decimal)words.Distinct(StringComparer.OrdinalIgnoreCase).Count() / words.Length;
                    if (distinctRatio < 0.5m)
                        score += 30;
                }

                // Check for excessive exclamation/question marks
                var excessivePunctuation = reviewText.Count(c => c == '!' || c == '?');
                if (excessivePunctuation > 5)
                    score += 15;

                // Check for very short reviews (less than 10 characters)
                if (reviewText.Trim().Length < 10)
                    score += 10;

                // Cap score at 100
                score = Math.Min(score, 100);

                return Task.FromResult(Result.Of(score));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for spam");
                return Task.FromResult(Result.Of(0m).WithErrors("An error occurred while checking for spam"));
            }
        }

        public async Task<Outcome<bool>> SubmitAppealAsync(string userId, ReviewAppealRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ReviewId))
                    return Result.Of(false).WithErrors("Review ID is required");

                if (string.IsNullOrWhiteSpace(request.Reason))
                    return Result.Of(false).WithErrors("Reason is required");

                var review = await _context.Set<Review>()
                    .Include(r => r.Author)
                    .FirstOrDefaultAsync(r => r.ReviewId == request.ReviewId);

                if (review == null)
                    return Result.Of(false).WithErrors("Review not found");

                if (review.ModerationStatus != ReviewModerationStatus.Rejected &&
                    review.ModerationStatus != ReviewModerationStatus.Removed)
                {
                    return Result.Of(false).WithErrors("Only rejected or removed reviews can be appealed");
                }

                // Record the appeal as a moderation action
                var appealAction = new ReviewModerationAction
                {
                    ActionId = Guid.NewGuid().ToString(),
                    ReviewId = request.ReviewId,
                    ActionType = ModerationActionType.FlaggedForReview,
                    PerformedBy = userId,
                    Reason = $"Appeal: {request.Reason}. Evidence: {request.Evidence ?? "N/A"}",
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReviewModerationActions.Add(appealAction);

                // Move back to pending for re-review
                review.ModerationStatus = ReviewModerationStatus.Pending;
                review.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Appeal submitted for review {ReviewId} by user {UserId}", request.ReviewId, userId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting appeal for review {ReviewId}", request.ReviewId);
                return Result.Of(false).WithErrors("An error occurred while submitting the appeal");
            }
        }
    }
}

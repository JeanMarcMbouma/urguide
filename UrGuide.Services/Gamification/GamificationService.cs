using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Gamification;
using UrGuide.Model.Gamification;

namespace UrGuide.Services.Gamification
{
    public class GamificationService : IGamificationService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<GamificationService> _logger;

        private static readonly Dictionary<LoyaltyTier, int> TierDiscounts = new()
        {
            { LoyaltyTier.Bronze, 0 },
            { LoyaltyTier.Silver, 5 },
            { LoyaltyTier.Gold, 10 },
            { LoyaltyTier.Platinum, 15 }
        };

        private static readonly Dictionary<LoyaltyTier, int> TierThresholds = new()
        {
            { LoyaltyTier.Bronze, 0 },
            { LoyaltyTier.Silver, 500 },
            { LoyaltyTier.Gold, 2000 },
            { LoyaltyTier.Platinum, 5000 }
        };

        public GamificationService(UrGuideContext context, ILogger<GamificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Loyalty
        public async Task<Outcome<LoyaltyAccountDto>> GetLoyaltyAccountAsync(string userId)
        {
            try
            {
                var account = await GetOrCreateLoyaltyAccountAsync(userId);
                return Result.Of(MapToLoyaltyDto(account));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loyalty account for user {UserId}", userId);
                return Result.Of<LoyaltyAccountDto>().WithErrors("Failed to retrieve loyalty account");
            }
        }

        public async Task<Outcome<LoyaltyAccountDto>> EarnPointsAsync(string userId, EarnPointsRequest request)
        {
            try
            {
                var account = await GetOrCreateLoyaltyAccountAsync(userId);

                account.Points += request.Points;
                account.UpdatedAt = DateTime.UtcNow;

                UpdateTier(account);

                var transaction = new LoyaltyTransaction
                {
                    LoyaltyTransactionId = Guid.NewGuid().ToString(),
                    LoyaltyAccountId = account.LoyaltyAccountId,
                    Points = request.Points,
                    TransactionType = LoyaltyTransactionType.Earned,
                    Description = request.Description ?? "Points earned",
                    ReferenceId = request.ReferenceId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.LoyaltyTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} earned {Points} loyalty points", userId, request.Points);
                return Result.Of(MapToLoyaltyDto(account));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error earning points for user {UserId}", userId);
                return Result.Of<LoyaltyAccountDto>().WithErrors("Failed to earn points");
            }
        }

        public async Task<Outcome<LoyaltyAccountDto>> RedeemPointsAsync(string userId, RedeemPointsRequest request)
        {
            try
            {
                var account = await GetOrCreateLoyaltyAccountAsync(userId);

                if (account.Points < request.Points)
                    return Result.Of<LoyaltyAccountDto>().WithErrors("Insufficient points");

                account.Points -= request.Points;
                account.UpdatedAt = DateTime.UtcNow;

                UpdateTier(account);

                var transaction = new LoyaltyTransaction
                {
                    LoyaltyTransactionId = Guid.NewGuid().ToString(),
                    LoyaltyAccountId = account.LoyaltyAccountId,
                    Points = -request.Points,
                    TransactionType = LoyaltyTransactionType.Redeemed,
                    Description = request.Description ?? "Points redeemed",
                    CreatedAt = DateTime.UtcNow
                };
                _context.LoyaltyTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} redeemed {Points} loyalty points", userId, request.Points);
                return Result.Of(MapToLoyaltyDto(account));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming points for user {UserId}", userId);
                return Result.Of<LoyaltyAccountDto>().WithErrors("Failed to redeem points");
            }
        }

        public async Task<Outcome<List<LoyaltyTransactionDto>>> GetLoyaltyHistoryAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var account = await _context.LoyaltyAccounts
                    .FirstOrDefaultAsync(a => a.UserId == userId);

                if (account == null)
                    return Result.Of(new List<LoyaltyTransactionDto>());

                var transactions = await _context.LoyaltyTransactions
                    .Where(t => t.LoyaltyAccountId == account.LoyaltyAccountId)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new LoyaltyTransactionDto
                    {
                        LoyaltyTransactionId = t.LoyaltyTransactionId,
                        Points = t.Points,
                        TransactionType = (int)t.TransactionType,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return Result.Of(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loyalty history for user {UserId}", userId);
                return Result.Of<List<LoyaltyTransactionDto>>().WithErrors("Failed to retrieve loyalty history");
            }
        }

        // Badges
        public async Task<Outcome<BadgeDto>> CreateBadgeAsync(CreateBadgeRequest request)
        {
            try
            {
                var badge = new Badge
                {
                    BadgeId = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Description = request.Description,
                    IconUrl = request.IconUrl,
                    Tier = (BadgeTier)request.Tier,
                    Category = request.Category,
                    Criteria = request.Criteria,
                    ThresholdValue = request.ThresholdValue,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Badges.Add(badge);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created badge {BadgeName}", request.Name);
                return Result.Of(MapToBadgeDto(badge));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating badge");
                return Result.Of<BadgeDto>().WithErrors("Failed to create badge");
            }
        }

        public async Task<Outcome<List<BadgeDto>>> GetAllBadgesAsync()
        {
            try
            {
                var badges = await _context.Badges
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Tier)
                    .ThenBy(b => b.Name)
                    .Select(b => MapToBadgeDto(b))
                    .ToListAsync();

                return Result.Of(badges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all badges");
                return Result.Of<List<BadgeDto>>().WithErrors("Failed to retrieve badges");
            }
        }

        public async Task<Outcome<List<UserBadgeDto>>> GetUserBadgesAsync(string userId)
        {
            try
            {
                var badges = await _context.UserBadges
                    .Include(ub => ub.Badge)
                    .Where(ub => ub.UserId == userId)
                    .OrderByDescending(ub => ub.EarnedAt)
                    .Select(ub => new UserBadgeDto
                    {
                        UserBadgeId = ub.UserBadgeId,
                        BadgeName = ub.Badge.Name,
                        BadgeDescription = ub.Badge.Description,
                        BadgeIconUrl = ub.Badge.IconUrl,
                        BadgeTier = (int)ub.Badge.Tier,
                        BadgeTierName = ub.Badge.Tier.ToString(),
                        EarnedAt = ub.EarnedAt
                    })
                    .ToListAsync();

                return Result.Of(badges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting badges for user {UserId}", userId);
                return Result.Of<List<UserBadgeDto>>().WithErrors("Failed to retrieve user badges");
            }
        }

        public async Task<Outcome<UserBadgeDto>> AwardBadgeAsync(string userId, string badgeId)
        {
            try
            {
                var existing = await _context.UserBadges
                    .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);

                if (existing != null)
                    return Result.Of<UserBadgeDto>().WithErrors("User already has this badge");

                var badge = await _context.Badges.FirstOrDefaultAsync(b => b.BadgeId == badgeId);
                if (badge == null)
                    return Result.Of<UserBadgeDto>().WithErrors("Badge not found");

                var userBadge = new UserBadge
                {
                    UserBadgeId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    BadgeId = badgeId,
                    EarnedAt = DateTime.UtcNow
                };

                _context.UserBadges.Add(userBadge);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Awarded badge {BadgeName} to user {UserId}", badge.Name, userId);
                return Result.Of(new UserBadgeDto
                {
                    UserBadgeId = userBadge.UserBadgeId,
                    BadgeName = badge.Name,
                    BadgeDescription = badge.Description,
                    BadgeIconUrl = badge.IconUrl,
                    BadgeTier = (int)badge.Tier,
                    BadgeTierName = badge.Tier.ToString(),
                    EarnedAt = userBadge.EarnedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error awarding badge to user {UserId}", userId);
                return Result.Of<UserBadgeDto>().WithErrors("Failed to award badge");
            }
        }

        // Lottery
        public async Task<Outcome<LotteryDrawDto>> CreateLotteryDrawAsync(CreateLotteryDrawRequest request)
        {
            try
            {
                var draw = new LotteryDraw
                {
                    LotteryDrawId = Guid.NewGuid().ToString(),
                    TourId = request.TourId,
                    Title = request.Title,
                    Description = request.Description,
                    MaxEntries = request.MaxEntries,
                    WinnerCount = request.WinnerCount,
                    Status = LotteryStatus.Open,
                    EntryDeadline = request.EntryDeadline,
                    DrawDate = request.DrawDate,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LotteryDraws.Add(draw);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created lottery draw {DrawId}", draw.LotteryDrawId);
                return Result.Of(MapToLotteryDto(draw));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lottery draw");
                return Result.Of<LotteryDrawDto>().WithErrors("Failed to create lottery draw");
            }
        }

        public async Task<Outcome<LotteryDrawDto>> GetLotteryDrawAsync(string drawId)
        {
            try
            {
                var draw = await _context.LotteryDraws
                    .Include(d => d.Entries)
                    .FirstOrDefaultAsync(d => d.LotteryDrawId == drawId);

                if (draw == null)
                    return Result.Of<LotteryDrawDto>().WithErrors("Lottery draw not found");

                return Result.Of(MapToLotteryDto(draw));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lottery draw {DrawId}", drawId);
                return Result.Of<LotteryDrawDto>().WithErrors("Failed to retrieve lottery draw");
            }
        }

        public async Task<Outcome<List<LotteryDrawDto>>> GetActiveLotteriesAsync()
        {
            try
            {
                var draws = await _context.LotteryDraws
                    .Include(d => d.Entries)
                    .Where(d => d.Status == LotteryStatus.Open && d.EntryDeadline > DateTime.UtcNow)
                    .OrderBy(d => d.DrawDate)
                    .ToListAsync();

                return Result.Of(draws.Select(MapToLotteryDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active lotteries");
                return Result.Of<List<LotteryDrawDto>>().WithErrors("Failed to retrieve active lotteries");
            }
        }

        public async Task<Outcome<bool>> EnterLotteryAsync(string userId, string drawId)
        {
            try
            {
                var draw = await _context.LotteryDraws
                    .Include(d => d.Entries)
                    .FirstOrDefaultAsync(d => d.LotteryDrawId == drawId);

                if (draw == null)
                    return Result.Of(false).WithErrors("Lottery draw not found");

                if (draw.Status != LotteryStatus.Open)
                    return Result.Of(false).WithErrors("Lottery is not open for entries");

                if (draw.EntryDeadline <= DateTime.UtcNow)
                    return Result.Of(false).WithErrors("Entry deadline has passed");

                if (draw.Entries.Count >= draw.MaxEntries)
                    return Result.Of(false).WithErrors("Lottery is full");

                if (draw.Entries.Any(e => e.UserId == userId))
                    return Result.Of(false).WithErrors("User has already entered this lottery");

                var entry = new LotteryEntry
                {
                    LotteryEntryId = Guid.NewGuid().ToString(),
                    LotteryDrawId = drawId,
                    UserId = userId,
                    IsWinner = false,
                    EnteredAt = DateTime.UtcNow
                };

                _context.LotteryEntries.Add(entry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} entered lottery {DrawId}", userId, drawId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error entering lottery for user {UserId}", userId);
                return Result.Of(false).WithErrors("Failed to enter lottery");
            }
        }

        public async Task<Outcome<LotteryDrawDto>> DrawWinnersAsync(string drawId)
        {
            try
            {
                var draw = await _context.LotteryDraws
                    .Include(d => d.Entries)
                    .FirstOrDefaultAsync(d => d.LotteryDrawId == drawId);

                if (draw == null)
                    return Result.Of<LotteryDrawDto>().WithErrors("Lottery draw not found");

                if (draw.Status == LotteryStatus.Drawn)
                    return Result.Of<LotteryDrawDto>().WithErrors("Winners have already been drawn");

                var entries = draw.Entries.ToList();
                var winnerCount = Math.Min(draw.WinnerCount, entries.Count);

                // Cryptographically secure random selection
                var selectedIndices = new HashSet<int>();
                while (selectedIndices.Count < winnerCount)
                {
                    var index = RandomNumberGenerator.GetInt32(entries.Count);
                    selectedIndices.Add(index);
                }

                foreach (var index in selectedIndices)
                {
                    entries[index].IsWinner = true;
                }

                draw.Status = LotteryStatus.Drawn;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Drew {WinnerCount} winners for lottery {DrawId}", winnerCount, drawId);
                return Result.Of(MapToLotteryDto(draw));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error drawing winners for lottery {DrawId}", drawId);
                return Result.Of<LotteryDrawDto>().WithErrors("Failed to draw winners");
            }
        }

        // Achievements
        public async Task<Outcome<AchievementDto>> CreateAchievementAsync(CreateAchievementRequest request)
        {
            try
            {
                var achievement = new Achievement
                {
                    AchievementId = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Description = request.Description,
                    IconUrl = request.IconUrl,
                    Category = request.Category,
                    ThresholdValue = request.ThresholdValue,
                    PointsReward = request.PointsReward,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Achievements.Add(achievement);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created achievement {AchievementName}", request.Name);
                return Result.Of(MapToAchievementDto(achievement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating achievement");
                return Result.Of<AchievementDto>().WithErrors("Failed to create achievement");
            }
        }

        public async Task<Outcome<List<AchievementDto>>> GetAllAchievementsAsync()
        {
            try
            {
                var achievements = await _context.Achievements
                    .Where(a => a.IsActive)
                    .OrderBy(a => a.Category)
                    .ThenBy(a => a.Name)
                    .Select(a => MapToAchievementDto(a))
                    .ToListAsync();

                return Result.Of(achievements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all achievements");
                return Result.Of<List<AchievementDto>>().WithErrors("Failed to retrieve achievements");
            }
        }

        public async Task<Outcome<List<UserAchievementDto>>> GetUserAchievementsAsync(string userId)
        {
            try
            {
                var achievements = await _context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .Where(ua => ua.UserId == userId)
                    .OrderByDescending(ua => ua.UpdatedAt)
                    .Select(ua => new UserAchievementDto
                    {
                        UserAchievementId = ua.UserAchievementId,
                        AchievementName = ua.Achievement.Name,
                        AchievementDescription = ua.Achievement.Description,
                        AchievementIconUrl = ua.Achievement.IconUrl,
                        Category = ua.Achievement.Category,
                        Progress = ua.Progress,
                        ThresholdValue = ua.Achievement.ThresholdValue,
                        IsCompleted = ua.IsCompleted,
                        CompletedAt = ua.CompletedAt
                    })
                    .ToListAsync();

                return Result.Of(achievements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting achievements for user {UserId}", userId);
                return Result.Of<List<UserAchievementDto>>().WithErrors("Failed to retrieve user achievements");
            }
        }

        public async Task<Outcome<UserAchievementDto>> UpdateProgressAsync(string userId, UpdateProgressRequest request)
        {
            try
            {
                var achievement = await _context.Achievements
                    .FirstOrDefaultAsync(a => a.AchievementId == request.AchievementId && a.IsActive);

                if (achievement == null)
                    return Result.Of<UserAchievementDto>().WithErrors("Achievement not found");

                var userAchievement = await _context.UserAchievements
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == request.AchievementId);

                if (userAchievement == null)
                {
                    userAchievement = new UserAchievement
                    {
                        UserAchievementId = Guid.NewGuid().ToString(),
                        UserId = userId,
                        AchievementId = request.AchievementId,
                        Progress = 0,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.UserAchievements.Add(userAchievement);
                }

                if (userAchievement.IsCompleted)
                    return Result.Of<UserAchievementDto>().WithErrors("Achievement already completed");

                userAchievement.Progress += request.ProgressIncrement;
                userAchievement.UpdatedAt = DateTime.UtcNow;

                if (userAchievement.Progress >= achievement.ThresholdValue)
                {
                    userAchievement.IsCompleted = true;
                    userAchievement.CompletedAt = DateTime.UtcNow;
                    _logger.LogInformation("User {UserId} completed achievement {AchievementName}", userId, achievement.Name);
                }

                await _context.SaveChangesAsync();

                return Result.Of(new UserAchievementDto
                {
                    UserAchievementId = userAchievement.UserAchievementId,
                    AchievementName = achievement.Name,
                    AchievementDescription = achievement.Description,
                    AchievementIconUrl = achievement.IconUrl,
                    Category = achievement.Category,
                    Progress = userAchievement.Progress,
                    ThresholdValue = achievement.ThresholdValue,
                    IsCompleted = userAchievement.IsCompleted,
                    CompletedAt = userAchievement.CompletedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress for user {UserId}", userId);
                return Result.Of<UserAchievementDto>().WithErrors("Failed to update progress");
            }
        }

        // Dashboard
        public async Task<Outcome<GamificationDashboardDto>> GetDashboardAsync(string userId)
        {
            try
            {
                var loyaltyResult = await GetLoyaltyAccountAsync(userId);
                var badgesResult = await GetUserBadgesAsync(userId);
                var achievementsResult = await GetUserAchievementsAsync(userId);
                var lotteriesResult = await GetActiveLotteriesAsync();

                var dashboard = new GamificationDashboardDto
                {
                    Loyalty = loyaltyResult.IsSuccessful ? loyaltyResult.Value : null,
                    Badges = badgesResult.IsSuccessful ? badgesResult.Value : new List<UserBadgeDto>(),
                    Achievements = achievementsResult.IsSuccessful ? achievementsResult.Value : new List<UserAchievementDto>(),
                    ActiveLotteries = lotteriesResult.IsSuccessful ? lotteriesResult.Value : new List<LotteryDrawDto>()
                };

                return Result.Of(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gamification dashboard for user {UserId}", userId);
                return Result.Of<GamificationDashboardDto>().WithErrors("Failed to retrieve gamification dashboard");
            }
        }

        // Helper methods
        private async Task<LoyaltyAccount> GetOrCreateLoyaltyAccountAsync(string userId)
        {
            var account = await _context.LoyaltyAccounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                account = new LoyaltyAccount
                {
                    LoyaltyAccountId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Points = 0,
                    Tier = LoyaltyTier.Bronze,
                    DiscountPercentage = 0,
                    TotalToursCompleted = 0,
                    TotalSpent = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.LoyaltyAccounts.Add(account);
                await _context.SaveChangesAsync();
            }

            return account;
        }

        private static void UpdateTier(LoyaltyAccount account)
        {
            var newTier = LoyaltyTier.Bronze;

            foreach (var threshold in TierThresholds.OrderByDescending(t => t.Value))
            {
                if (account.Points >= threshold.Value)
                {
                    newTier = threshold.Key;
                    break;
                }
            }

            account.Tier = newTier;
            account.DiscountPercentage = TierDiscounts[newTier];
        }

        private static LoyaltyAccountDto MapToLoyaltyDto(LoyaltyAccount account)
        {
            return new LoyaltyAccountDto
            {
                LoyaltyAccountId = account.LoyaltyAccountId,
                UserId = account.UserId,
                Points = account.Points,
                Tier = (int)account.Tier,
                TierName = account.Tier.ToString(),
                DiscountPercentage = account.DiscountPercentage,
                TotalToursCompleted = account.TotalToursCompleted,
                TotalSpent = account.TotalSpent,
                CreatedAt = account.CreatedAt
            };
        }

        private static BadgeDto MapToBadgeDto(Badge badge)
        {
            return new BadgeDto
            {
                BadgeId = badge.BadgeId,
                Name = badge.Name,
                Description = badge.Description,
                IconUrl = badge.IconUrl,
                Tier = (int)badge.Tier,
                TierName = badge.Tier.ToString(),
                Category = badge.Category,
                ThresholdValue = badge.ThresholdValue,
                CreatedAt = badge.CreatedAt
            };
        }

        private static LotteryDrawDto MapToLotteryDto(LotteryDraw draw)
        {
            var dto = new LotteryDrawDto
            {
                LotteryDrawId = draw.LotteryDrawId,
                TourId = draw.TourId,
                Title = draw.Title,
                Description = draw.Description,
                MaxEntries = draw.MaxEntries,
                CurrentEntries = draw.Entries?.Count ?? 0,
                WinnerCount = draw.WinnerCount,
                Status = (int)draw.Status,
                EntryDeadline = draw.EntryDeadline,
                DrawDate = draw.DrawDate,
                CreatedAt = draw.CreatedAt
            };

            if (draw.Status == LotteryStatus.Drawn && draw.Entries != null)
            {
                dto.Winners = draw.Entries
                    .Where(e => e.IsWinner)
                    .Select(e => new LotteryWinnerDto { UserId = e.UserId, EnteredAt = e.EnteredAt })
                    .ToList();
            }

            return dto;
        }

        private static AchievementDto MapToAchievementDto(Achievement achievement)
        {
            return new AchievementDto
            {
                AchievementId = achievement.AchievementId,
                Name = achievement.Name,
                Description = achievement.Description,
                IconUrl = achievement.IconUrl,
                Category = achievement.Category,
                ThresholdValue = achievement.ThresholdValue,
                PointsReward = achievement.PointsReward,
                CreatedAt = achievement.CreatedAt
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Gamification
{
    // Loyalty
    public class LoyaltyAccountDto
    {
        public string LoyaltyAccountId { get; set; }
        public string UserId { get; set; }
        public int Points { get; set; }
        public int Tier { get; set; }
        public string TierName { get; set; }
        public int DiscountPercentage { get; set; }
        public int TotalToursCompleted { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LoyaltyTransactionDto
    {
        public string LoyaltyTransactionId { get; set; }
        public int Points { get; set; }
        public int TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EarnPointsRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Points must be greater than 0")]
        public int Points { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string ReferenceId { get; set; }
    }

    public class RedeemPointsRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Points must be greater than 0")]
        public int Points { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    // Badges
    public class BadgeDto
    {
        public string BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Tier { get; set; }
        public string TierName { get; set; }
        public string Category { get; set; }
        public int ThresholdValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserBadgeDto
    {
        public string UserBadgeId { get; set; }
        public string BadgeName { get; set; }
        public string BadgeDescription { get; set; }
        public string BadgeIconUrl { get; set; }
        public int BadgeTier { get; set; }
        public string BadgeTierName { get; set; }
        public DateTime EarnedAt { get; set; }
    }

    public class CreateBadgeRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string IconUrl { get; set; }

        [Required]
        [Range(0, 2)]
        public int Tier { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(4000)]
        public string Criteria { get; set; }

        [Range(0, int.MaxValue)]
        public int ThresholdValue { get; set; }
    }

    // Lottery
    public class LotteryDrawDto
    {
        public string LotteryDrawId { get; set; }
        public string TourId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEntries { get; set; }
        public int CurrentEntries { get; set; }
        public int WinnerCount { get; set; }
        public int Status { get; set; }
        public DateTime EntryDeadline { get; set; }
        public DateTime DrawDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<LotteryWinnerDto> Winners { get; set; } = new List<LotteryWinnerDto>();
    }

    public class LotteryWinnerDto
    {
        public string UserId { get; set; }
        public DateTime EnteredAt { get; set; }
    }

    public class CreateLotteryDrawRequest
    {
        public string TourId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 10000)]
        public int MaxEntries { get; set; }

        [Range(1, 100)]
        public int WinnerCount { get; set; } = 1;

        [Required]
        public DateTime EntryDeadline { get; set; }

        [Required]
        public DateTime DrawDate { get; set; }
    }

    // Achievements
    public class AchievementDto
    {
        public string AchievementId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string Category { get; set; }
        public int ThresholdValue { get; set; }
        public int PointsReward { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserAchievementDto
    {
        public string UserAchievementId { get; set; }
        public string AchievementName { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementIconUrl { get; set; }
        public string Category { get; set; }
        public int Progress { get; set; }
        public int ThresholdValue { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CreateAchievementRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string IconUrl { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ThresholdValue { get; set; }

        [Range(0, int.MaxValue)]
        public int PointsReward { get; set; }
    }

    public class UpdateProgressRequest
    {
        [Required]
        public string AchievementId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProgressIncrement { get; set; }
    }

    // Dashboard
    public class GamificationDashboardDto
    {
        public LoyaltyAccountDto Loyalty { get; set; }
        public List<UserBadgeDto> Badges { get; set; } = new List<UserBadgeDto>();
        public List<UserAchievementDto> Achievements { get; set; } = new List<UserAchievementDto>();
        public List<LotteryDrawDto> ActiveLotteries { get; set; } = new List<LotteryDrawDto>();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Gamification;

namespace UrGuide.Services.Gamification
{
    public interface IGamificationService
    {
        // Loyalty
        Task<Outcome<LoyaltyAccountDto>> GetLoyaltyAccountAsync(string userId);
        Task<Outcome<LoyaltyAccountDto>> EarnPointsAsync(string userId, EarnPointsRequest request);
        Task<Outcome<LoyaltyAccountDto>> RedeemPointsAsync(string userId, RedeemPointsRequest request);
        Task<Outcome<List<LoyaltyTransactionDto>>> GetLoyaltyHistoryAsync(string userId, int page = 1, int pageSize = 20);

        // Badges
        Task<Outcome<BadgeDto>> CreateBadgeAsync(CreateBadgeRequest request);
        Task<Outcome<List<BadgeDto>>> GetAllBadgesAsync();
        Task<Outcome<List<UserBadgeDto>>> GetUserBadgesAsync(string userId);
        Task<Outcome<UserBadgeDto>> AwardBadgeAsync(string userId, string badgeId);

        // Lottery
        Task<Outcome<LotteryDrawDto>> CreateLotteryDrawAsync(CreateLotteryDrawRequest request);
        Task<Outcome<LotteryDrawDto>> GetLotteryDrawAsync(string drawId);
        Task<Outcome<List<LotteryDrawDto>>> GetActiveLotteriesAsync();
        Task<Outcome<bool>> EnterLotteryAsync(string userId, string drawId);
        Task<Outcome<LotteryDrawDto>> DrawWinnersAsync(string drawId);

        // Achievements
        Task<Outcome<AchievementDto>> CreateAchievementAsync(CreateAchievementRequest request);
        Task<Outcome<List<AchievementDto>>> GetAllAchievementsAsync();
        Task<Outcome<List<UserAchievementDto>>> GetUserAchievementsAsync(string userId);
        Task<Outcome<UserAchievementDto>> UpdateProgressAsync(string userId, UpdateProgressRequest request);

        // Dashboard
        Task<Outcome<GamificationDashboardDto>> GetDashboardAsync(string userId);
    }
}

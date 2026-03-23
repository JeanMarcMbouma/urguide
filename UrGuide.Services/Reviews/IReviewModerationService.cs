using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Model.Reviews;

namespace UrGuide.Services.Reviews
{
    public interface IReviewModerationService
    {
        Task<Outcome<bool>> FlagReviewAsync(string userId, string reviewId, FlagReviewRequest request);
        Task<Outcome<List<ModerationQueueItem>>> GetModerationQueueAsync(int page, int pageSize, string statusFilter);
        Task<Outcome<bool>> ModerateReviewAsync(string adminId, string reviewId, ReviewModerationResult action);
        Task<Outcome<ModerationStatsDto>> GetModerationStatsAsync();
        Task<Outcome<decimal>> CheckForSpamAsync(string reviewText);
        Task<Outcome<bool>> SubmitAppealAsync(string userId, ReviewAppealRequest request);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Data.Entities.Referrals;
using UrGuide.Model.Referrals;

namespace UrGuide.Services.Referrals
{
    public interface IReferralService
    {
        Task<Outcome<ReferralCodeDto>> GenerateReferralCodeAsync(string userId, ReferralCodeType type);
        Task<Outcome<ReferralCodeDto>> GetUserReferralCodeAsync(string userId);
        Task<Outcome<bool>> ApplyReferralCodeAsync(string newUserId, string code);
        Task<Outcome<ReferralDashboardDto>> GetReferralDashboardAsync(string userId);
        Task<Outcome<bool>> CompleteReferralAsync(string referredUserId);
        Task<Outcome<List<ReferralDto>>> GetReferralsByCodeAsync(string code, int page, int pageSize);
    }
}

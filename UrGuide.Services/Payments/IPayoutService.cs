using System.Threading.Tasks;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IPayoutService
    {
        Task<PayoutResponse> CreatePayoutAsync(string guideId, CreatePayoutRequest request);
        Task<PayoutResponse> GetPayoutAsync(string payoutId);
        Task<PayoutListResponse> GetGuidePayoutsAsync(string guideId, int page = 1, int pageSize = 20);
        Task<bool> ProcessPayoutAsync(string payoutId);
        Task<decimal> GetGuideAvailableBalanceAsync(string guideId);
    }
}

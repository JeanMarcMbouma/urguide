using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IPayoutService
    {
        Task<PayoutResponse> CreatePayoutAsync(string guideId, CreatePayoutRequest request, CancellationToken cancellationToken = default);
        Task<PayoutResponse> GetPayoutAsync(string payoutId, CancellationToken cancellationToken = default);
        Task<PayoutListResponse> GetGuidePayoutsAsync(string guideId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<bool> ProcessPayoutAsync(string payoutId, CancellationToken cancellationToken = default);
        Task<decimal> GetGuideAvailableBalanceAsync(string guideId, CancellationToken cancellationToken = default);
    }
}

using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IRefundService
    {
        Task<RefundResponse> CreateRefundAsync(string userId, CreateRefundRequest request, CancellationToken cancellationToken = default);
        Task<RefundResponse> GetRefundAsync(string refundId, CancellationToken cancellationToken = default);
        Task<RefundListResponse> GetPaymentRefundsAsync(string paymentId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<bool> ProcessRefundAsync(string refundId, CancellationToken cancellationToken = default);
        Task UpdateRefundStatusAsync(string stripeRefundId, string status, CancellationToken cancellationToken = default);
    }
}

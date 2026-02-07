using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IRefundService
    {
        Task<RefundResponse> CreateRefundAsync(string userId, CreateRefundRequest request);
        Task<RefundResponse> GetRefundAsync(string refundId);
        Task<RefundListResponse> GetPaymentRefundsAsync(string paymentId, int page = 1, int pageSize = 20);
        Task<bool> ProcessRefundAsync(string refundId);
        Task UpdateRefundStatusAsync(string stripeRefundId, string status);
    }
}

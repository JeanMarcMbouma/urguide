using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data.Entities.Payments;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(string userId, CreatePaymentRequest request, CancellationToken cancellationToken = default);
        Task<PaymentDetailsResponse> GetPaymentAsync(string paymentId, CancellationToken cancellationToken = default);
        Task<TransactionHistoryResponse> GetUserTransactionHistoryAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<bool> ConfirmPaymentAsync(string paymentId, CancellationToken cancellationToken = default);
        Task<bool> CancelPaymentAsync(string paymentId, CancellationToken cancellationToken = default);
        Task UpdatePaymentStatusAsync(string stripePaymentIntentId, PaymentStatus status, CancellationToken cancellationToken = default);
    }
}

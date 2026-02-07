using System.Threading.Tasks;
using UrGuide.Data.Entities.Payments;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(string userId, CreatePaymentRequest request);
        Task<PaymentDetailsResponse> GetPaymentAsync(string paymentId);
        Task<TransactionHistoryResponse> GetUserTransactionHistoryAsync(string userId, int page = 1, int pageSize = 20);
        Task<bool> ConfirmPaymentAsync(string paymentId);
        Task<bool> CancelPaymentAsync(string paymentId);
        Task UpdatePaymentStatusAsync(string stripePaymentIntentId, PaymentStatus status);
    }
}

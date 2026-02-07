using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using UrGuide.Data;
using UrGuide.Data.Entities.Payments;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public class RefundService : IRefundService
    {
        private readonly UrGuideContext _context;
        private readonly IConfiguration _configuration;
        private readonly Stripe.RefundService _stripeRefundService;

        public RefundService(UrGuideContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            
            var stripeApiKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(stripeApiKey))
            {
                StripeConfiguration.ApiKey = stripeApiKey;
            }
            
            _stripeRefundService = new Stripe.RefundService();
        }

        public async Task<RefundResponse> CreateRefundAsync(string userId, CreateRefundRequest request)
        {
            // Validate payment
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == request.PaymentId);

            if (payment == null)
            {
                throw new ArgumentException("Payment not found");
            }

            if (payment.Status != PaymentStatus.Succeeded)
            {
                throw new InvalidOperationException("Can only refund succeeded payments");
            }

            // Calculate refund amount
            var refundAmount = request.Amount ?? payment.Amount;

            // Check if already fully refunded
            var existingRefunds = await _context.Refunds
                .Where(r => r.PaymentId == request.PaymentId && r.Status == RefundStatus.Succeeded)
                .SumAsync(r => r.Amount);

            if (existingRefunds >= payment.Amount)
            {
                throw new InvalidOperationException("Payment is already fully refunded");
            }

            if (existingRefunds + refundAmount > payment.Amount)
            {
                throw new InvalidOperationException($"Refund amount exceeds available amount. Available: {payment.Amount - existingRefunds}");
            }

            // Create refund record
            var refund = new Data.Entities.Payments.Refund
            {
                RefundId = Guid.NewGuid().ToString(),
                PaymentId = request.PaymentId,
                Amount = refundAmount,
                CurrencyCode = payment.CurrencyCode,
                Status = RefundStatus.Pending,
                Reason = request.Reason,
                RequestedBy = userId,
                RequestedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Refunds.Add(refund);

            // Create transaction record
            var transaction = new PaymentTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentId = request.PaymentId,
                Type = TransactionType.Refund,
                Amount = refundAmount,
                CurrencyCode = payment.CurrencyCode,
                Description = $"Refund for payment {request.PaymentId}: {request.Reason}",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            return new RefundResponse
            {
                RefundId = refund.RefundId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                CurrencyCode = refund.CurrencyCode,
                Status = refund.Status.ToString(),
                Reason = refund.Reason,
                RequestedBy = refund.RequestedBy,
                RequestedAt = refund.RequestedAt,
                ProcessedAt = refund.ProcessedAt
            };
        }

        public async Task<RefundResponse> GetRefundAsync(string refundId)
        {
            var refund = await _context.Refunds
                .FirstOrDefaultAsync(r => r.RefundId == refundId);

            if (refund == null)
            {
                throw new ArgumentException("Refund not found");
            }

            return new RefundResponse
            {
                RefundId = refund.RefundId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                CurrencyCode = refund.CurrencyCode,
                Status = refund.Status.ToString(),
                Reason = refund.Reason,
                RequestedBy = refund.RequestedBy,
                RequestedAt = refund.RequestedAt,
                ProcessedAt = refund.ProcessedAt,
                FailureReason = refund.FailureReason
            };
        }

        public async Task<RefundListResponse> GetPaymentRefundsAsync(string paymentId, int page = 1, int pageSize = 20)
        {
            var query = _context.Refunds
                .Where(r => r.PaymentId == paymentId)
                .OrderByDescending(r => r.RequestedAt);

            var totalCount = await query.CountAsync();
            var refunds = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RefundResponse
                {
                    RefundId = r.RefundId,
                    PaymentId = r.PaymentId,
                    Amount = r.Amount,
                    CurrencyCode = r.CurrencyCode,
                    Status = r.Status.ToString(),
                    Reason = r.Reason,
                    RequestedBy = r.RequestedBy,
                    RequestedAt = r.RequestedAt,
                    ProcessedAt = r.ProcessedAt,
                    FailureReason = r.FailureReason
                })
                .ToListAsync();

            return new RefundListResponse
            {
                Refunds = refunds,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> ProcessRefundAsync(string refundId)
        {
            var refund = await _context.Refunds
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RefundId == refundId);

            if (refund == null)
            {
                return false;
            }

            try
            {
                // Create Stripe refund
                if (!string.IsNullOrEmpty(refund.Payment.StripePaymentIntentId))
                {
                    var refundOptions = new Stripe.RefundCreateOptions
                    {
                        PaymentIntent = refund.Payment.StripePaymentIntentId,
                        Amount = (long)(refund.Amount * 100), // Convert to cents
                        Reason = Stripe.RefundReasons.RequestedByCustomer,
                        Metadata = new Dictionary<string, string>
                        {
                            { "refund_id", refund.RefundId },
                            { "payment_id", refund.PaymentId }
                        }
                    };

                    var stripeRefund = await _stripeRefundService.CreateAsync(refundOptions);
                    refund.StripeRefundId = stripeRefund.Id;
                }

                refund.Status = RefundStatus.Succeeded;
                refund.ProcessedAt = DateTime.UtcNow;
                refund.UpdatedAt = DateTime.UtcNow;

                // Update payment status
                var totalRefunded = await _context.Refunds
                    .Where(r => r.PaymentId == refund.PaymentId && r.Status == RefundStatus.Succeeded)
                    .SumAsync(r => r.Amount);

                if (totalRefunded >= refund.Payment.Amount)
                {
                    refund.Payment.Status = PaymentStatus.Refunded;
                }
                else
                {
                    refund.Payment.Status = PaymentStatus.PartiallyRefunded;
                }
                refund.Payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                refund.Status = RefundStatus.Failed;
                refund.FailureReason = ex.Message;
                refund.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return false;
            }
        }

        public async Task UpdateRefundStatusAsync(string stripeRefundId, string status)
        {
            var refund = await _context.Refunds
                .FirstOrDefaultAsync(r => r.StripeRefundId == stripeRefundId);

            if (refund != null)
            {
                refund.Status = status.ToLower() switch
                {
                    "succeeded" => RefundStatus.Succeeded,
                    "failed" => RefundStatus.Failed,
                    "canceled" => RefundStatus.Cancelled,
                    _ => refund.Status
                };
                refund.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}

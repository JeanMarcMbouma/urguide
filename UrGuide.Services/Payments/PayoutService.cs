using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using UrGuide.Data;
using UrGuide.Data.Entities.Payments;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public class PayoutService : IPayoutService
    {
        private readonly UrGuideContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayoutService> _logger;
        private readonly Stripe.PayoutService _stripePayoutService;

        public PayoutService(UrGuideContext context, IConfiguration configuration, ILogger<PayoutService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var stripeApiKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(stripeApiKey))
            {
                StripeConfiguration.ApiKey = stripeApiKey;
            }
            
            _stripePayoutService = new Stripe.PayoutService();
        }

        public async Task<PayoutResponse> CreatePayoutAsync(string guideId, CreatePayoutRequest request, CancellationToken cancellationToken = default)
        {
            // Validate guide
            var guide = await _context.Set<Data.Entities.Users.Author>()
                .FirstOrDefaultAsync(a => a.AuthorId == guideId, cancellationToken);

            if (guide == null)
            {
                throw new ArgumentException("Guide not found");
            }

            // Check available balance
            var availableBalance = await GetGuideAvailableBalanceAsync(guideId, cancellationToken);
            if (availableBalance < request.Amount)
            {
                throw new InvalidOperationException($"Insufficient balance. Available: {availableBalance}, Requested: {request.Amount}");
            }

            // Create payout record
            var payout = new Data.Entities.Payments.Payout
            {
                PayoutId = Guid.NewGuid().ToString(),
                GuideId = guideId,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode,
                Status = PayoutStatus.Pending,
                Description = request.Description ?? $"Payout to guide {guideId}",
                RequestedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Payouts.Add(payout);

            // Create transaction record
            var transaction = new PaymentTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentId = null, // This is a payout, not tied to a specific payment
                Type = TransactionType.Payout,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode,
                Description = payout.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payout {PayoutId} created for guide {GuideId}, amount: {Amount} {Currency}",
                payout.PayoutId, guideId, request.Amount, request.CurrencyCode);

            return new PayoutResponse
            {
                PayoutId = payout.PayoutId,
                GuideId = payout.GuideId,
                Amount = payout.Amount,
                CurrencyCode = payout.CurrencyCode,
                Status = payout.Status.ToString(),
                RequestedAt = payout.RequestedAt,
                ProcessedAt = payout.ProcessedAt
            };
        }

        public async Task<PayoutResponse> GetPayoutAsync(string payoutId, CancellationToken cancellationToken = default)
        {
            var payout = await _context.Payouts
                .FirstOrDefaultAsync(p => p.PayoutId == payoutId, cancellationToken);

            if (payout == null)
            {
                throw new ArgumentException("Payout not found");
            }

            return new PayoutResponse
            {
                PayoutId = payout.PayoutId,
                GuideId = payout.GuideId,
                Amount = payout.Amount,
                CurrencyCode = payout.CurrencyCode,
                Status = payout.Status.ToString(),
                RequestedAt = payout.RequestedAt,
                ProcessedAt = payout.ProcessedAt,
                FailureReason = payout.FailureReason
            };
        }

        public async Task<PayoutListResponse> GetGuidePayoutsAsync(string guideId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = _context.Payouts
                .Where(p => p.GuideId == guideId)
                .OrderByDescending(p => p.RequestedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var payouts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PayoutResponse
                {
                    PayoutId = p.PayoutId,
                    GuideId = p.GuideId,
                    Amount = p.Amount,
                    CurrencyCode = p.CurrencyCode,
                    Status = p.Status.ToString(),
                    RequestedAt = p.RequestedAt,
                    ProcessedAt = p.ProcessedAt,
                    FailureReason = p.FailureReason
                })
                .ToListAsync(cancellationToken);

            return new PayoutListResponse
            {
                Payouts = payouts,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> ProcessPayoutAsync(string payoutId, CancellationToken cancellationToken = default)
        {
            var payout = await _context.Payouts.FindAsync(new object[] { payoutId }, cancellationToken);
            if (payout == null)
            {
                return false;
            }

            try
            {
                // In a real implementation, you would create a Stripe payout here
                // For now, we'll just update the status
                payout.Status = PayoutStatus.Paid;
                payout.ProcessedAt = DateTime.UtcNow;
                payout.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payout {PayoutId} processed successfully", payoutId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payout {PayoutId}", payoutId);

                payout.Status = PayoutStatus.Failed;
                payout.FailureReason = ex.Message;
                payout.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return false;
            }
        }

        public async Task<decimal> GetGuideAvailableBalanceAsync(string guideId, CancellationToken cancellationToken = default)
        {
            // Calculate total earnings from successful payments
            var totalEarnings = await _context.Payments
                .Where(p => p.Booking.Tour.AuthorId == guideId && p.Status == PaymentStatus.Succeeded)
                .SumAsync(p => p.GuidePayout, cancellationToken);

            // Calculate total payouts
            var totalPayouts = await _context.Payouts
                .Where(p => p.GuideId == guideId && 
                           (p.Status == PayoutStatus.Paid || p.Status == PayoutStatus.Processing))
                .SumAsync(p => p.Amount, cancellationToken);

            return totalEarnings - totalPayouts;
        }
    }
}

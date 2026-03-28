using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using UrGuide.Data;
using UrGuide.Data.Entities.Payments;
using UrGuide.Data.Entities.Tour;
using UrGuide.Model.Payments;

namespace UrGuide.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly UrGuideContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly PaymentIntentService _paymentIntentService;
        private readonly CustomerService _customerService;

        public PaymentService(UrGuideContext context, IConfiguration configuration, ILogger<PaymentService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var stripeApiKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(stripeApiKey))
            {
                StripeConfiguration.ApiKey = stripeApiKey;
            }
            
            _paymentIntentService = new PaymentIntentService();
            _customerService = new CustomerService();
        }

        public async Task<PaymentResponse> CreatePaymentAsync(string userId, CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            // Validate booking
            var booking = await _context.Set<Booking>()
                .Include(b => b.Tour)
                    .ThenInclude(t => t.Author)
                    .ThenInclude(a => a.Subscription)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId, cancellationToken);

            if (booking == null)
            {
                throw new ArgumentException("Booking not found");
            }

            // Get or create Stripe customer
            var user = await _context.Users.FindAsync(new object?[] { userId }, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            string? stripeCustomerId = null;
            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = user.UserName,
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId }
                    }
                };
                var customer = await _customerService.CreateAsync(customerOptions, cancellationToken: cancellationToken);
                stripeCustomerId = customer.Id;
                user.StripeCustomerId = stripeCustomerId;
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                stripeCustomerId = user.StripeCustomerId;
            }

            // Calculate platform fee based on guide's membership
            var membership = booking.Tour?.Author?.Subscription?.Membership ?? Membership.Basic;
            var platformFee = PlatformFee.CalculateFee(request.Amount, membership);
            var guidePayout = request.Amount - platformFee;

            // Create Stripe Payment Intent
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Convert to cents
                Currency = request.CurrencyCode.ToLower(),
                Customer = stripeCustomerId,
                Description = request.Description ?? $"Payment for booking {request.BookingId}",
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", request.BookingId },
                    { "user_id", userId },
                    { "platform_fee", platformFee.ToString() },
                    { "guide_payout", guidePayout.ToString() }
                },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(paymentIntentOptions, cancellationToken: cancellationToken);

            // Create payment record
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                UserId = userId,
                BookingId = request.BookingId,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode,
                Status = PaymentStatus.Pending,
                StripePaymentIntentId = paymentIntent.Id,
                StripeCustomerId = stripeCustomerId,
                PaymentMethod = Data.Entities.Payments.PaymentMethod.Card,
                Description = request.Description,
                PlatformFeeAmount = platformFee,
                GuidePayout = guidePayout,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // Create platform fee record
            var feeRecord = new PlatformFee
            {
                FeeId = Guid.NewGuid().ToString(),
                PaymentId = payment.PaymentId,
                Amount = platformFee,
                Percentage = membership == Membership.Basic ? 0.02m : 0.05m,
                MembershipTier = membership,
                CurrencyCode = request.CurrencyCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.PlatformFees.Add(feeRecord);

            // Create transaction record
            var transaction = new PaymentTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentId = payment.PaymentId,
                Type = TransactionType.Payment,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode,
                Description = $"Payment for booking {request.BookingId}",
                StripeTransactionId = paymentIntent.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} created for booking {BookingId} by user {UserId}, amount: {Amount} {Currency}",
                payment.PaymentId, request.BookingId, userId, request.Amount, request.CurrencyCode);

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                ClientSecret = paymentIntent.ClientSecret,
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                CurrencyCode = payment.CurrencyCode,
                PlatformFeeAmount = platformFee,
                GuidePayout = guidePayout,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<PaymentDetailsResponse> GetPaymentAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken);

            if (payment == null)
            {
                throw new ArgumentException("Payment not found");
            }

            return new PaymentDetailsResponse
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                CurrencyCode = payment.CurrencyCode,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                Description = payment.Description,
                PlatformFeeAmount = payment.PlatformFeeAmount,
                GuidePayout = payment.GuidePayout,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }

        public async Task<TransactionHistoryResponse> GetUserTransactionHistoryAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = from payment in _context.Payments
                        where payment.UserId == userId
                        join transaction in _context.PaymentTransactions on payment.PaymentId equals transaction.PaymentId
                        orderby transaction.CreatedAt descending
                        select new TransactionItem
                        {
                            TransactionId = transaction.TransactionId,
                            Type = transaction.Type.ToString(),
                            Amount = transaction.Amount,
                            CurrencyCode = transaction.CurrencyCode,
                            Description = transaction.Description,
                            CreatedAt = transaction.CreatedAt,
                            Status = payment.Status.ToString()
                        };

            var totalCount = await query.CountAsync(cancellationToken);
            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new TransactionHistoryResponse
            {
                Transactions = transactions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync(new object?[] { paymentId }, cancellationToken);
            if (payment == null)
            {
                return false;
            }

            payment.Status = PaymentStatus.Succeeded;
            payment.UpdatedAt = DateTime.UtcNow;

            // Update booking status
            var booking = await _context.Set<Booking>().FindAsync(new object?[] { payment.BookingId }, cancellationToken);
            if (booking != null)
            {
                booking.Status = BookingStatus.Confirmed;
                booking.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} confirmed", paymentId);

            return true;
        }

        public async Task<bool> CancelPaymentAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync(new object?[] { paymentId }, cancellationToken);
            if (payment == null)
            {
                return false;
            }

            // Cancel Stripe payment intent
            if (!string.IsNullOrEmpty(payment.StripePaymentIntentId))
            {
                try
                {
                    await _paymentIntentService.CancelAsync(payment.StripePaymentIntentId, cancellationToken: cancellationToken);
                }
                catch (StripeException ex)
                {
                    _logger.LogError(ex, "Failed to cancel Stripe payment intent {PaymentIntentId} for payment {PaymentId}",
                        payment.StripePaymentIntentId, paymentId);
                }
            }

            payment.Status = PaymentStatus.Cancelled;
            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} cancelled", paymentId);

            return true;
        }

        public async Task UpdatePaymentStatusAsync(string stripePaymentIntentId, PaymentStatus status, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId, cancellationToken);

            if (payment != null)
            {
                payment.Status = status;
                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payment status updated for Stripe intent {PaymentIntentId} to {Status}", stripePaymentIntentId, status);
            }
        }
    }
}

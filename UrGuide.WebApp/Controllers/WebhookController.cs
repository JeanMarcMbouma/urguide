using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using UrGuide.Data.Entities.Payments;
using UrGuide.Services.Payments;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRefundService _refundService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IPaymentService paymentService,
            IRefundService refundService,
            IConfiguration configuration,
            ILogger<WebhookController> logger)
        {
            _paymentService = paymentService;
            _refundService = refundService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handle Stripe webhook events
        /// </summary>
        [HttpPost("stripe")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _configuration["Stripe:WebhookSecret"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                _logger.LogInformation("Stripe webhook received: {EventType}", stripeEvent.Type);

                // Handle different event types
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await HandlePaymentIntentSucceeded(stripeEvent);
                        break;

                    case "payment_intent.payment_failed":
                        await HandlePaymentIntentFailed(stripeEvent);
                        break;

                    case "payment_intent.canceled":
                        await HandlePaymentIntentCanceled(stripeEvent);
                        break;

                    case "charge.refunded":
                        await HandleChargeRefunded(stripeEvent);
                        break;

                    case "payout.paid":
                        await HandlePayoutPaid(stripeEvent);
                        break;

                    case "payout.failed":
                        await HandlePayoutFailed(stripeEvent);
                        break;

                    default:
                        _logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500);
            }
        }

        private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent != null)
            {
                _logger.LogInformation("Payment succeeded: {PaymentIntentId}", paymentIntent.Id);
                await _paymentService.UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Succeeded);
                await _paymentService.ConfirmPaymentAsync(paymentIntent.Id);
            }
        }

        private async Task HandlePaymentIntentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent != null)
            {
                _logger.LogWarning("Payment failed: {PaymentIntentId}", paymentIntent.Id);
                await _paymentService.UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Failed);
            }
        }

        private async Task HandlePaymentIntentCanceled(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent != null)
            {
                _logger.LogInformation("Payment canceled: {PaymentIntentId}", paymentIntent.Id);
                await _paymentService.UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Cancelled);
            }
        }

        private async Task HandleChargeRefunded(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge != null && charge.Refunded)
            {
                _logger.LogInformation("Charge refunded: {ChargeId}", charge.Id);
                // The refund status is already updated in the RefundService.ProcessRefundAsync
            }
        }

        private Task HandlePayoutPaid(Event stripeEvent)
        {
            var payout = stripeEvent.Data.Object as Stripe.Payout;
            if (payout != null)
            {
                _logger.LogInformation("Payout paid: {PayoutId}", payout.Id);
                // Update payout status in database if needed
            }
            return Task.CompletedTask;
        }

        private Task HandlePayoutFailed(Event stripeEvent)
        {
            var payout = stripeEvent.Data.Object as Stripe.Payout;
            if (payout != null)
            {
                _logger.LogWarning("Payout failed: {PayoutId}", payout.Id);
                // Update payout status in database if needed
            }
            return Task.CompletedTask;
        }
    }
}

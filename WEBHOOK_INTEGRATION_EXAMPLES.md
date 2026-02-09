# Webhook Integration Examples

This document provides practical examples of integrating webhook publishing into existing UrGuide services.

## Example 1: Payment Service Integration

Here's how to add webhook publishing to the payment service:

```csharp
// In PaymentService.cs
using UrGuide.Services.Webhooks;
using UrGuide.Model.Webhooks;

public class PaymentService : IPaymentService
{
    private readonly UrGuideContext _context;
    private readonly IWebhookService _webhookService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        UrGuideContext context,
        IWebhookService webhookService,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _webhookService = webhookService;
        _logger = logger;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(string userId, CreatePaymentRequest request)
    {
        // Create payment logic...
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = request.Amount,
            // ... other properties
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Publish webhook event
        try
        {
            await _webhookService.PublishEventAsync(
                WebhookEvent.PaymentCreated,
                new
                {
                    PaymentId = payment.PaymentId,
                    UserId = payment.UserId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    Currency = payment.CurrencyCode,
                    Status = payment.Status.ToString(),
                    CreatedAt = payment.CreatedAt
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish PaymentCreated webhook event");
            // Don't fail the payment creation if webhook publishing fails
        }

        return MapToResponse(payment);
    }

    public async Task UpdatePaymentStatusAsync(string stripePaymentIntentId, PaymentStatus status)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId);

        if (payment == null) return;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Publish status change webhook
        try
        {
            WebhookEvent webhookEvent = status switch
            {
                PaymentStatus.Succeeded => WebhookEvent.PaymentSucceeded,
                PaymentStatus.Failed => WebhookEvent.PaymentFailed,
                PaymentStatus.Cancelled => WebhookEvent.PaymentCancelled,
                PaymentStatus.Refunded => WebhookEvent.PaymentRefunded,
                _ => (WebhookEvent)0 // Don't publish for other statuses
            };

            if ((int)webhookEvent > 0)
            {
                await _webhookService.PublishEventAsync(webhookEvent, new
                {
                    PaymentId = payment.PaymentId,
                    UserId = payment.UserId,
                    Status = status.ToString(),
                    Amount = payment.Amount,
                    Currency = payment.CurrencyCode,
                    UpdatedAt = payment.UpdatedAt
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish payment status webhook event for {status}");
        }
    }
}
```

## Example 2: Booking Service Integration

```csharp
// In BookingService.cs (or similar)
public class BookingService
{
    private readonly UrGuideContext _context;
    private readonly IWebhookService _webhookService;
    private readonly ILogger<BookingService> _logger;

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest request)
    {
        var booking = new Booking
        {
            BookingId = Guid.NewGuid().ToString(),
            // ... other properties
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Publish webhook
        try
        {
            await _webhookService.PublishEventAsync(
                WebhookEvent.BookingCreated,
                new
                {
                    BookingId = booking.BookingId,
                    TourId = booking.TourId,
                    UserId = booking.UserId,
                    GuideId = booking.GuideId,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    Participants = booking.Participants,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status.ToString(),
                    CreatedAt = booking.CreatedAt
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish BookingCreated webhook event");
        }

        return booking;
    }

    public async Task ConfirmBookingAsync(string bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return;

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Publish webhook
        try
        {
            await _webhookService.PublishEventAsync(
                WebhookEvent.BookingConfirmed,
                new
                {
                    BookingId = booking.BookingId,
                    TourId = booking.TourId,
                    UserId = booking.UserId,
                    GuideId = booking.GuideId,
                    ConfirmedAt = booking.UpdatedAt
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish BookingConfirmed webhook event");
        }
    }
}
```

## Example 3: Tour Service Integration

```csharp
// In PostService.cs or TourService.cs
public async Task<Post> CreateTourPostAsync(PostCreationModel model)
{
    var post = new Post
    {
        // ... create tour post
    };

    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    // Publish webhook
    try
    {
        await _webhookService.PublishEventAsync(
            WebhookEvent.TourCreated,
            new
            {
                TourId = post.Id,
                Title = post.Title,
                Description = post.Description,
                Location = post.Location,
                Price = post.Price,
                GuideId = post.AuthorId,
                CreatedAt = post.Created
            }
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to publish TourCreated webhook event");
    }

    return post;
}
```

## Example 4: User Registration Integration

```csharp
// In AccountController.cs or UserService.cs
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] CreateUserModel model)
{
    // Create user logic...
    var user = await _userService.CreateAsync(model);

    // Publish webhook
    try
    {
        await _webhookService.PublishEventAsync(
            WebhookEvent.UserRegistered,
            new
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role,
                CreatedAt = user.Created
            }
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to publish UserRegistered webhook event");
    }

    return Ok(user);
}
```

## Example 5: Review Service Integration

```csharp
// In FeedbackService.cs or ReviewService.cs
public async Task<Review> CreateReviewAsync(CreateReviewRequest request)
{
    var review = new Review
    {
        ReviewId = Guid.NewGuid().ToString(),
        // ... other properties
    };

    _context.Reviews.Add(review);
    await _context.SaveChangesAsync();

    // Publish webhook
    try
    {
        await _webhookService.PublishEventAsync(
            WebhookEvent.ReviewCreated,
            new
            {
                ReviewId = review.ReviewId,
                TourId = review.TourId,
                UserId = review.UserId,
                GuideId = review.GuideId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to publish ReviewCreated webhook event");
    }

    return review;
}
```

## Best Practices

### 1. Error Handling
Always wrap webhook publishing in try-catch blocks to prevent webhook failures from breaking your core business logic:

```csharp
try
{
    await _webhookService.PublishEventAsync(eventType, data);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to publish webhook event");
    // Continue execution - webhook failure shouldn't break core functionality
}
```

### 2. Asynchronous Publishing
The webhook service uses fire-and-forget publishing, so it won't block your API responses:

```csharp
// The webhook delivery happens in the background
await _webhookService.PublishEventAsync(eventType, data);
// Your API response is sent immediately
```

### 3. Data Minimization
Only include necessary data in webhook payloads:

```csharp
// Good - only essential data
await _webhookService.PublishEventAsync(
    WebhookEvent.PaymentCreated,
    new { PaymentId = payment.Id, Amount = payment.Amount, Status = payment.Status }
);

// Avoid - sending entire entities with sensitive data
// await _webhookService.PublishEventAsync(
//     WebhookEvent.PaymentCreated,
//     payment // Don't do this!
// );
```

### 4. Logging
Log webhook publishing attempts for debugging:

```csharp
_logger.LogInformation($"Publishing {eventType} webhook event for {resourceId}");
await _webhookService.PublishEventAsync(eventType, data);
```

### 5. Testing
Create unit tests for webhook publishing:

```csharp
[Test]
public async Task CreatePayment_ShouldPublishWebhookEvent()
{
    // Arrange
    var webhookService = new Mock<IWebhookService>();
    var paymentService = new PaymentService(context, webhookService.Object, logger);

    // Act
    await paymentService.CreatePaymentAsync(userId, request);

    // Assert
    webhookService.Verify(
        x => x.PublishEventAsync(
            WebhookEvent.PaymentCreated,
            It.IsAny<object>()
        ),
        Times.Once
    );
}
```

## Testing Webhooks

### Using webhook.site
For quick testing, you can use [webhook.site](https://webhook.site):

1. Go to https://webhook.site
2. Copy the unique URL
3. Register it as a webhook in UrGuide
4. Trigger an event
5. View the payload on webhook.site

### Using ngrok for Local Testing
For local development:

```bash
# Start your local webhook receiver on port 3000
node webhook-receiver.js

# In another terminal, expose it with ngrok
ngrok http 3000

# Use the ngrok HTTPS URL to register your webhook
# Example: https://abc123.ngrok.io/webhooks
```

### Sample Webhook Receiver (Node.js)

```javascript
const express = require('express');
const crypto = require('crypto');
const app = express();

app.use(express.json());

app.post('/webhooks', (req, res) => {
  const signature = req.headers['x-webhook-signature'];
  const payload = JSON.stringify(req.body);
  const secret = process.env.WEBHOOK_SECRET;

  // Verify signature
  const expectedSignature = 'sha256=' + 
    crypto
      .createHmac('sha256', secret)
      .update(payload)
      .digest('hex');

  if (signature !== expectedSignature) {
    return res.status(401).send('Invalid signature');
  }

  // Process webhook
  console.log('Webhook received:', req.body);
  
  // Respond quickly
  res.status(200).send('OK');
});

app.listen(3000, () => {
  console.log('Webhook receiver listening on port 3000');
});
```

## Troubleshooting

### Webhooks Not Firing
1. Check that the webhook is active: `GET /api/webhook-management/{id}`
2. Verify the event type is subscribed
3. Check application logs for errors
4. Review webhook delivery history: `GET /api/webhook-management/{id}/deliveries`

### Webhook Deliveries Failing
1. Ensure your endpoint returns 2xx status codes
2. Verify your endpoint responds within 30 seconds
3. Check webhook delivery logs for error messages
4. Test your endpoint independently

### Signature Verification Failing
1. Use the exact secret from the webhook registration response
2. Ensure you're comparing the raw request body (before parsing)
3. Use constant-time comparison to prevent timing attacks
4. Log both signatures for debugging (but never in production!)

# Webhook System Integration Guide

This document explains how to integrate webhook event publishing into existing services.

## Overview

The webhook system allows external applications to receive real-time notifications when important events occur in the UrGuide platform.

## Available Events

The following events are available for subscription:

### Payment Events
- `PaymentCreated` (1000) - A new payment has been created
- `PaymentSucceeded` (1001) - A payment has been completed successfully
- `PaymentFailed` (1002) - A payment has failed
- `PaymentCancelled` (1003) - A payment has been cancelled
- `PaymentRefunded` (1004) - A payment has been refunded

### Booking Events
- `BookingCreated` (2000) - A new tour booking has been created
- `BookingConfirmed` (2001) - A booking has been confirmed
- `BookingCancelled` (2002) - A booking has been cancelled
- `BookingCompleted` (2003) - A booking has been completed

### Tour Events
- `TourCreated` (3000) - A new tour has been created
- `TourUpdated` (3001) - A tour has been updated
- `TourDeleted` (3002) - A tour has been deleted

### User Events
- `UserRegistered` (4000) - A new user has registered
- `UserUpdated` (4001) - A user profile has been updated

### Review Events
- `ReviewCreated` (5000) - A new review has been posted
- `ReviewUpdated` (5001) - A review has been updated

## Publishing Webhook Events

To publish a webhook event from any service, inject `IWebhookService` and call `PublishEventAsync`:

```csharp
using UrGuide.Services.Webhooks;
using UrGuide.Model.Webhooks;

public class PaymentService : IPaymentService
{
    private readonly IWebhookService _webhookService;
    
    public PaymentService(IWebhookService webhookService, ...)
    {
        _webhookService = webhookService;
    }
    
    public async Task<Payment> CreatePaymentAsync(...)
    {
        // ... create payment logic
        
        // Publish webhook event
        await _webhookService.PublishEventAsync(
            WebhookEvent.PaymentCreated,
            new {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                Amount = payment.Amount,
                Currency = payment.CurrencyCode,
                Status = payment.Status.ToString(),
                CreatedAt = payment.CreatedAt
            }
        );
        
        return payment;
    }
}
```

## Payload Structure

All webhook payloads follow this standard structure:

```json
{
  "eventId": "unique-event-id",
  "event": "PaymentCreated",
  "timestamp": "2026-02-09T22:00:00Z",
  "data": {
    // Event-specific data
  }
}
```

## Payload Signing

All webhook payloads are signed using HMAC-SHA256. The signature is sent in the `X-Webhook-Signature` header as:

```
X-Webhook-Signature: sha256=<hex_encoded_signature>
```

Recipients should verify the signature using their webhook secret before processing the payload.

### Verifying Signatures (Example in Node.js)

```javascript
const crypto = require('crypto');

function verifyWebhookSignature(payload, signature, secret) {
  const expectedSignature = 'sha256=' + 
    crypto
      .createHmac('sha256', secret)
      .update(payload)
      .digest('hex');
  
  return crypto.timingSafeEqual(
    Buffer.from(signature),
    Buffer.from(expectedSignature)
  );
}
```

## Retry Logic

The webhook system implements exponential backoff with the following retry schedule:

- **Attempt 1**: Immediate delivery
- **Attempt 2**: After 5 seconds
- **Attempt 3**: After 15 seconds  
- **Attempt 4**: After 45 seconds
- **Attempt 5**: After 135 seconds (final attempt)

A webhook delivery is considered successful when the endpoint returns a 2xx HTTP status code.

## API Endpoints

### Register a Webhook

```http
POST /api/webhook-management
Authorization: Bearer <token>
Content-Type: application/json

{
  "url": "https://example.com/webhooks",
  "description": "Production webhook for payments",
  "events": [1000, 1001, 1002]
}
```

**Response:**
```json
{
  "id": "webhook-id",
  "url": "https://example.com/webhooks",
  "secret": "base64-encoded-secret",
  "isActive": true,
  "description": "Production webhook for payments",
  "events": [1000, 1001, 1002],
  "createdAt": "2026-02-09T22:00:00Z",
  "updatedAt": "2026-02-09T22:00:00Z",
  "successCount": 0,
  "failureCount": 0
}
```

### List Webhooks

```http
GET /api/webhook-management
Authorization: Bearer <token>
```

### Get Webhook Details

```http
GET /api/webhook-management/{id}
Authorization: Bearer <token>
```

### Update Webhook

```http
PUT /api/webhook-management/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "url": "https://example.com/new-webhook",
  "events": [1000, 1001, 1002, 1003],
  "isActive": true
}
```

### Delete Webhook

```http
DELETE /api/webhook-management/{id}
Authorization: Bearer <token>
```

### Get Delivery History

```http
GET /api/webhook-management/{id}/deliveries?page=1&pageSize=20
Authorization: Bearer <token>
```

### Test Webhook

```http
POST /api/webhook-management/test
Authorization: Bearer <token>
Content-Type: application/json

{
  "webhookId": "webhook-id",
  "event": 1000,
  "samplePayload": {
    "test": true,
    "message": "This is a test"
  }
}
```

## Best Practices

1. **Idempotency**: Design your webhook handlers to be idempotent, as the same event may be delivered multiple times
2. **Security**: Always verify webhook signatures before processing payloads
3. **Response Time**: Respond quickly (under 5 seconds) to webhook requests to avoid timeouts
4. **Error Handling**: Return appropriate HTTP status codes (2xx for success, 5xx for retry)
5. **Logging**: Log all webhook events for debugging and auditing purposes
6. **Monitoring**: Monitor webhook delivery success rates and response times

## Security Considerations

- Store webhook secrets securely (use environment variables or secrets management)
- Use HTTPS endpoints only
- Verify webhook signatures on every request
- Implement rate limiting on webhook endpoints
- Log and monitor suspicious webhook activity

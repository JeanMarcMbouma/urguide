# UrGuide API Tutorials

Step-by-step tutorials for common integration scenarios with the UrGuide Tourism Platform API.

## Table of Contents

- [Tutorial 1: Tourist — Discover and Book a Tour](#tutorial-1-tourist--discover-and-book-a-tour)
- [Tutorial 2: Guide — Create a Tour Listing](#tutorial-2-guide--create-a-tour-listing)
- [Tutorial 3: Guide — Respond to Tour Requests](#tutorial-3-guide--respond-to-tour-requests)
- [Tutorial 4: Build a Search Integration](#tutorial-4-build-a-search-integration)
- [Tutorial 5: Set Up Webhook Notifications](#tutorial-5-set-up-webhook-notifications)
- [Tutorial 6: Implement Real-time Updates](#tutorial-6-implement-real-time-updates)
- [Tutorial 7: Admin — Manage Users and Monitor the Platform](#tutorial-7-admin--manage-users-and-monitor-the-platform)

---

## Tutorial 1: Tourist — Discover and Book a Tour

This tutorial walks through the complete tourist journey: creating an account, searching for tours, making a reservation, and processing payment.

### Step 1: Register a Tourist Account

```bash
curl -X POST https://your-instance.com/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "tourist@example.com",
    "password": "SecurePass123!",
    "firstName": "Jane",
    "lastName": "Smith"
  }'
```

### Step 2: Authenticate

```bash
TOKEN=$(curl -s -X POST https://your-instance.com/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "tourist@example.com",
    "password": "SecurePass123!"
  }' | jq -r '.accessToken')
```

### Step 3: Browse Available Tours

```bash
# Search for tours in Paris
curl -X POST https://your-instance.com/api/posts/search \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 10,
    "query": "paris"
  }'
```

### Step 4: Get Personalized Recommendations

```bash
# Get recommendations based on your preferences and location
curl "https://your-instance.com/api/recommendation?count=5&lat=48.8566&lng=2.3522" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 5: View Tour Details

```bash
curl https://your-instance.com/api/posts/{tourId}/retrieve
```

### Step 6: Make a Reservation

```bash
curl -X POST https://your-instance.com/api/posts/{tourId}/makereservation \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "numberOfSeats": 2,
    "date": "2026-07-01"
  }'
```

### Step 7: Process Payment

```bash
curl -X POST https://your-instance.com/api/payment \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tourId": "{tourId}",
    "amount": 300.00,
    "currency": "USD"
  }'
```

### Step 8: Confirm Payment

```bash
curl -X POST https://your-instance.com/api/payment/{paymentId}/confirm \
  -H "Authorization: Bearer $TOKEN"
```

### What You Learned

- How to register and authenticate
- How to search and browse tours
- How to use the recommendation engine
- How to make reservations and process payments

---

## Tutorial 2: Guide — Create a Tour Listing

This tutorial shows how a tour guide can register, create tour listings, and manage their tours.

### Step 1: Register as a Guide

```bash
curl -X POST https://your-instance.com/newguide \
  -H "Content-Type: application/json" \
  -d '{
    "email": "guide@example.com",
    "password": "GuidePass123!",
    "firstName": "Pierre",
    "lastName": "Dupont",
    "bio": "Professional tour guide in Paris with 10 years of experience",
    "languages": ["English", "French", "Spanish"],
    "regions": ["europe-france"]
  }'
```

### Step 2: Authenticate

```bash
TOKEN=$(curl -s -X POST https://your-instance.com/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "guide@example.com",
    "password": "GuidePass123!"
  }' | jq -r '.accessToken')
```

### Step 3: Create a Tour Listing

```bash
curl -X POST https://your-instance.com/api/posts/create \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Historical Paris Walking Tour",
    "description": "Explore the rich history of Paris from the medieval streets to modern landmarks",
    "price": 75.00,
    "currency": "EUR",
    "maxGroupSize": 12,
    "duration": "3 hours",
    "meetingPoint": "Notre-Dame Cathedral",
    "includes": ["Water bottle", "Map", "Audio guide"],
    "languages": ["English", "French"]
  }'
```

### Step 4: Set Your Availability

```bash
# Set available dates for the tour
curl -X POST https://your-instance.com/api/availability \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2026-07-01",
    "startTime": "09:00",
    "endTime": "12:00",
    "isAvailable": true
  }'
```

### Step 5: Manage Your Listings

```bash
# View all your tour listings
curl -X POST https://your-instance.com/api/posts/owned \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"page": 1, "pageSize": 20}'

# Update a listing
curl -X PUT https://your-instance.com/api/posts/{postId}/update \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "price": 80.00,
    "description": "Updated description with new stops on the tour"
  }'
```

### What You Learned

- How to register as a guide
- How to create and manage tour listings
- How to set your availability calendar

---

## Tutorial 3: Guide — Respond to Tour Requests

Learn how to find tour requests from tourists and submit competitive bids.

### Step 1: Browse Tour Requests in Your Region

```bash
# View tour requests in your region
curl "https://your-instance.com/api/tour-requests/region/europe-france?page=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 2: View Request Details

```bash
curl https://your-instance.com/api/tour-requests/{requestId} \
  -H "Authorization: Bearer $TOKEN"
```

### Step 3: Submit a Bid

```bash
curl -X POST https://your-instance.com/api/bid/{postId}/newbid \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 250.00,
    "message": "I specialize in cultural tours of Paris and have been a guide for 10 years. I can customize the itinerary based on your interests."
  }'
```

### Step 4: Check Bid Status

```bash
# View bid history for a tour
curl -X POST https://your-instance.com/api/bid/{postId}/history
```

### What You Learned

- How to discover tour requests from tourists
- How to submit competitive bids
- How to track bid status

---

## Tutorial 4: Build a Search Integration

Build a search-powered tour discovery feature using the Elasticsearch-backed search API.

### Step 1: Check Search Service Health

```bash
curl https://your-instance.com/api/search/health
# Response: { "status": "healthy", "message": "Elasticsearch cluster is available" }
```

### Step 2: Full-Text Search

```bash
curl -X POST https://your-instance.com/api/search/posts \
  -H "Content-Type: application/json" \
  -d '{
    "query": "sunset boat tour",
    "page": 1,
    "pageSize": 10
  }'
```

**Response includes:**
- `results` — Matching tours with relevance scores
- `facets` — Filter options (regions, price ranges, ratings)
- `totalHits` — Total number of matching results
- `highlights` — Text snippets showing matched terms

### Step 3: Implement Autocomplete

```javascript
// Debounced autocomplete for search input
let debounceTimer;

searchInput.addEventListener('input', (e) => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(async () => {
    if (e.target.value.length < 2) return;
    
    const response = await fetch('https://your-instance.com/api/search/autocomplete', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ query: e.target.value })
    });
    
    const { suggestions } = await response.json();
    renderSuggestions(suggestions);
  }, 300);
});
```

### Step 4: Faceted Search with Filters

```javascript
async function searchWithFilters(query, filters) {
  const response = await fetch('https://your-instance.com/api/search/posts', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      query,
      page: 1,
      pageSize: 20,
      filters: {
        priceRange: filters.priceRange,
        rating: { min: filters.minRating },
        categories: filters.selectedCategories,
        languages: filters.selectedLanguages
      }
    })
  });

  const { results, facets, totalHits } = await response.json();
  
  // Use facets to render filter options
  renderFilterSidebar(facets);
  renderResults(results);
  renderPagination(totalHits, 20);
}
```

### What You Learned

- How to use full-text search with Elasticsearch
- How to implement autocomplete suggestions
- How to build faceted search with dynamic filters

---

## Tutorial 5: Set Up Webhook Notifications

Subscribe to UrGuide events and process them in your application.

### Step 1: Register a Webhook Endpoint

```bash
curl -X POST https://your-instance.com/api/webhook-management \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://your-app.com/webhooks/urguide",
    "events": [
      "tour.booked",
      "payment.completed",
      "bid.accepted",
      "bid.rejected",
      "review.created"
    ],
    "secret": "whsec_your_webhook_secret_here"
  }'
```

### Step 2: Implement the Webhook Handler

```javascript
const express = require('express');
const crypto = require('crypto');

const app = express();
app.use(express.json());

app.post('/webhooks/urguide', (req, res) => {
  // 1. Verify the signature
  const signature = req.headers['x-webhook-signature'];
  const expectedSignature = 'sha256=' + crypto
    .createHmac('sha256', process.env.WEBHOOK_SECRET)
    .update(JSON.stringify(req.body))
    .digest('hex');
  
  if (!crypto.timingSafeEqual(Buffer.from(signature), Buffer.from(expectedSignature))) {
    return res.status(401).send('Invalid signature');
  }

  // 2. Process the event
  const { event, data, timestamp } = req.body;
  
  switch (event) {
    case 'tour.booked':
      handleTourBooked(data);
      break;
    case 'payment.completed':
      handlePaymentCompleted(data);
      break;
    case 'bid.accepted':
      handleBidAccepted(data);
      break;
    default:
      console.log(`Unhandled event: ${event}`);
  }

  // 3. Respond quickly (within 30 seconds)
  res.status(200).send('OK');
});
```

### Step 3: Handle Delivery Failures

UrGuide uses HMAC-SHA256 for webhook payload signing. If your endpoint fails to respond with 2xx status, the webhook will be retried. Implement idempotency to handle duplicate deliveries:

```javascript
const processedEvents = new Set();

function handleEvent(eventId, handler) {
  if (processedEvents.has(eventId)) {
    console.log(`Skipping duplicate event: ${eventId}`);
    return;
  }
  processedEvents.add(eventId);
  handler();
}
```

### What You Learned

- How to register webhook subscriptions
- How to verify webhook signatures
- How to handle webhook events safely

---

## Tutorial 6: Implement Real-time Updates

Add live notifications and chat to your application using SignalR.

### Step 1: Install the SignalR Client

```bash
npm install @microsoft/signalr
```

### Step 2: Connect to the Notification Hub

```javascript
import * as signalR from '@microsoft/signalr';

class NotificationService {
  constructor(accessToken) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-instance.com/notify', {
        accessTokenFactory: () => accessToken
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build();
    
    this.setupHandlers();
  }

  setupHandlers() {
    this.connection.on('ReceiveNotification', (notification) => {
      this.onNotification?.(notification);
    });

    this.connection.on('BidUpdate', (data) => {
      this.onBidUpdate?.(data);
    });

    this.connection.on('BookingConfirmed', (data) => {
      this.onBookingConfirmed?.(data);
    });

    this.connection.onreconnecting((error) => {
      console.warn('Reconnecting...', error);
    });

    this.connection.onreconnected((connectionId) => {
      console.log('Reconnected:', connectionId);
    });
  }

  async start() {
    try {
      await this.connection.start();
      console.log('Connected to notification hub');
    } catch (err) {
      console.error('Failed to connect:', err);
      setTimeout(() => this.start(), 5000);
    }
  }

  async stop() {
    await this.connection.stop();
  }
}

// Usage
const notifications = new NotificationService(accessToken);
notifications.onNotification = (n) => showToast(n.title, n.message);
notifications.onBidUpdate = (data) => updateBidUI(data);
await notifications.start();
```

### Step 3: Implement Chat

```javascript
class ChatService {
  constructor(accessToken) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-instance.com/chat', {
        accessTokenFactory: () => accessToken
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (senderId, message, timestamp) => {
      this.onMessage?.({ senderId, message, timestamp });
    });
  }

  async start() {
    await this.connection.start();
  }

  async sendMessage(recipientId, message) {
    await this.connection.invoke('SendMessage', recipientId, message);
  }
}

// Usage
const chat = new ChatService(accessToken);
chat.onMessage = (msg) => appendMessageToUI(msg);
await chat.start();

// Send a message
await chat.sendMessage('guide-user-id', 'Hi! I have questions about your Paris tour.');
```

### What You Learned

- How to connect to SignalR hubs with authentication
- How to handle real-time notifications
- How to implement chat functionality

---

## Tutorial 7: Admin — Manage Users and Monitor the Platform

Learn how to use the admin API for user management, system monitoring, and audit trail review.

### Step 1: Authenticate as Admin

```bash
TOKEN=$(curl -s -X POST https://your-instance.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "admin@urguide.com",
    "password": "AdminPassword123!"
  }' | jq -r '.accessToken')
```

### Step 2: List Users with Pagination

```bash
curl "https://your-instance.com/api/admin/users?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 3: View User Details

```bash
curl "https://your-instance.com/api/admin/users/{userId}" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 4: Freeze a User Account

```bash
curl -X POST "https://your-instance.com/api/admin/users/{userId}/freeze" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "reason": "Violation of terms of service",
    "duration": "7d"
  }'
```

### Step 5: Review Audit Logs

```bash
# Get recent audit events
curl "https://your-instance.com/api/admin/audit-log?page=1&pageSize=50" \
  -H "Authorization: Bearer $TOKEN"

# Filter by category and severity
curl "https://your-instance.com/api/admin/audit-log?category=Security&severity=Warning" \
  -H "Authorization: Bearer $TOKEN"
```

### Step 6: Monitor System Health

```bash
# Check application health
curl https://your-instance.com/health

# Check individual service health
curl https://your-instance.com/api/search/health
```

### What You Learned

- How to authenticate as an admin
- How to manage users (list, view, freeze)
- How to review audit logs with filters
- How to monitor system health

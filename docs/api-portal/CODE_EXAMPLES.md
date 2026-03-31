# UrGuide API Code Examples

Ready-to-use code examples for integrating with the UrGuide API in multiple programming languages.

## Table of Contents

- [Authentication](#authentication)
- [Tour Discovery](#tour-discovery)
- [Tour Requests & Bidding](#tour-requests--bidding)
- [Payments](#payments)
- [Search](#search)
- [Recommendations](#recommendations)
- [Real-time Notifications (SignalR)](#real-time-notifications-signalr)
- [Webhooks](#webhooks)
- [Error Handling](#error-handling)

---

## Authentication

### Obtain a JWT Token

#### cURL

```bash
curl -X POST https://your-instance.com/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "user@example.com",
    "password": "SecurePassword123!"
  }'
```

#### C# (.NET)

```csharp
using System.Net.Http;
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://your-instance.com") };

var response = await client.PostAsJsonAsync("/api/auth/token", new
{
    userName = "user@example.com",
    password = "SecurePassword123!"
});

var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
client.DefaultRequestHeaders.Authorization = 
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

public record TokenResponse(
    string AccessToken, 
    string TokenType, 
    int ExpiresIn,
    UserInfo User);

public record UserInfo(string Id, string Email, string[] Roles);
```

#### JavaScript (Fetch API)

```javascript
const response = await fetch('https://your-instance.com/api/auth/token', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userName: 'user@example.com',
    password: 'SecurePassword123!'
  })
});

const { accessToken, expiresIn, user } = await response.json();

// Use the token for subsequent requests
const authHeaders = {
  'Authorization': `Bearer ${accessToken}`,
  'Content-Type': 'application/json'
};
```

#### Python (requests)

```python
import requests

base_url = "https://your-instance.com"

# Obtain token
auth_response = requests.post(f"{base_url}/api/auth/token", json={
    "userName": "user@example.com",
    "password": "SecurePassword123!"
})
token_data = auth_response.json()
access_token = token_data["accessToken"]

# Create session with auth header
session = requests.Session()
session.headers.update({
    "Authorization": f"Bearer {access_token}",
    "Content-Type": "application/json"
})
```

### Refresh an Expired Token

#### cURL

```bash
curl -X POST https://your-instance.com/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "your-refresh-token"}'
```

#### JavaScript

```javascript
async function refreshToken(currentRefreshToken) {
  const response = await fetch('https://your-instance.com/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: currentRefreshToken })
  });
  
  if (!response.ok) throw new Error('Token refresh failed');
  return await response.json();
}
```

#### Python

```python
def refresh_token(refresh_token_value):
    response = requests.post(f"{base_url}/api/auth/refresh", json={
        "refreshToken": refresh_token_value
    })
    response.raise_for_status()
    return response.json()
```

---

## Tour Discovery

### Browse Recent Tours

#### cURL

```bash
# Get 10 most recent tours (no auth required)
curl https://your-instance.com/api/posts/last10

# Get 10 top-rated tours
curl https://your-instance.com/api/posts/top10
```

#### C#

```csharp
// No authentication required for browsing
var recentTours = await client.GetFromJsonAsync<List<PostModel>>("/api/posts/last10");
var topTours = await client.GetFromJsonAsync<List<PostModel>>("/api/posts/top10");

foreach (var tour in recentTours)
{
    Console.WriteLine($"{tour.Title} - {tour.Price:C} ({tour.Rating} stars)");
}
```

#### JavaScript

```javascript
// Browse recent tours (public endpoint)
const response = await fetch('https://your-instance.com/api/posts/last10');
const tours = await response.json();

tours.forEach(tour => {
  console.log(`${tour.title} - $${tour.price} (${tour.rating} stars)`);
});
```

#### Python

```python
# Public endpoint - no auth needed
response = requests.get(f"{base_url}/api/posts/last10")
tours = response.json()

for tour in tours:
    print(f"{tour['title']} - ${tour['price']} ({tour['rating']} stars)")
```

### Search Tours with Filters

#### cURL

```bash
curl -X POST https://your-instance.com/api/posts/search \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 20,
    "query": "paris walking tour",
    "filters": {
      "minPrice": 50,
      "maxPrice": 200,
      "region": "Europe"
    }
  }'
```

#### JavaScript

```javascript
const searchResults = await fetch('https://your-instance.com/api/posts/search', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    page: 1,
    pageSize: 20,
    query: 'paris walking tour',
    filters: { minPrice: 50, maxPrice: 200, region: 'Europe' }
  })
});

const { items, totalCount, totalPages } = await searchResults.json();
```

### Get Tour Details

#### cURL

```bash
curl https://your-instance.com/api/posts/{postId}/retrieve
```

#### Python

```python
tour_id = "abc123-def456"
response = requests.get(f"{base_url}/api/posts/{tour_id}/retrieve")
tour = response.json()
print(f"Tour: {tour['title']}")
print(f"Guide: {tour['guideName']}")
print(f"Itineraries: {len(tour.get('itineraries', []))}")
```

---

## Tour Requests & Bidding

### Create a Tour Request

#### cURL

```bash
curl -X POST https://your-instance.com/api/tour-requests \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "3-day Paris cultural tour",
    "description": "Looking for a knowledgeable guide for museums and historical sites",
    "regionId": "europe-france",
    "budget": 500,
    "startDate": "2026-06-15",
    "endDate": "2026-06-17",
    "groupSize": 4
  }'
```

#### C#

```csharp
var request = new CreateTourRequestModel
{
    Title = "3-day Paris cultural tour",
    Description = "Looking for a knowledgeable guide for museums and historical sites",
    RegionId = "europe-france",
    Budget = 500,
    StartDate = new DateTime(2026, 6, 15),
    EndDate = new DateTime(2026, 6, 17),
    GroupSize = 4
};

var response = await client.PostAsJsonAsync("/api/tour-requests", request);
var tourRequest = await response.Content.ReadFromJsonAsync<TourRequestModel>();
Console.WriteLine($"Tour request created: {tourRequest.Id}");
```

### Place a Bid on a Tour

#### cURL

```bash
curl -X POST https://your-instance.com/api/bid/{postId}/newbid \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 150,
    "message": "I am an experienced guide with 5 years in Paris."
  }'
```

#### JavaScript

```javascript
const bidResponse = await fetch(`https://your-instance.com/api/bid/${postId}/newbid`, {
  method: 'POST',
  headers: authHeaders,
  body: JSON.stringify({
    amount: 150,
    message: 'I am an experienced guide with 5 years in Paris.'
  })
});

const result = await bidResponse.json();
console.log('Bid placed:', result);
```

### Accept or Reject a Bid

#### Python

```python
# Accept a bid
response = session.post(f"{base_url}/api/bid/{post_id}/accept")
result = response.json()

# Reject a bid
response = session.post(f"{base_url}/api/bid/{post_id}/reject")
result = response.json()
```

---

## Payments

### Create a Payment

#### cURL

```bash
curl -X POST https://your-instance.com/api/payment \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tourId": "tour-abc123",
    "amount": 150.00,
    "currency": "USD"
  }'
```

#### C#

```csharp
var payment = await client.PostAsJsonAsync("/api/payment", new
{
    tourId = "tour-abc123",
    amount = 150.00m,
    currency = "USD"
});

var paymentResult = await payment.Content.ReadFromJsonAsync<PaymentResult>();
Console.WriteLine($"Payment ID: {paymentResult.Id}, Status: {paymentResult.Status}");
```

### Get Transaction History

#### JavaScript

```javascript
const transactions = await fetch(
  'https://your-instance.com/api/payment/transactions?page=1&pageSize=20',
  { headers: authHeaders }
);

const { items, totalCount } = await transactions.json();
items.forEach(tx => {
  console.log(`${tx.date} | ${tx.type} | $${tx.amount} | ${tx.status}`);
});
```

### Confirm a Payment

#### Python

```python
payment_id = "pay-abc123"
response = session.post(f"{base_url}/api/payment/{payment_id}/confirm")
if response.ok:
    print("Payment confirmed successfully")
```

---

## Search

### Full-Text Search with Elasticsearch

#### cURL

```bash
curl -X POST https://your-instance.com/api/search/posts \
  -H "Content-Type: application/json" \
  -d '{
    "query": "historical walking tour",
    "filters": {
      "priceRange": { "min": 0, "max": 500 },
      "rating": { "min": 4.0 }
    },
    "page": 1,
    "pageSize": 20,
    "sortBy": "relevance"
  }'
```

#### JavaScript

```javascript
const searchResponse = await fetch('https://your-instance.com/api/search/posts', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    query: 'historical walking tour',
    filters: {
      priceRange: { min: 0, max: 500 },
      rating: { min: 4.0 }
    },
    page: 1,
    pageSize: 20
  })
});

const { results, facets, totalHits } = await searchResponse.json();
console.log(`Found ${totalHits} results`);
```

### Autocomplete Suggestions

#### cURL

```bash
curl -X POST https://your-instance.com/api/search/autocomplete \
  -H "Content-Type: application/json" \
  -d '{"query": "par"}'
```

#### Python

```python
response = requests.post(f"{base_url}/api/search/autocomplete", json={
    "query": "par"
})
suggestions = response.json()
for suggestion in suggestions.get("suggestions", []):
    print(suggestion["text"])
```

---

## Recommendations

### Get Personalized Recommendations

#### cURL

```bash
# With location for nearby tours
curl "https://your-instance.com/api/recommendation?count=10&lat=48.8566&lng=2.3522" \
  -H "Authorization: Bearer $TOKEN"
```

#### C#

```csharp
var recommendations = await client.GetFromJsonAsync<List<RecommendationDto>>(
    "/api/recommendation?count=10&lat=48.8566&lng=2.3522");

foreach (var rec in recommendations)
{
    Console.WriteLine($"[{rec.Score:F2}] {rec.Tour.Title} - {rec.Reason}");
}
```

### Set User Preferences

#### JavaScript

```javascript
await fetch('https://your-instance.com/api/recommendation/preferences', {
  method: 'PUT',
  headers: authHeaders,
  body: JSON.stringify({
    preferences: [
      { type: 'category', value: 'cultural' },
      { type: 'category', value: 'historical' },
      { type: 'price_range', value: '50-200' },
      { type: 'language', value: 'english' },
      { type: 'duration', value: 'half-day' }
    ]
  })
});
```

### Record Tour Interaction

#### Python

```python
# Track when a user views or interacts with a tour
session.post(f"{base_url}/api/recommendation/interactions", json={
    "tourId": "tour-abc123",
    "type": "view"  # or "click", "bookmark", "share"
})
```

---

## Real-time Notifications (SignalR)

### Connect to Notification Hub

#### JavaScript

```javascript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://your-instance.com/notify', {
    accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .build();

// Listen for notifications
connection.on('ReceiveNotification', (notification) => {
  console.log('New notification:', notification.title, notification.message);
});

// Listen for bid updates
connection.on('BidUpdate', (bidData) => {
  console.log('Bid updated:', bidData);
});

await connection.start();
console.log('Connected to notification hub');
```

### Connect to Chat Hub

#### JavaScript

```javascript
const chatConnection = new signalR.HubConnectionBuilder()
  .withUrl('https://your-instance.com/chat', {
    accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .build();

chatConnection.on('ReceiveMessage', (senderId, message, timestamp) => {
  console.log(`[${timestamp}] ${senderId}: ${message}`);
});

await chatConnection.start();

// Send a message
await chatConnection.invoke('SendMessage', recipientId, 'Hello! I am interested in your tour.');
```

#### C#

```csharp
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("https://your-instance.com/notify", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(accessToken);
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<NotificationDto>("ReceiveNotification", notification =>
{
    Console.WriteLine($"Notification: {notification.Title}");
});

await connection.StartAsync();
```

---

## Webhooks

### Register a Webhook

#### cURL

```bash
curl -X POST https://your-instance.com/api/webhook-management \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://your-app.com/webhooks/urguide",
    "events": ["tour.booked", "payment.completed", "bid.accepted"],
    "secret": "your-webhook-secret"
  }'
```

### Verify Webhook Signatures

#### JavaScript (Express.js)

```javascript
const crypto = require('crypto');

function verifyWebhookSignature(payload, signature, secret) {
  const expected = crypto
    .createHmac('sha256', secret)
    .update(payload)
    .digest('hex');
  
  return crypto.timingSafeEqual(
    Buffer.from(signature),
    Buffer.from(`sha256=${expected}`)
  );
}

app.post('/webhooks/urguide', (req, res) => {
  const signature = req.headers['x-webhook-signature'];
  const isValid = verifyWebhookSignature(
    JSON.stringify(req.body),
    signature,
    process.env.WEBHOOK_SECRET
  );
  
  if (!isValid) return res.status(401).send('Invalid signature');
  
  // Process the event
  const { event, data } = req.body;
  console.log(`Received webhook: ${event}`, data);
  
  res.status(200).send('OK');
});
```

#### Python (Flask)

```python
import hmac
import hashlib

def verify_signature(payload, signature, secret):
    expected = hmac.new(
        secret.encode(), payload.encode(), hashlib.sha256
    ).hexdigest()
    return hmac.compare_digest(f"sha256={expected}", signature)

@app.route('/webhooks/urguide', methods=['POST'])
def handle_webhook():
    signature = request.headers.get('X-Webhook-Signature')
    if not verify_signature(request.data.decode(), signature, WEBHOOK_SECRET):
        return 'Invalid signature', 401
    
    event = request.json
    print(f"Webhook received: {event['event']}")
    return 'OK', 200
```

---

## Error Handling

### Handling API Errors

#### JavaScript

```javascript
async function apiRequest(url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: { ...authHeaders, ...options.headers }
  });

  if (response.status === 401) {
    // Token expired - attempt refresh
    const newTokens = await refreshToken(currentRefreshToken);
    authHeaders.Authorization = `Bearer ${newTokens.accessToken}`;
    return apiRequest(url, options); // Retry with new token
  }

  if (response.status === 429) {
    const retryAfter = response.headers.get('Retry-After') || 60;
    console.warn(`Rate limited. Retrying in ${retryAfter}s`);
    await new Promise(r => setTimeout(r, retryAfter * 1000));
    return apiRequest(url, options); // Retry after delay
  }

  const data = await response.json();
  
  if (data.isError) {
    throw new ApiError(response.status, data.errors);
  }

  return data;
}

class ApiError extends Error {
  constructor(status, errors) {
    super(errors.join(', '));
    this.status = status;
    this.errors = errors;
  }
}
```

#### C#

```csharp
public class UrGuideApiClient
{
    private readonly HttpClient _client;
    
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await RefreshTokenAsync();
            response = await _client.GetAsync(endpoint);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<ApiEnvelope<T>>(content);
        
        if (envelope.IsError)
            throw new ApiException(envelope.Errors);
        
        return envelope.Value;
    }
}

public record ApiEnvelope<T>(T Value, List<string> Errors, bool IsError);
```

#### Python

```python
class UrGuideApiError(Exception):
    def __init__(self, status_code, errors):
        self.status_code = status_code
        self.errors = errors
        super().__init__(f"API Error {status_code}: {', '.join(errors)}")

def api_request(method, url, **kwargs):
    response = session.request(method, f"{base_url}{url}", **kwargs)
    
    if response.status_code == 401:
        new_tokens = refresh_token(current_refresh_token)
        session.headers["Authorization"] = f"Bearer {new_tokens['accessToken']}"
        response = session.request(method, f"{base_url}{url}", **kwargs)
    
    if response.status_code == 429:
        retry_after = int(response.headers.get("Retry-After", 60))
        import time
        time.sleep(retry_after)
        return api_request(method, url, **kwargs)
    
    data = response.json()
    if data.get("isError"):
        raise UrGuideApiError(response.status_code, data["errors"])
    
    return data
```

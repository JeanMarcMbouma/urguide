# UrGuide API Changelog

All notable changes to the UrGuide API are documented in this file. The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [v1.0] - Current

### Core Platform

#### Added
- RESTful API architecture with standardized response envelopes
- OpenAPI 3.0 / Swagger interactive documentation at `/swagger/index.html`
- API versioning support via URL segment, header (`X-Api-Version`), and query parameter (`api-version`)
- CORS support with configurable policies
- Response caching and output caching middleware
- Request localization for 5 languages (en, fr, es, de, ar)
- Health check endpoints at `/health` and `/alive`
- .NET Aspire integration for cloud-native orchestration and observability
- Correlation ID middleware for distributed tracing
- Request performance logging middleware

### Authentication & Security

#### Added
- JWT Bearer token authentication with 8-hour token lifetime
- OAuth2 Authorization Code flow with PKCE via Duende IdentityServer
- Social login providers: Google, Microsoft, and Apple
- Account linking for social login accounts
- Two-Factor Authentication (2FA) with TOTP
- WebAuthn/FIDO2 passkey support for passwordless authentication
- Backup recovery codes for 2FA
- Token refresh flow with single-use refresh tokens
- IP-based rate limiting on authentication endpoints
- Tiered rate limiting middleware with analytics
- Data protection with configurable key storage

### Tours & Content

#### Added
- Tour/post creation, update, and deletion (`/api/posts/`)
- Tour search with pagination and filtering (`/api/posts/search`)
- Recent and top-rated tour browsing (`/api/posts/last10`, `/api/posts/top10`)
- Tour itinerary management (`/api/posts/{id}/itineraries`)
- Seat reservation system (`/api/posts/{id}/makereservation`)
- User reaction tracking (likes/dislikes) (`/api/posts/{id}/reaction`)

### Tour Requests & Bidding

#### Added
- Tour request creation and management (`/api/tour-requests/`)
- Regional tour request browsing (`/api/tour-requests/region/{regionId}`)
- Bidding system with bid placement (`/api/bid/{postId}/newbid`)
- Bid acceptance and rejection (`/api/bid/{postId}/accept`, `/api/bid/{postId}/reject`)
- Bid history tracking (`/api/bid/{postId}/history`)
- Budget update for existing requests

### Payments & Financial

#### Added
- Stripe payment processing (`/api/payment/`)
- Payment confirmation and cancellation
- Transaction history with pagination
- Platform fee calculation (2% Basic, 5% Premium tiers)
- Guide payout management system (`/api/payout/`)
- Refund processing (`/api/refund/`)
- Financial reporting and earnings dashboard (`/api/financial/`)
- Webhook handlers for Stripe events (`/api/webhook/`)

### Search & Discovery

#### Added
- Elasticsearch-powered full-text search (`/api/search/posts`, `/api/search/tours`)
- Autocomplete suggestions (`/api/search/autocomplete`)
- Faceted search with dynamic filter options
- Search analytics tracking
- Admin reindexing endpoints (`/api/search/admin/reindex/`)
- Elasticsearch health monitoring

### Recommendations

#### Added
- Personalized tour recommendation engine (`/api/recommendation/`)
- Location-based scoring using Haversine distance
- User preference management (category, location, price range, duration, language)
- Tour interaction tracking (views, clicks, bookmarks)
- Recommendation feedback collection
- Popular tours endpoint with configurable count
- Admin statistics endpoint

### Real-time Communication

#### Added
- SignalR notification hub at `/notify` for live updates
- SignalR chat hub at `/chat` for direct messaging
- Push notification support via Firebase Cloud Messaging (FCM)
- Push notification templates with variable substitution
- In-app notification management (`/api/notification/`)
- Email template system (`/api/email-template/`)

### Guide Management

#### Added
- Guide registration and profile management
- Guide verification/KYC system (`/api/guide-verification/`)
- Availability calendar management (`/api/availability/`)
- Guide dashboard with analytics (`/api/guide-dashboard/`)
- Photo gallery management (`/api/galleries/`)
- Tour package templates (`/api/tour-template/`)

### Administration

#### Added
- Admin user management with CRUD operations (`/api/admin/`)
- Account freeze/unfreeze with timed expiry
- Enhanced audit logging with category and severity filtering
- System monitoring endpoints
- Analytics and reporting dashboard (`/api/analytics/`)
- User activity tracking (`/api/activity/`)

### Data & Compliance

#### Added
- GDPR data export with background processing (`/api/data-export/`)
- User data download endpoint
- Account deletion support
- Audit event logging with 33 event codes across 7 categories

### Integration

#### Added
- Webhook subscription management (`/api/webhook-management/`)
- 16 webhook event types with HMAC-SHA256 signing
- MassTransit message queue integration with RabbitMQ
- Lookup data endpoints for countries, regions, and currencies (`/api/lookup/`)

### Gamification & Premium

#### Added
- Gamification system with achievements and rewards (`/api/gamification/`)
- Referral and affiliate system (`/api/referral/`)
- Premium features and subscription management (`/api/premium/`)
- Review moderation system (`/api/review-moderation/`)
- Dispute resolution workflow (`/api/dispute/`)

### Documentation

#### Added
- Interactive Swagger UI with OAuth2 and JWT authentication support
- XML documentation comments included in OpenAPI specification
- API Documentation Portal with tutorials, code examples, and SDK guides
- Authentication guide covering all auth methods
- Code examples in C#, JavaScript, Python, and cURL
- Step-by-step integration tutorials
- SDK generation documentation for multiple platforms
- API versioning documentation with migration guidance

---

## Conventions

### Response Envelope

All API responses use a standardized envelope:

```json
// Success
{ "value": { ... }, "isError": false }

// Error  
{ "errors": ["message"], "isError": true }
```

### Pagination

Paginated endpoints accept `page` and `pageSize` parameters and return:

```json
{
  "items": [...],
  "totalCount": 100,
  "totalPages": 5,
  "currentPage": 1,
  "pageSize": 20
}
```

### Authentication

- Public endpoints: No authentication required
- User endpoints: `Authorization: Bearer <jwt-token>`
- Admin endpoints: Bearer token with `Admin` role claim

### Rate Limiting

- Auth endpoints: 5 requests/minute per IP
- General API: Configurable tiered rate limits
- Rate limit headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `Retry-After`

# UrGuide API - Outstanding Issues Catalog

This document catalogues all outstanding feature requests and improvements for the UrGuide Tourism Platform API.

## 🎯 High Priority Issues

### 1. Payment Integration
**Title:** Implement payment processing system  
**Labels:** enhancement, high-priority, financial  
**Status:** ✅ **COMPLETED**  
**Description:**
Integrate a payment processing system to handle tour payments, guide payouts, and platform fees.

**Implemented:**
- ✅ Stripe payment integration for tour bookings
- ✅ Support for multiple currencies
- ✅ Transaction history tracking
- ✅ Refund processing (full and partial)
- ✅ Platform fee calculation (2% for Basic, 5% for Premium)
- ✅ Secure payment data handling (PCI compliance - no card data storage)
- ✅ Webhook handling for payment events
- ✅ Payment intent creation with automatic payment methods
- ✅ Customer management in Stripe
- ✅ Guide payout system with balance tracking
- ✅ Payment status tracking (pending, processing, succeeded, failed, refunded)

**API Endpoints:**
- `POST /api/payment` - Create payment for tour booking
- `GET /api/payment/{paymentId}` - Get payment details
- `GET /api/payment/transactions` - Get user transaction history
- `POST /api/payment/{paymentId}/confirm` - Confirm payment
- `POST /api/payment/{paymentId}/cancel` - Cancel payment
- `POST /api/payout` - Request payout
- `GET /api/payout/{payoutId}` - Get payout details
- `GET /api/payout/guide/{guideId}` - Get guide payout history
- `GET /api/payout/guide/{guideId}/balance` - Get guide available balance
- `POST /api/refund` - Request refund
- `GET /api/refund/{refundId}` - Get refund details
- `GET /api/refund/payment/{paymentId}` - Get payment refunds
- `POST /api/webhook/stripe` - Handle Stripe webhook events

**Acceptance Criteria:**
- [x] Users can make payments for tours
- [x] Guides can receive payouts
- [x] Platform automatically deducts fees
- [x] Refund requests can be processed
- [x] Transaction history is tracked
- [x] Payment webhooks are handled properly

---

### 2. Docker Containerization
**Title:** Add Docker support for easy deployment  
**Labels:** enhancement, DevOps, containerization  
**Status:** ✅ **COMPLETED**  
**Description:**
Docker containers have been implemented for the UrGuide API to simplify deployment and ensure consistency across environments.

**Implemented:**
- ✅ Dockerfile for the API with multi-stage builds
- ✅ Docker Compose for local development (API + SQL Server)
- ✅ Optimized image size with multi-stage builds
- ✅ Environment variable configuration
- ✅ Health check configuration
- ✅ Volume mapping for logs and uploads
- ✅ .dockerignore for optimized builds
- ✅ Development override for hot reload

**Documentation:**
- README.md updated with Docker instructions
- docker-compose.yml with full service configuration
- docker-compose.override.yml for development

---

### 3. CI/CD Pipeline
**Title:** Implement automated CI/CD pipeline  
**Labels:** enhancement, DevOps, automation  
**Status:** ✅ **COMPLETED**  
**Description:**
Comprehensive CI/CD pipeline implemented using GitHub Actions with multi-stage workflows.

**Implemented:**
- ✅ `.github/workflows/dotnet-ci.yml` - Main CI/CD pipeline
  - Build and test on pull requests
  - NuGet package caching for faster builds
  - Automated test execution with result publishing
  - Build artifact uploading
- ✅ `.github/workflows/docker-publish.yml` - Docker image publishing
  - Automatic publishing to GitHub Container Registry
  - Multi-tag strategy (branch, version, SHA, latest)
  - Build provenance attestation
- ✅ **Code Quality & Security**:
  - CodeQL security scanning for C# code
  - Dependency vulnerability scanning
  - Vulnerability report generation and archiving
- ✅ `.github/workflows/migration-validation.yml` - Database migration validation
  - Automated migration testing against SQL Server
  - Idempotent SQL script generation
  - Migration script artifacts for deployment
- ✅ **Notifications**: Build status reporting in all workflows
- ✅ **Documentation**: README.md updated with workflow descriptions

**Pipeline Features:**
- Multi-stage execution (build → test → security → docker → notify)
- Parallel job execution for faster feedback
- Caching strategies for dependencies and Docker layers
- Conditional execution based on file paths
- Manual workflow dispatch capability

---

## 🔒 Security & Authentication

### 4. Two-Factor Authentication (2FA)
**Title:** Add two-factor authentication support  
**Labels:** enhancement, security, authentication  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement 2FA for enhanced account security using TOTP (Time-based One-Time Password) and Passkey/WebAuthn.

**Implemented:**
- ✅ TOTP-based 2FA using authenticator apps (Google Authenticator compatible)
- ✅ QR code generation for easy setup using QRCoder library
- ✅ Backup codes for account recovery (10 codes generated)
- ✅ Passkey/WebAuthn support using Fido2.AspNet
- ✅ API endpoints for 2FA management
- ✅ API endpoints for Passkey management
- ✅ Database schema updates with migration
- ✅ Updated login flow to check for 2FA requirement

**API Endpoints - TOTP 2FA:**
- `GET /api/account/2fa/status` - Get current 2FA status
- `POST /api/account/2fa/enable` - Enable 2FA and get QR code
- `POST /api/account/2fa/verify` - Verify TOTP code and complete setup
- `POST /api/account/2fa/disable` - Disable 2FA
- `POST /api/account/2fa/backup-codes/generate` - Generate new backup codes
- `POST /api/account/2fa/verify-code` - Verify 2FA code during login

**API Endpoints - Passkey/WebAuthn:**
- `POST /api/account/passkey/register/options` - Get passkey registration options
- `POST /api/account/passkey/register/complete` - Complete passkey registration
- `POST /api/account/passkey/login/options` - Get passkey login options
- `POST /api/account/passkey/login/complete` - Complete passkey login
- `GET /api/account/passkey/list` - List registered passkeys
- `DELETE /api/account/passkey/{id}` - Remove passkey

**Acceptance Criteria:**
- [x] Users can enable/disable 2FA
- [x] QR code generation works
- [x] Backup codes are generated
- [x] Login requires 2FA when enabled
- [x] Passkey support is enabled
- [x] API documentation updated
- [x] Issues catalog updated

**Note:** SMS-based 2FA was not implemented (optional requirement). The implementation focuses on TOTP and Passkey/WebAuthn which provide stronger security.

---

### 5. Social Login Integration
**Title:** Add OAuth social login providers  
**Labels:** enhancement, authentication, integration  
**Description:**
Enable users to sign in using Google, Apple, and Microsoft accounts.

**Requirements:**
- Google OAuth integration
- Apple Sign-In integration
- Microsoft OAuth integration
- Account linking (social + email)
- Profile data synchronization
- Consent management

**Acceptance Criteria:**
- [ ] Users can sign in with Google
- [ ] Users can sign in with Apple
- [ ] Users can sign in with Microsoft
- [ ] Social accounts can be linked
- [ ] Profile data is synchronized
- [ ] Documentation updated

---

## 📊 Monitoring & Observability

### 6. Enhanced Monitoring and Observability
**Title:** Implement comprehensive monitoring solution  
**Labels:** enhancement, monitoring, observability  
**Description:**
Add advanced monitoring, metrics, and distributed tracing for production environments.

**Requirements:**
- Application Insights or similar APM
- Prometheus metrics endpoint
- Distributed tracing (OpenTelemetry)
- Custom metrics for business events
- Performance monitoring
- Error tracking and alerting

**Acceptance Criteria:**
- [ ] APM is configured
- [ ] Metrics endpoint is available
- [ ] Distributed tracing works
- [ ] Custom metrics are tracked
- [ ] Alerts are configured
- [ ] Dashboard is created

---

### 7. Structured Logging Enhancement
**Title:** Enhance structured logging with correlation IDs  
**Labels:** enhancement, logging, observability  
**Description:**
Improve logging with correlation IDs, structured data, and better log aggregation support.

**Requirements:**
- Correlation ID propagation
- Structured logging with JSON output
- Log aggregation support (Elasticsearch, Seq)
- Performance logging
- Security event logging
- User activity logging

**Acceptance Criteria:**
- [ ] Correlation IDs are generated and propagated
- [ ] Logs are in JSON format
- [ ] Log aggregation is configured
- [ ] Performance metrics are logged
- [ ] Security events are logged
- [ ] Documentation updated

---

## 🧪 Testing & Quality

### 8. Unit Testing Suite
**Title:** Implement comprehensive unit tests  
**Labels:** enhancement, testing, quality, unit-tests  
**Description:**
Create unit tests for all service classes, business logic, and utilities.

**Note:** This issue is a refinement of the original "API Testing Suite" (#8), which had a broad scope covering all testing types. The original issue has been split into four focused issues for better tracking: #8 (Unit Testing), #18 (Integration Testing), #36 (E2E Testing), and #37 (Performance Testing).

**Requirements:**
- xUnit testing framework
- Moq for mocking dependencies
- FluentAssertions for readable assertions
- Test coverage reporting with Coverlet
- Tests for all services in UrGuide.Services
- Tests for utilities in UrGuide.Core
- Repository pattern tests
- Validation logic tests

**Test Coverage Goals:**
- Service classes: >90%
- Business logic: >95%
- Utilities: >85%
- Overall: >80%

**Acceptance Criteria:**
- [ ] Unit tests for all service classes
- [ ] Mocking external dependencies
- [ ] Test coverage >80%
- [ ] CI/CD integration
- [ ] Coverage reports generated
- [ ] All tests pass consistently

---

### 18. Integration Testing Suite
**Title:** Implement API integration tests  
**Labels:** enhancement, testing, quality, integration-tests  
**Description:**
Create integration tests for API endpoints using WebApplicationFactory.

**Requirements:**
- xUnit with WebApplicationFactory
- In-memory database for testing
- Test authentication/authorization
- Test all API endpoints
- Test error scenarios
- Test validation logic
- Test middleware behavior

**Test Categories:**
- Authentication endpoints
- User management endpoints
- Guide management endpoints
- Tour and bidding endpoints
- Payment endpoints
- Webhook endpoints
- Admin endpoints

**Acceptance Criteria:**
- [ ] Integration tests for all controllers
- [ ] Database integration tests
- [ ] Auth/authz tests comprehensive
- [ ] Error handling tests
- [ ] Validation tests
- [ ] Tests run in CI/CD
- [ ] All tests pass consistently

---

### 36. End-to-End Testing Suite
**Title:** Implement E2E tests with Playwright  
**Labels:** enhancement, testing, quality, e2e-tests  
**Description:**
Create end-to-end tests for critical user workflows using Playwright.

**Requirements:**
- Playwright for .NET or TypeScript
- Test critical user journeys
- Visual regression testing
- Cross-browser testing
- Mobile viewport testing

**Test Scenarios:**
- User registration and login flow
- Guide registration and profile setup
- Tour request creation and bidding
- Payment processing flow
- Booking completion
- Review submission

**Acceptance Criteria:**
- [ ] E2E tests for critical workflows
- [ ] Cross-browser testing
- [ ] Mobile testing
- [ ] Visual regression tests
- [ ] Tests run in CI/CD
- [ ] All tests stable and pass consistently

---

### 37. Performance and Load Testing
**Title:** Implement performance and load tests  
**Labels:** enhancement, testing, performance  
**Description:**
Create performance benchmarks and load tests to ensure scalability.

**Requirements:**
- BenchmarkDotNet for micro-benchmarks
- k6 or JMeter for load testing
- Performance baselines
- Load testing scenarios
- Stress testing
- Spike testing

**Test Scenarios:**
- Search API performance (Elasticsearch)
- Payment processing under load
- Real-time notifications (SignalR)
- Database query performance
- API rate limiting behavior
- Concurrent user scenarios

**Acceptance Criteria:**
- [ ] Benchmarks for critical operations
- [ ] Load tests for main endpoints
- [ ] Performance baselines established
- [ ] Bottlenecks identified
- [ ] Results documented
- [ ] CI/CD integration

---

## 🚀 Features & Enhancements

### 9. API Rate Limiting Improvements
**Title:** Enhance rate limiting with tiered limits  
**Labels:** enhancement, performance, API  
**Status:** ✅ **COMPLETED**  
**Description:**
Improve rate limiting to support different tiers based on user roles and subscription levels.

**Implemented:**
- ✅ Tiered rate limits (Anonymous, Authenticated, Premium, Internal)
- ✅ Rate limit headers in responses
- ✅ Custom rate limits per endpoint
- ✅ Rate limit analytics tracking
- ✅ Graceful degradation
- ✅ Rate limit exemptions for internal services
- ✅ Distributed caching support for rate limit state
- ✅ Middleware-based implementation (TieredRateLimitMiddleware)

**Acceptance Criteria:**
- [x] Different limits for different user tiers
- [x] Rate limit headers are returned
- [x] Custom limits work per endpoint
- [x] Analytics track rate limit hits
- [x] Exemptions work for internal calls
- [x] Documentation updated

---

### 10. API Client SDK Generation
**Title:** Generate client SDKs for popular languages  
**Labels:** enhancement, developer-experience, SDK  
**Description:**
Automatically generate client SDKs for .NET, JavaScript/TypeScript, Python, and Java.

**Requirements:**
- OpenAPI spec generation
- NSwag or similar SDK generator
- .NET client SDK
- TypeScript/JavaScript SDK
- Python SDK
- Java SDK
- SDK documentation
- SDK versioning

**Acceptance Criteria:**
- [ ] OpenAPI spec is complete
- [ ] .NET SDK is generated
- [ ] TypeScript SDK is generated
- [ ] Python SDK is generated
- [ ] Java SDK is generated
- [ ] SDKs are published to package managers
- [ ] Documentation is created

---

### 11. GDPR Compliance - Data Export
**Title:** Implement GDPR-compliant data export functionality  
**Labels:** enhancement, compliance, privacy  
**Status:** ✅ **COMPLETED**  
**Description:**
Allow users to export all their personal data in machine-readable format for GDPR compliance.

**Implemented:**
- ✅ DataExportRequest entity for tracking export requests
- ✅ Comprehensive data export service with JSON and CSV support
- ✅ API endpoints for requesting, checking status, and downloading exports
- ✅ Email notifications when export is ready
- ✅ Secure download tokens with 7-day expiration
- ✅ Background service for processing exports and cleanup
- ✅ Database migration for DataExportRequests table

**API Endpoints:**
- `POST /api/dataexport/request` - Request data export (JSON or CSV format)
- `GET /api/dataexport/status/{requestId}` - Check export status
- `GET /api/dataexport/download/{token}` - Download export file (secure token-based)
- `DELETE /api/dataexport/{requestId}` - Cancel pending export

**Exported Data Includes:**
- User profile information
- Account metadata (email, creation date, last activity)
- Activity history (audit events)
- Given feedback/reviews
- Received feedback/reviews
- Tour posts
- Tour requests
- Bids on tours
- Image galleries/catalogs
- Notifications

**Export Formats:**
- **JSON**: Single JSON file with all user data
- **CSV**: ZIP archive containing multiple CSV files for different data types

**Acceptance Criteria:**
- [x] Users can request data export
- [x] All personal data is included
- [x] Export is in JSON/CSV format
- [x] Download link is secure (token-based)
- [x] Email notification is sent
- [x] Export expires after 7 days

---

### 12. Advanced Search and Filtering
**Title:** Implement advanced search with Elasticsearch  
**Labels:** enhancement, search, performance  
**Status:** ✅ **COMPLETED**  
**Description:**
Integrate Elasticsearch for advanced search capabilities with fuzzy matching, filters, and facets.

**Implemented:**
- ✅ Elasticsearch 8.11.0 integration with NEST 7.17.5 client
- ✅ Docker Compose configuration for Elasticsearch service
- ✅ Fuzzy search with configurable fuzziness (AUTO, 0-2)
- ✅ Multi-field search across text, description, tags, and location
- ✅ Advanced filters: location, geo-distance, price range, rating, date ranges, tags, seat availability
- ✅ Faceted search with aggregations by tags, locations, and rating distribution
- ✅ SearchAnalytics entity for tracking queries, results, timing, and user behavior
- ✅ Autocomplete/suggestions with search-as-you-type functionality
- ✅ Automatic indexing on Post create/update/delete operations
- ✅ Bulk re-indexing endpoints for admin users
- ✅ Health check integration for Elasticsearch connectivity

**API Endpoints:**
- `POST /api/search/posts` - Advanced post search with filters and facets
- `POST /api/search/tours` - Advanced tour search
- `POST /api/search/autocomplete` - Autocomplete suggestions
- `GET /api/search/health` - Elasticsearch health check
- `POST /api/search/admin/reindex/posts` - Bulk re-index all posts (admin only)
- `POST /api/search/admin/reindex/tours` - Bulk re-index all tours (admin only)

**Database Migration:**
- `20260209081856_AddSearchAnalytics` - SearchAnalytics table with indexes

**Acceptance Criteria:**
- [x] Elasticsearch is integrated
- [x] Fuzzy search works
- [x] Multi-field search is implemented
- [x] Filters work correctly
- [x] Facets are returned
- [x] Analytics track searches
- [x] Autocomplete suggestions work

---

## 📱 Mobile & Integration

### 13. Mobile App Push Notifications
**Title:** Add push notification support for mobile apps  
**Labels:** enhancement, mobile, notifications  
**Description:**
Integrate Firebase Cloud Messaging (FCM) and Apple Push Notification Service (APNs) for mobile push notifications.

**Requirements:**
- FCM integration for Android
- APNs integration for iOS
- Device token registration API
- Notification sending API
- Notification templates
- Delivery tracking
- Opt-in/opt-out management

**Acceptance Criteria:**
- [ ] FCM is integrated
- [ ] APNs is integrated
- [ ] Device registration works
- [ ] Notifications are delivered
- [ ] Templates are configurable
- [ ] Users can opt-out
- [ ] Documentation updated

---

### 14. Webhook System
**Title:** Implement webhooks for external integrations  
**Labels:** enhancement, integration, webhooks  
**Status:** ✅ **COMPLETED**  
**Description:**
Create a webhook system to notify external systems of important events.

**Implemented:**
- ✅ Webhook registration API with secure secret generation
- ✅ Event subscription management for multiple event types
- ✅ Payload signing using HMAC-SHA256 for security
- ✅ Retry logic with exponential backoff (5s, 15s, 45s, 135s)
- ✅ Comprehensive webhook delivery history and logs
- ✅ Test webhook endpoint for validation
- ✅ Database schema with EF Core migration
- ✅ Support for 14 different event types (payments, bookings, tours, users, reviews)

**API Endpoints:**
- `POST /api/webhook-management` - Register new webhook
- `GET /api/webhook-management` - List user webhooks
- `GET /api/webhook-management/{id}` - Get webhook details
- `PUT /api/webhook-management/{id}` - Update webhook
- `DELETE /api/webhook-management/{id}` - Delete webhook
- `GET /api/webhook-management/{id}/deliveries` - Get delivery history
- `POST /api/webhook-management/test` - Test webhook endpoint

**Acceptance Criteria:**
- [x] Webhooks can be registered
- [x] Events trigger webhooks
- [x] Payloads are signed with HMAC-SHA256
- [x] Retries work correctly with exponential backoff
- [x] History is logged in webhook_deliveries table
- [x] Test endpoint is available
- [x] Integration guide documented

---

## 📦 Infrastructure

### 15. Redis Caching Integration
**Title:** Add Redis for distributed caching  
**Labels:** enhancement, performance, infrastructure  
**Description:**
Integrate Redis for distributed caching to improve performance and enable horizontal scaling.

**Requirements:**
- Redis integration
- Cache-aside pattern implementation
- Distributed session storage
- Rate limiting with Redis
- Cache invalidation strategy
- Redis Sentinel for high availability

**Acceptance Criteria:**
- [ ] Redis is integrated
- [ ] Caching works correctly
- [ ] Sessions are distributed
- [ ] Rate limiting uses Redis
- [ ] Invalidation works
- [ ] HA is configured

---

### 16. Message Queue Integration
**Title:** Add message queue for asynchronous processing  
**Labels:** enhancement, performance, async  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement RabbitMQ message queue for asynchronous processing of emails, images, and notifications using MassTransit 8.3.4.

**Implemented:**
- ✅ MassTransit 8.3.4 with RabbitMQ integration
- ✅ Three dedicated message queues (email, image-processing, notification)
- ✅ Message contracts for asynchronous operations
- ✅ Consumer implementations with retry policies
- ✅ Email sending via queue with SendGrid integration
- ✅ Image processing via background queue
- ✅ Notification dispatch via queue with SignalR integration
- ✅ Dead letter queue handling (automatic via MassTransit)
- ✅ RabbitMQ health monitoring integration
- ✅ Docker Compose configuration with RabbitMQ service
- ✅ Configurable opt-in/opt-out for async processing
- ✅ Avatar URL persistence to User.ProfileImage

**Configuration:**
- RabbitMQ connection settings in appsettings.json
- Optional flag `MessageQueue:UseQueuedServices` to enable/disable async processing
- Retry policies: 5s/15s/30s for email and notifications, 10s/30s/60s for images
- RabbitMQ Management UI available at http://localhost:15672

**API Monitoring:**
- `/health` endpoint includes RabbitMQ connection status
- Health checks validate queue connectivity

**Documentation:**
- README.md updated with message queue section
- MESSAGE_QUEUE_IMPLEMENTATION.md with detailed architecture
- Docker Compose setup guide included

**Acceptance Criteria:**
- [x] Message queue is configured
- [x] Emails are sent asynchronously
- [x] Images are processed in background
- [x] Notifications are queued
- [x] Dead letters are handled
- [x] Monitoring dashboard exists

---

## 📈 Analytics & Reporting

### 17. Analytics Dashboard
**Title:** Create analytics dashboard for administrators  
**Labels:** enhancement, analytics, admin  
**Status:** ✅ **COMPLETED**  
**Description:**
Build an analytics dashboard showing key metrics and trends.

**Implemented:**
- ✅ User registration trends with growth rate calculation (using CreatedAt field)
- ✅ Tour booking statistics and completion rates
- ✅ Revenue metrics with platform fees and payouts breakdown
- ✅ Guide performance metrics with top performers
- ✅ Popular destinations by bookings and revenue
- ✅ Conversion funnel tracking (requests → bids → bookings → completions)
- ✅ Data export capabilities (JSON and CSV formats)
- ✅ Flexible date range filtering
- ✅ Period grouping (hourly, daily, weekly, monthly, yearly)
- ✅ Admin-only access with role-based authorization
- ✅ Added CreatedAt field to User entity for accurate registration tracking

**API Endpoints:**
- `GET /api/analytics/dashboard` - Get complete dashboard with all metrics
- `GET /api/analytics/user-registration-trends` - User registration trends and growth
- `GET /api/analytics/tour-booking-statistics` - Tour booking statistics
- `GET /api/analytics/revenue-metrics` - Revenue, fees, payouts, and refunds
- `GET /api/analytics/guide-performance` - Guide performance metrics and top performers
- `GET /api/analytics/popular-destinations` - Most popular destinations
- `GET /api/analytics/conversion-funnel` - Conversion funnel analysis
- `GET /api/analytics/export` - Export analytics data in JSON or CSV

**Acceptance Criteria:**
- [x] Dashboard displays key metrics
- [x] Trends can be visualized (data provided via API)
- [x] Filters work correctly (date range, period grouping)
- [x] Data can be exported (JSON and CSV formats)
- [x] Real-time updates (queries live database)
- [x] API provides mobile-friendly JSON responses

---

## 🎨 Frontend & User Interface

### 19. Admin Dashboard Web Application
**Title:** Build comprehensive admin dashboard  
**Labels:** enhancement, frontend, admin, high-priority  
**Description:**
Create a modern web-based admin dashboard for platform management and monitoring.

**Technology Choice:**
- React 18+ with TypeScript (recommended) OR Vue 3 with TypeScript
- UI Framework: Material-UI (MUI) for React OR Vuetify for Vue
- State Management: Redux Toolkit (React) OR Pinia (Vue)
- API Client: Auto-generated from OpenAPI spec
- Charts: Recharts or Chart.js

**Core Features:**
- User management (view, edit, suspend, delete users)
- Guide approval and verification workflow
- Tour post moderation and management
- Payment and transaction monitoring
- Analytics dashboard integration
- Webhook management interface
- System health and monitoring
- Audit log viewer
- Content moderation tools
- Platform configuration management

**Acceptance Criteria:**
- [ ] Admin authentication and authorization
- [ ] User management interface
- [ ] Guide verification workflow
- [ ] Tour post moderation
- [ ] Financial dashboard with charts
- [ ] Real-time notifications via SignalR
- [ ] Responsive design (mobile-friendly)
- [ ] Role-based access control (Super Admin, Admin, Moderator)
- [ ] Audit trail for all admin actions

---

### 20. Public-Facing Website (Tourist Platform)
**Title:** Build main tourist-facing web application  
**Labels:** enhancement, frontend, website, high-priority  
**Description:**
Create the main public website for tourists to discover guides, request tours, and manage bookings.

**Technology Choice:**
- React 18+ with TypeScript (recommended) OR Vue 3 with TypeScript
- UI Framework: Material-UI (MUI) or Tailwind CSS
- Routing: React Router v6 OR Vue Router
- State Management: Redux Toolkit OR Pinia
- Map Integration: Google Maps or Mapbox
- Real-time: SignalR client integration

**Core Features:**
- Homepage with featured guides and destinations
- Guide discovery and search (with Elasticsearch integration)
- Advanced filtering (location, price, rating, availability)
- Guide profile pages with reviews and galleries
- Tour request creation and management
- Bidding system interface
- Booking management
- User profile and settings
- Payment interface (Stripe Elements)
- Review and rating system
- Real-time notifications
- Multi-currency support
- Responsive design (mobile-first)

**Acceptance Criteria:**
- [ ] User registration and login (OAuth + 2FA)
- [ ] Guide search and discovery
- [ ] Tour request workflow
- [ ] Bidding interface
- [ ] Secure payment processing
- [ ] Booking management
- [ ] Review system
- [ ] Real-time notifications
- [ ] Mobile-responsive design
- [ ] SEO optimization
- [ ] Performance optimized (lazy loading, code splitting)

---

### 21. Guide Portal Web Application
**Title:** Build dedicated guide management portal  
**Labels:** enhancement, frontend, guide-portal  
**Description:**
Create a dedicated portal for guides to manage their profiles, respond to bids, and track earnings.

**Core Features:**
- Guide registration and onboarding
- Profile management with photo galleries
- Tour request inbox
- Bidding interface (create, edit, track bids)
- Booking calendar and availability management
- Earnings dashboard with payout requests
- Review and rating management
- Client communication interface
- Tour history and statistics
- Performance analytics

**Acceptance Criteria:**
- [ ] Guide onboarding flow
- [ ] Profile and gallery management
- [ ] Bid management interface
- [ ] Calendar integration
- [ ] Financial dashboard
- [ ] Real-time notifications
- [ ] Mobile-responsive design
- [ ] Performance metrics

---

### 22. Mobile-Responsive PWA Features
**Title:** Progressive Web App enhancements  
**Labels:** enhancement, pwa, mobile  
**Description:**
Add PWA capabilities to web applications for app-like experience on mobile devices.

**Requirements:**
- Service worker for offline support
- Web app manifest
- Push notification support (via FCM)
- Install prompt
- Offline mode for critical features
- Background sync
- Cache strategies

**Acceptance Criteria:**
- [ ] Service worker implemented
- [ ] Offline mode works
- [ ] Push notifications functional
- [ ] App installable on mobile
- [ ] Background sync enabled
- [ ] Lighthouse PWA score >90

---

## 💡 Additional Platform Enhancements

### 23. Content Delivery Network (CDN) Integration
**Title:** Implement CDN for static assets and media  
**Labels:** enhancement, infrastructure, performance  
**Description:**
Integrate a CDN service to improve global content delivery performance and reduce server load.

**Requirements:**
- CDN provider integration (Cloudflare, Azure CDN, or AWS CloudFront)
- Static asset caching (CSS, JS, images)
- Media file delivery optimization
- Cache invalidation strategy
- Custom domain support
- SSL/TLS support
- Bandwidth optimization
- Geographic distribution

**Acceptance Criteria:**
- [ ] CDN integrated and configured
- [ ] Static assets served via CDN
- [ ] Media files cached appropriately
- [ ] Cache invalidation works
- [ ] Performance improvement measured
- [ ] SSL/TLS enabled

---

### 24. Real-time Chat System
**Title:** Implement real-time chat between tourists and guides  
**Labels:** enhancement, communication, real-time  
**Description:**
Add a real-time chat system using SignalR for communication between tourists and guides.

**Requirements:**
- SignalR hub for chat
- Message persistence in database
- Chat history retrieval
- Online/offline status
- Typing indicators
- Read receipts
- File/image sharing
- Message notifications

**Acceptance Criteria:**
- [ ] Real-time messaging works
- [ ] Messages are persisted
- [ ] Chat history accessible
- [ ] Notifications for new messages
- [ ] File sharing implemented
- [ ] Mobile-friendly chat UI

---

### 25. Review Moderation System
**Title:** Implement review moderation and flagging  
**Labels:** enhancement, moderation, content-management  
**Description:**
Add moderation tools for reviews to prevent spam and inappropriate content.

**Requirements:**
- Review flagging system
- Admin review queue
- Automated spam detection
- Profanity filtering
- Review approval workflow
- Appeal system
- Moderation analytics

**Acceptance Criteria:**
- [ ] Users can flag inappropriate reviews
- [ ] Admin moderation queue
- [ ] Automated spam detection
- [ ] Review approval workflow
- [ ] Appeal process
- [ ] Moderation reports

---

### 26. Booking Calendar and Availability
**Title:** Implement calendar system for guide availability  
**Labels:** enhancement, scheduling, calendar  
**Description:**
Add calendar functionality for guides to manage availability and tourists to book specific dates.

**Requirements:**
- iCal format support
- Google Calendar integration
- Availability slots management
- Block dates/times
- Recurring availability patterns
- Timezone support
- Calendar sync
- Booking conflict prevention

**Acceptance Criteria:**
- [ ] Guides can set availability
- [ ] Calendar view implemented
- [ ] Booking conflicts prevented
- [ ] iCal export/import
- [ ] Google Calendar sync
- [ ] Timezone handling

---

### 27. Tour Package Templates
**Title:** Create reusable tour package templates  
**Labels:** enhancement, tours, templates  
**Description:**
Allow guides to create and reuse tour package templates for common offerings.

**Requirements:**
- Template creation and management
- Pre-filled tour details
- Pricing templates
- Itinerary templates
- Image galleries
- Template categories
- Template sharing (optional)

**Acceptance Criteria:**
- [ ] Guides can create templates
- [ ] Templates can be edited
- [ ] Quick tour creation from templates
- [ ] Template categories
- [ ] Template preview

---

### 28. Referral and Affiliate System
**Title:** Implement referral program for user growth  
**Labels:** enhancement, growth, gamification  
**Description:**
Create a referral system to incentivize user growth and guide recruitment.

**Requirements:**
- Unique referral codes
- Referral tracking
- Reward calculation
- Referral dashboard
- Affiliate links
- Commission tracking
- Payout integration

**Acceptance Criteria:**
- [ ] Referral codes generated
- [ ] Tracking system works
- [ ] Rewards calculated correctly
- [ ] Dashboard shows referrals
- [ ] Automated payouts

---

### 29. Advanced Image Management
**Title:** Enhance image handling with compression and CDN  
**Labels:** enhancement, media, performance  
**Description:**
Improve image management with automatic optimization, thumbnails, and CDN integration.

**Requirements:**
- Image compression (WebP, AVIF)
- Automatic thumbnail generation
- Multiple size variants
- CDN integration (Cloudflare, Azure CDN)
- Lazy loading support
- Image watermarking
- EXIF data handling

**Acceptance Criteria:**
- [ ] Images automatically compressed
- [ ] Thumbnails generated
- [ ] CDN integration
- [ ] Multiple formats supported
- [ ] Watermarking optional

---

### 30. Dispute Resolution System
**Title:** Implement dispute resolution for bookings  
**Labels:** enhancement, support, dispute-management  
**Description:**
Add a system for handling disputes between tourists and guides.

**Requirements:**
- Dispute creation
- Evidence submission
- Admin review workflow
- Resolution tracking
- Refund processing integration
- Communication thread
- Dispute history
- Escalation process

**Acceptance Criteria:**
- [ ] Users can file disputes
- [ ] Evidence upload works
- [ ] Admin review interface
- [ ] Resolution tracking
- [ ] Automated refunds

---

### 31. Email Template System
**Title:** Create customizable email template system  
**Labels:** enhancement, communication, email  
**Description:**
Implement a flexible email template system with customization and localization.

**Requirements:**
- Template editor
- Variable substitution
- HTML email templates
- Plain text fallback
- Template versioning
- A/B testing support
- Preview functionality
- Multi-language templates

**Acceptance Criteria:**
- [ ] Template editor works
- [ ] Variables substituted correctly
- [ ] HTML and text versions
- [ ] Preview functionality
- [ ] Multi-language support

---

### 32. Tour Recommendation Engine
**Title:** Build AI/ML-powered tour recommendations  
**Labels:** enhancement, ai, recommendations  
**Description:**
Implement a recommendation system to suggest relevant tours and guides to users.

**Requirements:**
- Collaborative filtering
- Content-based recommendations
- User preference tracking
- Popularity-based suggestions
- Location-based recommendations
- Rating-weighted scoring
- Real-time updates
- A/B testing framework

**Acceptance Criteria:**
- [ ] Recommendations generated
- [ ] Personalized suggestions
- [ ] Location-aware recommendations
- [ ] Performance optimized
- [ ] A/B testing enabled

---

### 33. Advanced Reporting System
**Title:** Create comprehensive reporting for guides and admins  
**Labels:** enhancement, reporting, analytics  
**Description:**
Build detailed reporting system for financial, performance, and operational insights.

**Requirements:**
- PDF report generation
- Scheduled reports
- Custom date ranges
- Export formats (PDF, Excel, CSV)
- Financial reports
- Performance reports
- Operational reports
- Email delivery

**Report Types:**
- Guide earnings reports
- Booking summaries
- Tax documents
- Performance metrics
- Customer satisfaction
- Platform revenue

**Acceptance Criteria:**
- [ ] PDF generation works
- [ ] Scheduled reports
- [ ] Multiple formats supported
- [ ] Email delivery
- [ ] Custom date ranges

---

### 34. Backup and Disaster Recovery
**Title:** Implement automated backup and disaster recovery  
**Labels:** enhancement, infrastructure, backup, high-priority  
**Description:**
Create comprehensive backup strategy and disaster recovery plan.

**Requirements:**
- Automated database backups
- File storage backups
- Backup verification
- Point-in-time recovery
- Disaster recovery plan
- Backup retention policy
- Recovery testing
- Documentation

**Acceptance Criteria:**
- [ ] Daily automated backups
- [ ] Backup verification
- [ ] Recovery procedures tested
- [ ] Documentation complete
- [ ] RTO/RPO defined

---

### 35. API Documentation Portal
**Title:** Create interactive API documentation portal  
**Labels:** enhancement, documentation, developer-experience  
**Description:**
Build a comprehensive API documentation portal with interactive examples.

**Requirements:**
- OpenAPI/Swagger integration
- Interactive API explorer
- Code examples (multiple languages)
- Authentication guide
- Tutorial documentation
- SDK documentation
- Changelog
- Versioning support

**Acceptance Criteria:**
- [ ] Interactive documentation
- [ ] Code examples provided
- [ ] Authentication guides
- [ ] Tutorials available
- [ ] SDK documentation
- [ ] Regularly updated

---

## 🌍 Localization

### 38. Multi-language Support
**Title:** Add internationalization (i18n) support  
**Labels:** enhancement, i18n, localization  
**Description:**
Implement multi-language support for API responses and error messages.

**Note:** Renumbered from original #18 to #38 to maintain consecutive issue numbering after the testing suite split created new issues #18 (Integration Testing), #36 (E2E Testing), and #37 (Performance Testing).

**Requirements:**
- Resource file structure
- Accept-Language header support
- Translated error messages
- Translated email templates
- Language detection
- Admin UI for managing translations

**Acceptance Criteria:**
- [ ] Multiple languages supported
- [ ] Accept-Language header works
- [ ] Error messages are translated
- [ ] Email templates are translated
- [ ] Language detection works
- [ ] Admin can manage translations

---

## Summary

**Total Issues: 38**

**By Status:**
- ✅ Completed: 10 (Docker Containerization, CI/CD Pipeline, Payment Integration, Two-Factor Authentication, GDPR Data Export, Message Queue Integration, Advanced Search & Filtering, Webhook System, Analytics Dashboard, API Rate Limiting Improvements)
- 🚧 In Progress: 0
- 📋 Pending: 28

**By Priority:**
- High Priority: 4 (Payment Integration ✅, 2FA ✅, Admin Dashboard 📋, Backup & DR 📋)
- Medium Priority: 18
- Nice to Have: 17
- ✅ Completed: 10

**By Category:**
- **Frontend & UI**: 4 new issues (#19-22: Admin Dashboard, Tourist Website, Guide Portal, PWA Features)
- **Testing & Quality**: 4 issues (#8, #18, #36-37: Unit, Integration, E2E, Performance Tests)
- **Security & Authentication**: 2 (1 completed: 2FA ✅, 1 pending: Social Login)
- **Monitoring & Observability**: 2 (0 completed, 2 pending)
- **Features & Enhancements**: 5 (3 completed: GDPR Data Export ✅, Advanced Search ✅, API Rate Limiting ✅, 2 pending)
- **Mobile & Integration**: 2 (1 completed: Webhook System ✅, 1 pending: Push Notifications)
- **Infrastructure**: 4 (1 completed: Message Queue ✅, 3 pending: Redis Caching, CDN, Backup & DR)
- **Analytics & Reporting**: 2 (1 completed: Analytics Dashboard ✅, 1 pending: Advanced Reporting)
- **Communication**: 2 new issues (#24, #31: Real-time Chat, Email Templates)
- **Content Management**: 2 new issues (#25, #27: Review Moderation, Tour Templates)
- **Platform Growth**: 1 new issue (#28: Referral System)
- **Media Management**: 1 new issue (#29: Advanced Image Management)
- **Support & Dispute**: 1 new issue (#30: Dispute Resolution)
- **AI & Recommendations**: 1 new issue (#32: Tour Recommendation Engine)
- **Scheduling**: 1 new issue (#26: Booking Calendar)
- **Documentation**: 1 new issue (#35: API Documentation Portal)
- **Localization**: 1 (#38: Multi-language Support - pending)
- **DevOps**: 2 (✅ Both completed)
- **Financial**: 1 (✅ Payment Integration completed)
- **Compliance**: 1 (✅ GDPR Data Export completed)
- **Search & Performance**: 2 (1 completed: Advanced Search ✅, 1 pending: CDN)

**Priority Recommendations:**
1. **Admin Dashboard** (#19) - Critical for platform management
2. **Tourist Website** (#20) - Core user-facing application
3. **Unit Testing Suite** (#8) - Essential for code quality
4. **Integration Testing** (#18) - Ensure API reliability
5. **Backup & DR** (#34) - Protect platform data
6. **Guide Portal** (#21) - Enable guide onboarding
7. **Real-time Chat** (#24) - Improve user communication
8. **Calendar System** (#26) - Essential booking feature

**Issue Numbering History:**
- **Original catalog**: Issues #1-18
- **Testing suite split**: Original #8 "API Testing Suite" was split into four focused issues:
  - #8: Unit Testing Suite (refinement of original #8)
  - #18: Integration Testing Suite (new)
  - #36: E2E Testing Suite (new)
  - #37: Performance and Load Testing (new)
- **Renumbering**: Original #18 "Multi-language Support" renumbered to #38 to maintain sequential order
- **New additions**: 
  - Frontend issues: #19-22
  - Infrastructure: #23 (CDN)
  - Platform enhancements: #24-35

These issues provide a comprehensive roadmap for building a complete tourism platform with robust backend API, user-facing applications, and essential supporting features.

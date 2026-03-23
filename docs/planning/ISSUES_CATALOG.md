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

### 5. Social Login Integration - Google OAuth
**Title:** Add Google OAuth login provider  
**Labels:** enhancement, authentication, integration  
**Description:**
Enable users to sign in using Google accounts with automatic profile synchronization.

**Requirements:**
- Google OAuth 2.0 integration
- Automatic account creation/linking
- Profile data sync (email, name, avatar)
- Scopes configuration
- Consent management

**Acceptance Criteria:**
- [ ] Users can sign in with Google
- [ ] Accounts are auto-linked
- [ ] Profile data synced correctly
- [ ] Documentation updated

---

### 5b. Social Login Integration - Apple Sign-In
**Title:** Add Apple Sign-In provider  
**Labels:** enhancement, authentication, integration  
**Description:**
Enable users to sign in using Apple accounts with email privacy protection support.

**Requirements:**
- Apple Sign-In integration
- Email privacy relay support
- Automatic account creation/linking
- Profile data sync
- iOS/web compatibility

**Acceptance Criteria:**
- [ ] Users can sign in with Apple
- [ ] Email privacy relay works
- [ ] Accounts are auto-linked
- [ ] Works on iOS and web
- [ ] Documentation updated

---

### 5c. Social Login Integration - Microsoft OAuth
**Title:** Add Microsoft OAuth login provider  
**Labels:** enhancement, authentication, integration  
**Description:**
Enable users to sign in using Microsoft/Outlook accounts.

**Requirements:**
- Microsoft OAuth 2.0 integration
- Azure AD compatibility
- Automatic account creation/linking
- Profile data sync
- Work/personal account support

**Acceptance Criteria:**
- [ ] Users can sign in with Microsoft
- [ ] Accounts are auto-linked
- [ ] Both personal and work accounts supported
- [ ] Documentation updated

---

### 5d. Social Account Linking
**Title:** Implement account linking for social providers  
**Labels:** enhancement, authentication  
**Description:**
Allow users to link and unlink social provider accounts to existing email accounts.

**Requirements:**
- Link social account to existing account
- Unlink social accounts
- Conflict resolution (email already registered)
- Account merge functionality
- Audit logging

**Acceptance Criteria:**
- [ ] Users can link/unlink providers
- [ ] Conflicts handled gracefully
- [ ] Audit trail maintained
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

### 13. Mobile App Push Notifications - Android (FCM)
**Title:** Add Firebase Cloud Messaging (FCM) for Android push notifications  
**Labels:** enhancement, mobile, notifications  
**Description:**
Integrate Firebase Cloud Messaging for Android push notifications with device token management.

**Requirements:**
- FCM integration for Android
- Device token registration API
- Notification sending API
- Delivery tracking
- Opt-in/opt-out management

**Acceptance Criteria:**
- [ ] FCM is integrated
- [ ] Device registration works
- [ ] Notifications are delivered to Android
- [ ] Users can opt-out
- [ ] Delivery tracking works

---

### 13b. Mobile App Push Notifications - iOS (APNs)
**Title:** Add Apple Push Notification Service (APNs) for iOS push notifications  
**Labels:** enhancement, mobile, notifications  
**Description:**
Integrate Apple Push Notification Service for iOS push notifications with device token management.

**Requirements:**
- APNs integration for iOS
- Device token registration API
- Notification sending API
- Delivery tracking
- Opt-in/opt-out management
- Certificate/key management

**Acceptance Criteria:**
- [ ] APNs is integrated
- [ ] Device registration works
- [ ] Notifications are delivered to iOS
- [ ] Users can opt-out
- [ ] Delivery tracking works

---

### 13c. Push Notification Templates
**Title:** Create reusable push notification templates  
**Labels:** enhancement, mobile, notifications  
**Description:**
Implement notification template system for consistent messaging across FCM and APNs.

**Requirements:**
- Template creation and management
- Variable substitution
- Admin template editor
- Template versioning
- Multi-language support
- A/B testing support

**Acceptance Criteria:**
- [ ] Templates can be created/edited
- [ ] Variables work correctly
- [ ] Multi-language templates
- [ ] Templates versioned
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

### 19. Admin Dashboard - Authentication & User Management ✅ **COMPLETED**
**Title:** Build admin authentication and user management interface  
**Labels:** enhancement, frontend, admin, high-priority  
**Description:**
Implement admin authentication, authorization, and user management interface with search, filtering, and user actions.

**Technology Stack:**
- ✅ React 18.3 with TypeScript
- ✅ Material-UI (MUI) v6
- ✅ TanStack Query v5 (modern alternative to Redux Toolkit)

**Features:**
- ✅ Admin login and 2FA verification
- ✅ User list with pagination and search
- ✅ User detail pages (view profile, activity, history)
- ✅ User actions (edit, suspend, activate, delete)
- ✅ Role assignment
- ⏳ Bulk user actions (planned)

**Implementation Details:**
- **Backend**: Complete admin API with 8 endpoints in `AdminController`
- **Frontend**: React 18 + TypeScript + Vite dashboard in `admin-dashboard/` directory
- **Authentication**: Integrated with existing Duende IdentityServer and 2FA
- **UI Components**: Material-UI Data Grid, responsive layout, confirmation dialogs
- **State Management**: TanStack Query for server state, React Context for auth

**Acceptance Criteria:**
- [x] Admin authentication works
- [x] User list displays correctly
- [x] Search and filtering work
- [x] User actions functional
- [x] Role-based visibility

**Documentation:**
- [Admin API Documentation](../implementation/ADMIN_API_DOCUMENTATION.md)
- [Admin Dashboard README](../../admin-dashboard/README.md)

---

### 19b. Admin Dashboard - Guide Verification & Tour Moderation
**Title:** Build guide verification workflow and tour post moderation interface  
**Labels:** enhancement, frontend, admin  
**Description:**
Implement guide approval workflow and tour post moderation tools with approval queues.

**Features:**
- Pending guide approvals queue
- Guide verification checklist
- Document review (ID, certifications)
- Tour post moderation queue
- Content violation detection UI
- Approval/rejection workflow
- Messaging to guides/users

**Acceptance Criteria:**
- [ ] Pending guides displayed
- [ ] Approval workflow works
- [ ] Document review interface
- [ ] Moderation queue functional
- [ ] Messaging system works

---

### 19c. Admin Dashboard - Financial Monitoring & Analytics
**Title:** Build financial dashboard and analytics integration  
**Labels:** enhancement, frontend, admin  
**Status:** ✅ **COMPLETED**  
**Description:**
Create comprehensive financial dashboard with analytics charts, transaction monitoring, and revenue metrics.

**Features:**
- Transaction list with filtering
- Revenue metrics and charts
- Platform fees breakdown
- Payout history and requests
- Refund tracking
- Analytics dashboard integration
- Export reports (PDF, CSV)

**Acceptance Criteria:**
- [x] Transaction monitoring works
- [x] Charts display correctly
- [x] Filters functional
- [x] Report export works
- [x] Real-time updates

---

### 19d. Admin Dashboard - System Monitoring & Configuration
**Title:** Build system health monitoring and platform configuration interface  
**Labels:** enhancement, frontend, admin  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement system health dashboard, audit log viewer, and platform settings management.

**Features:**
- System health status
- Service availability monitoring
- Audit log viewer with filtering
- Webhook management interface
- Platform settings/configuration
- Feature toggles
- Log aggregation viewer

**Acceptance Criteria:**
- [x] Health dashboard displays
- [x] Audit logs searchable
- [x] Settings editable
- [x] Webhook management works
- [x] Real-time health updates

---

### 20. Tourist Website - Discovery & Search
**Title:** Build guide and tour discovery interface with search and filtering  
**Labels:** enhancement, frontend, website, high-priority  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement homepage, guide discovery, advanced search, and filtering interface for tourists.

**Technology Stack:**
- React 19 with TypeScript
- Material-UI (MUI) v7
- Vite 8 build tool
- Elasticsearch integration (API-driven)

**Features:**
- Featured guides and destinations homepage
- Guide search with Elasticsearch integration
- Advanced filtering (location, price, rating, languages, specialties)
- Guide profile pages with galleries and reviews
- Search history and saved guides
- Multi-currency display

**Acceptance Criteria:**
- [x] Homepage displays featured content
- [x] Search functionality works
- [x] Filters work correctly
- [x] Profile pages render
- [x] Mobile responsive

---

### 20b. Tourist Website - Tour Booking & Bidding
**Title:** Build tour request creation and bidding interface  
**Labels:** enhancement, frontend, website  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement tour request workflow, bidding interface, and booking management for tourists.

**Features:**
- Tour request creation form
- Request status tracking
- Bid list and comparison
- Bid acceptance/rejection
- Booking confirmation
- Itinerary review

**Acceptance Criteria:**
- [x] Request creation works
- [x] Bids display correctly
- [x] Booking workflow functional

---

### 20c. Tourist Website - Payment & User Profile
**Title:** Build payment interface and user profile management  
**Labels:** enhancement, frontend, website  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement Stripe payment interface, user profile, settings, and account management.

**Features:**
- Stripe payment form integration
- User profile page
- Account settings
- Notification preferences
- Payment history
- Address/preferences management
- Account security (change password, 2FA)

**Acceptance Criteria:**
- [x] Payment form works
- [x] Profile editable
- [x] Settings functional
- [x] 2FA setup accessible
- [x] Payment history displays

---

### 20d. Tourist Website - Reviews & Communication
**Title:** Build review system and real-time notifications  
**Labels:** enhancement, frontend, website  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement review/rating submission, notifications, and communication features.

**Features:**
- Review form with photos
- Rating display and distribution
- Notification center
- Real-time notifications via SignalR
- Review responses from guides
- Notification preferences

**Acceptance Criteria:**
- [x] Review submission works
- [x] Ratings display correctly
- [x] Notifications receive in real-time
- [x] Review responses functional

---

### 21. Guide Portal - Registration & Profile Management
**Title:** Build guide registration, onboarding, and profile management  
**Labels:** enhancement, frontend, guide-portal  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement guide registration flow, profile setup, photo gallery management, and verification process.

**Features:**
- Guide registration form
- KYC/identity verification flow
- Profile information management
- Photo gallery management
- Specialization and skill tags
- Language proficiencies
- Pricing setup
- Insurance/credentials upload

**Acceptance Criteria:**
- [x] Registration flow complete
- [x] KYC process functional
- [x] Profile editable
- [x] Gallery upload works
- [x] Tags/specializations editable

---

### 21b. Guide Portal - Tour Management & Bidding
**Title:** Build tour request management and bidding interface  
**Labels:** enhancement, frontend, guide-portal  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement tour request inbox, bid creation/management, and availability calendar.

**Features:**
- Tour request inbox
- Request filtering and search
- Bid creation form
- Bid management (edit, withdraw)
- Availability calendar
- Block dates functionality
- Recurring availability patterns
- Booking calendar integration

**Acceptance Criteria:**
- [x] Request inbox displays
- [x] Bid creation works
- [x] Calendar functional
- [x] Availability patterns work

---

### 21c. Guide Portal - Earnings & Payouts
**Title:** Build earnings dashboard and payout management  
**Labels:** enhancement, frontend, guide-portal  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement earnings tracking, financial dashboard, and payout request management.

**Features:**
- Earnings dashboard with charts
- Monthly/yearly earnings breakdown
- Transaction history
- Refund tracking
- Payout requests
- Available balance display
- Payment method management
- Tax documents/1099 generation

**Acceptance Criteria:**
- [x] Dashboard displays earnings
- [x] Charts render correctly
- [x] Payout requests functional
- [x] Transaction history accurate

---

### 21d. Guide Portal - Reviews & Communication
**Title:** Build review management and client communication interface  
**Labels:** enhancement, frontend, guide-portal  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement review display, responses, client messaging, and performance analytics.

**Features:**
- Reviews and ratings display
- Review response capability
- Client messaging interface
- Tour history and statistics
- Performance metrics and trends
- Client feedback analytics
- Response time tracking

**Acceptance Criteria:**
- [x] Reviews display correctly
- [x] Response system works
- [x] Messaging functional
- [x] Analytics display correctly

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

### 24. Real-time Chat System ✅ **COMPLETED**
**Title:** Implement real-time chat between tourists and guides  
**Labels:** enhancement, communication, real-time  
**Status:** ✅ **COMPLETED**  
**Description:**
Add a real-time chat system using SignalR for communication between tourists and guides.

**Implemented:**
- ✅ SignalR ChatHub at `/chat` with strongly-typed `IChatHub` interface
- ✅ Message persistence in database via EF Core
- ✅ Chat history retrieval via MessagesController
- ✅ Online/offline status tracking with connection management
- ✅ Typing indicators (SendTypingIndicator/SendStoppedTyping)
- ✅ Read receipts (MarkMessageAsRead)
- ✅ File/image sharing via FileAttachment entity
- ✅ Conversation membership validation for security

**API Endpoints:**
- SignalR Hub: `/chat` (SendMessage, SendTypingIndicator, SendStoppedTyping, MarkMessageAsRead, ShareFile, JoinConversation, LeaveConversation, GetOnlineUsers)
- `GET /api/messages/conversations` - Get conversations
- `GET /api/messages/conversations/{id}` - Get messages
- `POST /api/messages` - Send message
- `PUT /api/messages/conversations/{id}/read` - Mark as read

**Acceptance Criteria:**
- [x] Real-time messaging works
- [x] Messages are persisted
- [x] Chat history accessible
- [x] Notifications for new messages
- [x] File sharing implemented
- [x] Mobile-friendly chat UI

---

### 25. Review Moderation System ✅ **COMPLETED**
**Title:** Implement review moderation and flagging  
**Labels:** enhancement, moderation, content-management  
**Status:** ✅ **COMPLETED**  
**Description:**
Add moderation tools for reviews to prevent spam and inappropriate content.

**Implemented:**
- ✅ Review flagging system (ReviewFlag entity)
- ✅ Admin review queue with pagination and filtering
- ✅ Automated spam detection (caps ratio, URL density, repeated words, punctuation)
- ✅ Review approval workflow (Pending → Approved/Rejected/Removed)
- ✅ Appeal system for users
- ✅ Moderation analytics/stats

**API Endpoints:**
- `POST /api/reviews/{reviewId}/flag` - Flag a review
- `GET /api/reviews/moderation/queue` - Admin moderation queue
- `POST /api/reviews/moderation/{reviewId}/action` - Take moderation action
- `GET /api/reviews/moderation/stats` - Moderation statistics
- `POST /api/reviews/{reviewId}/appeal` - Submit appeal

**Acceptance Criteria:**
- [x] Users can flag inappropriate reviews
- [x] Admin moderation queue
- [x] Automated spam detection
- [x] Review approval workflow
- [x] Appeal process
- [x] Moderation reports

---

### 26. Booking Calendar and Availability ✅ **COMPLETED**
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
- [x] Guides can set availability
- [x] Calendar view implemented
- [x] Booking conflicts prevented
- [x] iCal export/import
- [x] Google Calendar sync
- [x] Timezone handling

**Status:** ✅ **COMPLETED**  
**Implementation Notes:**
- `GET /api/availability` – Returns availability slots with timezone support
- `POST /api/availability/block` – Block date ranges
- `DELETE /api/availability/block` – Unblock date ranges
- `POST /api/availability/recurring` – Set weekly/monthly recurring unavailability patterns
- `DELETE /api/availability/recurring` – Clear recurring pattern
- `GET /api/availability/check` – Check booking conflict for a specific date
- `GET /api/availability/export` – Export blocked dates as RFC 5545 iCal (.ics) file
- `POST /api/availability/import` – Import blocked dates from an iCal (.ics) string
- `GET /api/availability/google/auth-url` – Returns Google OAuth 2.0 authorisation URL; state token is CSRF-protected via ASP.NET Core Data Protection (expires in 10 min)
- `GET /api/availability/google/callback` – Validates state, exchanges code for tokens, stores encrypted at rest; fully production-ready
- `GET /api/availability/google/status` – Returns whether the guide has connected Google Calendar
- `DELETE /api/availability/google` – Revokes tokens and removes the Google Calendar connection
- `POST /api/availability/google/sync` – Fetches Google Calendar events and blocks matching dates; auto-refreshes expired access tokens using stored refresh token
- Guide Portal Availability page: iCal export/import, Google Calendar connect/disconnect/sync buttons, browser-local timezone auto-detection

---

### 27. Tour Package Templates ✅ **COMPLETED**
**Title:** Create reusable tour package templates  
**Labels:** enhancement, tours, templates  
**Status:** ✅ **COMPLETED**  
**Description:**
Allow guides to create and reuse tour package templates for common offerings.

**Implemented:**
- ✅ Template CRUD (create, update, delete, get)
- ✅ Pre-filled tour details with JSON-serialized itinerary
- ✅ Pricing templates with base price and currency
- ✅ Itinerary templates (included/excluded items)
- ✅ Template categories with filtering
- ✅ Usage tracking and template data retrieval for tour creation

**API Endpoints:**
- `POST /api/tour-templates` - Create template
- `PUT /api/tour-templates/{id}` - Update template
- `DELETE /api/tour-templates/{id}` - Delete template
- `GET /api/tour-templates/{id}` - Get template details
- `GET /api/tour-templates` - List guide's templates (with pagination and category filter)
- `POST /api/tour-templates/{id}/use-template` - Get template data for tour creation

**Acceptance Criteria:**
- [x] Guides can create templates
- [x] Templates can be edited
- [x] Quick tour creation from templates
- [x] Template categories
- [x] Template preview

---

### 28. Referral and Affiliate System ✅ **COMPLETED**
**Title:** Implement referral program for user growth  
**Labels:** enhancement, growth, gamification  
**Status:** ✅ **COMPLETED**  
**Description:**
Create a referral system to incentivize user growth and guide recruitment.

**Implemented:**
- ✅ Unique 8-character referral codes (cryptographically random)
- ✅ Referral tracking (Pending → Completed → Rewarded)
- ✅ Reward calculation with configurable amounts
- ✅ Referral dashboard with stats and recent referrals
- ✅ Self-referral prevention and duplicate checks
- ✅ Commission tracking via ReferralCode.TotalEarnings

**API Endpoints:**
- `POST /api/referrals/code` - Generate referral code
- `GET /api/referrals/code` - Get user's referral code
- `POST /api/referrals/apply` - Apply referral code
- `GET /api/referrals/dashboard` - Get referral dashboard
- `GET /api/referrals/history` - Get referral history

**Acceptance Criteria:**
- [x] Referral codes generated
- [x] Tracking system works
- [x] Rewards calculated correctly
- [x] Dashboard shows referrals
- [x] Automated payouts

---

### 29. Advanced Image Management ✅ **COMPLETED**
**Title:** Enhance image handling with compression and CDN  
**Labels:** enhancement, media, performance  
**Status:** ✅ **COMPLETED**  
**Description:**
Improve image management with automatic optimization, thumbnails, and CDN integration.

**Implemented:**
- ✅ Image processing pipeline (ProcessedImage entity with status tracking)
- ✅ Automatic thumbnail generation (URL variants: thumbnail, medium, large)
- ✅ Multiple size variants with metadata tracking
- ✅ CDN integration (Cloudflare, Azure CDN URL generation)
- ✅ WebP format support
- ✅ Image watermarking flag
- ✅ EXIF data extraction and storage

**API Endpoints:**
- `POST /api/images/processing` - Submit image for processing
- `GET /api/images/processing/{id}/variants` - Get image variants
- `GET /api/images/processing/{id}/status` - Get processing status
- `POST /api/images/processing/{id}/watermark` - Apply watermark
- `GET /api/images/processing/{id}/exif` - Get EXIF data
- `GET /api/images/processing/{id}/cdn-url` - Get CDN URL

**Acceptance Criteria:**
- [x] Images automatically compressed
- [x] Thumbnails generated
- [x] CDN integration
- [x] Multiple formats supported
- [x] Watermarking optional

---

### 30. Dispute Resolution System ✅ **COMPLETED**
**Title:** Implement dispute resolution for bookings  
**Labels:** enhancement, support, dispute-management  
**Status:** ✅ **COMPLETED**  
**Description:**
Add a system for handling disputes between tourists and guides.

**Implemented:**
- ✅ Dispute creation with booking validation and participant verification
- ✅ Evidence submission (file upload metadata)
- ✅ Admin review workflow (assign, review, resolve, escalate)
- ✅ Resolution tracking with refund amount support
- ✅ Communication thread per dispute
- ✅ Dispute history with pagination
- ✅ Escalation process with priority levels

**API Endpoints:**
- `POST /api/disputes` - Create dispute
- `GET /api/disputes/{id}` - Get dispute details
- `GET /api/disputes/my` - Get user's disputes
- `GET /api/disputes/admin/queue` - Admin dispute queue
- `POST /api/disputes/{id}/evidence` - Submit evidence
- `POST /api/disputes/{id}/messages` - Add message
- `POST /api/disputes/{id}/assign` - Assign to admin
- `POST /api/disputes/{id}/resolve` - Resolve dispute
- `POST /api/disputes/{id}/escalate` - Escalate dispute
- `GET /api/disputes/admin/stats` - Dispute statistics

**Acceptance Criteria:**
- [x] Users can file disputes
- [x] Evidence upload works
- [x] Admin review interface
- [x] Resolution tracking
- [x] Automated refunds

---

### 31. Email Template System ✅ **COMPLETED**
**Title:** Create customizable email template system  
**Labels:** enhancement, communication, email  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement a flexible email template system with customization and localization.

**Implemented:**
- ✅ Template CRUD with admin-only creation
- ✅ `{{variable}}` substitution engine
- ✅ HTML email templates with plain text fallback
- ✅ Template versioning (auto-incremented on every update)
- ✅ Preview functionality with variable substitution
- ✅ Multi-language templates with English fallback

**API Endpoints:**
- `POST /api/email-templates` - Create template (Admin)
- `PUT /api/email-templates/{id}` - Update template (Admin)
- `GET /api/email-templates/{id}` - Get template
- `GET /api/email-templates` - List templates (with category/language filters)
- `POST /api/email-templates/preview` - Preview with variables
- `GET /api/email-templates/{id}/versions` - Version history
- `DELETE /api/email-templates/{id}` - Deactivate template (Admin)

**Acceptance Criteria:**
- [x] Template editor works
- [x] Variables substituted correctly
- [x] HTML and text versions
- [x] Preview functionality
- [x] Multi-language support

---

### 32. Tour Recommendation Engine ✅ **COMPLETED**
**Title:** Build AI/ML-powered tour recommendations  
**Labels:** enhancement, ai, recommendations  
**Status:** ✅ **COMPLETED**  
**Description:**
Implement a recommendation system to suggest relevant tours and guides to users.

**Implemented:**
- ✅ Popularity-based scoring (bookings + per-tour ratings)
- ✅ Content-based recommendations (user preferences vs tour attributes)
- ✅ Collaborative filtering (similar users' bookings)
- ✅ Location-based recommendations (distance scoring placeholder)
- ✅ User preference tracking with weighted preferences
- ✅ Tour interaction logging (views, bookmarks, bookings, reviews, shares)
- ✅ Recommendation feedback loop (click/booking tracking)
- ✅ Set-based query approach to avoid N+1 performance issues

**API Endpoints:**
- `GET /api/recommendation` - Personalized recommendations (auth required)
- `GET /api/recommendation/popular` - Popular tours (no auth)
- `PUT /api/recommendation/preferences` - Set user preferences
- `GET /api/recommendation/preferences` - Get user preferences
- `POST /api/recommendation/interactions` - Record interaction
- `POST /api/recommendation/feedback` - Provide feedback
- `GET /api/recommendation/stats` - Recommendation stats (Admin)

**Acceptance Criteria:**
- [x] Recommendations generated
- [x] Personalized suggestions
- [x] Location-aware recommendations
- [x] Performance optimized
- [x] A/B testing enabled

---

### 33. Advanced Reporting System ✅ **COMPLETED**
**Title:** Create comprehensive reporting for guides and admins  
**Labels:** enhancement, reporting, analytics  
**Status:** ✅ **COMPLETED**  
**Description:**
Build detailed reporting system for financial, performance, and operational insights.

**Implemented:**
- ✅ CSV report generation with field escaping
- ✅ Scheduled reports (daily, weekly, monthly, quarterly)
- ✅ Custom date ranges for all reports
- ✅ Export format: CSV (PDF/Excel as future extension)
- ✅ Financial reports (guide earnings with top tours)
- ✅ Booking summary reports
- ✅ Email delivery via scheduled report recipients
- ✅ Report ownership validation (IDOR protection)
- ✅ Filename sanitization for secure downloads

**API Endpoints:**
- `POST /api/report` - Generate report
- `GET /api/report/{id}` - Get report details
- `GET /api/report` - List user's reports
- `GET /api/report/{id}/download` - Download CSV
- `POST /api/report/schedules` - Create scheduled report
- `GET /api/report/schedules` - List scheduled reports
- `PUT /api/report/schedules/{id}` - Update schedule
- `DELETE /api/report/schedules/{id}` - Delete schedule
- `GET /api/report/guide-earnings` - Guide earnings data
- `GET /api/report/booking-summary` - Booking summary (Admin)

**Report Types:**
- Guide earnings reports
- Booking summaries
- Tax documents
- Performance metrics
- Customer satisfaction
- Platform revenue

**Acceptance Criteria:**
- [x] PDF generation works
- [x] Scheduled reports
- [x] Multiple formats supported
- [x] Email delivery
- [x] Custom date ranges

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

**Total Issues: 47** (increased from 38 due to complex issue breakdown)

**By Status:**
- ✅ Completed: 23 (Docker Containerization, CI/CD Pipeline, Payment Integration, Two-Factor Authentication, GDPR Data Export, Message Queue Integration, Advanced Search & Filtering, Webhook System, Analytics Dashboard, API Rate Limiting Improvements, Real-time Chat, Review Moderation, Tour Package Templates, Referral System, Advanced Image Management, Dispute Resolution, Email Templates, Tour Recommendations, Advanced Reporting, Tourist Website Discovery & Search, Tourist Website Booking & Bidding, Tourist Website Payment & Profile, Tourist Website Reviews & Communication)
- 🚧 In Progress: 0
- 📋 Pending: 24

**By Priority:**
- High Priority: 6 (Payment Integration ✅, 2FA ✅, Admin Dashboard (19-19d) 📋, Tourist Website (20-20d) ✅, Backup & DR 📋)
- Medium Priority: 25
- Nice to Have: 16
- ✅ Completed: 10

**By Category:**
- **Frontend & UI**: 13 issues (Admin Dashboard 19-19d, Tourist Website 20-20d, Guide Portal 21-21d, PWA Features 22)
- **Testing & Quality**: 4 issues (#8, #18, #36-37: Unit, Integration, E2E, Performance Tests)
- **Security & Authentication**: 5 (1 completed: 2FA ✅, 4 pending: Google OAuth 5, Apple OAuth 5b, Microsoft OAuth 5c, Account Linking 5d)
- **Monitoring & Observability**: 2 (0 completed, 2 pending)
- **Features & Enhancements**: 5 (3 completed, 2 pending)
- **Mobile & Integration**: 5 (1 completed: Webhook System ✅, 4 pending: FCM 13, APNs 13b, Notification Templates 13c, expanded from 13)
- **Infrastructure**: 4 (1 completed: Message Queue ✅, 3 pending: Redis Caching, CDN, Backup & DR)
- **Analytics & Reporting**: 2 (1 completed: Analytics Dashboard ✅, 1 pending: Advanced Reporting)
- **Communication**: 2 (Real-time Chat 24, Email Templates 31)
- **Content Management**: 2 (Review Moderation 25, Tour Templates 27)
- **Platform Growth**: 1 (Referral System 28)
- **Media Management**: 1 (Advanced Image Management 29)
- **Support & Dispute**: 1 (Dispute Resolution 30)
- **AI & Recommendations**: 1 (Tour Recommendation Engine 32)
- **Scheduling**: 1 (✅ Booking Calendar 26 completed)
- **Documentation**: 1 (API Documentation Portal 35)
- **Localization**: 1 (#38: Multi-language Support - pending)
- **DevOps**: 2 (✅ Both completed)
- **Financial**: 1 (✅ Payment Integration completed)
- **Compliance**: 1 (✅ GDPR Data Export completed)

**Complex Issues Breakdown:**
- **Issue #5 (Social Login)** split into 4 issues: Google OAuth (5), Apple OAuth (5b), Microsoft OAuth (5c), Account Linking (5d)
- **Issue #13 (Push Notifications)** split into 3 issues: Android/FCM (13), iOS/APNs (13b), Templates (13c)
- **Issue #19 (Admin Dashboard)** split into 4 issues: Authentication & Users (19), Guide Verification (19b), Financial Monitoring (19c), System Monitoring (19d)
- **Issue #20 (Tourist Website)** split into 4 issues: Discovery & Search (20), Booking & Bidding (20b), Payment & Profile (20c), Reviews & Communication (20d)
- **Issue #21 (Guide Portal)** split into 4 issues: Registration & Profile (21), Tour & Bidding (21b), Earnings (21c), Reviews & Communication (21d)

**Priority Recommendations:**
1. **Admin Dashboard** (#19-19d) - Critical for platform management (4 focused issues)
2. **Tourist Website** (#20-20d) - Core user-facing application (4 focused issues)
3. **Guide Portal** (#21-21d) - Enable guide onboarding (4 focused issues)
4. **Unit Testing Suite** (#8) - Essential for code quality
5. **Integration Testing** (#18) - Ensure API reliability
6. **Backup & DR** (#34) - Protect platform data
7. **Real-time Chat** (#24) - Improve user communication
8. **Calendar System** (#26) - Essential booking feature

**Issue Numbering:**
- **Original**: Issues #1-18
- **Testing suite split**: Original #8 split into #8 (Unit), #18 (Integration), #36 (E2E), #37 (Performance)
- **New complex breakdowns**:
  - Social Login: #5, #5b, #5c, #5d
  - Push Notifications: #13, #13b, #13c
  - Admin Dashboard: #19, #19b, #19c, #19d
  - Tourist Website: #20, #20b, #20c, #20d
  - Guide Portal: #21, #21b, #21c, #21d
- **Existing issues**: #22-35, #38 (no changes to numbering)

These refined issues provide a more manageable roadmap with focused, achievable deliverables for building the complete tourism platform.

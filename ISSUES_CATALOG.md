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

### 8. API Testing Suite
**Title:** Create comprehensive API testing suite  
**Labels:** enhancement, testing, quality  
**Description:**
Develop a complete testing suite for the API including unit, integration, and end-to-end tests.

**Requirements:**
- Unit tests for services and controllers
- Integration tests for API endpoints
- Database integration tests
- Authentication/authorization tests
- Performance tests
- Test coverage reporting

**Acceptance Criteria:**
- [ ] Unit tests cover core logic
- [ ] Integration tests cover all endpoints
- [ ] Database tests are automated
- [ ] Auth/authz tests are comprehensive
- [ ] Performance benchmarks exist
- [ ] Test coverage is >80%

---

## 🚀 Features & Enhancements

### 9. API Rate Limiting Improvements
**Title:** Enhance rate limiting with tiered limits  
**Labels:** enhancement, performance, API  
**Description:**
Improve rate limiting to support different tiers based on user roles and subscription levels.

**Requirements:**
- Tiered rate limits (anonymous, authenticated, premium)
- Rate limit headers in responses
- Custom rate limits per endpoint
- Rate limit analytics
- Graceful degradation
- Rate limit exemptions for internal services

**Acceptance Criteria:**
- [ ] Different limits for different user tiers
- [ ] Rate limit headers are returned
- [ ] Custom limits work per endpoint
- [ ] Analytics track rate limit hits
- [ ] Exemptions work for internal calls
- [ ] Documentation updated

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
**Description:**
Integrate Elasticsearch for advanced search capabilities with fuzzy matching, filters, and facets.

**Requirements:**
- Elasticsearch integration
- Fuzzy search for guides and tours
- Multi-field search
- Filters (location, price, rating, etc.)
- Faceted search results
- Search analytics
- Search suggestions/autocomplete

**Acceptance Criteria:**
- [ ] Elasticsearch is integrated
- [ ] Fuzzy search works
- [ ] Multi-field search is implemented
- [ ] Filters work correctly
- [ ] Facets are returned
- [ ] Analytics track searches
- [ ] Autocomplete suggestions work

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
**Description:**
Create a webhook system to notify external systems of important events.

**Requirements:**
- Webhook registration API
- Event subscription management
- Payload signing for security
- Retry logic with exponential backoff
- Webhook history and logs
- Test webhook endpoint

**Acceptance Criteria:**
- [ ] Webhooks can be registered
- [ ] Events trigger webhooks
- [ ] Payloads are signed
- [ ] Retries work correctly
- [ ] History is logged
- [ ] Test endpoint is available

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
**Description:**
Build an analytics dashboard showing key metrics and trends.

**Requirements:**
- User registration trends
- Tour booking statistics
- Revenue metrics
- Guide performance metrics
- Popular destinations
- Conversion funnels
- Export capabilities

**Acceptance Criteria:**
- [ ] Dashboard displays key metrics
- [ ] Trends are visualized
- [ ] Filters work correctly
- [ ] Data can be exported
- [ ] Real-time updates
- [ ] Mobile-friendly

---

## 🌍 Localization

### 18. Multi-language Support
**Title:** Add internationalization (i18n) support  
**Labels:** enhancement, i18n, localization  
**Description:**
Implement multi-language support for API responses and error messages.

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

**Total Issues: 18**

**By Status:**
- ✅ Completed: 5 (Docker Containerization, CI/CD Pipeline, Payment Integration, Two-Factor Authentication, Message Queue Integration)
- 🚧 In Progress: 0
- 📋 Pending: 13

**By Priority:**
- High Priority: 0 (All completed: Payment Integration ✅, 2FA ✅)
- Medium Priority: 7
- Nice to Have: 5
- ✅ Completed: 5

**By Category:**
- Security & Authentication: 2 (1 completed: 2FA ✅, 1 pending: Social Login)
- Monitoring & Observability: 2
- Testing & Quality: 1
- Features & Enhancements: 5 (1 completed: GDPR Data Export ✅, 4 pending)
- Mobile & Integration: 2
- Infrastructure: 2 (1 completed: Message Queue ✅, 1 pending: Redis Caching)
- Analytics & Reporting: 1
- Localization: 1
- DevOps: 2 (✅ Both completed)
- Financial: 1 (✅ Payment Integration completed)
- Compliance: 1 (✅ GDPR Data Export completed)

These issues will be created in the GitHub repository to track progress on the UrGuide API platform.

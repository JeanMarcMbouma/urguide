# UrGuide Tourism Platform API

A comprehensive RESTful API for connecting travelers with local guides, enabling authentic and personalized travel experiences.

## 🌟 Overview

UrGuide is a modern tourism API platform built with .NET 10 LTS. The API allows developers to integrate guide profiles, tour requests, bidding systems, and user review functionality into their applications.

**Make yourself a tourism guide at your ease and pace.**

## 📚 Documentation

Comprehensive documentation is available in the [`docs/`](docs/) directory:

- **[Guides](docs/guides/)** - User and integration guides (2FA, Webhooks, etc.)
- **[Implementation](docs/implementation/)** - Technical architecture and implementation details
- **[CI/CD](docs/cicd/)** - Continuous integration and deployment documentation
- **[Security](docs/security/)** - Security audits and best practices
- **[Planning](docs/planning/)** - Feature requests and roadmap

📖 [View complete documentation index →](docs/README.md)

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 10.0 Web API (.NET 10 LTS)
- **Database**: Entity Framework Core 10.0 with SQL Server
- **Authentication**: Duende IdentityServer 7.4 with Duende.IdentityModel 8.0 (OAuth 2.0/OpenID Connect)
- **Two-Factor Authentication**: Custom TOTP implementation with QR code generation (QRCoder 1.4.3)
- **Passkey/WebAuthn**: Fido2.AspNet 3.0.1 for passwordless authentication
- **Real-time Communication**: SignalR for notifications
- **Message Queue**: MassTransit 8.3.4 with RabbitMQ for asynchronous processing
- **API Documentation**: Swagger/OpenAPI 3.0 (Swashbuckle.AspNetCore 10.1)
- **API Versioning**: Asp.Versioning.Mvc 8.1
- **Validation**: FluentValidation 12.1
- **Logging**: NLog 6.1 with structured logging
- **Rate Limiting**: AspNetCoreRateLimit + Custom Tiered Rate Limiting (Anonymous, Authenticated, Premium)
- **Email**: SendGrid integration
- **Payments**: Stripe.net 48.0 for payment processing
- **Health Checks**: ASP.NET Core Health Checks with SQL Server monitoring
- **.NET Aspire**: Service defaults for OpenTelemetry, resilience, and service discovery
- **OpenTelemetry**: Version 1.15.0 for distributed tracing, metrics, and logging

## ✅ Implemented Features

### 🚀 Platform Core
- [x] **RESTful API Architecture**: Modern Web API with versioning support (v1.0)
- [x] **API Versioning**: Support for URL segment, header, and query string versioning
- [x] **Health Checks**: `/health` and `/alive` endpoints for monitoring with .NET Aspire service defaults
- [x] **CORS Support**: Configurable cross-origin resource sharing for client apps
- [x] **Response Caching**: Built-in response and output caching
- [x] **Rate Limiting**: Configurable IP-based rate limiting
- [x] **Tiered Rate Limiting**: Advanced rate limiting with user tier support (Anonymous, Authenticated, Premium)
  - Different rate limits per user tier
  - Custom rate limits per endpoint
  - Rate limit headers in responses
  - Analytics tracking
  - Exemptions for internal services
  - Graceful degradation
- [x] **.NET Aspire Integration**: OpenTelemetry observability, HTTP resilience patterns, and service discovery

### 🔐 Authentication & User Management
- [x] User registration and profile creation via API
- [x] OAuth 2.0 and OpenID Connect authentication
- [x] JWT Bearer token authentication
- [x] User account deletion
- [x] Profile picture upload and management
- [x] Secure API endpoints with role-based authorization
- [x] **Two-Factor Authentication (2FA)**: TOTP-based 2FA with Google Authenticator support
- [x] **Passkey/WebAuthn Support**: Passwordless authentication using FIDO2 passkeys
- [x] **Backup Codes**: Recovery codes for 2FA account access

### �️ Admin Dashboard
- [x] **Modern Admin Interface**: React 19 + TypeScript + Vite with Material-UI v7
- [x] **Admin Authentication**: Secure login with 2FA integration and role-based access
- [x] **User Management**: Complete CRUD operations for user accounts
  - Paginated user list with advanced search and filtering
  - User detail pages with profile information and statistics
  - Account actions: Suspend, activate, and delete users
  - Role assignment and management
  - User activity audit trail with timestamps and IP tracking
- [x] **Admin APIs**: RESTful endpoints with `[Authorize(Roles = "Admin")]` protection
  - GET `/api/admin/users` - Paginated user list with search
  - GET `/api/admin/users/{id}` - User details
  - POST `/api/admin/users/{id}/suspend` - Suspend account
  - POST `/api/admin/users/{id}/activate` - Activate account
  - DELETE `/api/admin/users/{id}` - Delete user
  - PUT `/api/admin/users/roles` - Update user roles
  - GET `/api/admin/users/{id}/activity` - Activity log
  - GET `/api/admin/roles` - List available roles
- [x] **Real-time Updates**: TanStack Query for efficient server state management
- [x] **Responsive Design**: Mobile-friendly with MUI's responsive components
- [x] **Progressive Web App**: Service worker, web app manifest, offline support, install prompt, background sync, and FCM push notifications (all three frontend apps)

### 🧭 Guide Portal
- [x] **Standalone Guide Application**: React 19 + TypeScript + Vite with Material-UI v7
- [x] **Guide Registration & Profile**: Complete registration flow, profile editing, and KYC/identity verification
- [x] **Photo Gallery Management**: Create, upload, and manage tour photo galleries
- [x] **Tour Request Inbox**: Browse, filter, and respond to incoming tour requests
- [x] **Bid Management**: Create, edit, and withdraw bids on tour requests
- [x] **Availability Calendar**: Block dates, recurring patterns, iCal import/export, Google Calendar sync
- [x] **Earnings Dashboard**: Track earnings, view transaction history with Recharts-based trend charts
- [x] **Payout Management**: Request payouts, view payout history, manage payment methods
- [x] **Reviews & Ratings**: View reviews, respond to tourist feedback
- [x] **Client Messaging**: Database-persisted conversations with real-time chat UI
- [x] **Analytics Dashboard**: Performance metrics, tour statistics, and Recharts-based visualizations
  - Performance metrics (response rate, completion rate, cancellation rate, repeat client rate)
  - Tour statistics (total, completed, cancelled, average duration, top destinations)
  - Rating distribution bar chart and response time trend line chart
- [x] **Dashboard Activity Feed**: Live activity feed showing recent reviews, tour requests, and payouts
- [x] **Internationalization**: Full 5-language support (English, Spanish, French, German, Arabic)
- [x] **Docker Integration**: Production Dockerfile with Nginx and development Dockerfile.dev with hot-reload
- [x] **Guide Portal APIs**:
  - GET `/api/guide/dashboard` – Dashboard summary stats
  - GET `/api/guide/dashboard/activity` – Recent activity feed
  - GET `/api/guide/analytics/performance` – Performance metrics
  - GET `/api/guide/analytics/tour-stats` – Tour statistics
  - GET/POST `/api/messages/*` – Database-persisted messaging

### �👥 Guide System API
- [x] Guide registration with comprehensive questionnaire
- [x] Guide profile management with detailed information:
  - Date of birth
  - Driving license (with issuing year)
  - Gender and contact information
  - City of residence and address (show/hide to public)
  - Personal description and telephone number
  - Country information
- [x] Photo gallery creation for guides
- [x] Guide discovery and search endpoints

### 🎯 Tour & Bidding System API
- [x] Tour request creation and management
- [x] Bidding system for tour services
- [x] Bid modification (increase or decrease amounts)
- [x] Proposal acceptance and rejection
- [x] Tour participation and booking

### ⭐ Review & Feedback System API
- [x] 5-star rating system for tours and guides
- [x] Text-based feedback and reviews
- [x] Email notifications for new feedback
- [x] Comprehensive review display and management

### 🔔 Communication & Notifications
- [x] Real-time notification system via SignalR Hub (`/notify`)
- [x] In-app messaging and alerts
- [x] Email notification integration
- [x] Activity tracking and user history
- [x] **Message Queue Integration**: Asynchronous processing infrastructure
  - Email sending via queue
  - Image processing via queue
  - Notification dispatch via queue
  - Configurable retry policies
  - Dead letter queue handling
  - RabbitMQ health monitoring

### 🔍 Advanced Search & Analytics
- [x] **Elasticsearch Integration**: Advanced search capabilities with NEST 7.17.5 client
- [x] **Fuzzy Search**: Configurable fuzziness (AUTO, 0-2) for typo-tolerant searches
- [x] **Multi-field Search**: Query across text, description, tags, and location fields
- [x] **Advanced Filters**: Location, geo-distance, price range, rating, date ranges, tags, seat availability
- [x] **Faceted Search**: Aggregations by tags, locations, and rating distribution for drill-down navigation
- [x] **Autocomplete/Suggestions**: Search-as-you-type functionality with configurable result limits
- [x] **Search Analytics**: Track queries, results, timing, and user behavior
- [x] **Automatic Sync**: Real-time indexing on Post create/update/delete operations
- [x] **Bulk Re-indexing**: Admin endpoints for full data re-indexing
- [x] **Docker Compose**: Elasticsearch 8.11.0 service with single-node configuration

### 📊 Administrative Features
- [x] Complete action auditing system
- [x] User activity tracking
- [x] System monitoring and logging
- [x] **Analytics Dashboard**: Comprehensive admin analytics with key metrics and trends
  - User registration trends with growth rate calculation
  - Tour booking statistics and completion rates
  - Revenue metrics with platform fees and payouts
  - Guide performance metrics with top performers
  - Popular destinations by bookings and revenue
  - Conversion funnel tracking (requests → bids → bookings → completions)
  - Data export capabilities (JSON and CSV formats)
  - Flexible date range filtering and period grouping (hourly, daily, weekly, monthly, yearly)
  - Admin-only access with role-based authorization

### 💳 Payment & Financial System
- [x] Stripe payment integration for tour bookings
- [x] Payment intent creation with automatic payment methods
- [x] Platform fee calculation based on guide membership (2% for Basic, 5% for Premium)
- [x] Guide payout system with balance tracking
- [x] Full and partial refund processing
- [x] Transaction history tracking for users
- [x] Secure webhook handling for payment events
- [x] PCI-compliant payment data handling (no card data storage)
- [x] Multi-currency support
- [x] Payment status tracking (pending, processing, succeeded, failed, refunded)

### 🔗 Integration & Webhooks
- [x] **Webhook System**: External integration support with secure webhooks
  - Webhook registration with custom URLs
  - Support for 16 event types (payments, bookings, tours, users, reviews)
  - HMAC-SHA256 payload signing for security
  - Automatic retry with exponential backoff (5s, 15s, 45s, 135s)
  - Comprehensive delivery history and logging
  - Test webhook endpoint for validation
  - Fire-and-forget publishing for non-blocking operations

## 📋 API Roadmap & Future Features

### 💳 Payment & Financial System (Enhanced)
- [x] Platform currency system (coins) - Coin wallet with purchase, spend, and transaction tracking
- [x] Fund withdrawal to bank accounts - Bank withdrawal requests with status tracking
- [x] Automated payout scheduling - Configurable payout schedules (weekly/bi-weekly/monthly/on-demand)
- [x] Advanced financial reporting - Revenue, payouts, platform fees, and period-based reporting

### 🎁 Gamification & Rewards
- [x] User loyalty program with discounts - Tiered loyalty (Bronze/Silver/Gold/Platinum) with discount percentages
- [x] Badge system (Silver, Gold, Platinum) - Configurable badges with criteria and tier-based awards
- [x] Built-in lottery system for free tours - Lottery draws with cryptographically secure winner selection
- [x] Achievement tracking - Progress-based achievements with points rewards

### 📈 Premium Features
- [x] Guide membership tiers:
  - **Basic**: 2% platform fee, top 100 appearance, small group tours (<3 members)
  - **Premium**: 5% platform fee, top 10 local search, unlimited group size
- [x] Monthly/quarterly/yearly subscription plans - Configurable billing cycles with auto-renewal
- [x] Advanced search and visibility boosts - Search ranking, featured listing, top result, and highlighted profile boosts
- [x] Personalized advertising system - Targeted ads with budget management, impression/click tracking, and performance analytics

### 🔒 Security & Privacy
- [x] Two-factor authentication (2FA) - TOTP and Passkey/WebAuthn support implemented
- [x] Social login (Google/Apple/Microsoft) - OAuth integration via SocialAuthController
- [x] Data export functionality (GDPR compliance) - JSON and CSV export implemented
- [ ] Account freeze/temporary suspension
- [ ] Enhanced audit logging

### 📅 Advanced Scheduling
- [ ] Calendar integration and sharing
- [ ] Availability toggle system
- [ ] Automated scheduling tools
- [ ] Tour planning and routing

## 🛠️ Development Setup

### Prerequisites
- .NET 10 SDK
- SQL Server or SQL Server LocalDB
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/JeanMarcMbouma/urguide.git
   cd urguide
   ```

2. **Restore .NET packages**
   ```bash
   dotnet restore UrGuide.WebApp/UrGuide.WebApp.csproj
   ```

3. **Configure secrets (IMPORTANT)**
   
   **⚠️ SECURITY: Never commit secrets to source control!**
   
   For detailed instructions on managing secrets securely, see **[SECRETS_MANAGEMENT.md](./docs/security/SECRETS_MANAGEMENT.md)**
   
   **Quick Start for Docker:**
   ```bash
   # Copy the example file
   cp .env.example .env
   
   # Edit .env with your actual values
   # The .env file is in .gitignore and will NOT be committed
   nano .env  # or use your preferred editor
   ```
   
   **Quick Start for Windows (LocalDB):**
   ```bash
   cd UrGuide.WebApp
   
   # Set required secrets
   dotnet user-secrets set "IdentityServer:Clients:AdminDashboard:ClientSecret" "$(openssl rand -base64 32)"
   dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
   
   # Set optional API keys
   dotnet user-secrets set "IpStack:ApiKey" "your-ipstack-api-key"
   dotnet user-secrets set "SENDGRID_URGUIDE_API_KEY" "your-sendgrid-api-key"
   dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
   ```
   dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
   
   # Set Xamarin Client Secret (required for mobile app)
   dotnet user-secrets set "IdentityServer:Clients:Xamarin:ClientSecret" "your-secure-secret"
   ```
   
   📖 **For detailed secrets management instructions, see the [Security & Secrets Management](#-security--secrets-management) section below.**

4. **Update database connection strings**
   
   Edit `UrGuide.WebApp/appsettings.json` and update the connection strings:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=urguide_data;Trusted_Connection=True;MultipleActiveResultSets=true",
       "AuthConnection": "Server=(localdb)\\mssqllocaldb;Database=urguide_id4;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

5. **(Optional) Start RabbitMQ for message queue**
   
   If you want to use the message queue features, start RabbitMQ using Docker:
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management-alpine
   ```
   
   Or use the provided docker-compose file:
   ```bash
   docker-compose up -d rabbitmq
   ```
   
   RabbitMQ Management UI will be available at: `http://localhost:15672` (guest/guest)

6. **Run database migrations**
   
   The migrations will run automatically on application startup. Alternatively, run manually:
   ```bash
   dotnet ef database update --project UrGuide.WebApp
   ```

7. **Build the project**
   ```bash
   dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj
   ```

### Running the API

#### Development Mode
```bash
dotnet run --project UrGuide.WebApp
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`
- Health Check: `https://localhost:5001/health`

#### Using Docker Compose

Start all services (SQL Server, RabbitMQ, and the API):
```bash
docker-compose up -d
```

This will start:
- SQL Server on port `1433`
- RabbitMQ on ports `5672` (AMQP) and `15672` (Management UI)
- UrGuide API on port `5000`

Access services:
- API: `http://localhost:5000`
- API Health: `http://localhost:5000/health`
- RabbitMQ Management: `http://localhost:15672` (guest/guest)

Stop all services:
```bash
docker-compose down
```

#### Production Mode
```bash
dotnet run --project UrGuide.WebApp --configuration Release
```

### API Documentation

Once the application is running, you can access the interactive API documentation at:
- **Swagger UI**: `https://localhost:5001/swagger`

The Swagger UI provides:
- Complete API endpoint documentation
- Request/response schemas
- Interactive API testing
- OAuth 2.0 authentication flow

## 🔑 Authentication

The API uses **OAuth 2.0** and **OpenID Connect** for authentication via Duende IdentityServer 7.0.

### Authentication Flow

1. **Obtain Access Token**
   ```
   POST /connect/token
   Content-Type: application/x-www-form-urlencoded
   
   grant_type=password&
   username=user@example.com&
   password=YourPassword123!&
   client_id=UrGuide.WebAPI&
   scope=openid profile offline_access
   ```

2. **Use Access Token**
   ```
   GET /api/account/profile
   Authorization: Bearer {access_token}
   ```

### API Versioning

The API supports multiple versioning strategies:

1. **URL Segment** (recommended):
   ```
   GET /api/v1/tours
   ```

2. **Header**:
   ```
   GET /api/tours
   X-Api-Version: 1.0
   ```

3. **Query String**:
   ```
   GET /api/tours?api-version=1.0
   ```

### Rate Limiting

The API implements tiered rate limiting based on user authentication and subscription levels:

#### Rate Limit Tiers

1. **Anonymous Users** (not authenticated):
   - Global: 10 requests per minute
   - Sensitive endpoints (e.g., tour requests): 2 per hour
   - Payment endpoints: 1 per hour

2. **Authenticated Users** (basic subscription):
   - Global: 60 requests per minute
   - Tour requests: 10 per hour
   - Payment endpoints: 5 per hour

3. **Premium Users** (premium subscription):
   - Global: 300 requests per minute
   - Tour requests: 50 per hour
   - Payment endpoints: 20 per hour

#### Rate Limit Headers

Every API response includes rate limit information:

```http
X-RateLimit-Limit: 60
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1709123456
X-RateLimit-Tier: Authenticated
```

#### Rate Limit Response

When rate limit is exceeded, the API returns HTTP 429:

```json
{
  "error": "Rate limit exceeded",
  "message": "Too many requests. Please try again later.",
  "retryAfter": 60
}
```

#### Custom Rate Limits per Endpoint

Developers can apply custom rate limits using attributes:

```csharp
[RateLimit(5, "1m")]  // 5 requests per minute
[HttpPost("/login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{
    // Login logic
}
```

#### Rate Limit Exemptions

Certain endpoints and IP addresses can be exempt from rate limiting:
- Endpoints marked with `[RateLimitExempt]` attribute
- Internal service IP addresses (configurable in `appsettings.json` under `TieredRateLimit.Exemptions`)

**Note:** Health check endpoints (such as `/health` and `/alive`) are not automatically exempt. To exclude them from rate limiting:
1. Apply `[RateLimitExempt]` to the corresponding controller actions, OR
2. Add the endpoint paths to the `TieredRateLimit.Exemptions` array in configuration:
   ```json
   "TieredRateLimit": {
     "Exemptions": ["127.0.0.1", "::1", "/health", "/alive"]
   }
   ```

#### Configuration

Rate limits are configured in `appsettings.json` under `TieredRateLimit`:

```json
{
  "TieredRateLimit": {
    "Enabled": true,
    "EnableAnalytics": true,
    "Policies": {
      "Anonymous": { "Limit": 10, "Period": "1m" },
      "Authenticated": { "Limit": 60, "Period": "1m" },
      "Premium": { "Limit": 300, "Period": "1m" }
    },
    "EndpointPolicies": {
      "POST:/tour-requests": {
        "Anonymous": { "Limit": 2, "Period": "1h" },
        "Authenticated": { "Limit": 10, "Period": "1h" },
        "Premium": { "Limit": 50, "Period": "1h" }
      }
    },
    "Exemptions": ["127.0.0.1", "::1"]
  }
}
```

#### Rate Limit Analytics

The system tracks rate limit usage and violations for monitoring and optimization:
- Request counts per tier
- Violation tracking
- User-specific statistics
- Endpoint usage patterns

**Note:** Legacy IP-based rate limiting (`IpRateLimitPolicies`) has been disabled to avoid conflicts with the new tiered rate limiting system. The tiered system provides more granular control and better user experience.

### Message Queue Integration

The API implements asynchronous message processing using **MassTransit 8.3.4** with **RabbitMQ** for improved scalability and reliability.

#### Features

- **Email Sending**: Emails are queued for asynchronous delivery
- **Image Processing**: Image resizing and optimization happen in the background
- **Notifications**: Real-time notifications are dispatched via queue
- **Retry Policies**: Automatic retry with exponential backoff for failed messages
- **Dead Letter Queue**: Failed messages are moved to dead letter queue for investigation
- **Health Monitoring**: RabbitMQ connection health is monitored via `/health` endpoint

#### Configuration

Configure RabbitMQ connection in `appsettings.json`:

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  },
  "MessageQueue": {
    "UseQueuedServices": false,
    "EnableMonitoring": true
  }
}
```

#### Enable Queued Services

By default, message queue infrastructure is configured but services still run synchronously. To enable async processing, set `MessageQueue:UseQueuedServices` to `true`:

```json
{
  "MessageQueue": {
    "UseQueuedServices": true
  }
}
```

This will:
- Queue all email sending operations
- Queue image processing operations
- Queue notification dispatch operations

#### Retry Policies

The message queue implements automatic retry with the following intervals:

- **Email Queue**: 5s, 15s, 30s
- **Image Processing Queue**: 10s, 30s, 60s
- **Notification Queue**: 5s, 15s, 30s

After all retries are exhausted, failed messages are moved to the dead letter queue for manual investigation.

#### Monitoring

RabbitMQ health is included in the health check endpoint:

```bash
curl https://localhost:5001/health
```

Response includes RabbitMQ status:
```json
{
  "status": "Healthy",
  "entries": {
    "auth-db": { "status": "Healthy" },
    "data-db": { "status": "Healthy" },
    "rabbitmq": { 
      "status": "Healthy",
      "description": "RabbitMQ is connected"
    }
  }
}
```

#### Message Consumers

Three dedicated consumers process messages:

1. **SendEmailConsumer**: Processes email sending via SendGrid
2. **ProcessImageConsumer**: Handles image resizing and optimization
3. **SendNotificationConsumer**: Dispatches real-time notifications via SignalR



### Health Checks

Monitor the API and database health:
```bash
curl https://localhost:5001/health
```

Response:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "auth-db": {
      "status": "Healthy",
      "duration": "00:00:00.0567890"
    },
    "data-db": {
      "status": "Healthy",
      "duration": "00:00:00.0666777"
    }
  }
}
```

## 📡 API Endpoints

### Main Endpoint Categories

- **`/api/account`** - User account management, registration, profile
- **`/api/post`** - Tour posts and guide profiles
- **`/api/bid`** - Bidding system for tours
- **`/api/feedback`** - Reviews and ratings
- **`/api/galleries`** - Photo galleries
- **`/api/tourrequest`** - Tour request management
- **`/api/payment`** - Payment processing for tour bookings
- **`/api/payout`** - Guide payout management
- **`/api/refund`** - Refund request processing
- **`/api/dataexport`** - GDPR-compliant user data export
- **`/api/analytics`** - Admin analytics dashboard and reporting
- **`/api/webhook`** - Stripe webhook events
- **`/api/webhook-management`** - Webhook registration and management for external integrations
- **`/api/lookup`** - Reference data (countries, cities, etc.)
- **`/api/notification`** - Notification management
- **`/api/activity`** - User activity tracking
- **`/notify`** - SignalR Hub for real-time notifications

### Webhook Management

The API provides a webhook system for external integrations, allowing third-party applications to receive real-time notifications of important events:

- **`POST /api/webhook-management`** - Register a new webhook
- **`GET /api/webhook-management`** - List all registered webhooks
- **`GET /api/webhook-management/{id}`** - Get webhook details
- **`PUT /api/webhook-management/{id}`** - Update a webhook
- **`DELETE /api/webhook-management/{id}`** - Delete a webhook
- **`GET /api/webhook-management/{id}/deliveries`** - Get webhook delivery history
- **`POST /api/webhook-management/test`** - Test a webhook with sample payload

**Features:**
- Support for 16 event types: payments, bookings, tours, users, and reviews
- HMAC-SHA256 payload signing for security verification
- Automatic retry with exponential backoff (5 attempts)
- Comprehensive delivery history and logging
- Test endpoint for webhook validation

**Documentation:**
- [Webhook Integration Guide](docs/guides/WEBHOOK_INTEGRATION_GUIDE.md) - Complete API reference and security details
- [Integration Examples](docs/guides/WEBHOOK_INTEGRATION_EXAMPLES.md) - Code examples for common integration scenarios

### Analytics Dashboard (Admin Only)

The API provides comprehensive analytics endpoints for administrators to monitor platform performance and user behavior:

- **`GET /api/analytics/dashboard`** - Get complete dashboard with all metrics
- **`GET /api/analytics/user-registration-trends`** - User registration trends and growth
- **`GET /api/analytics/tour-booking-statistics`** - Tour booking statistics and completion rates
- **`GET /api/analytics/revenue-metrics`** - Revenue, fees, payouts, and refunds
- **`GET /api/analytics/guide-performance`** - Guide performance metrics and top performers
- **`GET /api/analytics/popular-destinations`** - Most popular destinations by bookings
- **`GET /api/analytics/conversion-funnel`** - Conversion funnel from requests to completions
- **`GET /api/analytics/export`** - Export analytics data in JSON or CSV format

**Features:**
- Flexible date range filtering (start/end dates)
- Period grouping (hourly, daily, weekly, monthly, yearly)
- Top N filtering for rankings (top guides, destinations)
- JSON and CSV export formats
- Admin-only access with role-based authorization
- Real-time data aggregation from production database

**Query Parameters:**
- `startDate` - Start date for analytics (optional, defaults to 6 months ago)
- `endDate` - End date for analytics (optional, defaults to now)
- `period` - Period grouping: Hourly, Daily, Weekly, Monthly, Yearly (default: Daily)
- `topN` - Number of top items to return for rankings (default: 10)
- `format` - Export format: json or csv (default: json)

**Dashboard Metrics:**
- User registration trends with growth rates
- Booking statistics by status (pending, completed, cancelled)
- Revenue breakdown (total, fees, payouts, refunds)
- Guide performance (total tours, bookings, revenue, ratings)
- Popular destinations by bookings and revenue
- Conversion funnel analysis (requests → bids → bookings → completions)

### GDPR Data Export

The API provides GDPR-compliant data export functionality allowing users to export all their personal data:

- **`POST /api/dataexport/request`** - Request a data export (JSON or CSV format)
- **`GET /api/dataexport/status/{requestId}`** - Check the status of an export request
- **`GET /api/dataexport/download/{token}`** - Download exported data using secure token
- **`DELETE /api/dataexport/{requestId}`** - Cancel a pending export request

**Features:**
- Export includes profile, activity history, reviews, tours, bids, and more
- Supports JSON (single file) and CSV (ZIP with multiple files) formats
- Secure token-based downloads with 7-day expiration
- Email notification when export is ready
- Background processing for large exports

For complete endpoint documentation, refer to the Swagger UI at `/swagger`.

## 🐳 Docker Support

The UrGuide platform supports full Docker containerization with orchestrated services.

### Quick Start with Docker Compose

```bash
# 1. Start all services (API + Admin Dashboard + Databases)
docker-compose up -d

# 2. Wait for health checks (30-60 seconds)
docker-compose ps

# 3. Access applications:
# - API: http://localhost:5000
# - Admin Dashboard: http://localhost:3001
# - Swagger: http://localhost:5000/swagger
# - RabbitMQ UI: http://localhost:15672
```

**📖 See [DOCKER_QUICKSTART.md](DOCKER_QUICKSTART.md) for comprehensive Docker guide**

### Services Included

The `docker-compose.yml` orchestrates **8 containers**:
- **SQL Server 2022** - Database with persistent volumes
- **RabbitMQ 3** - Message broker with management UI
- **Elasticsearch 8.11** - Advanced search engine
- **UrGuide API** - .NET 10 backend application
- **Admin Dashboard** - React 19 + Nginx frontend (port 3001)
- **Guide Portal** - React 19 + Nginx frontend (port 3002)
- **Tourist Website** - React 19 + Nginx frontend (port 3003)
- **Seq** - Structured log aggregation and search (port 5341/8080)

### Environment Configuration

**⚠️ SECURITY:** See **[SECRETS_MANAGEMENT.md](./docs/security/SECRETS_MANAGEMENT.md)** for complete security guidelines.

Create a `.env` file for secrets and configuration:
```bash
cp .env.example .env
# Edit .env with your API keys and passwords
```

**Required secrets** in `.env`:
- `SQL_SA_PASSWORD` - Strong database password
- `ADMIN_DASHBOARD_CLIENT_SECRET` - OAuth2 client secret for admin dashboard
- `ADMIN_PASSWORD` - Default admin user password

**Optional but recommended**:
- `IPSTACK_API_KEY` - For IP geolocation
- `SENDGRID_API_KEY` - For email notifications
- `STRIPE_SECRET_KEY` - For payment processing
- `JWT__KEY` - Custom JWT secret (auto-generated if not set)  
- `IPSTACK_API_KEY` - IPStack API key (optional)
- `SENDGRID_API_KEY` - SendGrid API key (optional)
**Required Environment Variables:**
- `SQL_SA_PASSWORD` - SQL Server SA password (minimum 8 characters, complexity required)
- `IPSTACK_API_KEY` - IPStack API key for geolocation
- `SENDGRID_API_KEY` - SendGrid API key for emails
- `XAMARIN_CLIENT_SECRET` - Client secret for mobile app
- `ADMIN_DASHBOARD_CLIENT_SECRET` - Client secret for admin dashboard (generate with `openssl rand -base64 32`)
- `SEED_ADMIN_ENABLED` - Enable automatic admin user provisioning (true/false)
- `ADMIN_EMAIL`, `ADMIN_PASSWORD`, `ADMIN_FIRST_NAME`, `ADMIN_LAST_NAME` - Admin credentials
- `JWT__KEY` - Secure random key for JWT token generation (optional, auto-generated if not set)
- `JWT__EXPIRESINHOUHS` - JWT token expiration in hours (default: 8)

**IMPORTANT**: Never commit `.env` files to version control (already in .gitignore).
**SECURITY**: Generate all secrets using `openssl rand -base64 32` or similar secure random generators.
See the [Security & Secrets Management](#-security--secrets-management) section for detailed guidance.

### JWT Token Authentication

The admin dashboard uses JWT Bearer tokens for API authentication:

- **Login Endpoint**: `POST /api/auth/login` returns a JWT token
- **Token Storage**: Stored in localStorage on the client
- **Token Claims**: Includes user ID, email, username, and roles
- **Token Validation**: Symmetric key-based validation (HS256 algorithm)
- **Configuration**: Set `JWT__KEY` environment variable for production (use `openssl rand -base64 32`)
- **Expiration**: Configurable via `JWT__EXPIRESINHOUHS` (default: 8 hours)

**Development**: If no JWT key is configured, a development key is auto-generated (insecure for production).
**Production**: Always configure a secure, randomly generated JWT key via environment variables or Azure Key Vault.

### Development Mode with Hot Reload

```bash
# Start with development overrides (auto-reload on code changes)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

Admin dashboard dev proxy target can be configured with `VITE_API_TARGET`.
- Docker dev default: `http://api:80` (set in `docker-compose.override.yml`)
- Local dev default: `http://localhost:5000`

### Build Images Manually

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build admin-dashboard

# Build without cache (clean build)
docker-compose build --no-cache
```

### Individual Service Management

```bash
# Start only API + Databases
docker-compose up -d sqlserver rabbitmq elasticsearch api

# Start only Admin Dashboard (requires API)
docker-compose up -d api admin-dashboard

# Restart specific service
docker-compose restart admin-dashboard
```

### Docker Compose Configuration

The `docker-compose.yml` includes:
- **SQL Server 2022**: Database server with persistent volumes
- **RabbitMQ 3**: Message broker with management UI
- **Elasticsearch 8.11**: Advanced search engine
- **UrGuide API**: The main .NET 10 application with health checks
- **Admin Dashboard**: React 19 admin interface with Nginx
- **Guide Portal**: React 19 guide interface with Nginx
- **Tourist Website**: React 19 tourist interface with Nginx
- **Seq**: Structured log aggregation (Datalust Seq)
- **Automatic migrations**: Database migrations run on startup
- **Volume mounts**: Persistent storage for uploads and logs

### Development with Docker

Use `docker-compose.override.yml` for development with hot reload:

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

This enables:
- File watching and hot reload
- Source code mounted as volumes
- Development environment settings

### Admin Dashboard in Docker

The admin dashboard runs as a separate container with Nginx:

```bash
# Start admin dashboard container
docker-compose up -d admin-dashboard

# Access at http://localhost:3001
# API proxy configured internally
```

Features:
- Multi-stage build (Node.js build → Nginx runtime)
- Production-optimized React bundle
- Automatic API proxy to backend
- Health checks and auto-restart
- ~25MB final image size

See [admin-dashboard/DOCKER.md](admin-dashboard/DOCKER.md) for comprehensive Docker documentation.

## 🧪 Testing

### Manual Testing

Use the Swagger UI at `/swagger` for interactive API testing.

### Automated Testing

Test projects will be automatically detected and run by the CI/CD pipeline. Add test projects following this naming convention:
- `UrGuide.*.Tests.csproj` for unit tests
- `UrGuide.*.IntegrationTests.csproj` for integration tests

_(Comprehensive API testing suite coming soon - see issues catalog)_

## � Admin Dashboard Development

The admin dashboard is a separate React 19 + TypeScript + Vite application located in the `admin-dashboard/` directory.

### Prerequisites
- Node.js 18+ and npm
- UrGuide API running on `https://localhost:5001`

### Setup and Run

```bash
# Navigate to admin dashboard
cd admin-dashboard

# Install dependencies (already done if you followed setup)
npm install

# Start development server
npm run dev

# Access dashboard at http://localhost:3001
```

### Available Commands

- `npm run dev` - Start development server with hot reload
- `npm run build` - Build production bundle (output to `dist/`)
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint for code quality

### Features

- **User Management**: Search, filter, suspend, activate, delete users
- **Role Management**: Assign and update user roles (User, Guide, Admin)
- **Activity Monitoring**: View user activity logs with timestamps and IP tracking
- **Admin Authentication**: Secure login with 2FA integration
- **Responsive Design**: Mobile-friendly Material-UI interface
- **Real-time Updates**: TanStack Query for efficient data synchronization

### Technology Stack

- **React 19.2** - Modern React with hooks
- **TypeScript 5.9** - Type-safe development
- **Vite 8.0** - Fast build tool and dev server
- **Material-UI v7** - Enterprise-ready component library
- **MUI X Data Grid** - Advanced table with pagination, sorting, filtering
- **TanStack Query v5** - Server state management
- **React Router v7** - Client-side routing
- **Axios** - HTTP client for API calls

### API Integration

The dashboard communicates with the backend admin API endpoints:

```typescript
// Example: Get users with search
GET /api/admin/users?PageNumber=1&PageSize=20&Term=john

// Example: Suspend user
POST /api/admin/users/{userId}/suspend?durationDays=7

// Example: Update roles
PUT /api/admin/users/roles
Body: { "userId": "...", "roles": ["User", "Guide"] }
```

See [Admin API Documentation](docs/implementation/ADMIN_API_DOCUMENTATION.md) for complete API reference.

### Production Build

```bash
cd admin-dashboard
npm run build

# Output will be in admin-dashboard/dist/
# Can be deployed to any static hosting or embedded in ASP.NET Core wwwroot
```

## �🚀 Deployment

### CI/CD Pipeline

The project includes automated GitHub Actions workflows:

#### 1. **Main CI/CD Pipeline** (`.github/workflows/dotnet-ci.yml`)
- Triggers on push/PR to `main` or `develop` branches
- Multi-stage pipeline:
  - **Build & Test**: Compiles the application and runs tests
  - **Code Quality**: CodeQL security scanning
  - **Dependency Check**: Scans for vulnerable packages
  - **Docker Build**: Builds and validates Docker images
  - **Notifications**: Reports pipeline status

#### 2. **Docker Publishing** (`.github/workflows/docker-publish.yml`)
- Publishes Docker images to GitHub Container Registry
- Automatic tagging with version numbers and git SHA
- Triggered on pushes to `main` or version tags

#### 3. **Migration Validation** (`.github/workflows/migration-validation.yml`)
- Validates database migrations against SQL Server
- Generates idempotent migration scripts
- Triggers on changes to migration files

### Manual Deployment

#### Prerequisites
- .NET 10 Runtime
- SQL Server
- HTTPS certificate (Let's Encrypt or other)

#### Using Docker (Recommended)

1. **Pull the image**:
   ```bash
   docker pull ghcr.io/jeanmarcmbouma/urguide:latest
   ```

2. **Run with environment variables**:
   ```bash
   docker run -d \
     -p 80:80 \
     -e ConnectionStrings__DefaultConnection="Server=your-server;Database=urguide_data;..." \
     -e ConnectionStrings__AuthConnection="Server=your-server;Database=urguide_id4;..." \
     ghcr.io/jeanmarcmbouma/urguide:latest
   ```

#### Traditional Deployment

1. **Publish the API**:
   ```bash
   dotnet publish UrGuide.WebApp/UrGuide.WebApp.csproj -c Release -o ./publish
   ```

2. **Configure for Production**:
   - Update `appsettings.Production.json` with production connection strings
   - Configure HTTPS certificates
   - Set up proper CORS origins
   - Configure rate limiting for production traffic
   - Update SendGrid API keys for email
   - Configure Azure SignalR Service (optional, for scale-out)

3. **Apply database migrations**:
   ```bash
   dotnet ef database update --project UrGuide.WebApp
   ```

### Monitoring

- **Health Checks**: `/health` endpoint monitors database connectivity
- **Logs**: Structured logging with NLog (available in `/app/logs` in Docker)
- **Metrics**: Built-in ASP.NET Core metrics and health checks

## 🔒 Security & Secrets Management

UrGuide implements robust security practices for managing sensitive configuration:

### Secrets Management Strategy

- **Local Development**: Use .NET User Secrets (never commit secrets to Git)
- **Docker/Containers**: Use environment variables from `.env` files
- **Production**: Azure Key Vault or Kubernetes Secrets for cloud deployments
- **CI/CD**: GitHub Secrets for automated workflows

### Quick Setup (Local Development)

```bash
cd UrGuide.WebApp

# Configure required secrets
dotnet user-secrets set "IpStack:ApiKey" "your-api-key"
dotnet user-secrets set "SENDGRID_URGUIDE_API_KEY" "your-api-key"
dotnet user-secrets set "IdentityServer:Clients:Xamarin:ClientSecret" "your-secret"
```

### Security Best Practices

✅ **DO**:
- Use User Secrets for local development
- Rotate secrets regularly (every 30-90 days)
- Use strong, randomly generated secrets
- Store production secrets in Azure Key Vault
- Use different secrets per environment

❌ **DON'T**:
- Commit secrets to source control
- Share secrets via email or chat
- Use default/example passwords in production
- Reuse secrets across environments

📖 **For complete security guidance, see the [Security Audit Report](docs/security/SECURITY_AUDIT_REPORT.md)**

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Issues & Feature Requests

See the [Issues Catalog](https://github.com/JeanMarcMbouma/urguide/issues) for:
- Payment integration
- Enhanced security features
- ~~API rate limiting improvements~~ ✅ **Implemented**
- Monitoring and observability
- API testing suite
- ~~Docker containerization~~ ✅ **Implemented**
- ~~CI/CD pipeline~~ ✅ **Implemented**
- API client SDK generation

## 📞 Support

For questions and support:
- Create an issue in the GitHub repository
- Check the API documentation at `/swagger`
- Review the codebase and inline documentation

---

**Built with ❤️ for the tourism community**


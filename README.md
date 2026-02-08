# UrGuide Tourism Platform API

A comprehensive RESTful API for connecting travelers with local guides, enabling authentic and personalized travel experiences.

## 🌟 Overview

UrGuide is a modern tourism API platform built with .NET 10 LTS. The API allows developers to integrate guide profiles, tour requests, bidding systems, and user review functionality into their applications.

**Make yourself a tourism guide at your ease and pace.**

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 10.0 Web API (.NET 10 LTS)
- **Database**: Entity Framework Core 10.0 with SQL Server
- **Authentication**: Duende IdentityServer 7.4 with Duende.IdentityModel 8.0 (OAuth 2.0/OpenID Connect)
- **Two-Factor Authentication**: Custom TOTP implementation with QR code generation (QRCoder 1.4.3)
- **Passkey/WebAuthn**: Fido2.AspNet 3.0.1 for passwordless authentication
- **Real-time Communication**: SignalR for notifications
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

### 👥 Guide System API
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

### 📊 Administrative Features
- [x] Complete action auditing system
- [x] User activity tracking
- [x] System monitoring and logging

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

## 📋 API Roadmap & Future Features

### 💳 Payment & Financial System (Enhanced)
- [ ] Platform currency system (coins)
- [ ] Fund withdrawal to bank accounts
- [ ] Automated payout scheduling
- [ ] Advanced financial reporting

### 🎁 Gamification & Rewards
- [ ] User loyalty program with discounts
- [ ] Badge system (Silver, Gold, Platinum)
- [ ] Built-in lottery system for free tours
- [ ] Achievement tracking

### 📈 Premium Features
- [ ] Guide membership tiers:
  - **Basic**: 2% platform fee, top 100 appearance, small group tours (<3 members)
  - **Premium**: 5% platform fee, top 10 local search, unlimited group size
- [ ] Monthly/quarterly/yearly subscription plans
- [ ] Advanced search and visibility boosts
- [ ] Personalized advertising system

### 🔒 Security & Privacy
- [ ] Two-factor authentication (2FA)
- [ ] Social login (Google/Apple/Microsoft)
- [ ] Data export functionality (GDPR compliance)
- [ ] Account freeze/temporary suspension
- [ ] Enhanced audit logging

### 📅 Advanced Scheduling
- [ ] Calendar integration and sharing
- [ ] Availability toggle system
- [ ] Automated scheduling tools
- [ ] Tour planning and routing

### 💰 Payment Integration
- [ ] Google Pay/Apple Pay support
- [ ] PayPal integration
- [ ] Multiple payment method support
- [ ] Secure payment processing

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
   
   **Never commit secrets to source control!** Use .NET User Secrets for local development:
   
   ```bash
   cd UrGuide.WebApp
   
   # Set IPStack API Key (optional, for IP geolocation)
   dotnet user-secrets set "IpStack:ApiKey" "your-ipstack-api-key"
   
   # Set SendGrid API Key (optional, for email notifications)
   dotnet user-secrets set "SENDGRID_URGUIDE_API_KEY" "your-sendgrid-api-key"
   
   # Set Stripe API Keys (required for payment processing)
   dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
   dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
   dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
   
   # Set Xamarin Client Secret (required for mobile app)
   dotnet user-secrets set "IdentityServer:Clients:Xamarin:ClientSecret" "your-secure-secret"
   ```
   
   📖 **For detailed secrets management instructions, see [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md)**

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

5. **Run database migrations**
   
   The migrations will run automatically on application startup. Alternatively, run manually:
   ```bash
   dotnet ef database update --project UrGuide.WebApp
   ```

6. **Build the project**
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
- Internal service IP addresses (configurable in `appsettings.json`)

**Note:** Health check endpoints (such as `/health` and `/alive`) are not automatically exempt. To exclude them from rate limiting, apply `[RateLimitExempt]` to the corresponding actions or add them to the exemptions list in configuration.

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
- **`/api/webhook`** - Stripe webhook events
- **`/api/lookup`** - Reference data (countries, cities, etc.)
- **`/api/notification`** - Notification management
- **`/api/activity`** - User activity tracking
- **`/notify`** - SignalR Hub for real-time notifications

For complete endpoint documentation, refer to the Swagger UI at `/swagger`.

## 🐳 Docker Support

The UrGuide API now supports Docker containerization for easy development and deployment.

### Quick Start with Docker Compose

1. **Configure environment variables**:
   ```bash
   cp .env.example .env
   # Edit .env and set all required secrets (see SECRETS_MANAGEMENT.md)
   ```
   
   **Required secrets** in `.env`:
   - `SQL_SA_PASSWORD` - Strong database password
   - `IPSTACK_API_KEY` - IPStack API key (optional)
   - `SENDGRID_API_KEY` - SendGrid API key (optional)
   - `XAMARIN_CLIENT_SECRET` - Client secret for mobile app

2. **Start all services** (API + SQL Server):
   ```bash
   docker-compose up -d
   ```

3. **Access the API**:
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

4. **View logs**:
   ```bash
   docker-compose logs -f api
   ```

5. **Stop all services**:
   ```bash
   docker-compose down
   ```

### Security Note

**IMPORTANT**: The `.env.example` file contains default/placeholder values for demonstration purposes. Always:
- Copy `.env.example` to `.env` 
- Change **ALL** secrets to strong, unique values
- Never commit `.env` files to version control (already in .gitignore)
- See [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) for detailed security guidance

### Build Docker Image Manually

```bash
docker build -t urguide-api:latest .
```

### Run Docker Container

```bash
docker run -d \
  -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  --name urguide-api \
  urguide-api:latest
```

### Docker Compose Configuration

The `docker-compose.yml` includes:
- **SQL Server 2022**: Database server with persistent volumes
- **UrGuide API**: The main application with health checks
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

## 🧪 Testing

### Manual Testing

Use the Swagger UI at `/swagger` for interactive API testing.

### Automated Testing

Test projects will be automatically detected and run by the CI/CD pipeline. Add test projects following this naming convention:
- `UrGuide.*.Tests.csproj` for unit tests
- `UrGuide.*.IntegrationTests.csproj` for integration tests

_(Comprehensive API testing suite coming soon - see issues catalog)_

## 🚀 Deployment

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

📖 **For complete documentation, see [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md)**

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

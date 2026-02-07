# UrGuide Tourism Platform API

A comprehensive RESTful API for connecting travelers with local guides, enabling authentic and personalized travel experiences.

## 🌟 Overview

UrGuide is a modern tourism API platform built with .NET 10 LTS. The API allows developers to integrate guide profiles, tour requests, bidding systems, and user review functionality into their applications.

**Make yourself a tourism guide at your ease and pace.**

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 10.0 Web API (.NET 10 LTS)
- **Database**: Entity Framework Core 10.0 with SQL Server
- **Authentication**: Duende IdentityServer 7.4 with Duende.IdentityModel 8.0 (OAuth 2.0/OpenID Connect)
- **Real-time Communication**: SignalR for notifications
- **API Documentation**: Swagger/OpenAPI 3.0 (Swashbuckle.AspNetCore 10.1)
- **API Versioning**: Asp.Versioning.Mvc 8.1
- **Validation**: FluentValidation 12.1
- **Logging**: NLog 6.1 with structured logging
- **Rate Limiting**: AspNetCoreRateLimit
- **Email**: SendGrid integration
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
- [x] **.NET Aspire Integration**: OpenTelemetry observability, HTTP resilience patterns, and service discovery

### 🔐 Authentication & User Management
- [x] User registration and profile creation via API
- [x] OAuth 2.0 and OpenID Connect authentication
- [x] JWT Bearer token authentication
- [x] User account deletion
- [x] Profile picture upload and management
- [x] Secure API endpoints with role-based authorization

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

## 📋 API Roadmap & Future Features

### 💳 Payment & Financial System
- [ ] Integrated payment processing
- [ ] Platform currency system (coins)
- [ ] Fund withdrawal to bank accounts
- [ ] Refund request system
- [ ] Transaction history tracking

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

3. **Update database connection strings**
   
   Edit `UrGuide.WebApp/appsettings.json` and update the connection strings:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=urguide_data;Trusted_Connection=True;MultipleActiveResultSets=true",
       "AuthConnection": "Server=(localdb)\\mssqllocaldb;Database=urguide_id4;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run database migrations**
   
   The migrations will run automatically on application startup. Alternatively, run manually:
   ```bash
   dotnet ef database update --project UrGuide.WebApp
   ```

5. **Build the project**
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

The API implements IP-based rate limiting:
- Default: 100 requests per 15 minutes per IP address
- Configure in `appsettings.json` under `IpRateLimiting`

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
   # Edit .env and set SQL_SA_PASSWORD to a strong password
   ```

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

**IMPORTANT**: The `.env.example` file contains a default password for demonstration purposes. Always:
- Copy `.env.example` to `.env` 
- Change `SQL_SA_PASSWORD` to a strong, unique password
- Never commit `.env` files to version control (already in .gitignore)

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
- API rate limiting improvements
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

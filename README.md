# UrGuide Tourism Platform

A comprehensive tourism web application that connects travelers with local guides, enabling authentic and personalized travel experiences.

## 🌟 Overview

UrGuide is a modern tourism platform built with .NET 8 backend, React 18 SPA frontend, and Xamarin mobile apps. The platform allows users to create guide profiles, request tours, bid on services, and share experiences through a comprehensive review system.

**Make yourself a tourism guide at your ease and pace.**

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 8.0 with Entity Framework Core 8.0
- **Frontend**: React 18 SPA with Material-UI v5 (@mui/material)
- **Mobile**: UrGuide.MAUI (.NET 8.0 MAUI) - *Migration in progress from Xamarin.Forms*
- **Database**: SQL Server with LocalDB
- **Authentication**: Duende IdentityServer 7.0
- **Real-time Communication**: SignalR
- **Orchestration**: .NET Aspire for cloud-native deployment
- **Observability**: OpenTelemetry, health checks, and distributed tracing
- **API Documentation**: Swagger/OpenAPI

## ✅ Implemented Features

### 🚀 Platform Modernization
- [x] **Framework Migration**: Upgraded to .NET 8 and React 18 for modern compatibility
- [x] **Security Enhancements**: Migrated from deprecated IdentityServer4 to Duende IdentityServer 7.0
- [x] **Cloud-Native Architecture**: Integrated .NET Aspire for deployment orchestration
- [x] **Observability**: Added OpenTelemetry, health checks, and distributed tracing
- [x] **Frontend Modernization**: Upgraded to Material-UI v5, React Router v6, Bootstrap v5
- [x] **Node.js Compatibility**: Resolved dependency conflicts for Node 20+ support
- [x] **Mobile Migration**: Started migration from Xamarin.Forms to .NET MAUI (foundation complete)

### 🔐 Authentication & User Management
- [x] User registration and profile creation
- [x] IdentityServer4 authentication system
- [x] User account deletion
- [x] Profile picture upload and management

### 👥 Guide System
- [x] Guide registration with comprehensive questionnaire
- [x] Guide profile with detailed information:
  - Date of birth
  - Driving license (with issuing year)
  - Gender and contact information
  - City of residence and address (show/hide to public)
  - Personal description and telephone number
  - Country information
- [x] Photo gallery creation for guides
- [x] Guide discovery and search functionality

### 🎯 Tour & Bidding System
- [x] Tour request creation and management
- [x] Bidding system for tour services
- [x] Bid modification (increase or decrease amounts)
- [x] Proposal acceptance and rejection
- [x] Tour participation and booking

### ⭐ Review & Feedback System
- [x] 5-star rating system for tours and guides
- [x] Text-based feedback and reviews
- [x] Email notifications for new feedback
- [x] Comprehensive review display and management

### 🔔 Communication & Notifications
- [x] Real-time notification system
- [x] In-app messaging and alerts
- [x] Email notification integration
- [x] Activity tracking and user history

### 📊 Administrative Features
- [x] Complete action auditing system
- [x] User activity tracking
- [x] System monitoring and logging

### 🏗️ Architecture & Infrastructure
- [x] .NET Aspire orchestration for cloud-native deployment
- [x] Service defaults with health checks and observability
- [x] OpenTelemetry integration for distributed tracing
- [x] Modern hosting model with .NET 8 minimal APIs
- [x] Modernized authentication with Duende IdentityServer

## 🚧 Work in Progress

Currently no features are actively in development. See the roadmap below for planned enhancements.

## 📋 Roadmap & Future Features

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
- [ ] Barcode authentication integration

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
- .NET 8 SDK
- Node.js 18+ (with npm)
- SQL Server LocalDB

### Installation
1. Clone the repository
2. Restore .NET packages: `dotnet restore`
3. Install frontend dependencies: `cd UrGuide.WebApp/ClientApp && npm install --legacy-peer-deps`
4. Build the project: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`

### Running the Application

**Option 1: With .NET Aspire Orchestration (Recommended)**
```bash
# Run the complete application with orchestration
dotnet run --project UrGuide.AppHost
```

**Option 2: Traditional Development**
```bash
# Backend API only
dotnet run --project UrGuide.WebApp

# Frontend development server (in a separate terminal)
cd UrGuide.WebApp/ClientApp
NODE_OPTIONS="--openssl-legacy-provider" npm start
```

### Build Commands
```bash
# Build backend (.NET 8)
dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj

# Install frontend dependencies (works with Node 20+)
cd UrGuide.WebApp/ClientApp
npm install --legacy-peer-deps

# Run frontend tests
NODE_OPTIONS="--openssl-legacy-provider" npm test
```

## 📱 Mobile Apps

Native mobile applications are available for both Android and iOS platforms. The project is currently migrating from Xamarin.Forms to .NET MAUI for improved performance and modern .NET 8 support.

### Current Status
- **Legacy**: Xamarin.Forms apps (Mobile/UrGuide.Mobile/)
- **New**: .NET MAUI foundation (UrGuide.MAUI/) - Service layer migration complete
- **Next Steps**: UI layer migration to MAUI

See [MAUI_MIGRATION.md](./MAUI_MIGRATION.md) for detailed migration documentation.

## 🤝 Contributing

We welcome contributions! Please see our contributing guidelines and feel free to submit issues and pull requests.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

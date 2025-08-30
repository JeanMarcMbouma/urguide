# UrGuide Tourism Platform

A comprehensive tourism web application that connects travelers with local guides, enabling authentic and personalized travel experiences.

## 🌟 Overview

UrGuide is a modern tourism platform built with .NET Core 3.1 backend, React SPA frontend, and Xamarin mobile apps. The platform allows users to create guide profiles, request tours, bid on services, and share experiences through a comprehensive review system.

**Make yourself a tourism guide at your ease and pace.**

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 3.1 with Entity Framework Core
- **Frontend**: React SPA with Material-UI components
- **Mobile**: Xamarin.Forms (Android & iOS)
- **Database**: SQL Server with LocalDB
- **Authentication**: IdentityServer4
- **Real-time Communication**: SignalR
- **API Documentation**: Swagger/OpenAPI

## ✅ Implemented Features

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
- .NET Core 3.1 SDK
- Node.js (with npm)
- SQL Server LocalDB

### Installation
1. Clone the repository
2. Restore .NET packages: `dotnet restore`
3. Install frontend dependencies: `cd UrGuide.WebApp/ClientApp && npm install --legacy-peer-deps`
4. Build the project: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`

### Running the Application
- Backend: `dotnet run --project UrGuide.WebApp`
- Frontend development: `cd UrGuide.WebApp/ClientApp && npm start`

## 📱 Mobile Apps

Native mobile applications are available for both Android and iOS platforms, built with Xamarin.Forms and sharing the same backend API.

## 🤝 Contributing

We welcome contributions! Please see our contributing guidelines and feel free to submit issues and pull requests.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

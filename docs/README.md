# UrGuide Documentation

Welcome to the UrGuide Tourism Platform documentation! This directory contains comprehensive guides, implementation details, and technical documentation for the platform.

## 📚 Documentation Structure

### 📖 Guides
User and integration guides for working with UrGuide features.

- [**Two-Factor Authentication & Passkey Guide**](guides/2FA_PASSKEY_GUIDE.md) - How to implement and use 2FA and WebAuthn passkeys
- [**Webhook Integration Guide**](guides/WEBHOOK_INTEGRATION_GUIDE.md) - Integration guide for webhook event publishing
- [**Webhook Integration Examples**](guides/WEBHOOK_INTEGRATION_EXAMPLES.md) - Practical examples for webhook integration
- [**Push Notifications Guide**](guides/PUSH_NOTIFICATIONS_GUIDE.md) - FCM push notification setup, cache strategies, and background sync

### 🔧 Implementation
Technical implementation summaries and architecture documentation.

- [**Admin API Documentation**](implementation/ADMIN_API_DOCUMENTATION.md) - Admin dashboard backend API reference and implementation guide
- [**Admin User Provisioning**](implementation/ADMIN_USER_PROVISIONING.md) - Admin account seeding and provisioning utilities
- [**JWT Authentication**](implementation/JWT_AUTHENTICATION.md) - JWT token generation and validation for admin dashboard API
- [**Issue #19 Implementation Summary**](implementation/ISSUE_19_IMPLEMENTATION_SUMMARY.md) - Complete implementation details for admin dashboard
- [**Message Queue Implementation**](implementation/MESSAGE_QUEUE_IMPLEMENTATION.md) - MassTransit + RabbitMQ async processing
- [**Webhook Implementation Summary**](implementation/WEBHOOK_IMPLEMENTATION_SUMMARY.md) - Webhook system architecture and features
- [**Refactoring Summary**](implementation/REFACTORING_SUMMARY.md) - Platform transformation to API-only architecture
- [**Social Login Integration**](implementation/SOCIAL_LOGIN_INTEGRATION.md) - Google, Apple, Microsoft OAuth social login and account linking
- [**Push Notification Templates**](implementation/PUSH_NOTIFICATION_TEMPLATES.md) - Reusable templates with variable substitution, versioning, multi-language and A/B testing
- [**Email Template Engine**](implementation/EMAIL_TEMPLATE_ENGINE.md) - Proprietary database-backed email templating with multi-language support, admin dashboard CRUD, and SMTP delivery

### 🚀 CI/CD
Continuous Integration and Deployment documentation.

- [**CI/CD Documentation**](cicd/CICD_DOCUMENTATION.md) - Complete CI/CD pipeline reference
- [**CI/CD Summary**](cicd/CI_CD_SUMMARY.md) - Quick overview of CI/CD implementation

### 🔒 Security
Security documentation, audits, and best practices.

- [**Secrets Management Guide**](security/SECRETS_MANAGEMENT.md) - Complete guide for managing credentials, API keys, and secrets securely
- [**Security Audit Report**](security/SECURITY_AUDIT_REPORT.md) - Secrets management implementation audit

### 🧪 Testing
Testing infrastructure and guidelines.

- **Unit Tests** (`tests/UrGuide.UnitTests/`) - 69 unit tests with xUnit, BbQ.MockLite, FluentAssertions, Coverlet
- **Integration Tests** (`tests/UrGuide.IntegrationTests/`) - 89 controller integration tests
- **E2E Tests** (`tests/UrGuide.E2ETests/`) - Playwright-based end-to-end test infrastructure
- **Performance Tests** (`tests/UrGuide.PerformanceTests/`) - BenchmarkDotNet micro-benchmarks

Run tests:
```bash
dotnet test tests/UrGuide.UnitTests/          # Unit tests (69 tests)
dotnet test tests/UrGuide.IntegrationTests/   # Integration tests (89 tests)
dotnet run -c Release --project tests/UrGuide.PerformanceTests/  # Benchmarks
```

### 📋 Planning
Project planning, roadmaps, and issue tracking.

- [**Issues Catalog**](planning/ISSUES_CATALOG.md) - Outstanding feature requests and improvements

### 🌐 API Documentation Portal
Interactive API documentation with examples, tutorials, and SDK guides.

- [**API Portal Overview**](api-portal/README.md) - Portal index, interactive explorer, and quick start
- [**Authentication Guide**](api-portal/AUTHENTICATION_GUIDE.md) - JWT, OAuth2, social login, and 2FA authentication
- [**Code Examples**](api-portal/CODE_EXAMPLES.md) - Ready-to-use examples in C#, JavaScript, Python, and cURL
- [**Tutorials**](api-portal/TUTORIALS.md) - Step-by-step integration tutorials for common scenarios
- [**SDK Documentation**](api-portal/SDK_DOCUMENTATION.md) - SDK generation, client libraries, and mobile integration
- [**API Versioning**](api-portal/API_VERSIONING.md) - Versioning strategy, supported versions, and migration guidance
- [**Changelog**](api-portal/CHANGELOG.md) - API changes, new features, and deprecation notices

## 🚀 Quick Start

New to UrGuide? Start with these resources:

1. [**Main README**](../README.md) - Project overview, features, and getting started
2. [**Developer Instructions**](../.github/copilot-instructions.md) - Development setup and workflow
3. [**Webhook Integration Guide**](guides/WEBHOOK_INTEGRATION_GUIDE.md) - Integrate with UrGuide events

## 🔗 Related Resources

- [Main Project README](../README.md) - Complete project overview and API documentation
- [GitHub Repository](https://github.com/JeanMarcMbouma/urguide) - Source code and issues
- [API Documentation](../README.md#-api-endpoints) - RESTful API reference

## 📝 Contributing to Documentation

When adding new documentation:

1. Place files in the appropriate subdirectory
2. Update this index with a link to your new document
3. Follow the existing naming conventions (UPPERCASE_WITH_UNDERSCORES.md)
4. Include clear headings and table of contents for longer documents
5. Cross-reference related documents when appropriate

## 📞 Support

For questions or issues:
- Create an issue in the [GitHub repository](https://github.com/JeanMarcMbouma/urguide/issues)
- Check the [Issues Catalog](planning/ISSUES_CATALOG.md) for known issues and feature requests

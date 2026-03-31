# UrGuide API Documentation Portal

Welcome to the UrGuide API Documentation Portal. This portal provides everything you need to integrate with the UrGuide Tourism Platform API, including interactive exploration, code examples, authentication guides, and tutorials.

## 🌐 Interactive API Explorer

The UrGuide API ships with an interactive Swagger UI that lets you explore and test all endpoints directly in your browser:

| Resource | URL | Description |
|----------|-----|-------------|
| **Swagger UI** | `/swagger/index.html` | Interactive API explorer with OAuth2 and JWT authentication |
| **OpenAPI Spec** | `/swagger/v1/swagger.json` | Machine-readable OpenAPI 3.0 specification |
| **Health Check** | `/health` | Service health status |
| **Liveness Probe** | `/alive` | Basic liveness check |

### Using Swagger UI

1. Navigate to `/swagger/index.html` on your running UrGuide instance
2. Click **Authorize** to authenticate:
   - **OAuth2**: Use the Authorization Code flow with PKCE
   - **Bearer**: Paste a JWT token obtained from `POST /api/auth/token`
3. Expand any endpoint group to see available operations
4. Click **Try it out** to execute requests interactively
5. Review response bodies, headers, and status codes

The Swagger UI supports filtering, deep linking, and displays request duration for performance analysis.

## 📚 Documentation Index

| Document | Description |
|----------|-------------|
| [**Authentication Guide**](AUTHENTICATION_GUIDE.md) | How to authenticate with the API using JWT, OAuth2, and social providers |
| [**Code Examples**](CODE_EXAMPLES.md) | Ready-to-use code samples in C#, JavaScript, Python, and cURL |
| [**Tutorials**](TUTORIALS.md) | Step-by-step guides for common integration scenarios |
| [**SDK Documentation**](SDK_DOCUMENTATION.md) | SDK generation, usage, and client library reference |
| [**API Versioning**](API_VERSIONING.md) | API versioning strategy, supported versions, and migration guidance |
| [**Changelog**](CHANGELOG.md) | API changes, new features, deprecations, and migration notes |

## 🏗️ API Overview

The UrGuide API is a RESTful HTTP API built with ASP.NET Core. It connects travelers with local tour guides worldwide, providing endpoints for:

- **Authentication & Users** — Registration, login, JWT tokens, 2FA, passkeys, social login
- **Tours & Posts** — Create, search, browse, and manage tour listings
- **Tour Requests & Bidding** — Request tours, place bids, accept/reject proposals
- **Booking & Payments** — Reserve seats, process payments via Stripe, manage refunds
- **Search & Discovery** — Elasticsearch-powered full-text search with facets and autocomplete
- **Recommendations** — Personalized tour recommendations based on preferences and behavior
- **Messaging & Notifications** — Real-time chat via SignalR, push notifications, email templates
- **Guide Management** — Guide profiles, verification, availability calendars, earnings
- **Reviews & Moderation** — Leave reviews, moderate content, manage disputes
- **Administration** — User management, analytics, system monitoring, audit logs

### Base URL

```
https://your-urguide-instance.com/api/
```

### Response Format

All API responses follow a standard envelope pattern:

**Success Response:**
```json
{
  "value": { ... },
  "isError": false
}
```

**Error Response:**
```json
{
  "errors": ["Validation failed: field is required"],
  "isError": true
}
```

### Common HTTP Status Codes

| Code | Meaning |
|------|---------|
| `200 OK` | Request succeeded |
| `201 Created` | Resource created |
| `400 Bad Request` | Validation or business logic error |
| `401 Unauthorized` | Missing or invalid authentication |
| `403 Forbidden` | Insufficient permissions |
| `404 Not Found` | Resource not found |
| `429 Too Many Requests` | Rate limit exceeded |
| `500 Internal Server Error` | Unexpected server error |

### Content Type

All requests and responses use `application/json` unless otherwise specified. File upload endpoints accept `multipart/form-data`.

### Localization

The API supports localized error messages and responses. Set the `Accept-Language` header to one of the supported locales:

- `en` — English (default)
- `fr` — French
- `es` — Spanish
- `de` — German
- `ar` — Arabic

## 🔑 Quick Authentication

Get started immediately with a JWT token:

```bash
# 1. Obtain a JWT token
curl -X POST https://your-instance.com/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"userName": "your@email.com", "password": "YourPassword123!"}'

# 2. Use the token in subsequent requests
curl https://your-instance.com/api/posts/owned \
  -H "Authorization: Bearer <your-access-token>"
```

See the [Authentication Guide](AUTHENTICATION_GUIDE.md) for complete details on all authentication methods.

## 🔗 Related Resources

- [Main Project README](../../README.md) — Project overview and features
- [Webhook Integration Guide](../guides/WEBHOOK_INTEGRATION_GUIDE.md) — Subscribe to platform events
- [Push Notifications Guide](../guides/PUSH_NOTIFICATIONS_GUIDE.md) — Mobile push notification setup
- [2FA & Passkey Guide](../guides/2FA_PASSKEY_GUIDE.md) — Two-factor authentication implementation
- [GitHub Repository](https://github.com/JeanMarcMbouma/urguide) — Source code and issue tracking

# UrGuide API Versioning

This document explains the UrGuide API versioning strategy, supported versions, and guidance for migrating between versions.

## Table of Contents

- [Versioning Strategy](#versioning-strategy)
- [Specifying the API Version](#specifying-the-api-version)
- [Supported Versions](#supported-versions)
- [Version Lifecycle](#version-lifecycle)
- [Response Headers](#response-headers)
- [Migration Guide](#migration-guide)

---

## Versioning Strategy

The UrGuide API uses **semantic versioning** with a major.minor scheme (e.g., `1.0`). The platform supports multiple methods for specifying the desired API version, giving clients flexibility in how they integrate.

### Key Principles

- **Default version**: When no version is specified, the API defaults to **v1.0**
- **Backward compatibility**: Minor version bumps (e.g., 1.0 → 1.1) are backward-compatible
- **Breaking changes**: Major version bumps (e.g., 1.0 → 2.0) may include breaking changes
- **Version reporting**: Every API response includes version information in headers

---

## Specifying the API Version

The UrGuide API supports three methods for specifying the API version. All three methods can be used interchangeably and are combined using `ApiVersionReader.Combine()`.

### 1. URL Segment (Recommended)

Include the version in the URL path:

```
GET /api/v1/posts/last10
POST /api/v1/search/posts
```

This is the recommended method as it makes the version explicit in every request and is easy to observe in logs.

### 2. Request Header

Use the `X-Api-Version` header:

```http
GET /api/posts/last10
X-Api-Version: 1.0
```

Useful when you want to keep URLs clean or when switching versions via configuration.

### 3. Query String Parameter

Append `api-version` as a query parameter:

```
GET /api/posts/last10?api-version=1.0
POST /api/search/posts?api-version=1.0
```

Convenient for quick testing in browsers and tools like Postman.

### Priority Order

When multiple methods are used simultaneously, the API resolves conflicts in this order:

1. URL segment (highest priority)
2. Query string parameter
3. Request header (lowest priority)

---

## Supported Versions

| Version | Status | Release Date | Notes |
|---------|--------|-------------|-------|
| **v1.0** | ✅ Current | Initial release | Full API with all endpoints |

### Version 1.0 Endpoint Groups

| Group | Route Prefix | Endpoints | Description |
|-------|-------------|-----------|-------------|
| Authentication | `/api/auth/` | 5 | JWT tokens, login, refresh, 2FA |
| Account | `/api/account/` | 12 | Registration, profile, password |
| Posts/Tours | `/api/posts/` | 16 | Tour CRUD, search, reservations |
| Tour Requests | `/api/tour-requests/` | 7 | Request creation and management |
| Bidding | `/api/bid/` | 4 | Bid placement and management |
| Payments | `/api/payment/` | 5 | Payment processing and history |
| Search | `/api/search/` | 6 | Elasticsearch-powered search |
| Recommendations | `/api/recommendation/` | 7 | Personalized tour recommendations |
| Notifications | `/api/notification/` | varies | Push and in-app notifications |
| Messaging | `/api/messages/` | varies | Direct messaging |
| Webhooks | `/api/webhook-management/` | varies | Webhook subscription management |
| Admin | `/api/admin/` | varies | User management, audit logs |
| Two-Factor | `/api/twofactor/` | 6 | 2FA setup and management |
| Passkeys | `/api/passkey/` | 4 | WebAuthn/FIDO2 authentication |
| Availability | `/api/availability/` | varies | Guide calendar management |
| Reviews | `/api/review-moderation/` | varies | Review management |
| Disputes | `/api/dispute/` | 10 | Dispute resolution |
| Financial | `/api/financial/` | varies | Earnings and payouts |
| Data Export | `/api/data-export/` | 4 | GDPR data export |

---

## Version Lifecycle

Each API version goes through these stages:

```
Preview → Current → Deprecated → Retired
```

| Stage | Duration | Support Level |
|-------|----------|---------------|
| **Preview** | Variable | Beta — may change without notice |
| **Current** | Until next major version | Full support, bug fixes, enhancements |
| **Deprecated** | 12 months | Security fixes only, migration recommended |
| **Retired** | N/A | Endpoints return `410 Gone` |

### Deprecation Policy

When an API version is deprecated:

1. **Announcement**: Deprecation notice published in the [Changelog](CHANGELOG.md) at least 6 months before retirement
2. **Response headers**: Deprecated endpoints include `Sunset` and `Deprecation` headers
3. **Migration guide**: A migration guide is published for transitioning to the new version
4. **Grace period**: Deprecated versions remain functional for at least 12 months

---

## Response Headers

Every API response includes version-related headers:

### Standard Headers

| Header | Description | Example |
|--------|-------------|---------|
| `api-supported-versions` | All supported API versions | `1.0` |
| `api-deprecated-versions` | Deprecated versions (if any) | — |

### Deprecation Headers (when applicable)

| Header | Description | Example |
|--------|-------------|---------|
| `Sunset` | Date when the version will be retired | `Sat, 01 Jan 2028 00:00:00 GMT` |
| `Deprecation` | Whether the version is deprecated | `true` |
| `Link` | URL to migration documentation | `<https://docs.urguide.com/migration>; rel="successor-version"` |

### Example Response with Version Headers

```http
HTTP/1.1 200 OK
Content-Type: application/json
api-supported-versions: 1.0
X-Api-Version: 1.0

{
  "value": [...],
  "isError": false
}
```

---

## Migration Guide

### General Migration Steps

When migrating between API versions:

1. **Review the changelog** — Check [CHANGELOG.md](CHANGELOG.md) for breaking changes
2. **Update the version** — Change the version in your API client configuration
3. **Update models** — Adjust request/response models for any schema changes
4. **Regenerate SDKs** — If using auto-generated SDKs, regenerate from the new spec
5. **Test thoroughly** — Run your integration tests against the new version
6. **Deploy gradually** — Use feature flags or canary deployments when switching versions

### Backward-Compatible Changes (Non-Breaking)

These changes may be introduced in any version without a version bump:

- Adding new optional fields to request/response models
- Adding new API endpoints
- Adding new enum values
- Adding new HTTP headers
- Relaxing validation constraints (e.g., increasing max length)

### Breaking Changes (Require Major Version Bump)

These changes trigger a new major version:

- Removing or renaming endpoints
- Removing or renaming fields in request/response models
- Changing field data types
- Changing authentication requirements
- Modifying error response format
- Tightening validation constraints

### Version Negotiation Example

```csharp
// C# - Configure API version in HTTP client
var client = new HttpClient();
client.BaseAddress = new Uri("https://your-instance.com");
client.DefaultRequestHeaders.Add("X-Api-Version", "1.0");

// Or use URL segment versioning
var response = await client.GetAsync("/api/v1/posts/last10");
```

```javascript
// JavaScript - Configure API version
const API_VERSION = '1.0';

async function apiRequest(path, options = {}) {
  const headers = {
    'X-Api-Version': API_VERSION,
    ...options.headers
  };
  
  return fetch(`https://your-instance.com/api/v${API_VERSION}${path}`, {
    ...options,
    headers
  });
}
```

```python
# Python - Configure API version
API_VERSION = "1.0"

session = requests.Session()
session.headers["X-Api-Version"] = API_VERSION

# Or use URL segment
response = session.get(f"{base_url}/api/v{API_VERSION}/posts/last10")
```

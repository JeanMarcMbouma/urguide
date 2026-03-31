# UrGuide API Authentication Guide

This guide covers all authentication methods supported by the UrGuide API, including JWT bearer tokens, OAuth2 flows, social login, and two-factor authentication.

## Table of Contents

- [Authentication Methods Overview](#authentication-methods-overview)
- [JWT Bearer Token Authentication](#jwt-bearer-token-authentication)
- [OAuth2 Authorization Code Flow](#oauth2-authorization-code-flow)
- [Social Login Providers](#social-login-providers)
- [Two-Factor Authentication (2FA)](#two-factor-authentication-2fa)
- [Token Refresh](#token-refresh)
- [Rate Limiting on Auth Endpoints](#rate-limiting-on-auth-endpoints)
- [Security Best Practices](#security-best-practices)

---

## Authentication Methods Overview

| Method | Use Case | Endpoint |
|--------|----------|----------|
| **JWT Bearer Token** | API integrations, admin dashboard, mobile apps | `POST /api/auth/token` |
| **OAuth2 Authorization Code + PKCE** | Browser-based apps, Swagger UI | `/connect/authorize` |
| **Social Login (Google)** | End-user authentication via Google | Configured in IdentityServer |
| **Social Login (Microsoft)** | End-user authentication via Microsoft/Azure AD | Configured in IdentityServer |
| **Social Login (Apple)** | End-user authentication via Apple ID | Configured in IdentityServer |

---

## JWT Bearer Token Authentication

JWT bearer tokens are the primary authentication method for API integrations.

### Obtaining a Token

**Endpoint:** `POST /api/auth/token`

```http
POST /api/auth/token
Content-Type: application/json

{
  "userName": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 28800,
  "user": {
    "id": "abc123-def456",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"]
  }
}
```

**Error Response (401):**
```json
{
  "errors": ["Invalid username or password"],
  "isError": true
}
```

### Token Details

| Property | Value |
|----------|-------|
| Algorithm | HS256 |
| Lifetime | 8 hours (28800 seconds) |
| Issuer | Application URI (configurable) |
| Claims | `sub` (user ID), `email`, `name`, `role` |

### Using the Token

Include the token in the `Authorization` header for all authenticated requests:

```http
GET /api/posts/owned
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Alternative Login Endpoint

For admin dashboard and client applications, use:

**Endpoint:** `POST /api/auth/login`

```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin@urguide.com",
  "password": "AdminPassword123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "d4f8a2b1-9c3e-4f5a-b6d7-e8f9a0b1c2d3",
  "expiresIn": 28800,
  "user": {
    "id": "admin-user-id",
    "email": "admin@urguide.com",
    "roles": ["Admin"]
  }
}
```

---

## OAuth2 Authorization Code Flow

For browser-based applications, use the OAuth2 Authorization Code flow with PKCE.

### Configuration

| Parameter | Value |
|-----------|-------|
| Authorization Endpoint | `/connect/authorize` |
| Token Endpoint | `/connect/token` |
| Client ID | `UrGuide.WebAPI` (Swagger) or configured client ID |
| Response Type | `code` |
| Grant Type | `authorization_code` |
| PKCE | Required (S256) |

### Available Scopes

| Scope | Description |
|-------|-------------|
| `openid` | OpenID Connect identity |
| `profile` | User profile information |
| `offline_access` | Refresh token support |
| `api1` | UrGuide API access |

### Flow

1. **Redirect to authorization endpoint:**
   ```
   GET /connect/authorize?
     client_id=your-client-id&
     response_type=code&
     scope=openid profile api1 offline_access&
     redirect_uri=https://your-app.com/callback&
     code_challenge=<S256 challenge>&
     code_challenge_method=S256&
     state=<random state>
   ```

2. **User authenticates and consents**

3. **Exchange authorization code for tokens:**
   ```http
   POST /connect/token
   Content-Type: application/x-www-form-urlencoded

   grant_type=authorization_code&
   code=<authorization_code>&
   redirect_uri=https://your-app.com/callback&
   client_id=your-client-id&
   code_verifier=<original code verifier>
   ```

4. **Receive tokens:**
   ```json
   {
     "access_token": "eyJhbGciOiJSUzI1NiIs...",
     "token_type": "Bearer",
     "expires_in": 3600,
     "refresh_token": "abc123...",
     "id_token": "eyJhbGciOiJSUzI1NiIs..."
   }
   ```

---

## Social Login Providers

UrGuide supports authentication via Google, Microsoft, and Apple. These are configured server-side and available through IdentityServer.

### Google OAuth2

- **Scopes:** `email`, `profile`
- **Configuration Keys:** `SocialAuth:Google:ClientId`, `SocialAuth:Google:ClientSecret`

### Microsoft Account

- **Scopes:** Standard OpenID Connect
- **Supports:** Azure AD and personal Microsoft accounts
- **Configuration Keys:** `SocialAuth:Microsoft:ClientId`, `SocialAuth:Microsoft:ClientSecret`

### Apple Sign-In

- **Scopes:** OpenID
- **Configuration Keys:** `SocialAuth:Apple:ClientId`, `SocialAuth:Apple:TeamId`, `SocialAuth:Apple:KeyId`, `SocialAuth:Apple:PrivateKey`

### Account Linking

Users who sign in with social providers can link additional accounts. The API provides endpoints for managing linked social accounts through the Account controller.

---

## Two-Factor Authentication (2FA)

The API supports TOTP-based 2FA and WebAuthn/FIDO2 passkeys.

### TOTP Setup Flow

1. **Enable 2FA:**
   ```http
   POST /api/twofactor/enable
   Authorization: Bearer <token>
   ```
   Returns a shared secret and QR code URI for authenticator apps.

2. **Verify Setup:**
   ```http
   POST /api/twofactor/verify-enable
   Authorization: Bearer <token>
   Content-Type: application/json

   {
     "code": "123456"
   }
   ```

3. **Login with 2FA:**
   After initial login returns a `requires2fa` flag, verify with:
   ```http
   POST /api/auth/verify-2fa
   Content-Type: application/json

   {
     "code": "123456",
     "rememberMachine": true
   }
   ```

### Backup Codes

When 2FA is enabled, backup recovery codes are generated. Store these securely — they can be used if the authenticator device is lost.

```http
POST /api/twofactor/generate-recovery-codes
Authorization: Bearer <token>
```

### Passkey/WebAuthn

For passwordless authentication via FIDO2:

```http
# Start registration
POST /api/passkey/register-begin
Authorization: Bearer <token>

# Complete registration
POST /api/passkey/register-complete
Authorization: Bearer <token>
Content-Type: application/json
{ ... attestation response ... }

# Start authentication
POST /api/passkey/login-begin
Content-Type: application/json
{ "userName": "user@example.com" }

# Complete authentication  
POST /api/passkey/login-complete
Content-Type: application/json
{ ... assertion response ... }
```

---

## Token Refresh

Access tokens expire after 8 hours. Use refresh tokens to obtain new access tokens without re-authentication.

**Endpoint:** `POST /api/auth/refresh`

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "d4f8a2b1-9c3e-4f5a-b6d7-e8f9a0b1c2d3"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "new-refresh-token-value",
  "expiresIn": 28800
}
```

> **Note:** Each refresh token can only be used once. The response includes a new refresh token for the next refresh cycle.

---

## Rate Limiting on Auth Endpoints

Authentication endpoints are rate-limited to prevent brute-force attacks:

| Endpoint | Limit |
|----------|-------|
| `POST /api/auth/login` | 5 requests per minute per IP |
| `POST /api/auth/token` | 5 requests per minute per IP |
| `POST /api/auth/refresh` | 10 requests per minute per IP |

When rate-limited, the API returns:
```http
HTTP/1.1 429 Too Many Requests
Retry-After: 60

{
  "errors": ["Rate limit exceeded. Please try again later."],
  "isError": true
}
```

---

## Security Best Practices

1. **Store tokens securely** — Use secure HTTP-only cookies or encrypted storage. Never store tokens in `localStorage` in browser apps.

2. **Use HTTPS** — All API communication must use HTTPS in production.

3. **Rotate refresh tokens** — Each refresh token is single-use. Implement proper token rotation.

4. **Enable 2FA** — Require two-factor authentication for administrative accounts.

5. **Validate token expiry** — Check the `expiresIn` value and refresh tokens proactively before expiry.

6. **Handle 401 responses** — Implement automatic token refresh when receiving 401 Unauthorized responses.

7. **Use PKCE for browser apps** — Always use the Authorization Code flow with PKCE for public clients.

8. **Limit token scope** — Request only the scopes your application needs.

---

## JWT Claims Reference

| Claim | Type | Description |
|-------|------|-------------|
| `sub` | `string` | User ID (maps to `ClaimTypes.NameIdentifier`) |
| `email` | `string` | User email address |
| `name` | `string` | User display name |
| `role` | `string[]` | User roles (e.g., `User`, `Guide`, `Admin`) |
| `iat` | `number` | Token issued at (Unix timestamp) |
| `exp` | `number` | Token expiry (Unix timestamp) |
| `iss` | `string` | Token issuer (Application URI) |

---

## Error Codes

| HTTP Status | Error | Resolution |
|-------------|-------|------------|
| `400` | Invalid request body | Check required fields and data types |
| `401` | Invalid credentials | Verify username/password or token |
| `401` | Token expired | Use refresh token to obtain new access token |
| `403` | Insufficient role | Endpoint requires a higher privilege role |
| `423` | Account locked | Account frozen by admin; contact support |
| `429` | Rate limit exceeded | Wait and retry after the `Retry-After` period |

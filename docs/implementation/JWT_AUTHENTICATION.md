# JWT Authentication Implementation

## Overview

UrGuide uses **IdentityServer** (Duende IdentityServer 7.0) for OAuth2/OpenID Connect authentication. The admin dashboard integrates with IdentityServer's standard token endpoint using the Resource Owner Password Credentials (ROPC) grant for simple username/password login.

## Architecture

### IdentityServer Configuration

**File**: `UrGuide.WebApp/Extensions/ServiceCollectionExtensions.cs`

IdentityServer is configured with:
- **Identity Resources**: OpenId, Profile
- **API Scopes**: `api1` (UrGuide API)
- **Clients**: 
  - `admin-dashboard` - Admin dashboard client with ROPC grant
  - `xamarin` - Mobile app client with Authorization Code flow
  - `UrGuide.WebAPI` - Swagger client

### Admin Dashboard Client Configuration

The admin dashboard is registered as an IdentityServer client:

```csharp
new Client
{
    ClientId = "admin-dashboard",
    ClientName = "UrGuide Admin Dashboard",
    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
    ClientSecrets = { new Secret("admin-dashboard-secret".Sha256()) },
    AllowedScopes = {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        IdentityServerConstants.StandardScopes.OfflineAccess,
        "api1"
    },
    AllowOfflineAccess = true, // Enable refresh tokens
    AccessTokenLifetime = 28800, // 8 hours
    RefreshTokenUsage = TokenUsage.ReUse,
    RefreshTokenExpiration = TokenExpiration.Sliding,
    SlidingRefreshTokenLifetime = 604800 // 7 days
}
```

### Token Types

**Access Tokens**:
- RS256 asymmetric JWT tokens signed by IdentityServer
- 8-hour lifetime (28800 seconds)
- Validated using IdentityServer's public key

**Refresh Tokens**:
- 7-day sliding lifetime
- Reusable (not one-time use)
- Allows getting new access tokens without re-authentication

## Authentication Endpoints

### POST /api/auth/login

Admin dashboard login endpoint that proxies to IdentityServer's token endpoint.

**Implementation**: The endpoint internally calls IdentityServer's `/connect/token` endpoint using the Resource Owner Password Credentials grant.

**Request**:
```json
{
  "email": "admin@urguide.local",
  "password": "Admin123!",
  "persist": false
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
  "refreshToken": "...",
  "expiresIn": 28800,
  "tokenType": "Bearer",
  "user": {
    "id": "user-id-guid",
    "email": "admin@urguide.local",
    "userName": "admin@urguide.local",
    "firstName": "Admin",
    "lastName": "User",
    "roles": ["Admin"]
  }
}
```

**Internal Flow**:
1. Receives login request from admin dashboard
2. Calls IdentityServer: `POST /connect/token` with:
   - `client_id=admin-dashboard`
   - `client_secret=admin-dashboard-secret`
   - `grant_type=password`
   - `username` and `password` from request
   - `scope=openid profile api1 offline_access`
3. Returns IdentityServer's access token and refresh token
4. Includes user info from UserService for convenience

**Why proxy through `/api/auth/login`?**
- Maintains consistent API contract for admin dashboard
- Hides IdentityServer client credentials from frontend
- Allows backend to fetch additional user details
- Simpler frontend implementation

### POST /api/auth/refresh

Refresh access token using refresh token.

**Request**:
```json
{
  "refreshToken": "..."
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
  "refreshToken": "...",
  "expiresIn": 28800,
  "tokenType": "Bearer"
}
```

**Internal Flow**:
1. Receives refresh token from admin dashboard
2. Calls IdentityServer: `POST /connect/token` with:
   - `client_id=admin-dashboard`
   - `client_secret=admin-dashboard-secret`
   - `grant_type=refresh_token`
   - `refresh_token` from request
3. Returns new access token and refresh token

**Status Codes**:
- `200 OK` - Token refreshed successfully
- `400 Bad Request` - Invalid or expired refresh token
- `500 Internal Server Error` - Failed to communicate with IdentityServer

### GET /api/auth/me

Get current authenticated user information.

**Request**:
```
GET /api/auth/me
Authorization: Bearer <jwt-token>
```

**Response**:
```json
{
  "userId": "user-id-guid",
  "userName": "admin@urguide.local",
  "email": "admin@urguide.local",
  "firstName": "Admin",
  "lastName": "User",
  "roles": ["Admin"]
}
```

**Status Codes**:
- `200 OK` - User info returned
- `401 Unauthorized` - Invalid or expired token

## Client-Side Integration

### Admin Dashboard (React)

**File**: `admin-dashboard/src/services/authService.ts`

The admin dashboard stores tokens in localStorage and includes them in all API requests:

```typescript
// Login
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});
const { accessToken, refreshToken, expiresIn, user } = await response.json();
localStorage.setItem('adminToken', accessToken);
localStorage.setItem('adminRefreshToken', refreshToken);

// Authenticated API call
const response = await fetch('/api/admin/users', {
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('adminToken')}`
  }
});

// Handle 401 - refresh token
if (response.status === 401) {
  const refreshResponse = await fetch('/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ 
      refreshToken: localStorage.getItem('adminRefreshToken') 
    })
  });
  const { accessToken, refreshToken } = await refreshResponse.json();
  localStorage.setItem('adminToken', accessToken);
  localStorage.setItem('adminRefreshToken', refreshToken);
  // Retry original request
}
```

### Token Expiration Handling

**Access Tokens**: Expire after 8 hours
**Refresh Tokens**: Expire after 7 days (sliding)

When an access token expires:
1. API returns `401 Unauthorized`
2. Client calls `/api/auth/refresh` with refresh token
3. Receives new access token and refresh token
4. Retries failed request with new access token

When refresh token expires:
1. Refresh endpoint returns `400 Bad Request`
2. Client clears stored tokens
3. User is redirected to login page

## Security Considerations

### Token Types

**Access Tokens**:
- RS256 asymmetric encryption (public/private key)
- Signed by IdentityServer's private key
- Validated using IdentityServer's public key (from `.well-known/jwks`)
- Short-lived (8 hours) to limit exposure window

**Refresh Tokens**:
- Opaque tokens (not JWT)
- Stored in IdentityServer's token store
- Longer lifetime (7 days sliding)
- Can be revoked server-side

### Best Practices

1. **Use HTTPS** in production to prevent token interception
2. **Store tokens securely** in httpOnly cookies or secure localStorage
3. **Implement token refresh** to avoid frequent re-authentication
4. **Revoke refresh tokens** on logout or security events
5. **Monitor token usage** for suspicious patterns
6. **Set appropriate lifetimes** (shorter for access, longer for refresh)
7. **Validate tokens on every request** (done automatically by middleware)
8. **Include roles in tokens** for authorization checks

### Client Credentials

The admin dashboard client credentials are configured via:

**appsettings.json** (Placeholder):
```json
{
  "IdentityServer": {
    "Clients": {
      "AdminDashboard": {
        "ClientId": "admin-dashboard",
        "ClientSecret": ""
      }
    }
  }
}
```

**Environment Variables** (Recommended):
```env
IDENTITYSERVER__CLIENTS__ADMINDASHBOARD__CLIENTID=admin-dashboard
IDENTITYSERVER__CLIENTS__ADMINDASHBOARD__CLIENTSECRET=your-secure-secret-here
```

**Docker Compose** (.env file):
```env
ADMIN_DASHBOARD_CLIENT_SECRET=your-secure-secret-here
```

**Generate Secure Secret**:
```bash
openssl rand -base64 32
```

### Security Requirements

The admin dashboard client secret must be:
- **At least 32 characters** (base64 encoded)
- **Never committed to source control** (use .env files, User Secrets, or Azure Key Vault)
- **Different per environment** (dev, staging, production)
- **Rotated periodically** for security (every 90 days recommended)
- **Stored securely** in production (Azure Key Vault, AWS Secrets Manager, etc.)

**Development**: Set in User Secrets:
```bash
dotnet user-secrets set "IdentityServer:Clients:AdminDashboard:ClientSecret" "$(openssl rand -base64 32)" --project UrGuide.WebApp
```

**Production**: Use Azure Key Vault or environment-specific configuration.

### Rate Limiting

The `/api/auth/login` endpoint is rate-limited to 5 attempts per minute using the `[RateLimit]` attribute to prevent brute-force attacks.

## Implementation Details

### IdentityServer Setup

**File**: `UrGuide.WebApp/Extensions/ServiceCollectionExtensions.cs`

IdentityServer is configured with AspNetIdentity integration:

```csharp
services.AddIdentityServer(options =>
{
    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
    options.IssuerUri = applicationUri;
})
.AddInMemoryIdentityResources(GetIdentityResources())
.AddInMemoryApiScopes(GetApiScopes())
.AddInMemoryClients(GetClients(configuration, applicationUri))
.AddAspNetIdentity<UrGuideUser>()
.AddDeveloperSigningCredential();
```

**Development Signing Credential**: Uses `AddDeveloperSigningCredential()` for local development. In production, use `AddSigningCredential()` with a real certificate.

### HTTP Client Registration

**File**: `UrGuide.WebApp/Program.cs`

HttpClient is registered for calling IdentityServer:

```csharp
builder.Services.AddHttpClient();
```

### Controller Integration

**File**: `UrGuide.WebApp/Controllers/AccountController.cs`

The `AccountController` injects:
- `IConfiguration` - Get ApplicationUri and IdentityServer settings
- `IHttpClientFactory` - Create HTTP client for calling IdentityServer
- `UserManager<UrGuideUser>` - Retrieve user roles
- `IUserService` - Validate credentials and get user details

```csharp
public AccountController(
    IUserService userService, 
    IAuthService authService, 
    IIdentityServerInteractionService interactionService,
    IJwtTokenService jwtTokenService,
    UserManager<UrGuideUser> userManager,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory)
```

### Token Generation Flow

1. User submits credentials to `/api/auth/login`
2. Controller calls IdentityServer's `/connect/token` endpoint:
   - HTTP POST with `application/x-www-form-urlencoded` content
   - Includes client credentials, username, password, and scopes
3. IdentityServer validates credentials via AspNetIdentity
4. IdentityServer issues access token and refresh token
5. Controller validates login via `UserService.LoginAsync()`
6. Controller fetches user details and roles
7. Returns IdentityServer tokens with user info to client

### Token Validation Flow

1. Client sends request with `Authorization: Bearer <token>` header
2. ASP.NET Core JWT Bearer middleware validates token:
   - Downloads IdentityServer's public key from `/.well-known/jwks`
   - Verifies RS256 signature
   - Checks expiration
   - Extracts claims
3. If valid, `HttpContext.User` populated with claims
4. `[Authorize]` attributes check roles and authentication
5. If invalid, returns `401 Unauthorized`

## Testing

### Direct IdentityServer Token Endpoint

You can call IdentityServer directly for testing:

```bash
# Get token from IdentityServer
curl -X POST http://localhost:5000/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=admin-dashboard" \
  -d "client_secret=admin-dashboard-secret" \
  -d "grant_type=password" \
  -d "username=admin@urguide.local" \
  -d "password=Admin123!" \
  -d "scope=openid profile api1 offline_access"

# Refresh token
curl -X POST http://localhost:5000/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=admin-dashboard" \
  -d "client_secret=admin-dashboard-secret" \
  -d "grant_type=refresh_token" \
  -d "refresh_token=<refresh_token_from_login>"
```

### Via Admin Dashboard API

```bash
# Login via proxy endpoint
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@urguide.local","password":"Admin123!"}'

# Refresh via proxy endpoint
curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh_token>"}'

# Use token for authenticated request
curl http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer <access_token>"
```

### Admin Dashboard Testing

1. Start API and admin dashboard:
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
   ```

2. Open admin dashboard: http://localhost:3001

3. Login with admin credentials (from `.env` file)

4. Verify tokens in browser DevTools:
   - Application tab → Local Storage → `adminToken`, `adminRefreshToken`
   - Network tab → Check `Authorization` header on API requests

5. Test token refresh:
   - Wait for access token to expire (or manually expire in localStorage)
   - Make API request that triggers 401
   - Verify refresh endpoint is called automatically
   - Verify new token is stored and request retries

### IdentityServer Discovery Endpoint

Check IdentityServer configuration:

```bash
curl http://localhost:5000/.well-known/openid-configuration
```

Response includes:
- `token_endpoint`: `/connect/token`
- `jwks_uri`: `/.well-known/openid-configuration/jwks`
- `grant_types_supported`: `["password", "refresh_token", "authorization_code"]`

## Troubleshooting

### "Admin dashboard client secret not configured" Error

**Cause**: `IdentityServer:Clients:AdminDashboard:ClientSecret` not set in configuration

**Solution**: Set the environment variable:
```bash
# Docker (.env file)
echo "ADMIN_DASHBOARD_CLIENT_SECRET=$(openssl rand -base64 32)" >> .env

# User Secrets
dotnet user-secrets set "IdentityServer:Clients:AdminDashboard:ClientSecret" "$(openssl rand -base64 32)" --project UrGuide.WebApp

# Or set in appsettings.Development.json (NOT for production!)
{
  "IdentityServer": {
    "Clients": {
      "AdminDashboard": {
        "ClientSecret": "your-dev-secret-here"
      }
    }
  }
}
```

### Token Not Generated (Returns "cookie-based-auth")

**Cause**: Controller not using `IJwtTokenService`

**Solution**: Ensure `JwtTokenService` is injected and used in login endpoint

### 401 Unauthorized After Login

**Cause**: Token validation failing

**Check**:
1. JWT key matches between generation and validation
2. Token not expired
3. Token format is valid (3 base64 segments separated by dots)
4. `Authorization: Bearer <token>` header format correct

### "No JWT key configured" Warning

**Cause**: `JWT__KEY` environment variable not set

**Solution**: Set secure JWT key:
```bash
# Docker
echo "JWT__KEY=$(openssl rand -base64 32)" >> .env

# User Secrets
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)" --project UrGuide.WebApp
```

### Token Expires Too Quickly/Slowly

**Cause**: Default expiration time (8 hours) doesn't match requirements

**Solution**: Configure `JWT__EXPIRESINHOUHS` environment variable:
```bash
# 1 hour expiration
JWT__EXPIRESINHOUHS=1

# 24 hour expiration
JWT__EXPIRESINHOUHS=24
```

## Related Documentation

- [Admin Dashboard README](../../admin-dashboard/README.md)
- [Admin API Documentation](ADMIN_API_DOCUMENTATION.md)
- [Security & Secrets Management](../../README.md#-security--secrets-management)
- [Docker Quickstart](../../DOCKER_QUICKSTART.md)

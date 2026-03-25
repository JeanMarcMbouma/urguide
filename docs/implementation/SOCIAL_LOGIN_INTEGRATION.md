# Social Login Integration

This document covers the implementation of social login support for UrGuide, including Google OAuth, Apple Sign-In, Microsoft OAuth, and social account linking.

## Overview

UrGuide supports social login via three providers:

| Provider | Protocol | Package | Features |
|----------|----------|---------|----------|
| **Google** | OAuth 2.0 | `Microsoft.AspNetCore.Authentication.Google` | Email, profile, avatar sync |
| **Apple** | Sign-In with Apple | `AspNet.Security.OAuth.Apple` | Email privacy relay, iOS/web |
| **Microsoft** | OAuth 2.0 | `Microsoft.AspNetCore.Authentication.MicrosoftAccount` | Personal + work accounts |

## Architecture

### Backend Components

```
UrGuide.WebApp/
├── Controllers/
│   └── SocialAuthController.cs        # OAuth flow endpoints
├── Entities/
│   ├── SocialLoginProvider.cs          # Linked social account entity
│   └── SocialLoginAuditLog.cs          # Audit trail entity
├── Services/
│   ├── ISocialAuthService.cs           # Service interface + DTOs
│   └── SocialAuthService.cs            # Service implementation
├── Data/
│   └── UrGuideAuthContext.cs           # Updated with social login DbSets
└── Extensions/
    └── ServiceCollectionExtensions.cs  # Provider registration
```

### Frontend Components

Each frontend app (tourist-website, guide-portal, admin-dashboard) includes:

```
src/
├── services/
│   └── socialAuthService.ts            # API client for social auth
├── components/shared/
│   └── SocialLoginButtons.tsx          # Reusable social login buttons
└── pages/
    └── Login.tsx                       # Updated with social login buttons
```

## API Endpoints

### Public Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/social-auth/login/{provider}` | Initiates OAuth flow (redirects to provider) |
| `GET` | `/api/social-auth/callback/{provider}` | OAuth callback handler |
| `GET` | `/api/social-auth/providers/available` | Lists available social providers |

### Authenticated Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/social-auth/link/{provider}` | Initiates provider linking |
| `GET` | `/api/social-auth/link-callback/{provider}` | Linking callback handler |
| `DELETE` | `/api/social-auth/unlink/{provider}` | Unlinks a provider |
| `GET` | `/api/social-auth/providers` | Lists user's linked providers |
| `GET` | `/api/social-auth/audit-log` | Gets social login audit trail |

### Supported Providers

- `Google` — Google OAuth 2.0
- `Apple` — Apple Sign-In
- `Microsoft` — Microsoft Account (personal + Azure AD)

## Configuration

### Environment Variables

Social login credentials are configured via environment variables (never committed to source):

```bash
# Google OAuth
SOCIAL_AUTH_GOOGLE_CLIENT_ID=your-google-client-id
SOCIAL_AUTH_GOOGLE_CLIENT_SECRET=your-google-client-secret

# Microsoft OAuth
SOCIAL_AUTH_MICROSOFT_CLIENT_ID=your-microsoft-client-id
SOCIAL_AUTH_MICROSOFT_CLIENT_SECRET=your-microsoft-client-secret

# Apple Sign-In
SOCIAL_AUTH_APPLE_CLIENT_ID=your-apple-service-id
SOCIAL_AUTH_APPLE_TEAM_ID=your-apple-team-id
SOCIAL_AUTH_APPLE_KEY_ID=your-apple-key-id
SOCIAL_AUTH_APPLE_PRIVATE_KEY=your-apple-private-key
```

### appsettings.json Structure

```json
{
  "SocialAuth": {
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Microsoft": {
      "ClientId": "",
      "ClientSecret": "",
      "TenantId": "common"
    },
    "Apple": {
      "ClientId": "",
      "TeamId": "",
      "KeyId": "",
      "PrivateKey": ""
    }
  }
}
```

> **Note:** Providers are only registered when their credentials are configured. If a provider's ClientId is empty, it won't be available at runtime.

### Provider Setup Guides

#### Google OAuth 2.0

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable "Google+ API" or "People API"
4. Create OAuth 2.0 credentials (Web Application)
5. Add redirect URI: `https://your-domain/api/social-auth/callback/Google`
6. Set `SOCIAL_AUTH_GOOGLE_CLIENT_ID` and `SOCIAL_AUTH_GOOGLE_CLIENT_SECRET`

#### Apple Sign-In

1. Go to [Apple Developer Portal](https://developer.apple.com/)
2. Create an App ID with "Sign In with Apple" capability
3. Create a Services ID for web authentication
4. Generate a private key for Sign-In with Apple
5. Add redirect URI: `https://your-domain/api/social-auth/callback/Apple`
6. Set environment variables for Client ID, Team ID, Key ID, and Private Key

> **Apple Email Privacy Relay:** Apple allows users to hide their real email address. The system handles relay addresses transparently.

#### Microsoft OAuth 2.0

1. Go to [Azure Portal](https://portal.azure.com/) > App registrations
2. Register a new application (select "Accounts in any organizational directory and personal Microsoft accounts")
3. Add redirect URI: `https://your-domain/api/social-auth/callback/Microsoft`
4. Create a client secret
5. Set `SOCIAL_AUTH_MICROSOFT_CLIENT_ID` and `SOCIAL_AUTH_MICROSOFT_CLIENT_SECRET`

> **Work/Personal Accounts:** The `TenantId` is set to `common` by default, supporting both personal Microsoft accounts and Azure AD (work/school) accounts.

## Account Linking Flow

### Automatic Linking

When a user signs in with a social provider:

1. **Existing link found** → Log in and issue JWT token
2. **No link but email matches** → Auto-link provider to existing account
3. **No link, no email match** → Create new account with provider data

### Manual Linking

Authenticated users can link additional social providers via:

1. Navigate to account settings
2. Click "Link" on the desired provider
3. Complete OAuth consent flow
4. Provider is linked to the existing account

### Unlinking

Users can unlink social providers with safety checks:

- Cannot unlink the last login method (must have a password or another provider)
- Audit log entry created for every link/unlink action

## Audit Logging

Every social login action is recorded with:

- User ID
- Provider name
- Action type (`Login`, `AccountCreated`, `Linked`, `Unlinked`, `ConflictResolved`)
- IP address and user agent
- Timestamp

Access audit logs via `GET /api/social-auth/audit-log`.

## Database Entities

### SocialLoginProvider

| Column | Type | Description |
|--------|------|-------------|
| Id | string (PK) | Unique identifier |
| UserId | string (FK) | Reference to UrGuideUser |
| Provider | string(50) | Provider name |
| ProviderKey | string(256) | Provider's user ID |
| Email | string(256)? | Email from provider |
| DisplayName | string(256)? | Display name from provider |
| AvatarUrl | string(1024)? | Avatar URL from provider |
| LinkedAt | DateTime | When the link was created |
| LastLoginAt | DateTime? | Last social login timestamp |

**Indexes:**
- Unique: `(Provider, ProviderKey)` — one provider account links to one user
- Unique: `(UserId, Provider)` — one user has one link per provider

### SocialLoginAuditLog

| Column | Type | Description |
|--------|------|-------------|
| Id | string (PK) | Unique identifier |
| UserId | string | User who performed the action |
| Provider | string(50) | Provider name |
| Action | string(50) | Action type |
| Details | string(1000)? | Additional details |
| IpAddress | string(50)? | Client IP address |
| UserAgent | string(500)? | Client user agent |
| Timestamp | DateTime | When the action occurred |

## Security Considerations

1. **Credentials stored via environment variables** — never in source code
2. **CSRF protection** — OAuth state parameter validated by ASP.NET Core
3. **Token security** — JWT tokens issued after successful social login
4. **Audit trail** — all social login actions are logged
5. **Unlink safety** — cannot remove last authentication method
6. **Email verification** — social provider emails are treated as pre-verified
7. **Provider isolation** — each provider has unique constraint preventing duplicate links

## Testing

Integration tests are located at:
```
tests/UrGuide.IntegrationTests/Controllers/SocialAuthControllerTests.cs
```

Tests cover:
- Unsupported provider validation
- Supported provider challenge responses (Google, Apple, Microsoft)
- Unlink with success and failure scenarios
- Linked providers listing
- Audit log retrieval
- Authentication requirements enforcement

Run tests:
```bash
dotnet test tests/UrGuide.IntegrationTests/
```

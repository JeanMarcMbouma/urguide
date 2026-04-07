# Secrets Management Guide

## Overview

This document explains how to securely manage secrets, credentials, and API keys in the UrGuide project. **Never commit real secrets to source control!**

## Security Principles

1. ✅ **DO** store secrets in environment variables
2. ✅ **DO** use `.env` files for local development (ignored by Git)
3. ✅ **DO** use Azure Key Vault or similar in production
4. ✅ **DO** use `.env.example` as a template (committed to Git without real values)
5. ❌ **DON'T** commit `.env` files with real secrets
6. ❌ **DON'T** hardcode secrets in `appsettings.json`
7. ❌ **DON'T** include secrets in Dockerfiles
8. ❌ **DON'T** share secrets via email or chat

## Local Development Setup

### 1. Create Your .env File

```bash
# Copy the example file
cp .env.example .env

# Edit with your actual values
# The .env file is already in .gitignore and will NOT be committed
```

### 2. Required Secrets for Development

**Minimum required:**
- `SQL_SA_PASSWORD` - SQL Server password
- `ADMIN_DASHBOARD_CLIENT_SECRET` - For admin dashboard authentication
- `ADMIN_PASSWORD` - Default admin user password

**Optional but recommended:**
- `IPSTACK_API_KEY` - For IP geolocation
- `SMTP__USERNAME` / `SMTP__PASSWORD` - For email delivery (SMTP credentials)
- `STRIPE_SECRET_KEY` - For payment testing
- `JWT__KEY` - Custom JWT secret (auto-generated if not set)

### 3. Generate Secure Secrets

```bash
# Generate a secure random secret (Linux/Mac/WSL)
openssl rand -base64 32

# Generate on Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))

# Generate UUID
uuidgen  # or use online: https://www.uuidgenerator.net/
```

## Environment Variable Mapping

### Docker Compose (via .env file)

| Environment Variable | Configuration Path | Description |
|---------------------|-------------------|-------------|
| `SQL_SA_PASSWORD` | ConnectionStrings | SQL Server SA password |
| `ADMIN_DASHBOARD_CLIENT_SECRET` | IdentityServer:Clients:AdminDashboard:ClientSecret | OAuth2 client secret |
| `XAMARIN_CLIENT_SECRET` | IdentityServer:Clients:Xamarin:ClientSecret | Mobile app client secret |
| `IPSTACK_API_KEY` | IpStack:ApiKey | IP geolocation API key |
| `SMTP__HOST` | Smtp:Host | SMTP server hostname |
| `SMTP__USERNAME` | Smtp:Username | SMTP authentication username |
| `SMTP__PASSWORD` | Smtp:Password | SMTP authentication password |
| `STRIPE_SECRET_KEY` | Stripe:SecretKey | Payment gateway secret |
| `STRIPE_WEBHOOK_SECRET` | Stripe:WebhookSecret | Stripe webhook signature |
| `JWT__KEY` | Jwt:Key | Custom JWT signing key |
| `ADMIN_EMAIL` | Seeding:AdminEmail | Default admin username |
| `ADMIN_PASSWORD` | Seeding:AdminPassword | Default admin password |
| `RABBITMQ_USER` | RabbitMQ:Username | Message queue username |
| `RABBITMQ_PASS` | RabbitMQ:Password | Message queue password |
| `ELASTICSEARCH_USERNAME` | Elasticsearch:Username | Search engine username |
| `ELASTICSEARCH_PASSWORD` | Elasticsearch:Password | Search engine password |

### Windows Local Development (without Docker)

For Windows development using LocalDB, you can use User Secrets:

```bash
# Initialize user secrets
cd UrGuide.WebApp
dotnet user-secrets init

# Set individual secrets
dotnet user-secrets set "IdentityServer:Clients:AdminDashboard:ClientSecret" "your-secret-here"
dotnet user-secrets set "IpStack:ApiKey" "your-key-here"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_your-key"
dotnet user-secrets set "Jwt:Key" "your-jwt-secret-key-here"

# List all secrets
dotnet user-secrets list
```

User Secrets are stored in:
- **Windows:** `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **Linux/Mac:** `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

## Production Deployment

### Azure App Service

1. **Environment Variables:**
   - Go to Azure Portal → Your App Service → Configuration → Application Settings
   - Add each secret as a new application setting
   - Settings are encrypted at rest and transmitted over encrypted channels

2. **Azure Key Vault (Recommended):**
   ```csharp
   // Already configured in Program.cs for production
   builder.Configuration.AddAzureKeyVault(
       new Uri(keyVaultUrl),
       new DefaultAzureCredential());
   ```

3. **Setting Key Vault Secrets:**
   ```bash
   # Using Azure CLI
   az keyvault secret set --vault-name "your-vault" \
       --name "IdentityServer--Clients--AdminDashboard--ClientSecret" \
       --value "your-secret-value"
   
   # Note: Use '--' instead of ':' for nested configuration paths
   ```

### Docker Production

1. **Use Docker Secrets:**
   ```yaml
   # docker-compose.prod.yml
   services:
     api:
       secrets:
         - db_password
         - admin_secret
       environment:
         - SQL_SA_PASSWORD_FILE=/run/secrets/db_password
   
   secrets:
     db_password:
       external: true
     admin_secret:
       external: true
   ```

2. **Or use external secret management:**
   - HashiCorp Vault
   - AWS Secrets Manager
   - Azure Key Vault
   - Google Cloud Secret Manager

## Rotating Secrets

### When to Rotate

- Immediately if secret is compromised
- Periodically (every 90 days recommended)
- When team member with access leaves
- After security audit findings

### How to Rotate

1. **Generate new secret**
2. **Update in secret store** (Key Vault, .env, etc.)
3. **Restart application** to load new secrets
4. **Verify functionality**
5. **Revoke old secret** after confirmation

Example for JWT key rotation:
```bash
# 1. Generate new key
NEW_JWT_KEY=$(openssl rand -base64 32)

# 2. Update in Key Vault
az keyvault secret set --vault-name "urguide-vault" \
    --name "Jwt--Key" --value "$NEW_JWT_KEY"

# 3. Restart App Service
az webapp restart --name "urguide-api" --resource-group "urguide-rg"
```

## Checking for Leaked Secrets

### Before Committing

```bash
# Check for potential secrets in staged files
git diff --cached | grep -E "password|secret|key|token" -i

# Use git-secrets tool
git secrets --scan

# Use gitleaks
gitleaks detect --source . --verbose
```

### Audit Repository History

```bash
# Search for potential secrets in commit history
git log -p | grep -E "password|secret|key|token" -i

# Use truffleHog
trufflehog git file://. --only-verified
```

### If Secret is Accidentally Committed

1. **Immediately rotate the secret** - consider it compromised
2. **Remove from Git history:**
   ```bash
   # Using BFG Repo-Cleaner (recommended)
   bfg --replace-text passwords.txt
   git reflog expire --expire=now --all
   git gc --prune=now --aggressive
   
   # Force push to remote (WARNING: Destructive!)
   git push --force --all
   ```
3. **Notify team members** to re-clone the repository
4. **Update all environments** with new secrets

## Validation Checklist

Before deploying:

- [ ] No secrets in `appsettings.json` or `appsettings.Development.json`
- [ ] `.env` file exists locally and is in `.gitignore`
- [ ] `.env.example` is up-to-date with all required variables
- [ ] All secrets are properly configured in production secret store
- [ ] Connection strings use environment variables
- [ ] API keys are not hardcoded in source files
- [ ] Dockerfile doesn't contain secrets
- [ ] `docker-compose.yml` references environment variables only
- [ ] User secrets configured for local Windows development
- [ ] Production uses Azure Key Vault or equivalent

## Troubleshooting

### "Secret not found" Error

1. Check environment variable name matches configuration path
2. Verify `.env` file is in the root directory (same level as `docker-compose.yml`)
3. Ensure `docker-compose` is loading `.env`: `docker-compose config` shows resolved values
4. Restart containers after updating `.env`: `docker-compose down && docker-compose up -d`

### "Missing secret" in Azure

1. Verify Key Vault access policy allows the app to read secrets
2. Check Managed Identity is enabled for App Service
3. Verify secret name format uses `--` instead of `:` for nested paths
4. Check application logs for specific secret that's missing

### JWT Bearer "RequireHttpsMetadata" Error

- Development: Ensure `ApplicationUri` starts with `http://` (not `https://`) in `.env`
- Production: Always use `https://` for security
- The app automatically sets `RequireHttpsMetadata = false` for HTTP URLs

## Additional Resources

- [ASP.NET Core Configuration](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)
- [OWASP Secrets Management Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)

## Support

For security concerns, contact the development team immediately. Never discuss actual secret values in public channels.

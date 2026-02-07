# Secrets Management Guide

This document describes how to securely manage secrets and API keys in the UrGuide application across different environments.

## Overview

The UrGuide platform uses several external services that require API keys and secrets:

1. **IPStack API** - For IP geolocation services
2. **SendGrid** - For email notifications
3. **Database Connections** - SQL Server connection strings with passwords
4. **IdentityServer** - OAuth2/OIDC client secrets

## Security Principles

- **Never commit secrets to source control**
- **Use different secrets for each environment** (Development, Staging, Production)
- **Rotate secrets regularly**
- **Use minimal permissions** for service accounts
- **Audit secret access**

## Local Development - User Secrets

For local development, use .NET User Secrets to store sensitive configuration outside of your project directory.

### Setup User Secrets

The UrGuide.WebApp project is already configured with a UserSecretsId: `d216c234-b1d4-4e9e-be44-0c9a46cdacde`

#### Initialize or Update User Secrets

```bash
cd UrGuide.WebApp

# Set IPStack API Key
dotnet user-secrets set "IpStack:ApiKey" "your-ipstack-api-key-here"

# Set SendGrid API Key
dotnet user-secrets set "SENDGRID_URGUIDE_API_KEY" "your-sendgrid-api-key-here"

# Set Xamarin Client Secret (for mobile app authentication)
dotnet user-secrets set "IdentityServer:Clients:Xamarin:ClientSecret" "your-secure-client-secret-here"

# Set SQL Server password (if using SQL authentication)
dotnet user-secrets set "ConnectionStrings:Data" "Server=localhost;Database=urguide_data;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
dotnet user-secrets set "ConnectionStrings:Id4" "Server=localhost;Database=urguide_id4;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

#### List Current Secrets

```bash
dotnet user-secrets list
```

#### Remove a Secret

```bash
dotnet user-secrets remove "IpStack:ApiKey"
```

#### Clear All Secrets

```bash
dotnet user-secrets clear
```

### How User Secrets Work

- Secrets are stored in a JSON file at:
  - **Windows**: `%APPDATA%\Microsoft\UserSecrets\d216c234-b1d4-4e9e-be44-0c9a46cdacde\secrets.json`
  - **Linux/macOS**: `~/.microsoft/usersecrets/d216c234-b1d4-4e9e-be44-0c9a46cdacde/secrets.json`
- User Secrets override values from `appsettings.json` and `appsettings.Development.json`
- They are loaded automatically in Development environment
- They are **never** checked into source control

## Docker Development - Environment Variables

When running the application in Docker, use environment variables or Docker secrets.

### Using .env File

1. Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` and set your secrets:
   ```env
   SQL_SA_PASSWORD=YourStrong@Passw0rd123
   IPSTACK_API_KEY=your-ipstack-api-key
   SENDGRID_API_KEY=your-sendgrid-api-key
   XAMARIN_CLIENT_SECRET=your-client-secret
   ```

3. The `.env` file is already in `.gitignore` and will not be committed

### Environment Variables in docker-compose.yml

You can also set environment variables directly in `docker-compose.override.yml`:

```yaml
version: '3.8'

services:
  api:
    environment:
      - IpStack__ApiKey=${IPSTACK_API_KEY}
      - SENDGRID_URGUIDE_API_KEY=${SENDGRID_API_KEY}
      - IdentityServer__Clients__Xamarin__ClientSecret=${XAMARIN_CLIENT_SECRET}
```

## Production - Azure Key Vault (Recommended)

For production deployments, use Azure Key Vault to store and manage secrets.

### Prerequisites

1. Azure subscription with Key Vault created
2. Managed Identity or Service Principal with Key Vault access
3. Azure Key Vault NuGet package installed

### Configuration

1. Install the Azure Key Vault configuration provider:
   ```bash
   dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
   dotnet add package Azure.Identity
   ```

2. Update `Program.cs` to load secrets from Key Vault:
   ```csharp
   using Azure.Identity;
   using Azure.Security.KeyVault.Secrets;
   using Azure.Extensions.AspNetCore.Configuration.Secrets;

   var builder = WebApplication.CreateBuilder(args);

   // Add Azure Key Vault if configured
   var keyVaultUrl = builder.Configuration["KeyVault:Url"];
   if (!string.IsNullOrEmpty(keyVaultUrl))
   {
       var secretClient = new SecretClient(
           new Uri(keyVaultUrl), 
           new DefaultAzureCredential());
       
       builder.Configuration.AddAzureKeyVault(
           secretClient, 
           new KeyVaultSecretManager());
   }
   ```

3. Set the Key Vault URL in `appsettings.Production.json`:
   ```json
   {
     "KeyVault": {
       "Url": "https://your-keyvault.vault.azure.net/"
     }
   }
   ```

### Key Vault Secret Naming

Azure Key Vault secrets should follow this naming convention (Key Vault doesn't support `:` or `.`):

| Configuration Key | Key Vault Secret Name |
|------------------|----------------------|
| `IpStack:ApiKey` | `IpStack--ApiKey` |
| `SENDGRID_URGUIDE_API_KEY` | `SENDGRID-URGUIDE-API-KEY` |
| `IdentityServer:Clients:Xamarin:ClientSecret` | `IdentityServer--Clients--Xamarin--ClientSecret` |

The configuration provider automatically converts `--` to `:` when loading secrets.

### Setting Secrets in Azure Key Vault

Using Azure CLI:
```bash
# Login to Azure
az login

# Set secrets
az keyvault secret set --vault-name your-keyvault --name "IpStack--ApiKey" --value "your-ipstack-api-key"
az keyvault secret set --vault-name your-keyvault --name "SENDGRID-URGUIDE-API-KEY" --value "your-sendgrid-api-key"
az keyvault secret set --vault-name your-keyvault --name "IdentityServer--Clients--Xamarin--ClientSecret" --value "your-client-secret"
```

Using Azure Portal:
1. Navigate to your Key Vault
2. Go to "Secrets" section
3. Click "+ Generate/Import"
4. Enter the secret name and value
5. Click "Create"

## Kubernetes - Secrets

For Kubernetes deployments, use Kubernetes Secrets.

### Create Secret from Literal Values

```bash
kubectl create secret generic urguide-secrets \
  --from-literal=ipstack-api-key='your-ipstack-api-key' \
  --from-literal=sendgrid-api-key='your-sendgrid-api-key' \
  --from-literal=xamarin-client-secret='your-client-secret'
```

### Use Secrets in Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: urguide-api
spec:
  template:
    spec:
      containers:
      - name: api
        image: urguide/api:latest
        env:
        - name: IpStack__ApiKey
          valueFrom:
            secretKeyRef:
              name: urguide-secrets
              key: ipstack-api-key
        - name: SENDGRID_URGUIDE_API_KEY
          valueFrom:
            secretKeyRef:
              name: urguide-secrets
              key: sendgrid-api-key
        - name: IdentityServer__Clients__Xamarin__ClientSecret
          valueFrom:
            secretKeyRef:
              name: urguide-secrets
              key: xamarin-client-secret
```

## Environment-Specific Configuration

### appsettings.json (Default/Template)
- Contains **NO** secret values
- Uses empty strings or placeholder text
- Serves as documentation for required settings

### appsettings.Development.json (Local)
- Can contain local connection strings with Windows Authentication
- Should **NOT** contain API keys or secrets
- Uses User Secrets for sensitive values

### appsettings.Production.json (Production)
- Contains production URLs and non-sensitive settings
- References Key Vault for secrets
- Should **NOT** contain actual secret values

## Secret Rotation

Regularly rotate secrets to maintain security:

1. **Generate new secret** in the provider (SendGrid, IPStack, etc.)
2. **Update secret** in your secret store (User Secrets, Key Vault, etc.)
3. **Test** the application with new secret
4. **Revoke old secret** from the provider
5. **Document** the rotation in your security audit log

### Recommended Rotation Schedule

- **Development secrets**: Every 90 days
- **Production secrets**: Every 30-60 days
- **Database passwords**: Every 90 days
- **Client secrets**: Every 180 days or immediately if compromised

## Troubleshooting

### Secret Not Loading

1. Verify the secret name matches exactly (case-sensitive)
2. Check the environment (User Secrets only load in Development)
3. Verify UserSecretsId in `.csproj` file
4. Check Key Vault permissions (if using Azure)
5. Review application logs for configuration errors

### Testing Secret Configuration

Add this temporary code to verify secrets are loaded:

```csharp
// In Program.cs (remove after testing)
var ipStackKey = builder.Configuration["IpStack:ApiKey"];
var sendGridKey = builder.Configuration["SENDGRID_URGUIDE_API_KEY"];

if (string.IsNullOrEmpty(ipStackKey))
    logger.LogWarning("IPStack API Key is not configured");
if (string.IsNullOrEmpty(sendGridKey))
    logger.LogWarning("SendGrid API Key is not configured");
```

## Security Checklist

- [ ] All secrets removed from `appsettings.json`
- [ ] User Secrets configured for local development
- [ ] `.env` file in `.gitignore`
- [ ] Production secrets in Azure Key Vault
- [ ] Service principal has minimal Key Vault permissions
- [ ] Secrets rotated on schedule
- [ ] Team members trained on secret management
- [ ] Audit logging enabled for secret access
- [ ] Backup secrets stored securely offline

## Support

For questions or issues with secrets management:
1. Check this documentation first
2. Review application logs for configuration errors
3. Open an issue on GitHub (without including actual secrets)
4. Contact the platform administrator

## References

- [.NET User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)
- [Kubernetes Secrets](https://kubernetes.io/docs/concepts/configuration/secret/)
- [.NET Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

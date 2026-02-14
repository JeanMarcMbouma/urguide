# Admin User Provisioning

This directory contains utilities for creating admin users in the UrGuide application for development and deployment purposes.

## Overview

The UrGuide platform provides multiple methods to provision admin users:

1. **Docker Compose with .env (Recommended)** - Environment variable-based provisioning for containerized deployments
2. **Automatic Seeding (Development)** - Automatically creates an admin user when the application starts
3. **PowerShell Script** - Manually provisions admin users via database access
4. **C# Service** - Programmatically creates admin users via ASP.NET Identity

## Option 1: Docker Compose with .env File (Recommended for Docker)

When running with Docker Compose, configure admin provisioning using environment variables in your `.env` file.

### Configuration

Create or edit `.env` file in the project root:

```bash
# Enable automatic admin user provisioning
SEED_ADMIN_ENABLED=true

# Admin credentials (change these for production!)
ADMIN_EMAIL=admin@urguide.local
ADMIN_PASSWORD=Admin123!
ADMIN_FIRST_NAME=Admin
ADMIN_LAST_NAME=User
```

### Usage

```bash
# 1. Copy the example .env file
cp .env.example .env

# 2. Edit .env with your admin credentials
# (Use a text editor to change ADMIN_EMAIL, ADMIN_PASSWORD, etc.)

# 3. Start with Docker Compose
docker-compose up -d

# OR with development overrides
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# 4. Admin user is created automatically on first startup
# Check logs to confirm:
docker-compose logs api | grep "admin user"
```

### Environment Variables

The following environment variables control admin provisioning:

| Variable | Description | Default |
|----------|-------------|---------|
| `SEED_ADMIN_ENABLED` | Enable/disable auto-provisioning | `false` |
| `ADMIN_EMAIL` | Admin user email address | `admin@urguide.local` |
| `ADMIN_PASSWORD` | Admin user password | `Admin123!` |
| `ADMIN_FIRST_NAME` | Admin user first name | `Admin` |
| `ADMIN_LAST_NAME` | Admin user last name | `User` |

### How It Works

1. Environment variables from `.env` file are passed to the Docker container
2. ASP.NET Core configuration reads these variables (they override `appsettings.json`)
3. On startup, `AdminSeedingService` creates the admin user
4. Subsequent startups detect the existing user (no duplicates)

### Security Notes

- ⚠️ **Development**: Default credentials are acceptable for local development
- ⚠️ **Production**: Always use strong, unique passwords
- ⚠️ **Never commit** `.env` file to source control
- ⚠️ **Change password** immediately after first login in production
- ⚠️ **Disable provisioning** in production after initial setup (`SEED_ADMIN_ENABLED=false`)

See [DOCKER_QUICKSTART.md](../DOCKER_QUICKSTART.md) for complete Docker deployment guide.

## Option 2: Automatic Seeding (Development)

The application automatically seeds a default admin user when running in Development mode.

### Configuration

Edit `UrGuide.WebApp/appsettings.Development.json`:

```json
{
  "Seeding": {
    "SeedDefaultAdmin": true,
    "AdminEmail": "admin@urguide.local",
    "AdminPassword": "Admin123!",
    "AdminFirstName": "Admin",
    "AdminLastName": "User"
  }
}
```

### Usage

1. Set `SeedDefaultAdmin` to `true` in `appsettings.Development.json`
2. Configure the admin email, password, and name
3. Run the application: `dotnet run --project UrGuide.WebApp`
4. The admin user will be created automatically on startup

### Disabling Auto-Seeding

To disable automatic admin seeding, set `SeedDefaultAdmin` to `false` or remove the `Seeding` section from the configuration.

**Note**: When running with Docker Compose, environment variables from `.env` file take precedence over `appsettings.json` values.

## Option 3: PowerShell Script (Manual Provisioning)

Use the PowerShell script to manually create admin users without running the full application.

### Prerequisites

- PowerShell 5.1 or higher
- SQL Server PowerShell module (auto-installed if missing)
- Access to the SQL Server LocalDB or target database

### Usage

**Basic usage with defaults:**
```powershell
.\scripts\provision-admin-user.ps1
```

**Custom parameters:**
```powershell
.\scripts\provision-admin-user.ps1 `
    -Email "admin@example.com" `
    -Password "SecurePass123!" `
    -FirstName "John" `
    -LastName "Doe"
```

**Custom connection string:**
```powershell
.\scripts\provision-admin-user.ps1 `
    -Email "admin@example.com" `
    -Password "SecurePass123!" `
    -ConnectionString "Server=myserver;Database=urguide_id4;User Id=sa;Password=mypassword;"
```

### Default Values

- **Email**: `admin@urguide.local`
- **Password**: `Admin123!`
- **First Name**: `Admin`
- **Last Name**: `User`
- **Connection String**: `Server=(localdb)\mssqllocaldb;Database=urguide_id4;Trusted_Connection=True;`

### Important Notes

⚠️ **Security Warning**: The PowerShell script uses a simplified password hash for demonstration purposes. In production environments, you should:
- Use the automatic seeding method, or
- Change the password immediately after first login, or
- Use prop4r ASP.NET Identity password hashing

## Option 3: AdminSeedingService (Programmatic)

The `AdminSeedingService` provides a programmatic way to create admin users using ASP.NET Identity.

### Service Methods

- **`SeedDefaultAdminAsync()`** - Creates admin user from configuration
- **`SeedAdminUserAsync(email, password, firstName, lastName)`** - Creates admin user with specific credentials

### Usage in Code

```csharp
// Inject the service
private readonly IAdminSeedingService _adminSeedingService;

public MyService(IAdminSeedingService adminSeedingService)
{
    _adminSeedingService = adminSeedingService;
}

// Create admin user
await _adminSeedingService.SeedAdminUserAsync(
    "admin@example.com",
    "SecurePassword123!",
    "Admin",
    "User"
);
```

### Features

- Uses ASP.NET Identity for proper password hashing
- Automatically creates the "Admin" role if it doesn't exist
- Safely handles existing users (won't create duplicates)
- Ensures existing users have the Admin role
- Provides comprehensive logging

## Security Considerations

### Development Environment

- Default credentials are acceptable for local development
- The automatic seeding feature is only enabled in Development mode
- Credentials are stored in `appsettings.Development.json` (git-ignored)

### Production Environment

⚠️ **Never commit production credentials to source control!**

For production deployments:

1. **Disable automatic seeding** - Set `SeedDefaultAdmin` to `false` in production config
2. **Use strong passwords** - Follow your organization's password policy
3. **Secure connection strings** - Store in Azure Key Vault, AWS Secrets Manager, or environment variables
4. **Change default passwords** - Immediately change any default passwords after deployment
5. **Use environment variables** - Override configuration with environment variables:
   ```bash
   Seeding__AdminEmail="admin@production.com"
   Seeding__AdminPassword="VerySecurePassword123!"
   ```

## Troubleshooting

### "User already exists" Error

If you see this error, the user already exists in the database. You can:
- Use a different email address
- Delete the existing user from the database
- Use the application's password reset feature

### "Cannot connect to database" Error

Ensure that:
- SQL Server LocalDB is installed (for development)
- The connection string is correct
- The database exists (run migrations first: `dotnet ef database update`)

### "Admin role not found" Error

The scripts automatically create the "Admin" role if it doesn't exist. If you still see this error:
1. Check database permissions
2. Verify the connection string is correct
3. Ensure migrations have been applied

### Password Hash Not Working

The PowerShell script uses a simplified password hash. To fix:
1. Use the automatic seeding method (recommended)
2. Change the password via the application after first login
3. Use the `AdminSeedingService` for proper password hashing

## Examples

### Create Multiple Admin Users

```powershell
# Create primary admin
.\scripts\provision-admin-user.ps1 -Email "admin@urguide.com" -Password "Primary123!"

# Create backup admin
.\scripts\provision-admin-user.ps1 -Email "admin.backup@urguide.com" -Password "Backup123!"
```

### Create Admin for Testing

```powershell
.\scripts\provision-admin-user.ps1 `
    -Email "test.admin@urguide.local" `
    -Password "Test123!" `
    -FirstName "Test" `
    -LastName "Admin"
```

## Best Practices

1. **Use automatic seeding for development** - It's the easiest and most reliable method
2. **Change default passwords immediately** - Never use default credentials in production
3. **Document admin accounts** - Keep track of who has admin access
4. **Use strong passwords** - Follow OWASP password guidelines
5. **Enable 2FA** - Configure two-factor authentication for admin accounts
6. **Audit admin actions** - The platform logs all admin activities
7. **Principle of least privilege** - Only grant admin access when necessary

## Related Documentation

- [Admin Dashboard Guide](../docs/guides/ADMIN_DASHBOARD.md)
- [Security Best Practices](../docs/security/SECURITY_BEST_PRACTICES.md)
- [Deployment Guide](../docs/DEPLOYMENT.md)
- [2FA & Passkey Guide](../docs/guides/2FA_PASSKEY_GUIDE.md)

## Support

If you encounter issues with admin provisioning:
1. Check the troubleshooting section above
2. Review application logs in `logs/` directory
3. Open an issue on GitHub with error details
4. Contact the development team

---

**Last Updated**: January 2025  
**Maintainer**: UrGuide Development Team

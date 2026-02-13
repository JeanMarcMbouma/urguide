# CI/CD Pipeline Documentation

## Overview

The UrGuide API project includes a comprehensive CI/CD pipeline built with GitHub Actions. This document describes the workflows, their purposes, and how to use them effectively.

## Workflows

### 1. Main CI/CD Pipeline (`dotnet-ci.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual workflow dispatch

**Jobs:**

#### Build and Test
- Sets up .NET 8.0 SDK
- Caches NuGet packages for faster builds
- Restores dependencies
- Builds the application in Release configuration
- Runs tests (if test projects exist)
- Publishes test results
- Uploads build artifacts

#### Code Quality & Security Scan
- Initializes CodeQL for C# analysis
- Builds the project for analysis
- Performs comprehensive security scanning
- Reports vulnerabilities to GitHub Security tab

#### Dependency Check
- Scans all NuGet packages for known vulnerabilities
- Includes transitive dependencies
- Generates vulnerability report
- Uploads report as artifact (30-day retention)
- Fails on vulnerable packages (with continue-on-error)

#### Docker Build
- Sets up Docker Buildx for multi-platform support
- Builds Docker image using GitHub Actions cache
- Validates image builds successfully
- Does not push (push happens in docker-publish.yml)

#### Notification
- Runs after all jobs complete
- Reports overall pipeline status
- Fails if any critical job fails

**Environment Variables:**
```yaml
DOTNET_VERSION: '8.0.x'
BUILD_CONFIGURATION: 'Release'
SOLUTION_PATH: 'UrGuide.sln'
WEBAPP_PROJECT: 'UrGuide.WebApp/UrGuide.WebApp.csproj'
```

### 2. Docker Publishing (`docker-publish.yml`)

**Triggers:**
- Push to `main` branch
- Version tags (v*.*.*)
- Manual workflow dispatch

**Features:**
- Builds and publishes Docker images to GitHub Container Registry (ghcr.io)
- Automatic image tagging:
  - Branch name (e.g., `main`)
  - Semantic version (e.g., `1.0.0`, `1.0`)
  - Git SHA (e.g., `main-abc1234`)
  - `latest` for default branch
- Generates build provenance attestation
- Uses GitHub Actions cache for faster builds

**Image Location:**
```
ghcr.io/jeanmarcmbouma/urguide:latest
ghcr.io/jeanmarcmbouma/urguide:main
ghcr.io/jeanmarcmbouma/urguide:v1.0.0
```

### 3. Migration Validation (`migration-validation.yml`)

**Triggers:**
- Pull requests that modify:
  - `**/Migrations/**` files
  - `**/Data/**` files
  - `appsettings*.json` files
- Manual workflow dispatch

**Features:**
- Spins up SQL Server 2022 in GitHub Actions
- Installs EF Core tools
- Applies migrations to test databases
- Generates idempotent SQL migration scripts
- Validates migrations before merging
- Uploads migration scripts as artifacts

**Test Databases:**
- `urguide_data_test` - Main application database
- `urguide_id4_test` - IdentityServer database

## Pipeline Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Push/PR Event                         │
└─────────────────────┬───────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│              dotnet-ci.yml (Main Pipeline)               │
├─────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Build & Test │  │Code Quality  │  │  Dependency  │  │
│  │              │  │   (CodeQL)   │  │    Check     │  │
│  └──────┬───────┘  └──────────────┘  └──────────────┘  │
│         │                                                │
│         ▼                                                │
│  ┌──────────────┐                                       │
│  │Docker Build  │                                       │
│  └──────┬───────┘                                       │
│         │                                                │
│         ▼                                                │
│  ┌──────────────┐                                       │
│  │   Notify     │                                       │
│  └──────────────┘                                       │
└─────────────────────────────────────────────────────────┘
                      │
                      ▼ (on main branch push)
┌─────────────────────────────────────────────────────────┐
│         docker-publish.yml (Container Registry)          │
└─────────────────────────────────────────────────────────┘
```

## Usage Guide

### For Developers

#### Running Builds Locally

**Build the project:**
```bash
dotnet restore UrGuide.WebApp/UrGuide.WebApp.csproj
dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj --configuration Release
```

**Run tests:**
```bash
dotnet test --configuration Release
```

**Build Docker image:**
```bash
docker build -t urguide-api:local .
```

#### Pull Request Workflow

1. Create a feature branch
2. Make your changes
3. Push to GitHub
4. Create a pull request to `develop` or `main`
5. CI pipeline runs automatically:
   - ✅ Build must pass
   - ✅ Tests must pass (if any)
   - ✅ Security scans must pass
   - ✅ Docker build must succeed

#### Working with Migrations

1. Add a new migration:
   ```bash
   dotnet ef migrations add YourMigrationName --project UrGuide.WebApp
   ```

2. Commit and push the migration files
3. Migration validation workflow runs automatically
4. Review migration script in artifacts

### For DevOps/Admins

#### Deploying to Production

**Using Docker (Recommended):**
```bash
# Pull latest image
docker pull ghcr.io/jeanmarcmbouma/urguide:latest

# Run with environment variables
docker run -d \
  -p 80:80 \
  -e ConnectionStrings__DefaultConnection="Server=prod-db;..." \
  -e ConnectionStrings__AuthConnection="Server=prod-db;..." \
  -e ASPNETCORE_ENVIRONMENT=Production \
  ghcr.io/jeanmarcmbouma/urguide:latest
```

**Using published artifacts:**
1. Download build artifacts from successful workflow run
2. Extract to deployment directory
3. Configure `appsettings.Production.json`
4. Run migrations:
   ```bash
   dotnet ef database update --project UrGuide.WebApp
   ```
5. Start the application

#### Manual Workflow Trigger

All workflows support manual triggering via GitHub Actions UI:
1. Go to Actions tab
2. Select the workflow
3. Click "Run workflow"
4. Choose branch and run

#### Monitoring Builds

- **GitHub Actions Tab**: View all workflow runs
- **Security Tab**: View CodeQL findings
- **Packages**: View published Docker images
- **Artifacts**: Download build outputs, test results, migration scripts

## Security Features

### CodeQL Analysis
- Scans C# code for security vulnerabilities
- Runs on every push and PR
- Results visible in Security → Code scanning alerts

### Dependency Scanning
- Checks NuGet packages for known vulnerabilities
- Includes transitive dependencies
- Generates detailed reports
- Continues on error to avoid blocking PRs (warning only)

### Container Security
- Multi-stage Docker builds minimize attack surface
- Non-root user execution (using pre-defined 'app' user)
- Debian-based images for compatibility
- Health checks for monitoring

## Performance Optimizations

### Caching Strategy
1. **NuGet Packages**: Cached between runs (~3-5 minutes saved)
2. **Docker Layers**: GitHub Actions cache mode (`type=gha`)
3. **Build Artifacts**: Reused between jobs

### Parallel Execution
- Build & Test, Code Quality, and Dependency Check run in parallel
- Reduces total pipeline time by ~40%

### Conditional Execution
- Migration validation only runs when relevant files change
- Docker publish only runs on main branch or tags

## Troubleshooting

### Build Failures

**"dotnet restore failed"**
- Check network connectivity
- Verify package sources in NuGet.config
- Clear NuGet cache: `dotnet nuget locals all --clear`

**"Docker build failed"**
- Check Dockerfile syntax
- Verify base image availability
- Review .dockerignore to ensure needed files aren't excluded

### Test Failures

**"Tests not found"**
- Ensure test projects follow naming convention: `*.Tests.csproj`
- Verify test projects are included in solution

### Security Scan Failures

**"CodeQL initialization failed"**
- Ensure code compiles successfully first
- Check for C# syntax errors

**"Vulnerable packages detected"**
- Review vulnerability report artifact
- Update affected packages
- Or suppress if false positive

### Migration Failures

**"Migration validation failed"**
- Check SQL Server availability in Actions
- Verify connection string format
- Test migrations locally first
- Review DbContext configuration

## Best Practices

### Commit Messages
Use conventional commits for better changelog generation:
- `feat:` New feature
- `fix:` Bug fix
- `chore:` Maintenance
- `docs:` Documentation
- `test:` Tests
- `ci:` CI/CD changes

### Branch Strategy
- `main`: Production-ready code
- `develop`: Integration branch
- `feature/*`: Feature development
- `hotfix/*`: Production fixes

### Versioning
Use semantic versioning for tags:
```bash
git tag v1.0.0
git push origin v1.0.0
```

Triggers Docker image with version tags.

### Secret Management
Store sensitive data in GitHub Secrets:
- Navigate to Settings → Secrets → Actions
- Add secrets (never commit credentials)
- Reference in workflows: `${{ secrets.SECRET_NAME }}`

## Extending the Pipeline

### Adding New Jobs

Edit workflow files in `.github/workflows/`:

```yaml
new-job:
  name: My New Job
  runs-on: ubuntu-latest
  needs: [build-and-test]  # Runs after build-and-test
  steps:
    - uses: actions/checkout@v4
    - name: My Step
      run: echo "Hello World"
```

### Adding Deployment Stages

Create new workflow file for production deployment:

```yaml
name: Deploy to Production
on:
  push:
    tags:
      - 'v*.*.*'
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Azure
        # ... deployment steps
```

### Custom Notifications

Add notification steps to workflows:
- Slack notifications
- Email notifications
- GitHub Discussions
- Microsoft Teams webhooks

## Support

For issues with CI/CD pipeline:
1. Check workflow run logs in Actions tab
2. Review this documentation
3. Create issue in GitHub repository
4. Tag with `ci/cd` or `devops` label

---

**Last Updated:** 2024
**Maintained By:** UrGuide Development Team

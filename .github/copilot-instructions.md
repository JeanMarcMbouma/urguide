# UrGuide Tourism Platform
UrGuide is a tourism web application with .NET 8 backend API, React 18 SPA frontend, and .NET MAUI mobile apps. Users can create guide profiles, bid on tours, join tours, and leave reviews.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Initial Setup and Dependencies
- Install .NET 8 SDK (required for building and running):
  - **Use the official Microsoft installation for .NET 8:** https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify installation: `dotnet --version` should show 8.0.x
- Install .NET Aspire workload: `dotnet workload install aspire` -- takes 20+ seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- Restore .NET packages: `dotnet restore UrGuide.WebApp/UrGuide.WebApp.csproj` -- takes 62 seconds. NEVER CANCEL. Set timeout to 120+ seconds.
- Install Node dependencies: `cd UrGuide.WebApp/ClientApp && npm install --legacy-peer-deps` -- takes 138 seconds (fresh), 2 seconds (cached). NEVER CANCEL. Set timeout to 300+ seconds for fresh installs.

### Building the Project
- Build .NET web project: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj` -- takes 247 seconds (4+ minutes). NEVER CANCEL. Set timeout to 300+ seconds.
- Full solution build fails: `dotnet build` -- fails due to missing Aspire workload requirement for UrGuide.AppHost project.
- DO NOT attempt React production build (`npm run build`) - fails due to incomplete Material-UI migration from @material-ui to @mui packages.
- DO NOT attempt React dev server (`npm start`) - fails to compile due to incomplete Material-UI migration and React Router v6 breaking changes.
- DO NOT attempt .NET Aspire AppHost (`dotnet run --project UrGuide.AppHost`) - requires compatible Aspire workload version.

### Testing
- DO NOT run React tests (`npm test`) - fails due to incomplete Material-UI migration from @material-ui to @mui packages.
- DO NOT run `npm run lint` - ESLint configuration uses deprecated version and has compatibility issues.
- No .NET test projects found in solution.
- **CRITICAL**: Material-UI migration is incomplete - many components still import from `@material-ui/core` instead of `@mui/material`.

### Running the Application
- **CRITICAL**: The application requires SQL Server LocalDB which is not available in Linux environments.
- Running `dotnet run --project UrGuide.WebApp` will fail with `System.PlatformNotSupportedException: LocalDB is not supported on this platform`.
- .NET Aspire orchestration (`dotnet run --project UrGuide.AppHost`) currently has workload compatibility issues.
- Development workflow should focus on .NET backend compilation and React frontend syntax validation only.
- Full application testing requires Windows environment with SQL Server LocalDB or modified connection strings for alternative databases.

## Validation
- ALWAYS build the .NET web project after making backend changes: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`.
- Frontend validation is severely limited due to incomplete Material-UI migration - focus on ensuring React components use correct @mui imports.
- You cannot run the full application due to SQL Server LocalDB requirement on Linux.
- You cannot run React dev server or tests due to Material-UI migration issues.
- Manual validation should focus on .NET project compilation and careful code review for React components.
- **VALIDATION SCENARIO**: After any changes, run `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj` to ensure backend compiles successfully.

## Common Issues and Workarounds

## Common Issues and Workarounds

### Node.js and React Issues
- **npm install fails**: Use `npm install --legacy-peer-deps` to resolve dependency conflicts.
- **Material-UI migration incomplete**: Frontend still uses old `@material-ui/core`, `@material-ui/icons`, `@material-ui/lab` imports instead of `@mui/material`, `@mui/icons-material`, `@mui/lab`. All React dev/build/test commands fail until migration is complete.
- **React Router v6 breaking changes**: Code uses deprecated APIs like `Switch` (now `Routes`) and `useRouteMatch` (now `useMatch`).
- **Node.js polyfill issues**: Webpack errors about missing Node.js core modules like 'timers'.
- **ESLint deprecated**: Uses deprecated ESLint 8.x with compatibility issues. Skip linting validation.

### .NET Issues  
- **Full solution build fails**: Missing Aspire workload for AppHost project. Build web project only: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`.
- **Aspire workload compatibility**: .NET Aspire AppHost requires specific workload versions. Use `dotnet workload install aspire` but AppHost build may still fail.
- **Database connectivity**: Uses SQL Server LocalDB connection strings. Application cannot run without Windows + SQL Server LocalDB or connection string modifications.
- **Runtime platform**: Application will throw `System.PlatformNotSupportedException: LocalDB is not supported` on Linux/macOS.

### Security Warnings
- Multiple NuGet package security vulnerabilities in dependencies (SixLabors.ImageSharp, Duende.IdentityServer).
- 9 npm package vulnerabilities (3 moderate, 6 high) - run `npm audit` for details.
- Deprecated ESLint version with security concerns.
- .NET 8 is supported, but dependencies have known vulnerabilities that should be updated.

## Project Structure

### Repository Layout
```
UrGuide.slnx                # Main solution file (XML format)
├── UrGuide.WebApp/         # Main web application (ASP.NET Core 8.0 + React 18 SPA)
│   ├── ClientApp/          # React 18 frontend application (Material-UI v5 migration incomplete)
│   ├── Controllers/        # API controllers  
│   ├── Data/              # Entity Framework data context
│   ├── Migrations/        # EF database migrations
│   └── Program.cs         # App configuration and DI setup (.NET 8 minimal hosting)
├── UrGuide.AppHost/        # .NET Aspire orchestration host
├── UrGuide.ServiceDefaults/ # .NET Aspire service defaults
├── UrGuide.Data/          # Entity Framework data layer
├── UrGuide.Services/      # Business logic services
├── UrGuide.Model/         # Domain models and DTOs
├── UrGuide.Core/          # Core utilities and extensions
├── UrGuide.Shared/        # Shared code between projects
├── UrGuide.MAUI/          # .NET MAUI mobile app
```

### Key Configuration Files
- `UrGuide.WebApp/appsettings.json` - Main app configuration
- `UrGuide.WebApp/ClientApp/package.json` - React dependencies and scripts
- `UrGuide.WebApp/UrGuide.WebApp.csproj` - .NET project configuration
- `UrGuide.slnx` - Solution file with all projects (XML format)

### Database Configuration
- Uses Entity Framework Core with SQL Server LocalDB
- Connection strings in appsettings.json target `(localdb)\mssqllocaldb`
- Two databases: `urguide_id4` (IdentityServer) and `urguide_data` (application data)
- Database migrations located in `UrGuide.WebApp/Migrations/`

## Technology Stack

### Backend (.NET 8)
- **Web Framework**: ASP.NET Core 8.0 with minimal APIs and MVC
- **Authentication**: Duende IdentityServer 7.0 with ASP.NET Core Identity
- **Database**: Entity Framework Core 8.0 with SQL Server
- **Real-time**: SignalR for live updates
- **Validation**: FluentValidation
- **Documentation**: Swagger/OpenAPI
- **Orchestration**: .NET Aspire for cloud-native deployment

### Frontend (React 18 SPA)
- **Framework**: React 18 with React Router v6
- **UI Library**: Material-UI v5 (@mui/material) - **MIGRATION INCOMPLETE**
- **Build Tool**: Create React App (react-scripts 5.0.1)
- **State Management**: React Context API
- **HTTP Client**: Fetch API with custom wrappers
- **Authentication**: OIDC Client TS integration

### Mobile (.NET MAUI)
- **.NET MAUI**: Cross-platform mobile app (UrGuide.MAUI) 
- **Platforms**: Android and iOS support
- **API Client**: Auto-generated from OpenAPI specification
- **Note**: Legacy Xamarin projects have been removed from the solution

## Development Workflow

### Making Frontend Changes
1. Navigate to React app: `cd UrGuide.WebApp/ClientApp`
2. Install dependencies if needed: `npm install --legacy-peer-deps`
3. Make changes to files in `src/` directory
4. **CRITICAL**: When editing React components, ensure imports use `@mui/material` instead of `@material-ui/core`
5. **CRITICAL**: When using React Router, use v6 API (`Routes` instead of `Switch`, `useMatch` instead of `useRouteMatch`)
6. Verify syntax through careful code review (dev server and tests cannot run due to migration issues)

### Making Backend Changes
1. Make changes to C# files in any project
2. Build to verify: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`
3. Fix any compilation errors
4. For database changes, add EF migrations: `dotnet ef migrations add MigrationName --project UrGuide.WebApp`

### Key Development Files
- **Main React App**: `UrGuide.WebApp/ClientApp/src/App.js`
- **API Controllers**: `UrGuide.WebApp/Controllers/`
- **Business Services**: `UrGuide.Services/`
- **Data Models**: `UrGuide.Model/`
- **Database Context**: `UrGuide.Data/UrGuideContext.cs`

## Critical Reminders
- **NEVER CANCEL long-running builds** - .NET build takes 4+ minutes (247 seconds), npm install takes 2+ minutes (138 seconds fresh). Set appropriate timeouts.
- **ALWAYS use --legacy-peer-deps** for npm install commands
- **USE web project build primarily** - `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj` for faster builds (full solution build fails due to missing Aspire workload)
- **DO NOT attempt React builds/dev server/tests** - they all fail due to incomplete Material-UI migration
- **DO NOT attempt to run the application** - requires SQL Server LocalDB not available on Linux
- **FOCUS on .NET backend development and careful React code review** for validation workflow
- **MATERIAL-UI MIGRATION**: Frontend code still uses old `@material-ui/*` imports instead of `@mui/*` packages

## Documentation Requirements
- **ALWAYS update README.md** after implementing new features or making significant changes
- **Update the roadmap sections** in README.md when features move from planned to implemented
- **Create GitHub issues** for new feature requests with appropriate labels (enhancement, feature, nice-to-have)
- **Document breaking changes** and migration steps when updating dependencies or frameworks
- **Keep the technology stack section current** when adding new libraries or changing versions
- **Organized documentation structure**: All documentation is in the `docs/` directory:
  - `docs/guides/` - User and integration guides (2FA, Webhooks, etc.)
  - `docs/implementation/` - Technical architecture and implementation details
  - `docs/cicd/` - Continuous integration and deployment documentation
  - `docs/security/` - Security audits and best practices
  - `docs/planning/` - Feature requests and roadmap (ISSUES_CATALOG.md)
- **Update docs/README.md** when adding new documentation files to maintain the index
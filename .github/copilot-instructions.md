# UrGuide Tourism Platform
UrGuide is a tourism web application with .NET Core 3.1 backend API, React SPA frontend, and Xamarin mobile apps. Users can create guide profiles, bid on tours, join tours, and leave reviews.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Initial Setup and Dependencies
- Install .NET Core 3.1 SDK and runtime (required for building and running):
  - **Follow the official Microsoft installation instructions for .NET Core 3.1:**  
    https://docs.microsoft.com/en-us/dotnet/core/install/
  - *Do not download and execute installation scripts from the internet without verifying their integrity.*
- Restore .NET packages: `dotnet restore` -- takes 65 seconds. NEVER CANCEL. Set timeout to 120+ seconds.
- Install Node dependencies: `cd UrGuide.WebApp/ClientApp && npm install --legacy-peer-deps` -- takes 4.5 minutes. NEVER CANCEL. Set timeout to 10+ minutes.

### Building the Project
- Build .NET web project only: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj` -- takes 2.5 seconds.
- React development server: `cd UrGuide.WebApp/ClientApp && NODE_OPTIONS="--openssl-legacy-provider" npm start` -- takes 30+ seconds to start. NEVER CANCEL. Set timeout to 120+ seconds.
- DO NOT attempt to build the full solution (`dotnet build`) - fails due to missing Xamarin Android targets.
- DO NOT attempt React production build (`npm run build`) - fails due to TypeScript compatibility issues with Node.js 20+.
- DO NOT attempt React dev server (`npm start`) - starts but fails to compile due to TypeScript @types/babel__traverse compatibility issues.

### Testing
- React tests: `cd UrGuide.WebApp/ClientApp && NODE_OPTIONS="--openssl-legacy-provider" npm test` -- takes 8 seconds. Tests pass but exits with code 1 due to React warnings.
- DO NOT run `npm run lint` - ESLint configuration is incompatible with current dependencies.
- No .NET test projects found in solution.

### Running the Application
- The application requires SQL Server LocalDB which is not available in Linux environments.
- Running `dotnet run` requires both .NET 3.1 SDK and database connectivity - will fail in most development environments.
- Development workflow should focus on the React frontend using `npm start` for UI changes and `dotnet build` for backend validation.
- Full application testing requires Windows environment with SQL Server LocalDB or modified connection strings for alternative databases.

## Validation
- ALWAYS build the .NET web project after making backend changes: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`.
- Frontend validation is limited due to compilation issues - focus on ensuring React components are syntactically correct.
- You cannot run the full application or React dev server due to TypeScript compatibility and database constraints.
- Manual validation should focus on .NET project compilation and code review for React components.

## Common Issues and Workarounds

### Node.js and React Issues
- **npm install fails**: Use `npm install --legacy-peer-deps` to resolve dependency conflicts.
- **React build/start fails**: Use `NODE_OPTIONS="--openssl-legacy-provider"` environment variable due to Node.js 20+ OpenSSL changes. However, compilation still fails due to TypeScript compatibility issues.
- **TypeScript compilation errors**: Both development and production builds fail due to @types/babel__traverse compatibility issues with TypeScript. No current workaround available.
- **ESLint configuration errors**: Linting is broken due to configuration incompatibilities. Skip linting validation.

### .NET Issues  
- **Full solution build fails**: Xamarin Android project missing targets. Build web project only: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`.
- **SDK/Runtime errors**: Application targets .NET Core 3.1. Install complete .NET 3.1 SDK for building: `./dotnet-install.sh --channel 3.1`.
- **Database connectivity**: Uses SQL Server LocalDB connection strings. Application cannot run without database setup or connection string modifications.

### Security Warnings
- Multiple NuGet package security vulnerabilities in dependencies (SixLabors.ImageSharp, Newtonsoft.Json).
- Target framework (.NET Core 3.1) is out of support and no longer receives security updates.

## Project Structure

### Repository Layout
```
UrGuide.sln                 # Main solution file
├── UrGuide.WebApp/         # Main web application (ASP.NET Core 3.1 + React SPA)
│   ├── ClientApp/          # React frontend application
│   ├── Controllers/        # API controllers  
│   ├── Data/              # Entity Framework data context
│   ├── Migrations/        # EF database migrations
│   └── Startup.cs         # App configuration and DI setup
├── UrGuide.Data/          # Entity Framework data layer
├── UrGuide.Services/      # Business logic services
├── UrGuide.Model/         # Domain models and DTOs
├── UrGuide.Core/          # Core utilities and extensions
├── UrGuide.Shared/        # Shared code between projects
└── Mobile/                # Xamarin mobile applications
    ├── UrGuide.Mobile/    # Shared mobile code
    ├── UrGuide.Mobile.Android/
    └── UrGuide.Mobile.iOS/
```

### Key Configuration Files
- `UrGuide.WebApp/appsettings.json` - Main app configuration
- `UrGuide.WebApp/ClientApp/package.json` - React dependencies and scripts
- `UrGuide.WebApp/UrGuide.WebApp.csproj` - .NET project configuration
- `UrGuide.sln` - Solution file with all projects

### Database Configuration
- Uses Entity Framework Core with SQL Server LocalDB
- Connection strings in appsettings.json target `(localdb)\mssqllocaldb`
- Two databases: `urguide_id4` (IdentityServer) and `urguide_data` (application data)
- Database migrations located in `UrGuide.WebApp/Migrations/`

## Technology Stack

### Backend (.NET Core 3.1)
- **Web Framework**: ASP.NET Core 3.1 with MVC and Web API
- **Authentication**: IdentityServer4 with ASP.NET Core Identity
- **Database**: Entity Framework Core with SQL Server
- **Real-time**: SignalR for live updates
- **Validation**: FluentValidation
- **Documentation**: Swagger/OpenAPI

### Frontend (React SPA)
- **Framework**: React 16.13 with React Router
- **UI Library**: Material-UI 4.x components
- **Build Tool**: Create React App (react-scripts 3.4.1)
- **State Management**: React Context API
- **HTTP Client**: Fetch API with custom wrappers
- **Authentication**: OIDC Client integration

### Mobile (Xamarin)
- **Framework**: Xamarin.Forms shared project
- **Platforms**: Android and iOS native projects
- **API Client**: Auto-generated from OpenAPI specification

## Development Workflow

### Making Frontend Changes
1. Navigate to React app: `cd UrGuide.WebApp/ClientApp`
2. Install dependencies if needed: `npm install --legacy-peer-deps`
3. Make changes to files in `src/` directory
4. Verify syntax and structure through code review (dev server compilation fails due to TypeScript issues)
5. Consider running tests: `NODE_OPTIONS="--openssl-legacy-provider" npm test`

### Making Backend Changes
1. Make changes to C# files in any project
2. Build to verify: `dotnet build UrGuide.WebApp/UrGuide.WebApp.csproj`
3. Fix any compilation errors
4. For database changes, add EF migrations: `dotnet ef migrations add MigrationName`

### Key Development Files
- **Main React App**: `UrGuide.WebApp/ClientApp/src/App.js`
- **API Controllers**: `UrGuide.WebApp/Controllers/`
- **Business Services**: `UrGuide.Services/`
- **Data Models**: `UrGuide.Model/`
- **Database Context**: `UrGuide.Data/UrGuideContext.cs`

## Critical Reminders
- **NEVER CANCEL long-running builds** - npm install takes 4.5+ minutes, set 10+ minute timeouts
- **ALWAYS use --legacy-peer-deps** for npm install commands
- **ALWAYS use NODE_OPTIONS="--openssl-legacy-provider"** for React commands
- **DO NOT build full solution** - build web project only to avoid Xamarin errors
- **DO NOT attempt production React builds** - they fail due to TypeScript issues
- **DO NOT attempt React dev server** - starts but fails to compile due to TypeScript issues
- **Focus on .NET backend development and code review** for validation workflow
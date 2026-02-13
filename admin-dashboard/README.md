# UrGuide Admin Dashboard

Modern admin dashboard for UrGuide Tourism Platform built with React 18, TypeScript, and Vite.

## 🚀 Technology Stack

- **React 18.3** - Modern React with hooks and concurrent features
- **TypeScript 5.7** - Type-safe development
- **Vite 6.0** - Lightning-fast build tool and dev server
- **Material-UI (MUI) v6** - Comprehensive component library
- **MUI X Data Grid** - Advanced data table with pagination, sorting, filtering
- **TanStack Query v5** - Powerful server state management
- **React Router v6** - Client-side routing
- **Axios** - HTTP client for API calls

## 📁 Project Structure

```
admin-dashboard/
├── src/
│   ├── main.tsx              # Application entry point
│   ├── App.tsx               # Root component with routing
│   ├── pages/                # Page components
│   │   ├── Login.tsx         # Admin login with 2FA
│   │   ├── Dashboard.tsx     # Overview and statistics
│   │   ├── UserList.tsx      # User management table
│   │   ├── UserDetail.tsx    # Individual user detail page
│   │   └── ActivityLog.tsx   # Audit trail viewer
│   ├── components/           # Reusable components
│   │   ├── Layout/           # Layout components
│   │   └── shared/           # Shared UI components
│   ├── services/             # API services
│   │   ├── adminApi.ts       # Admin API client
│   │   └── authService.ts    # Authentication service
│   ├── types/                # TypeScript type definitions
│   │   └── admin.types.ts    # Admin-related types
│   ├── hooks/                # Custom React hooks
│   │   └── useAuth.ts        # Authentication hook
│   └── styles/               # Global styles
├── public/                   # Static assets
├── index.html                # HTML template
├── vite.config.ts            # Vite configuration
├── tsconfig.json             # TypeScript configuration
└── package.json              # Dependencies and scripts
```

## 🛠️ Development Setup

### Prerequisites
- Node.js 18+ and npm/yarn/pnpm
- UrGuide API running on `https://localhost:5001`
- Or Docker and Docker Compose

### Installation

#### Option 1: Local Development (Recommended for Dev)
```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Access dashboard at http://localhost:3001
```

#### Option 2: Docker (Recommended for Production)
```bash
# From project root
docker-compose up -d admin-dashboard

# Access dashboard at http://localhost:3001
```

See [DOCKER.md](DOCKER.md) for comprehensive Docker documentation.

### Available Scripts
- `npm run dev` - Start development server with hot reload
- `npm run build` - Build production bundle
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint for code quality

## 🔐 Authentication

The dashboard integrates with UrGuide's existing authentication system:

1. **Admin Login** - OAuth 2.0/OpenID Connect with Duende IdentityServer
2. **JWT Bearer Tokens** - Stored in localStorage/sessionStorage
3. **2FA Verification** - TOTP integration with existing 2FA system
4. **Role-based Access** - Requires `Admin` role claim in JWT token

## 📡 API Integration

All API calls proxy through Vite dev server to backend:
- **Dev**: `http://localhost:3001/api` → `https://localhost:5001/api`
- **Production**: Configure `VITE_API_URL` environment variable

### API Endpoints Used
- `GET /api/admin/users` - List users with pagination and search
- `GET /api/admin/users/{id}` - Get user details
- `POST /api/admin/users/{id}/suspend` - Suspend user account
- `POST /api/admin/users/{id}/activate` - Activate user account
- `DELETE /api/admin/users/{id}` - Delete user
- `PUT /api/admin/users/roles` - Update user roles
- `GET /api/admin/users/{id}/activity` - Get user activity log
- `GET /api/admin/roles` - List available roles

See [Admin API Documentation](../docs/implementation/ADMIN_API_DOCUMENTATION.md) for complete API reference.

## 🎨 Features

### User Management
- ✅ Paginated user list with search
- ✅ Advanced filtering (role, verification status, lockout status)
- ✅ User detail view with profile information
- ✅ Account actions: Suspend, Activate, Delete
- ✅ Role assignment with multi-select
- ✅ Bulk operations for multiple users

### Activity Monitoring
- ✅ User activity audit trail
- ✅ Login attempt tracking
- ✅ Action history with timestamps and IP addresses
- ✅ Filterable activity log

### Dashboard Overview
- ⏳ User statistics (total, active, guides)
- ⏳ Activity metrics (logins, registrations)
- ⏳ Recent activity feed
- ⏳ Quick actions panel

## 🔒 Security Considerations

- All API requests include JWT Bearer token in Authorization header
- Admin role validation on protected routes
- Confirmation dialogs for destructive actions (suspend, delete)
- XSS protection through React's built-in escaping
- CSRF protection via SameSite cookies
- Rate limiting on API endpoints

## 🚀 Deployment

### Build for Production
```bash
npm run build
# Output: dist/ folder
```

### Deployment Options

1. **Static Hosting** (Netlify, Vercel, Cloudflare Pages)
   - Deploy `dist/` folder
   - Configure environment variables for API URL

2. **Embed in ASP.NET Core** (Recommended)
   - Copy `dist/` to `UrGuide.WebApp/wwwroot/admin/`
   - Configure route in Program.cs to serve static files

3. **Docker Container**
   - Use nginx to serve static files
   - Configure API proxy in nginx.conf

## 📝 Development Guidelines

### Code Style
- Use functional components with hooks
- Prefer TypeScript strict mode
- Follow MUI theming conventions
- Use TanStack Query for all API calls
- Implement proper error boundaries

### State Management
- **Server State**: TanStack Query (users, roles, activity)
- **Client State**: React Context (auth, theme)
- **Form State**: Controlled components with useState

### Testing (Future)
- Unit tests with Vitest
- Component tests with React Testing Library
- E2E tests with Playwright

## 🤝 Contributing

1. Follow existing code structure and patterns
2. Add TypeScript types for all data structures
3. Update documentation for new features
4. Test thoroughly before committing

## 📄 License

Same as UrGuide platform - see parent LICENSE file.

## 🔗 Related Documentation

- [Admin API Documentation](../docs/implementation/ADMIN_API_DOCUMENTATION.md)
- [Main Project README](../README.md)
- [Issues Catalog](../docs/planning/ISSUES_CATALOG.md)

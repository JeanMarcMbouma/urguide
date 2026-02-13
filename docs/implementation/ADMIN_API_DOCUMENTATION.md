# Admin API Documentation

## Overview
Backend implementation for Issue #19 - Admin Dashboard Authentication & User Management. Provides authenticated endpoints for administrators to manage users, roles, and view activity logs.

## Status
✅ **Backend Complete** - All models, services, and controllers implemented and compiling successfully  
⏳ **Frontend Pending** - React dashboard implementation needed

## Authentication
All endpoints require:
- `[Authorize(Roles = "Admin")]` - User must be authenticated and have Admin role
- JWT Bearer token in Authorization header: `Authorization: Bearer {token}`

## Base URL
```
/api/admin
```

## Endpoints

### 1. Get All Users (Paginated)
**GET** `/api/admin/users`

Returns paginated list of users with search capability.

**Query Parameters:**
- `PageNumber` (optional, default: 1) - Page number for pagination
- `PageSize` (optional, default: 20) - Number of items per page
- `Term` (optional) - Search term to filter by email, first name, or last name

**Response:** `200 OK`
```json
{
  "items": [
    {
      "id": "string",
      "email": "string",
      "firstName": "string",
      "lastName": "string",
      "isGuide": true,
      "emailConfirmed": true,
      "twoFactorEnabled": true,
      "lockoutEnabled": true,
      "lockoutEnd": "2025-01-19T12:00:00Z",
      "accessFailedCount": 0,
      "phoneNumber": "string",
      "roles": ["User", "Guide"],
      "postCount": 5,
      "tourCount": 3
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 100,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

**Example:**
```bash
GET /api/admin/users?PageNumber=1&PageSize=20&Term=john
```

---

### 2. Get User Details
**GET** `/api/admin/users/{userId}`

Returns detailed information about a specific user.

**Path Parameters:**
- `userId` (required) - User ID (GUID)

**Response:** `200 OK`
```json
{
  "id": "string",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "isGuide": true,
  "emailConfirmed": true,
  "twoFactorEnabled": true,
  "lockoutEnabled": true,
  "lockoutEnd": null,
  "accessFailedCount": 0,
  "phoneNumber": "+1234567890",
  "roles": ["User", "Guide"],
  "postCount": 10,
  "tourCount": 5
}
```

**Error Response:** `404 Not Found`
```json
{
  "errors": ["User not found"]
}
```

---

### 3. Suspend User Account
**POST** `/api/admin/users/{userId}/suspend`

Locks out a user account for a specified duration. User won't be able to login during suspension.

**Path Parameters:**
- `userId` (required) - User ID (GUID)

**Query Parameters:**
- `durationDays` (optional, default: 30) - Number of days to suspend the account

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "User suspended successfully"
}
```

**Error Response:** `404 Not Found`
```json
{
  "errors": ["User not found"]
}
```

**Example:**
```bash
POST /api/admin/users/12345678-1234-1234-1234-123456789012/suspend?durationDays=7
```

---

### 4. Activate User Account
**POST** `/api/admin/users/{userId}/activate`

Removes lockout from a user account, allowing them to login.

**Path Parameters:**
- `userId` (required) - User ID (GUID)

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "User activated successfully"
}
```

**Error Response:** `404 Not Found`
```json
{
  "errors": ["User not found"]
}
```

---

### 5. Delete User Account
**DELETE** `/api/admin/users/{userId}`

**⚠️ PERMANENT ACTION** - Deletes a user account and all associated data.

**Path Parameters:**
- `userId` (required) - User ID (GUID)

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "User deleted successfully"
}
```

**Error Response:** `404 Not Found`
```json
{
  "errors": ["User not found"]
}
```

---

### 6. Update User Roles
**PUT** `/api/admin/users/roles`

Updates the roles assigned to a user. Removes all current roles and assigns new ones.

**Request Body:**
```json
{
  "userId": "string",
  "roles": ["User", "Guide", "Admin"]
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "User roles updated successfully"
}
```

**Error Responses:**

`404 Not Found` - User doesn't exist
```json
{
  "errors": ["User not found"]
}
```

`400 Bad Request` - Invalid role name
```json
{
  "errors": ["Role 'InvalidRole' does not exist"]
}
```

---

### 7. Get User Activity Log
**GET** `/api/admin/users/{userId}/activity`

Returns audit trail of user actions (login attempts, tours, bookings, etc).

**Path Parameters:**
- `userId` (required) - User ID (GUID)

**Query Parameters:**
- `PageNumber` (optional, default: 1) - Page number for pagination
- `PageSize` (optional, default: 50) - Number of items per page

**Response:** `200 OK`
```json
{
  "items": [
    {
      "userId": "string",
      "actionType": "UserLogin",
      "description": "User logged in successfully",
      "timestamp": "2025-01-19T10:30:00Z",
      "ipAddress": "192.168.1.100"
    }
  ],
  "pageNumber": 1,
  "pageSize": 50,
  "totalCount": 150,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

### 8. Get All Available Roles
**GET** `/api/admin/roles`

Returns list of all role names in the system.

**Response:** `200 OK`
```json
["User", "Guide", "Admin"]
```

---

## Data Models

### AdminUserInfo
Extended user information for admin dashboard display.

```csharp
public class AdminUserInfo
{
    public string Id { get; set; }                    // User ID (GUID)
    public string Email { get; set; }                 // Email address
    public string FirstName { get; set; }             // First name
    public string LastName { get; set; }              // Last name
    public bool IsGuide { get; set; }                 // Whether user is a guide
    public bool EmailConfirmed { get; set; }          // Email verification status
    public bool TwoFactorEnabled { get; set; }        // 2FA enabled flag
    public bool LockoutEnabled { get; set; }          // Can be locked out
    public DateTimeOffset? LockoutEnd { get; set; }   // When lockout expires (null = not locked)
    public int AccessFailedCount { get; set; }        // Failed login attempts
    public string? PhoneNumber { get; set; }          // Phone number
    public List<string> Roles { get; set; }           // Assigned roles
    public int PostCount { get; set; }                // Number of posts created
    public int TourCount { get; set; }                // Number of tours created
}
```

### UpdateUserRolesModel
Request model for role assignment operations.

```csharp
public class UpdateUserRolesModel
{
    public string UserId { get; set; }          // Target user ID
    public IList<string> Roles { get; set; }    // New roles to assign
}
```

### UserActivityModel
Represents single audit event for display.

```csharp
public class UserActivityModel
{
    public string UserId { get; set; }          // User who performed action
    public string ActionType { get; set; }      // Event type/category
    public string Description { get; set; }     // Human-readable description
    public DateTime Timestamp { get; set; }     // When action occurred
    public string IpAddress { get; set; }       // IP address of request
}
```

## Implementation Details

### Service Layer
**IAdminService** interface with implementations in:
- `UrGuide.Services/Contracts/IAdminService.cs` - Interface definition
- `UrGuide.WebApp/Services/AdminService.cs` - Implementation using ASP.NET Core Identity

**Dependencies:**
- `UserManager<UrGuideUser>` - ASP.NET Core Identity user management
- `RoleManager<IdentityRole>` - Role management operations
- `UrGuideContext` - Entity Framework database context for queries
- `IUserContext` - Current user information service
- `ILogger<AdminService>` - Logging for diagnostics

### Controller
**AdminController** in `UrGuide.WebApp/Controllers/AdminController.cs`
- Base route: `[Route("api/[controller]")]`
- Authorization: `[Authorize(Roles = "Admin")]` on all endpoints
- API versioning: `[ApiController]`
- Swagger documentation with `[ProducesResponseType]` attributes

### Database Entities
**AuditEvent table** stores user activity:
```sql
CREATE TABLE AuditEvent (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450),
    EventCode INT,
    Message NVARCHAR(MAX),
    Created DATETIME2,
    IpAddress NVARCHAR(45)
)
```

## Key Implementation Notes

### PagedList Construction
Use static factory method, not constructor:
```csharp
// ✅ Correct
var pagedList = PagedList.Of(users, parameters.PageNumber);

// ❌ Wrong (constructor is internal)
var pagedList = new PagedList<User>(users, totalCount, pageNumber, pageSize);
```

### Entity Property Names
- **TourRequest**: Uses `RequesterId` not `UserId` for user relationship
- **AuditEvent**: Uses `Created` timestamp not `Timestamp` or `OccurredOn`
- **SearchParameters**: Uses `Term` property not `SearchText`

### Suspension Implementation
Account suspension uses ASP.NET Core Identity lockout:
```csharp
user.LockoutEnd = DateTimeOffset.UtcNow.AddDays(durationDays);
user.LockoutEnabled = true;
```

### Role Management
Role updates remove ALL current roles, then add new ones:
```csharp
var currentRoles = await _userManager.GetRolesAsync(user);
await _userManager.RemoveFromRolesAsync(user, currentRoles);
await _userManager.AddToRolesAsync(user, model.Roles);
```

## Testing

### Manual Testing with Swagger
1. Navigate to `/swagger` endpoint
2. Click "Authorize" and enter JWT token with Admin role
3. Expand `/api/admin` endpoints
4. Test each endpoint with sample data

### Sample Test Data
```bash
# Get users with search
curl -X GET "https://localhost:5001/api/admin/users?Term=john&PageSize=10" \
  -H "Authorization: Bearer {token}"

# Suspend user for 7 days
curl -X POST "https://localhost:5001/api/admin/users/{userId}/suspend?durationDays=7" \
  -H "Authorization: Bearer {token}"

# Update user roles
curl -X PUT "https://localhost:5001/api/admin/users/roles" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"userId": "{userId}", "roles": ["User", "Guide"]}'
```

## Security Considerations

### Authorization
- All endpoints require authenticated admin user
- JWT token must include Admin role claim
- Non-admin users receive 403 Forbidden response

### Audit Trail
- User account modifications should be logged to AuditEvent table
- Consider adding admin actions to activity log
- Track IP addresses and timestamps for security analysis

### Deletion Safety
- DELETE endpoint performs permanent deletion
- Consider soft-delete implementation for data recovery
- May need cascade deletion for related entities

### Rate Limiting
- Consider rate limiting on admin endpoints to prevent abuse
- Especially important for suspension/deletion operations

## Next Steps - Frontend Implementation

### 1. React Admin Dashboard Setup
Create separate admin app or admin section:
```
UrGuide.WebApp/ClientApp/src/admin/
├── AdminApp.tsx           # Main admin application
├── routes/                # Admin routing configuration
├── components/            # Shared admin components
├── pages/
│   ├── Login.tsx          # Admin login with 2FA
│   ├── Dashboard.tsx      # Overview/statistics
│   ├── UserList.tsx       # User management table
│   ├── UserDetail.tsx     # Individual user page
│   └── ActivityLog.tsx    # Audit trail viewer
└── services/
    └── adminApi.ts        # API client for admin endpoints
```

### 2. User Management Interface Features
- **User List**: DataGrid/Table component with pagination
- **Search**: Real-time search by email/name
- **Filters**: Filter by role, verification status, lockout status
- **Actions**: Suspend/Activate/Delete with confirmation dialogs
- **Bulk operations**: Select multiple users for batch actions
- **Role editor**: Multi-select dropdown or chip list

### 3. Authentication Flow
- Admin login page separate from user login
- 2FA verification integration with existing TOTP system (Issue #4)
- JWT token storage in localStorage/sessionStorage
- Automatic token refresh before expiration
- Admin role verification on protected routes

### 4. Technology Stack Recommendations
**UI Framework Options:**
- Material-UI (MUI) Data Grid - `@mui/x-data-grid`
- TanStack Table v8 - More flexible, headless UI
- Ant Design - Enterprise admin UI components

**State Management:**
- React Query (TanStack Query) - Server state management
- Zustand/Redux - Client state for auth tokens

**Routing:**
- React Router v6 - Client-side routing
- Protect admin routes with AuthGuard/RequireAuth wrapper

### 5. API Integration Example
```typescript
// adminApi.ts
import axios from 'axios';

const api = axios.create({
  baseURL: '/api/admin',
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('adminToken')}`
  }
});

export const getUsers = async (params: SearchParameters) => {
  const { data } = await api.get('/users', { params });
  return data;
};

export const suspendUser = async (userId: string, durationDays: number = 30) => {
  const { data } = await api.post(`/users/${userId}/suspend`, null, {
    params: { durationDays }
  });
  return data;
};

export const updateUserRoles = async (model: UpdateUserRolesModel) => {
  const { data } = await api.put('/users/roles', model);
  return data;
};
```

---

## Build Status
✅ **All projects compiled successfully** (8.6 seconds)
- UrGuide.Core.dll
- UrGuide.Model.dll  
- UrGuide.ServiceDefaults.dll
- UrGuide.Shared.dll
- UrGuide.Data.dll
- UrGuide.Services.dll
- UrGuide.WebApp.dll ✅

**Ready for frontend development!**

# UrGuide SDK Documentation

This guide covers how to generate, configure, and use client SDKs for the UrGuide API.

## Table of Contents

- [Overview](#overview)
- [OpenAPI Specification](#openapi-specification)
- [Generating Client SDKs](#generating-client-sdks)
- [C# / .NET SDK](#c--net-sdk)
- [TypeScript / JavaScript SDK](#typescript--javascript-sdk)
- [Python SDK](#python-sdk)
- [Mobile SDKs](#mobile-sdks)
- [SDK Configuration](#sdk-configuration)
- [Authentication in SDKs](#authentication-in-sdks)

---

## Overview

The UrGuide API publishes an OpenAPI 3.0 specification that can be used to auto-generate client SDKs in any language. This eliminates hand-written HTTP client code and provides strongly-typed models for all request/response objects.

### Available Resources

| Resource | URL | Format |
|----------|-----|--------|
| OpenAPI Spec (JSON) | `/swagger/v1/swagger.json` | OpenAPI 3.0 JSON |
| Swagger UI | `/swagger/index.html` | Interactive HTML |

---

## OpenAPI Specification

Download the OpenAPI specification from your running UrGuide instance:

```bash
# Download the OpenAPI spec
curl https://your-instance.com/swagger/v1/swagger.json -o urguide-api.json

# Validate the spec (requires swagger-cli)
npx @apidevtools/swagger-cli validate urguide-api.json
```

The specification includes:
- All API endpoints with HTTP methods and routes
- Request/response schemas with data types
- Authentication requirements (OAuth2, Bearer JWT)
- Error response formats
- XML documentation comments from controller methods

---

## Generating Client SDKs

### Using OpenAPI Generator

[OpenAPI Generator](https://openapi-generator.tech/) supports 50+ languages and frameworks.

#### Installation

```bash
# Via npm
npm install @openapitools/openapi-generator-cli -g

# Via Homebrew (macOS)
brew install openapi-generator

# Via Docker
docker pull openapitools/openapi-generator-cli
```

#### Generate SDKs

```bash
# C# SDK
openapi-generator-cli generate \
  -i https://your-instance.com/swagger/v1/swagger.json \
  -g csharp \
  -o ./sdk/csharp \
  --additional-properties=packageName=UrGuide.Client,targetFramework=net8.0

# TypeScript SDK (Fetch)
openapi-generator-cli generate \
  -i https://your-instance.com/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o ./sdk/typescript \
  --additional-properties=npmName=urguide-client,supportsES6=true

# Python SDK
openapi-generator-cli generate \
  -i https://your-instance.com/swagger/v1/swagger.json \
  -g python \
  -o ./sdk/python \
  --additional-properties=packageName=urguide_client
```

### Using NSwag (.NET)

For .NET projects, [NSwag](https://github.com/RicoSuter/NSwag) generates C# and TypeScript clients directly.

```bash
# Install NSwag CLI
dotnet tool install -g NSwag.ConsoleCore

# Generate C# client
nswag openapi2csclient \
  /input:https://your-instance.com/swagger/v1/swagger.json \
  /output:UrGuideApiClient.cs \
  /namespace:UrGuide.Client \
  /generateClientInterfaces:true \
  /useBaseUrl:false
```

### Using Kiota (Microsoft)

[Kiota](https://learn.microsoft.com/en-us/openapi/kiota/) is Microsoft's API client generator.

```bash
# Install Kiota
dotnet tool install -g Microsoft.OpenApi.Kiota

# Generate client
kiota generate \
  -l CSharp \
  -d https://your-instance.com/swagger/v1/swagger.json \
  -o ./sdk/kiota-csharp \
  -n UrGuide.Client
```

---

## C# / .NET SDK

### Generated Client Usage

After generating with NSwag or OpenAPI Generator:

```csharp
using UrGuide.Client;

// Create and configure the client
var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://your-instance.com")
};

var client = new UrGuideApiClient(httpClient);

// Authenticate
var tokenResponse = await client.Auth_TokenAsync(new AdminLoginRequest
{
    UserName = "user@example.com",
    Password = "SecurePassword123!"
});

httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

// Use the API with strongly-typed models
var recentTours = await client.Posts_Last10Async();
foreach (var tour in recentTours)
{
    Console.WriteLine($"{tour.Title} - {tour.Price:C}");
}

// Search for tours
var searchResults = await client.Posts_SearchAsync(new SearchParameters
{
    Page = 1,
    PageSize = 20,
    Query = "paris walking tour"
});

// Create a tour request
var tourRequest = await client.TourRequests_CreateAsync(new CreateTourRequestModel
{
    Title = "Private wine tour in Bordeaux",
    Budget = 500,
    StartDate = DateTimeOffset.Now.AddMonths(2)
});
```

### Manual HTTP Client Wrapper

If you prefer not to use code generation:

```csharp
public class UrGuideClient : IDisposable
{
    private readonly HttpClient _http;
    private string? _accessToken;
    private string? _refreshToken;

    public UrGuideClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _http.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task AuthenticateAsync(string userName, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { userName, password });
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _accessToken = result!.AccessToken;
        _refreshToken = result.RefreshToken;
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<List<PostModel>> GetRecentToursAsync()
    {
        return await _http.GetFromJsonAsync<List<PostModel>>("/api/posts/last10")
            ?? new List<PostModel>();
    }

    public async Task<PostModel> CreateTourAsync(PostCreationModel model)
    {
        var response = await _http.PostAsJsonAsync("/api/posts/create", model);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PostModel>())!;
    }

    public void Dispose() => _http.Dispose();

    private record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);
}
```

---

## TypeScript / JavaScript SDK

### Generated Client Usage

After generating with OpenAPI Generator (typescript-fetch):

```typescript
import { Configuration, PostsApi, AuthApi, SearchApi } from 'urguide-client';

// Configure the client
const config = new Configuration({
  basePath: 'https://your-instance.com',
  accessToken: '' // Will be set after authentication
});

const authApi = new AuthApi(config);
const postsApi = new PostsApi(config);
const searchApi = new SearchApi(config);

// Authenticate
const tokenResponse = await authApi.authTokenPost({
  adminLoginRequest: {
    userName: 'user@example.com',
    password: 'SecurePassword123!'
  }
});

// Update configuration with token
config.accessToken = tokenResponse.accessToken;

// Browse tours
const recentTours = await postsApi.postsLast10Get();
recentTours.forEach(tour => {
  console.log(`${tour.title} - $${tour.price}`);
});

// Search
const results = await searchApi.searchPostsPost({
  searchRequest: {
    query: 'paris walking tour',
    page: 1,
    pageSize: 20
  }
});
```

### Manual Fetch Wrapper

```typescript
class UrGuideClient {
  private baseUrl: string;
  private accessToken: string = '';
  private refreshToken: string = '';

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl.replace(/\/$/, '');
  }

  async authenticate(userName: string, password: string): Promise<void> {
    const response = await this.post('/api/auth/login', { userName, password });
    this.accessToken = response.accessToken;
    this.refreshToken = response.refreshToken;
  }

  async getRecentTours(): Promise<Tour[]> {
    return this.get('/api/posts/last10');
  }

  async searchTours(query: string, page = 1, pageSize = 20): Promise<SearchResult> {
    return this.post('/api/posts/search', { query, page, pageSize });
  }

  private async get<T>(path: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`, {
      headers: this.getHeaders()
    });
    return this.handleResponse(response);
  }

  private async post<T>(path: string, body: unknown): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(body)
    });
    return this.handleResponse(response);
  }

  private getHeaders(): Record<string, string> {
    const headers: Record<string, string> = { 'Content-Type': 'application/json' };
    if (this.accessToken) {
      headers['Authorization'] = `Bearer ${this.accessToken}`;
    }
    return headers;
  }

  private async handleResponse<T>(response: Response): Promise<T> {
    if (response.status === 401 && this.refreshToken) {
      await this.refreshAccessToken();
      // Retry logic would go here
    }
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.errors?.join(', ') || 'API request failed');
    }
    return response.json();
  }

  private async refreshAccessToken(): Promise<void> {
    const response = await fetch(`${this.baseUrl}/api/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });
    const data = await response.json();
    this.accessToken = data.accessToken;
    this.refreshToken = data.refreshToken;
  }
}
```

---

## Python SDK

### Generated Client Usage

After generating with OpenAPI Generator:

```python
import urguide_client
from urguide_client.api import auth_api, posts_api, search_api

# Configure the client
configuration = urguide_client.Configuration(
    host="https://your-instance.com"
)

with urguide_client.ApiClient(configuration) as api_client:
    # Authenticate
    auth = auth_api.AuthApi(api_client)
    token_response = auth.auth_token_post(
        admin_login_request={"userName": "user@example.com", "password": "SecurePass123!"}
    )
    
    # Set the token
    configuration.access_token = token_response.access_token
    
    # Browse tours
    posts = posts_api.PostsApi(api_client)
    recent_tours = posts.posts_last10_get()
    for tour in recent_tours:
        print(f"{tour.title} - ${tour.price}")
    
    # Search
    search = search_api.SearchApi(api_client)
    results = search.search_posts_post(
        search_request={"query": "paris walking tour", "page": 1, "pageSize": 20}
    )
```

### Manual Requests Wrapper

```python
import requests
from typing import Optional

class UrGuideClient:
    def __init__(self, base_url: str):
        self.base_url = base_url.rstrip('/')
        self.session = requests.Session()
        self.session.headers['Content-Type'] = 'application/json'
        self._refresh_token: Optional[str] = None

    def authenticate(self, user_name: str, password: str) -> None:
        response = self.session.post(
            f"{self.base_url}/api/auth/login",
            json={"userName": user_name, "password": password}
        )
        response.raise_for_status()
        data = response.json()
        self.session.headers['Authorization'] = f"Bearer {data['accessToken']}"
        self._refresh_token = data.get('refreshToken')

    def get_recent_tours(self) -> list:
        return self._get("/api/posts/last10")

    def search_tours(self, query: str, page: int = 1, page_size: int = 20) -> dict:
        return self._post("/api/posts/search", {
            "query": query, "page": page, "pageSize": page_size
        })

    def create_tour_request(self, title: str, budget: float, **kwargs) -> dict:
        return self._post("/api/tour-requests", {
            "title": title, "budget": budget, **kwargs
        })

    def get_recommendations(self, count: int = 10, lat: float = None, lng: float = None) -> list:
        params = {"count": count}
        if lat is not None: params["lat"] = lat
        if lng is not None: params["lng"] = lng
        return self._get("/api/recommendation", params=params)

    def _get(self, path: str, params: dict = None):
        response = self.session.get(f"{self.base_url}{path}", params=params)
        self._handle_errors(response)
        return response.json()

    def _post(self, path: str, data: dict):
        response = self.session.post(f"{self.base_url}{path}", json=data)
        self._handle_errors(response)
        return response.json()

    def _handle_errors(self, response: requests.Response):
        if response.status_code == 401 and self._refresh_token:
            self._refresh_access_token()
            return
        response.raise_for_status()
        data = response.json()
        if isinstance(data, dict) and data.get('isError'):
            raise UrGuideApiError(response.status_code, data['errors'])

    def _refresh_access_token(self):
        response = self.session.post(
            f"{self.base_url}/api/auth/refresh",
            json={"refreshToken": self._refresh_token}
        )
        response.raise_for_status()
        data = response.json()
        self.session.headers['Authorization'] = f"Bearer {data['accessToken']}"
        self._refresh_token = data.get('refreshToken')


class UrGuideApiError(Exception):
    def __init__(self, status_code: int, errors: list):
        self.status_code = status_code
        self.errors = errors
        super().__init__(f"API Error {status_code}: {', '.join(errors)}")
```

---

## Mobile SDKs

### .NET MAUI

The UrGuide repository includes a .NET MAUI mobile app (`UrGuide.MAUI/`) that uses auto-generated API clients from the OpenAPI specification.

For new MAUI projects, generate the client with NSwag:

```bash
nswag openapi2csclient \
  /input:https://your-instance.com/swagger/v1/swagger.json \
  /output:ApiClient.cs \
  /namespace:UrGuide.MAUI.Services \
  /generateClientInterfaces:true
```

### React Native / Expo

Use the TypeScript SDK generated above:

```bash
# Generate the SDK
openapi-generator-cli generate \
  -i https://your-instance.com/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o ./src/api \
  --additional-properties=npmName=urguide-api

# Use in React Native
import { PostsApi, Configuration } from './api';
```

### Flutter / Dart

```bash
openapi-generator-cli generate \
  -i https://your-instance.com/swagger/v1/swagger.json \
  -g dart \
  -o ./packages/urguide_api \
  --additional-properties=pubName=urguide_api
```

---

## SDK Configuration

### Environment Variables

All SDKs should support configuration via environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `URGUIDE_API_URL` | Base URL of the UrGuide API | `https://localhost:5001` |
| `URGUIDE_API_KEY` | API key (if using API key auth) | — |
| `URGUIDE_TIMEOUT` | Request timeout in seconds | `30` |
| `URGUIDE_RETRY_COUNT` | Number of retries on failure | `3` |

### API Versioning in SDKs

SDKs should include the API version in requests. Three methods are supported:

```javascript
// URL segment (recommended)
const baseUrl = 'https://your-instance.com/api/v1';

// Header
headers['X-Api-Version'] = '1.0';

// Query parameter
const url = '/api/posts/last10?api-version=1.0';
```

### Localization

Set the `Accept-Language` header to receive localized responses:

```javascript
// In SDK configuration
headers['Accept-Language'] = 'fr'; // French
headers['Accept-Language'] = 'es'; // Spanish
```

---

## Authentication in SDKs

### Token Lifecycle Management

All SDKs should implement automatic token refresh:

1. **Authenticate** — Call `/api/auth/login` or `/api/auth/token` to get initial tokens
2. **Store tokens** — Keep access token and refresh token securely
3. **Use access token** — Include in `Authorization: Bearer <token>` header
4. **Check expiry** — Monitor the `expiresIn` value (8 hours / 28800 seconds)
5. **Refresh** — Call `/api/auth/refresh` before the access token expires
6. **Handle 401** — If a request returns 401, attempt a token refresh and retry

### Token Storage Recommendations

| Platform | Storage Method |
|----------|---------------|
| Server-side | Environment variables or secure vault |
| Browser SPA | Memory only (not localStorage) |
| Mobile app | Secure Keychain (iOS) / Keystore (Android) |
| Desktop app | OS credential manager |

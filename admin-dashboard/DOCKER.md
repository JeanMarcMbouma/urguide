# Docker Integration for Admin Dashboard

## Overview
The admin dashboard is now fully integrated with Docker using a multi-stage build:
1. **Build stage**: Node.js 18 Alpine builds the React production bundle
2. **Runtime stage**: Nginx 1.25 Alpine serves the static files and proxies API requests

## Architecture

```
┌─────────────────────────────────────────┐
│   Docker Compose Network (urguide)      │
│                                         │
│  ┌──────────────┐    ┌──────────────┐  │
│  │   Admin      │────▶│  API (5000)  │  │
│  │ Dashboard    │ API │              │  │
│  │  (3001)      │◀────│  .NET 10     │  │
│  └──────────────┘ HTTP└──────┬───────┘  │
│        │                     │          │
│        │                     │          │
│  ┌─────▼────────────────────▼────────┐  │
│  │  SQL │ RabbitMQ │ Elasticsearch  │  │
│  └────────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

## Container Details

### Admin Dashboard Container
- **Image**: Custom (multi-stage build)
- **Base**: nginx:1.25-alpine
- **Port**: 3001 (host) → 80 (container)
- **Dependencies**: API container (healthy)
- **Health Check**: wget on /health endpoint
- **Restart Policy**: unless-stopped

### Features
- ✅ Production-optimized React build
- ✅ Nginx with gzip compression
- ✅ Security headers (X-Frame-Options, CSP, etc.)
- ✅ API proxy to backend at `/api/*`
- ✅ SignalR Hub proxy at `/notify`
- ✅ React Router SPA fallback
- ✅ Static asset caching (1 year for immutable files)
- ✅ Health check endpoint at `/health`

## Usage

### Build and Run All Services
```bash
# Start all services (API + Admin + Databases)
docker-compose up -d

# View logs
docker-compose logs -f admin-dashboard

# Check health
docker-compose ps
```

### Build Only Admin Dashboard
```bash
# Build the image
docker-compose build admin-dashboard

# Run just the dashboard
docker-compose up -d admin-dashboard
```

### Access Points
- **Admin Dashboard**: http://localhost:3001
- **API**: http://localhost:5000
- **API Docs (Swagger)**: http://localhost:5000/swagger

### Development vs Production

#### Development (Local)
```bash
cd admin-dashboard
npm run dev
# Hot reload, fast refresh, dev tools
# Access at http://localhost:3001
```

#### Production (Docker)
```bash
docker-compose up -d admin-dashboard
# Optimized bundle, Nginx serving, production mode
# Access at http://localhost:3001
```

## Nginx Configuration

### Proxy Rules
```nginx
# API requests → Backend API
/api/* → http://api:80/api/*

# SignalR Hub → Backend WebSocket
/notify → http://api:80/notify
```

### Security Headers
- `X-Frame-Options: SAMEORIGIN` - Prevent clickjacking
- `X-Content-Type-Options: nosniff` - Prevent MIME sniffing
- `X-XSS-Protection: 1; mode=block` - XSS protection
- `Referrer-Policy: no-referrer-when-downgrade` - Privacy

### Caching Strategy
- **HTML files**: No cache (always fresh)
- **JS/CSS/Images**: 1 year cache (immutable with content hashes)
- **API responses**: No cache (dynamic data)

## Build Process

### Stage 1: Build React App
```dockerfile
FROM node:18-alpine
- npm ci --legacy-peer-deps
- npm run build
- Output: /app/dist
```

### Stage 2: Serve with Nginx
```dockerfile
FROM nginx:1.25-alpine
- Copy nginx.conf
- Copy dist/ to /usr/share/nginx/html
- Set permissions
- Expose port 80
```

### Final Image Size
- **Build stage**: ~500MB (Node.js + dependencies)
- **Final image**: ~25MB (Nginx + React bundle)
- **Compression**: ~95% reduction from build stage

## Troubleshooting

### Container not starting
```bash
# Check logs
docker-compose logs admin-dashboard

# Inspect container
docker inspect urguide-admin-dashboard
```

### API proxy not working
```bash
# Test from inside container
docker exec urguide-admin-dashboard wget -O- http://api:80/health

# Check network
docker network inspect urguide-network
```

### Build failures
```bash
# Clean build
docker-compose build --no-cache admin-dashboard

# Check Dockerfile syntax
docker build -t test ./admin-dashboard
```

### Health check failing
```bash
# Test health endpoint
curl http://localhost:3001/health

# Check Nginx logs
docker exec urguide-admin-dashboard cat /var/log/nginx/error.log
```

## Environment Variables

### Build-time Variables (Optional)
Add to `admin-dashboard/.env`:
```bash
# API URL (already proxied via nginx)
VITE_API_URL=/api

# Other config...
```

### Runtime Variables
Configured in `docker-compose.yml`:
```yaml
environment:
  - NODE_ENV=production
```

## CI/CD Integration

### GitHub Actions (Example)
```yaml
- name: Build Admin Dashboard
  run: docker-compose build admin-dashboard

- name: Push to Registry
  run: |
    docker tag urguide-admin-dashboard ghcr.io/jeanmarcmbouma/urguide-admin:latest
    docker push ghcr.io/jeanmarcmbouma/urguide-admin:latest
```

### Production Deployment
```bash
# Pull and run
docker pull ghcr.io/jeanmarcmbouma/urguide-admin:latest
docker run -d -p 3001:80 --name admin ghcr.io/jeanmarcmbouma/urguide-admin:latest
```

## Best Practices

### Security
- ✅ Non-root user (nginx user)
- ✅ Security headers enabled
- ✅ Hidden files blocked (/.*)
- ✅ Health checks for monitoring
- ✅ Multi-stage build (no dev dependencies in final image)

### Performance
- ✅ Gzip compression enabled
- ✅ Aggressive caching for static assets
- ✅ Minimal Alpine-based images
- ✅ Build artifacts optimized (Vite production build)

### Reliability
- ✅ Health checks (30s interval)
- ✅ Restart policy (unless-stopped)
- ✅ Dependency ordering (waits for API)
- ✅ Graceful shutdown support

## Related Documentation
- [Admin Dashboard README](README.md) - Features and local development
- [Admin API Documentation](../docs/implementation/ADMIN_API_DOCUMENTATION.md) - Backend API reference
- [Docker Compose](../docker-compose.yml) - Full orchestration configuration

# 🐳 Quick Start with Docker

Complete guide to run UrGuide platform with Docker Compose.

## 🚀 Prerequisites

- Docker 20.10+ and Docker Compose 2.0+
- Windows/Linux/macOS with Docker Desktop installed
- 8GB RAM recommended (for all services)
- Ports available: 1433, 5000, 3001, 5672, 15672, 9200, 9300

## 📦 What Gets Started

Running `docker-compose up` starts **5 containers**:

1. **SQL Server 2022** - Database (port 1433)
2. **RabbitMQ 3** - Message queue (port 5672, UI: 15672)
3. **Elasticsearch 8.11** - Search engine (port 9200)
4. **UrGuide API** - .NET 10 backend (port 5000)
5. **Admin Dashboard** - React 18 frontend (port 3001)

## ⚡ Quick Start (Production Mode)

```bash
# 1. Clone repository
git clone https://github.com/JeanMarcMbouma/urguide.git
cd urguide

# 2. Start all services
docker-compose up -d

# 3. Wait for health checks (30-60 seconds)
docker-compose ps

# 4. Access the applications
# API: http://localhost:5000
# Admin Dashboard: http://localhost:3001
# Swagger: http://localhost:5000/swagger
# RabbitMQ UI: http://localhost:15672 (guest/guest)
```

## 🛠️ Development Mode (with Hot Reload)

```bash
# 1. Create .env file for secrets
cp .env.example .env
# Edit .env with your API keys and admin credentials

# 2. Configure admin user provisioning (optional)
# Edit .env file:
#   SEED_ADMIN_ENABLED=true
#   ADMIN_EMAIL=admin@urguide.local
#   ADMIN_PASSWORD=Admin123!
#   ADMIN_FIRST_NAME=Admin
#   ADMIN_LAST_NAME=User

# 3. Start with development override
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# 4. Code changes auto-reload
# - .NET: dotnet watch (backend)
# - React: Vite HMR (admin dashboard)
```

### 👤 Admin User Auto-Provisioning

When running with Docker Compose, the system can automatically create an admin user on startup using environment variables from your `.env` file.

**Configuration in `.env`**:
```bash
# Enable/disable admin provisioning
SEED_ADMIN_ENABLED=true

# Admin credentials (change these!)
ADMIN_EMAIL=admin@urguide.local
ADMIN_PASSWORD=Admin123!
ADMIN_FIRST_NAME=Admin
ADMIN_LAST_NAME=User
```

**How it works**:
1. Environment variables override `appsettings.json` values
2. On first startup, admin user is created automatically
3. Subsequent startups detect existing user (no duplicates)
4. Admin role is assigned automatically

**Login after provisioning**:
- URL: http://localhost:3001
- Email: Value from `ADMIN_EMAIL` in `.env`
- Password: Value from `ADMIN_PASSWORD` in `.env`

⚠️ **Remember**: Change the default password after first login!

## 🎯 Individual Services

### Start Only Admin Dashboard
```bash
# Requires API to be running
docker-compose up -d api admin-dashboard
```

### Start Only API + Databases
```bash
docker-compose up -d sqlserver rabbitmq elasticsearch api
```

### Rebuild After Changes
```bash
# Rebuild specific service
docker-compose build admin-dashboard

# Rebuild all services
docker-compose build

# Rebuild without cache (clean build)
docker-compose build --no-cache
```

## 🔍 Monitoring & Logs

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f admin-dashboard
docker-compose logs -f api

# Last 100 lines
docker-compose logs --tail=100 admin-dashboard
```

### Check Health Status
```bash
# Service status
docker-compose ps

# Detailed container info
docker inspect urguide-admin-dashboard
docker inspect urguide-api
```

### Test Endpoints
```bash
# API health
curl http://localhost:5000/health

# Admin dashboard health
curl http://localhost:3001/health

# Elasticsearch health
curl http://localhost:9200/_cluster/health
```

## 🛑 Stopping & Cleaning

### Stop Services
```bash
# Stop all services (preserves data)
docker-compose down

# Stop and remove volumes (deletes data)
docker-compose down -v

# Stop specific service
docker-compose stop admin-dashboard
```

### Clean Up
```bash
# Remove all containers and networks
docker-compose down --remove-orphans

# Remove all including volumes (DESTRUCTIVE)
docker-compose down -v --remove-orphans

# Remove images
docker-compose down --rmi all
```

## 🐛 Troubleshooting

### Admin Dashboard Not Loading

**Problem**: Dashboard shows connection errors

**Solution**:
```bash
# Check if API is healthy
docker-compose ps api

# Check if dashboard can reach API
docker exec urguide-admin-dashboard wget -O- http://api:80/health

# Restart dashboard
docker-compose restart admin-dashboard
```

### API Not Starting

**Problem**: API container exits immediately

**Solution**:
```bash
# Check logs for errors
docker-compose logs api

# Common issues:
# 1. Database not ready - wait for SQL Server health check
docker-compose ps sqlserver

# 2. Port already in use
lsof -i :5000  # Linux/Mac
netstat -ano | findstr :5000  # Windows
```

### Database Connection Failed

**Problem**: API can't connect to SQL Server

**Solution**:
```bash
# Check SQL Server health
docker-compose logs sqlserver

# Test connection from API container
docker exec urguide-api curl -f http://sqlserver:1433 || echo "Cannot reach DB"

# Verify network
docker network inspect urguide-network
```

### Build Failures

**Problem**: `docker-compose build` fails

**Solution**:
```bash
# Admin Dashboard build issues
cd admin-dashboard
npm install --legacy-peer-deps
cd ..

# .NET API build issues
dotnet restore UrGuide.WebApp/UrGuide.WebApp.csproj

# Clean build
docker-compose build --no-cache
```

### Port Conflicts

**Problem**: "Port is already allocated"

**Solution**:
```bash
# Change ports in docker-compose.yml
# Admin Dashboard: Change "3001:80" to "3002:80"
# API: Change "5000:80" to "5001:80"

# Or stop conflicting services
docker ps  # Find container using port
docker stop <container-name>
```

## 📊 Resource Usage

### Expected Memory Usage
- SQL Server: ~500MB
- RabbitMQ: ~150MB
- Elasticsearch: ~1GB (configurable via ES_JAVA_OPTS)
- UrGuide API: ~200MB
- Admin Dashboard: ~25MB
- **Total**: ~2GB RAM

### Reduce Elasticsearch Memory
Edit `docker-compose.yml`:
```yaml
elasticsearch:
  environment:
    - "ES_JAVA_OPTS=-Xms256m -Xmx256m"  # Reduced from 512m
```

## 🔐 Security Notes

### Production Deployment

⚠️ **DO NOT** use default passwords in production!

```bash
# Set strong passwords in .env
SQL_SA_PASSWORD=YourVeryStrong@Password123!
RABBITMQ_USER=admin
RABBITMQ_PASS=SuperSecurePassword456!

# Admin user credentials (change these!)
SEED_ADMIN_ENABLED=true
ADMIN_EMAIL=admin@yourcompany.com
ADMIN_PASSWORD=VerySecureP@ssw0rd789!
ADMIN_FIRST_NAME=John
ADMIN_LAST_NAME=Doe

# Or use environment variables
export SQL_SA_PASSWORD="..."
export RABBITMQ_USER="..."
export RABBITMQ_PASS="..."
export ADMIN_EMAIL="..."
export ADMIN_PASSWORD="..."

docker-compose up -d
```

**Admin User Security Best Practices**:
- ✅ Use strong passwords (12+ characters, mixed case, numbers, symbols)
- ✅ Change default password immediately after first login
- ✅ Use unique email addresses (not shared accounts)
- ✅ Enable 2FA after logging in
- ✅ Disable auto-provisioning in production (`SEED_ADMIN_ENABLED=false`)
- ✅ Store `.env` file securely (never commit to git)
- ✅ Use secrets management in production (Azure Key Vault, AWS Secrets Manager)

### HTTPS Configuration

For production, use reverse proxy (Nginx/Traefik):
```bash
# Example with Nginx
# Admin Dashboard: https://admin.example.com → http://localhost:3001
# API: https://api.example.com → http://localhost:5000
```

## 📚 Advanced Usage

### Scale Services
```bash
# Run multiple API instances (requires load balancer)
docker-compose up -d --scale api=3
```

### Custom Network
```bash
# Connect to existing network
docker-compose --project-name urguide up -d
```

### Volume Management
```bash
# Backup database volume
docker run --rm -v urguide_sqlserver-data:/data -v $(pwd):/backup \
  alpine tar czf /backup/sqlserver-backup.tar.gz /data

# Restore database volume
docker run --rm -v urguide_sqlserver-data:/data -v $(pwd):/backup \
  alpine tar xzf /backup/sqlserver-backup.tar.gz -C /
- [ ] Admin user created: Check API logs for "Successfully created admin user"
- [ ] Admin login works: Login at http://localhost:3001 with credentials from `.env`

---

**Ready to go! 🎉** Access admin dashboard at http://localhost:3001

**Admin Credentials** (from `.env`):
- Email: Value from `ADMIN_EMAIL`
- Password: Value from `ADMIN_PASSWORD`
- [Admin Dashboard Docker Documentation](admin-dashboard/DOCKER.md)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [UrGuide Main README](README.md)

## ✅ Verification Checklist

After starting services, verify:

- [ ] SQL Server: `docker-compose logs sqlserver | grep "SQL Server is now ready"`
- [ ] RabbitMQ: Visit http://localhost:15672 (login: guest/guest)
- [ ] Elasticsearch: `curl http://localhost:9200` returns cluster info
- [ ] API: Visit http://localhost:5000/swagger
- [ ] Admin Dashboard: Visit http://localhost:3001
- [ ] All containers healthy: `docker-compose ps` shows all as "healthy"

---

**Ready to go! 🎉** Access admin dashboard at http://localhost:3001

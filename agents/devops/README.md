# DevOps Agent

Generic specification for the DevOps role.

## Overview

The DevOps Engineer builds **deployment and operations infrastructure** using 100% open source tools. DevOps makes code deployable, scalable, and observable through containerization, CI/CD automation, and monitoring.

## Quick Start

```bash
cat agents/devops/SKILL.md
cat planning-mds/INCEPTION.md
```

## Core Workflow (Summary)

1) Read `planning-mds/INCEPTION.md` (tech stack, deployment requirements)
2) Containerize applications (Docker, docker-compose)
3) Set up local development environment
4) Configure CI/CD pipelines (GitHub Actions)
5) Set up monitoring (Prometheus, Grafana)
6) Write deployment documentation
7) Deploy to staging and production

## Framework-Only Mode

Before application code exists (`engine/`, `experience/`, deploy manifests), infra validation is expected to report missing artifacts.

- Non-strict check: `python agents/devops/scripts/validate-infrastructure.py .` (informational warnings)
- Strict gate (for implementation phase): `python agents/devops/scripts/validate-infrastructure.py . --strict`

## Tech Stack (All Free & Open Source)

**Containerization:**
- Docker (Apache 2.0)
- docker-compose (Apache 2.0)

**CI/CD:**
- GitHub Actions (free tier)
- GitLab CI (open source, self-hosted)

**Services:**
- PostgreSQL (PostgreSQL License)
- Keycloak (Apache 2.0) - Auth
- Temporal (MIT) - Workflows

**Monitoring:**
- Prometheus (Apache 2.0) - Metrics
- Grafana (AGPL v3) - Dashboards
- Loki (AGPL v3) - Logs
- Jaeger (Apache 2.0) - Tracing

**Reverse Proxy:**
- Nginx (BSD-2-Clause)
- Traefik (MIT)

**Secrets:**
- HashiCorp Vault (MPL 2.0)
- Sealed Secrets (Apache 2.0)
- SOPS (MPL 2.0)

**Total Cost:** $0/year

## Directory Structure

```
your-app/
├── docker/
│   ├── backend/Dockerfile
│   ├── frontend/Dockerfile
│   ├── neuron/Dockerfile
│   └── nginx/nginx.conf
├── docker-compose.yml
├── docker-compose.dev.yml
├── docker-compose.prod.yml
├── .github/workflows/
│   ├── ci.yml
│   ├── deploy-staging.yml
│   └── deploy-prod.yml
├── scripts/
│   ├── deploy.sh
│   ├── backup-db.sh
│   └── setup-dev.sh
├── monitoring/
│   ├── prometheus.yml
│   └── grafana-dashboards/
├── .env.example
└── docs/operations/
    ├── deployment-guide.md
    └── troubleshooting.md
```

## Common Commands

### Local Development

```bash
# Start all services
docker-compose up

# Start in detached mode
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Rebuild and start
docker-compose up --build

# Remove volumes (clean slate)
docker-compose down -v
```

### Docker Image Management

```bash
# Build image
docker build -t myapp/backend -f docker/backend/Dockerfile .

# Run container
docker run -p 5000:5000 myapp/backend

# View running containers
docker ps

# View logs
docker logs -f <container-id>

# Execute command in container
docker exec -it <container-id> bash

# Remove stopped containers
docker container prune

# Remove unused images
docker image prune
```

### Database Operations

```bash
# Run migrations
docker-compose exec backend dotnet ef database update

# Backup database
docker-compose exec postgres pg_dump -U myapp myapp > backup.sql

# Restore database
docker-compose exec -T postgres psql -U myapp myapp < backup.sql

# Connect to database
docker-compose exec postgres psql -U myapp myapp
```

### Monitoring

```bash
# View Prometheus metrics
open http://localhost:9090

# View Grafana dashboards
open http://localhost:3001  # admin/admin

# Query logs with Loki
docker-compose exec loki logcli query '{job="backend"}'
```

## Dockerfile Best Practices

### Multi-Stage Build Pattern

```dockerfile
# Build stage
FROM <build-image> AS build
WORKDIR /app
COPY package.json .
RUN install-dependencies
COPY . .
RUN build-app

# Runtime stage
FROM <runtime-image> AS final
WORKDIR /app
COPY --from=build /app/dist .
EXPOSE 8080
CMD ["start-app"]
```

### Security Best Practices

```dockerfile
# 1. Use official minimal images
FROM node:20-alpine  # Not node:latest

# 2. Create non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# 3. Health checks
HEALTHCHECK --interval=30s --timeout=3s \
  CMD curl -f http://localhost/health || exit 1

# 4. Explicit versions
FROM postgres:15-alpine  # Not postgres:latest
```

### Optimization

```dockerfile
# 1. Layer caching - copy dependencies first
COPY package.json package-lock.json ./
RUN npm ci

# 2. Copy source code after (changes more frequently)
COPY . .

# 3. Use .dockerignore
# .dockerignore:
# node_modules
# .git
# *.log
```

## docker-compose.yml Pattern

```yaml
version: '3.8'

services:
  service-name:
    build:
      context: .
      dockerfile: docker/service/Dockerfile
    container_name: myapp-service
    environment:
      - VAR_NAME=value
    ports:
      - "8080:8080"
    volumes:
      - data:/var/lib/data
    depends_on:
      dependency:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health"]
      interval: 30s
      timeout: 3s
      retries: 3
    restart: unless-stopped
    networks:
      - app-network

volumes:
  data:

networks:
  app-network:
    driver: bridge
```

## CI/CD Pipeline Pattern

```yaml
name: CI/CD

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run tests
        run: npm test

  build:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Docker image
        run: docker build -t app:${{ github.sha }} .
      - name: Push to registry
        run: docker push app:${{ github.sha }}

  deploy:
    needs: build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Deploy to production
        run: ./scripts/deploy.sh
```

## Environment Management

### .env.example Template

```bash
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/myapp

# Keycloak
KEYCLOAK_URL=http://localhost:8080
KEYCLOAK_REALM=myapp

# Application
ASPNETCORE_ENVIRONMENT=Development
VITE_API_URL=http://localhost:5000

# AI
CLAUDE_API_KEY=your-api-key-here
```

### Usage

```bash
# Copy template
cp .env.example .env

# Edit with real values
nano .env

# Load in docker-compose
docker-compose --env-file .env up
```

## Health Checks

### Backend (C# ASP.NET Core)

```csharp
app.MapGet("/health", () => Results.Ok(new
{
    Status = "healthy",
    Timestamp = DateTime.UtcNow
}));

app.MapGet("/health/ready", async (DbContext db) =>
{
    await db.Database.CanConnectAsync()
        ? Results.Ok(new { Status = "ready" })
        : Results.ServiceUnavailable();
});
```

### Docker Health Check

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1
```

### docker-compose Health Check

```yaml
services:
  backend:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 10s
```

## Secrets Management

### Development (.env files)

```bash
# .env (git-ignored)
DATABASE_PASSWORD=dev_password
CLAUDE_API_KEY=sk-...
```

### Production (HashiCorp Vault)

```bash
# Store secret
vault kv put secret/database password=prod_password

# Read secret in app
export DB_PASSWORD=$(vault kv get -field=password secret/database)
```

### Kubernetes (Sealed Secrets)

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
type: Opaque
data:
  database-password: <base64-encoded>
  api-key: <base64-encoded>
```

## Monitoring Setup

### Prometheus + Grafana

```yaml
services:
  prometheus:
    image: prom/prometheus:latest
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
    ports:
      - "9090:9090"

  grafana:
    image: grafana/grafana:latest
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
    ports:
      - "3001:3000"
    depends_on:
      - prometheus
```

### Alert Rules

```yaml
# prometheus.yml
rule_files:
  - 'alerts.yml'

# alerts.yml
groups:
  - name: app_alerts
    rules:
      - alert: HighErrorRate
        expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.05
        annotations:
          summary: "High error rate detected"
```

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker-compose logs backend

# Inspect container
docker inspect <container-id>

# Check health
docker ps  # Look at STATUS column
```

### Can't Connect to Database

```bash
# Check database is running
docker-compose ps postgres

# Check logs
docker-compose logs postgres

# Test connection
docker-compose exec postgres psql -U myapp -c "SELECT 1"
```

### Disk Space Issues

```bash
# Remove unused containers
docker container prune

# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# View disk usage
docker system df
```

### Performance Issues

```bash
# Check resource usage
docker stats

# Limit resources in docker-compose.yml
deploy:
  resources:
    limits:
      cpus: '1.0'
      memory: 512M
```

## Best Practices

1. **Use Multi-Stage Builds** - Smaller images, faster builds
2. **Non-Root Users** - Security best practice
3. **Health Checks** - Enable automatic restart on failure
4. **Resource Limits** - Prevent one service from consuming all resources
5. **Secrets Management** - Never commit secrets to git
6. **Image Tagging** - Use specific versions, not `latest`
7. **Logging** - Log to stdout, aggregate with Loki
8. **Monitoring** - Prometheus + Grafana for observability
9. **Backups** - Automate database backups
10. **Documentation** - Runbooks for common operations

## References

### Solution-Specific
- `planning-mds/INCEPTION.md` - Tech stack
- `planning-mds/architecture/SOLUTION-PATTERNS.md` - DevOps patterns
- `docs/operations/deployment-guide.md` - Deployment runbook

### Generic
- `agents/devops/references/devops-best-practices.md`

### External Resources
- [Docker Documentation](https://docs.docker.com/)
- [docker-compose Documentation](https://docs.docker.com/compose/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)

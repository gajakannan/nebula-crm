# DevOps Agent

Complete specification and resources for the DevOps builder agent role.

## Overview

The DevOps Agent is responsible for containerization, CI/CD pipelines, deployment automation, and infrastructure management during Phase C and ongoing operations.

**Key Principle:** Infrastructure as Code. Automate Everything. Environment Parity.

---

## Quick Start

### 1. Set Up Local Development Environment

```bash
# Create docker-compose.yml
# Start all services
docker-compose up

# Initialize database
docker-compose exec backend dotnet ef database update
```

### 2. Create Dockerfiles

```bash
# Backend Dockerfile
touch src/Nebula.Api/Dockerfile

# Frontend Dockerfile
touch nebula-ui/Dockerfile
```

### 3. Set Up CI/CD

```bash
# GitHub Actions
mkdir -p .github/workflows
touch .github/workflows/ci-cd.yml
```

---

## Agent Structure

```
devops/
├── SKILL.md              # Main agent specification
├── README.md             # This file
├── references/           # DevOps best practices
│   ├── docker-best-practices.md
│   ├── cicd-guide.md
│   ├── kubernetes-guide.md (if using K8s)
│   └── monitoring-guide.md
└── scripts/              # Automation scripts
    ├── deploy-dev.sh
    ├── deploy-staging.sh
    ├── deploy-prod.sh
    └── rollback.sh
```

---

## Core Responsibilities

1. **Local Dev Environment** - docker-compose for all services
2. **Dockerfiles** - Optimized, secure container images
3. **CI/CD Pipelines** - Automated build, test, deploy
4. **Deployment Automation** - One-command deployments
5. **Secrets Management** - Secure secret storage and injection
6. **Monitoring & Logging** - Centralized logs and metrics
7. **Infrastructure as Code** - Terraform/ARM templates
8. **Runbooks** - Operational documentation

---

## Technology Stack

- **Containers:** Docker, docker-compose
- **CI/CD:** GitHub Actions, Jenkins, GitLab CI
- **Infrastructure:** Terraform, ARM, CloudFormation
- **Secrets:** Azure Key Vault, AWS Secrets Manager
- **Monitoring:** Prometheus, Grafana, Application Insights
- **Logging:** ELK Stack, CloudWatch Logs

---

## Key Files

### docker-compose.yml (Project Root)

All services for local development:
- PostgreSQL database
- Keycloak (authentication)
- Temporal (workflows)
- Backend API
- Frontend UI

### Dockerfiles

**Backend:** `src/Nebula.Api/Dockerfile`
- Multi-stage build
- Alpine base image
- Non-root user

**Frontend:** `nebula-ui/Dockerfile`
- Build with Node
- Serve with nginx
- Non-root user

### CI/CD Configuration

**GitHub Actions:** `.github/workflows/ci-cd.yml`
- Build and test on PR
- Deploy dev on merge to develop
- Deploy staging/prod on merge to main (with approval)

---

## Common Commands

### Local Development

```bash
# Start all services
docker-compose up

# Start in background
docker-compose up -d

# View logs
docker-compose logs -f backend

# Stop all services
docker-compose down

# Rebuild containers
docker-compose up --build

# Run migrations
docker-compose exec backend dotnet ef database update
```

### Docker

```bash
# Build image
docker build -t nebula-backend:latest -f src/Nebula.Api/Dockerfile .

# Run container
docker run -p 5000:80 nebula-backend:latest

# Push to registry
docker push ghcr.io/username/nebula-backend:latest
```

### Deployment

```bash
# Deploy to dev
./scripts/deploy-dev.sh

# Deploy to staging (requires approval)
./scripts/deploy-staging.sh

# Deploy to prod (requires approval)
./scripts/deploy-prod.sh

# Rollback
./scripts/rollback.sh production v1.2.3
```

---

## Definition of Done

### Local Development Done
- [ ] docker-compose.yml includes all services
- [ ] One command starts entire stack
- [ ] Database migrations run automatically
- [ ] Hot reload works for development
- [ ] README has setup instructions

### Dockerfiles Done
- [ ] Multi-stage builds
- [ ] Optimized image size (<500MB backend)
- [ ] Non-root user
- [ ] Health checks included
- [ ] Security scan passes

### CI/CD Done
- [ ] Build pipeline configured
- [ ] Test pipeline configured
- [ ] Security scanning configured
- [ ] Deployment pipeline configured
- [ ] Notifications configured

### Production Ready Done
- [ ] Monitoring and alerting configured
- [ ] Logs centralized
- [ ] Backups automated
- [ ] Disaster recovery tested
- [ ] Runbooks documented

---

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker-compose logs backend

# Check health
docker-compose ps

# Rebuild
docker-compose build backend
docker-compose up backend
```

### Database Connection Issues

```bash
# Verify database is running
docker-compose ps postgres

# Check connection string
docker-compose exec backend env | grep ConnectionStrings

# Test connection
docker-compose exec postgres psql -U nebula -d nebula
```

### CI/CD Pipeline Failures

- Check build logs in CI/CD platform
- Run build locally: `dotnet build`
- Run tests locally: `dotnet test`
- Check for environment-specific issues

---

## Version History

**Version 1.0** - 2026-01-28 - Initial DevOps agent

---

## Next Steps

1. Create docker-compose.yml for local development
2. Create Dockerfiles for each service
3. Set up CI/CD pipeline
4. Configure deployment automation
5. Set up monitoring and logging

**Remember:** Automate everything. Environment parity is critical.

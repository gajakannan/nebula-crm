# Builder Agent Roles — Quick Reference

This document provides a quick index of all agent roles. For detailed specifications, see individual role files.

---

## Core Builder Agents

### [Product Manager](./product-manager.md)
**Scope:** Define requirements, user stories, acceptance criteria
**Phase:** Phase A (Product Manager Mode)
**Key Outputs:** Vision, personas, epics, MVP stories, screen specs

### [Architect](./architect.md)
**Scope:** Design system architecture, data models, API contracts
**Phase:** Phase B (Architect/Tech Lead Mode)
**Key Outputs:** Service boundaries, data model, workflow rules, API contracts, authorization model

### [Backend Developer](./backend-dev.md)
**Scope:** Implement ASP.NET Core APIs, EF Core, domain logic, migrations
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** C# code, EF migrations, REST APIs, domain services, repositories

### [Frontend Developer](./frontend-dev.md)
**Scope:** Implement React/TypeScript UI, TanStack Query, forms
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** React components, hooks, forms, routing, state management

---

## Quality & Operations Agents

### [Quality Engineer](./quality-engineer.md)
**Scope:** Write unit/integration/E2E tests, define quality standards
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** Test suites, test plans, quality metrics, coverage reports

### [DevOps](./devops.md)
**Scope:** Manage Docker, docker-compose, deployment configs
**Phase:** Phase C (Implementation Mode) + ongoing
**Key Outputs:** Dockerfiles, docker-compose.yml, CI/CD configs, deployment scripts

### [Security](./security.md)
**Scope:** Review auth/authz, security practices, auditability
**Phase:** Phase B (design review) + Phase C (implementation review)
**Key Outputs:** Security review reports, threat models, audit recommendations

---

## Review & Quality Agents

### [Code Reviewer](./code-reviewer.md)
**Scope:** Review code quality, standards, test coverage
**Phase:** Phase C (Implementation Mode) — per pull request
**Key Outputs:** Code review comments, approval/rejection, improvement recommendations

---

## Documentation Agents

### [Technical Writer](./technical-writer.md)
**Scope:** Create API docs, README, runbooks
**Phase:** Phase C (Implementation Mode) + ongoing
**Key Outputs:** API documentation, README.md, runbooks, developer guides

### [Blogger](./blogger.md)
**Scope:** Publish dev logs, decisions, lessons learned
**Phase:** All phases (continuous)
**Key Outputs:** Blog posts, development logs, technical articles

---

## Agent Activation by Phase

### Phase A — Product Manager Mode
- **Product Manager** (primary)
- Blogger (optional)

### Phase B — Architect/Tech Lead Mode
- **Architect** (primary)
- Security (design review)
- Blogger (optional)

### Phase C — Implementation Mode
- **Backend Developer**
- **Frontend Developer**
- **Quality Engineer**
- **DevOps**
- **Technical Writer**
- Code Reviewer (per PR)
- Security (testing & validation)
- Blogger (continuous)

---

## Quick Decision Tree

**Need to define what to build?** → Product Manager
**Need to design how to build it?** → Architect
**Need to implement backend logic?** → Backend Developer
**Need to implement UI?** → Frontend Developer
**Need to test functionality?** → Quality Engineer
**Need to deploy or containerize?** → DevOps
**Need security validation?** → Security
**Need documentation?** → Technical Writer
**Need code quality review?** → Code Reviewer
**Need to blog about progress?** → Blogger

---

## Next Steps

1. Review [README.md](./README.md) for agent collaboration model and handoff rules
2. Read the specific agent specification file for your current task
3. Use templates from `templates/` directory for consistent deliverables
4. Follow quality gates and definition of done for each agent role

# Builder Agent Roles — Quick Reference

This document provides a quick index of all agent roles. For detailed specifications, see individual role files.

---

## Core Builder Agents

### [Product Manager](./product-manager/SKILL.md) ✅ **COMPLETE**
**Scope:** Define requirements, user stories, acceptance criteria
**Phase:** Phase A (Product Manager Mode)
**Key Outputs:** Vision, personas, epics, MVP stories, screen specs
**Status:** SKILL.md, README.md, references/, scripts/ all created

### [Architect](./architect/SKILL.md) ✅ **COMPLETE**
**Scope:** Design system architecture, data models, API contracts
**Phase:** Phase B (Architect/Tech Lead Mode)
**Key Outputs:** Service boundaries, data model, workflow rules, API contracts, authorization model
**Status:** SKILL.md, README.md, references/, scripts/ all created

### [Backend Developer](./backend-developer/SKILL.md) ✅ **COMPLETE**
**Scope:** Implement ASP.NET Core APIs, EF Core, domain logic, migrations
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** C# code, EF migrations, REST APIs, domain services, repositories
**Status:** SKILL.md, README.md, references/ (3 files) created; scripts optional

### [Frontend Developer](./frontend-developer/SKILL.md) ✅ **COMPLETE**
**Scope:** Implement React/TypeScript UI, TanStack Query, forms
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** React components, hooks, forms, routing, state management
**Status:** SKILL.md, README.md created; references/scripts optional

---

## Quality & Operations Agents

### [Quality Engineer](./quality-engineer/SKILL.md) ✅ **COMPLETE**
**Scope:** Write unit/integration/E2E tests, define quality standards
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** Test suites, test plans, quality metrics, coverage reports
**Status:** SKILL.md, README.md, references/ (1 file) created; scripts optional

### [DevOps](./devops/SKILL.md) ✅ **COMPLETE**
**Scope:** Manage Docker, docker-compose, deployment configs
**Phase:** Phase C (Implementation Mode) + ongoing
**Key Outputs:** Dockerfiles, docker-compose.yml, CI/CD configs, deployment scripts
**Status:** SKILL.md, README.md, references/ (1 file) created; scripts optional

### [Security](./security/SKILL.md) ✅ **COMPLETE**
**Scope:** Review auth/authz, security practices, auditability
**Phase:** Phase B (design review) + Phase C (implementation review)
**Key Outputs:** Security review reports, threat models, audit recommendations
**Status:** SKILL.md expanded (full workflow + severity model + deliverables), README.md, references/ (4 files), scripts/ created

---

## Review & Quality Agents

### [Code Reviewer](./code-reviewer/SKILL.md) ✅ **COMPLETE**
**Scope:** Review code quality, standards, test coverage
**Phase:** Phase C (Implementation Mode) — per pull request
**Key Outputs:** Code review comments, approval/rejection, improvement recommendations
**Status:** SKILL.md, README.md, references/ (1 file) created; scripts optional

---

## Documentation Agents

### [Technical Writer](./technical-writer/SKILL.md) ✅ **COMPLETE**
**Scope:** Create API docs, README, runbooks
**Phase:** Phase C (Implementation Mode) + ongoing
**Key Outputs:** API documentation, README.md, runbooks, developer guides
**Status:** SKILL.md expanded (artifact workflows + quality gates + collaboration rules), README.md, references/ (1 file) created

### [Blogger](./blogger/SKILL.md) ✅ **COMPLETE**
**Scope:** Publish dev logs, decisions, lessons learned
**Phase:** All phases (continuous)
**Key Outputs:** Blog posts, development logs, technical articles
**Status:** SKILL.md expanded (editorial workflow + safety guardrails + publishing quality gates), README.md, references/ (1 file) created

---

### [AI Engineer](./ai-engineer/SKILL.md) ✅ **COMPLETE**
**Scope:** Build AI intelligence layer - LLM integrations, agentic workflows, MCP servers
**Phase:** Phase C (Implementation Mode)
**Key Outputs:** Python code in neuron/, model integrations, MCP servers, agent workflows, prompts
**Status:** SKILL.md created

---

## Completion Status: 11/11 Agents Complete

All builder agent specifications are now complete and ready to use.

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
- **Backend Developer** (C# .NET APIs, business logic)
- **Frontend Developer** (React UI, forms)
- **AI Engineer** (Python neuron/ layer, LLMs, agents, MCP)
- **Quality Engineer** (Tests)
- **DevOps** (Docker, deployment)
- **Technical Writer** (Documentation)
- Code Reviewer (per PR)
- Security (testing & validation)
- Blogger (continuous)

---

## Quick Decision Tree

**Need to define what to build?** → Product Manager
**Need to design how to build it?** → Architect
**Need to implement backend logic?** → Backend Developer (C# .NET)
**Need to implement UI?** → Frontend Developer (React)
**Need AI features or agents?** → AI Engineer (Python neuron/)
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
5. **Ready to build!** All agents are available for project development

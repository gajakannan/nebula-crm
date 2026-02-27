# Generic Agent Roles - Agent-Driven Builder Framework

## Purpose

This directory contains **generic, reusable** agent role definitions for building software using an agent-driven builder methodology. Agents are designed to be copied across projects unchanged.

## Framework Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                     Agent-Driven Builder Framework                         │
│                    Plan → Spec → Design → Build → Ship                      │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│  ACTION FLOW (User-Facing Compositions)                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  init       │ Bootstrap project structure                                   │
│  plan       │ Phase A (PM) → Phase B (Architect) [2 approval gates]         │
│  build      │ Backend + Frontend + AI* + QA + DevOps → Review [2 gates]     │
│  feature    │ Single vertical slice (Backend + Frontend + AI* + QA + DevOps) │
│  review     │ Code Reviewer + Security [1 gate]                             │
│  validate   │ Architect + PM validation (read-only)                         │
│  test       │ Quality Engineer testing workflow                             │
│  document   │ Technical Writer documentation                                │
│  blog       │ Blogger dev logs & articles                                   │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
* AI Engineer runs when stories include AI/LLM/MCP scope. Architect orchestrates implementation sequencing.
                                        ↓
                              Actions compose Agents
                                        ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│  AGENTS (Role-Based Specialists) - 11 Agents                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Planning Phase (Phase A-B)                                                 │
│  ├─ product-manager    │ Requirements, stories, acceptance criteria         │
│  └─ architect          │ Design, data model, API contracts, patterns        │
│                                                                              │
│  Implementation Phase (Phase C)                                             │
│  ├─ backend-developer  │ C# APIs, EF Core, domain logic (engine/)           │
│  ├─ frontend-developer │ React, TypeScript, forms (experience/)             │
│  ├─ ai-engineer        │ Python LLMs, agents, MCP, workflows (neuron/) 🧠   │
│  ├─ quality-engineer   │ Unit, integration, E2E tests                       │
│  └─ devops             │ Docker, docker-compose, deployment                 │
│                                                                              │
│  Quality & Documentation                                                    │
│  ├─ code-reviewer      │ Code quality, standards, patterns                  │
│  ├─ security           │ OWASP, auth/authz, vulnerabilities                 │
│  ├─ technical-writer   │ API docs, README, runbooks                         │
│  └─ blogger            │ Dev logs, technical articles                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
                                        ↓
                        Agents read from & write to
                                        ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│  SOLUTION-SPECIFIC CONTENT (planning-mds/)                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Single Source of Truth                                                     │
│  └─ BLUEPRINT.md       │ Master specification (Sections 0-6)                │
│                                                                              │
│  Domain Knowledge                                                           │
│  └─ domain/            │ Glossary, competitive analysis                     │
│                                                                              │
│  Architecture                                                               │
│  ├─ architecture/                                                           │
│  │  ├─ SOLUTION-PATTERNS.md  │ Solution-specific patterns ⭐               │
│  │  ├─ decisions/            │ ADRs                                         │
│  │  └─ ...                   │ Data model docs, testing strategy, patterns  │
│                                                                              │
│  API Contracts                                                              │
│  └─ api/               │ OpenAPI specifications (*.yaml)                    │
│                                                                              │
│  Examples & Artifacts                                                       │
│  ├─ examples/          │ Personas, features, stories, screens               │
│  ├─ security/          │ Threat models, security reviews                    │
│  └─ ...                                                                      │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

─────────────────────────────────────────────────────────────────────────────

  9 Actions · 11 Agents · 1 Source of Truth (BLUEPRINT.md)
  SOLUTION-PATTERNS.md for institutional knowledge
  neuron/ for AI intelligence layer 🧠
```

## How to Use

### For Users
1) Use **[Action Flow](./actions/README.md)** to compose agents for common workflows (init, plan, build, review, etc.)
2) Actions provide user-friendly entry points that orchestrate agents automatically
3) Example: `"Run the plan action"` → PM (Phase A) → Architect (Phase B) with approval gates

### For New Projects
1) Copy `agents/` into a new repo
2) Create a fresh `planning-mds/` for the new solution
3) Use the agents as-is; all solution-specific content must live in `planning-mds/`

## Single Source of Truth

All agents read requirements from `planning-mds/BLUEPRINT.md` and related planning artifacts.

## Agent Action Flow

The **[Action Flow](./actions/README.md)** provides a user-friendly interface for composing agents to accomplish complete workflows:

- **[init](./actions/init.md)** - Bootstrap a new project
- **[plan](./actions/plan.md)** - Complete planning (Phase A + B)
- **[build](./actions/build.md)** - Full implementation workflow
- **[feature](./actions/feature.md)** - Single vertical slice
- **[review](./actions/review.md)** - Code and security review
- **[validate](./actions/validate.md)** - Validate alignment
- **[test](./actions/test.md)** - Test suite development
- **[document](./actions/document.md)** - Technical documentation
- **[blog](./actions/blog.md)** - Development logs and articles

See **[actions/README.md](./actions/README.md)** for complete action flow documentation.

## Tech Stack Assumptions

The framework is opinionated about delivery practices and provides stack-specific references in some agent guides. In this repo, the default references assume a modern .NET + React + PostgreSQL stack. If you adopt a different stack, keep the agent roles but replace the stack-specific reference guides and examples with your own.

See `agents/TECH-STACK-ADAPTATION.md` for a concise adaptation guide.
See `agents/SKILL-CHANGELOG.md` for skill definition change history.

---

If you’re starting a new project, see `planning-mds/README.md` for a minimal setup checklist.

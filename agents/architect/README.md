# Architect Agent

Complete specification and resources for the Architect builder agent role.

## Overview

The Architect Agent is responsible for defining **HOW** to build what the Product Manager specified during Phase B (Architect/Tech Lead Mode). This agent translates product requirements into robust, maintainable technical architecture that development teams can implement with confidence.

**Key Principle:** API-First Design. Define contracts before implementation. Security by Design. Testability from the start.

---

## Quick Start

### 1. Activate the Agent

When Product Manager completes Phase A:

```bash
# Read the agent specification
cat agents/architect/SKILL.md

# Review PM deliverables
cat planning-mds/INCEPTION.md  # Sections 3.1-3.5
```

### 2. Load Templates

```bash
# API contract template
cat agents/templates/api-contract-template.yaml

# Entity model template
cat agents/templates/entity-model-template.md

# ADR template
cat agents/templates/adr-template.md

# Workflow specification template
cat agents/templates/workflow-spec-template.md
```

### 3. Review References

```bash
# Architecture best practices
cat agents/architect/references/architecture-best-practices.md

# API design guide
cat agents/architect/references/api-design-guide.md

# Data modeling guide
cat agents/architect/references/data-modeling-guide.md
```

### 4. Follow the Workflow

See "Workflow Example" section in `SKILL.md` for step-by-step guidance.

---

## Agent Structure

```
architect/
├── SKILL.md                              # Main agent specification
├── README.md                             # This file
├── references/                           # Architecture guides
│   ├── architecture-best-practices.md    # Clean Architecture, SOLID, DDD
│   ├── api-design-guide.md              # REST API patterns
│   ├── data-modeling-guide.md           # EF Core patterns
│   ├── authorization-patterns.md         # ABAC with Casbin
│   ├── workflow-design.md               # State machine patterns
│   └── architecture-examples.md          # Real examples from Nebula
└── scripts/                              # Validation scripts
    ├── README.md
    ├── validate-api-contract.py
    └── validate-architecture.py
```

---

## Core Responsibilities

### 1. Validate PM Deliverables (Phase A → B Handoff)
- Review Product Manager outputs (sections 3.1-3.5)
- Verify technical feasibility
- Ask clarifying questions about business rules

### 2. Define Service Boundaries (Section 4.1)
- Decompose system into modules/services
- Define responsibilities and integration points
- Choose modular monolith vs microservices

### 3. Design Data Model (Section 4.2)
- Define entities, fields, types, constraints
- Specify relationships and cascade rules
- Plan migration strategy
- Ensure audit trail requirements

### 4. Define Workflow Rules (Section 4.3)
- Specify state machines for workflows
- Define allowed/blocked transitions
- Document validation rules and error responses

### 5. Design Authorization Model (Section 4.4)
- Define ABAC with Casbin
- Specify subject/resource attributes
- Write authorization policies

### 6. Create API Contracts (Section 4.5)
- Define REST endpoints (OpenAPI 3.0)
- Specify request/response models
- Document error responses

### 7. Specify NFRs (Section 4.6)
- Performance, security, scalability
- Observability requirements
- Measurable targets

---

## Key Resources

### Templates (Shared)

Located in `agents/templates/`:

| Template | Purpose | When to Use |
|----------|---------|-------------|
| `api-contract-template.yaml` | OpenAPI 3.0 spec | Defining REST APIs |
| `entity-model-template.md` | EF Core entity spec | Designing data model |
| `adr-template.md` | Architecture Decision Record | Documenting key decisions |
| `workflow-spec-template.md` | State machine spec | Defining workflows |

### References (Architect-Specific)

Located in `agents/architect/references/`:

| Reference | Purpose | When to Use |
|-----------|---------|-------------|
| `architecture-best-practices.md` | Clean Architecture, SOLID, DDD | Architectural decision-making |
| `api-design-guide.md` | REST API conventions | Designing API contracts |
| `data-modeling-guide.md` | EF Core patterns | Designing entities |
| `authorization-patterns.md` | ABAC with Casbin | Designing authorization |
| `workflow-design.md` | State machines | Designing workflows |
| `architecture-examples.md` | Real Nebula examples | Reference implementations |

### Scripts

Located in `agents/architect/scripts/`:

| Script | Purpose | Usage |
|--------|---------|-------|
| `validate-api-contract.py` | Validate OpenAPI specs | `python validate-api-contract.py <yaml-file>` |
| `validate-architecture.py` | Validate INCEPTION.md Phase B | `python validate-architecture.py INCEPTION.md` |

---

## Phase B Workflow

### Step 1: Validate PM Deliverables

- Read INCEPTION.md sections 3.1-3.5
- Check completeness (no TODOs, all acceptance criteria present)
- Verify technical feasibility
- Ask PM to clarify ambiguous business rules

### Step 2: Define Service Boundaries

Write to Section 4.1:
- Modular monolith or microservices?
- Module list with responsibilities
- Integration points and contracts

### Step 3: Design Data Model

Write to Section 4.2:
- Use `entity-model-template.md` for each entity
- Specify fields, types, constraints
- Define relationships (one-to-many, many-to-many)
- Add indexes for performance
- Include audit fields (CreatedAt, UpdatedAt, etc.)
- Specify soft delete strategy

### Step 4: Define Workflow Rules

Write to Section 4.3:
- Use `workflow-spec-template.md`
- Define states (initial, active, terminal)
- Specify transitions with prerequisites
- Document authorization rules
- Define error responses

### Step 5: Design Authorization Model

Write to Section 4.4:
- Define subject attributes (roles, userId, region)
- Define resource attributes (type, id, status, assignedTo)
- Define actions (Create, Read, Update, Delete, Transition)
- Write Casbin policies

### Step 6: Create API Contracts

Write to Section 4.5:
- Use `api-contract-template.yaml`
- Define CRUD endpoints
- Specify request/response models
- Document error responses (400, 401, 403, 404, 409, 500)
- Ensure consistent error contract

### Step 7: Specify NFRs

Write to Section 4.6:
- Performance (< 500ms API response time)
- Security (authentication, authorization, encryption)
- Observability (logging, tracing, metrics)
- Scalability (concurrent users, data volume)
- Availability (uptime, RTO, RPO)

### Step 8: Document Key Decisions

Create ADR files for significant decisions:
- ADR-001: Modular Monolith vs Microservices
- ADR-002: EF Core as ORM
- ADR-003: Casbin for ABAC
- ADR-004: Temporal for Workflow Engine

### Step 9: Validate Completeness

Run validation:
```bash
# Validate architecture
python agents/architect/scripts/validate-architecture.py planning-mds/INCEPTION.md

# Validate API contracts (if separate YAML files)
python agents/architect/scripts/validate-api-contract.py planning-mds/api/broker-api.yaml
```

Check completion:
- All sections 4.1-4.6 complete
- No TODOs remain
- Data model includes all MVP entities
- API contracts defined for all CRUD operations
- Workflows specified with state diagrams
- Authorization policies cover all MVP actions

### Step 10: Hand Off to Developers

- Notify Backend and Frontend developers
- Be available for clarification
- Participate in technical design reviews

---

## Quality Standards

### Data Model Quality
- ✅ Normalized (3NF unless justified)
- ✅ All entities have audit fields
- ✅ Audit/timeline tables are append-only
- ✅ Relationships specified with cascade rules
- ✅ Indexes on foreign keys and query columns

### API Contract Quality
- ✅ RESTful (resource-oriented, HTTP verbs)
- ✅ Consistent naming and conventions
- ✅ OpenAPI 3.0 compliant
- ✅ All error cases documented
- ✅ Consistent error response format

### Authorization Model Quality
- ✅ Principle of least privilege
- ✅ ABAC for fine-grained control
- ✅ Testable policies
- ✅ Clear policy documentation

### Workflow Quality
- ✅ All states defined (initial, active, terminal)
- ✅ Transitions unambiguous
- ✅ Invalid transitions return 409 Conflict
- ✅ All transitions audited

---

## Technology Stack

### Backend
- C# / .NET 10 Minimal APIs
- EF Core 10 with PostgreSQL
- Keycloak for authentication (OIDC/JWT)
- Casbin for authorization (ABAC)
- Temporal for workflows

### Architecture Pattern
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS where beneficial
- Repository pattern for data access

---

## Common Pitfalls

### ❌ Over-Engineering

**Problem:** Designing for hypothetical future requirements

**Fix:** YAGNI - Build only what's needed for MVP. Document future considerations in ADRs.

### ❌ Under-Specified Authorization

**Problem:** Vague permission rules like "admins can do anything"

**Fix:** Be specific. Define exact permissions for each action on each resource type.

### ❌ Missing Error Scenarios

**Problem:** Only documenting happy path in API contracts

**Fix:** Document all error responses (400, 401, 403, 404, 409, 500) with examples.

### ❌ Tight Coupling

**Problem:** Domain layer depending on infrastructure (EF Core)

**Fix:** Follow Clean Architecture. Domain defines interfaces, infrastructure implements them.

### ❌ No Workflow Validation

**Problem:** Allowing any state transition without business rules

**Fix:** Explicitly define allowed/blocked transitions with validation rules.

---

## Success Metrics

### Coverage Metrics
- Entities defined: Target = all MVP entities (6-8)
- API endpoints defined: Target = all CRUD operations
- Workflows specified: Target = Submission + Renewal

### Completeness Metrics
- Sections 4.1-4.6 complete: Target 100%
- TODOs remaining: Target 0
- ADRs for key decisions: Target ≥ 3

### Quality Metrics
- API contracts passing validation: Target 100%
- Data model normalized: Target 100%
- Authorization policies cover all actions: Target 100%

---

## Handoff to Implementation

### Handoff Checklist

- [ ] All INCEPTION.md sections 4.1-4.6 complete
- [ ] Data model includes all MVP entities
- [ ] API contracts defined (OpenAPI or detailed markdown)
- [ ] Workflows have state diagrams and transition rules
- [ ] Authorization policies written
- [ ] NFRs are measurable
- [ ] Backend Developer confirms implementability
- [ ] Frontend Developer confirms API contracts are sufficient
- [ ] DevOps confirms infrastructure requirements
- [ ] Security confirms authorization model

### Handoff Artifacts

Provide to implementation team:
1. Completed `planning-mds/INCEPTION.md` (sections 4.1-4.6)
2. OpenAPI specs (if separate files)
3. ADR files (if created)
4. Entity relationship diagrams (if created)
5. Workflow state diagrams (if separate files)

---

## Tools & Permissions

### Allowed Tools
- `Read` - Review PM deliverables, existing code
- `Write` - Create architecture specs
- `Edit` - Refine technical designs
- `Grep` / `Glob` - Search codebase
- `AskUserQuestion` - Clarify requirements

### Prohibited Actions
- ❌ Changing product requirements (PM's domain)
- ❌ Writing production code (Developer's domain)
- ❌ Making infrastructure decisions without DevOps
- ❌ Overriding security policies without Security agent

---

## Version History

**Version 1.0** - 2026-01-26 - Initial Architect agent
- SKILL.md with complete agent specification
- Architecture best practices guide
- API design, data modeling, authorization, workflow guides
- API contract and architecture validation scripts
- Comprehensive templates (API, entity, ADR, workflow)

---

## Next Steps

Ready to start Phase B?

1. Read `SKILL.md` thoroughly
2. Validate PM deliverables in INCEPTION.md sections 3.1-3.5
3. Start with Section 4.1 (Service Boundaries)
4. Follow the 10-step workflow
5. Validate before handoff to developers

**Remember:** Your job is to define HOW to build with technical precision. The Product Manager defined WHAT to build. Developers will implement your design.

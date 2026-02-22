# Application Assembly Plan (Phase C)

**Owner:** Architect
**Status:** Draft
**Last Updated:** 2026-02-21

## Purpose

Provide a sequenced, cross‑role plan to assemble the Nebula CRM implementation without blocking dependencies, while keeping F0 (Dashboard) and F1 (Broker Relationship Management) aligned to the approved architecture and contracts.

## Scope

- Modules: BrokerRelationship, Submission, Renewal, TaskManagement, TimelineAudit, IdentityAuthorization, Dashboard
- Features in scope: F0, F1 (MVP)
- Out of scope: External portal, analytics/insights beyond F0/F1, deployment hardening

## Required Inputs (Must Exist)

- `planning-mds/INCEPTION.md` (Phase A complete)
- `planning-mds/api/nebula-api.yaml` (OpenAPI contract)
- `planning-mds/security/authorization-matrix.md` and `planning-mds/security/policies/policy.csv`
- `planning-mds/architecture/SOLUTION-PATTERNS.md`
- JSON Schemas in `planning-mds/schemas/`
- ADRs in `planning-mds/architecture/decisions/`

## Assembly Order (High Level)

### 1) Foundation (Shared Infrastructure)

**Backend**
- Set up .NET solution structure per module boundaries.
- Wire ProblemDetails error contract and consistent error codes.
- Implement Casbin ABAC enforcement middleware and policy loading.
- Implement schema validation layer (NJsonSchema) using `planning-mds/schemas/`.

**Frontend**
- Scaffold app shell, routing, auth gate, error boundaries.
- Apply design tokens and base layout components.

**QA/DevOps**
- Baseline test harness (unit + integration) and CI validation gate alignment.

**Checkpoint A:**
- Auth middleware in place, basic request validation, and API skeleton running with health endpoint.

### 2) Data Model + Migrations

**Backend**
- Implement core entities (Broker, Contact, Submission, Renewal, ActivityTimelineEvent, WorkflowTransition, Task) with audit fields and soft delete.
- Apply indexes per `planning-mds/architecture/data-model.md` and `planning-mds/architecture/SOLUTION-PATTERNS.md`.

**Checkpoint B:**
- Migrations applied and seed/reference data ready for local dev.

### 3) Contract‑First API Implementation

**Backend**
- Implement endpoints defined in `planning-mds/api/nebula-api.yaml`.
- Enforce schema validation on request payloads.
- Ensure policy checks for every endpoint (ABAC + role action).

**Frontend**
- Generate client types from OpenAPI (if used) or align TS types to schemas.

**Checkpoint C:**
- Contract validation passes; all endpoints return correct shape for dummy data.

### 4) Feature Assembly (F0 + F1)

Follow the feature assembly plan to build complete vertical slices.

### 5) System Hardening

- Logging, tracing, and metrics wiring.
- Performance checks (p95 targets in stories).
- Security checks: verify deny‑by‑default behavior for missing policies.

**Checkpoint D:**
- End‑to‑end smoke tests across F0/F1 succeed.

## Handoff Contracts

- Backend → Frontend: stable API endpoints and response schemas.
- Backend → QA: test data seeding and deterministic error codes.
- Architect → All: updated contracts + schemas are authoritative.

## Exit Criteria (Phase C)

- All F0 and F1 stories pass acceptance criteria.
- API contract validation passes.
- ABAC enforcement verified with basic integration tests.
- UI renders core flows without fallback errors.

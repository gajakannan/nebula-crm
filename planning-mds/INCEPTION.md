# Insurance CRM — Single Source of Truth Master Build Spec (Inception Prompt)

You are an AI development partner helping me create a production-grade Commercial P&C Insurance CRM.
This document is the ONLY source of truth. Do not rely on any other spec packages unless explicitly told to.
If information is missing, ask questions or mark TODOs — do NOT invent business rules.

## 0) How we will work (Process + Roles)

We will proceed in three explicit phases. You must stay within the current phase.

### Phase A — Product Manager Mode (PM/BA)
Goal: define product requirements (vision, users, epics, features, stories, acceptance criteria).
Output: a complete PM-ready spec section with minimal technical assumptions.

### Phase B — Architect/Tech Lead Mode (Dev/Arch)
Goal: define technical approach (stack, architecture, data model, workflows, APIs, security, NFRs).
Output: a complete build-ready technical spec section that maps to Phase A.

### Phase C — Implementation Mode
Goal: generate the actual repository and code in incremental vertical slices with tests.
Output: production-quality code + migrations + OpenAPI + tests + run instructions.

IMPORTANT RULES:

- Single source of truth is THIS document.
- If a requirement isn’t written here, do not implement it.
- If there is ambiguity, list questions and propose minimal default assumptions labeled clearly.
- No scope creep. Build only what’s specified for the current phase.

---

## 1) Product Context

### 1.1 What we’re building

Name: Nebula

Domain: Commercial Property & Casualty Insurance CRM

Purpose: Manage broker/MGA relationships, accounts, submissions, renewals, activities, reminders, and broker insights.


### 1.2 Target users

- Distribution & Marketing (primary users)
- Underwriters (workflow updates + collaboration)
- Broker Relationship Managers
- MGA Program Managers
- Admin

External users (future): MGA users with limited access (not in Phase 0 MVP unless explicitly stated).

### 1.3 Core entities (baseline)

- Account (insured business)
- Broker
- MGA
- Program
- Contact
- Submission
- Renewal
- Document (versioned)
- ActivityTimelineEvent (immutable audit/timeline)
- WorkflowTransition (immutable append-only transitions)
- UserProfile (internal profile driven by Keycloak subject)
- UserPreference (separate table)

### 1.4 Critical workflows (baseline)

Submission: Received → Triaging → WaitingOnBroker → ReadyForUWReview → InReview → Quoted → BindRequested → Bound (or Declined/Withdrawn)
Renewal: Created → Early → OutreachStarted → InReview → Quoted → Bound (or Lost/Lapsed)

Non-negotiables:

- Audit logging and timeline events are mandatory for every mutation and every workflow transition.
- Role-based visibility is mandatory: InternalOnly vs BrokerVisible content separation.

---

## 2) Technology and Platform (baseline decisions)

These are locked unless explicitly changed later:

- Frontend: React 18 + TypeScript + Vite + Tailwind + shadcn/ui
- State: TanStack Query, React Hook Form, AJV (JSON Schema validation)
- Backend: C# / .NET 10 Minimal APIs
- Database: PostgreSQL (dev + prod)
- ORM: EF Core 10
- AuthN: Keycloak (OIDC/JWT)
- AuthZ: Casbin ABAC enforced server-side
- Workflow engine: Temporal (included in Phase 0)
- Deploy: Docker + docker-compose
- Agentic ops: Python MCP server (later, secondary interface; never source of truth)
- Testing:
  - Frontend: Vitest (unit/component), Playwright (E2E browser), @axe-core/playwright (a11y), Lighthouse CI (performance)
  - Backend: xUnit (unit/integration), Testcontainers (database), Bruno CLI (API collections), Coverlet (coverage), k6 (load)
  - AI/Neuron: pytest (unit/integration/evaluation), pytest-benchmark (performance), custom evaluation metrics
  - Cross-cutting: Pact.NET (contract testing), OWASP ZAP (security), Trivy (vulnerability scanning)

Architecture constraints:

- Clean Architecture: Domain → Application → Infrastructure → API
- Application depends on repository interfaces; Infrastructure implements with EF.
- Audit/timeline/transition tables are append-only (immutable).
- Reference data uses tables + deterministic seed data (not hardcoded enums when configurable).
- API error contract must be consistent across all services.

---

## 3) Phase A — Product Manager Spec (Current Baseline)

Status: This repository is currently focused on the agent builder framework. Nebula application implementation is planned but not started. The following Phase A content is the baseline specification for the reference app.

### 3.1 Vision + Non-Goals

- Vision:
  - Provide a single operating system for commercial P&C distribution teams to manage broker/MGA relationships, accounts, submissions, renewals, and activity history with strong auditability.
  - Replace spreadsheet/email-driven processes with structured workflows, permission-aware collaboration, and traceable transitions.
  - Deliver a modular foundation that supports AI-assisted workflows later without changing the source-of-truth system.

- Non-goals (explicit):
  - No external broker/MGA self-service portal in MVP.
  - No advanced analytics dashboards beyond basic broker insight summaries in MVP.
  - No document OCR/intelligence workflows in MVP.
  - No claims management module in MVP.
  - No multi-region regulatory rules engine in MVP.

### 3.2 Personas

- Persona 1: Distribution user
  - Primary job: intake and triage submissions, manage broker interactions, track pipeline movement.
  - Success metric: reduced intake turnaround and fewer handoff delays.

- Persona 2: Underwriter
  - Primary job: review triaged submissions, provide quote/bind decisions, maintain decision traceability.
  - Success metric: faster, consistent movement from ReadyForUWReview to Quoted/Bound or Declined.

- Persona 3: Relationship Manager
  - Primary job: maintain broker/account relationships, contacts, and timeline context.
  - Success metric: complete broker/account context available in one place.

- Persona 4: Program Manager
  - Primary job: oversee MGA/program-level relationships and program performance signals.
  - Success metric: program-level visibility with clear ownership and activity traces.

### 3.3 Features

**Note:** Features are written as separate markdown files in `planning-mds/features/` directory using the feature template (`agents/templates/feature-template.md`). Each feature includes: business objective, scope, success metrics, and links to related stories.

**MVP Features:**
- [F1: Broker & MGA Relationship Management](features/F1-broker-relationship-management.md) - Draft ready
- F2: Account 360 & Activity Timeline - Planned
- F3: Submission Intake Workflow - Planned
- F4: Renewal Pipeline - Planned
- F5: Task Center + Reminders - Planned
- F6: Broker Insights - Planned

### 3.4 MVP Features and Stories (vertical-slice friendly)

**Note:** User stories are written as separate markdown files organized by feature in `planning-mds/stories/{feature-name}/` directories using the story template (`agents/templates/story-template.md`). Each story includes: description, acceptance criteria, edge cases, roles, and audit/timeline requirements.

**MVP Stories (Feature F1: Broker Relationship Management):**
- [S1: Create Broker](stories/F1-broker-relationship-management/S1-create-broker.md) - Draft ready
- [S2: Search Brokers](stories/F1-broker-relationship-management/S2-search-brokers.md) - Draft ready
- S3: Read Broker (Broker 360 View) - Planned
- S4: Update Broker - Planned
- S5: Delete Broker - Planned
- S6: Manage Broker Contacts - Planned
- S7: View Broker Activity Timeline - Planned

**Story Index:** See `planning-mds/stories/STORY-INDEX.md` for auto-generated summary of all stories (if generated).

Reference examples also live under `planning-mds/examples/stories/`.

### 3.5 Screen list (MVP)

- Navigation Shell
- Broker List
- Broker 360
- Task Center (optional MVP)
- Admin minimal (roles/policies optional MVP)

Screen baseline details:
- Navigation Shell: authenticated app shell, role-aware navigation, global search entry, notifications placeholder.
- Broker List: sortable/filterable list, quick search, create action, status tags.
- Broker 360: profile header, contacts, hierarchy/program links, immutable timeline panel.
- Task Center: assigned tasks, due dates, simple status states, reminder hooks.
- Admin minimal: role assignment visibility and policy diagnostics (read-focused in MVP).

---

## 4) Phase B — Architect Spec (Public Baseline)

This section defines the build-ready technical baseline for the reference implementation.

### 4.1 Service boundaries

- Architecture shape: modular monolith (single deployable) with clean module boundaries and internal APIs.
- BrokerRelationship module:
  - Owns Broker, Contact, MGA, Program relationship mappings.
  - Handles broker/contact CRUD, hierarchy links, broker search.
- Account module:
  - Owns Account profile and account-level relationship views.
  - Provides account context for submissions and renewals.
- Submission module:
  - Owns Submission aggregate and Submission workflow operations.
  - Enforces transition gates/checklists before status moves.
- Renewal module:
  - Owns Renewal aggregate and Renewal workflow operations.
  - Tracks outreach and renewal-specific lifecycle.
- TimelineAudit module:
  - Owns ActivityTimelineEvent and WorkflowTransition append-only records.
  - Provides timeline query/read APIs.
- IdentityAuthorization module:
  - Validates Keycloak tokens and resolves subject attributes.
  - Enforces Casbin ABAC policies at API/application boundaries.

(MVP may be a modular monolith with clean boundaries.)

### 4.2 Data model (detailed)

Define tables/fields for:

- Broker, Contact, UserProfile, UserPreference, ActivityTimelineEvent, WorkflowTransition
- Reference tables + seed strategy

Core entities (minimum baseline):
- Account
  - Id (uuid), Name, Industry, PrimaryState, Status
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
- Broker
  - Id (uuid), LegalName, LicenseNumber, State, Status
  - MgaId (nullable), PrimaryProgramId (nullable)
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
- MGA
  - Id (uuid), Name, ExternalCode, Status
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
- Program
  - Id (uuid), Name, ProgramCode, MgaId
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
- Contact
  - Id (uuid), BrokerId (nullable), AccountId (nullable), FullName, Email, Phone, Role
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
- Submission
  - Id (uuid), AccountId, BrokerId, ProgramId (nullable), CurrentStatus, EffectiveDate, PremiumEstimate
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
- Renewal
  - Id (uuid), AccountId, BrokerId, SubmissionId (nullable), CurrentStatus, RenewalDate
  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
- UserProfile
  - Subject (Keycloak sub), Email, DisplayName, Department, Region, RolesJson
  - CreatedAt, UpdatedAt
- UserPreference
  - Id (uuid), Subject, PreferenceKey, PreferenceValueJson
  - CreatedAt, UpdatedAt
- ActivityTimelineEvent (append-only)
  - Id (uuid), EntityType, EntityId, EventType, EventPayloadJson, ActorSubject, OccurredAt
- WorkflowTransition (append-only)
  - Id (uuid), WorkflowType, EntityId, FromState, ToState, Reason, ActorSubject, OccurredAt

Reference tables and seed strategy:
- ReferenceState, ReferenceIndustry, ReferenceTaskStatus, ReferenceSubmissionStatus, ReferenceRenewalStatus
- Deterministic EF seed/migration scripts with idempotent upsert semantics.
- Runtime writes to reference tables are restricted to admin-only actions.

### 4.3 Workflow rules

Define allowed transitions and gating validations (Submission and Renewal).

Submission workflow transitions:
- Received -> Triaging
- Triaging -> WaitingOnBroker or ReadyForUWReview
- WaitingOnBroker -> ReadyForUWReview
- ReadyForUWReview -> InReview
- InReview -> Quoted or Declined
- Quoted -> BindRequested or Withdrawn
- BindRequested -> Bound or Declined

Renewal workflow transitions:
- Created -> Early
- Early -> OutreachStarted
- OutreachStarted -> InReview
- InReview -> Quoted or Lost
- Quoted -> Bound or Lapsed

Transition rules and validations:
- Invalid transition pairs return HTTP 409 with ErrorResponse code `invalid_transition`.
- Missing required checklist/data preconditions return HTTP 409 with ErrorResponse code `missing_transition_prerequisite`.
- Subject must have permission for the requested transition action (otherwise HTTP 403).
- Every successful transition appends:
  - one WorkflowTransition record
  - one ActivityTimelineEvent record
- Transition records are immutable; corrections happen via compensating transitions.

### 4.4 Authorization model (ABAC)

Define subject attributes (from UserProfile), resource attributes, actions.
Define minimal policies for Phase 0 and Phase 1.

Subject attributes (from JWT + UserProfile):
- subjectId, roles, department, region, internalUser flag

Resource attributes:
- resourceType, ownerAccountId, brokerId, programId, internalOnly flag, workflowState

Actions (examples):
- broker:create, broker:read, broker:update, broker:delete
- contact:create, contact:read, contact:update, contact:delete
- submission:transition, renewal:transition
- timeline:read

Policy baseline:
- Internal distribution and relationship roles can create/read/update Broker and Contact.
- Underwriters have read access to broker/account context and transition access within underwriting stages.
- Admin has broad management access including policy administration.
- InternalOnly resources are denied to non-internal subjects.
- Enforcement is server-side only via Casbin middleware and application guards.

### 4.5 API Contracts

Define endpoints + request/response contracts + error contract.

Primary OpenAPI contract:
- `planning-mds/api/nebula-api.yaml`

Entity coverage in API surface:
- Account, Broker, MGA, Program, Contact, Submission, Renewal, ActivityTimelineEvent, WorkflowTransition

MVP endpoint pattern examples:
- GET `/api/brokers`
- POST `/api/brokers`
- GET `/api/brokers/{brokerId}`
- PUT `/api/brokers/{brokerId}`
- DELETE `/api/brokers/{brokerId}`
- GET `/api/contacts`
- POST `/api/contacts`
- GET `/api/submissions/{submissionId}/transitions`
- POST `/api/submissions/{submissionId}/transitions`
- GET `/api/renewals/{renewalId}/transitions`
- POST `/api/renewals/{renewalId}/transitions`

Error contract:
- All non-success responses return `ErrorResponse` with at least `code`, `message`, optional `details`, and `traceId`.

### 4.6 Observability + NFRs

Logging, tracing, metrics, performance, security.

Observability baseline:
- Structured logging with correlation id and subject id where available.
- Distributed traces for API request path and DB calls.
- Metrics: request latency, error rate, transition counts, authorization denials.

Performance:
- API read endpoints: p95 < 300ms under nominal load.
- API write/transition endpoints: p95 < 500ms under nominal load.
- List endpoints support pagination and bounded query size.

Security:
- OIDC JWT validation against Keycloak issuer/audience.
- Casbin ABAC for all protected actions.
- Secrets via environment variables; no hardcoded credentials in code or config.
- Immutable audit trail for every mutation and transition.

Availability:
- Target service availability 99.9% for production environments.
- Health/readiness endpoints for orchestration.

Scalability:
- Horizontal API scaling behind stateless app instances.
- Database indexing on high-cardinality lookup fields (license number, status, foreign keys).
- Transition/timeline tables partition-ready for growth.

---

## 5) Phase C — Implementation Plan (locked order)

We will implement progressively over 4–5 weeks, starting with Phase 0 foundation by Jan 30.

### Phase 0 Foundation — required components

Must include Postgres + Keycloak + Casbin + Temporal in docker-compose.
Backend: Clean Architecture scaffold + auth + ABAC wiring + error contract + timeline append-only.
Frontend: authenticated shell.
Tests: auth test + timeline append test.

No scope creep in Phase 0:

- No submission/renewal UI
- No document upload implementation
- No analytics
- No external MGA portal
- No Python MCP server

Definition of Done for Phase 0:

- [ ] docker-compose includes Postgres, Keycloak, Casbin policy source, Temporal dependencies
- [ ] backend scaffolded with clean architecture boundaries and auth/ABAC wiring
- [ ] frontend authenticated shell with protected routes
- [ ] consistent error contract implemented across API endpoints
- [ ] append-only timeline and workflow transition persistence in place
- [ ] baseline tests passing: auth enforcement + timeline append + one transition flow
- [ ] run instructions and local setup documented

---

## 6) Immediate Next Step

AI: We are currently in Phase A (PM/BA Mode).
Start by asking me the smallest set of questions needed to fill in sections 3.1–3.5.

Then propose a first-pass draft of:

- Vision
- Non-goals
- Personas
- Epics
- MVP stories with acceptance criteria (Broker vertical slice)

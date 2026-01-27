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
Name: BrokerHub
Domain: Commercial Property & Casualty Insurance CRM (Surplus Lines / Non-Admitted market)
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
- State: TanStack Query, React Hook Form, Zod
- Backend: C# / ASP.NET Core 8 REST APIs
- Database: PostgreSQL (dev + prod)
- ORM: EF Core 8
- AuthN: Keycloak (OIDC/JWT)
- AuthZ: Casbin ABAC enforced server-side
- Workflow engine: Temporal (included in Phase 0)
- Deploy: Docker + docker-compose
- Agentic ops: Python MCP server (later, secondary interface; never source of truth)

Architecture constraints:

- Clean Architecture: Domain → Application → Infrastructure → API
- Application depends on repository interfaces; Infrastructure implements with EF.
- Audit/timeline/transition tables are append-only (immutable).
- Reference data uses tables + deterministic seed data (not hardcoded enums when configurable).
- API error contract must be consistent across all services.

---

## 3) Phase A — Product Manager Spec (TO BE FILLED IN)

AI: Help me fill these out with strong structure, but do not invent underwriting or regulatory rules beyond what I provide.

### 3.1 Vision + Non-Goals

- Vision: TODO
- Non-goals (explicit): TODO

### 3.2 Personas

- Persona 1: Distribution user — TODO
- Persona 2: Underwriter — TODO
- Persona 3: Relationship Manager — TODO
- Persona 4: Program Manager — TODO

### 3.3 Epics

Provide a list of epics with objectives.

- Epic E1: Broker & MGA Relationship Management — TODO
- Epic E2: Account 360 & Activity Timeline — TODO
- Epic E3: Submission Intake Workflow — TODO
- Epic E4: Renewal Pipeline — TODO
- Epic E5: Task Center + Reminders — TODO
- Epic E6: Broker Insights — TODO

### 3.4 MVP Features and Stories (vertical-slice friendly)

For each story include: description, acceptance criteria, roles, and audit/timeline requirements.

- MVP Story S1: Broker CRUD + Broker 360 — TODO
- MVP Story S2: Broker Hierarchy — TODO
- MVP Story S3: Broker Contacts — TODO
- MVP Story S4: Timeline for Broker mutations — TODO

(We will expand later to Account/Submission/Renewal after proving the approach.)

### 3.5 Screen list (MVP)

- Navigation Shell
- Broker List
- Broker 360
- Task Center (optional MVP)
- Admin minimal (roles/policies optional MVP)

TODO: define fields and layout.

---

## 4) Phase B — Architect Spec (TO BE FILLED IN)

AI: Once Phase A is complete, you will help convert it into build-ready technical specs.

### 4.1 Service boundaries

- Broker service (or module) — TODO
- Account service — TODO
- Submission service — TODO
- Renewal service — TODO
- Timeline/Audit module — TODO

(MVP may be a modular monolith with clean boundaries.)

### 4.2 Data model (detailed)

Define tables/fields for:

- Broker, Contact, UserProfile, UserPreference, ActivityTimelineEvent, WorkflowTransition
- Reference tables + seed strategy

TODO: fill in.

### 4.3 Workflow rules

Define allowed transitions and gating validations (Submission and Renewal).

TODO: fill in (must include 409 responses on invalid transitions, missing checklist items, etc.)

### 4.4 Authorization model (ABAC)

Define subject attributes (from UserProfile), resource attributes, actions.
Define minimal policies for Phase 0 and Phase 1.

TODO: fill in.

### 4.5 API Contracts

Define endpoints + request/response contracts + error contract.

TODO: fill in.

### 4.6 Observability + NFRs

Logging, tracing, metrics, performance, security.

TODO: fill in.

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

TODO: finalize checklist (infra, backend, frontend, tests).

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

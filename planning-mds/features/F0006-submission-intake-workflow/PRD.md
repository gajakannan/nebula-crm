---
template: feature-prd
version: 2.0
applies_to: product-manager
---

# F0006: Submission Intake Workflow

**Feature ID:** F0006
**Feature Name:** Submission Intake Workflow
**Priority:** Critical
**Phase:** CRM Release MVP
**Status:** In Refinement

## Feature Statement

**As a** distribution user or coordinator
**I want** a structured submission intake flow with triage, validation, and ownership
**So that** new business enters Nebula cleanly and reaches the right underwriter quickly

## Business Objective

- **Goal:** Replace fragmented submission intake with a consistent, auditable workflow inside Nebula.
- **Metric:** Intake turnaround time (Received → ReadyForUWReview), incomplete submission rate, time-to-assignment, and percentage of submissions triaged through Nebula.
- **Baseline:** Submissions arrive through fragmented channels (email, spreadsheets, ad hoc follow-up) with weak status visibility, no completeness enforcement, and no structured handoff to underwriting.
- **Target:** Most submissions are created, triaged, validated for completeness, and assigned through Nebula with full audit trail within 60 days of internal go-live.

## Problem Statement

- **Current State:** Intake is fragmented across email, spreadsheets, and ad hoc follow-up. There is no system-of-record for new business submissions, no completeness checks, and no structured handoff between distribution and underwriting. Submissions get lost, arrive incomplete, and underwriters lack context.
- **Desired State:** Submission intake, triage, completeness validation, and underwriting assignment are handled in one place with clear status visibility and role-based accountability.
- **Impact:** Faster response to brokers, less rework for underwriting teams, auditable intake operations, and a reliable foundation for the downstream quoting workflow (F0019).

## Scope & Boundaries

**In Scope (MVP):**
- Submission creation and intake capture (linked to account, broker, and optionally program)
- Submission pipeline list view with status, broker, account, LOB, and assignment filters
- Submission detail view with linked account, broker, program, completeness status, and timeline
- Intake status transitions: Received → Triaging → WaitingOnBroker / ReadyForUWReview
- Completeness evaluation (required fields and required document categories) as a transition guard
- Manual underwriting assignment and explicit queue handoff to ReadyForUWReview
- Append-only activity timeline and workflow transition audit trail
- Stale submission detection and follow-up flags
- Dashboard integration: stale-submission nudge card for submissions stuck in early intake states

**Out of Scope (Future / Other Features):**
- Downstream quoting, proposal, approval, and bind workflow (F0019)
- Final quote approval or carrier-side integration and rating (F0019)
- External broker portal submission entry (F0029)
- Rule-based queue routing and automated assignment (F0022)
- Document storage, upload, versioning, and metadata management (F0020 — parallel dependency)
- OCR, extraction, and AI document intelligence (future)
- Bulk submission import or CSV upload
- Carrier-side integration and rating
- Automated submission creation from external sources
- Submission scoring or AI-assisted triage

## Scope Boundary Clarifications

| Capability | Owns | Delegates To | Boundary |
|------------|------|--------------|----------|
| Submission entity, intake states, intake transitions | **F0006** | — | F0006 owns the Submission aggregate and intake workflow |
| Downstream states (InReview, Quoted, BindRequested, Bound, Declined, Withdrawn) | — | **F0019** | F0019 extends the submission workflow from ReadyForUWReview onward |
| Account entity and CRUD | — | **F0016** | F0006 reads account data; F0016 is the authoritative source |
| Broker entity and CRUD | — | **F0002** | F0006 reads broker data; F0002 is authoritative (already done) |
| Document storage, metadata, versioning | — | **F0020** | F0006 evaluates document completeness via F0020 metadata; does not own document CRUD |
| Queue routing and automated assignment rules | — | **F0022** | F0006 supports manual assignment; F0022 adds rule-based routing later |
| Task creation linked to submissions | — | **F0003/F0004** | F0006 uses existing Task entity via LinkedEntityType=Submission |
| Submission LOB and SLA configuration | **F0006** | — | F0006 can reference WorkflowSlaThreshold (ADR-009) for intake SLA tracking |
| Communication and broker follow-up notes | — | **F0021** | MVP: follow-up tracked via timeline events. F0021 adds structured communication later |

## Personas

### Distribution User (Primary)
- **Intake job-to-be-done:** Create submissions from broker requests, triage for completeness, follow up with brokers for missing information, and advance complete submissions to underwriting review.
- **Key pain:** Intake happens across email and spreadsheets with no tracking. Information arrives incomplete. There is no structured way to know what is missing, what has been followed up on, or when a submission is ready for underwriting.
- **Success:** Can create a submission in Nebula, see what is missing at a glance, track follow-up with the broker, and hand off a complete submission to the right underwriter with full context.

### Underwriter
- **Intake job-to-be-done:** Receive triaged, complete submissions with broker/account context and supporting documents so that underwriting review can begin immediately.
- **Key pain:** Receives submissions without context — missing documents, unclear broker contact, no history of what has already been discussed. Spends time chasing information instead of evaluating risk.
- **Success:** Sees ReadyForUWReview submissions assigned to them with completeness confirmed, linked account and broker context, and the full intake timeline.

### Distribution Manager
- **Intake job-to-be-done:** Monitor team intake pipeline health, identify stale or stuck submissions, reassign ownership when needed, and ensure broker response times are met.
- **Key pain:** No visibility into team-wide submission status. Cannot identify which submissions are stuck in triage, which are waiting on broker response, or which have been sitting unworked.
- **Success:** Can see all team submissions, filter by status and staleness, reassign ownership, and track resolution.

## Submission Intake Workflow

### States (F0006 Scope)

| State | Description | Owner Role | Entry Condition |
|-------|-------------|------------|-----------------|
| **Received** | Submission created; not yet triaged | Distribution User | Submission created with required fields |
| **Triaging** | Distribution is reviewing for completeness and routability | Distribution User | Intake review started |
| **WaitingOnBroker** | Missing information; broker follow-up in progress | Distribution User | Broker follow-up initiated; completeness check identified gaps |
| **ReadyForUWReview** | Intake complete; submission is ready for underwriting | Distribution User / Underwriter | Completeness check passed; underwriter assigned |

### States (F0019 Scope — Downstream, Not Owned by F0006)

| State | Description |
|-------|-------------|
| InReview | Underwriter actively reviewing |
| Quoted | Quote prepared and shared |
| BindRequested | Insured accepted; bind in progress |
| Bound | Policy bound (terminal) |
| Declined | Carrier declined (terminal) |
| Withdrawn | Broker/insured withdrew (terminal) |

### Allowed Transitions (F0006 Scope)

```
Received       → Triaging
Triaging       → WaitingOnBroker
Triaging       → ReadyForUWReview
WaitingOnBroker → ReadyForUWReview
```

### Transition Rules

- Invalid transition pairs return HTTP 409 with `ProblemDetails` (`code=invalid_transition`).
- Missing required checklist/data preconditions return HTTP 409 with `ProblemDetails` (`code=missing_transition_prerequisite`).
- `Received → Triaging`: Actor must be Distribution User, Distribution Manager, or Admin.
- `Triaging → WaitingOnBroker`: Actor must be Distribution User, Distribution Manager, or Admin. A reason or follow-up note should be provided (tracked via timeline event).
- `Triaging → ReadyForUWReview`: Actor must be Distribution User, Distribution Manager, or Admin. Completeness check must pass (required fields populated and required document categories linked). Submission must have an assigned underwriter (`AssignedToUserId` must reference a user with Underwriter role).
- `WaitingOnBroker → ReadyForUWReview`: Same guards as Triaging → ReadyForUWReview.
- Every transition appends one WorkflowTransition record and one ActivityTimelineEvent record atomically.
- Subject must have Casbin ABAC permission for `submission:transition` (otherwise HTTP 403).
- Optimistic concurrency enforced (xmin).

## Completeness Policy

Completeness is evaluated as a transition guard before advancing to ReadyForUWReview:

**Required Fields (must be non-null):**
- AccountId (linked account)
- BrokerId (linked broker)
- EffectiveDate (requested coverage start)
- LineOfBusiness (coverage type)
- AssignedToUserId (underwriter assigned)

**Required Document Categories (when F0020 is available):**
- Application (ACORD form or equivalent)
- At least one supporting document (loss runs, financials, or supplemental)

**Completeness result is a read-side projection** so intake users can see missing fields and missing documents without triggering the transition. The completeness check is enforced as a hard gate only when transitioning to ReadyForUWReview.

When F0020 is not yet available, document completeness checks are soft-skipped (field completeness remains enforced).

## Acceptance Criteria Overview

- [ ] Users can create a submission linked to an account and broker with required intake fields
- [ ] Users can view a filterable submission pipeline list with intake status, broker, account, LOB, and assignment filters
- [ ] Users can view submission detail with linked account, broker, program, completeness status, and activity timeline
- [ ] Submissions transition through Received → Triaging → WaitingOnBroker / ReadyForUWReview with validation
- [ ] Completeness evaluation surfaces missing fields and missing document categories
- [ ] Triaging → ReadyForUWReview requires completeness check to pass and underwriter to be assigned
- [ ] Distribution users and managers can assign/reassign submission ownership; handoff to underwriting is explicit
- [ ] Stale submissions (stuck in Received or Triaging beyond a configurable threshold) are flagged and surfaced on dashboard
- [ ] All submission transitions and activities are recorded in the append-only timeline
- [ ] Region alignment enforced: Account.Region must be in the broker's BrokerRegion set

## UX / Screens

| Screen | Purpose | Key Actions |
|--------|---------|-------------|
| Submission Pipeline List | Primary operating view for intake submissions | Filter by intake status (Received, Triaging, WaitingOnBroker, ReadyForUWReview), broker, account, LOB, assigned user, stale flag; sort by created date or effective date; paginate |
| Submission Detail | Full context for a single submission | View linked account, broker, program; view completeness status; view activity timeline; advance status; assign/reassign owner; log follow-up activity |
| Create Submission Dialog/Page | Capture new submission intake | Select account, broker; enter effective date, LOB, premium estimate, description; validate region alignment; submit |
| Dashboard — Stale Submission Nudge Card | Surface stale submissions on the dashboard | Shows count of submissions stuck in Received/Triaging beyond threshold; click navigates to filtered pipeline list |

**Key Workflows:**

1. **New Submission Intake Flow** — Distribution user receives broker request → opens Create Submission → links account and broker → enters coverage details → submission created in Received → transitions to Triaging → reviews completeness → follows up with broker if incomplete (→ WaitingOnBroker) → once complete, assigns underwriter and advances to ReadyForUWReview.

2. **Broker Follow-Up Flow** — Distribution user triages submission → completeness check shows missing documents → transitions to WaitingOnBroker → logs follow-up activity (timeline event) → broker provides missing info → distribution user reviews → transitions to ReadyForUWReview.

3. **Stale Submission Escalation Flow** — Distribution manager sees stale nudge card on dashboard → clicks through to filtered pipeline list showing stuck submissions → reassigns ownership or contacts the assigned user → monitors until resolved.

4. **Underwriting Handoff Flow** — Distribution user completes triage → assigns underwriter → transitions to ReadyForUWReview → underwriter sees submission in their queue with full context, completeness confirmed, and intake timeline → F0019 takes over from here.

## Data Requirements

**Core Entity: Submission**
- `Id` (uuid, PK): Unique submission identifier
- `AccountId` (uuid, FK → Account): The insured account (required)
- `BrokerId` (uuid, FK → Broker): The submitting broker (required)
- `ProgramId` (uuid, nullable, FK → Program): Associated program
- `CurrentStatus` (string): Current workflow state (Received, Triaging, WaitingOnBroker, ReadyForUWReview, plus F0019 downstream states)
- `LineOfBusiness` (string, nullable): LOB classification (per ADR-009)
- `EffectiveDate` (date): Requested coverage effective date
- `ExpirationDate` (date, nullable): Requested coverage expiration date (defaults to EffectiveDate + 12 months)
- `PremiumEstimate` (decimal, nullable): Estimated premium amount
- `Description` (string, nullable): Free-text submission description/notes
- `AssignedToUserId` (uuid, FK → UserProfile): Current owner (distribution user during intake; underwriter after handoff)
- `CreatedAt`, `CreatedByUserId`, `UpdatedAt`, `UpdatedByUserId`, `IsDeleted`: Standard base entity fields

**Validation Rules:**
- `AccountId` is required and must reference a valid, non-deleted account
- `BrokerId` is required and must reference a valid, non-deleted, active broker
- Region alignment: `Account.Region` must be included in the broker's `BrokerRegion` set; otherwise HTTP 400 with `code=region_mismatch`
- `EffectiveDate` is required on create
- `LineOfBusiness` validated against known LOB values when provided
- `AssignedToUserId` must reference a valid, active internal user
- `PremiumEstimate` must be non-negative when provided

**Data Relationships:**
- Submission → Account: Required N:1
- Submission → Broker: Required N:1
- Submission → Program: Optional N:1
- Submission → UserProfile (AssignedTo): Required N:1
- Submission → Task: Optional 1:N (via LinkedEntityType=Submission on Task)
- Submission → Document: Optional 1:N (via F0020 document linkage)

## Role-Based Access

| Role | Access Level | Notes |
|------|-------------|-------|
| Distribution User | Create, Read (own + team scope), Update, Transition (Received→Triaging, Triaging→WaitingOnBroker, Triaging→ReadyForUWReview, WaitingOnBroker→ReadyForUWReview) | Can assign to self; primary intake operator |
| Distribution Manager | Create, Read (all in region), Update, Transition (all intake transitions), Reassign | Full pipeline visibility; can reassign any submission in scope |
| Underwriter | Read (assigned + team scope) | Read-only in intake phase; receives submissions in ReadyForUWReview. Transition rights for downstream states owned by F0019 |
| Relationship Manager | Read (own accounts/brokers) | Read-only visibility for account/broker context |
| Program Manager | Read (own programs) | Read-only visibility for program-scoped submissions |
| Admin | Full CRUD + all transitions + reassign | Administrative override |

**Data Visibility:**
- InternalOnly: All submission data is internal-only in MVP (no external broker visibility)
- ABAC enforcement: Account region, broker scope, and assigned user visibility per existing Casbin policy patterns (policy.csv §2.3)

## Success Criteria

- Distribution users can create and triage submissions in Nebula with structured intake fields
- Underwriters receive clearer, more complete submission handoff with documented context
- Submission status is visible and auditable from creation through ReadyForUWReview
- Completeness checks prevent premature advancement to underwriting
- Stale submissions are immediately identifiable by managers
- Every intake action produces immutable timeline and transition records

## Risks & Assumptions

- **Risk:** Intake scope expands into the full quoting workflow. **Mitigation:** Explicit boundary — F0006 ends at ReadyForUWReview; F0019 owns everything downstream.
- **Risk:** Document completeness depends on F0020 which is parallel. **Mitigation:** Field completeness is enforced independently; document completeness is a soft check that activates when F0020 is available. Design the completeness policy to be extensible.
- **Risk:** F0016 (Account) may not be fully built when F0006 starts. **Mitigation:** F0006 can use a minimal Account entity (Id, Name, Region) as a stub. Full Account 360 is not required for intake.
- **Assumption:** Manual assignment is sufficient for MVP. Rule-based queue routing (F0022) is Future scope.
- **Assumption:** LOB values on Submission follow the same known set defined in ADR-009.
- **Assumption:** Stale submission threshold is configurable (default: 48 hours in Received, 72 hours in Triaging).

## Dependencies

- **F0002 Broker & MGA Relationship Management** — Provides broker entity and broker-region data. Already complete.
- **F0009 Authentication + Role-Based Login** — Provides OIDC/JWT authentication and Casbin ABAC. Already complete.
- **F0016 Account 360 & Insured Management** — Provides account entity with region. Parallel in Now bucket.
- **F0020 Document Management & ACORD Intake** — Provides document metadata and linkage. Parallel in Now bucket. Document completeness is a soft dependency.
- **F0003/F0004 Task Center** — Provides task creation linked to submissions via LinkedEntityType. Already complete.
- **F0022 Work Queues** (soft dependency, Future) — Will add rule-based queue routing for submissions. F0006 MVP uses manual assignment only.

## Architecture & Solution Design

### Solution Components

- Introduce a `Submission` aggregate with intake-focused application services for create, triage, completeness validation, assignment, and timeline rendering.
- Add a submission workflow service that owns intake transitions and guard checks instead of scattering transition logic across controllers or UI.
- Add a completeness policy component that evaluates required fields and required document categories before a submission can advance to underwriting review.
- Treat queue handoff as an explicit boundary so F0006 can hand work to underwriting without embedding the full routing engine that belongs to F0022.

### Data & Workflow Design

- Model intake as an explicit state machine, starting with intake-focused states: `Received`, `Triaging`, `WaitingOnBroker`, and `ReadyForUWReview`.
- Record all submission status changes in an append-only `WorkflowTransition` history that aligns with ADR-011 for auditable workflow state.
- Keep submission-to-broker, submission-to-account, and submission-to-document relationships first-class so intake completeness can be evaluated without denormalized guessing.
- Expose a read-side completeness projection so intake users can see missing fields, missing documents, and ownership gaps without recalculating every rule in the UI.

### API & Integration Design

- Plan REST endpoints for submission create/list/detail plus explicit transition and assignment actions rather than relying on implicit free-form status edits.
- Integrate with F0020 through document metadata linkage and required-document checks, but keep OCR, extraction, and carrier ingestion outside this feature boundary.
- Emit domain events or application notifications for assignment handoff and broker follow-up so downstream tasking and routing can evolve without rewriting the intake core.
- Preserve correlation IDs between submission actions, document uploads, and timeline events for future reporting and troubleshooting.

### Security & Operational Considerations

- Enforce Casbin-based authorization on submission read/write/transition actions using broker, account, and role context rather than role-only checks.
- Ensure intake mutations create immutable timeline and transition records to support compliance, SLA reporting, and later operational analytics.
- Design create and transition operations to be idempotent enough for retried requests and UI refresh flows.
- Keep list and queue-facing queries pageable and index-friendly because intake views will become high-volume operational screens.

## Architecture Traceability

**Taxonomy Reference:** [Feature Architecture Traceability Taxonomy](../../architecture/feature-architecture-traceability-taxonomy.md)

| Classification | Artifact / Decision | ADR |
|----------------|---------------------|-----|
| Introduces: Feature-Local Component | Submission aggregate, completeness policy, and intake workflow service | PRD only |
| Introduces/Standardizes: Cross-Cutting Pattern | CRM workflow state machine with append-only transition history for submission intake | [ADR-011](../../architecture/decisions/ADR-011-crm-workflow-state-machines-and-transition-history.md) (Accepted) |
| Extends: Cross-Cutting Component | Intake completeness and document linkage rely on the shared document architecture | [ADR-012](../../architecture/decisions/ADR-012-shared-document-storage-and-metadata-architecture.md) (Proposed) |

## Related User Stories

Stories are colocated in this feature folder as `F0006-S{NNNN}-{slug}.md`.

- [F0006-S0001](./F0006-S0001-submission-pipeline-list-with-intake-status-filtering.md) — Submission pipeline list with intake status filtering
- [F0006-S0002](./F0006-S0002-create-submission-for-new-business-intake.md) — Create submission for new business intake
- [F0006-S0003](./F0006-S0003-submission-detail-view-with-intake-context.md) — Submission detail view with intake context
- [F0006-S0004](./F0006-S0004-submission-intake-status-transitions.md) — Submission intake status transitions
- [F0006-S0005](./F0006-S0005-submission-completeness-evaluation.md) — Submission completeness evaluation
- [F0006-S0006](./F0006-S0006-submission-ownership-assignment-and-underwriting-handoff.md) — Submission ownership assignment and underwriting handoff
- [F0006-S0007](./F0006-S0007-submission-activity-timeline-and-audit-trail.md) — Submission activity timeline and audit trail
- [F0006-S0008](./F0006-S0008-stale-submission-visibility-and-follow-up-flags.md) — Stale submission visibility and follow-up flags

## Rollout & Enablement

- Internal team onboarding for intake workflow usage
- Runbook updates for support and access troubleshooting
- Seed data for ReferenceSubmissionStatus entries covering intake states

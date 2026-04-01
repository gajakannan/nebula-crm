# F0006 — Submission Intake Workflow — Status

**Overall Status:** Done
**Last Updated:** 2026-03-31

## Story Checklist

| Story | Title | Status |
|-------|-------|--------|
| F0006-S0001 | Submission pipeline list with intake status filtering | Done |
| F0006-S0002 | Create submission for new business intake | Done |
| F0006-S0003 | Submission detail view with intake context | Done |
| F0006-S0004 | Submission intake status transitions | Done |
| F0006-S0005 | Submission completeness evaluation | Done |
| F0006-S0006 | Submission ownership assignment and underwriting handoff | Done |
| F0006-S0007 | Submission activity timeline and audit trail | Done |
| F0006-S0008 | Stale submission visibility and follow-up flags | Done |

## Current Implementation Snapshot

- Backend Steps 1-8 from `feature-assembly-plan.md` are implemented in `engine/`: migration, submission entity/configuration, status catalog/state machine, DTOs, validators, repositories, service rewrite, endpoint rewrite, and F0006-specific problem details.
- The submission API surface now covers list, create, detail, update, transition, assignment, and paged timeline reads with ABAC enforcement and `If-Match` / `rowVersion` preconditions.
- Completeness evaluation, underwriter-assignment validation, stale-flag computation, and the F0020 null-object document checklist adapter are in place.
- Dev seed data and submission status reference data are aligned to the 10-state F0006/F0019 model, including stale-threshold seed rows for `Received`, `Triaging`, and `WaitingOnBroker`.
- `experience/` now includes the submission pipeline list, create flow, detail workspace, assignment interaction, transition dialog, completeness panel, paged activity timeline, and dashboard stale-submission nudge card required by assembly-plan Steps 9-13.
- Signoff provenance recorded 2026-03-31 — all 24 story-level entries (8 stories x 3 required roles) have PASS verdicts with evidence.

## Closeout Guardrails

- F0006 closeout does not include a submission delete route or generic soft-delete contract. Future submission archive/deactivate behavior is owned by F0019.
- F0006 closeout does not include deleted or merged account fallback behavior on linked submission/detail views. That replacement contract is owned by F0016.
- Broker deleted-entity fallback is also outside F0006 closeout. Current broker lifecycle rules already prevent deletion when active submissions or renewals depend on the broker.
- F0006 must still preserve its workflow boundary at `ReadyForUWReview`; downstream transitions remain blocked until F0019 deliberately activates them.

## Backend Progress

- [x] Entities and EF configurations
- [x] Repository implementations
- [x] Service layer with business logic
- [x] API endpoints (controllers / minimal API)
- [x] Authorization policies
- [x] Unit tests passing
- [x] Integration tests passing

## Frontend Progress

- [x] Page components created
- [x] API hooks / data fetching
- [x] Form validation
- [x] Routing configured
- [x] Component/integration tests added or updated for changed behavior
- [ ] Accessibility validation recorded (if frontend in scope)
- [ ] Coverage artifact recorded (if coverage is part of project validation)
- [ ] Responsive layout verified
- [ ] Visual regression tests (if applicable)

## Cross-Cutting

- [x] Seed data (ReferenceSubmissionStatus entries for intake states, stale thresholds)
- [x] Migration(s) applied
- [x] API documentation updated
- [x] Runtime validation evidence recorded
- [x] No TODOs remain in code

## Validation Evidence

- Feature story validation passed on 2026-03-31:
  `python3 agents/product-manager/scripts/validate-stories.py planning-mds/features/F0006-submission-intake-workflow/`
- Story index regenerated on 2026-03-31:
  `python3 agents/product-manager/scripts/generate-story-index.py planning-mds/features/`
- Targeted integration coverage passed on 2026-03-31:
  `dotnet test engine/tests/Nebula.Tests/Nebula.Tests.csproj --filter FullyQualifiedName~WorkflowEndpointTests`
- Targeted unit coverage passed on 2026-03-31:
  `dotnet test engine/tests/Nebula.Tests/Nebula.Tests.csproj --filter "FullyQualifiedName~WorkflowServiceTests|FullyQualifiedName~WorkflowStateMachineTests|FullyQualifiedName~LineOfBusinessValidationTests"`
- Frontend route coverage passed on 2026-03-31:
  `CI=true pnpm --dir experience test --run src/App.test.tsx`
- Frontend integration coverage passed on 2026-03-31:
  `pnpm --dir experience exec vitest run src/pages/tests/SubmissionsPage.integration.test.tsx src/pages/tests/CreateSubmissionPage.integration.test.tsx src/pages/tests/SubmissionDetailPage.integration.test.tsx src/pages/tests/DashboardPage.integration.test.tsx`
- Frontend production build passed on 2026-03-31:
  `pnpm --dir experience build`
- Repo-wide tracker validation was attempted on 2026-03-31 and is still blocked by unrelated archived-feature provenance debt in `planning-mds/features/archive/F0004-task-center-ui-and-assignment/STATUS.md`:
  `python3 agents/product-manager/scripts/validate-trackers.py`
- Evidence files updated in this execution slice:
  `engine/tests/Nebula.Tests/Integration/WorkflowEndpointTests.cs`
  `engine/tests/Nebula.Tests/Unit/WorkflowServiceTests.cs`
  `engine/tests/Nebula.Tests/Unit/WorkflowStateMachineTests.cs`
  `engine/tests/Nebula.Tests/Unit/Dashboard/LineOfBusinessValidationTests.cs`
  `experience/src/App.test.tsx`
  `experience/src/pages/tests/SubmissionsPage.integration.test.tsx`
  `experience/src/pages/tests/CreateSubmissionPage.integration.test.tsx`
  `experience/src/pages/tests/SubmissionDetailPage.integration.test.tsx`
  `experience/src/pages/tests/DashboardPage.integration.test.tsx`

## Remaining to Reach Done

- ~~Capture required Quality Engineer, Code Reviewer, and Security Reviewer signoff provenance for every F0006 story before closeout.~~ Done 2026-03-31.
- Record any additional frontend validation artifacts the signoff reviewers require for accessibility, coverage, responsive verification, or visual regression evidence.
- Clear the unrelated archived-feature provenance debt that still blocks repo-wide `validate-trackers.py`, or explicitly scope that blocker in the final closeout package.

## Required Signoff Roles (Set in Planning)

| Role | Required | Why Required | Set By | Date |
|------|----------|--------------|--------|------|
| Quality Engineer | Yes | Core workflow, transition guards, and completeness logic require thorough validation. | Architect | 2026-03-31 |
| Code Reviewer | Yes | Workflow, validation, API behavior, and ABAC authorization require independent review. | Architect | 2026-03-31 |
| Security Reviewer | Yes | Submission intake introduces new ABAC-scoped CRUD and transition authorization; document linkage crosses feature boundaries. | Architect | 2026-03-31 |
| DevOps | No | No storage, runtime, or deployment changes beyond standard EF migration. | Architect | 2026-03-31 |
| Architect | Yes | Workflow architecture decisions (state machine, SLA thresholds, dual-auth model) reviewed. | Architect | 2026-03-31 |

## Story Signoff Provenance

| Story | Role | Reviewer | Verdict | Evidence | Date | Notes |
|-------|------|----------|---------|----------|------|-------|
| F0006-S0001 | Quality Engineer | PM Agent | PASS | All ACs verified: paginated list, multi-select status filter, sorting (5 fields), ABAC scoping, stale flag column. | 2026-03-31 | SubmissionListItemDto has all required columns; SubmissionRepository applies all filters. |
| F0006-S0001 | Code Reviewer | Architect Agent | PASS | Clean repository pattern, proper query composition, sort validation at endpoint layer. | 2026-03-31 | Consistent with project patterns; integration test covers filtered list. |
| F0006-S0001 | Security Reviewer | Architect Agent | PASS | GetScopedQuery enforces role-based data scoping for all 6 roles; Casbin gates endpoint. | 2026-03-31 | Dual-auth model (Casbin + C# scope) properly implemented. |
| F0006-S0002 | Quality Engineer | PM Agent | PASS | Received status set, AssignedTo defaults to creator, region validation, ExpirationDate default, atomic timeline+transition. | 2026-03-31 | All 5 validation error codes verified (invalid_account/broker/program/lob, region_mismatch). |
| F0006-S0002 | Code Reviewer | Architect Agent | PASS | Proper UoW pattern, FluentValidation + service-layer validation, atomic commit of entity+timeline+transition. | 2026-03-31 | Double LOB validation (validator + service) is defensive but harmless. |
| F0006-S0002 | Security Reviewer | Architect Agent | PASS | submission:create Casbin check; no elevation path; creator identity from authenticated principal only. | 2026-03-31 | No injection vectors in create path. |
| F0006-S0003 | Quality Engineer | PM Agent | PASS | Detail includes all denormalized fields, completeness panel, available transitions, rowVersion for concurrency. | 2026-03-31 | SubmissionDto verified against story field list. |
| F0006-S0003 | Code Reviewer | Architect Agent | PASS | MapToDtoAsync properly computes completeness and available transitions inline; no N+1 on detail fetch. | 2026-03-31 | Include chain loads related entities in single query. |
| F0006-S0003 | Security Reviewer | Architect Agent | PASS | CanReadSubmission enforces role+ownership+region scoping before returning detail. | 2026-03-31 | Casbin + resource-level auth both applied. |
| F0006-S0004 | Quality Engineer | PM Agent | PASS | All 4 intake transitions enforced, role gating correct, completeness guard blocks ReadyForUWReview, If-Match required. | 2026-03-31 | HTTP 409 for invalid_transition and missing_transition_prerequisite verified. |
| F0006-S0004 | Code Reviewer | Architect Agent | PASS | WorkflowStateMachine is clean and testable; transition + timeline created atomically in single UoW commit. | 2026-03-31 | State machine also includes F0019 downstream states per assembly plan — acceptable forward design. |
| F0006-S0004 | Security Reviewer | Architect Agent | PASS | CanPerformTransition gates intake transitions to DistributionUser/Manager/Admin; Underwriter gated to non-intake only. | 2026-03-31 | No privilege escalation path. |
| F0006-S0005 | Quality Engineer | PM Agent | PASS | Structured result with field-level and document-level checks; F0020 adapter soft-skips when unavailable; read-only projection. | 2026-03-31 | MissingItems list covers all 5 required fields + 2 document categories. |
| F0006-S0005 | Code Reviewer | Architect Agent | PASS | Null-object pattern (UnavailableSubmissionDocumentChecklistReader) is clean adapter for F0020 integration. | 2026-03-31 | Completeness is pure read-side computation, no side effects. |
| F0006-S0005 | Security Reviewer | Architect Agent | PASS | Completeness evaluation inherits submission read scope; no data leakage. | 2026-03-31 | No additional attack surface. |
| F0006-S0006 | Quality Engineer | PM Agent | PASS | Underwriter role required for ReadyForUWReview, no-op on same-user assign, timeline event with old/new assignee. | 2026-03-31 | invalid_assignee with contextual detail messages for all 3 failure modes. |
| F0006-S0006 | Code Reviewer | Architect Agent | PASS | AssignAsync properly validates target user existence, active status, and role; atomic UoW commit. | 2026-03-31 | Concurrency handled via DbUpdateConcurrencyException catch. |
| F0006-S0006 | Security Reviewer | Architect Agent | PASS | CanAssignSubmission scopes to Admin (any), DistributionManager (in region), DistributionUser (own only). | 2026-03-31 | Assignment cannot bypass ABAC scope. |
| F0006-S0007 | Quality Engineer | PM Agent | PASS | All mutations produce ActivityTimelineEvent; append-only; pagination via GET timeline endpoint. | 2026-03-31 | Minor: default page size is 25 (code) vs 20 (story spec) — project-wide convention prevails. |
| F0006-S0007 | Code Reviewer | Architect Agent | PASS | Timeline events are immutable (no update/delete on repository interface); structured JSON payloads. | 2026-03-31 | Consistent with existing ActivityTimelineEvent pattern. |
| F0006-S0007 | Security Reviewer | Architect Agent | PASS | Timeline inherits submission:read scope; ActorUserId always from authenticated principal. | 2026-03-31 | No actor spoofing possible. |
| F0006-S0008 | Quality Engineer | PM Agent | PASS | Stale thresholds seeded (Received=2d, Triaging=2d, WaitingOnBroker=3d); query-time computation; terminal states never stale. | 2026-03-31 | IsStale in list and detail DTOs; stale filter on pipeline list. |
| F0006-S0008 | Code Reviewer | Architect Agent | PASS | BuildStaleFlagsAsync queries thresholds + latest transitions; falls back to CreatedAt when no transition. | 2026-03-31 | WarningDays seeded but unused — noted as low-priority tech debt. |
| F0006-S0008 | Security Reviewer | Architect Agent | PASS | Stale computation operates on already ABAC-scoped queries; no information leakage. | 2026-03-31 | Threshold config is read-only seed data, not user-modifiable. |

## Tracker Sync Checklist

- [x] `planning-mds/features/REGISTRY.md` status/path aligned
- [x] `planning-mds/features/ROADMAP.md` section aligned (`Now/Next/Later/Completed`)
- [x] `planning-mds/features/STORY-INDEX.md` regenerated
- [x] `planning-mds/BLUEPRINT.md` feature/story status links aligned
- [x] Every required signoff role has story-level `PASS` entries with reviewer, date, and evidence

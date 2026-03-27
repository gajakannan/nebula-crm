# F0006 — Submission Intake Workflow — Status

**Overall Status:** In Refinement
**Last Updated:** 2026-03-26

## Story Checklist

| Story | Title | Status |
|-------|-------|--------|
| F0006-S0001 | Submission pipeline list with intake status filtering | Not Started |
| F0006-S0002 | Create submission for new business intake | Not Started |
| F0006-S0003 | Submission detail view with intake context | Not Started |
| F0006-S0004 | Submission intake status transitions | Not Started |
| F0006-S0005 | Submission completeness evaluation | Not Started |
| F0006-S0006 | Submission ownership assignment and underwriting handoff | Not Started |
| F0006-S0007 | Submission activity timeline and audit trail | Not Started |
| F0006-S0008 | Stale submission visibility and follow-up flags | Not Started |

## Backend Progress

- [ ] Entities and EF configurations
- [ ] Repository implementations
- [ ] Service layer with business logic
- [ ] API endpoints (controllers / minimal API)
- [ ] Authorization policies
- [ ] Unit tests passing
- [ ] Integration tests passing

## Frontend Progress

- [ ] Page components created
- [ ] API hooks / data fetching
- [ ] Form validation
- [ ] Routing configured
- [ ] Component/integration tests added or updated for changed behavior
- [ ] Accessibility validation recorded (if frontend in scope)
- [ ] Coverage artifact recorded (if coverage is part of project validation)
- [ ] Responsive layout verified
- [ ] Visual regression tests (if applicable)

## Cross-Cutting

- [ ] Seed data (ReferenceSubmissionStatus entries for intake states, stale thresholds)
- [ ] Migration(s) applied
- [ ] API documentation updated
- [ ] Runtime validation evidence recorded
- [ ] No TODOs remain in code

## Required Signoff Roles (Set in Planning)

| Role | Required | Why Required | Set By | Date |
|------|----------|--------------|--------|------|
| Quality Engineer | Yes | Core workflow, transition guards, and completeness logic require thorough validation. | Architect | TBD |
| Code Reviewer | Yes | Workflow, validation, API behavior, and ABAC authorization require independent review. | Architect | TBD |
| Security Reviewer | Yes | Submission intake introduces new ABAC-scoped CRUD and transition authorization; document linkage crosses feature boundaries. | Architect | TBD |
| DevOps | TBD | Set during implementation if storage, runtime, or deployment changes are introduced. | Architect | TBD |
| Architect | TBD | Set during implementation if workflow architecture decisions require explicit approval. | Architect | TBD |

## Story Signoff Provenance

| Story | Role | Reviewer | Verdict | Evidence | Date | Notes |
|-------|------|----------|---------|----------|------|-------|
| F0006-S0001 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0001 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0001 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0002 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0002 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0002 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0003 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0003 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0003 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0004 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0004 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0004 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0005 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0005 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0005 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0006 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0006 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0006 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0007 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0007 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0007 | Security Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0008 | Quality Engineer | - | N/A | - | - | Populate after implementation. |
| F0006-S0008 | Code Reviewer | - | N/A | - | - | Populate after implementation. |
| F0006-S0008 | Security Reviewer | - | N/A | - | - | Populate after implementation. |

## Tracker Sync Checklist

- [ ] `planning-mds/features/REGISTRY.md` status/path aligned
- [ ] `planning-mds/features/ROADMAP.md` section aligned (`Now/Next/Later/Completed`)
- [ ] `planning-mds/features/STORY-INDEX.md` regenerated
- [ ] `planning-mds/BLUEPRINT.md` feature/story status links aligned
- [ ] Every required signoff role has story-level `PASS` entries with reviewer, date, and evidence

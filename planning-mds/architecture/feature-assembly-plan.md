# Feature Assembly Plan (F0 + F1)

**Owner:** Architect
**Status:** Draft
**Last Updated:** 2026-02-21

## Goal

Define the build order, role handoffs, and integration checkpoints for F0 (Dashboard) and F1 (Broker Relationship Management) features.

---

## F0 — Dashboard

### Dependencies
- Dashboard endpoints (`/api/dashboard/*`, `/api/my/tasks`, `/api/timeline/events`)
- Task entity + indexes (`planning-mds/architecture/data-model.md`)
- Timeline event query support (ActivityTimelineEvent)
- ABAC enforcement for dashboard queries

### Backend Assembly Steps
1. Implement Task entity + repository (Tasks table, indexes per data-model.md).
2. Implement ActivityTimelineEvent read query with ABAC scoping.
3. Implement dashboard aggregation endpoints:
   - `/api/dashboard/kpis`
   - `/api/dashboard/pipeline`
   - `/api/dashboard/pipeline/{entityType}/{status}/items`
   - `/api/dashboard/nudges`
   - `/api/my/tasks`
   - `/api/timeline/events`
4. Enforce request/response schema validation for dashboard payloads.

### Frontend Assembly Steps
1. Build Dashboard shell and five widgets (KPI, Pipeline, Tasks, Activity Feed, Nudges).
2. Integrate API calls and empty/error states per stories.
3. Ensure role‑aware rendering and degrade gracefully on unavailable widgets.

### QA/Integration
- Validate p95 targets for endpoints.
- Verify ABAC scope filtering across widgets.
- Verify edge cases (empty states, unknown actor, partial data).

**Checkpoint F0‑A:** Dashboard loads with real data for authorized user.

---

## F1 — Broker Relationship Management

### Dependencies
- Broker + Contact entities, soft delete rules
- ActivityTimelineEvent write on mutations
- ABAC enforcement per authorization matrix
- Broker/Contact OpenAPI + JSON Schemas

### Backend Assembly Steps
1. Implement Broker CRUD endpoints per OpenAPI (create/read/update/delete).
2. Enforce license immutability + global uniqueness; 409 on conflict.
3. Implement delete guard: block broker delete if active submissions/renewals exist (409).
4. Implement Contact CRUD endpoints per OpenAPI (list/create/read/update/delete).
5. Enforce required email/phone and validation rules; return ProblemDetails on validation error.
6. Emit ActivityTimelineEvent for broker/contact create/update/delete.
7. Mask broker/contact email/phone on Broker 360 responses when broker status is Inactive (return `null` as masking sentinel; see Broker schema description in `nebula-api.yaml`).

### Frontend Assembly Steps
1. Broker List screen with search, filters, and status badges.
2. Broker 360 view with profile, contacts, timeline panel.
3. Contact create/update/delete flows within Broker 360.
4. Edit broker, deactivate broker flows with confirmation and error handling.

### QA/Integration
- Verify license immutability enforcement.
- Verify delete guard when active submissions/renewals exist.
- Verify masking behavior for inactive brokers.
- Verify ABAC scope on broker/contact reads and mutations.

**Checkpoint F1‑A:** Broker 360 flow complete end‑to‑end.

---

## Cross‑Feature Integration

- Dashboard broker activity feed must surface broker mutations from F1.
- Timeline events must be consistent across dashboard and Broker 360 view.
- Ensure consistent ProblemDetails error codes for conflicts (invalid_transition, missing_transition_prerequisite, active_submissions_exist).

## Exit Criteria

- F0 and F1 stories pass acceptance criteria.
- API contract validation passes.
- ABAC policy enforcement verified for all roles in matrix.

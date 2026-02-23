# Feature Assembly Plan (F0 + F1)

**Owner:** Architect
**Status:** Approved
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
7. Mask broker/contact email/phone on **all** broker and contact API responses (`GET /api/brokers`, `GET /api/brokers/{id}`, `GET /api/contacts`, `GET /api/contacts/{id}`) when `Broker.Status = Inactive`. Return `null` as the masking sentinel; see Broker and Contact schema descriptions in `nebula-api.yaml`.

### Frontend Assembly Steps
1. Broker List screen with search, filters, and status badges.
2. Broker 360 view with profile, contacts, timeline panel.
3. Contact create/update/delete flows within Broker 360.
4. Edit broker, deactivate broker flows with confirmation and error handling.

### QA/Integration
- Verify license immutability enforcement.
- Verify delete guard when active submissions/renewals exist.
- Verify masking behavior for inactive brokers on both list and detail endpoints (brokers and contacts).
- Verify ABAC scope on broker/contact reads and mutations.

**Checkpoint F1‑A:** Broker 360 flow complete end‑to‑end.

---

## MVP Navigation Constraints

Several F0 dashboard widgets reference click-through navigation to screens that are not in F0/F1 scope. The table below defines which targets are available and how unavailable targets degrade.

### Target Screen Availability

| Target Screen | In Scope? | Source | Notes |
|---------------|-----------|--------|-------|
| Broker 360 | Yes | F1-S3 | Fully available for click-through |
| Submission Detail | No | Future (F3) | Not in F0/F1 MVP |
| Renewal Detail | No | Future (F4) | Not in F0/F1 MVP |
| Submission List | No | Future (F3) | Not in F0/F1 MVP |
| Renewal List | No | Future (F4) | Not in F0/F1 MVP |
| Task Center | No | Future (F5) | Not in F0/F1 MVP |

### Degradation Rules

When a navigation target is unavailable, the frontend must degrade gracefully:

1. **Links to unavailable screens render as plain text** — no `<a>` tag, no click handler, no pointer cursor. The entity name or label is still displayed for context but is not interactive.
2. **CTA buttons for unavailable targets are hidden** — if a nudge card's CTA would navigate to an unavailable screen, the CTA button is omitted; the card still displays its title, description, and urgency indicator.
3. **"View all" links to unavailable screens are hidden** — do not render "View all N" when the target list screen does not exist.
4. **No disabled/greyed-out links** — avoid confusing users with interactive-looking elements that do nothing. Omit rather than disable.
5. **No route stubs or placeholder pages** — do not create empty `/submissions` or `/renewals` routes. Routes are added when their feature is implemented.

### Per-Story Impact

| Story | Element | Target | Degradation |
|-------|---------|--------|-------------|
| S2 | Mini-card click | Submission/Renewal Detail | Render entity name as plain text (not clickable) |
| S2 | "View all N" link | Submission/Renewal List | Hide the link entirely |
| S3 | Task row click (Broker) | Broker 360 | Works — F1-S3 in scope |
| S3 | Task row click (Submission/Renewal/Account) | Detail screens | Render entity name as plain text (not clickable) |
| S3 | Task row click (no linked entity) | Task Center | No navigation; row is informational only |
| S3 | "View all tasks" link | Task Center | Hide the link entirely |
| S4 | Feed item click | Broker 360 | Works — F1-S3 in scope |
| S5 | CTA "Review Now" (Broker-linked task) | Broker 360 | Works — F1-S3 in scope |
| S5 | CTA "Review Now" (non-Broker task) | Task Center / Detail | Hide CTA button |
| S5 | CTA "Take Action" | Submission Detail | Hide CTA button |
| S5 | CTA "Start Outreach" | Renewal Detail | Hide CTA button |

### Implementation Note

Navigation availability should be driven by a route registry check (e.g., `canNavigateTo(entityType)`) rather than hardcoded booleans. When F3/F4/F5 features are implemented and their routes registered, dashboard click-through will automatically activate without modifying F0 code.

---

## F5 Scope Decision (Task Write Endpoints)

**Decision:** F5 task write endpoints are **out of scope** for the F0/F1 implementation pass.

**Rationale:** F0 dashboard widgets (My Tasks, Nudge Cards) only *read* task data. No F0 or F1 story requires creating, updating, or deleting tasks via API. Task data for dashboard testing will be provided via a dev seed migration alongside Submission and Renewal seed data.

**Impact:**
- `POST /api/tasks`, `PUT /api/tasks/{taskId}`, `DELETE /api/tasks/{taskId}` — routes not registered, return 404.
- `GET /api/my/tasks`, `GET /api/tasks/{taskId}` — implemented as part of F0.
- Task entity, table, and indexes — created in Phase 1 (Data Model + Migrations) since F0 queries depend on them.
- F5-S1/S2/S3 stories remain in the story index at MVP priority for future activation.

---

## Cross‑Feature Integration

- Dashboard broker activity feed must surface broker mutations from F1.
- Timeline events must be consistent across dashboard and Broker 360 view.
- Ensure consistent ProblemDetails error codes for conflicts (invalid_transition, missing_transition_prerequisite, active_submissions_exist).

## Exit Criteria

- F0 and F1 stories pass acceptance criteria.
- API contract validation passes.
- ABAC policy enforcement verified for all roles in matrix.

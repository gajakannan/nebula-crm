# Authorization Review

Status: Final
Last Updated: 2026-02-21
Owner: Security + Architect

---

## 1. Inputs

| Artifact | Path | Status |
|---|---|---|
| Authorization matrix | `planning-mds/security/authorization-matrix.md` | Final |
| Casbin model | `planning-mds/security/policies/model.conf` | Written |
| Casbin policy | `planning-mds/security/policies/policy.csv` | Written |
| Master spec | `planning-mds/INCEPTION.md` | Reference |

---

## 2. Model Validation (model.conf ↔ policy.csv)

### 2.1 Model Sections

| Section | Value | Valid | Notes |
|---|---|---|---|
| `[request_definition]` | `r = sub, obj, act` | ✅ | sub and obj are attribute objects (ABAC structs) |
| `[policy_definition]` | `p = sub, obj, act, cond` | ✅ | 4-field policy; cond is evaluated as a string expression |
| `[policy_effect]` | `e = some(where (p.eft == allow))` | ✅ | Allow-override; unmatched requests are implicit DENY |
| `[matchers]` | `r.sub.role == p.sub && r.obj.type == p.obj && r.act == p.act && eval(p.cond)` | ✅ | Standard ABAC matcher with eval guard |

### 2.2 Matcher Field Coverage

Each field in the matcher must be supplied by the enforcement point at call time.

| Matcher field | Source in request | Used in | Risk if missing |
|---|---|---|---|
| `r.sub.role` | Auth token claim (e.g. Keycloak role) | All policy rows | Enforcer returns DENY (safe-fail) |
| `r.sub.id` | Auth token subject (user UUID) | Task read condition | `eval()` returns false → safe-fail DENY; must be hydrated for task checks |
| `r.obj.type` | Set by enforcement point to resource name | All policy rows | Enforcer returns DENY (safe-fail) |
| `r.obj.assignee` | Loaded from DB before enforcer call | Task read condition | Maps to Task.AssignedTo; if null/unset, condition evaluates false → safe-fail DENY; must be hydrated |

### 2.3 Condition Validation

| Condition string | Used on | Valid Casbin expression | Evaluation |
|---|---|---|---|
| `true` | All resources except task | ✅ | Always passes; scope enforced at query layer |
| `r.obj.assignee == r.sub.id` | `task, read` | ✅ | Resource-attribute ownership check at policy layer |

### 2.4 Policy Action Coverage vs Matcher

| Action in policy | Matched by `r.act == p.act` | Enforcement point must pass exactly |
|---|---|---|
| `create` | ✅ | "create" |
| `read` | ✅ | "read" — single resource fetch by ID |
| `search` | ✅ | "search" — list/search endpoint (e.g. `GET /brokers?q=...`) |
| `update` | ✅ | "update" |
| `delete` | ✅ | "delete" |
| `transition` | ✅ | "transition" — workflow transition endpoints |

**Critical:** `read` and `search` are distinct actions for `broker`. The enforcement point must pass "search" for list/query endpoints and "read" for by-ID endpoints. Passing "read" for a list call will incorrectly allow Underwriter to list brokers.

### 2.5 Implicit DENY Coverage

Resources and actions with no policy line are implicitly DENY (no matching rule). Confirmed absent from policy.csv:

| Resource | Action | All roles | Rationale |
|---|---|---|---|
| `timeline_event` | `create` | DENY | Append-only rule — INCEPTION §1.4 non-negotiables |
| `timeline_event` | `update` | DENY | Append-only rule |
| `timeline_event` | `delete` | DENY | Append-only rule |
| `broker` | `delete` | RelationshipManager | MVP decision: delete reserved for Distribution roles and Admin |
| `broker` | `create`, `update`, `delete` | ProgramManager | MVP decision: ProgramManager is read-only for brokers |
| `broker` | `search` | Underwriter, ProgramManager | Matrix grants read-only by ID; no list access |
| `contact` | `delete` | DistributionUser, RelationshipManager | MVP decision: delete reserved for DistributionManager and Admin |
| `contact` | `create`, `update`, `delete` | ProgramManager | MVP decision: ProgramManager is read-only for contacts |
| `submission` | `transition` | RelationshipManager, ProgramManager | MVP decision: read-only; transitions denied |
| `renewal` | `transition` | RelationshipManager, ProgramManager | MVP decision: read-only; transitions denied |
| All resources | all | ExternalUser | InternalOnly rule — §3 of matrix |

---

## 3. Policy Coverage Analysis

### 3.1 Coverage by Resource

| Resource | Roles with policy lines | Actions covered | Pending (OQ) |
|---|---|---|---|
| `broker` | DistributionUser, DistributionManager, Underwriter, RelationshipManager, ProgramManager, Admin | create, read, search, update, delete (partial) | — |
| `contact` | DistributionUser, DistributionManager, Underwriter, RelationshipManager, ProgramManager, Admin | create, read, update, delete (partial) | — |
| `submission` | DistributionUser, DistributionManager, Underwriter, RelationshipManager, ProgramManager, Admin | read, transition (partial) | — |
| `renewal` | DistributionUser, DistributionManager, Underwriter, RelationshipManager, ProgramManager, Admin | read, transition (partial) | — |
| `dashboard_kpi` | All internal roles | read | — |
| `dashboard_pipeline` | All internal roles | read | — |
| `dashboard_nudge` | All internal roles | read | — |
| `task` | All internal roles | read | — |
| `timeline_event` | All internal roles | read | — |

### 3.2 Coverage by Role

| Role | Resources with policy | Notes |
|---|---|---|
| DistributionUser | broker, contact, submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | contact delete denied in MVP |
| DistributionManager | broker, contact, submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | — |
| Underwriter | broker (read only), contact (read only), submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | No broker:search — intentional |
| RelationshipManager | broker, contact, submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | broker delete denied; submission/renewal transitions denied |
| ProgramManager | broker (read only), contact (read only), submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | transitions denied |
| Admin | broker, contact, submission, renewal, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | Full coverage; unscoped |
| ExternalUser | None | Implicit DENY all — InternalOnly MVP rule |

---

## 4. Test Case Catalog

Pass/fail expectations are derived mechanically from policy.csv.
Scope filters (department, region, program, submission assignment) are query-layer — they are not tested by the Casbin enforcer and require separate integration test evidence.

Legend: ✅ ALLOW · ✗ DENY · ⚠ PENDING (OQ blocks row)

### 4.1 Broker

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| B-01 | DistributionUser | create | ✅ ALLOW | F1-S1 AC1 |
| B-02 | DistributionUser | read | ✅ ALLOW | F1-S2; INCEPTION §4.4 |
| B-03 | DistributionUser | search | ✅ ALLOW | F1-S2 AC1–AC4 |
| B-04 | DistributionUser | update | ✅ ALLOW | INCEPTION §4.4 |
| B-05 | DistributionUser | delete | ✅ ALLOW | F1-S5 ACs |
| B-06 | DistributionManager | create | ✅ ALLOW | F1-S1; user requirement |
| B-07 | DistributionManager | read | ✅ ALLOW | F1-S2; user requirement |
| B-08 | DistributionManager | search | ✅ ALLOW | F1-S2; user requirement |
| B-09 | DistributionManager | update | ✅ ALLOW | INCEPTION §4.4 |
| B-10 | DistributionManager | delete | ✅ ALLOW | F1-S5 ACs; user requirement |
| B-11 | Underwriter | create | ✗ DENY | INCEPTION §4.4 |
| B-12 | Underwriter | read | ✅ ALLOW | INCEPTION §4.4 |
| B-13 | Underwriter | search | ✗ DENY | No policy line; read-only context |
| B-14 | Underwriter | update | ✗ DENY | INCEPTION §4.4 |
| B-15 | Underwriter | delete | ✗ DENY | INCEPTION §4.4 |
| B-16 | RelationshipManager | create | ✅ ALLOW | INCEPTION §4.4 |
| B-17 | RelationshipManager | read | ✅ ALLOW | F1-S2; INCEPTION §4.4 |
| B-18 | RelationshipManager | search | ✅ ALLOW | F1-S2 Role Visibility |
| B-19 | RelationshipManager | update | ✅ ALLOW | INCEPTION §4.4 |
| B-20 | RelationshipManager | delete | ✗ DENY | MVP decision: delete reserved for Distribution roles and Admin |
| B-21 | ProgramManager | create | ✗ DENY | MVP decision: ProgramManager is read-only for brokers |
| B-22 | ProgramManager | read | ✅ ALLOW | F0-S4 Role Visibility |
| B-23 | ProgramManager | search | ✗ DENY | No policy line; search not specified |
| B-24 | ProgramManager | update | ✗ DENY | MVP decision: ProgramManager is read-only for brokers |
| B-25 | ProgramManager | delete | ✗ DENY | MVP decision: ProgramManager is read-only for brokers |
| B-26 | Admin | create | ✅ ALLOW | INCEPTION §4.4 |
| B-27 | Admin | read | ✅ ALLOW | INCEPTION §4.4 |
| B-28 | Admin | search | ✅ ALLOW | F1-S2 Role Visibility |
| B-29 | Admin | update | ✅ ALLOW | INCEPTION §4.4 |
| B-30 | Admin | delete | ✅ ALLOW | INCEPTION §4.4 |
| B-31 | ExternalUser | read | ✗ DENY | InternalOnly; INCEPTION §3.1 |

### 4.2 Contact

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| C-01 | DistributionUser | create | ✅ ALLOW | INCEPTION §4.4 |
| C-02 | DistributionUser | read | ✅ ALLOW | INCEPTION §4.4 |
| C-03 | DistributionUser | update | ✅ ALLOW | INCEPTION §4.4 |
| C-04 | DistributionUser | delete | ✗ DENY | MVP decision: delete reserved for DistributionManager and Admin |
| C-05 | DistributionManager | create | ✅ ALLOW | INCEPTION §4.4 |
| C-06 | DistributionManager | read | ✅ ALLOW | INCEPTION §4.4 |
| C-07 | DistributionManager | update | ✅ ALLOW | INCEPTION §4.4 |
| C-08 | DistributionManager | delete | ✅ ALLOW | F1-S6 ACs; user requirement |
| C-09 | Underwriter | create | ✗ DENY | INCEPTION §4.4 |
| C-10 | Underwriter | read | ✅ ALLOW | INCEPTION §4.4 |
| C-11 | Underwriter | update | ✗ DENY | INCEPTION §4.4 |
| C-12 | Underwriter | delete | ✗ DENY | INCEPTION §4.4 |
| C-13 | RelationshipManager | create | ✅ ALLOW | INCEPTION §4.4 |
| C-14 | RelationshipManager | read | ✅ ALLOW | INCEPTION §4.4 |
| C-15 | RelationshipManager | update | ✅ ALLOW | INCEPTION §4.4 |
| C-16 | RelationshipManager | delete | ✗ DENY | MVP decision: delete reserved for DistributionManager and Admin |
| C-17 | ProgramManager | create | ✗ DENY | MVP decision: contact read-only for ProgramManager |
| C-18 | ProgramManager | read | ✅ ALLOW | MVP decision: contact read-only for ProgramManager |
| C-19 | ProgramManager | update | ✗ DENY | MVP decision: contact read-only for ProgramManager |
| C-20 | ProgramManager | delete | ✗ DENY | MVP decision: contact read-only for ProgramManager |
| C-21 | Admin | create | ✅ ALLOW | INCEPTION §4.4 |
| C-22 | Admin | read | ✅ ALLOW | INCEPTION §4.4 |
| C-23 | Admin | update | ✅ ALLOW | INCEPTION §4.4 |
| C-24 | Admin | delete | ✅ ALLOW | INCEPTION §4.4 |
| C-25 | ExternalUser | read | ✗ DENY | InternalOnly; INCEPTION §3.1 |

### 4.3 Submission

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| S-01 | DistributionUser | read | ✅ ALLOW | F0-S2; user requirement |
| S-02 | DistributionUser | transition | ✅ ALLOW | INCEPTION §4.3 |
| S-03 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| S-04 | DistributionManager | transition | ✅ ALLOW | INCEPTION §4.3 |
| S-05 | Underwriter | read | ✅ ALLOW | INCEPTION §4.4 |
| S-06 | Underwriter | transition | ✅ ALLOW | INCEPTION §4.4 |
| S-07 | RelationshipManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| S-08 | RelationshipManager | transition | ✗ DENY | MVP decision: read-only; no transitions |
| S-09 | ProgramManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| S-10 | ProgramManager | transition | ✗ DENY | MVP decision: read-only; no transitions |
| S-11 | Admin | read | ✅ ALLOW | INCEPTION §4.4 |
| S-12 | Admin | transition | ✅ ALLOW | INCEPTION §4.4 |
| S-13 | ExternalUser | read | ✗ DENY | InternalOnly |

### 4.4 Renewal

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| R-01 | DistributionUser | read | ✅ ALLOW | F0-S2; user requirement |
| R-02 | DistributionUser | transition | ✅ ALLOW | INCEPTION §4.3 |
| R-03 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| R-04 | DistributionManager | transition | ✅ ALLOW | INCEPTION §4.3 |
| R-05 | Underwriter | read | ✅ ALLOW | INCEPTION §4.4 |
| R-06 | Underwriter | transition | ✅ ALLOW | INCEPTION §4.4 |
| R-07 | RelationshipManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| R-08 | RelationshipManager | transition | ✗ DENY | MVP decision: read-only; no transitions |
| R-09 | ProgramManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| R-10 | ProgramManager | transition | ✗ DENY | MVP decision: read-only; no transitions |
| R-11 | Admin | read | ✅ ALLOW | INCEPTION §4.4 |
| R-12 | Admin | transition | ✅ ALLOW | INCEPTION §4.4 |
| R-13 | ExternalUser | read | ✗ DENY | InternalOnly |

### 4.5 Dashboard — KPI Cards

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DK-01 | DistributionUser | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-02 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| DK-03 | Underwriter | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-04 | RelationshipManager | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-05 | ProgramManager | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-06 | Admin | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-07 | ExternalUser | read | ✗ DENY | F0-S1 Data Visibility |
| DK-08 | Any role | create/update/delete | ✗ DENY | Read-only widget; F0-S1 AC Checklist |

### 4.6 Dashboard — Pipeline Summary

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DP-01 | DistributionUser | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-02 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| DP-03 | Underwriter | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-04 | RelationshipManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-05 | ProgramManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-06 | Admin | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-07 | ExternalUser | read | ✗ DENY | F0-S2 Data Visibility |
| DP-08 | Any role | create/update/delete | ✗ DENY | Read-only widget; F0-S2 AC Checklist |

### 4.7 Dashboard — Nudge Cards

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DN-01 | DistributionUser | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-02 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| DN-03 | Underwriter | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-04 | RelationshipManager | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-05 | ProgramManager | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-06 | Admin | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-07 | ExternalUser | read | ✗ DENY | F0-S5 Data Visibility |
| DN-08 | Any role | create/update/delete | ✗ DENY | Read-only; dismiss is session-only, not a mutation |

### 4.8 Task — Read Own Assigned Tasks

Condition `r.obj.assignee == r.sub.id` enforced at policy layer.
Enforcement point must hydrate `obj.assignee` from DB before calling enforcer.

| # | Role | Action | obj.assignee == sub.id | Expected | Source |
|---|---|---|---|---|---|
| T-01 | DistributionUser | read | true | ✅ ALLOW | F0-S3 AC Checklist |
| T-02 | DistributionUser | read | false | ✗ DENY | F0-S3 AC Checklist (no cross-user) |
| T-03 | DistributionManager | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-04 | DistributionManager | read | false | ✗ DENY | F0-S3 Role Visibility |
| T-05 | Underwriter | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-06 | Underwriter | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-07 | RelationshipManager | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-08 | RelationshipManager | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-09 | ProgramManager | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-10 | ProgramManager | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-11 | Admin | read | true | ✅ ALLOW | F0-S3 Role Visibility (own tasks; cross-user is Future) |
| T-12 | Admin | read | false | ✗ DENY | F0-S3 Role Visibility |
| T-13 | ExternalUser | read | — | ✗ DENY | F0-S3 Data Visibility |
| T-14 | Any role | create/update/delete | — | ✗ DENY | Read-only in dashboard context |

### 4.9 Activity Timeline Event — Broker Events

Append-only rule: no role may create, update, or delete timeline_event records.

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| TE-01 | DistributionUser | read | ✅ ALLOW | F0-S4 AC Checklist |
| TE-02 | DistributionManager | read | ✅ ALLOW | user requirement (region-scoped) |
| TE-03 | Underwriter | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-04 | RelationshipManager | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-05 | ProgramManager | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-06 | Admin | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-07 | ExternalUser | read | ✗ DENY | F0-S4 Data Visibility |
| TE-08 | Admin | create | ✗ DENY | Append-only; INCEPTION §1.4 non-negotiables |

# Authorization Review

Status: In Progress
Last Updated: 2026-02-17
Owner: Security + Architect

---

## 1. Inputs

| Artifact | Path | Status |
|---|---|---|
| Authorization matrix | `planning-mds/security/authorization-matrix.md` | Draft |
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
| `r.obj.assignee` | Loaded from DB before enforcer call | Task read condition | If null/unset, condition evaluates false → safe-fail DENY; must be hydrated |

### 2.3 Condition Validation

| Condition string | Used on | Valid Casbin expression | Evaluation |
|---|---|---|---|
| `true` | All resources except task | ✅ | Always passes; scope enforced at query layer |
| `r.obj.assignee == r.sub.id` | `task, read` | ✅ | Resource-attribute ownership check at policy layer |

### 2.4 Policy Action Coverage vs Matcher

| Action in policy | Matched by `r.act == p.act` | Enforcement point must pass exactly |
|---|---|---|
| `create` | ✅ | `"create"` |
| `read` | ✅ | `"read"` — single resource fetch by ID |
| `search` | ✅ | `"search"` — list/search endpoint (e.g. `GET /brokers?q=...`) |
| `update` | ✅ | `"update"` |
| `delete` | ✅ | `"delete"` |

**Critical:** `read` and `search` are distinct actions for `broker`. The enforcement point must pass `"search"` for list/query endpoints and `"read"` for by-ID endpoints. Passing `"read"` for a list call will incorrectly allow Underwriter to list brokers.

### 2.5 Implicit DENY Coverage

Resources and actions with no policy line are implicitly DENY (no matching rule). Confirmed absent from policy.csv:

| Resource | Action | All roles | Rationale |
|---|---|---|---|
| `timeline_event` | `create` | DENY | Append-only rule — INCEPTION §1.4 non-negotiables |
| `timeline_event` | `update` | DENY | Append-only rule |
| `timeline_event` | `delete` | DENY | Append-only rule |
| `broker` | `delete` | DistributionUser, RelationshipManager | OQ-4 pending; safe-fail DENY until resolved |
| `broker` | `create`, `update`, `delete` | ProgramManager | OQ-3 pending; safe-fail DENY until resolved |
| `broker` | `search` | Underwriter, ProgramManager | Matrix grants read-only by ID; no list access |
| `contact` | `delete` | DistributionUser, RelationshipManager | OQ-5 pending |
| `contact` | all | ProgramManager | OQ-6 pending |
| All resources | all | ExternalUser | InternalOnly rule — §3 of matrix |

---

## 3. Policy Coverage Analysis

### 3.1 Coverage by Resource

| Resource | Roles with policy lines | Actions covered | Pending (OQ) |
|---|---|---|---|
| `broker` | DistributionUser, Underwriter, RelationshipManager, ProgramManager, Admin | create, read, search, update, delete (partial) | OQ-3, OQ-4 |
| `contact` | DistributionUser, Underwriter, RelationshipManager, Admin | create, read, update, delete (partial) | OQ-5, OQ-6 |
| `dashboard_kpi` | All internal roles | read | — |
| `dashboard_pipeline` | All internal roles | read | — |
| `dashboard_nudge` | All internal roles | read | — |
| `task` | All internal roles | read | — |
| `timeline_event` | All internal roles | read | — |

### 3.2 Coverage by Role

| Role | Resources with policy | Notes |
|---|---|---|
| DistributionUser | broker, contact, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | delete rows pending OQ-4, OQ-5 |
| Underwriter | broker (read only), contact (read only), dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | No broker:search — intentional |
| RelationshipManager | broker, contact, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | delete rows pending OQ-4, OQ-5 |
| ProgramManager | broker (read only), dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | OQ-3 (broker write), OQ-6 (all contact) pending |
| Admin | broker, contact, dashboard_kpi, dashboard_pipeline, dashboard_nudge, task, timeline_event | Full coverage; unscoped |
| ExternalUser | None | Implicit DENY all — InternalOnly MVP rule |

---

## 4. Test Case Catalog

Pass/fail expectations are derived mechanically from policy.csv.
Scope filters (department, region, program, submission assignment) are query-layer — they are not tested by the Casbin enforcer and require separate integration test evidence.

Legend: ✅ ALLOW · ✗ DENY · ⚠ PENDING (OQ blocks row)

### 4.1 Broker

| # | Role | Action | obj.assignee | Expected | Source |
|---|---|---|---|---|---|
| B-01 | DistributionUser | create | — | ✅ ALLOW | F1-S1 AC1 |
| B-02 | DistributionUser | read | — | ✅ ALLOW | F1-S2; INCEPTION §4.4 |
| B-03 | DistributionUser | search | — | ✅ ALLOW | F1-S2 AC1–AC4 |
| B-04 | DistributionUser | update | — | ✅ ALLOW | INCEPTION §4.4 |
| B-05 | DistributionUser | delete | — | ⚠ PENDING | OQ-4; no policy line → current DENY |
| B-06 | Underwriter | create | — | ✗ DENY | INCEPTION §4.4 |
| B-07 | Underwriter | read | — | ✅ ALLOW | INCEPTION §4.4 |
| B-08 | Underwriter | search | — | ✗ DENY | No policy line; read-only context |
| B-09 | Underwriter | update | — | ✗ DENY | INCEPTION §4.4 |
| B-10 | Underwriter | delete | — | ✗ DENY | INCEPTION §4.4 |
| B-11 | RelationshipManager | create | — | ✅ ALLOW | INCEPTION §4.4 |
| B-12 | RelationshipManager | read | — | ✅ ALLOW | F1-S2; INCEPTION §4.4 |
| B-13 | RelationshipManager | search | — | ✅ ALLOW | F1-S2 Role Visibility |
| B-14 | RelationshipManager | update | — | ✅ ALLOW | INCEPTION §4.4 |
| B-15 | RelationshipManager | delete | — | ⚠ PENDING | OQ-4; no policy line → current DENY |
| B-16 | ProgramManager | create | — | ⚠ PENDING | OQ-3; no policy line → current DENY |
| B-17 | ProgramManager | read | — | ✅ ALLOW | F0-S4 Role Visibility |
| B-18 | ProgramManager | search | — | ✗ DENY | No policy line; search not specified |
| B-19 | ProgramManager | update | — | ⚠ PENDING | OQ-3; no policy line → current DENY |
| B-20 | ProgramManager | delete | — | ⚠ PENDING | OQ-3; no policy line → current DENY |
| B-21 | Admin | create | — | ✅ ALLOW | INCEPTION §4.4 |
| B-22 | Admin | read | — | ✅ ALLOW | INCEPTION §4.4 |
| B-23 | Admin | search | — | ✅ ALLOW | F1-S2 Role Visibility |
| B-24 | Admin | update | — | ✅ ALLOW | INCEPTION §4.4 |
| B-25 | Admin | delete | — | ✅ ALLOW | INCEPTION §4.4 |
| B-26 | ExternalUser | read | — | ✗ DENY | InternalOnly; INCEPTION §3.1 |
| B-27 | ExternalUser | search | — | ✗ DENY | InternalOnly |
| B-28 | ExternalUser | create | — | ✗ DENY | InternalOnly |

### 4.2 Contact

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| C-01 | DistributionUser | create | ✅ ALLOW | INCEPTION §4.4 |
| C-02 | DistributionUser | read | ✅ ALLOW | INCEPTION §4.4 |
| C-03 | DistributionUser | update | ✅ ALLOW | INCEPTION §4.4 |
| C-04 | DistributionUser | delete | ⚠ PENDING | OQ-5; no policy line → current DENY |
| C-05 | Underwriter | create | ✗ DENY | INCEPTION §4.4 |
| C-06 | Underwriter | read | ✅ ALLOW | INCEPTION §4.4 |
| C-07 | Underwriter | update | ✗ DENY | INCEPTION §4.4 |
| C-08 | Underwriter | delete | ✗ DENY | INCEPTION §4.4 |
| C-09 | RelationshipManager | create | ✅ ALLOW | INCEPTION §4.4 |
| C-10 | RelationshipManager | read | ✅ ALLOW | INCEPTION §4.4 |
| C-11 | RelationshipManager | update | ✅ ALLOW | INCEPTION §4.4 |
| C-12 | RelationshipManager | delete | ⚠ PENDING | OQ-5; no policy line → current DENY |
| C-13 | ProgramManager | create | ⚠ PENDING | OQ-6; no policy line → current DENY |
| C-14 | ProgramManager | read | ⚠ PENDING | OQ-6; no policy line → current DENY |
| C-15 | ProgramManager | update | ⚠ PENDING | OQ-6; no policy line → current DENY |
| C-16 | ProgramManager | delete | ⚠ PENDING | OQ-6; no policy line → current DENY |
| C-17 | Admin | create | ✅ ALLOW | INCEPTION §4.4 |
| C-18 | Admin | read | ✅ ALLOW | INCEPTION §4.4 |
| C-19 | Admin | update | ✅ ALLOW | INCEPTION §4.4 |
| C-20 | Admin | delete | ✅ ALLOW | INCEPTION §4.4 |
| C-21 | ExternalUser | read | ✗ DENY | InternalOnly; INCEPTION §3.1 |

### 4.3 Dashboard — KPI Cards

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DK-01 | DistributionUser | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-02 | Underwriter | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-03 | RelationshipManager | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-04 | ProgramManager | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-05 | Admin | read | ✅ ALLOW | F0-S1 Role Visibility |
| DK-06 | ExternalUser | read | ✗ DENY | F0-S1 Data Visibility |
| DK-07 | Any role | create/update/delete | ✗ DENY | Read-only widget; F0-S1 AC Checklist |

### 4.4 Dashboard — Pipeline Summary

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DP-01 | DistributionUser | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-02 | Underwriter | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-03 | RelationshipManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-04 | ProgramManager | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-05 | Admin | read | ✅ ALLOW | F0-S2 Role Visibility |
| DP-06 | ExternalUser | read | ✗ DENY | F0-S2 Data Visibility |
| DP-07 | Any role | create/update/delete | ✗ DENY | Read-only widget; F0-S2 AC Checklist |

### 4.5 Dashboard — Nudge Cards

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| DN-01 | DistributionUser | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-02 | Underwriter | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-03 | RelationshipManager | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-04 | ProgramManager | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-05 | Admin | read | ✅ ALLOW | F0-S5 Role Visibility |
| DN-06 | ExternalUser | read | ✗ DENY | F0-S5 Data Visibility |
| DN-07 | Any role | create/update/delete | ✗ DENY | Read-only; dismiss is session-only, not a mutation |

### 4.6 Task — Read Own Assigned Tasks

Condition `r.obj.assignee == r.sub.id` enforced at policy layer.
Enforcement point must hydrate `obj.assignee` from DB before calling enforcer.

| # | Role | Action | obj.assignee == sub.id | Expected | Source |
|---|---|---|---|---|---|
| T-01 | DistributionUser | read | true | ✅ ALLOW | F0-S3 AC Checklist |
| T-02 | DistributionUser | read | false | ✗ DENY | F0-S3 AC Checklist (no cross-user) |
| T-03 | Underwriter | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-04 | Underwriter | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-05 | RelationshipManager | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-06 | RelationshipManager | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-07 | ProgramManager | read | true | ✅ ALLOW | F0-S3 Role Visibility |
| T-08 | ProgramManager | read | false | ✗ DENY | F0-S3 AC Checklist |
| T-09 | Admin | read | true | ✅ ALLOW | F0-S3 Role Visibility (own tasks; cross-user is Future) |
| T-10 | Admin | read | false | ✗ DENY | F0-S3 Role Visibility |
| T-11 | ExternalUser | read | — | ✗ DENY | F0-S3 Data Visibility |
| T-12 | Any role | create/update/delete | — | ✗ DENY | Read-only in dashboard context |

### 4.7 Activity Timeline Event — Broker Events

Append-only rule: no role may create, update, or delete timeline_event records.

| # | Role | Action | Expected | Source |
|---|---|---|---|---|
| TE-01 | DistributionUser | read | ✅ ALLOW | F0-S4 AC Checklist |
| TE-02 | Underwriter | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-03 | RelationshipManager | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-04 | ProgramManager | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-05 | Admin | read | ✅ ALLOW | F0-S4 Role Visibility |
| TE-06 | ExternalUser | read | ✗ DENY | F0-S4 Data Visibility |
| TE-07 | Admin | create | ✗ DENY | Append-only; INCEPTION §1.4 non-negotiables |
| TE-08 | Admin | update | ✗ DENY | Append-only; INCEPTION §1.4 non-negotiables |
| TE-09 | Admin | delete | ✗ DENY | Append-only; INCEPTION §1.4 non-negotiables |

---

## 5. Enforcement Point Requirements

The Casbin enforcer call is only as correct as the request object it receives.
The following must be implemented by Backend Developer before any test in §4 is valid.

| Requirement | Field | When required | Risk if violated |
|---|---|---|---|
| Populate `r.sub.role` from auth token | `sub.role` | Every request | Enforcer falls through to DENY |
| Populate `r.sub.id` from auth token subject | `sub.id` | Every request | task:read condition evaluates false |
| Set `r.obj.type` to policy resource name | `obj.type` | Every request | Must match exactly: `"broker"`, `"contact"`, `"task"`, etc. |
| Load `r.obj.assignee` from DB before enforcer call | `obj.assignee` | `task, read` | If null, condition evaluates false → safe-fail DENY, but silently wrong |
| Pass `"search"` not `"read"` for list/query endpoints | `r.act` | Broker list/search endpoints | Underwriter incorrectly allowed to list brokers if `"read"` is passed |
| Apply scope query filters after policy ALLOW | query layer | All scoped reads | ALLOW without scope filter leaks cross-scope data |

---

## 6. Open Questions Blocking Policy Completion

| OQ | Question | Blocking test cases | Unblocked by |
|---|---|---|---|
| OQ-3 | ProgramManager broker:create, update, delete permissions | B-16, B-19, B-20 | PM clarification |
| OQ-4 | DistributionUser + RelationshipManager broker:delete | B-05, B-15 | F1-S5 (not yet written) |
| OQ-5 | DistributionUser + RelationshipManager contact:delete | C-04, C-12 | F1-S6 (not yet written) |
| OQ-6 | ProgramManager contact:read, create, update, delete | C-13 through C-16 | PM clarification |
| OQ-7 | DistributionUser vs DistributionManager naming | B-01 through B-04; C-01 through C-03 | PM + F1-S1/S2 naming alignment |

---

## 7. Resource-Action Review Checklist

| Area | Expected Control | Status | Notes |
|---|---|---|---|
| Endpoint access | Explicit policy per resource+action | ✅ Written | policy.csv covers all specified rows from matrix; pending OQ-3/4/5/6 |
| Object-level checks | Ownership or scope validation | ✅ Partial | Task ownership at policy layer; scope filters documented for query layer |
| Append-only resources | No mutate policy lines for timeline_event | ✅ Confirmed | No create/update/delete in policy; enforcer implicitly denies |
| ExternalUser isolation | All resources deny ExternalUser | ✅ Confirmed | No ExternalUser policy lines; implicit DENY all |
| Admin scoping | Unscoped on entity reads; own-tasks only on task widget | ✅ Confirmed | Admin task:read condition = r.obj.assignee == r.sub.id |
| Workflow actions | Transition-level authorization | ⚠ Not yet modeled | Workflow state machine stories not yet written; no policy lines for workflow transitions |
| Auditability | Denied/allowed decisions traceable | ⚠ Pending | Casbin audit logger contract not yet defined; Backend Developer to implement |

---

## 8. Gaps Remaining Before Implementation Sign-Off

1. **OQ-3, OQ-4, OQ-5, OQ-6** — resolve open questions to complete policy.csv rows marked PENDING.
2. **OQ-7** — align DistributionUser / DistributionManager naming across all stories before policy.csv is treated as final.
3. **Workflow transitions** — no Casbin policy lines for submission state transitions (e.g. Triage → Quote). Requires state machine story authorship first.
4. **Casbin audit logger** — logging contract (what is logged on ALLOW and DENY, at what level) not yet defined.
5. **Enforcement point integration tests** — §4 test cases must be executed against a running enforcer (not just inspected against policy.csv) for lifecycle gate promotion.

---

## 9. Sign-Off

- Security Reviewer: Pending (blocked by OQ-3 through OQ-7)
- Architect: Pending (blocked by workflow transition policy)
- Date: Pending

# Authorization Matrix (Requirements)

Owner: Product Manager
Status: Draft
Last Updated: 2026-02-17

Sources used: INCEPTION.md §1.2, §3.1, §3.2, §4.4; F0-S1, F0-S2, F0-S3, F0-S4, F0-S5; F1-S1, F1-S2.
No requirements invented. Gaps are marked "Not yet specified" with a reference to the blocking story.

---

## 1. Roles

| Role | Description | Source |
|------|-------------|--------|
| DistributionUser | Internal Distribution & Marketing user. Primary submission intake and broker management role. F1 stories refer to this role as "DistributionManager" — treated as the same role pending clarification (see Open Question 7). | INCEPTION §1.2, §3.2; F1-S1, F1-S2, F0-S1–S5 |
| Underwriter | Internal underwriter. Reviews triaged submissions and provides quote/bind decisions. Read-only access to broker and account context. | INCEPTION §1.2, §3.2, §4.4; F0-S2, F0-S3 |
| RelationshipManager | Internal broker relationship manager. Maintains broker/account relationships and timeline context. | INCEPTION §1.2, §3.2; F1-S2, F0-S4 |
| ProgramManager | Internal MGA/program manager. Oversees MGA program-level relationships. | INCEPTION §1.2, §3.2; F0-S1, F0-S4 |
| Admin | Internal administrator. Broad management access including policy administration. | INCEPTION §4.4 |
| ExternalUser | External broker/MGA user. No access to any MVP resource. Self-service portal deferred to future. | INCEPTION §3.1 non-goals |

---

## 2. Authorization Matrix

### 2.1 Broker

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | create | **ALLOW** | Must hold broker:create permission. License number must be globally unique (assumption — see Open Question 1). | F1-S1 AC1, AC3; INCEPTION §4.4 |
| DistributionUser | read / search | **ALLOW** | Full name and license search. InternalOnly metadata (inactive flags) visible. Results scoped to authorized entities. | F1-S2 AC1–AC4, Role Visibility; INCEPTION §4.4 |
| DistributionUser | update | **ALLOW** | Internal distribution role may update broker profile. | INCEPTION §4.4 |
| DistributionUser | delete | **Not yet specified** | Pending F1-S5 (not yet written). | — |
| Underwriter | create | **DENY** | Read-only access to broker context. | INCEPTION §4.4 |
| Underwriter | read | **ALLOW** | Read access to broker context for submission review. No write access. | INCEPTION §4.4 |
| Underwriter | update | **DENY** | Read-only access to broker context. | INCEPTION §4.4 |
| Underwriter | delete | **DENY** | Read-only access. | INCEPTION §4.4 |
| RelationshipManager | create | **ALLOW** | Internal relationship role may create brokers. | INCEPTION §4.4 |
| RelationshipManager | read / search | **ALLOW** | Full broker search. Results scoped to authorized entities. | F1-S2 Role Visibility; INCEPTION §4.4 |
| RelationshipManager | update | **ALLOW** | Internal relationship role may update broker profile. | INCEPTION §4.4 |
| RelationshipManager | delete | **Not yet specified** | Pending F1-S5 (not yet written). | — |
| ProgramManager | create | **Not yet specified** | Not addressed in F0/F1 stories or INCEPTION §4.4. | — |
| ProgramManager | read | **ALLOW** | Implied by broker activity feed scoped to their programs. | F0-S4 Role Visibility |
| ProgramManager | update | **Not yet specified** | Not addressed in available stories. | — |
| ProgramManager | delete | **Not yet specified** | Not addressed in available stories. | — |
| Admin | create | **ALLOW** | Full unscoped access. | F1-S1 Role Visibility; INCEPTION §4.4 |
| Admin | read / search | **ALLOW** | Full unscoped access. | F1-S2 Role Visibility; INCEPTION §4.4 |
| Admin | update | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| Admin | delete | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| ExternalUser | all | **DENY** | No external broker portal in MVP. | INCEPTION §3.1 non-goals |

**Constraints applying to all ALLOW decisions on Broker:**
- Duplicate license number on create must return a deterministic conflict error; the record must not be created. (F1-S1 edge case)
- All read results must be limited to entities the user is authorized to access; no cross-scope reads. (F1-S2 AC4)
- Broker records are InternalOnly in MVP; no content is visible to ExternalUser. (F1-S1, F1-S2 Data Visibility)

---

### 2.2 Contact

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | create | **ALLOW** | Internal distribution role may create contacts. | INCEPTION §4.4 |
| DistributionUser | read | **ALLOW** | Full contact read scoped to authorized entities. | INCEPTION §4.4 |
| DistributionUser | update | **ALLOW** | Internal distribution role may update contacts. | INCEPTION §4.4 |
| DistributionUser | delete | **Not yet specified** | Pending F1-S6 (not yet written). | — |
| Underwriter | create | **DENY** | Read-only access to contact context. | INCEPTION §4.4 |
| Underwriter | read | **ALLOW** | Read access to contact context. No write. | INCEPTION §4.4 |
| Underwriter | update | **DENY** | Read-only access. | INCEPTION §4.4 |
| Underwriter | delete | **DENY** | Read-only access. | INCEPTION §4.4 |
| RelationshipManager | create | **ALLOW** | Internal relationship role may create contacts. | INCEPTION §4.4 |
| RelationshipManager | read | **ALLOW** | Full contact read scoped to authorized entities. | INCEPTION §4.4 |
| RelationshipManager | update | **ALLOW** | Internal relationship role may update contacts. | INCEPTION §4.4 |
| RelationshipManager | delete | **Not yet specified** | Pending F1-S6 (not yet written). | — |
| ProgramManager | create | **Not yet specified** | Not addressed in available stories or INCEPTION §4.4. | — |
| ProgramManager | read | **Not yet specified** | Not addressed in available stories. | — |
| ProgramManager | update | **Not yet specified** | Not addressed in available stories. | — |
| ProgramManager | delete | **Not yet specified** | Not addressed in available stories. | — |
| Admin | create | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| Admin | read | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| Admin | update | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| Admin | delete | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| ExternalUser | all | **DENY** | No external contact access in MVP. | INCEPTION §3.1 non-goals |

**Constraints applying to all ALLOW decisions on Contact:**
- Contact data is InternalOnly in MVP; no content visible to ExternalUser. (F1-S1, F1-S2 Data Visibility)

---

### 2.3 Dashboard — KPI Cards

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Counts scoped to the user's authorized department and/or region. | F0-S1 Role Visibility, AC Checklist |
| Underwriter | read | **ALLOW** | Counts scoped to submissions assigned to or accessible by the user. | F0-S1 Role Visibility |
| RelationshipManager | read | **ALLOW** | Counts scoped to the user's managed broker relationships. | F0-S1 Role Visibility |
| ProgramManager | read | **ALLOW** | Counts scoped to the user's programs. | F0-S1 Role Visibility |
| Admin | read | **ALLOW** | Unscoped; sees all counts across all entities. | F0-S1 Role Visibility |
| ExternalUser | read | **DENY** | KPI data is InternalOnly. | F0-S1 Data Visibility |

**Constraints applying to all ALLOW decisions on KPI Cards:**
- Active Brokers count: includes only brokers within the user's authorized scope.
- Open Submissions and Renewal Rate: computed only from entities the user is authorized to access.
- Each card must show "—" (not an error) if underlying data is missing or the query fails; the failure must not block other widgets. (F0-S1 AC: edge cases, reliability)
- Read-only. No mutations are permitted from this view. (F0-S1 AC Checklist)

---

### 2.4 Dashboard — Pipeline Summary (Status Counts and Mini-Cards)

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Submissions and renewals scoped to user's department/region. Mini-card expansion uses the same scope. | F0-S2 Role Visibility, AC Checklist |
| Underwriter | read | **ALLOW** | Submissions assigned to or accessible by the user. | F0-S2 Role Visibility |
| RelationshipManager | read | **ALLOW** | Submissions and renewals linked to managed broker relationships. | F0-S2 Role Visibility |
| ProgramManager | read | **ALLOW** | Submissions and renewals within the user's programs. | F0-S2 Role Visibility |
| Admin | read | **ALLOW** | Unscoped; sees all statuses and mini-cards. | F0-S2 Role Visibility |
| ExternalUser | read | **DENY** | Pipeline data is InternalOnly. | F0-S2 Data Visibility |

**Constraints applying to all ALLOW decisions on Pipeline Summary:**
- Only non-terminal statuses are shown. Terminal statuses (Bound, Declined, Withdrawn, Lost, Lapsed) must be excluded. (F0-S2 Validation Rules)
- Zero-count status pills must remain visible; they may not be hidden. (F0-S2 edge cases)
- Mini-card expansion: up to 5 items per status; sorted by days-in-status descending (longest-stuck first). Same scope as counts. (F0-S2 edge cases)
- "View all" navigation must carry the same authorization scope to the destination list screen. (F0-S2 AC)
- Read-only. No mutations permitted from this view. (F0-S2 AC Checklist)

---

### 2.5 Dashboard — Nudge Cards

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Own overdue tasks + submissions and renewals within ABAC scope. | F0-S5 Role Visibility, Nudge Selection Rules |
| Underwriter | read | **ALLOW** | Own overdue tasks + submissions and renewals within ABAC scope. | F0-S5 Role Visibility |
| RelationshipManager | read | **ALLOW** | Own overdue tasks + submissions and renewals within ABAC scope. | F0-S5 Role Visibility |
| ProgramManager | read | **ALLOW** | Own overdue tasks + submissions and renewals within ABAC scope. | F0-S5 Role Visibility |
| Admin | read | **ALLOW** | Own overdue tasks + submissions and renewals within ABAC scope. | F0-S5 Role Visibility |
| ExternalUser | read | **DENY** | Nudge data is InternalOnly. | F0-S5 Data Visibility |

**Constraints applying to all ALLOW decisions on Nudge Cards:**
- Overdue task nudges: only tasks assigned to the authenticated user. Linked entity must not be soft-deleted. (F0-S5 Nudge Selection Rules, edge cases)
- Stale submission nudges: only submissions the user is authorized to access. Submission must not be soft-deleted. (F0-S5 Nudge Selection Rules)
- Upcoming renewal nudges: only renewals the user is authorized to access. (F0-S5 Nudge Selection Rules)
- Priority order is fixed: overdue tasks > stale submissions > upcoming renewals. Maximum 3 cards shown. (F0-S5 AC Checklist)
- Dismiss is session-scoped only (no persisted state in MVP). Dismiss does not constitute a mutation requiring audit. (F0-S5 AC Checklist, out of scope)
- If the nudge query fails, the "Needs Your Attention" section must be omitted entirely; the failure must not block other widgets. (F0-S5 Non-Functional)
- Read-only. No persisted mutations permitted from this view in MVP. (F0-S5 AC Checklist)

---

### 2.6 Task — Read Own Assigned Tasks

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Own tasks only (task assigned to the authenticated user). Done tasks excluded. | F0-S3 AC Checklist, Role Visibility |
| Underwriter | read | **ALLOW** | Own tasks only. Done tasks excluded. | F0-S3 Role Visibility |
| RelationshipManager | read | **ALLOW** | Own tasks only. Done tasks excluded. | F0-S3 Role Visibility |
| ProgramManager | read | **ALLOW** | Own tasks only. Done tasks excluded. | F0-S3 Role Visibility |
| Admin | read | **ALLOW** | Own tasks only for the dashboard widget. Viewing other users' tasks is explicitly Future (not MVP). | F0-S3 Role Visibility |
| ExternalUser | read | **DENY** | Task data is InternalOnly. | F0-S3 Data Visibility |

**Constraints applying to all ALLOW decisions on Task:**
- Only tasks in status Open or InProgress are returned. Done tasks are excluded from the dashboard widget view. (F0-S3 Validation Rules)
- A user may only read tasks where they are the assigned user. No cross-user task visibility in MVP. (F0-S3 AC Checklist, Non-Functional)
- If a linked entity on a task has been soft-deleted, the task is still displayed but the entity name must show as "[Deleted]". (F0-S3 edge cases)
- Read-only in dashboard context. No create, update, or delete from the dashboard widget in MVP. (F0-S3 out of scope)

---

### 2.7 Activity Timeline Event — Broker Events

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Events for brokers within the user's authorized scope (department/region). | F0-S4 Role Visibility, AC Checklist |
| Underwriter | read | **ALLOW** | Events for brokers linked to submissions accessible by the user. | F0-S4 Role Visibility |
| RelationshipManager | read | **ALLOW** | Events for brokers the user manages. | F0-S4 Role Visibility |
| ProgramManager | read | **ALLOW** | Events for brokers within the user's programs. | F0-S4 Role Visibility |
| Admin | read | **ALLOW** | Unscoped; sees all broker timeline events. | F0-S4 Role Visibility |
| ExternalUser | read | **DENY** | Timeline events are InternalOnly. | F0-S4 Data Visibility |

**Constraints applying to all ALLOW decisions on Activity Timeline Event:**
- Only events where EntityType = "Broker" are included in the dashboard feed view. (F0-S4 Validation Rules)
- Maximum 20 most recent events per load; sorted by occurrence time descending. (F0-S4 Validation Rules)
- If the actor account has been deactivated, the actor display name must show as "Unknown User" (not an error). (F0-S4 edge cases)
- Timeline event records are append-only and must never be modified or deleted by any role. (INCEPTION §1.4 non-negotiables)
- Read-only view. No mutations permitted from the dashboard feed. (F0-S4 AC Checklist)

---

## 3. InternalOnly Content Rule

All resources in this matrix are classified **InternalOnly** for MVP. No data is accessible to ExternalUser under any circumstances. This rule applies universally to all resources above.

Sources: INCEPTION §1.2 (external users are Future only), §3.1 non-goals ("No external broker/MGA self-service portal in MVP"), F0-S1 through F0-S5 Data Visibility sections, F1-S1 and F1-S2 Data Visibility sections.

---

## 4. Open Questions

| # | Question | Source | Blocking |
|---|----------|--------|---------|
| 1 | Is broker license uniqueness **global** or **state-scoped**? Current assumption is global. | F1-S1 Open Questions | Affects duplicate-detection constraint on broker:create for DistributionUser and Admin |
| 2 | Should **partial license number search** be enabled in MVP? Current assumption is exact license match only. | F1-S2 Open Questions | Affects broker:read/search scope for DistributionUser, RelationshipManager, Admin |
| 3 | What are broker:create, update, and delete permissions for **ProgramManager**? | Not addressed in F0/F1 stories or INCEPTION §4.4 | ProgramManager broker rows currently "Not yet specified" |
| 4 | What are broker:**delete** permissions for **DistributionUser** and **RelationshipManager**? | F1-S5 not yet written | Delete rows for both roles currently "Not yet specified" |
| 5 | What are contact:**delete** permissions for **DistributionUser** and **RelationshipManager**? | F1-S6 not yet written | Contact delete rows for both roles currently "Not yet specified" |
| 6 | What are contact read and write permissions for **ProgramManager**? | Not addressed in available stories or INCEPTION §4.4 | All contact rows for ProgramManager currently "Not yet specified" |
| 7 | Are **DistributionUser** and **DistributionManager** the same role? F1 stories use "DistributionManager"; F0 stories and INCEPTION use "Distribution User". If they are different permission levels (e.g., a Manager sub-role with create/update; a User sub-role with read-only), the broker:create and broker:update rows must be split. | F1-S1/S2 Role Visibility vs F0-S1–S5 Role Visibility | Affects broker:create and broker:update allow/deny decisions |

# Authorization Matrix (Requirements)

Owner: Product Manager
Status: Final
Last Updated: 2026-02-21

Sources used: INCEPTION.md §1.2, §3.1, §3.2, §4.3, §4.4; F0-S1, F0-S2, F0-S3, F0-S4, F0-S5; F1-S1 through F1-S7.
No requirements invented. Gaps are marked "Not yet specified" with a reference to the blocking story.

---

## 1. Roles

| Role | Description | Source |
|------|-------------|--------|
| DistributionUser | Internal Distribution & Marketing user. Works assigned opportunities only. | INCEPTION §1.2, §3.2; user requirement for assigned opportunities |
| DistributionManager | Internal Distribution manager. Can see and act on all opportunities within their region. | User requirement for manager access |
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
| DistributionUser | create | **ALLOW** | Must hold broker:create permission. License number must be globally unique. | F1-S1 AC1, AC3; INCEPTION §4.4 |
| DistributionUser | read / search | **ALLOW** | Full name search; license search is exact match only. InternalOnly metadata (inactive flags) visible. Results scoped to authorized entities. | F1-S2 AC1–AC4, Role Visibility; INCEPTION §4.4 |
| DistributionUser | update | **ALLOW** | Internal distribution role may update broker profile. | INCEPTION §4.4 |
| DistributionUser | delete | **ALLOW** | Scoped to authorized entities. Delete blocked if active submissions or renewals exist. | F1-S5 ACs |
| DistributionManager | create | **ALLOW** | Same as DistributionUser; manager can act across all opportunities. | F1-S1; user requirement |
| DistributionManager | read / search | **ALLOW** | Scoped to region; no team/user restrictions within region. License search is exact match only. | F1-S2 Role Visibility; user requirement |
| DistributionManager | update | **ALLOW** | Scoped to region; no team/user restrictions within region. | INCEPTION §4.4; user requirement |
| DistributionManager | delete | **ALLOW** | Scoped to region; no team/user restrictions within region. Delete blocked if active submissions or renewals exist. | F1-S5 ACs; user requirement |
| Underwriter | create | **DENY** | Read-only access to broker context. | INCEPTION §4.4 |
| Underwriter | read | **ALLOW** | Read access to broker context for submission review. No write access. | INCEPTION §4.4 |
| Underwriter | update | **DENY** | Read-only access to broker context. | INCEPTION §4.4 |
| Underwriter | delete | **DENY** | Read-only access. | INCEPTION §4.4 |
| RelationshipManager | create | **ALLOW** | Internal relationship role may create brokers. | INCEPTION §4.4 |
| RelationshipManager | read / search | **ALLOW** | Full broker search. License search is exact match only. Results scoped to authorized entities. | F1-S2 Role Visibility; INCEPTION §4.4 |
| RelationshipManager | update | **ALLOW** | Internal relationship role may update broker profile. | INCEPTION §4.4 |
| RelationshipManager | delete | **DENY** | Delete reserved for Distribution roles and Admin in MVP. | F1-S5 ACs |
| ProgramManager | create | **DENY** | Program managers are read-only for broker records in MVP. | INCEPTION §4.4 |
| ProgramManager | read | **ALLOW** | Implied by broker activity feed scoped to their programs. License search is exact match only. | F0-S4 Role Visibility |
| ProgramManager | update | **DENY** | Program managers are read-only for broker records in MVP. | INCEPTION §4.4 |
| ProgramManager | delete | **DENY** | Program managers are read-only for broker records in MVP. | INCEPTION §4.4 |
| Admin | create | **ALLOW** | Full unscoped access. | F1-S1 Role Visibility; INCEPTION §4.4 |
| Admin | read / search | **ALLOW** | Full unscoped access. | F1-S2 Role Visibility; INCEPTION §4.4 |
| Admin | update | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| Admin | delete | **ALLOW** | Full unscoped access. | INCEPTION §4.4 |
| ExternalUser | all | **DENY** | No external broker portal in MVP. | INCEPTION §3.1 non-goals |

**Constraints applying to all ALLOW decisions on Broker:**
- Duplicate license number on create must return a deterministic conflict error; the record must not be created. (F1-S1 edge case)
- All read results must be limited to entities the user is authorized to access; no cross-scope reads. (F1-S2 AC4)
- Broker records are InternalOnly in MVP; no content is visible to ExternalUser. (F1-S1, F1-S2 Data Visibility)
- Broker delete is blocked if active submissions or renewals exist. (F1-S5 ACs)

---

### 2.2 Contact

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | create | **ALLOW** | Internal distribution role may create contacts. | INCEPTION §4.4 |
| DistributionUser | read | **ALLOW** | Full contact read scoped to authorized entities. | INCEPTION §4.4 |
| DistributionUser | update | **ALLOW** | Internal distribution role may update contacts. | INCEPTION §4.4 |
| DistributionUser | delete | **DENY** | Delete reserved for DistributionManager and Admin in MVP. | F1-S6 ACs |
| DistributionManager | create | **ALLOW** | Same as DistributionUser; manager can act across all opportunities. | INCEPTION §4.4; user requirement |
| DistributionManager | read | **ALLOW** | Scoped to region; no team/user restrictions within region. | INCEPTION §4.4; user requirement |
| DistributionManager | update | **ALLOW** | Scoped to region; no team/user restrictions within region. | INCEPTION §4.4; user requirement |
| DistributionManager | delete | **ALLOW** | Scoped to region; no team/user restrictions within region. | F1-S6 ACs; user requirement |
| Underwriter | create | **DENY** | Read-only access to contact context. | INCEPTION §4.4 |
| Underwriter | read | **ALLOW** | Read access to contact context. No write. | INCEPTION §4.4 |
| Underwriter | update | **DENY** | Read-only access. | INCEPTION §4.4 |
| Underwriter | delete | **DENY** | Read-only access. | INCEPTION §4.4 |
| RelationshipManager | create | **ALLOW** | Internal relationship role may create contacts. | INCEPTION §4.4 |
| RelationshipManager | read | **ALLOW** | Full contact read scoped to authorized entities. | INCEPTION §4.4 |
| RelationshipManager | update | **ALLOW** | Internal relationship role may update contacts. | INCEPTION §4.4 |
| RelationshipManager | delete | **DENY** | Delete reserved for DistributionManager and Admin in MVP. | F1-S6 ACs |
| ProgramManager | create | **DENY** | Contact management is not within ProgramManager scope in MVP. | INCEPTION §4.4 |
| ProgramManager | read | **ALLOW** | Read-only for program context; no mutations. | INCEPTION §4.4 |
| ProgramManager | update | **DENY** | Contact management is not within ProgramManager scope in MVP. | INCEPTION §4.4 |
| ProgramManager | delete | **DENY** | Contact management is not within ProgramManager scope in MVP. | INCEPTION §4.4 |
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
| DistributionUser | read | **ALLOW** | Counts scoped to the user's assigned opportunities only. | F0-S1 Role Visibility; user requirement |
| DistributionManager | read | **ALLOW** | Scoped to region; no team/user restrictions within region. | F0-S1 Role Visibility; user requirement |
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
| DistributionUser | read | **ALLOW** | Submissions and renewals scoped to user's assigned opportunities only. | F0-S2 Role Visibility; user requirement |
| DistributionManager | read | **ALLOW** | Scoped to region; no team/user restrictions within region. | F0-S2 Role Visibility; user requirement |
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
| DistributionUser | read | **ALLOW** | Own overdue tasks + submissions and renewals within assigned opportunities only. | F0-S5 Role Visibility; user requirement |
| DistributionManager | read | **ALLOW** | Own overdue tasks + submissions and renewals within region. | F0-S5 Role Visibility; user requirement |
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

### 2.6 Task — Manage Own Tasks

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | create | **ALLOW** | Self-assigned tasks only. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| DistributionUser | read | **ALLOW** | Own tasks only (task assigned to the authenticated user). Dashboard list excludes Done. | F0-S3 AC Checklist, Role Visibility |
| DistributionUser | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| DistributionUser | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| DistributionManager | create | **ALLOW** | Self-assigned tasks only. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| DistributionManager | read | **ALLOW** | Own tasks only for the dashboard widget. Viewing other users' tasks is Future (not MVP). | F0-S3 Role Visibility |
| DistributionManager | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| DistributionManager | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| Underwriter | create | **ALLOW** | Self-assigned tasks only. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| Underwriter | read | **ALLOW** | Own tasks only. Dashboard list excludes Done. | F0-S3 Role Visibility |
| Underwriter | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| Underwriter | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| RelationshipManager | create | **ALLOW** | Self-assigned tasks only. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| RelationshipManager | read | **ALLOW** | Own tasks only. Dashboard list excludes Done. | F0-S3 Role Visibility |
| RelationshipManager | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| RelationshipManager | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| ProgramManager | create | **ALLOW** | Self-assigned tasks only. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| ProgramManager | read | **ALLOW** | Own tasks only. Dashboard list excludes Done. | F0-S3 Role Visibility |
| ProgramManager | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| ProgramManager | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| Admin | create | **ALLOW** | Self-assigned tasks only in MVP. `AssignedTo` must match authenticated subject. | F5-S1 ACs |
| Admin | read | **ALLOW** | Own tasks only for the dashboard widget. Viewing other users' tasks is explicitly Future (not MVP). | F0-S3 Role Visibility |
| Admin | update | **ALLOW** | Own tasks only. `AssignedTo` must match authenticated subject. | F5-S2 ACs |
| Admin | delete | **ALLOW** | Own tasks only. Soft delete only. | F5-S3 ACs |
| ExternalUser | all | **DENY** | Task data is InternalOnly. | F0-S3 Data Visibility |

**Constraints applying to all ALLOW decisions on Task:**
- A user may only create/update/delete tasks where `AssignedTo` equals their authenticated subject. No cross-user assignment in MVP. (F5-S1/S2/S3)
- A user may only read tasks where they are the assigned user. No cross-user task visibility in MVP. (F0-S3 AC Checklist, Non-Functional)
- Dashboard list excludes Done tasks; `GET /api/tasks/{taskId}` may return any status for own tasks. (F0-S3 Validation Rules)
- If a linked entity on a task has been soft-deleted, the task is still displayed but the entity name must show as "[Deleted]". (F0-S3 edge cases)
- Read-only in dashboard context. No create, update, or delete from the dashboard widget in MVP. (F0-S3 out of scope)

---

### 2.7 Activity Timeline Event — Broker Events

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Events for brokers within the user's authorized scope (assigned opportunities only). | F0-S4 Role Visibility; user requirement |
| DistributionManager | read | **ALLOW** | Broker events within region; no team/user restrictions within region. | F0-S4 Role Visibility; user requirement |
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

### 2.8 Submission — Read / Transition

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Submissions assigned to the user only. Applies to `GET /api/submissions/{submissionId}`. | F0-S2; user requirement |
| DistributionUser | transition | **ALLOW** | Only for assigned submissions and only for valid transitions. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.3; user requirement |
| DistributionManager | read | **ALLOW** | All submissions within region. Applies to `GET /api/submissions/{submissionId}`. | F0-S2; user requirement |
| DistributionManager | transition | **ALLOW** | Submissions within region; valid transitions only. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.3; user requirement |
| Underwriter | read | **ALLOW** | Submissions assigned to or accessible by the user. Applies to `GET /api/submissions/{submissionId}`. | INCEPTION §4.4 |
| Underwriter | transition | **ALLOW** | Underwriters can transition within underwriting stages. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.4; §4.3 |
| RelationshipManager | read | **ALLOW** | Submissions linked to managed broker relationships. Applies to `GET /api/submissions/{submissionId}`. | F0-S2 Role Visibility |
| RelationshipManager | transition | **DENY** | Read-only access; no submission transitions in MVP. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.4 |
| ProgramManager | read | **ALLOW** | Submissions within the user's programs. Applies to `GET /api/submissions/{submissionId}`. | F0-S2 Role Visibility |
| ProgramManager | transition | **DENY** | Read-only access; no submission transitions in MVP. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.4 |
| Admin | read | **ALLOW** | Unscoped access. Applies to `GET /api/submissions/{submissionId}`. | INCEPTION §4.4 |
| Admin | transition | **ALLOW** | Unscoped; valid transitions only. Applies to `POST /api/submissions/{submissionId}/transitions`. | INCEPTION §4.4; §4.3 |
| ExternalUser | all | **DENY** | No external portal in MVP. | INCEPTION §3.1 non-goals |

**Constraints applying to all ALLOW decisions on Submission:**
- Applies to `POST /api/submissions/{submissionId}/transitions` only.
- Invalid transition pairs return HTTP 409 with ProblemDetails code invalid_transition. (INCEPTION §4.3)
- Missing transition prerequisites return HTTP 409 with ProblemDetails code missing_transition_prerequisite. (INCEPTION §4.3)
- Every successful transition appends a WorkflowTransition and ActivityTimelineEvent record. (INCEPTION §4.3)

---

### 2.9 Renewal — Read / Transition

| Role | Action | Decision | Business Scope / Constraints | Story / AC Reference |
|------|--------|----------|------------------------------|----------------------|
| DistributionUser | read | **ALLOW** | Renewals assigned to the user only. Applies to `GET /api/renewals/{renewalId}`. | F0-S2; user requirement |
| DistributionUser | transition | **ALLOW** | Only for assigned renewals and only for valid transitions. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.3; user requirement |
| DistributionManager | read | **ALLOW** | All renewals within region. Applies to `GET /api/renewals/{renewalId}`. | F0-S2; user requirement |
| DistributionManager | transition | **ALLOW** | Renewals within region; valid transitions only. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.3; user requirement |
| Underwriter | read | **ALLOW** | Renewals assigned to or accessible by the user. Applies to `GET /api/renewals/{renewalId}`. | INCEPTION §4.4 |
| Underwriter | transition | **ALLOW** | Underwriters can transition within underwriting stages. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.4; §4.3 |
| RelationshipManager | read | **ALLOW** | Renewals linked to managed broker relationships. Applies to `GET /api/renewals/{renewalId}`. | F0-S2 Role Visibility |
| RelationshipManager | transition | **DENY** | Read-only access; no renewal transitions in MVP. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.4 |
| ProgramManager | read | **ALLOW** | Renewals within the user's programs. Applies to `GET /api/renewals/{renewalId}`. | F0-S2 Role Visibility |
| ProgramManager | transition | **DENY** | Read-only access; no renewal transitions in MVP. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.4 |
| Admin | read | **ALLOW** | Unscoped access. Applies to `GET /api/renewals/{renewalId}`. | INCEPTION §4.4 |
| Admin | transition | **ALLOW** | Unscoped; valid transitions only. Applies to `POST /api/renewals/{renewalId}/transitions`. | INCEPTION §4.4; §4.3 |
| ExternalUser | all | **DENY** | No external portal in MVP. | INCEPTION §3.1 non-goals |

**Constraints applying to all ALLOW decisions on Renewal:**
- Applies to `POST /api/renewals/{renewalId}/transitions` only.
- Invalid transition pairs return HTTP 409 with ProblemDetails code invalid_transition. (INCEPTION §4.3)
- Missing transition prerequisites return HTTP 409 with ProblemDetails code missing_transition_prerequisite. (INCEPTION §4.3)
- Every successful transition appends a WorkflowTransition and ActivityTimelineEvent record. (INCEPTION §4.3)

---

## 3. InternalOnly Content Rule

All resources in this matrix are classified **InternalOnly** for MVP. No data is accessible to ExternalUser under any circumstances. This rule applies universally to all resources above.

Sources: INCEPTION §1.2 (external users are Future only), §3.1 non-goals ("No external broker/MGA self-service portal in MVP"), F0-S1 through F0-S5 Data Visibility sections, F1-S1 and F1-S2 Data Visibility sections.

---

## 4. Open Questions

None.

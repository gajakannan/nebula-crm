# ADR-003: Task Entity and Nudge Engine Design

**Status:** Accepted

**Date:** 2026-02-14

**Deciders:** Architecture Team

**Technical Story:** Phase B — Dashboard F0 (S3: My Tasks, S5: Nudge Cards)

---

## Context and Problem Statement

Two dashboard widgets — **My Tasks & Reminders (S3)** and **Nudge Cards (S5)** — depend on a Task entity that is not yet defined in the data model. Additionally, nudge cards aggregate time-sensitive items from three different sources (overdue tasks, stale submissions, upcoming renewals) into a prioritized list of up to 3 cards.

**Key questions:**
1. What is the minimal Task entity shape needed for Dashboard MVP?
2. Where does the nudge computation logic live — frontend or backend?
3. How is nudge priority ordering enforced?

---

## Decision Drivers

- **Dashboard MVP:** Tasks widget and nudge cards are both High priority stories in F0
- **Feature F5 Alignment:** Task entity must support the future Task Center feature without rework
- **ABAC:** Task visibility must respect ownership (AssignedTo = current user) plus general ABAC scope
- **Performance:** Nudge computation must complete within 500 ms (p95) as part of the dashboard budget
- **Auditability:** Task mutations (create, update, complete) must generate timeline events

---

## Decision: Task Entity Design

### Task Table

**Table Name:** `Tasks`

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, NOT NULL | Unique identifier |
| Title | string(255) | NOT NULL | Task title |
| Description | string(2000) | NULL | Optional detail |
| Status | string(20) | NOT NULL, DEFAULT 'Open' | Open, InProgress, Done |
| Priority | string(20) | NOT NULL, DEFAULT 'Normal' | Low, Normal, High, Urgent |
| DueDate | Date | NULL | Optional due date |
| AssignedTo | string(255) | NOT NULL | Keycloak subject (user) |
| LinkedEntityType | string(50) | NULL | e.g., "Broker", "Submission", "Renewal" |
| LinkedEntityId | Guid | NULL | FK to linked entity (polymorphic) |
| CreatedAt | DateTime | NOT NULL | UTC timestamp |
| CreatedBy | string(255) | NOT NULL | Keycloak subject |
| UpdatedAt | DateTime | NOT NULL | UTC timestamp |
| UpdatedBy | string(255) | NOT NULL | Keycloak subject |
| CompletedAt | DateTime | NULL | When status changed to Done |
| IsDeleted | bool | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes

- `IX_Tasks_AssignedTo_Status_DueDate` — Composite index for My Tasks widget query
- `IX_Tasks_DueDate_Status` — For nudge computation (overdue tasks)
- `IX_Tasks_LinkedEntityType_LinkedEntityId` — For entity-linked task lookups

### Relationships

- **Many-to-One (polymorphic):** Task → Broker | Submission | Renewal | Account (via LinkedEntityType + LinkedEntityId)
- No hard FK constraint on LinkedEntityId (polymorphic reference); application-level validation ensures entity exists

### Audit Requirements

All Task mutations generate ActivityTimelineEvent:
- `TaskCreated` — on creation
- `TaskUpdated` — on field changes
- `TaskCompleted` — when Status changes to Done
- `TaskReopened` — when Status changes from Done back to Open/InProgress
- `TaskDeleted` — on soft delete

---

## Decision: Nudge Engine Design

### Server-Side Computation

Nudge logic runs **server-side** in a single endpoint (`GET /api/dashboard/nudges`). The backend executes three scoped queries in parallel, merges results by priority, and returns the top 3.

**Why server-side (not frontend):**
- ABAC scope filtering must happen before data leaves the server
- Cross-module query (tasks + submissions + renewals) is simpler to parallelise on the backend
- Avoids sending potentially large candidate lists to the client just to pick 3

### Nudge Computation Algorithm

```
1. Execute three queries in parallel (all ABAC-scoped):
   a. Overdue tasks: SELECT FROM Tasks
      WHERE AssignedTo = @currentUser
        AND DueDate < @today
        AND Status != 'Done'
        AND IsDeleted = false
        AND (LinkedEntityId IS NULL OR linked entity is not soft-deleted)
      ORDER BY DueDate ASC  -- most overdue first
      LIMIT 3

   b. Stale submissions: SELECT FROM Submissions
      JOIN WorkflowTransition (latest per entity)
      WHERE CurrentStatus NOT IN ('Bound','Declined','Withdrawn')
        AND DaysInCurrentStatus > 5
        AND ABAC-scoped
      ORDER BY DaysInCurrentStatus DESC  -- most stale first
      LIMIT 3

   c. Upcoming renewals: SELECT FROM Renewals
      WHERE RenewalDate BETWEEN @today AND @today+14
        AND CurrentStatus IN ('Created','Early')
        AND ABAC-scoped
      ORDER BY RenewalDate ASC  -- soonest first
      LIMIT 3

2. Merge results using priority fill:
   - Slot 1-3: Fill with overdue tasks first
   - Remaining slots: Fill with stale submissions
   - Remaining slots: Fill with upcoming renewals
   - Stop at 3 total

3. Return NudgeCard[] (max 3)
```

### Nudge Response Shape

```json
{
  "nudges": [
    {
      "nudgeType": "OverdueTask",
      "title": "Follow up with Acme Insurance",
      "description": "3 days overdue",
      "linkedEntityType": "Broker",
      "linkedEntityId": "uuid-here",
      "linkedEntityName": "Acme Insurance",
      "urgencyValue": 3,
      "ctaLabel": "Review Now"
    }
  ]
}
```

### DaysInCurrentStatus Computation

`DaysInCurrentStatus` is computed as:

```sql
EXTRACT(DAY FROM (CURRENT_DATE - MAX(wt.OccurredAt)))
```

where `wt` is the most recent WorkflowTransition for the entity. This is computed at query time (not stored), using the existing index on `WorkflowTransition(EntityId, OccurredAt DESC)`.

---

## Consequences

### Positive

- **Minimal Entity:** Task table is lean enough for MVP dashboard but extensible for F5 (Task Center)
- **No New Module:** Task lives in the existing TimelineAudit module (or a new lightweight TaskManagement module within the monolith)
- **Reusable:** Nudge endpoint can be extended with new nudge types (e.g., "pending approval") without changing the frontend
- **Testable:** Nudge priority logic is a pure function; unit-testable without database

### Negative

- **Polymorphic FK:** LinkedEntityType + LinkedEntityId is a loose coupling pattern — no DB-level referential integrity on linked entities. Mitigated by application-level validation and soft-delete awareness.
- **DaysInCurrentStatus is Computed:** Not stored, so requires a JOIN to WorkflowTransition on every nudge/pipeline query. Mitigated by the composite index and LIMIT 3 cap.

### Neutral

- **Task creation is manual for MVP:** Tasks are created by users or by workflow side-effects (e.g., "create follow-up task when submission enters WaitingOnBroker"). Automated task creation is a Phase 1 concern.

---

## Module Placement

The Task entity belongs to a new **TaskManagement** module within the modular monolith:

```
Nebula.Domain/
  TaskManagement/
    Task.cs                    # Domain entity
    TaskStatus.cs              # Value object/constants aligned to ReferenceTaskStatus seed values
    TaskPriority.cs            # Enum: Low, Normal, High, Urgent (CHECK constraint)

Nebula.Application/
  TaskManagement/
    CreateTaskCommand.cs
    UpdateTaskCommand.cs
    GetMyTasksQuery.cs
  Dashboard/
    GetNudgesQuery.cs          # Cross-module: reads Tasks, Submissions, Renewals
```

---

## Related ADRs

- ADR-002: Dashboard Data Aggregation Strategy (endpoint structure)
- ADR-004: Frontend Dashboard Widget Architecture (client-side dismiss handling)

---

**Last Updated:** 2026-02-22

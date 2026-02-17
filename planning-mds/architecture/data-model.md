# Nebula CRM — Data Model Supplement (Dashboard-First)

**Purpose:** This document supplements INCEPTION.md Section 4.2 with detailed data model specifications required by the Dashboard (F0) feature. It adds the **Task** entity (new) and documents **dashboard-specific indexes and query patterns**.

**Last Updated:** 2026-02-14

---

## 1. New Entity: Task

The Task entity is required by Dashboard stories S3 (My Tasks & Reminders) and S5 (Nudge Cards). It also serves as the foundation for Feature F5 (Task Center).

See [ADR-003](decisions/ADR-003-task-entity-nudge-engine.md) for design rationale.

### Table: `Tasks`

| Field | Type | Constraints | Default | Description |
|-------|------|-------------|---------|-------------|
| Id | uuid | PK, NOT NULL | gen_random_uuid() | Unique identifier |
| Title | varchar(255) | NOT NULL | — | Task title displayed in widgets |
| Description | varchar(2000) | NULL | — | Optional longer description |
| Status | varchar(20) | NOT NULL, CHECK IN ('Open','InProgress','Done') | 'Open' | Current task state |
| Priority | varchar(20) | NOT NULL, CHECK IN ('Low','Normal','High','Urgent') | 'Normal' | Task priority level |
| DueDate | date | NULL | — | Optional due date |
| AssignedTo | varchar(255) | NOT NULL | — | Keycloak subject (sub claim) of assigned user |
| LinkedEntityType | varchar(50) | NULL | — | Type of linked entity: 'Broker', 'Submission', 'Renewal', 'Account' |
| LinkedEntityId | uuid | NULL | — | ID of the linked entity (polymorphic; no hard FK) |
| CreatedAt | timestamptz | NOT NULL | now() | UTC creation timestamp |
| CreatedBy | varchar(255) | NOT NULL | — | Keycloak subject of creator |
| UpdatedAt | timestamptz | NOT NULL | now() | UTC last-update timestamp |
| UpdatedBy | varchar(255) | NOT NULL | — | Keycloak subject of last updater |
| CompletedAt | timestamptz | NULL | — | Set when Status transitions to Done |
| IsDeleted | boolean | NOT NULL | false | Soft delete flag |

### Indexes

| Index Name | Columns | Type | Purpose |
|-----------|---------|------|---------|
| `PK_Tasks_Id` | Id | PRIMARY KEY, clustered | — |
| `IX_Tasks_AssignedTo_Status_DueDate` | (AssignedTo, Status, DueDate) | B-tree | My Tasks widget query |
| `IX_Tasks_DueDate_Status` | (DueDate, Status) WHERE IsDeleted = false AND Status != 'Done' | Partial B-tree | Nudge: overdue task detection |
| `IX_Tasks_LinkedEntity` | (LinkedEntityType, LinkedEntityId) | B-tree | Entity-linked task lookups |

### Audit Events

| EventType | Trigger | Payload Fields |
|-----------|---------|---------------|
| TaskCreated | INSERT | title, assignedTo, dueDate, linkedEntityType, linkedEntityId |
| TaskUpdated | UPDATE (non-status) | changedFields |
| TaskCompleted | Status → Done | completedAt |
| TaskReopened | Status Done → Open/InProgress | previousCompletedAt |
| TaskDeleted | IsDeleted → true | — |

### Seed Data

- **No production seed data.** Tasks are user-created.
- **Dev/test seed:** Generate 20 tasks per test user using Faker, with varied DueDate spread (past, today, future) to exercise nudge and tasks widget edge cases.

---

## 2. Dashboard-Specific Query Patterns

These query patterns document how dashboard widgets access existing entities defined in INCEPTION.md Section 4.2. No schema changes are needed to existing entities — only indexes are added.

### 2.1 KPI Metrics Queries

| Metric | Query Pattern | Required Index |
|--------|--------------|---------------|
| Active Brokers | `SELECT COUNT(*) FROM Brokers WHERE Status = 'Active' AND IsDeleted = false` + ABAC scope | `IX_Brokers_Status` (exists) |
| Open Submissions | `SELECT COUNT(*) FROM Submissions WHERE CurrentStatus NOT IN ('Bound','Declined','Withdrawn')` + ABAC scope | `IX_Submissions_CurrentStatus` (new) |
| Renewal Rate | `SELECT COUNT(CASE WHEN CurrentStatus='Bound') / COUNT(*) FROM Renewals WHERE CurrentStatus IN ('Bound','Lost','Lapsed') AND updated_at > now()-90d` + ABAC scope | `IX_Renewals_CurrentStatus_UpdatedAt` (new) |
| Avg Turnaround | `SELECT AVG(wt.OccurredAt - s.CreatedAt) FROM Submissions s JOIN WorkflowTransition wt ON ... WHERE wt.ToState IN ('Bound','Declined','Withdrawn') AND wt.OccurredAt > now()-90d` | `IX_WorkflowTransition_EntityId_OccurredAt` (new) |

### 2.2 Pipeline Summary Queries

**Submission pipeline counts:**
```sql
SELECT CurrentStatus, COUNT(*)
FROM Submissions
WHERE CurrentStatus NOT IN ('Bound', 'Declined', 'Withdrawn')
  -- + ABAC scope filter
GROUP BY CurrentStatus
```

**Renewal pipeline counts:**
```sql
SELECT CurrentStatus, COUNT(*)
FROM Renewals
WHERE CurrentStatus NOT IN ('Bound', 'Lost', 'Lapsed')
  -- + ABAC scope filter
GROUP BY CurrentStatus
```

### 2.3 Pipeline Popover Mini-Cards (Lazy)

```sql
SELECT
  s.Id,
  COALESCE(a.Name, b.LegalName) AS EntityName,
  s.PremiumEstimate AS Amount,
  EXTRACT(DAY FROM (CURRENT_DATE - MAX(wt.OccurredAt))) AS DaysInStatus,
  up.DisplayName AS AssignedUserDisplayName,
  SUBSTRING(up.DisplayName, 1, 2) AS AssignedUserInitials
FROM Submissions s
  LEFT JOIN Accounts a ON s.AccountId = a.Id
  LEFT JOIN Brokers b ON s.BrokerId = b.Id
  LEFT JOIN WorkflowTransition wt ON wt.EntityId = s.Id AND wt.WorkflowType = 'Submission'
  LEFT JOIN UserProfile up ON up.Subject = s.UpdatedBy
WHERE s.CurrentStatus = @status
  -- + ABAC scope filter
GROUP BY s.Id, a.Name, b.LegalName, s.PremiumEstimate, up.DisplayName
ORDER BY DaysInStatus DESC
LIMIT 5
```

### 2.4 Activity Feed Query

```sql
SELECT
  ate.Id,
  ate.EventType,
  ate.EventPayloadJson,
  ate.OccurredAt,
  b.LegalName AS BrokerName,
  up.DisplayName AS ActorDisplayName,
  ate.EntityId
FROM ActivityTimelineEvent ate
  JOIN Brokers b ON ate.EntityId = b.Id AND ate.EntityType = 'Broker'
  LEFT JOIN UserProfile up ON up.Subject = ate.ActorSubject
WHERE ate.EntityType = 'Broker'
  -- + ABAC scope filter on broker visibility
ORDER BY ate.OccurredAt DESC
LIMIT 20
```

---

## 3. New Indexes on Existing Tables

These indexes are required for dashboard query performance. They do not change any existing table schema.

| Table | Index Name | Columns | Type | Dashboard Use |
|-------|-----------|---------|------|--------------|
| Submissions | `IX_Submissions_CurrentStatus` | (CurrentStatus) WHERE CurrentStatus NOT IN terminal | Partial B-tree | KPI open count, pipeline grouping |
| Renewals | `IX_Renewals_CurrentStatus` | (CurrentStatus) | B-tree | KPI renewal rate, pipeline grouping |
| Renewals | `IX_Renewals_RenewalDate_Status` | (RenewalDate, CurrentStatus) | B-tree | Nudge: upcoming renewals |
| WorkflowTransition | `IX_WT_EntityId_OccurredAt` | (EntityId, OccurredAt DESC) | B-tree | DaysInStatus computation, avg turnaround |
| ActivityTimelineEvent | `IX_ATE_EntityType_OccurredAt` | (EntityType, OccurredAt DESC) | B-tree | Broker activity feed |

---

## 4. Entity Relationship Diagram (Dashboard Scope)

```
                        ┌─────────────┐
                        │  Dashboard  │ (virtual — no table)
                        └──────┬──────┘
               ┌───────────────┼───────────────────────┐
               │               │                       │
      ┌────────▼────────┐ ┌────▼──────┐  ┌─────────────▼────────────┐
      │     Tasks       │ │  KPIs     │  │  Pipeline Summary        │
      │ (new entity)    │ │ (computed)│  │  (computed from S + R)   │
      └────────┬────────┘ └───────────┘  └─────────────────────────┘
               │
    ┌──────────┼──────────────┐
    │ LinkedEntityType/Id     │ (polymorphic)
    │                         │
┌───▼───┐  ┌─────────┐  ┌────▼────┐  ┌─────────┐
│Broker │  │ Account │  │Submissn │  │ Renewal │
└───┬───┘  └─────────┘  └────┬────┘  └────┬────┘
    │                        │             │
    │   ┌────────────────────┴─────────────┘
    │   │
┌───▼───▼──────────────────┐  ┌──────────────────────────┐
│ ActivityTimelineEvent    │  │ WorkflowTransition       │
│ (append-only)            │  │ (append-only)            │
└──────────────────────────┘  └──────────────────────────┘
```

---

## 5. Migration Strategy

### EF Core Migration Order (Dashboard-First)

1. **Migration 001:** Create `Tasks` table with all columns and indexes
2. **Migration 002:** Add dashboard-specific indexes to existing tables (Submissions, Renewals, WorkflowTransition, ActivityTimelineEvent)
3. **Migration 003:** Seed reference data for TaskStatus and TaskPriority (if using reference tables instead of CHECK constraints — TBD based on INCEPTION.md pattern of reference tables for configurable data)

**Decision:** Task Status and Priority use CHECK constraints (not reference tables) because they are not admin-configurable in MVP. If future requirements allow custom task statuses, migrate to reference tables at that time.

---

## Related Documents

- [INCEPTION.md Section 4.2](../INCEPTION.md) — Core entity definitions
- [ADR-003: Task Entity and Nudge Engine](decisions/ADR-003-task-entity-nudge-engine.md) — Design rationale
- [ADR-002: Dashboard Data Aggregation](decisions/ADR-002-dashboard-data-aggregation.md) — Endpoint structure
- [SOLUTION-PATTERNS.md](SOLUTION-PATTERNS.md) — Audit, ABAC, and repository patterns

---

**Version:** 1.0

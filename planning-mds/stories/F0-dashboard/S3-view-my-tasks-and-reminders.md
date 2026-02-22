# S3: View My Tasks and Reminders

**Story ID:** S3
**Epic/Feature:** F0 — Dashboard
**Title:** View My Tasks and Reminders
**Priority:** High
**Phase:** MVP

## User Story

**As a** Distribution User, Underwriter, or Relationship Manager
**I want** to see my assigned tasks and upcoming reminders on the dashboard
**So that** I can prioritize my day and avoid missing deadlines or follow-ups.

## Context & Background

Follow-up tasks and reminders are currently tracked in personal calendars, sticky notes, or email flags. A centralized task widget on the dashboard surfaces the most urgent items on login, reducing the risk of missed broker follow-ups, overdue submission reviews, and forgotten renewal outreach.

## Acceptance Criteria

**Happy Path:**
- **Given** the user is authenticated and on the Dashboard
- **When** the dashboard loads
- **Then** the My Tasks & Reminders widget displays:
  - Tasks assigned to the logged-in user (matched by Subject/UserId), sorted by DueDate ascending (soonest first)
  - Maximum 10 items displayed; if more exist, a "View all tasks" link is shown
  - Each task row shows: task title, due date, status (Open/InProgress/Done), and linked entity name (e.g., "Broker: Acme Insurance")

**Overdue Highlighting:**
- **Given** a task's DueDate is before the current date and status is not Done
- **When** the widget renders
- **Then** the task row is visually marked as overdue (e.g., red text or overdue badge)

**Click-Through:**
- **Given** the user clicks a task row
- **When** the click is processed
- **Then** the user is navigated to the linked entity's detail view (e.g., Broker 360) or the Task Center if no entity link exists

**"View All" Link:**
- **Given** the user has more than 10 tasks
- **When** the widget renders
- **Then** a "View all tasks" link appears at the bottom, navigating to Task Center

**Edge Cases:**
- No tasks assigned → Display: "No tasks assigned. You're all caught up."
- All tasks are Done → Display: "No tasks assigned. You're all caught up." (Done tasks are excluded)
- DueDate is null → Sort to end of list; display "No due date"
- Linked entity was deleted (soft-deleted) → Display task but show entity name as "[Deleted]"

**Checklist:**
- [ ] Tasks filtered to logged-in user only
- [ ] Sorted by DueDate ascending (nulls last)
- [ ] Max 10 items displayed with "View all" overflow link
- [ ] Overdue tasks visually highlighted
- [ ] Click navigates to linked entity or Task Center
- [ ] Empty state message when no open tasks
- [ ] Widget loads within the overall dashboard p95 < 2s target
- [ ] Permission check: only tasks for the authenticated and authorized user are returned
- [ ] Audit/timeline requirement: N/A (read-only view with no mutation)

## Data Requirements

**Required Fields (per task row):**
- TaskTitle: string
- DueDate: date | null
- Status: "Open" | "InProgress" | "Done"
- LinkedEntityType: string | null (e.g., "Broker", "Submission")
- LinkedEntityId: uuid | null
- LinkedEntityName: string | null

**Validation Rules:**
- Only tasks with Status in (Open, InProgress) are displayed
- Tasks must belong to the authenticated user (AssignedTo = current Subject)

## Role-Based Visibility

**Roles that can view their tasks:**
- All authenticated internal roles see only their own assigned tasks
- Admin may additionally see unassigned tasks (Future; not MVP)

**Data Visibility:**
- Task data is InternalOnly.

## Non-Functional Expectations

- Performance: Widget must render within the overall dashboard p95 < 2s target
- Security: Backend must filter tasks by authenticated user's Subject; no cross-user task visibility
- Reliability: If query fails, display "Unable to load tasks" and log the error; do not block other widgets

## Dependencies

**Depends On:**
- Task entity with AssignedTo, DueDate, Status fields
- ~~Task Center screen (for "View all" navigation)~~ — Not in F0/F1 scope; link hidden per MVP Navigation Constraints
- Broker 360 (F1-S3) for Broker-linked task click-through — **available**
- ~~Submission/Renewal/Account detail screens~~ — Not in F0/F1 scope; entity names render as plain text per MVP Navigation Constraints

**Related Stories:**
- F5 — Task Center + Reminders (full task management feature)
- S4 — Broker Activity Feed (complementary dashboard widget)

## Out of Scope

- Creating or editing tasks from the dashboard widget (Task Center/API only in MVP)
- Snooze or dismiss functionality
- Task notifications (push/email)
- Viewing other users' tasks
- Recurring task display

## UI/UX Notes

- Screens involved: Dashboard
- Layout: Vertical list below or beside the Pipeline Summary widget
- Each row: task title (bold), due date (right-aligned), status chip, linked entity name (secondary text)
- Overdue rows: red/amber left border or overdue badge

## Questions & Assumptions

**Assumptions:**
- Tasks have an AssignedTo field that matches Keycloak Subject
- "Done" tasks are excluded from the widget (only Open and InProgress shown)
- Task entity is created as part of F0/F1 foundation (see data-model.md); write endpoints deferred to F5

**MVP Navigation Constraints (confirmed):**
- Broker 360 click-through works (F1-S3 in scope).
- Task rows linked to Submission, Renewal, or Account render the entity name as plain text (not clickable) until those detail screens exist.
- Task rows with no linked entity are informational only (no navigation).
- "View all tasks" link is hidden until Task Center (F5) exists.
- See [feature-assembly-plan.md — MVP Navigation Constraints](../../architecture/feature-assembly-plan.md) for full degradation rules.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled (empty state, null due dates, deleted entities, query failure)
- [ ] Permissions enforced (user sees only own tasks)
- [ ] Audit/timeline logged: N/A (read-only)
- [ ] Tests pass (unit test for sorting/filtering logic, integration test for user-scoped query)
- [ ] Accessible: task list has proper ARIA roles (role="list")

# S5: View Nudge Cards

**Story ID:** S5
**Epic/Feature:** F0 — Dashboard
**Title:** View Nudge Cards
**Priority:** High
**Phase:** MVP

## User Story

**As a** Distribution User, Underwriter, or Relationship Manager
**I want** to see dismissible action prompts for time-sensitive items at the top of my dashboard
**So that** I can immediately act on the most urgent things without scanning through widgets.

## Context & Background

Inspired by Copper CRM's "Keep Things Moving" pattern, nudge cards are prominent, dismissible prompts that surface items requiring immediate attention. Unlike the tasks widget (which shows a full list), nudge cards highlight only the top 1–3 most urgent items with a clear call-to-action button. They create a sense of directed urgency on login — "here's what needs you right now."

## Acceptance Criteria

**Happy Path:**
- **Given** the user is authenticated and on the Dashboard
- **When** the dashboard loads
- **Then** a "Needs Your Attention" section appears at the top of the dashboard (above KPI cards) showing up to 3 nudge cards, selected by priority:
  1. **Overdue tasks** (highest priority): Tasks assigned to the user where DueDate < today and Status is not Done. Card shows: task title, how many days overdue, linked entity name. CTA button: "Review Now" (navigates to linked entity or Task Center).
  2. **Stale submissions** (second priority): Submissions in a non-terminal status where DaysInStatus > 5 and the user has access. Card shows: submission reference, broker name, days stuck, current status. CTA button: "Take Action" (navigates to Submission detail).
  3. **Upcoming renewal outreach** (third priority): Renewals with RenewalDate within 14 days and CurrentStatus in (Created, Early) that the user can access. Card shows: renewal reference, account name, days until renewal date. CTA button: "Start Outreach" (navigates to Renewal detail).

**Dismiss Behavior:**
- **Given** the user clicks the dismiss (X) button on a nudge card
- **When** the dismiss is processed
- **Then** the card is hidden for the current session (reappears on next login/page refresh if the condition still applies)
- **And** the dismissed card is replaced by the next eligible nudge (if any exist beyond the top 3)

**No Nudges Available:**
- **Given** no conditions match (no overdue tasks, no stale submissions, no upcoming renewals)
- **When** the dashboard loads
- **Then** the "Needs Your Attention" section is not rendered at all (no empty state — the section is absent)

**Edge Cases:**
- Multiple overdue tasks → Show only the most overdue (oldest DueDate first); remaining appear in Tasks widget
- All 3 slots filled by overdue tasks → Stale submissions and renewals do not appear (overdue tasks take precedence)
- User dismisses all 3 cards → Section collapses; no more nudges shown until next page load
- Linked entity was soft-deleted → Do not generate a nudge for deleted entities
- User has no tasks, submissions, or renewals (new user) → Section does not render

**Checklist:**
- [ ] Up to 3 nudge cards displayed above KPI cards
- [ ] Priority order: overdue tasks > stale submissions > upcoming renewals
- [ ] Each card has: icon, title/description, urgency indicator, CTA button
- [ ] Dismiss (X) hides card for current session
- [ ] Dismissed card replaced by next eligible nudge
- [ ] CTA navigates to the relevant entity detail screen
- [ ] Section absent when no nudges qualify
- [ ] Widget loads within the overall dashboard p95 < 2s target
- [ ] Authorization check: nudges include only items visible to authenticated user permissions (ownership + ABAC)
- [ ] Audit/timeline requirement: N/A (read-only view; dismiss is session-only UI state with no persisted mutation)

## Data Requirements

**Required Fields (per nudge card):**
- NudgeType: "OverdueTask" | "StaleSubmission" | "UpcomingRenewal"
- Title: string (e.g., "Follow up with Acme Insurance")
- Description: string (e.g., "3 days overdue")
- LinkedEntityType: string (e.g., "Task", "Submission", "Renewal")
- LinkedEntityId: uuid
- LinkedEntityName: string (display name for context)
- UrgencyValue: integer (days overdue, days stuck, or days until renewal)
- CTALabel: string (e.g., "Review Now", "Take Action", "Start Outreach")

**Nudge Selection Rules:**
- Overdue tasks: AssignedTo = current user AND DueDate < today AND Status != Done AND linked entity is not soft-deleted
- Stale submissions: DaysInStatus > 5 AND CurrentStatus is non-terminal AND user has read access (ABAC) AND submission is not soft-deleted
- Upcoming renewals: RenewalDate between today and today + 14 days AND CurrentStatus in (Created, Early) AND user has read access (ABAC)

**Sorting within each type:**
- Overdue tasks: most overdue first (oldest DueDate)
- Stale submissions: most stale first (highest DaysInStatus)
- Upcoming renewals: soonest first (nearest RenewalDate)

## Role-Based Visibility

**Roles that can view Nudge Cards:**
- All authenticated internal roles see nudges scoped to their own data:
  - Tasks: only tasks assigned to the user
  - Submissions: only submissions the user can access per ABAC
  - Renewals: only renewals the user can access per ABAC

**Data Visibility:**
- All nudge data is InternalOnly.

## Non-Functional Expectations

- Performance: Nudge cards must render within the overall dashboard p95 < 2s target. The nudge query should aggregate across tasks, submissions, and renewals in a single backend call (or parallelized calls).
- Security: Backend must enforce task ownership (AssignedTo) and Casbin ABAC scope for submissions/renewals
- Reliability: If nudge query fails, omit the "Needs Your Attention" section entirely (do not show error); log the failure. Dashboard must still render all other widgets.

## Dependencies

**Depends On:**
- Task entity with AssignedTo, DueDate, Status fields (for overdue task nudges)
- Submission entity with CurrentStatus, DaysInStatus computation (for stale submission nudges)
- Renewal entity with RenewalDate, CurrentStatus (for upcoming renewal nudges)
- Broker 360 (F1-S3) for Broker-linked overdue task CTA — **available**
- ~~Submission Detail (for "Take Action" CTA)~~ — Not in F0/F1 scope; CTA hidden per MVP Navigation Constraints
- ~~Renewal Detail (for "Start Outreach" CTA)~~ — Not in F0/F1 scope; CTA hidden per MVP Navigation Constraints

**Related Stories:**
- S3 — My Tasks & Reminders (tasks widget; nudges are the high-urgency subset)
- S2 — Pipeline Summary (stale submissions overlap with pipeline data)
- F3 — Submission Intake Workflow
- F4 — Renewal Pipeline
- F5 — Task Center + Reminders

## Out of Scope

- Persistent dismiss (remembering dismissals across sessions / stored in UserPreference) — Future enhancement
- Configurable nudge thresholds (e.g., user chooses "stale after 3 days" instead of 5) — Future
- Nudges for non-time-sensitive events (e.g., "New broker added" is informational, not a nudge)
- Nudge notifications via email or push
- More than 3 simultaneous nudge cards

## UI/UX Notes

- Screens involved: Dashboard (top section, above KPI cards)
- Layout: Horizontal row of up to 3 cards, each card roughly equal width
- Card anatomy: left icon (color-coded by type — red for overdue, amber for stale, blue for upcoming), title (bold), description (secondary text), CTA button (primary style), dismiss X button (top-right)
- Inspired by Copper CRM "Keep Things Moving" cards
- Cards have subtle background tint matching their urgency color
- Responsive: on narrow viewports, cards stack vertically
- Animation: smooth collapse when dismissed

## Questions & Assumptions

**Assumptions:**
- Session-scoped dismiss is acceptable for MVP (card reappears on next page load if condition persists)
- "Stale" threshold is > 5 days in current status, i.e. 6+ days (configurable in future, hardcoded for MVP)
- "Upcoming renewal" window is RenewalDate between today and today + 14 days (inclusive both ends)
- Maximum 3 nudge cards prevents visual overload
- Priority ordering (overdue > stale > upcoming) matches user expectations

**MVP Navigation Constraints (confirmed):**
- CTA "Review Now" for Broker-linked overdue tasks navigates to Broker 360 (F1-S3 in scope).
- CTA "Review Now" for non-Broker-linked tasks: CTA button hidden (card still displays title, description, urgency).
- CTA "Take Action" (Submission Detail): CTA button hidden until F3 exists.
- CTA "Start Outreach" (Renewal Detail): CTA button hidden until F4 exists.
- See [feature-assembly-plan.md — MVP Navigation Constraints](../../architecture/feature-assembly-plan.md) for full degradation rules.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled (no nudges, all dismissed, deleted entities, query failure)
- [ ] Permissions enforced (task ownership + ABAC for submissions/renewals)
- [ ] Audit/timeline logged: N/A (read-only; dismiss is client-side session state)
- [ ] Tests pass (unit test for nudge selection/priority logic, integration test for scoped queries)
- [ ] CTA navigation works for all nudge types
- [ ] Accessible: cards have role="alert" or role="status", dismiss button has aria-label="Dismiss"

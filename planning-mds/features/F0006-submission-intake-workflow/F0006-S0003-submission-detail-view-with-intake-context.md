---
template: user-story
version: 1.1
applies_to: product-manager
---

# F0006-S0003: Submission Detail View with Intake Context

**Story ID:** F0006-S0003
**Feature:** F0006 — Submission Intake Workflow
**Title:** Submission detail view with intake context
**Priority:** Critical
**Phase:** CRM Release MVP

## User Story

**As a** distribution user or underwriter
**I want** to view full submission details including linked account, broker, program, completeness status, and activity timeline
**So that** I can assess the submission's readiness for the next workflow step without switching screens or systems

## Context & Background

The submission detail view is the primary workspace for intake triage. Distribution users come here to review a submission's context, edit mutable intake fields as more information arrives, check completeness, log follow-up, assign an underwriter, and advance the submission through intake states. Underwriters come here to review the submission they have been assigned, see what outreach has happened, and confirm completeness before F0019 takes over for the review/quote phase. This view must render linked entity context (account, broker, program) and the completeness projection without requiring navigation away from the page.

## Acceptance Criteria

**Happy Path:**
- **Given** a distribution user with read access to a submission
- **When** they navigate to the submission detail view (e.g., from pipeline list row click)
- **Then** the following sections are displayed:
  - **Header:** Submission status badge, account name (linked), broker name (linked), LOB, effective date, assigned user, created date
  - **Submission Fields:** ProgramId (if linked), PremiumEstimate, Description, ExpirationDate
  - **Edit Action:** An explicit "Edit Intake Details" action that opens a form or dialog for mutable submission fields
  - **Completeness Panel:** Shows checklist of required fields and document categories with pass/fail indicators per item
  - **Activity Timeline:** Chronological list of ActivityTimelineEvents for this submission (most recent first)
  - **Action Bar:** Available transition buttons based on current state and user role

**Linked Entity Context:**
- **Given** the submission has an AccountId and BrokerId
- **When** the detail view loads
- **Then** account name, region, and industry are shown inline; broker name and license number are shown inline; clicking either navigates to the respective 360 view

**Happy Path — Edit Intake Details:**
- **Given** a distribution user with update access to a submission in their scope
- **When** they open the "Edit Intake Details" action, update mutable fields such as ProgramId, LineOfBusiness, PremiumEstimate, Description, or ExpirationDate, and save
- **Then** the submission detail reloads with updated values, completeness is re-evaluated, and a `SubmissionUpdated` timeline event is recorded when any field actually changed

**Completeness Panel:**
- **Given** a submission in Triaging state
- **When** the completeness panel renders
- **Then** each required field shows green check (populated) or red indicator (missing); each required document category shows count of linked documents or "Missing" indicator

**Alternative Flows / Edge Cases:**
- Submission not found or soft-deleted → HTTP 404 error page
- User lacks read permission → HTTP 403 error page
- Linked account or broker soft-deleted → show "[Deleted]" label with ID; do not block rendering
- No timeline events yet due to legacy or malformed seeded data → empty timeline section with message "No activity recorded yet"
- F0020 not available → document completeness section shows "Document management not yet configured" placeholder

**Checklist:**
- [ ] Header displays: status badge, account name (linked), broker name (linked), LOB, effective date, assigned user, created date/by
- [ ] Submission fields section: program, premium estimate, description, expiration date
- [ ] Edit action allows mutable intake fields to be updated from the detail workspace via a dedicated form or dialog
- [ ] Completeness panel: required-field checklist + document-category checklist (F0020-dependent with graceful fallback)
- [ ] Activity timeline section: chronological events, most recent first, with actor, timestamp, and event description
- [ ] Action bar: transition buttons rendered per current state and user role (F0006-S0004)
- [ ] Assignment display: shows current assigned user with edit action (F0006-S0006)
- [ ] Stale indicator: visual flag if submission exceeds stale threshold (F0006-S0008)
- [ ] ABAC enforced: user can only view submissions within their scope
- [ ] Responsive layout for desktop and tablet

## Data Requirements

**Required Fields (detail response):**
- All Submission entity fields
- `accountName`, `accountRegion`, `accountIndustry`: Denormalized from Account
- `brokerName`, `brokerLicenseNumber`: Denormalized from Broker
- `programName`: Denormalized from Program (nullable)
- `assignedToDisplayName`: Denormalized from UserProfile
- `completeness`: Object with field-level and document-level pass/fail status
- `timelineEvents`: Paginated list of ActivityTimelineEvents for EntityType=Submission, EntityId=this submission

**Validation Rules:**
- SubmissionId must be a valid uuid
- User must have ABAC read permission for this submission
- Edit actions require ABAC update permission for the submission

## Role-Based Visibility

**Roles that can view:**
- Distribution User — Submissions within assigned scope
- Distribution Manager — Submissions within region scope
- Underwriter — Submissions assigned to them
- Relationship Manager — Submissions for own accounts/brokers (read-only)
- Program Manager — Submissions within own programs (read-only)
- Admin — All submissions

**Roles that can edit mutable intake fields from the detail view:**
- Distribution User — Submissions within assigned scope
- Distribution Manager — Submissions within region scope
- Admin — All submissions
- Underwriter, Relationship Manager, Program Manager — Read-only in F0006 scope

**Data Visibility:**
- InternalOnly: All submission detail data is internal-only in MVP
- ExternalVisible: None

## Non-Functional Expectations

- Performance: Detail view loads in < 2s including completeness projection and timeline (first page)
- Security: ABAC-scoped read; no unauthorized submission data leaks
- Reliability: Graceful degradation for deleted linked entities; timeline paginates independently

## Dependencies

**Depends On:**
- F0006-S0001 — Navigation from pipeline list
- F0006-S0002 — Submissions must exist

**Related Stories:**
- F0006-S0004 — Action bar transition buttons
- F0006-S0005 — Completeness panel data
- F0006-S0006 — Assignment display and edit
- F0006-S0007 — Activity timeline section
- F0006-S0008 — Stale indicator

## Out of Scope

- Free-form inline editing of individual fields without an explicit edit action (use a dedicated edit form or dialog instead)
- Document upload directly from detail view (F0020 handles document management)
- Communication or email integration from detail view (F0021)
- Printing or PDF export of submission detail

## UI/UX Notes

- Screens involved: Submission Detail
- Key interactions: Status badge with color coding; linked entity names as clickable links to 360 views; an explicit edit action for mutable intake fields; completeness panel as a collapsible checklist; timeline as a scrollable, paginated feed; action bar with contextual transition buttons
- Layout: Header at top, two-column body (left: submission fields + completeness; right: timeline), action bar pinned at bottom or header

## Questions & Assumptions

**Open Questions:**
- None

**Assumptions (to be validated):**
- Timeline events are paginated independently (default 20 per page, load-more pattern)
- Completeness panel is read-only — it reflects state, while field edits happen through the dedicated edit action and document changes happen in F0020

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced (ABAC-scoped read)
- [ ] Audit/timeline logged for any successful field edits (`SubmissionUpdated`)
- [ ] Tests pass
- [ ] Documentation updated (if needed)
- [ ] Story filename matches `Story ID` prefix (`F0006-S0003-...`)
- [ ] Story index regenerated if story file was added/renamed/moved

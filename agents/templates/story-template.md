---
template: user-story
version: 1.0
applies_to: product-manager
---

# User Story Template

Use this template for all user stories to ensure consistency and completeness.

## Story Header

**Story ID:** [Unique identifier, e.g., S1, S2, US-001]
**Feature:** [Feature ID this story belongs to, e.g., F1]
**Title:** [Short descriptive title, e.g., "Broker CRUD - Create New Broker"]
**Priority:** [Critical | High | Medium | Low]
**Phase:** [MVP | Phase 1 | Phase 2 | Future]

## User Story

**As a** [specific persona - be precise, not just "user"]
**I want** [capability or feature]
**So that** [business value or benefit - the "why"]

## Context & Background

[Optional: Provide additional context about why this story exists, business drivers, or related stories]

## Acceptance Criteria

Use Given/When/Then format or checklist format. Be specific and testable.

**Happy Path:**
- **Given** [initial context or precondition]
- **When** [action or event]
- **Then** [expected outcome]
- **And** [additional expected outcome]

**Alternative Flows:**
- **Given** [different context]
- **When** [different action]
- **Then** [different outcome]

**Checklist Format Alternative:**
- [ ] Criterion 1 - specific, testable condition
- [ ] Criterion 2 - specific, testable condition
- [ ] Criterion 3 - specific, testable condition

## Edge Cases & Error Scenarios

Document non-happy-path scenarios:

**Validation Errors:**
- [Scenario]: [Expected behavior]
- Example: Missing required field → Show inline error "Field X is required"

**Business Rule Violations:**
- [Scenario]: [Expected behavior]
- Example: Duplicate license number → Show error "Broker with this license already exists"

**Permission Errors:**
- [Scenario]: [Expected behavior]
- Example: User lacks CreateBroker permission → Show 403 Forbidden error

**System Errors:**
- [Scenario]: [Expected behavior]
- Example: Database unavailable → Show "System temporarily unavailable, please try again"

## Audit & Timeline Requirements

For stories involving data mutations (create, update, delete, status changes):

- [ ] Create immutable timeline event with: timestamp, user ID, action type, entity ID
- [ ] Log all field changes (old value → new value) if applicable
- [ ] Ensure timeline events are visible in [Entity] 360 view
- [ ] Include audit context: IP address, user agent (if security-relevant)

## Data Requirements

List key data fields (non-technical, business perspective):

**Required Fields:**
- [Field name]: [Purpose/description]

**Optional Fields:**
- [Field name]: [Purpose/description]

**Calculated/Derived Fields:**
- [Field name]: [How it's derived]

## Role-Based Visibility

Specify who can see/do what:

**Roles that can [action]:**
- [Role 1]: [Specific permissions]
- [Role 2]: [Specific permissions]

**Data Visibility:**
- InternalOnly content: [What's internal]
- BrokerVisible content: [What brokers can see - if applicable]

## Dependencies

**Depends On (Blockers):**
- [Story ID]: [Why this is needed first]

**Related Stories:**
- [Story ID]: [Relationship description]

## Out of Scope (Explicit Non-Goals)

Document what this story does NOT include to prevent scope creep:

- [Feature/capability]: Deferred to [Phase/Story]
- [Feature/capability]: Not needed for MVP
- [Feature/capability]: Explicitly excluded from requirements

## UI/UX Notes

[Optional: Screen mockups, wireframes, or detailed UI specifications]

**Screen(s):**
- [Screen name]: [Key UI elements or interactions]

**User Workflow:**
1. [Step 1]
2. [Step 2]
3. [Step 3]

## Technical Constraints (PM Awareness)

[Optional: Any known technical constraints that PM is aware of - NOT technical design]

Example: "Must integrate with existing Keycloak authentication"

## Questions & Assumptions

**Open Questions:**
- [ ] [Question that needs stakeholder input]

**Assumptions (to be validated):**
- [Assumption made in this story that may need confirmation]

## Definition of Done

- [ ] All acceptance criteria are met
- [ ] Edge cases and error scenarios are handled
- [ ] Audit/timeline events are logged (if applicable)
- [ ] Role-based permissions are enforced
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Code review approved
- [ ] Documentation updated (if needed)
- [ ] Deployed to [environment]

---

## Example Usage

See `product-manager/references/story-examples.md` for complete examples of well-written stories.

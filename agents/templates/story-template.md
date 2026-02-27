---
template: user-story
version: 1.1
applies_to: product-manager
---

# User Story Template

Use this template for all user stories to ensure consistency and completeness. Keep it domain-neutral; project-specific examples live in `planning-mds/examples/`.

One story per file. Story files are colocated in their feature folder at `planning-mds/features/F{NNNN}-{slug}/`.

Filename convention: `F{NNNN}-S{NNNN}-{slug}.md` (e.g., `F0001-S0001-create-record.md`).

## Story Header

**Story ID:** [F0001-S0001, F0001-S0002, ...]
**Feature:** [F0001 — Feature Name]
**Title:** [Short descriptive title]
**Priority:** [Critical | High | Medium | Low]
**Phase:** [MVP | Phase 1 | Phase 2 | Future]

## User Story

**As a** [specific persona]
**I want** [capability]
**So that** [business value]

## Context & Background

[Why this story exists and what it unlocks]

## Acceptance Criteria

Use Given/When/Then or a checklist. Be specific and testable.

**Happy Path:**
- **Given** [context]
- **When** [action]
- **Then** [expected outcome]

**Alternative Flows / Edge Cases:**
- [Scenario] → [Expected behavior]

**Checklist (if simpler):**
- [ ] [Specific, testable condition]
- [ ] [Specific, testable condition]

## Data Requirements

**Required Fields:**
- [Field]: [Purpose]

**Optional Fields:**
- [Field]: [Purpose]

**Validation Rules:**
- [Rule]

## Role-Based Visibility

**Roles that can [action]:**
- [Role] — [Permission]

**Data Visibility:**
- InternalOnly content: [What's internal]
- ExternalVisible content: [What external users can see]

## Non-Functional Expectations (If Applicable)

- Performance: [e.g., page loads < 2s]
- Security: [e.g., authorized users only]
- Reliability: [e.g., error handling expectations]

## Dependencies

**Depends On:**
- [Story ID] — [Reason]

**Related Stories:**
- [Story ID] — [Relationship]

## Out of Scope

- [Explicit non-goal]
- [Deferred capability]

## UI/UX Notes (Optional)

- Screens involved: [Screen name(s)]
- Key interactions: [Summary]

## Questions & Assumptions

**Open Questions:**
- [ ] [Question requiring stakeholder input]

**Assumptions (to be validated):**
- [Assumption]

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Audit/timeline logged (if applicable)
- [ ] Tests pass
- [ ] Documentation updated (if needed)

---

## Example Library

See `planning-mds/examples/stories/` for project-specific story examples.

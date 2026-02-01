---
template: epic
version: 1.0
applies_to: product-manager
---

# Epic Template

Use this template to define epics that group related user stories and align with business objectives.

## Epic Header

**Epic ID:** [Unique identifier, e.g., E1, E2, EPIC-001]
**Epic Name:** [Descriptive name, e.g., "Broker & MGA Relationship Management"]
**Status:** [Planned | In Progress | Completed | Deferred]
**Priority:** [Critical | High | Medium | Low]
**Phase:** [MVP | Phase 1 | Phase 2 | Future]

---

## Epic Statement

**As a** [primary persona(s)]
**I want** [high-level capability]
**So that** [business value or strategic benefit]

---

## Business Objective

[Clear statement of the business goal this epic achieves]

**Aligns With:**
- [Company strategic goal or initiative]
- [Product vision statement]

**Success Metrics:**
- [Metric 1]: [Target value]
- [Metric 2]: [Target value]
- [Metric 3]: [Target value]

---

## Problem Statement

### Current State
[Describe the current situation or pain point this epic addresses]

### Desired State
[Describe what the world looks like when this epic is complete]

### Impact
- **Users Affected:** [Which personas benefit]
- **Business Impact:** [Revenue, efficiency, compliance, etc.]
- **Risk if Not Addressed:** [What happens if we don't do this]

---

## Scope & Boundaries

### In Scope
High-level capabilities included in this epic:
- [Capability 1]
- [Capability 2]
- [Capability 3]

### Out of Scope (Explicit Exclusions)
What this epic does NOT include:
- [Excluded capability 1]: [Reason - deferred to Phase X, not needed, etc.]
- [Excluded capability 2]: [Reason]
- [Excluded capability 3]: [Reason]

### MVP vs Full Epic
**MVP Scope (minimum viable):**
- [Essential capability 1]
- [Essential capability 2]

**Full Epic Scope (nice-to-have):**
- [Additional capability 1] - Phase 1
- [Additional capability 2] - Future

---

## Target Personas

**Primary Personas:**
- [Persona 1]: [How they benefit]
- [Persona 2]: [How they benefit]

**Secondary Personas:**
- [Persona 3]: [Indirect benefit]

---

## User Stories

List all user stories that belong to this epic (summary view):

| Story ID | Story Title | Priority | Status | Phase |
|----------|-------------|----------|--------|-------|
| S1 | [Title] | High | Planned | MVP |
| S2 | [Title] | Medium | Planned | MVP |
| S3 | [Title] | Low | Planned | Phase 1 |

**Total Stories:** [Count]
**MVP Stories:** [Count]
**Completed:** [Count]

---

## Key Features & Capabilities

Break down the epic into 3-7 major features:

### Feature 1: [Feature Name]
**Description:** [What this feature does]
**User Value:** [Why this matters to users]
**Stories:** [S1, S2]

### Feature 2: [Feature Name]
**Description:** [What this feature does]
**User Value:** [Why this matters to users]
**Stories:** [S3, S4]

### Feature 3: [Feature Name]
**Description:** [What this feature does]
**User Value:** [Why this matters to users]
**Stories:** [S5, S6]

---

## User Workflows

### Primary Workflow: [Workflow Name]
**Trigger:** [What initiates this workflow]
**Actors:** [Who is involved]

**Steps:**
1. [User action 1] → [System response 1]
2. [User action 2] → [System response 2]
3. [User action 3] → [System response 3]

**Outcome:** [End state or result]

### Secondary Workflow: [Another Workflow]
[Repeat structure above]

---

## Dependencies

### Depends On (Blockers)
- [Epic ID or capability]: [Why this is needed first]
- [Infrastructure/platform requirement]: [Reason]

### Blocks (Downstream Dependencies)
- [Epic ID]: [How this epic enables other work]

### Related Epics
- [Epic ID]: [Relationship description]

---

## Assumptions & Constraints

### Assumptions
- [Assumption 1 about user behavior, business rules, or technical capabilities]
- [Assumption 2]
- [Assumption 3]

### Constraints
- **Technical:** [Known technical limitations]
- **Business:** [Business rules or policies]
- **Regulatory:** [Compliance requirements]
- **Timeline:** [Time constraints if any]

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation Strategy |
|------|------------|--------|---------------------|
| [Risk description] | High/Med/Low | High/Med/Low | [How to address] |
| [Risk description] | High/Med/Low | High/Med/Low | [How to address] |

---

## Screens & UI Impact

**New Screens:**
- [Screen 1]: [Purpose]
- [Screen 2]: [Purpose]

**Modified Screens:**
- [Screen 1]: [What changes]
- [Screen 2]: [What changes]

---

## Non-Functional Requirements (NFR)

### Performance
- [Performance expectation, e.g., "List 1000 brokers in < 2 seconds"]

### Security
- [Security requirements, e.g., "Enforce row-level authorization"]

### Scalability
- [Scale expectations, e.g., "Support 10,000 broker records"]

### Auditability
- [Audit requirements, e.g., "All mutations must create timeline events"]

---

## Open Questions

- [ ] [Question 1 that needs stakeholder input]
- [ ] [Question 2 that needs technical validation]
- [ ] [Question 3 that needs user research]

---

## Definition of Done (Epic-Level)

- [ ] All MVP stories are completed and deployed
- [ ] Success metrics are defined and measurable
- [ ] User acceptance testing completed
- [ ] Documentation updated (user guides, API docs)
- [ ] Performance NFRs met
- [ ] Security review passed
- [ ] Stakeholder demo completed and approved

---

## Timeline (High-Level)

**Target Start:** [Date or Phase]
**Target Completion:** [Date or Phase]
**Actual Completion:** [Date - filled in when done]

**Milestones:**
- [Milestone 1]: [Date or dependency]
- [Milestone 2]: [Date or dependency]

---

## Notes & Context

[Any additional context, background research, competitive analysis, or links to related documentation]

---

## Version History

**Version 1.0** - [Date] - Epic defined
**Version 1.1** - [Date] - [Changes based on feedback or scope refinement]

---

## Example Usage

See `product-manager/references/epic-examples.md` for complete examples of well-defined epics.

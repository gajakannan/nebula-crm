# Product Manager Best Practices

Generic guidance for writing high-quality product requirements across domains.

## User Story Best Practices

### The Three C's: Card, Conversation, Confirmation

- **Card:** The written story (As a... I want... So that...)
- **Conversation:** Discussion with stakeholders to uncover details
- **Confirmation:** Acceptance criteria that define done

### Writing Effective User Stories

**DO:**
- Focus on user value
- Keep stories small and independent
- Write from the user perspective
- Include the "So that" business value
- Be specific about the persona

**DON'T:**
- Describe technical solutions
- Introduce unnecessary dependencies
- Use vague language without criteria
- Invent business rules

### Story Sizing

**Too Small (Tasks, not stories):**
- "Add a field to a table"
- "Change a button color"

**Right Size (Vertical slice):**
- "Create a new record with basic fields"
- "Search records by name"

**Too Large (Epics, not stories):**
- "Build the management system"
- "Implement end-to-end workflow"

## INVEST Criteria

Every story should be:
- **Independent**
- **Negotiable**
- **Valuable**
- **Estimable**
- **Small**
- **Testable**

## Vertical Slicing

Prefer slices that deliver end-to-end value (UI → API → DB). Avoid horizontal slices (all UI, then all API).

## Acceptance Criteria Patterns

**Given/When/Then** for scenarios; **checklists** for simple requirements.

**Example:**
- Given I am authorized
- When I submit valid data
- Then the record is created and appears in the list

## Epic Decomposition

Epics should be decomposable into 5–10 stories. Each story should be independently testable.

## Persona Development

Define personas with:
- Goals
- Pain points
- Jobs-to-be-done

## Requirements Elicitation

Ask clarifying questions when business rules are unclear. Do not assume.

## Scope Management

Define explicit non-goals to prevent scope creep.

---

See `planning-mds/examples/` for project-specific examples.

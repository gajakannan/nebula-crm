---
template: acceptance-criteria-checklist
version: 1.0
applies_to: product-manager, quality-engineer
---

# Acceptance Criteria Quality Checklist

Use this checklist to validate that acceptance criteria are well-written, testable, and complete.

## Purpose

Acceptance criteria define when a user story is "done" from a functional perspective. Poor acceptance criteria lead to:
- Ambiguous implementations
- Missed edge cases
- Failed QA cycles
- Rework and delays

This checklist ensures acceptance criteria meet quality standards before handoff to development.

---

## Core Quality Criteria

### ✅ Clarity & Specificity

- [ ] **Unambiguous**: Criteria use precise language with no room for interpretation
- [ ] **Specific**: Each criterion describes a single, specific behavior or outcome
- [ ] **Observable**: Each criterion describes something that can be observed or measured
- [ ] **No Technical Jargon**: Written in business language, not implementation details

**Good Example:**
> Given I'm on the Broker List screen, When I click "Add New Broker", Then I'm navigated to the Create Broker form

**Bad Example:**
> The system should handle broker creation properly ❌ (vague, not testable)

---

### ✅ Completeness

- [ ] **Happy Path Covered**: Primary success scenario is documented
- [ ] **Alternative Flows**: Valid alternative paths are covered
- [ ] **Edge Cases**: Boundary conditions and special cases are documented
- [ ] **Error Scenarios**: Invalid inputs and error states are specified
- [ ] **Validation Rules**: All business rules and validations are explicit

**Coverage Checklist:**
- [ ] What happens on success?
- [ ] What happens on failure?
- [ ] What happens when data is missing?
- [ ] What happens with invalid data?
- [ ] What happens when user lacks permission?
- [ ] What happens when system is unavailable?

---

### ✅ Testability

- [ ] **Measurable**: QA can write automated or manual tests directly from criteria
- [ ] **Pass/Fail Clear**: It's obvious when a criterion is met vs. not met
- [ ] **No Subjective Terms**: Avoid words like "fast", "user-friendly", "intuitive" without defining them
- [ ] **Setup Defined**: Preconditions (Given) are clear
- [ ] **Action Defined**: Trigger action (When) is clear
- [ ] **Outcome Defined**: Expected result (Then) is clear

**Good Example:**
> When I enter a duplicate license number "CA-12345" and click Save, Then I see error message "Broker with this license already exists"

**Bad Example:**
> The system should validate license numbers appropriately ❌ (not testable)

---

### ✅ Format & Structure

- [ ] **Consistent Format**: Uses Given/When/Then or checklist format consistently
- [ ] **One Behavior Per Criterion**: Each criterion tests one specific behavior
- [ ] **Logical Grouping**: Related criteria are grouped (happy path, errors, etc.)
- [ ] **Prioritized**: Critical criteria are listed before nice-to-have criteria

**Given/When/Then Format:**
```
Given [precondition/context]
When [action or event]
Then [expected outcome]
And [additional outcome]
```

**Checklist Format:**
```
- [ ] Criterion 1: Specific, testable condition
- [ ] Criterion 2: Specific, testable condition
```

---

## Domain-Specific Checks

### For CRUD Operations

- [ ] **Create**: Happy path, validation errors, duplicate handling, permission checks
- [ ] **Read**: Record found, record not found, permission checks, data format
- [ ] **Update**: Happy path, validation errors, concurrent updates, permission checks
- [ ] **Delete**: Soft vs hard delete, confirmation, cascade behavior, permission checks

### For Workflows & Status Transitions

- [ ] **Allowed Transitions**: All valid state transitions are documented
- [ ] **Blocked Transitions**: Invalid transitions return appropriate errors
- [ ] **Transition Triggers**: What causes each transition is clear
- [ ] **Side Effects**: Secondary actions (emails, notifications) are documented

### For Forms & Data Entry

- [ ] **Required Fields**: Missing required fields show inline errors
- [ ] **Optional Fields**: Optional fields can be left blank
- [ ] **Field Validation**: Format, length, pattern validations are specified
- [ ] **Field Dependencies**: Conditional fields and rules are documented

### For Lists & Tables

- [ ] **Empty State**: What's shown when no data exists
- [ ] **Sorting**: Default sort and available sort options
- [ ] **Filtering**: Available filters and filter behavior
- [ ] **Pagination**: Page size, navigation, total count display
- [ ] **Actions**: Row-level and bulk actions

### For Audit & Timeline Requirements

- [ ] **Timeline Event Created**: Event type, timestamp, user ID are logged
- [ ] **Event Visibility**: Where timeline events are visible
- [ ] **Event Immutability**: Events cannot be modified or deleted
- [ ] **Event Details**: What data is captured (old value → new value)

---

## Common Anti-Patterns (Avoid These)

### ❌ Vague Criteria
> "The system should work correctly"
> "Users should be able to manage brokers"

**Fix:** Be specific about what "work correctly" or "manage" means

---

### ❌ Implementation Details
> "When I click the button, the API calls POST /api/brokers with JSON payload"

**Fix:** Focus on user-observable behavior, not technical implementation

---

### ❌ Subjective Terms
> "The page should load quickly"
> "The UI should be user-friendly"

**Fix:** Define measurably: "Page loads in < 2 seconds" or provide specific UI requirements

---

### ❌ Missing Edge Cases
> "When I create a broker, it's saved successfully"

**Fix:** What if license is duplicate? What if required field is missing? What if user lacks permission?

---

### ❌ Multiple Behaviors in One Criterion
> "When I submit the form, the broker is created, an email is sent, and I'm redirected to the list"

**Fix:** Break into separate criteria:
- Broker is created in database
- Confirmation email is sent
- User is redirected to broker list

---

### ❌ No Error Scenarios
> Only documenting happy path

**Fix:** Add criteria for validation errors, permission errors, system errors

---

## Audit Trail Criteria Template

For all stories involving data mutations, include these criteria:

```
Audit & Timeline Requirements:
- [ ] When [action] is performed, a timeline event is created with:
  - Event type: "[ActionType]"
  - Timestamp: ISO 8601 format in UTC
  - User ID: ID of user who performed action
  - Entity ID: ID of entity being modified
  - Changes: [Old value] → [New value] (for updates)
- [ ] Timeline event is visible in [Entity] 360 view
- [ ] Timeline event is immutable (cannot be edited or deleted)
- [ ] Timeline event includes IP address (if security-relevant)
```

---

## Authorization Criteria Template

For all stories involving actions, include these criteria:

```
Authorization Requirements:
- [ ] Users with [Role1] permission can [action]
- [ ] Users without [Role1] permission see 403 Forbidden error
- [ ] Error message is: "You don't have permission to [action]"
- [ ] Users with [Role2] permission can [action] only on their own records
```

---

## Review Process

### Self-Review (PM)
Before handing off stories, Product Manager should:
1. Read each criterion out loud - does it make sense?
2. Try to think of scenarios where the criterion would fail
3. Ask: "Could a developer implement this in multiple different ways?"
4. If yes to #3, criteria need more specificity

### Peer Review (Another PM or BA)
- Fresh eyes catch ambiguity the author missed
- Reviewer should ask clarifying questions for anything unclear

### Stakeholder Review (Business Owner)
- Validates business rules and edge cases are correct
- Confirms priorities and scope

### Technical Feasibility Review (Architect)
- Confirms no impossible requirements (before implementation)
- Flags any criteria that need technical constraints
- Does NOT change functional requirements without PM approval

---

## Examples: Good vs. Bad

### Example 1: Create Broker

**❌ Bad Acceptance Criteria:**
> The user can create a new broker

**✅ Good Acceptance Criteria:**
```
Happy Path:
- Given I'm on the Broker List screen
- When I click "Add New Broker" button
- Then I'm navigated to the Create Broker form

- Given I'm on the Create Broker form
- When I fill in:
  - Name: "Acme Insurance"
  - License Number: "CA-12345"
  - State: "California"
- And I click "Save"
- Then the broker is created
- And I see success message "Broker created successfully"
- And I'm redirected to the Broker 360 view for the new broker
- And a timeline event is created: "Broker Created" with timestamp and my user ID

Validation Errors:
- When I submit the form with Name blank
- Then I see inline error "Name is required"
- And the broker is not created

- When I submit the form with duplicate License Number "CA-12345"
- Then I see error "Broker with this license already exists"
- And the broker is not created

Permission Errors:
- Given I don't have "CreateBroker" permission
- When I try to access the Create Broker form
- Then I see 403 Forbidden error
- And the error message is "You don't have permission to create brokers"
```

---

### Example 2: Workflow Transition

**❌ Bad Acceptance Criteria:**
> The submission moves to the next status

**✅ Good Acceptance Criteria:**
```
Allowed Transition:
- Given a submission is in "Triaging" status
- When I click "Ready for UW Review" button
- Then the submission status changes to "ReadyForUWReview"
- And a workflow transition event is logged with:
  - From Status: "Triaging"
  - To Status: "ReadyForUWReview"
  - Timestamp and user ID
- And the submission appears in the Underwriter's queue

Blocked Transition:
- Given a submission is in "Triaging" status
- When I try to transition directly to "Bound" (skipping intermediate steps)
- Then I see 409 Conflict error
- And the error message is "Cannot transition from Triaging to Bound. Next valid statuses are: WaitingOnBroker, ReadyForUWReview, Declined, Withdrawn"
- And the submission status remains "Triaging"
```

---

## Version History

**Version 1.0** - 2026-01-26 - Initial checklist created

---

## Related Resources

- `agents/templates/story-template.md` - Full user story template
- `product-manager/references/story-examples.md` - Complete story examples
- `quality-engineer/references/test-case-mapping.md` - How QA maps criteria to test cases

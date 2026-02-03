# Action: Feature

## User Intent

Build a single feature as a complete vertical slice (backend + frontend + tests) that can be deployed and tested independently. Ideal for incremental delivery.

## Agent Flow

```
(Backend Developer + Frontend Developer + Quality Engineer)
  ↓ [Parallel Implementation]
[SELF-REVIEW GATE: Each agent validates their work]
  ↓
Code Reviewer
  ↓
[APPROVAL GATE: User reviews and approves]
  ↓
Feature Complete
```

**Flow Type:** Mixed (parallel implementation, sequential review with approval gate)

---

## Execution Steps

### Step 1: Parallel Feature Implementation

**Instructions for Claude:**

Execute these agents **in parallel** for the specific feature:

#### 1a. Backend Developer (Feature Scope)
1. **Activate Backend Developer agent** by reading `agents/backend-developer/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` Section 4 (architecture for this feature)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - User stories for THIS FEATURE ONLY
3. **Execute responsibilities (feature-scoped):**
   - Implement domain entities for this feature
   - Create or update EF Core entities
   - Generate migration (if schema changes needed)
   - Implement API endpoints for this feature
   - Write application services for feature business logic
   - Create unit tests for feature domain logic
   - Write integration tests for feature API endpoints
4. **Follow SOLUTION-PATTERNS.md:**
   - Casbin ABAC for authorization
   - ActivityTimelineEvent for mutations
   - ProblemDetails for errors
   - Clean architecture layers
   - Audit fields, soft delete
5. **Outputs (feature-specific):**
   - Domain entities (created or updated)
   - EF Core migration (if needed)
   - API endpoints (controllers)
   - Application services
   - Unit tests
   - Integration tests

#### 1b. Frontend Developer (Feature Scope)
1. **Activate Frontend Developer agent** by reading `agents/frontend-developer/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` Section 3 (screens for this feature)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - API contracts for THIS FEATURE ONLY
3. **Execute responsibilities (feature-scoped):**
   - Create React components for feature screens
   - Implement forms for this feature (React Hook Form + Zod)
   - Set up TanStack Query hooks for feature API calls
   - Add routing for feature screens
   - Style with Tailwind + shadcn/ui
   - Write component tests
4. **Follow SOLUTION-PATTERNS.md:**
   - React Hook Form for forms
   - Zod for validation
   - TanStack Query for API
   - Tailwind + shadcn/ui for styling
5. **Outputs (feature-specific):**
   - React components (feature screens)
   - Form implementations
   - TanStack Query hooks
   - Routing updates
   - Component tests

#### 1c. Quality Engineer (Feature Scope)
1. **Activate Quality Engineer agent** by reading `agents/quality-engineer/SKILL.md`
2. **Read context:**
   - User stories for THIS FEATURE with acceptance criteria
   - Workflows for THIS FEATURE
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
3. **Execute responsibilities (feature-scoped):**
   - Create test plan for this feature
   - Write E2E test for feature happy path
   - Write E2E test for feature error scenarios
   - Validate feature acceptance criteria coverage
   - Generate coverage report for feature code
4. **Follow SOLUTION-PATTERNS.md:**
   - ≥80% unit coverage
   - Integration tests for all feature endpoints
   - E2E tests for feature workflows
5. **Outputs (feature-specific):**
   - Test plan for feature
   - E2E tests (happy path + errors)
   - Feature test coverage report

**Completion Criteria for Step 1:**
- [ ] All three agents completed feature implementation
- [ ] Feature code compiles/builds successfully
- [ ] No critical errors

---

### Step 2: SELF-REVIEW GATE (Agent Validation)

**Instructions for Claude:**

Each agent validates their feature work:

1. **Backend Developer self-review:**
   - [ ] Feature API endpoints implemented per contracts
   - [ ] Feature domain logic complete and tested
   - [ ] Unit tests passing for feature logic
   - [ ] Integration tests passing for feature endpoints
   - [ ] SOLUTION-PATTERNS.md followed
   - [ ] Feature acceptance criteria met
   - [ ] Migration applies successfully (if created)

2. **Frontend Developer self-review:**
   - [ ] Feature screens implemented per specs
   - [ ] Feature forms work with validation
   - [ ] API integration works for feature
   - [ ] Component tests passing
   - [ ] SOLUTION-PATTERNS.md followed
   - [ ] Feature acceptance criteria met

3. **Quality Engineer self-review:**
   - [ ] Feature test plan complete
   - [ ] E2E tests passing for feature
   - [ ] Coverage adequate for feature code
   - [ ] All feature acceptance criteria testable

**If any self-review fails:**
- Agent fixes issues
- Re-runs self-review
- Repeats until passing

**Gate Criteria:**
- [ ] All agents pass self-review for feature
- [ ] All feature tests passing
- [ ] Feature works end-to-end

---

### Step 3: Execute Code Reviewer

**Instructions for Claude:**

1. **Activate Code Reviewer agent** by reading `agents/code-reviewer/SKILL.md`

2. **Read context:**
   - Feature code produced in Step 1
   - `planning-mds/INCEPTION.md` (feature requirements)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - Feature user stories with acceptance criteria

3. **Execute code review (feature-focused):**
   - Review feature code structure
   - Check SOLID principles in feature code
   - Validate clean architecture boundaries
   - Review feature test coverage and quality
   - Identify code smells in feature
   - Validate feature acceptance criteria met
   - Check SOLUTION-PATTERNS.md compliance
   - Assess vertical slice completeness

4. **Produce feature code review report:**
   ```markdown
   # Feature Code Review Report

   Feature: [Feature Name]

   ## Summary
   - Assessment: [APPROVED / APPROVED WITH RECOMMENDATIONS / REJECTED]
   - Files reviewed: [count]
   - Issues found: [count by severity]

   ## Vertical Slice Completeness
   - [ ] Backend complete (API endpoints functional)
   - [ ] Frontend complete (screens functional)
   - [ ] Tests complete (unit, integration, E2E)
   - [ ] Can be deployed independently

   ## Findings
   - Critical: [list]
   - High: [list]
   - Medium: [list]
   - Low: [list]

   ## Pattern Compliance
   - [ ] Clean architecture respected
   - [ ] SOLID principles followed
   - [ ] SOLUTION-PATTERNS.md applied
   - [ ] Test coverage ≥80% for feature logic

   ## Acceptance Criteria
   - [ ] All feature ACs met
   - [ ] Edge cases handled
   - [ ] Error scenarios covered

   ## Recommendation
   [APPROVE / REQUEST CHANGES / REJECT]
   ```

**Code Review Outputs:**
- Feature code review report
- Approval or rejection

---

### Step 4: APPROVAL GATE (Feature Review)

**Instructions for Claude:**

1. **Present feature review results to user:**
   ```
   ═══════════════════════════════════════════════════════════
   Feature Code Review Complete
   ═══════════════════════════════════════════════════════════

   Feature: [Feature Name]
   Reviewer: Code Reviewer Agent
   Status: [APPROVED / APPROVED WITH RECOMMENDATIONS / REJECTED]

   ✓ Vertical Slice Completeness
     - Backend: [Complete/Incomplete]
     - Frontend: [Complete/Incomplete]
     - Tests: [Complete/Incomplete]
     - Deployable: [Yes/No]

   Issues Found:
     - Critical: [count]
     - High: [count]
     - Medium: [count]
     - Low: [count]

   ✓ Pattern Compliance
     - Clean Architecture: [Yes/No]
     - SOLID Principles: [Yes/No]
     - SOLUTION-PATTERNS.md: [Yes/No]
     - Test Coverage: [percentage]% (feature code)

   ✓ Acceptance Criteria
     - [count]/[total] feature ACs met
     - Edge cases: [Handled/Needs work]
     - Errors: [Covered/Needs work]

   ═══════════════════════════════════════════════════════════
   Review Details:
   [Link to feature code review report]
   ═══════════════════════════════════════════════════════════
   ```

2. **Present approval checklist:**
   ```
   Feature Approval Checklist:
   - [ ] Feature is a complete vertical slice
   - [ ] Backend implementation complete
   - [ ] Frontend implementation complete
   - [ ] Tests cover feature completely
   - [ ] No critical issues
   - [ ] SOLUTION-PATTERNS.md followed
   - [ ] All feature acceptance criteria met
   - [ ] Feature can be deployed independently
   ```

3. **Ask user for approval:**
   ```
   Do you approve this feature?

   Options:
   - "approve" - Feature approved, ready to merge
   - "fix issues" - Fix identified issues, then re-review
   - "reject" - Significant rework needed
   ```

4. **Handle user response:**
   - **If "approve":**
     - Proceed to Step 5 (Feature Complete)

   - **If "fix issues":**
     - Identify issues to fix
     - Agents fix issues
     - Return to Step 3 (re-run code review)

   - **If "reject":**
     - Capture feedback
     - Return to Step 1 (re-implement feature)

**Gate Criteria:**
- [ ] Code review passed
- [ ] No critical issues
- [ ] Feature is complete vertical slice
- [ ] User explicitly approves

---

### Step 5: Feature Complete

**Instructions for Claude:**

Present completion summary:

```
═══════════════════════════════════════════════════════════
Feature Complete! ✓
═══════════════════════════════════════════════════════════

Feature: [Feature Name]

Implementation:
  ✓ Backend Developer
    - [count] entities created/updated
    - [count] API endpoints implemented
    - [count] unit tests passing
    - [count] integration tests passing

  ✓ Frontend Developer
    - [count] components created
    - [count] screens implemented
    - [count] forms with validation
    - Component tests passing

  ✓ Quality Engineer
    - Test plan complete
    - [count] E2E tests passing
    - [percentage]% coverage for feature code

Code Review:
  ✓ Code Reviewer: APPROVED
  ✓ Vertical slice complete
  ✓ Acceptance criteria met
  Status: APPROVED

═══════════════════════════════════════════════════════════
Next Steps:
═══════════════════════════════════════════════════════════

Feature is ready to:
1. Merge to main branch
2. Deploy to staging
3. Get stakeholder feedback

To continue building:
- Run "feature" action for next feature
- Run "build" action for remaining features
- Run "document" action to update docs

Feature delivered! ✓
═══════════════════════════════════════════════════════════
```

---

## Validation Criteria

**Overall Feature Action Success:**
- [ ] Feature is complete vertical slice (backend + frontend + tests)
- [ ] All feature tests passing
- [ ] Code review approved
- [ ] All feature acceptance criteria met
- [ ] Feature can be deployed independently
- [ ] User approved

---

## Prerequisites

Before running feature action:
- [ ] Plan action completed for this feature
- [ ] Feature has clear user stories with acceptance criteria
- [ ] Feature scope is small (2-5 days of work)
- [ ] SOLUTION-PATTERNS.md exists
- [ ] User is available for approval

---

## Vertical Slicing Best Practices

### What Makes a Good Vertical Slice?

1. **Complete:** Includes backend, frontend, and tests
2. **Deployable:** Can be released independently
3. **Testable:** Has clear acceptance criteria
4. **Small:** Can be completed in 2-5 days
5. **Valuable:** Delivers user value on its own

### Good Feature Examples
- ✅ "View customer list with pagination"
- ✅ "Create new order with required fields"
- ✅ "Search accounts by name"
- ✅ "View activity timeline for an account"

### Bad Feature Examples (Too Large - Break Down!)
- ❌ "Build entire customer management module"
- ❌ "Implement full order processing workflow"
- ❌ "Complete onboarding process"

### Not Vertical Slices
- ❌ "Create all database tables" (not vertical)
- ❌ "Build all backend APIs" (not vertical)
- ❌ "Create all forms" (not vertical)

---

## Example Usage

### Scenario 1: Customer List Feature
```
User: "Run the feature action for customer list with pagination"

Claude: [Reads feature.md]
Claude: "I'll implement the customer list feature as a vertical slice..."

[Step 1: Parallel implementation]
  Backend Developer:
    - GET /api/customers endpoint with pagination
    - CustomerService.ListAsync method
    - Unit tests for service
    - Integration tests for endpoint

  Frontend Developer:
    - CustomerList component
    - TanStack Query hook for fetching customers
    - Pagination controls
    - Component tests

  Quality Engineer:
    - E2E test: View customer list
    - E2E test: Navigate pages
    - Test plan for customer list

[Step 2: Self-review - all pass]

[Step 3: Code review]
Claude: "Code review complete. Results:
  - Vertical slice: Complete
  - Issues: 2 medium (naming conventions)
  - Test coverage: 85%
  - Acceptance criteria: All met
  - Recommendation: APPROVED WITH RECOMMENDATIONS"

[Step 4: Approval gate]
Claude: "Feature review complete. Do you approve? (approve/fix issues/reject)"

User: "approve"

Claude: "Feature complete! Customer list with pagination ready to merge."
```

### Scenario 2: Iteration After Rejection
```
User: "Run the feature action for customer search"

[Steps 1-3 execute]

[Step 4: Approval gate]
Claude: "Feature review results:
  - Issues: 1 critical (missing authorization check)
  - Recommendation: FIX ISSUES"

User: "fix issues"

Claude: "Fixing authorization check..."
[Backend Developer adds authorization]

[Step 3: Re-review]
Claude: "Code review updated. Results:
  - Authorization: Fixed
  - Recommendation: APPROVED"

[Step 4: Re-approval]
Claude: "Do you approve now?"

User: "approve"

Claude: "Feature complete!"
```

---

## Related Actions

- **Before:** [plan action](./plan.md) - Plan the feature
- **Alternative:** [build action](./build.md) - Build multiple features together
- **After:** [document action](./document.md) - Document the feature
- **After:** [blog action](./blog.md) - Blog about the feature

---

## Notes

- Feature action is the **recommended way** to build incrementally
- Each feature should be merged to main after approval
- Features can build on each other (dependencies allowed)
- Prefer small, frequent features over large batches
- Feature action ensures true vertical slicing discipline
- Security review can be added if feature involves sensitive operations
- DevOps agent not included (assumes Docker setup already exists)

# Action: Build

## User Intent

Implement planned features by generating production-quality code, tests, and deployment configurations with comprehensive review and approval gates.

## Agent Flow

```
Architect (Implementation Orchestration)
  ↓
(Backend Developer + Frontend Developer + AI Engineer [if AI scope] + Quality Engineer + DevOps)
  ↓ [Parallel Implementation]
[SELF-REVIEW GATE: Each agent validates their work]
  ↓
Code Reviewer
  ↓
[APPROVAL GATE: User reviews code quality]
  ↓
Security
  ↓
[APPROVAL GATE: User reviews security findings]
  ↓
Build Complete
```

**Flow Type:** Mixed (architect-led orchestration kickoff, parallel implementation, sequential reviews with approval gates)

---

## Execution Steps

### Step 0: Architect-Led Assembly Planning

**Execution Instructions:**

1. **Activate Architect agent** by reading `agents/architect/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md`
   - `planning-mds/stories/`
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/api/`
3. **Produce assembly plan:**
   - Build order for vertical slices
   - Explicit backend/frontend/AI handoffs
   - Integration checkpoints for QA and DevOps
   - Clear definition of "done" for each slice
4. **Output artifact:**
   - `planning-mds/architecture/application-assembly-plan.md` (use `agents/templates/application-assembly-plan-template.md`)

**Completion Criteria for Step 0:**
- [ ] Application assembly plan exists
- [ ] Scope split across implementation agents is explicit
- [ ] Integration checkpoints defined

---

### Step 0.5: Assembly Plan Validation

**Execution Instructions:**

Validate the assembly plan before parallel implementation begins:

- [ ] Scope split matches story requirements
- [ ] Dependencies between agents are identified
- [ ] Integration checkpoints are feasible
- [ ] No missing or conflicting artifact ownership

Validator:
- Code Reviewer or a second Architect review (lightweight checklist is sufficient)

---

### Step 1: Parallel Implementation

**Execution Instructions:**

Execute these agents **in parallel** (all working simultaneously). Run AI Engineer when stories include AI/LLM/MCP scope:

**AI Scope Checklist — include AI Engineer if ANY apply:**
- [ ] Story mentions LLM, AI, or machine learning behavior
- [ ] Story requires MCP server/tool/resource work
- [ ] Story involves prompts, agent behavior, or tool orchestration
- [ ] Story changes files under `neuron/`
- [ ] Story requires model selection, cost controls, or guardrails

#### 1a. Backend Developer
1. **Activate Backend Developer agent** by reading `agents/backend-developer/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` Section 4 (architecture, data model, API contracts)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/stories/` (user stories to implement)
3. **Execute responsibilities:**
   - Implement domain entities matching data model
   - Create EF Core DbContext and entities
   - Generate and apply database migrations
   - Implement API endpoints per contracts
   - Write application services and business logic
   - Create repository implementations
   - Write unit tests (≥80% coverage for business logic)
   - Write integration tests (all API endpoints)
4. **Follow SOLUTION-PATTERNS.md:**
   - Use Casbin ABAC for authorization
   - Create ActivityTimelineEvent for all mutations
   - Use ProblemDetails for errors
   - Follow clean architecture layers
   - Include audit fields on all entities
   - Implement soft delete
5. **Outputs:**
   - C# domain models and entities
   - EF Core migrations
   - ASP.NET Core controllers/endpoints
   - Application services
   - Repository implementations
   - Unit tests
   - Integration tests

#### 1b. Frontend Developer
1. **Activate Frontend Developer agent** by reading `agents/frontend-developer/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` Section 3 (screens, user stories)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/api/` (OpenAPI contracts to implement against)
3. **Execute responsibilities:**
   - Create React components for screens
   - Implement forms with React Hook Form + AJV (JSON Schema) validation
   - Set up TanStack Query for API calls
   - Implement routing and navigation
   - Style with Tailwind + shadcn/ui components
   - Write component tests
4. **Follow SOLUTION-PATTERNS.md:**
   - React Hook Form for all forms
   - AJV + JSON Schema for validation
   - TanStack Query for API calls
   - Tailwind + shadcn/ui for styling
5. **Outputs:**
   - React components (pages, layouts, features)
   - TypeScript types and interfaces
   - TanStack Query hooks
   - Form implementations
   - Routing configuration
   - Component tests

#### 1c. Quality Engineer
1. **Activate Quality Engineer agent** by reading `agents/quality-engineer/SKILL.md`
2. **Read context:**
   - `planning-mds/stories/` (acceptance criteria)
   - `planning-mds/INCEPTION.md` Section 4.4 (workflows)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
3. **Execute responsibilities:**
   - Create test plan mapping stories to test cases
   - Write unit tests for domain logic (support Backend Developer)
   - Write integration tests for API endpoints
   - Write E2E tests for critical workflows
   - Generate coverage reports
   - Validate quality gates
4. **Follow SOLUTION-PATTERNS.md:**
   - ≥80% unit coverage for business logic
   - Integration tests for all endpoints
   - E2E tests for critical workflows
5. **Outputs:**
   - Test plan document
   - Unit test suites
   - Integration test suites
   - E2E test scenarios (Playwright)
   - Coverage reports

#### 1d. DevOps
1. **Activate DevOps agent** by reading `agents/devops/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` Section 2 (tech stack)
   - Existing Docker setup (if any)
3. **Execute responsibilities:**
   - Create/update Dockerfile for backend
   - Create/update Dockerfile for frontend (if needed)
   - Update docker-compose.yml for local dev
   - Configure environment variables
   - Create deployment scripts
   - Document run instructions
4. **Follow SOLUTION-PATTERNS.md:**
   - Docker for all services
   - docker-compose for local dev
   - Environment variables for configuration
5. **Outputs:**
   - Dockerfile (backend)
   - Dockerfile (frontend, if needed)
   - docker-compose.yml
   - .env.example
   - Deployment scripts
   - README updates (run instructions)

#### 1e. AI Engineer (if AI scope)
1. **Activate AI Engineer agent** by reading `agents/ai-engineer/SKILL.md`
2. **Read context:**
   - `planning-mds/INCEPTION.md` (AI-related stories and constraints)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/stories/` (AI feature acceptance criteria)
   - Existing `neuron/` code (if present)
3. **Execute responsibilities:**
   - Implement AI features in `neuron/` (model integration, prompts, workflows, tools)
   - Implement MCP servers/resources when required by stories
   - Add guardrails (input/output validation, retries/timeouts, error handling)
   - Add observability (decision/tool-call logs and usage tracking)
   - Write unit/integration tests for AI components
4. **Follow SOLUTION-PATTERNS.md:**
   - Keep configuration in environment/config files (no hardcoded secrets)
   - Align AI behavior and outputs with story acceptance criteria
   - Keep interfaces explicit for backend/frontend integration
5. **Outputs:**
   - `neuron/` implementation updates
   - AI configs and prompt templates
   - MCP server/tool definitions (if needed)
   - AI unit/integration tests
   - `neuron/README.md` updates (if behavior or setup changed)

**Completion Criteria for Step 1:**
- [ ] All required agents have completed their work (Backend, Frontend, Quality, DevOps, and AI Engineer if AI scope)
- [ ] Code compiles/builds successfully
- [ ] No critical errors or blockers

---

### Step 2: SELF-REVIEW GATE (Agent Validation)

**Execution Instructions:**

Each agent validates their own work before proceeding to code review:

1. **Backend Developer self-review:**
   - [ ] All API endpoints implemented per contracts
   - [ ] Domain logic complete and tested
   - [ ] Unit test coverage ≥80% for business logic
   - [ ] Integration tests passing for all endpoints
   - [ ] SOLUTION-PATTERNS.md followed (ABAC, audit, timeline events)
   - [ ] All acceptance criteria met
   - [ ] Code compiles and runs

2. **Frontend Developer self-review:**
   - [ ] All screens implemented per specs
   - [ ] Forms work with validation
   - [ ] API integration successful
   - [ ] Component tests passing
   - [ ] SOLUTION-PATTERNS.md followed (React Hook Form, AJV, TanStack Query)
   - [ ] All acceptance criteria met
   - [ ] No console errors

3. **Quality Engineer self-review:**
   - [ ] Test plan complete
   - [ ] All tests passing
   - [ ] Coverage reports generated
   - [ ] Critical workflows have E2E tests
   - [ ] Quality gates met

4. **DevOps self-review:**
   - [ ] Docker containers build successfully
   - [ ] docker-compose up works
   - [ ] All services start correctly
   - [ ] Environment variables documented

5. **AI Engineer self-review (if AI scope):**
   - [ ] AI workflows/prompts implemented per stories
   - [ ] MCP resources/tools functional (if required)
   - [ ] AI unit/integration tests passing
   - [ ] Cost/safety/observability controls implemented
   - [ ] No hardcoded API keys or secrets

**If any self-review fails:**
- Agent fixes issues
- Re-runs self-review
- Repeats until passing

**Gate Criteria:**
- [ ] Architect confirms assembled output matches Step 0 plan
- [ ] All required agents pass self-review
- [ ] All tests passing
- [ ] Application runs successfully

---

### Step 3: Execute Code Reviewer

**Execution Instructions:**

1. **Activate Code Reviewer agent** by reading `agents/code-reviewer/SKILL.md`

2. **Read context:**
   - All code produced in Step 1
   - `planning-mds/INCEPTION.md` (requirements and architecture)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/stories/` (acceptance criteria)

3. **Execute code review responsibilities:**
   - Review code structure and organization
   - Check SOLID principles adherence
   - Validate clean architecture boundaries
   - Review test coverage and quality
   - Identify code smells and anti-patterns
   - Check naming conventions and readability
   - Validate acceptance criteria mapping
   - Review error handling patterns
   - Check for over-engineering or under-engineering
   - Validate SOLUTION-PATTERNS.md compliance

4. **Produce code review report:**
   ```markdown
   # Code Review Report

   ## Summary
   - Overall assessment: [APPROVED / APPROVED WITH RECOMMENDATIONS / REJECTED]
   - Files reviewed: [count]
   - Issues found: [count by severity]

   ## Findings

   ### Critical Issues (must fix before approval)
   - [List critical issues]

   ### High Priority (should fix)
   - [List high priority issues]

   ### Medium Priority (nice to have)
   - [List medium priority issues]

   ### Low Priority (optional)
   - [List low priority suggestions]

   ## Pattern Compliance
   - [ ] Clean architecture layers respected
   - [ ] SOLID principles followed
   - [ ] SOLUTION-PATTERNS.md patterns applied
   - [ ] Test coverage adequate (≥80%)

   ## Acceptance Criteria
   - [ ] All user story ACs met
   - [ ] Edge cases handled
   - [ ] Error scenarios covered

   ## Recommendation
   [APPROVE / REQUEST CHANGES / REJECT]
   ```

**Code Review Outputs:**
- Code review report
- List of findings with severity
- Approval or rejection with rationale

---

### Step 4: APPROVAL GATE (Code Review)

**Execution Instructions:**

1. **Present code review results to user:**
   ```
   ═══════════════════════════════════════════════════════════
   Code Review Complete
   ═══════════════════════════════════════════════════════════

   Reviewer: Code Reviewer Agent
   Status: [APPROVED / APPROVED WITH RECOMMENDATIONS / REJECTED]

   Files Reviewed: [count]
   Issues Found:
     - Critical: [count]
     - High: [count]
     - Medium: [count]
     - Low: [count]

   ✓ Pattern Compliance
     - Clean Architecture: [Yes/No]
     - SOLID Principles: [Yes/No]
     - SOLUTION-PATTERNS.md: [Yes/No]
     - Test Coverage: [percentage]%

   ✓ Acceptance Criteria
     - [count]/[total] user stories fully met
     - Edge cases: [Handled/Needs work]
     - Error scenarios: [Covered/Needs work]

   ═══════════════════════════════════════════════════════════
   Review Details:
   [Link to code review report]
   ═══════════════════════════════════════════════════════════
   ```

2. **Present approval checklist:**
   ```
   Code Review Approval Checklist:
   - [ ] No critical issues found (or all fixed)
   - [ ] Code follows SOLID principles
   - [ ] Clean architecture boundaries respected
   - [ ] Test coverage ≥80% for business logic
   - [ ] SOLUTION-PATTERNS.md patterns followed
   - [ ] Acceptance criteria met
   - [ ] Code is maintainable and readable
   ```

3. **Ask user for approval:**
   ```
   Do you approve the code quality?

   Options:
   - "approve" - Code quality acceptable, proceed to security review
   - "fix critical" - Fix critical issues, then re-review
   - "reject" - Provide feedback and significant rework needed
   ```

4. **Handle user response:**
   - **If "approve":**
     - Proceed to Step 5 (Security Review)

   - **If "fix critical":**
     - Identify critical issues
     - Agents fix issues
     - Return to Step 3 (re-run code review)

   - **If "reject":**
     - Capture feedback
     - Return to Step 0 (re-plan and re-implement with feedback)

**Gate Criteria:**
- [ ] Code review passed or approved with minor recommendations
- [ ] No critical issues
- [ ] User explicitly approves

---

### Step 5: Execute Security Review

**Execution Instructions:**

1. **Activate Security agent** by reading `agents/security/SKILL.md`

2. **Read context:**
   - All code produced in Step 1
   - `planning-mds/INCEPTION.md` Section 4.5 (authorization model)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/security/` (threat model, if exists)

3. **Execute security review responsibilities:**
   - Scan for OWASP Top 10 vulnerabilities:
     1. Injection (SQL, command, XSS)
     2. Broken authentication
     3. Sensitive data exposure
     4. XML external entities
     5. Broken access control
     6. Security misconfiguration
     7. Cross-site scripting (XSS)
     8. Insecure deserialization
     9. Components with vulnerabilities
     10. Insufficient logging
   - Review authorization implementation (Casbin ABAC)
   - Check input validation and sanitization
   - Review secrets management (no hardcoded secrets)
   - Validate audit logging completeness
   - Review error messages (no info leakage)
   - Check HTTPS/TLS configuration
   - Validate CORS policies

4. **Produce security review report:**
   ```markdown
   # Security Review Report

   ## Summary
   - Overall assessment: [PASS / PASS WITH RECOMMENDATIONS / FAIL]
   - Vulnerabilities found: [count by severity]

   ## OWASP Top 10 Assessment
   - [X] Injection: [PASS / FAIL]
   - [X] Broken Authentication: [PASS / FAIL]
   - [X] Sensitive Data Exposure: [PASS / FAIL]
   - [X] XML External Entities: [PASS / FAIL]
   - [X] Broken Access Control: [PASS / FAIL]
   - [X] Security Misconfiguration: [PASS / FAIL]
   - [X] XSS: [PASS / FAIL]
   - [X] Insecure Deserialization: [PASS / FAIL]
   - [X] Vulnerable Components: [PASS / FAIL]
   - [X] Insufficient Logging: [PASS / FAIL]

   ## Findings

   ### Critical (must fix immediately)
   - [List critical security issues]

   ### High (fix before production)
   - [List high severity issues]

   ### Medium (should fix)
   - [List medium severity issues]

   ### Low (best practices)
   - [List low severity recommendations]

   ## Authorization Review
   - [ ] ABAC implementation correct
   - [ ] All endpoints protected
   - [ ] Per-endpoint authorization enforced

   ## Recommendation
   [APPROVE / FIX CRITICAL / FIX HIGH / REJECT]
   ```

**Security Review Outputs:**
- Security review report
- OWASP Top 10 assessment
- Vulnerability findings with severity and remediation
- Approval or rejection

---

### Step 6: APPROVAL GATE (Security Review)

**Execution Instructions:**

1. **Present security review results to user:**
   ```
   ═══════════════════════════════════════════════════════════
   Security Review Complete
   ═══════════════════════════════════════════════════════════

   Reviewer: Security Agent
   Status: [PASS / PASS WITH RECOMMENDATIONS / FAIL]

   Vulnerabilities Found:
     - Critical: [count]
     - High: [count]
     - Medium: [count]
     - Low: [count]

   ✓ OWASP Top 10 Assessment
     - Injection: [PASS/FAIL]
     - Broken Authentication: [PASS/FAIL]
     - Sensitive Data Exposure: [PASS/FAIL]
     - Broken Access Control: [PASS/FAIL]
     - [... remaining items]

   ✓ Authorization
     - ABAC implementation: [Correct/Needs work]
     - Endpoint protection: [Complete/Incomplete]
     - Server-side enforcement: [Yes/No]

   ✓ Audit Logging
     - All mutations logged: [Yes/No]
     - Timeline events: [Yes/No]

   ═══════════════════════════════════════════════════════════
   Review Details:
   [Link to security review report]
   ═══════════════════════════════════════════════════════════
   ```

2. **Present approval checklist:**
   ```
   Security Approval Checklist:
   - [ ] No critical vulnerabilities
   - [ ] No high-severity issues (or acceptable with mitigation plan)
   - [ ] OWASP Top 10 compliance
   - [ ] Authorization correctly implemented
   - [ ] No hardcoded secrets
   - [ ] Audit logging complete
   - [ ] Input validation comprehensive
   ```

3. **Ask user for approval:**
   ```
   Do you approve the security posture?

   Options:
   - "approve" - Security acceptable, build complete
   - "fix critical" - Fix critical/high issues, then re-review
   - "reject" - Significant security concerns, major rework needed
   ```

4. **Handle user response:**
   - **If "approve":**
     - Proceed to Step 7 (Build Complete)

   - **If "fix critical":**
     - Identify critical/high issues
     - Agents fix security issues
     - Return to Step 5 (re-run security review)

   - **If "reject":**
     - Capture feedback
     - Return to Step 0 (re-plan and re-implement with security focus)

**Gate Criteria:**
- [ ] Security review passed or approved with low-severity findings only
- [ ] No critical or high-severity vulnerabilities
- [ ] User explicitly approves

---

### Step 6.5: Performance Validation (Informational)

**Execution Instructions:**

If `planning-mds/INCEPTION.md` Section 4 defines measurable NFR targets, validate implementation against them and record results:

- [ ] API latency target (for example p95)
- [ ] Throughput/concurrency target
- [ ] Frontend performance targets (for example Core Web Vitals) when applicable

This is an informational gate for release readiness reporting and does not block progression by itself.

---

### Step 7: Build Complete

**Execution Instructions:**

Present completion summary:

```
═══════════════════════════════════════════════════════════
Build Action Complete! ✓
═══════════════════════════════════════════════════════════

Application Assembly:
  ✓ Architect
    - Assembly plan created
    - Cross-agent handoffs validated
    - Integration checkpoints tracked

Implementation (Parallel):
  ✓ Backend Developer
    - [count] entities implemented
    - [count] API endpoints created
    - [percentage]% unit test coverage
    - [count] integration tests passing

  ✓ Frontend Developer
    - [count] components created
    - [count] screens implemented
    - [count] forms with validation
    - Component tests passing

  ✓ Quality Engineer
    - Test plan complete
    - [count] E2E tests created
    - All tests passing

  ✓ DevOps
    - Docker containers ready
    - docker-compose configured
    - Deployment scripts created

  ✓ AI Engineer (if AI scope)
    - [count] AI workflows/prompts implemented
    - [count] MCP tools/resources added
    - [count] AI tests passing

Code Review:
  ✓ Code Reviewer: APPROVED
  ✓ Pattern compliance verified
  ✓ Test coverage adequate
  Status: APPROVED

Security Review:
  ✓ Security Agent: APPROVED
  ✓ OWASP Top 10: PASS
  ✓ Authorization: Correct
  Status: APPROVED

═══════════════════════════════════════════════════════════
Next Steps:
═══════════════════════════════════════════════════════════

1. Run application: docker-compose up
2. Test features manually
3. Run "document" action to generate docs
4. Deploy to staging environment

All features implemented and approved! ✓
═══════════════════════════════════════════════════════════
```

---

## Validation Criteria

**Overall Build Action Success:**
- [ ] Application assembly plan created and followed
- [ ] All required implementation agents completed work (including AI Engineer when AI scope exists)
- [ ] All tests passing (unit, integration, E2E)
- [ ] AI tests passing (if AI scope)
- [ ] Code review approved
- [ ] Security review approved
- [ ] Application runs successfully
- [ ] All acceptance criteria met

---

## Prerequisites

Before running build action:
- [ ] Plan action completed (requirements + architecture defined)
- [ ] SOLUTION-PATTERNS.md exists and is up-to-date
- [ ] User stories have clear acceptance criteria
- [ ] User is available for approval gates

---

## Related Actions

- **Before:** [plan action](./plan.md) - Define what to build
- **Alternative:** [feature action](./feature.md) - Build single feature incrementally
- **After:** [document action](./document.md) - Generate documentation
- **Quality:** [test action](./test.md) - Additional testing focus
- **Quality:** [review action](./review.md) - Additional review focus

---

## Notes

- Build action implements ALL features in scope (use feature action for incremental)
- Implementation agents work in parallel for efficiency
- Reviews are sequential to ensure quality gates
- Critical/high security issues must be fixed before approval
- Can re-run steps if approval gates fail
- All patterns in SOLUTION-PATTERNS.md must be followed

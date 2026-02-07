# Action: Review

## User Intent

Perform comprehensive code quality and security review on implemented features or the entire codebase with approval gates.

## Agent Flow

```
(Code Reviewer + Security)
  ↓ [Parallel Reviews]
[APPROVAL GATE: User reviews findings and decides next steps]
  ↓
Review Complete
```

**Flow Type:** Parallel reviews with single approval gate

---

## Execution Steps

### Step 1: Parallel Reviews

**Execution Instructions:**

Execute these review agents **in parallel**:

#### 1a. Code Reviewer
1. **Activate Code Reviewer agent** by reading `agents/code-reviewer/SKILL.md`

2. **Read context:**
   - Source code (backend, frontend, and `neuron/` when AI scope exists)
   - Test suites
   - `planning-mds/INCEPTION.md` (requirements and architecture)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - User stories with acceptance criteria

3. **Execute code review:**
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
   # Code Quality Review Report

   Scope: [Specific feature / PR / Full codebase]
   Date: [Date]

   ## Summary
   - Assessment: [APPROVED / APPROVED WITH RECOMMENDATIONS / CONDITIONAL / REJECTED]
   - Files reviewed: [count]
   - Total issues: [count]

   ## Findings by Severity

   ### Critical Issues (must fix before approval)
   1. [Issue description]
      - Location: [file:line]
      - Impact: [explanation]
      - Recommendation: [how to fix]

   ### High Priority (should fix)
   [Similar format]

   ### Medium Priority (nice to have)
   [Similar format]

   ### Low Priority (optional improvements)
   [Similar format]

   ## Pattern Compliance
   - [ ] Clean architecture layers respected
   - [ ] SOLID principles followed
   - [ ] SOLUTION-PATTERNS.md patterns applied
   - [ ] Naming conventions consistent
   - [ ] Error handling appropriate

   ## Test Quality
   - Unit test coverage: [percentage]%
   - Integration test coverage: [assessment]
   - E2E test coverage: [assessment]
   - Test quality: [Good / Needs improvement]

   ## Acceptance Criteria
   - [ ] All user story ACs met
   - [ ] Edge cases handled
   - [ ] Error scenarios covered

   ## Code Metrics
   - Cyclomatic complexity: [Average: X, Max: Y]
   - Lines of code: [count]
   - Technical debt estimate: [hours/days]

   ## Recommendation
   [APPROVE / APPROVE WITH MINOR CHANGES / FIX CRITICAL FIRST / REJECT]

   ## Action Items
   1. [Priority action item]
   2. [Priority action item]
   ```

5. **Outputs:**
   - Code quality review report
   - List of findings with severity
   - Metrics and recommendations

#### 1b. Security Reviewer
1. **Activate Security agent** by reading `agents/security/SKILL.md`

2. **Read context:**
   - Source code (backend and frontend)
   - `planning-mds/INCEPTION.md` Section 4.5 (authorization model)
   - `planning-mds/architecture/SOLUTION-PATTERNS.md`
   - `planning-mds/security/` (threat model, if exists)

3. **Execute security review:**
   - **OWASP Top 10 scan:**
     1. Injection (SQL, command, XSS)
     2. Broken authentication
     3. Sensitive data exposure
     4. XML external entities (XXE)
     5. Broken access control
     6. Security misconfiguration
     7. Cross-site scripting (XSS)
     8. Insecure deserialization
     9. Components with known vulnerabilities
     10. Insufficient logging and monitoring
   - Review authorization implementation (Casbin ABAC)
   - Check input validation and sanitization
   - Review secrets management (no hardcoded secrets)
   - Validate audit logging completeness
   - Review error messages (no information leakage)
   - Check HTTPS/TLS configuration
   - Validate CORS policies
   - Review dependency vulnerabilities

4. **Produce security review report:**
   ```markdown
   # Security Review Report

   Scope: [Specific feature / Full codebase]
   Date: [Date]

   ## Summary
   - Assessment: [PASS / PASS WITH RECOMMENDATIONS / CONDITIONAL PASS / FAIL]
   - Vulnerabilities found: [count]
   - Risk level: [Low / Medium / High / Critical]

   ## OWASP Top 10 Assessment

   ### 1. Injection
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 2. Broken Authentication
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 3. Sensitive Data Exposure
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 4. XML External Entities (XXE)
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 5. Broken Access Control
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 6. Security Misconfiguration
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 7. Cross-Site Scripting (XSS)
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 8. Insecure Deserialization
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 9. Using Components with Known Vulnerabilities
   - Status: [PASS / FAIL]
   - Findings: [details]

   ### 10. Insufficient Logging & Monitoring
   - Status: [PASS / FAIL]
   - Findings: [details]

   ## Vulnerability Findings

   ### Critical (fix immediately - actively exploitable)
   1. [Vulnerability description]
      - CVE/CWE: [reference]
      - Location: [file:line]
      - Exploit scenario: [how it could be exploited]
      - Remediation: [how to fix]

   ### High (fix before production)
   [Similar format]

   ### Medium (should fix)
   [Similar format]

   ### Low (best practice recommendations)
   [Similar format]

   ## Authorization Review
   - [ ] ABAC implementation correct
   - [ ] All endpoints protected
   - [ ] Per-endpoint authorization enforced
   - [ ] Server-side enforcement only
   - [ ] No client-side trust

   ## Audit & Compliance
   - [ ] All mutations create timeline events
   - [ ] Workflow transitions logged
   - [ ] User actions auditable
   - [ ] Sensitive data protected

   ## Secrets Management
   - [ ] No hardcoded secrets
   - [ ] Environment variables used
   - [ ] Secrets not in version control

   ## Recommendation
   [APPROVE / FIX CRITICAL / FIX HIGH / REJECT]

   ## Remediation Plan
   1. [Priority action]
   2. [Priority action]
   ```

5. **Outputs:**
   - Security review report
   - OWASP Top 10 assessment
   - Vulnerability findings with remediation
   - Compliance checklist
   - Save report under `planning-mds/security/reviews/` (for example: `security-review-YYYY-MM-DD.md`)

**Completion Criteria for Step 1:**
- [ ] Both reviews completed
- [ ] Reports generated

---

### Step 2: APPROVAL GATE (Review Results)

**Execution Instructions:**

1. **Present combined review results to user:**
   ```
   ═══════════════════════════════════════════════════════════
   Comprehensive Review Complete
   ═══════════════════════════════════════════════════════════

   CODE QUALITY REVIEW
   ─────────────────────────────────────────────────────────
   Reviewer: Code Reviewer Agent
   Status: [APPROVED / APPROVED WITH RECOMMENDATIONS / CONDITIONAL / REJECTED]

   Issues Found:
     - Critical: [count]
     - High: [count]
     - Medium: [count]
     - Low: [count]

   ✓ Pattern Compliance
     - Clean Architecture: [Yes/No]
     - SOLID Principles: [Yes/No]
     - SOLUTION-PATTERNS.md: [Yes/No]

   ✓ Test Coverage
     - Unit: [percentage]%
     - Integration: [assessment]
     - E2E: [assessment]

   ✓ Acceptance Criteria
     - [count]/[total] met
     - Edge cases: [Handled/Needs work]

   SECURITY REVIEW
   ─────────────────────────────────────────────────────────
   Reviewer: Security Agent
   Status: [PASS / PASS WITH RECOMMENDATIONS / CONDITIONAL / FAIL]

   Vulnerabilities Found:
     - Critical: [count]
     - High: [count]
     - Medium: [count]
     - Low: [count]

   ✓ OWASP Top 10
     - [count]/10 checks passed
     - Failed checks: [list]

   ✓ Authorization
     - ABAC implementation: [Correct/Issues found]
     - Endpoint protection: [Complete/Incomplete]

   ✓ Audit & Compliance
     - Timeline events: [Yes/No]
     - Secrets management: [Secure/Issues found]

   ═══════════════════════════════════════════════════════════
   Detailed Reports:
   - Code Quality Review: [link/location]
   - Security Review: [link/location]
   ═══════════════════════════════════════════════════════════
   ```

2. **Present decision matrix:**
   ```
   Review Decision Matrix:

   Based on findings, recommended action:

   IF no critical issues (code or security):
     → APPROVE - Ready to merge/deploy

   IF critical code issues only:
     → FIX CRITICAL CODE - Address critical code issues, re-review

   IF critical security issues:
     → FIX CRITICAL SECURITY - Address security vulnerabilities, re-review

   IF high-severity issues only:
     → CONDITIONAL APPROVAL - Fix high-priority items, can merge with plan

   IF rejected by either reviewer:
     → REJECT - Significant rework needed
   ```

3. **Present approval checklist:**
   ```
   Review Approval Checklist:
   - [ ] No critical code quality issues
   - [ ] No critical security vulnerabilities
   - [ ] No high-severity security issues (or mitigation plan exists)
   - [ ] OWASP Top 10 compliance acceptable
   - [ ] SOLUTION-PATTERNS.md followed
   - [ ] Test coverage adequate
   - [ ] Authorization correctly implemented
   - [ ] Audit logging complete
   ```

4. **Ask user for decision:**
   ```
   What action do you want to take?

   Options:
   - "approve" - Accept findings, approve for merge/deployment
   - "fix critical" - Fix critical issues, then re-review
   - "fix all high" - Fix critical + high issues, then re-review
   - "reject" - Significant rework needed, return to implementation
   ```

5. **Handle user response:**
   - **If "approve":**
     - Proceed to Step 3 (Review Complete)

   - **If "fix critical":**
     - Identify critical issues (code + security)
     - Developers fix critical issues
     - Return to Step 1 (re-run reviews)

   - **If "fix all high":**
     - Identify critical + high issues
     - Developers fix issues
     - Return to Step 1 (re-run reviews)

   - **If "reject":**
     - Capture feedback
     - Return to implementation
     - Full rebuild required

**Gate Criteria:**
- [ ] Both reviews completed
- [ ] User reviewed findings
- [ ] User made explicit decision

---

### Step 3: Review Complete

**Execution Instructions:**

Present completion summary based on user decision:

**If Approved:**
```
═══════════════════════════════════════════════════════════
Review Complete - APPROVED ✓
═══════════════════════════════════════════════════════════

Code Quality: [Status]
  - Critical issues: [count] (all fixed or accepted)
  - Pattern compliance: ✓
  - Test coverage: [percentage]%

Security: [Status]
  - Critical vulnerabilities: [count] (all fixed or accepted)
  - OWASP Top 10: [passed count]/10
  - Authorization: ✓

User Decision: APPROVED

═══════════════════════════════════════════════════════════
Next Steps:
═══════════════════════════════════════════════════════════

Code is approved for:
1. Merge to main branch
2. Deployment to staging/production
3. Release to users

Optional:
- Address medium/low priority findings in future iterations
- Update documentation
- Run "document" action if needed

Review approved! ✓
═══════════════════════════════════════════════════════════
```

**If Fix Required:**
```
═══════════════════════════════════════════════════════════
Review Complete - FIX REQUIRED
═══════════════════════════════════════════════════════════

Issues to Fix:
  - Critical code issues: [count]
  - Critical security issues: [count]
  - High priority issues: [count]

User Decision: FIX [critical/all high] THEN RE-REVIEW

═══════════════════════════════════════════════════════════
Action Items:
═══════════════════════════════════════════════════════════

1. Fix identified issues (see reports for details)
2. Run review action again after fixes
3. Repeat until approval

Issues identified. Fix and re-review.
═══════════════════════════════════════════════════════════
```

---

## Validation Criteria

**Overall Review Action Success:**
- [ ] Code quality review completed
- [ ] Security review completed
- [ ] User reviewed findings
- [ ] User made explicit decision
- [ ] If approved: No critical/high issues remaining
- [ ] If fix required: Issues documented with remediation plan

---

## Prerequisites

Before running review action:
- [ ] Implementation completed (features or full codebase)
- [ ] Tests written and passing
- [ ] Code committed to version control
- [ ] SOLUTION-PATTERNS.md exists

---

## Review Severity Levels

### Code Review Severity
- **Critical:** Breaks architecture, causes bugs, violates core principles, prevents deployment
- **High:** Code smells, poor patterns, missing tests, maintainability issues
- **Medium:** Style issues, minor improvements, optimization opportunities
- **Low:** Suggestions, nice-to-haves, subjective improvements

### Security Review Severity
- **Critical:** Actively exploitable vulnerabilities (SQL injection, auth bypass, XSS, etc.)
- **High:** Potential vulnerabilities requiring specific conditions, significant security gaps
- **Medium:** Security weaknesses, defense-in-depth gaps, non-critical misconfigurations
- **Low:** Best practice recommendations, hardening opportunities

---

## Example Usage

### Scenario 1: Clean Review with Approval
```
User: "Run the review action on the customer feature"

[Step 1: Parallel reviews]
Code Reviewer:
  - No critical issues
  - 2 medium issues (naming conventions)
  - Test coverage: 87%
  - Recommendation: APPROVED WITH RECOMMENDATIONS

Security:
  - No vulnerabilities
  - OWASP Top 10: 10/10 passed
  - Authorization: Correct
  - Recommendation: PASS

[Step 2: Approval gate]
Agent Runtime: "Review complete.
  Code: APPROVED WITH RECOMMENDATIONS (2 medium issues)
  Security: PASS

  Do you want to take action? (approve/fix critical/reject)"

User: "approve"

Agent Runtime: "Review approved! Code ready to merge."
```

### Scenario 2: Critical Security Issue
```
User: "Run the review action on order processing workflow"

[Step 1: Parallel reviews]
Code Reviewer:
  - 1 high issue (missing error handling)
  - Recommendation: APPROVED WITH HIGH-PRIORITY FIXES

Security:
  - 1 CRITICAL issue (SQL injection vulnerability in search)
  - 2 high issues (missing authorization checks)
  - Recommendation: FIX CRITICAL

[Step 2: Approval gate]
Agent Runtime: "Review complete.
  Code: APPROVED WITH RECOMMENDATIONS
  Security: FAIL (1 critical vulnerability)

  CRITICAL: SQL injection in customer search endpoint
  Location: CustomerController.cs:45
  Must fix before deployment!

  What action? (approve/fix critical/reject)"

User: "fix critical"

Agent Runtime: "Fixing SQL injection vulnerability..."
[Backend Developer fixes using parameterized queries]

Agent Runtime: "Issue fixed. Running security review again..."
[Security re-reviews]

Agent Runtime: "Security review: PASS. Critical issue resolved.
  Do you approve now?"

User: "approve"

Agent Runtime: "Review approved!"
```

---

## Related Actions

- **Before:** [build action](./build.md) or [feature action](./feature.md) - Implement first
- **After:** [document action](./document.md) - Document after approval
- **Part of:** [build action](./build.md) includes review as Phase 2

---

## Notes

- Review action can be run on any scope (feature, PR, full codebase)
- Both reviews run in parallel for efficiency
- Critical issues must be fixed before approval
- Review reports should be saved for tracking
- Reviews can be re-run after changes
- User has final decision on approval (agents recommend, user decides)
- Automated tools can supplement but not replace agent reviews

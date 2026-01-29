# Quality Engineer Agent

Complete specification and resources for the Quality Engineer builder agent role.

## Overview

The Quality Engineer Agent is responsible for comprehensive testing during Phase C (Implementation Mode). This agent ensures product quality through E2E tests, integration tests, performance testing, and accessibility validation.

**Key Principle:** Quality is built in, not tested in. Test early, test often, automate ruthlessly.

---

## Quick Start

### 1. Activate the Agent

When features are ready for testing:

```bash
# Read the agent specification
cat agents/quality-engineer/SKILL.md

# Review acceptance criteria
cat planning-mds/INCEPTION.md  # Section 3.4 (user stories)
```

### 2. Review Requirements

Understand what to test:
- Section 3.4: User stories with acceptance criteria
- Section 3.5: Screen specifications
- Section 4.3: Workflow rules
- Section 4.5: API contracts

### 3. Load References

```bash
# Testing best practices
cat agents/quality-engineer/references/testing-best-practices.md

# E2E testing guide
cat agents/quality-engineer/references/e2e-testing-guide.md

# Performance testing guide
cat agents/quality-engineer/references/performance-testing-guide.md
```

### 4. Follow the Workflow

See "Workflow Example" section in `SKILL.md` for step-by-step testing guidance.

---

## Agent Structure

```
quality-engineer/
├── SKILL.md                          # Main agent specification
├── README.md                         # This file
├── references/                       # Testing best practices
│   ├── testing-best-practices.md
│   ├── e2e-testing-guide.md
│   ├── api-testing-guide.md
│   ├── performance-testing-guide.md
│   ├── accessibility-testing-guide.md
│   └── test-data-management.md
└── scripts/                          # Testing scripts
    ├── README.md
    ├── run-all-tests.sh
    ├── generate-coverage-report.sh
    └── check-accessibility.sh
```

---

## Core Responsibilities

**Note on Ownership:** QE owns E2E, API, performance, and accessibility tests. Backend/Frontend Developers own unit and component tests. QE validates coverage and quality across all test types.

### 1. Test Planning
- Review acceptance criteria
- Create test plans (centralized in `planning-mds/testing/`)
- Identify test cases
- Prioritize by risk

### 2. E2E Testing (QE Owned)
- Write end-to-end tests (Playwright/Cypress)
- Test critical user workflows
- Validate full-stack integration
- Test multi-page workflows

### 3. API Testing (QE Owned)
- Test API endpoints directly
- Validate contracts
- Test error responses
- Test auth/authz at API level

### 4. Integration Testing (Shared)
- Test multi-service integration
- Test database transactions
- Test workflow transitions
- Backend Developer writes; QE validates and adds coverage

### 5. Performance Testing (QE Owned)
- Create load tests
- Measure response times
- Identify bottlenecks
- Set performance baselines

### 6. Accessibility Testing (QE Owned)
- Run automated accessibility scans
- Validate WCAG 2.1 AA compliance
- Test keyboard navigation
- Test with screen readers

### 7. Test Coverage Validation (QE Validates)
- Review code coverage reports from developers
- Identify coverage gaps
- Request additional unit/component tests from developers
- Ensure critical paths covered by all test types

### 8. Quality Reporting
- Track test pass/fail rates
- Monitor test execution times
- Report coverage metrics
- Provide quality dashboards

---

## Technology Stack

### E2E Testing
- **Tool:** Playwright (primary) or Cypress
- **Language:** TypeScript
- **Browsers:** Chromium, Firefox, WebKit

### API Testing
- **Tool:** Playwright API Testing or REST client
- **Language:** TypeScript or Python

### Performance Testing
- **Tool:** k6, Artillery, or Apache JMeter
- **Metrics:** Response time, throughput, error rate

### Accessibility Testing
- **Tool:** axe-core, Lighthouse, WAVE
- **Manual:** Keyboard nav, screen reader (NVDA, JAWS)

### Test Reporting
- **Tool:** Playwright HTML Reporter, Allure
- **CI/CD:** GitHub Actions, Jenkins, GitLab CI

---

## Test Structure

```
tests/
├── e2e/                           # End-to-end tests
│   ├── auth/
│   │   └── login.spec.ts
│   ├── brokers/
│   │   ├── create-broker.spec.ts
│   │   ├── broker-list.spec.ts
│   │   └── broker-360.spec.ts
│   └── fixtures/
│       └── test-data.ts
├── api/                           # API tests
│   ├── brokers.spec.ts
│   └── auth.spec.ts
├── performance/                   # Performance tests
│   ├── load-test-brokers.js
│   └── stress-test-api.js
└── accessibility/                 # Accessibility tests
    └── a11y-scan.spec.ts
```

---

## Key Resources

### References (QE-Specific)

Located in `agents/quality-engineer/references/`:

| Reference | Purpose | When to Use |
|-----------|---------|-------------|
| `testing-best-practices.md` | Testing principles and patterns | Daily testing reference |
| `e2e-testing-guide.md` | Playwright/Cypress patterns | Writing E2E tests |
| `api-testing-guide.md` | API testing strategies | Writing API tests |
| `performance-testing-guide.md` | Load/stress testing | Performance validation |
| `accessibility-testing-guide.md` | A11y testing | Accessibility validation |
| `test-data-management.md` | Test data strategies | Managing test data |

### Scripts

Located in `agents/quality-engineer/scripts/`:

| Script | Purpose | Usage |
|--------|---------|-------|
| `run-all-tests.sh` | Run all test suites | `./run-all-tests.sh` |
| `generate-coverage-report.sh` | Generate coverage reports | `./generate-coverage-report.sh` |
| `check-accessibility.sh` | Run accessibility scans | `./check-accessibility.sh` |

---

## Testing Workflow

### Step 1: Review User Story

- Read acceptance criteria
- Understand expected behavior
- Identify edge cases
- Note authorization requirements

### Step 2: Create Test Plan

Create test plan document:
```markdown
# Test Plan: [Feature Name]

## Scope
- User stories covered
- Acceptance criteria

## Test Cases
- TC-001: Happy path
- TC-002: Validation errors
- TC-003: Error scenarios

## Test Strategy
- E2E tests for user workflows
- API tests for contracts
- Performance tests for critical paths

## Success Criteria
- All tests pass
- Coverage >80%
```

### Step 3: Write E2E Tests

Create Playwright test:
```bash
touch tests/e2e/brokers/create-broker.spec.ts
```

Write test following AAA pattern:
- **Arrange:** Set up test data, navigate to page
- **Act:** Perform user actions
- **Assert:** Verify expected outcomes

### Step 4: Write API Tests

Create API test:
```bash
touch tests/api/brokers.spec.ts
```

Test API contracts directly:
- Request/response validation
- Error responses
- Auth/authz checks

### Step 5: Run Tests Locally

```bash
# Run E2E tests
npx playwright test

# Run E2E tests in headed mode (see browser)
npx playwright test --headed

# Run specific test file
npx playwright test tests/e2e/brokers/create-broker.spec.ts

# Debug tests
npx playwright test --debug
```

### Step 6: Review Test Results

Check test output:
- All tests passing?
- Failures with clear error messages?
- Screenshots captured for failures?

### Step 7: Run Performance Tests

```bash
# Run k6 load test
k6 run tests/performance/load-test-brokers.js
```

Verify response times meet targets.

### Step 8: Run Accessibility Tests

```bash
# Run automated accessibility scan
./agents/quality-engineer/scripts/check-accessibility.sh

# Manual keyboard testing
# - Tab through all interactive elements
# - Verify focus visible
# - Test Enter/Space on buttons
# - Test Escape to close dialogs
```

### Step 9: Generate Reports

```bash
# Generate coverage report
./agents/quality-engineer/scripts/generate-coverage-report.sh

# View Playwright HTML report
npx playwright show-report
```

### Step 10: Report Issues

Create bug reports for failures:
- Clear reproduction steps
- Expected vs actual behavior
- Screenshots/videos
- Logs and error messages

### Step 11: Validate Bug Fixes

When bugs are fixed:
- Reproduce original bug
- Verify fix resolves issue
- Ensure regression test added
- Close bug

---

## Quality Standards

### Test Quality Checklist

- [ ] Test names are descriptive (what is being tested)
- [ ] Tests follow AAA pattern (Arrange, Act, Assert)
- [ ] Tests are independent (no shared state)
- [ ] Tests clean up after themselves
- [ ] Tests use realistic data
- [ ] Tests handle async operations correctly
- [ ] Tests are reliable (no flakiness)

### Coverage Checklist

- [ ] Happy path tested for all acceptance criteria
- [ ] Edge cases tested
- [ ] Error scenarios tested
- [ ] Authentication tested
- [ ] Authorization tested
- [ ] Workflow transitions tested
- [ ] Critical user journeys have E2E tests

### E2E Test Checklist

- [ ] Uses realistic user interactions
- [ ] Validates UI state changes
- [ ] Validates data persistence
- [ ] Handles waits properly (no hardcoded sleeps)
- [ ] Takes screenshots on failure
- [ ] Runs in CI/CD

### API Test Checklist

- [ ] Tests all CRUD operations
- [ ] Validates request/response contracts
- [ ] Tests error responses (400, 401, 403, 404, 409, 500)
- [ ] Tests authentication
- [ ] Tests authorization
- [ ] Validates business logic

### Performance Checklist

- [ ] Load tests for critical endpoints
- [ ] Response time targets validated
- [ ] Bottlenecks identified
- [ ] Performance baselines established
- [ ] Results documented

### Accessibility Checklist

- [ ] Automated scans run (axe, Lighthouse)
- [ ] WCAG 2.1 AA violations checked
- [ ] Keyboard navigation tested
- [ ] Focus management validated
- [ ] Screen reader tested (basic)

---

## Common Pitfalls

### ❌ Flaky Tests

**Problem:** Tests pass/fail inconsistently

**Fix:**
```typescript
// Bad: Hardcoded sleep
await page.waitForTimeout(1000);

// Good: Wait for specific condition
await page.waitForSelector('text=Broker created successfully');
await page.waitForURL(/\/brokers\/[a-f0-9-]+/);
```

### ❌ Testing Implementation Details

**Problem:** Tests break on refactoring

**Fix:**
```typescript
// Bad: Testing internal class names
await page.click('.broker-form-submit-btn');

// Good: Testing user-visible text
await page.click('button:has-text("Create Broker")');
```

### ❌ Poor Test Data Management

**Problem:** Tests fail due to data conflicts

**Fix:**
- Use unique identifiers (timestamps, UUIDs)
- Clean up test data after each test
- Use database transactions (rollback after test)

### ❌ Slow E2E Tests

**Problem:** Tests take too long

**Fix:**
- Run tests in parallel (`workers: 4` in Playwright config)
- Use API calls for setup instead of UI clicks
- Keep E2E tests focused on critical workflows

### ❌ Missing Error Scenario Tests

**Problem:** Only testing happy path

**Fix:** Test validation errors, API failures, auth failures, etc.

---

## Definition of Done

### Before Reporting Tests Complete

- [ ] All E2E tests pass
- [ ] All API tests pass
- [ ] Performance tests pass
- [ ] Accessibility tests pass
- [ ] Test coverage meets minimum (>80%)
- [ ] No flaky tests
- [ ] Test documentation complete
- [ ] Issues reported for any failures
- [ ] Tests run in CI/CD

---

## Tools & Commands

### Playwright Commands

```bash
# Install Playwright browsers
npx playwright install

# Run all tests
npx playwright test

# Run tests in headed mode
npx playwright test --headed

# Run specific test file
npx playwright test tests/e2e/brokers/

# Run tests in debug mode
npx playwright test --debug

# Generate code (record interactions)
npx playwright codegen http://localhost:3000

# Show last test report
npx playwright show-report

# Run tests in specific browser
npx playwright test --project=chromium
npx playwright test --project=firefox
npx playwright test --project=webkit
```

### k6 Performance Testing

```bash
# Install k6
# macOS: brew install k6
# Linux: sudo apt-get install k6

# Run load test
k6 run tests/performance/load-test.js

# Run with specific VUs and duration
k6 run --vus 10 --duration 30s tests/performance/load-test.js

# Run stress test
k6 run --stage "0s:0,5s:100,10s:100,15s:0" tests/performance/stress-test.js
```

### Accessibility Testing

```bash
# Install axe CLI
npm install -g @axe-core/cli

# Run accessibility scan
axe http://localhost:3000 --exit
```

---

## Handoff to Code Reviewer

### Handoff Checklist

- [ ] All tests pass
- [ ] Test plan documented
- [ ] E2E tests cover critical workflows
- [ ] API tests validate contracts
- [ ] Performance tests pass
- [ ] Accessibility tests pass
- [ ] Coverage reports generated
- [ ] Issues reported for failures
- [ ] Tests committed to repository

### Handoff Artifacts

Provide to Code Reviewer:
1. Test plan document
2. Test execution report (all passing)
3. Coverage report
4. Performance test results
5. Accessibility scan results
6. Bug reports (if any issues found)

---

## Troubleshooting

### Test Failures

**Problem:** E2E test fails with "element not found"

**Fix:**
- Check selector accuracy
- Add proper wait (`waitForSelector`)
- Verify element exists in UI

### Timeout Errors

**Problem:** Test times out waiting for element

**Fix:**
- Increase timeout in config
- Use more specific selectors
- Check if element is actually rendered

### Authentication Issues

**Problem:** Tests fail with 401 Unauthorized

**Fix:**
- Verify test user credentials
- Check token storage/retrieval
- Ensure login flow completes

### Performance Issues

**Problem:** E2E tests run too slowly

**Fix:**
- Run tests in parallel
- Use API for test data setup
- Reduce test scope to critical paths

---

## Version History

**Version 1.0** - 2026-01-28 - Initial Quality Engineer agent
- SKILL.md with complete agent specification
- Testing best practices guides (pending creation)
- Testing scripts (pending creation)

---

## Next Steps

Ready to start testing?

1. Read `SKILL.md` thoroughly
2. Review acceptance criteria for feature
3. Create test plan
4. Write E2E tests for critical workflows
5. Write API tests for contracts
6. Run performance and accessibility tests
7. Report quality metrics
8. Validate before handoff

**Remember:** Your job is to ensure quality through comprehensive testing. Catch bugs early, prevent regressions, and provide clear feedback on product quality.

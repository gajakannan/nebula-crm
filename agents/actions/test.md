# Action: Test

## User Intent

Develop comprehensive test suites and execute testing to ensure quality, coverage, and correctness of the implementation.

## Agent Flow

```
Quality Engineer
```

**Flow Type:** Single agent

## Prerequisites

- [ ] Implementation code exists (backend and/or frontend)
- [ ] User stories with acceptance criteria available
- [ ] Test framework and tools configured

## Inputs

### From Planning
- User stories from `planning-mds/stories/`
- Acceptance criteria from `INCEPTION.md` section 3
- API contracts from `planning-mds/api/`
- Workflows from `INCEPTION.md` section 4.4

### From Codebase
- Backend API endpoints
- Frontend components
- Domain logic and services
- Database schema

### From User
- Testing scope (unit, integration, E2E, performance, or all)
- Specific features or workflows to test
- Quality gates and coverage requirements

## Outputs

### Test Suites
**Unit Tests:**
- Backend domain logic tests
- Backend service tests
- Frontend component tests
- Frontend utility/hook tests

**Integration Tests:**
- API endpoint tests
- Database integration tests
- External service integration tests

**E2E Tests:**
- Critical workflow tests
- User journey tests
- Cross-browser tests (if required)

### Test Documentation
- Test plan mapping stories to test cases
- Coverage reports
- Test execution results
- Quality metrics dashboard

### Test Infrastructure
- Test data fixtures
- Test helpers and utilities
- Mock/stub configurations
- Test environment setup scripts

## Agent Responsibilities

### Quality Engineer
1. **Test Planning:**
   - Read user stories and acceptance criteria
   - Create test plan mapping stories to test cases
   - Identify critical paths requiring E2E tests
   - Define quality gates and coverage targets

2. **Unit Testing:**
   - Write unit tests for domain logic
   - Write unit tests for services
   - Write component tests for UI
   - Achieve ≥80% coverage for business logic

3. **Integration Testing:**
   - Write API endpoint integration tests
   - Write database integration tests
   - Test external service integrations
   - Cover happy paths and error scenarios

4. **E2E Testing:**
   - Write E2E tests for critical workflows
   - Test complete user journeys
   - Validate acceptance criteria end-to-end
   - Cover edge cases and error paths

5. **Performance Testing (if required):**
   - Create performance test scenarios
   - Establish baseline metrics
   - Identify performance bottlenecks

6. **Test Execution:**
   - Run all test suites
   - Generate coverage reports
   - Document test results
   - Identify and report failures

7. **Quality Reporting:**
   - Provide test coverage metrics
   - Report quality gate status
   - Recommend improvements

## Validation Criteria

### Test Suite Complete
- [ ] Unit test coverage ≥80% for business logic
- [ ] Integration tests cover all API endpoints
- [ ] E2E tests cover critical workflows
- [ ] All acceptance criteria have tests
- [ ] Edge cases and errors tested

### Test Quality
- [ ] Tests are independent and isolated
- [ ] Tests have clear arrange-act-assert structure
- [ ] Tests have descriptive names
- [ ] No flaky tests
- [ ] Tests run quickly (unit < 1s, integration < 5s)

### Test Execution
- [ ] All tests pass
- [ ] Coverage reports generated
- [ ] Quality gates met
- [ ] No critical bugs found

## Example Usage

### Scenario 1: Test Single Feature
```
User: "Write tests for the customer management feature"

Test Action:
  ↓
Quality Engineer:
  - Reads customer user stories
  - Creates test plan for customer feature

  Unit Tests:
    - CustomerService.CreateCustomer
    - CustomerService.SearchCustomers
    - CustomerValidator tests

  Integration Tests:
    - POST /api/customers (create)
    - GET /api/customers (list)
    - GET /api/customers/:id (detail)
    - PUT /api/customers/:id (update)

  E2E Tests:
    - Create new customer flow
    - Search and filter customers flow
    - Update customer information flow

  Results:
    - 45 tests written
    - 92% coverage
    - All tests passing
```

### Scenario 2: Test Entire Application
```
User: "Run comprehensive testing on the entire app"

Test Action:
  ↓
Quality Engineer:
  - Creates comprehensive test plan

  Unit Tests:
    - All domain services
    - All validators
    - All components

  Integration Tests:
    - All API endpoints
    - All database operations

  E2E Tests:
    - All critical workflows
    - All user journeys

  Performance Tests:
    - API response times
    - Database query performance
    - Frontend render performance

  Results:
    - 347 tests written
    - 86% overall coverage
    - 2 failing tests identified
    - Performance baseline established
```

### Scenario 3: Regression Testing
```
User: "Run regression tests after bug fix"

Test Action:
  ↓
Quality Engineer:
  - Runs existing test suites
  - Adds new test for bug scenario

  Execution:
    - Unit tests: 156 passing
    - Integration tests: 89 passing
    - E2E tests: 23 passing

  Results:
    - All tests passing
    - Bug fix verified
    - No regressions detected
```

## Test Types and Scope

### Unit Tests
- **Scope:** Individual functions, methods, components
- **Speed:** Fast (< 1 second)
- **Dependencies:** Mocked/stubbed
- **Coverage Target:** ≥80% for business logic

### Integration Tests
- **Scope:** API endpoints, database operations, service integrations
- **Speed:** Medium (< 5 seconds)
- **Dependencies:** Real database (test DB), mocked external services
- **Coverage Target:** All API endpoints, critical integrations

### E2E Tests
- **Scope:** Complete user workflows, user journeys
- **Speed:** Slow (10-30 seconds)
- **Dependencies:** Full stack running
- **Coverage Target:** Critical paths, key workflows

### Performance Tests (Optional)
- **Scope:** API response times, database queries, UI rendering
- **Speed:** Variable
- **Dependencies:** Production-like environment
- **Coverage Target:** Critical operations, high-traffic endpoints

## Post-Test Next Steps

### If Tests Pass
1. Generate and review coverage reports
2. Ready to run **[review action](./review.md)** or deploy
3. Optionally improve coverage for low-coverage areas

### If Tests Fail
1. Analyze failures and fix bugs
2. Re-run test action
3. Repeat until all tests pass

## Related Actions

- **Part Of:** [build action](./build.md) - Testing is Phase 1 of build
- **Part Of:** [feature action](./feature.md) - Testing is part of feature slice
- **After:** [review action](./review.md) - Review can validate test quality
- **Before:** Deploy to production - Always test first

## Notes

- Test action can be run standalone or as part of build/feature actions
- Focus on quality over quantity (good tests > many tests)
- Prefer fast, focused tests over slow, broad tests
- Use test pyramid: many unit tests, fewer integration, fewest E2E
- Keep tests maintainable (avoid brittle tests)
- Run tests in CI/CD for continuous validation
- Test code should be as clean as production code

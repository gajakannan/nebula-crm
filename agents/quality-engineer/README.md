# Quality Engineer Agent

Generic specification for the Quality Engineer (QE) role.

## Overview

The Quality Engineer ensures quality through **automated testing** across Frontend, Backend, and AI layers. QE validates functionality, performance, security, and accessibility - not through manual clicking, but through comprehensive test automation.

## Quick Start

```bash
cat agents/quality-engineer/SKILL.md
cat planning-mds/architecture/TESTING-STRATEGY.md
cat planning-mds/architecture/TESTING-STACK-SUMMARY.md
```

## Core Workflow (Summary)

1) Read `planning-mds/INCEPTION.md` (acceptance criteria)
2) Review `planning-mds/architecture/TESTING-STRATEGY.md`
3) Set up test infrastructure (Testcontainers, MSW, Playwright)
4) Write tests following the Test Pyramid (70-20-10)
5) Run tests locally
6) Integrate with CI/CD
7) Monitor coverage (≥80% target)
8) Fix flaky tests

## Test Pyramid

```
      /\
     /  \  10% E2E Tests (Playwright)
    /----\     Slow, expensive, critical flows only
   /      \
  /--------\  20% Integration Tests (Testcontainers, MSW)
 /__________\     Medium speed, verify contracts

70% Unit Tests (xUnit, Vitest, pytest)
    Fast, isolated, business logic
```

## Testing by Layer

### Frontend (experience/)

| Type | Tool | Command |
|------|------|---------|
| **Unit/Component** | Vitest + React Testing Library | `npm test` |
| **Integration** | Vitest + MSW | `npm run test:integration` |
| **E2E** | Playwright | `npx playwright test` |
| **Accessibility** | @axe-core/playwright | `npm run test:a11y` |
| **Performance** | Lighthouse CI | `npm run lighthouse` |
| **Coverage** | Vitest | `npm run test:coverage` |

### Backend (engine/)

| Type | Tool | Command |
|------|------|---------|
| **Unit** | xUnit + FluentAssertions | `dotnet test` |
| **Integration** | xUnit + WebApplicationFactory | `dotnet test --filter Category=Integration` |
| **Database** | xUnit + Testcontainers | `dotnet test --filter Category=Database` |
| **API** | Bruno CLI | `bru run --env dev` |
| **Load** | k6 | `k6 run load-test.js` |
| **Coverage** | Coverlet | `dotnet test /p:CollectCoverage=true` |

### AI/Neuron (neuron/)

| Type | Tool | Command |
|------|------|---------|
| **Unit** | pytest | `pytest tests/` |
| **Integration** | pytest + FastAPI TestClient | `pytest tests/integration/` |
| **Evaluation** | pytest + custom metrics | `pytest tests/evaluation/` |
| **Performance** | pytest-benchmark | `pytest tests/ --benchmark-only` |
| **Coverage** | pytest-cov | `pytest --cov=neuron --cov-report=html` |

### Security (Cross-Cutting)

| Type | Tool | Command |
|------|------|---------|
| **Vulnerabilities** | Trivy | `trivy fs .` |
| **DAST** | OWASP ZAP | `docker run -t owasp/zap2docker-stable zap-baseline.py -t http://localhost` |
| **SAST** | SonarQube Community | `dotnet sonarscanner begin && dotnet build && dotnet sonarscanner end` |
| **Secrets** | Gitleaks | `gitleaks detect --source .` |

## Tech Stack (All Free & Open Source)

**Frontend:**
- Vitest (MIT) - Unit/component tests
- Playwright (Apache 2.0) - E2E tests
- MSW (MIT) - API mocking
- @axe-core/playwright (MPL 2.0) - Accessibility
- Lighthouse CI (Apache 2.0) - Performance

**Backend:**
- xUnit (Apache 2.0) - Unit/integration tests
- FluentAssertions (Apache 2.0) - Assertions
- Testcontainers (MIT) - Database tests
- Bruno CLI (MIT) - API collections
- k6 (AGPL v3) - Load testing
- Coverlet (MIT) - Coverage

**AI/Neuron:**
- pytest (MIT) - Testing framework
- pytest-benchmark (BSD) - Performance
- pytest-cov (MIT) - Coverage

**Security:**
- Trivy (Apache 2.0) - Vulnerability scanning
- OWASP ZAP (Apache 2.0) - DAST
- SonarQube Community (LGPL v3) - SAST

**Total Cost:** $0/year

## Best Practices Summary

1. **Test Pyramid** - 70% unit, 20% integration, 10% E2E
2. **Shift Left** - Test early, catch bugs before production
3. **Automation First** - Automate everything, minimize manual testing
4. **Fast Feedback** - Unit < 1s, Integration < 10s, E2E < 1min
5. **≥80% Coverage** - For business logic only
6. **No Flaky Tests** - Tests must be deterministic
7. **Test Behavior** - Test what users see, not implementation
8. **Quality Gates** - Block deployment if tests fail or coverage < 80%

## References

### Solution-Specific
- `planning-mds/architecture/TESTING-STRATEGY.md` - Complete testing strategy
- `planning-mds/architecture/TESTING-STACK-SUMMARY.md` - Tool reference
- `planning-mds/architecture/SOLUTION-PATTERNS.md` - Section 7 (Testing Patterns)

### Generic
- `agents/quality-engineer/references/testing-best-practices.md`
- `agents/quality-engineer/references/e2e-testing-guide.md`
- `agents/quality-engineer/references/performance-testing-guide.md`
- `agents/quality-engineer/references/test-case-mapping.md`

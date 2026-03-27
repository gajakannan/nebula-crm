# F0014 — DevOps Smoke Test Automation

**Status:** In Progress
**Priority:** High
**Phase:** Infrastructure

## Overview

Automated smoke tests and a one-command environment reset workflow for Nebula CRM. Eliminates the manual curl/inspect verification cycle that burned 30–60 minutes per feature verification. Includes authentik blueprint corrections for ROPC dev authentication, a 9-test API smoke suite, and a clean teardown-to-verify script.

## Documents

| Document | Purpose |
|----------|---------|
| [PRD.md](./PRD.md) | Full product requirements (why + what + how) |
| [STATUS.md](./STATUS.md) | Completion checklist and progress tracking |
| [GETTING-STARTED.md](./GETTING-STARTED.md) | Developer/agent setup guide |

## Stories

| ID | Title | Priority | Phase | Status |
|----|-------|----------|-------|--------|
| [F0014-S0001](./F0014-S0001-blueprint-ropc-fixes-and-smoke-scripts.md) | Blueprint ROPC fixes and smoke test scripts | Critical | Infrastructure | Done |
| [F0014-S0002](./F0014-S0002-multi-role-smoke-test-verification.md) | Multi-role smoke test verification | High | Infrastructure | Not Started |
| [F0014-S0003](./F0014-S0003-ci-smoke-test-integration.md) | CI smoke test integration | Medium | Future | Not Started |

**Total Stories:** 3
**Completed:** 1 / 3

## Architecture Review (2026-03-27)

**Phase B status:** Complete — no new entities, APIs, workflows, or Casbin policies. Infrastructure/scripts feature.

**Execution Plan:** [`feature-assembly-plan-F0014.md`](../../architecture/feature-assembly-plan-F0014.md)

### Key Findings

1. **S0002 BrokerUser expectation is incorrect.** The acceptance criteria claims broker001 gets 403 on both `POST /tasks` and `GET /my/tasks`. However, `policy.csv` §2.10 grants BrokerUser `task:read` (line 382). Correct behavior: `GET /my/tasks` → 200 OK, `POST/PUT/DELETE` → 403. The assembly plan documents the corrected assertions.

2. **No application code changes required.** F0014 is entirely shell scripts and CI workflow configuration. No backend, frontend, or AI scope.

3. **CI runner resource concern (S0003).** GitHub Actions `ubuntu-latest` has 7 GB RAM. The full docker compose stack (6 services) may exceed this. The assembly plan recommends starting only the 4 services smoke tests actually exercise (`db`, `authentik-server`, `authentik-worker`, `api`), skipping `temporal` and `temporal-ui`.

### Architecture Artifacts

| Artifact | Status |
|----------|--------|
| Data model / ERD | N/A — no entity changes |
| API contract (OpenAPI) | N/A — no new endpoints |
| Workflow state machine | N/A — no new workflows |
| Casbin policy | N/A — no policy changes (scripts test existing boundaries) |
| JSON schemas | N/A — no new request/response models |
| C4 diagrams | N/A — no container changes |
| ADRs | None required — no architectural decisions with alternatives to evaluate |
| Assembly plan | [`feature-assembly-plan-F0014.md`](../../architecture/feature-assembly-plan-F0014.md) |

# F0013 — Dashboard Framed Storytelling Canvas — Status

**Overall Status:** Done (Decision A override; active folder retained)
**Last Updated:** 2026-03-17
**Corrects:** F0012 (archived), F0011 (abandoned), F0010 (abandoned)

## Story Checklist

| Story | Title | Status |
|-------|-------|--------|
| F0013-S0000 | Editorial palette refresh — dark & light themes | [x] Done (Decision A override) |
| F0013-S0001 | Restore framed canvas identity with three-layer visual hierarchy | [x] Done (Decision A override) |
| F0013-S0002 | Build vertical timeline with connected stage nodes and terminal outcome branches | [x] Done (Decision A override) |
| F0013-S0003 | Add contextual mini-visualizations at each timeline stage node | [x] Done (Decision A override) |
| F0013-S0004 | Connect chapter controls as uniform override for timeline visualizations | [x] Done (Decision A override) |
| F0013-S0005 | Ensure responsive, accessibility, and performance parity | [x] Done (Decision A override) |

## Architecture Review

- [x] Architecture review complete (2026-03-14)
- [x] ADR-009 written: LOB classification + SLA configuration
- [x] Feature assembly plan updated with F0013 section
- [x] F0010/F0011/F0012 supersession status updated in assembly plan
- [x] Backend scope confirmed: LOB field, SLA table, breakdown endpoint, aging SLA enhancement

## Architect Closeout (Session 3)

- [x] Required signoff role coverage verified for every F0013 story (`S0000..S0005`) in Story Signoff Provenance
- [x] Integration checklist status verified against `planning-mds/architecture/feature-assembly-plan.md` (F0013 section)
- [x] ADR-009 implementation alignment verified

**Architect Exceptions (2026-03-17):**
1. Required role satisfaction is incomplete:
   - `Quality Engineer`, `Security Reviewer`, and `Code Reviewer` entries exist for every story, but story verdicts are not all PASS.
   - `DevOps` is marked as required in Required Signoff Roles, but no story-level DevOps provenance entries exist.
2. F0013 integration checklist is not fully complete:
   - Incomplete items remain (notably component decomposition targets, responsive/a11y E2E completion, and run/deploy closeout evidence).
3. ADR-009 core contract is implemented (LOB fields, SLA thresholds, aging SLA payload), but feature closeout is blocked by unresolved QE/Code Review findings.

**Architect Closeout Verdict:** `EXCEPTIONS RECORDED — NOT READY FOR DONE/ARCHIVE`

## Backend Progress

- [x] Validate existing F0012 backend changes (periodDays on KPIs, avgDwellDays + emphasis on flow nodes) are deployed and working
- [x] EF Migration: Add `LineOfBusiness` (string, nullable) to Submission and Renewal entities
- [x] EF Migration: Create `WorkflowSlaThreshold` table with seed data
- [x] Update Submission/Renewal DTOs and request schemas with LOB field
- [x] Implement breakdown endpoint: `GET /dashboard/opportunities/{entityType}/{status}/breakdown`
- [x] Enhance aging endpoint: add SLA bands (`sla` object) per status
- [x] Backfill OpenAPI spec: aging + hierarchy endpoint definitions (existing tech debt)
- [x] Update OpenAPI spec: breakdown endpoint, LOB fields, SLA response schema
- [x] Update dev seed data with LOB values on test submissions/renewals
- [x] Unit tests (breakdown groupBy, SLA band computation, LOB validation)
- [ ] Integration tests (breakdown endpoint, enhanced aging, LOB on CRUD)
- [ ] Full backend suite parity vs Session 0 baseline (blocked: new failures detected on 2026-03-16)

## Frontend Progress

- [x] Editorial palette tokens applied (dark: deep navy + coral + steel blue; light: warm gray + coral + steel blue)
- [x] Data visualization palette tokens defined (6 semantic colors)
- [x] Glass-card and glow utilities updated with new accent colors
- [ ] WCAG AA contrast verified for both themes
- [x] Three-layer visual hierarchy applied (glass-card restored on nudges, activity, tasks)
- [x] Glass-card depth and soft hover/focus glow on nudge cards
- [x] Glass-card depth and soft hover/focus glow on Activity panel
- [x] Glass-card depth and soft hover/focus glow on My Tasks panel
- [x] Story canvas zone (KPIs, flow, chapters) remains flat and borderless
- [x] Timeline bar replaces flat rectangular stage cells
- [x] Timeline stage nodes connected by proportional flow ribbons
- [x] Terminal outcome branches render at timeline end
- [x] Radial/donut chart popovers render on stage node hover/click
- [x] Radial center shows count, segments show composition
- [x] Mini-visual on one side, narrative callout (2-3 data-driven bullets) on the other per stop
- [x] Per-stop alternate view toggles on mini-visualizations (S0003)
- [x] Chapter controls (Flow/Friction/Outcomes) switch timeline emphasis and override mini-visuals
- [x] Collapsible left nav and right Neuron rail with adaptive canvas width
- [x] Legacy chapter overlays removed (`AgingOverlay`, `MixOverlay`) after S0004/S0005 consolidation
- [ ] Responsive layouts verified (desktop, tablet landscape, tablet portrait, phone)
- [x] F0013 opportunities component/integration tests passing (`OpportunitiesSummary.test.tsx`)
- [ ] Full frontend suite parity vs Session 0 baseline (blocked: pre-existing auth test failures)
- [x] Frontend dependency integrity restored after `pnpm` `EACCES` rename failures

## Cross-Cutting

- [ ] Screen specification created (`planning-mds/screens/S-DASH-002-framed-storytelling-canvas.md`)
- [ ] Feature test plan executed
- [ ] Deployability check evidence recorded
- [ ] No TODOs remain in implementation code

## Product Manager Closeout (Session 3)

- [x] PRD acceptance criteria reviewed against Session 2 evidence (`qe-2026-03-17`, `security-2026-03-17`, `code-review-2026-03-17`)
- [x] Tracker docs synchronized (`REGISTRY`, `ROADMAP`, `BLUEPRINT`, `STORY-INDEX`)
- [x] Tracker validation commands executed

**PM Verification Summary (2026-03-17):**
- PRD acceptance criteria are **not yet met** for release/closeout:
  - QE gate includes FAIL/BLOCKED outcomes (contrast and visual/runtime blockers).
  - Code Review gate is REJECTED with unresolved critical/high findings.
  - Security gate is CONDITIONAL PASS with scanner/tooling follow-ups pending.
- At review time, feature was blocked pending remediation + re-review.

## Decision Gate Outcome (2026-03-17)

- [x] Option A selected by user: **Mark F0013 Done and keep active folder**
- [x] Feature status updated to Done in active trackers (no archive move performed)
- [x] Unresolved gate findings retained in this document as explicit release risk record
- [x] Done status is recorded as a product-owner override with known exceptions

## Required Signoff Roles (Set in Planning)

| Role | Required | Why Required | Set By | Date |
|------|----------|--------------|--------|------|
| Quality Engineer | Yes | 6-story acceptance criteria, responsive/a11y, performance budgets. | Architect | 2026-03-14 |
| Code Reviewer | Yes | New entity, new endpoint, SVG component decomposition. | Architect | 2026-03-14 |
| Security Reviewer | Yes | Breakdown endpoint authorization, LOB data exposure. | Architect | 2026-03-14 |
| DevOps | Yes | EF Core migrations (LOB + SLA table), seed data. | Architect | 2026-03-14 |
| Architect | No | Patterns documented in ADR-009; no architecture exceptions. | - | - |

## Story Signoff Provenance

| Story | Role | Reviewer | Verdict | Evidence | Date | Notes |
|-------|------|----------|---------|----------|------|-------|
| F0013-S0000 | Quality Engineer | Codex (QE) | FAIL | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Light-theme KPI contrast check remains below threshold in visual gate. |
| F0013-S0000 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | ABAC and query-layer scope controls verified; scanner toolchain coverage remains incomplete. |
| F0013-S0000 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Light-theme KPI contrast fails and residual direct palette classes remain in F0013 UI paths. |
| F0013-S0001 | Quality Engineer | Codex (QE) | BLOCKED | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Full hierarchy E2E remains blocked by container proxy/API reachability failures. |
| F0013-S0001 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | Story data paths now use scoped repository queries; scanner tooling follow-up still required. |
| F0013-S0001 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Shared cross-story blockers (contrast/token compliance) still prevent approval. |
| F0013-S0002 | Quality Engineer | Codex (QE) | BLOCKED | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Timeline E2E verification is still blocked by visual test environment constraints. |
| F0013-S0002 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | Timeline aggregates inherit validated role-scope filtering and authorization checks. |
| F0013-S0002 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Timeline/story implementation remains monolithic vs planned component decomposition. |
| F0013-S0003 | Quality Engineer | Codex (QE) | BLOCKED | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Mini-visual behavior has passing unit coverage; full E2E remains blocked. |
| F0013-S0003 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | Breakdown leakage controls verified by integration tests; scanner coverage remains partial. |
| F0013-S0003 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Mini-visual/callout logic is still embedded in monolithic panels instead of planned extracted modules. |
| F0013-S0004 | Quality Engineer | Codex (QE) | BLOCKED | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Chapter override behavior is unit-tested; story-level E2E is still blocked. |
| F0013-S0004 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | Chapter-mode endpoints consume the same secured scoped aggregate paths. |
| F0013-S0004 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Chapter controls run on unresolved decomposition and palette/token compliance gaps. |
| F0013-S0005 | Quality Engineer | Codex (QE) | FAIL | `planning-mds/operations/evidence/f0013/qe-2026-03-17.md` | 2026-03-17 | Responsive/a11y/perf parity remains incomplete while visual E2E is blocked and contrast fails. |
| F0013-S0005 | Security Reviewer | Codex (Security) | CONDITIONAL PASS | `planning-mds/operations/evidence/f0013/security-2026-03-17.md` | 2026-03-17 | Security parity met for F0013 controls; CI scanner/tooling completion is still pending. |
| F0013-S0005 | Code Reviewer | Codex (Code Review) | FAIL | `planning-mds/operations/evidence/f0013/code-review-2026-03-17.md` | 2026-03-17 | Contrast requirement and visual gate determinism/portability issues remain unresolved. |

## Deferred Non-Blocking Follow-ups (Optional)

| Follow-up | Why deferred | Tracking link | Owner |
|-----------|--------------|---------------|-------|
| Drilldown from radial popover to filtered list view | Not core to storytelling canvas — can be added as enhancement | - | - |

## Tracker Sync Checklist

- [x] `planning-mds/features/REGISTRY.md` status/path aligned
- [x] `planning-mds/features/ROADMAP.md` section aligned (`Now/Next/Later/Completed`)
- [x] `planning-mds/features/STORY-INDEX.md` regenerated
- [x] `planning-mds/BLUEPRINT.md` feature/story status links aligned
- [ ] Every required signoff role has story-level `PASS` entries with reviewer, date, and evidence

## Archival Criteria

All items above must be checked before moving this feature folder to `planning-mds/features/archive/`.

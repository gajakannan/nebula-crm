# Manual Run Evidence

This directory stores evidence packages for human-orchestrated action runs.

## Structure

- One folder per run:
  - `planning-mds/operations/evidence/<run-id>/`

Each run folder should contain:
- `action-context.md`
- `artifact-trace.md`
- `gate-decisions.md`
- `commands.log`
- `lifecycle-gates.log`

## Example Package

- `plan-2026-02-08-preview-walkthrough/` demonstrates the required format for preview release readiness.

## Frontend UX Audit Evidence

- For frontend UI changes, add UX evidence files under:
  - `planning-mds/operations/evidence/frontend-ux/`
- Use:
  - `planning-mds/operations/evidence/frontend-ux/TEMPLATE.md`
- CI validates this requirement with:
  - `agents/frontend-developer/scripts/validate-frontend-ux-evidence.py`

## Frontend Quality Gate Evidence

- Solution-owned frontend validation evidence is tracked under:
  - `planning-mds/operations/evidence/frontend-quality/`
- The lifecycle gate consumes:
  - `planning-mds/operations/evidence/frontend-quality/latest-run.json`
- The manifest must distinguish:
  - component
  - integration
  - accessibility
  - coverage
  - visual
- The manifest must point to concrete artifacts, not review summaries alone.

For execution requirements, see `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`.

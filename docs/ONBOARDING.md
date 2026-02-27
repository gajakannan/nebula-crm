# New Project Onboarding Checklist

Initial preview mode note:
- Human-orchestrated execution is the default for this release.
- Use `docs/MANUAL-ORCHESTRATION-RUNBOOK.md` for required evidence capture.

## Step 1: Setup (15 minutes)

- [ ] Clone repository
- [ ] Read `README.md`
- [ ] (Optional) Build/run the framework container: `docker build -t nebula-agent-builder .`
- [ ] Review `blueprint-setup/README.md`
- [ ] Understand `BOUNDARY-POLICY.md`
- [ ] Review `docs/MANUAL-ORCHESTRATION-RUNBOOK.md`
- [ ] Review `docs/PREVIEW-RELEASE-CHECKLIST.md`

## Step 2: Copy Framework (30 minutes)

- [ ] Copy `agents/` to your new repo
- [ ] Copy `blueprint-setup/` to your new repo
- [ ] Update root `README.md` title
- [ ] (Optional) Adapt tech stack references

## Step 3: Bootstrap Planning (2–4 hours)

- [ ] Create `planning-mds/` folder structure
- [ ] Create `planning-mds/BLUEPRINT.md` from template
- [ ] Document domain knowledge in `planning-mds/domain/`
- [ ] Create 3–5 initial personas

## Step 4: First Sprint (1 week)

- [ ] Product Manager: Define first 3 features
- [ ] Architect: Draft first ADR
- [ ] Backend: Implement first endpoint
- [ ] Frontend: Implement first screen
- [ ] QA: Write first test cases

## Validation

- [ ] Review `lifecycle-stage.yaml` and confirm `current_stage` is correct
- [ ] Run `python3 scripts/run-lifecycle-gates.py --list`
- [ ] Run `python3 scripts/run-lifecycle-gates.py`
- [ ] Run `python3 agents/product-manager/scripts/validate-stories.py planning-mds/features`
- [ ] (Optional strict mode) Run `python3 agents/product-manager/scripts/validate-stories.py --strict-warnings planning-mds/features`
- [ ] Verify no solution-specific content in `agents/`
- [ ] Run `python3 scripts/validate-genericness.py`
- [ ] Confirm all specs follow templates

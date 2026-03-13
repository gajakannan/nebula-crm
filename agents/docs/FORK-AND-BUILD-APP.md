# Fork and Build App

This guide is the fastest path to fork this repository and build your own application with the agent framework.

## Outcome

After following this guide, you will have:
- A clean repo with reusable `agents/` framework content.
- Your own `planning-mds/` initialized for your product.
- First feature spec ready to implement through the action flow.

## Prerequisites

- You can run your preferred agent runtime (human-orchestrated preview is supported by default).
- You understand the boundary: `agents/` is generic, `planning-mds/` is solution-specific.
- You have a new project name, target users, and initial core entities.

## Choose Your Starting Mode

### Mode A: Framework-Only (Recommended for new products)

Use this when you want to reuse only the builder framework and start fresh requirements.

Keep:
- `agents/`
- `README.md` (rewrite for your project)
- `Dockerfile` and `docker-compose.agent-builder.yml` (optional but useful for builder runtime)

Delete:
- `planning-mds/`
- `engine/`
- `experience/`
- `neuron/` (if you do not need the starter AI layer yet)
- `blueprint-setup/` (optional; keep if you want sample bootstrap references)

### Mode B: Framework + Reference Implementation

Use this when you want to keep Nebula CRM as a working reference while adapting incrementally.

Keep everything initially, then replace `planning-mds/` and app code over time.

## Step 1: Fork and Clone

1. Fork this repository in GitHub.
2. Clone your fork locally.
3. Create your working branch.

## Step 2: Prune Solution-Specific Content

If you selected Mode A, remove Nebula-specific content:

```bash
git rm -r planning-mds engine experience neuron
```

If `neuron/` is part of your target architecture, keep it and adapt later.

## Step 3: Initialize Your Project Context

Run the `init` action from `agents/actions/init.md` using your agent runtime.

Provide:
- Project name
- Domain description
- Target users
- Core entities
- Optional stack preferences

Expected outputs:
- Root framework files (`lifecycle-stage.yaml`, `BOUNDARY-POLICY.md`, `CONTRIBUTING.md`, `.github/workflows/ci-gates.yml`)
- Fresh `planning-mds/` structure
- Seeded `planning-mds/BLUEPRINT.md`

## Step 4: Replace Defaults with Your Product Context

1. Rewrite root `README.md` for your product.
2. Replace any Nebula/insurance terminology in `planning-mds/`.
3. Keep all generic role/action content under `agents/` unchanged.
4. If your stack differs, follow `agents/TECH-STACK-ADAPTATION.md`.
5. Confirm boundary rules in `BOUNDARY-POLICY.md`.

## Step 5: Create Your First Feature Pack

1. Add your first feature folder: `planning-mds/features/F0001-<slug>/`
2. Add one story file in that folder using the story template.
3. Update `planning-mds/features/REGISTRY.md`.
4. Add any required API contract docs under `planning-mds/api/`.

## Step 6: Execute the Build Flow

Use action flow in this sequence:
1. `plan` (PM + Architect)
2. `feature` or `build` (implementation roles)
3. `review` (Code Reviewer + Security)
4. `test` and `validate`

Action definitions live in `agents/actions/README.md`.

## Step 7: Run Gates Before PRs

```bash
python3 -m pip install -r agents/scripts/requirements.txt
python3 agents/scripts/run-lifecycle-gates.py --list
python3 agents/scripts/run-lifecycle-gates.py
python3 agents/scripts/validate-genericness.py
python3 agents/product-manager/scripts/validate-stories.py planning-mds/features
```

## Common Mistakes to Avoid

- Putting project-specific terms into `agents/`.
- Keeping stale reference content in `planning-mds/` after forking.
- Adapting templates/roles before proving baseline flow with one real feature.
- Skipping lifecycle stage updates in `lifecycle-stage.yaml`.

## Related Docs

- `agents/README.md`
- `agents/actions/README.md`
- `agents/docs/ONBOARDING.md`
- `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`
- `agents/TECH-STACK-ADAPTATION.md`
- `BOUNDARY-POLICY.md`

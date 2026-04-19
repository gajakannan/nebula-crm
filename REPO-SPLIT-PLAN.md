# Repository Split Plan: nebula-crm → nebula-agents + nebula-insurance-crm

**Authored:** 2026-04-18
**Source repo:** `gajakannan/nebula-crm`
**Migration baseline commit:** `5150f7c16b43d8a639e111ffb2b9b9b3e8b595c5`
**Target repos:** `gajakannan/nebula-agents` · `gajakannan/nebula-insurance-crm`
**Status of nebula-crm:** Remains untouched — archival/historical reference only

---

## 1. Intent

`nebula-crm` currently serves two distinct purposes in a single repository:

1. A **reusable, tool-agnostic agent framework** (`agents/`, `blueprint-setup/`, framework root docs)
2. A **concrete insurance CRM product** (`planning-mds/`, `engine/`, `experience/`, `neuron/`, runtime scripts)

The boundary between these two is already explicit and documented (see `BOUNDARY-POLICY.md`). This plan formalizes the split into two independent repos with no submodules, no subtree embedding, and no build-time coupling.

`nebula-crm` is not being deleted or modified. It remains the historical source. Both new repos are initialized from a copy of its content, then evolved independently.

---

## 2. The Critical Design Decision: Consumption Model

This is the most important thing the executing agent needs to understand before doing anything else.

### How the two repos are used together

**nebula-agents is the session working directory.** When a developer or AI tool (Claude Code, Codex, Cursor, Aider, or any other) starts a session to work on the insurance CRM — or any other product — they open that session rooted in `nebula-agents`. The agent reads its role definitions, action protocols, and templates from `agents/` in that working directory. It then performs all implementation work in the sibling product repo (`../nebula-insurance-crm/` or any other `../product-repo/`).

**The contract is plain markdown, not a tool-specific config.** Because the framework guidance lives in ordinary `.md` files, any AI tool can consume it. There is no dependency on `CLAUDE.md`, `.cursorrules`, or any vendor-specific mechanism. Those files are ergonomic shortcuts for specific tools — they auto-load context that a human or tool would otherwise provide manually. They are not the contract.

**There is no build-time or runtime coupling.** nebula-insurance-crm does not import, reference, or depend on nebula-agents at compile time. The connection is entirely process-level: the agent running the session knows to look left for framework guidance and right for product artifacts.

### Expected workspace layout

```
workspace/
  nebula-agents/          ← session working directory for all agent work
  nebula-insurance-crm/   ← implementation target (or any other product repo)
```

Both repos sit as siblings. The executing agent references the product repo via relative path `../nebula-insurance-crm/`. This layout must be documented in nebula-agents' README so any operator cloning the framework knows the expected setup.

### What this means for CLAUDE.md (and equivalent files)

- `nebula-agents/.claude/settings.json` and `nebula-agents/CLAUDE.md` (if it exists) configure the session for framework-level work and document the cross-repo operating pattern.
- `nebula-insurance-crm/CLAUDE.md` (if used) is purely product-level context. It does NOT need to reference nebula-agents at all because the session that drives CRM work is rooted in nebula-agents, not the CRM repo.
- If a tool does not use `CLAUDE.md`, the operator provides the same context manually at session start. The framework is designed for this — nothing breaks.

---

## 3. Repository Boundaries

### nebula-agents contains

Everything generic and reusable regardless of domain:

| Source path (in nebula-crm) | Destination | Notes |
|---|---|---|
| `agents/` | `agents/` | Entire directory — roles, actions, templates, scripts, docs |
| `blueprint-setup/` | `blueprint-setup/` | Framework bootstrap guidance |
| `docker/agent-builder/` | `docker/agent-builder/` | Builder container entrypoint |
| `Dockerfile` | `Dockerfile` | Builder image (framework runtime, not app runtime) |
| `docker-compose.agent-builder.yml` | `docker-compose.agent-builder.yml` | Builder compose |
| `BOUNDARY-POLICY.md` | `BOUNDARY-POLICY.md` | Framework policy doc |
| `CONTRIBUTING.md` | `CONTRIBUTING.md` | Framework contribution guide |
| `LICENSE` | `LICENSE` | Copy verbatim |
| `README.md` | `README.md` | **Rewrite** — see Section 5 |
| `.github/workflows/ci-gates.yml` | `.github/workflows/ci-gates.yml` | Framework-level gates only (boundary_genericness, skill_regression) |

**Does NOT contain:**
- Any insurance/CRM planning artifacts
- Application source code (`engine/`, `experience/`, `neuron/`)
- CRM runtime scripts (`scripts/deploy/`, `scripts/kg/`, deploy shell scripts)
- CRM docker/compose files (`docker-compose.yml`, `docker-compose.qe.yml`, `docker/authentik/`, `docker/postgres/`)
- CRM API test collections (`bruno/`)
- `lifecycle-stage.yaml` from nebula-crm (that file is CRM-instance state; a generic template version belongs here — see Section 5)
- `pyproject.toml` (CRM-scoped pytest config)

### nebula-insurance-crm contains

Everything specific to the insurance CRM product:

| Source path (in nebula-crm) | Destination | Notes |
|---|---|---|
| `planning-mds/` | `planning-mds/` | All product planning, F0018 included |
| `engine/` | `engine/` | C# .NET backend |
| `experience/` | `experience/` | React frontend |
| `neuron/` | `neuron/` | AI runtime layer |
| `docker/authentik/` | `docker/authentik/` | Auth runtime infra |
| `docker/postgres/` | `docker/postgres/` | Database runtime infra |
| `docker-compose.yml` | `docker-compose.yml` | App runtime compose |
| `docker-compose.qe.yml` | `docker-compose.qe.yml` | QE compose |
| `scripts/` | `scripts/` | All deploy/runtime/QA scripts |
| `bruno/` | `bruno/` | API test collections |
| `pyproject.toml` | `pyproject.toml` | Pytest config |
| `lifecycle-stage.yaml` | `lifecycle-stage.yaml` | CRM lifecycle state (rename stage to `implementation`, active feature to F0018) |
| `LICENSE` | `LICENSE` | Copy verbatim |
| `README.md` | `README.md` | **Rewrite** — see Section 6 |
| `.github/workflows/` (all except ci-gates.yml) | `.github/workflows/` | All CRM CI workflows |

**Does NOT contain:**
- `agents/` directory
- `blueprint-setup/` directory
- Builder `Dockerfile` or `docker-compose.agent-builder.yml`
- `BOUNDARY-POLICY.md` or `CONTRIBUTING.md` (framework-owned; reference by URL instead)

---

## 4. Genericness Cleanup Required in nebula-agents

Before nebula-agents can stand alone as a generic framework, product-specific leakage in `agents/` must be cleaned. This cleanup happens **inside nebula-agents after the copy** — do not modify nebula-crm.

### Known leaks (confirmed by grep)

| File | Leak | Fix |
|---|---|---|
| `agents/templates/feature-assembly-plan-template.md` | Uses `Nebula.Domain/Entities/{Entity}.cs`, `Nebula.Application/DTOs/{Dto}.cs` etc. — hardcoded C# namespace prefix `Nebula.*` | Replace `Nebula.` prefix with a generic placeholder like `{AppName}.` or `YourApp.` |
| `agents/templates/feature-assembly-plan-template.md` | References "Nebula API profile" | Replace with "your project's API profile (see `agents/docs/ORCHESTRATION-CONTRACT.md`)" |
| `agents/backend-developer/SKILL.md` (line 31) | "Follow Nebula API profile for route patterns..." | Replace with "Follow your project's API profile" |
| `agents/actions/feature.md` (line 68) | References `planning-mds/api/nebula-api.yaml` by name | Replace with `planning-mds/api/<your-api>.yaml` or generic path placeholder |
| `agents/templates/prompts/feature-operator-friendly.md` | Hardcodes `planning-mds/api/nebula-api.yaml` | Same fix — use generic path |
| `agents/docs/FORK-AND-BUILD-APP.md` | References "Nebula CRM" by name as example | Replace with generic "your reference project" language or move specific example to a clearly-marked example block |
| `agents/docs/ONBOARDING.md` | References `nebula-agent-builder` Docker image tag | Replace with generic image name like `agent-builder` |

**Author attribution is intentional — do not change:** `"author": "Nebula Framework Team"` in SKILL.md frontmatter is the framework team identity, not a product reference. Leave it.

### How to find remaining leaks

Run the existing genericness validator after cleanup to confirm no new leaks:

```bash
# From nebula-agents root
python3 agents/scripts/validate-genericness.py
```

The validator reads blocked terms from `planning-mds/domain/glossary.md`. Since nebula-agents won't have `planning-mds/`, you will need to either:

1. Pass the glossary from the sibling nebula-insurance-crm during validation: `python3 agents/scripts/validate-genericness.py --glossary ../nebula-insurance-crm/planning-mds/domain/glossary.md`
2. **Or** embed a minimal blocked-terms list directly into the script (preferred for standalone operation) — extract the `Genericness-Blocked Terms` section from the glossary and copy it into the script as a static fallback.

Option 2 makes nebula-agents self-contained. Option 1 is acceptable short-term.

---

## 5. nebula-agents: New and Rewritten Documents

### 5.1 README.md (rewrite from scratch)

The current README serves both purposes. Rewrite it as a pure framework product README with these sections:

- **What it is:** A tool-agnostic, orchestrator-agnostic agent-driven development framework
- **What it owns:** Role definitions, action protocols, templates, genericness enforcement, bootstrap guidance, builder runtime
- **What it does not own:** Domain planning, application code, product runtime infrastructure, deployment scripts
- **How downstream products consume it:** The workspace layout pattern — open session in nebula-agents, implement in sibling product repo. Plain markdown contract works with any AI tool.
- **Quick start:** Clone, open session in this directory, follow `agents/docs/FORK-AND-BUILD-APP.md`
- **Available actions:** Link to `agents/actions/README.md`
- **Framework architecture diagram:** Preserve the existing diagram but remove any CRM-specific references

### 5.2 CONSUMER-CONTRACT.md (new file at root)

This document is the formal interface between nebula-agents and any downstream product repo. Contents:

- **Required planning structure** that the framework expects: `planning-mds/BLUEPRINT.md`, `planning-mds/domain/glossary.md`, `planning-mds/api/<api>.yaml`
- **Required action artifact paths** for each action (plan, build, feature, review, etc.)
- **Lifecycle gate contract:** what `lifecycle-stage.yaml` must contain, what stages are valid, what gates each stage requires
- **Genericness contract:** no framework files may reference product-specific terms; enforcement via `validate-genericness.py`
- **Workspace layout convention:** `workspace/nebula-agents/` and `workspace/<product-repo>/` as siblings
- **API reference path convention:** product repos own their OpenAPI spec; framework agents reference it via path provided at session start, not hardcoded
- **Versioning policy:** how downstream products pin to a nebula-agents version (git tag, commit ref)
- **Stack adaptation:** how to replace stack-specific reference guides (existing `TECH-STACK-ADAPTATION.md` can be referenced or merged here)

### 5.3 lifecycle-stage-template.yaml (new file under agents/templates/)

The current `lifecycle-stage.yaml` at the root of nebula-crm is a CRM instance configuration. Extract the schema and stage definitions into a generic template for downstream products to copy and fill in. Remove CRM-specific values. Add comments explaining each field.

### 5.4 docs/migration-from-nebula-crm.md (new file under agents/docs/)

A short migration note for anyone who was using nebula-crm as their framework source:

- What moved where
- The new workspace layout
- How to update their `lifecycle-stage.yaml` references
- Link to CONSUMER-CONTRACT.md

### 5.5 CHANGELOG.md (new file at root)

Initial entry: `v0.1.0 — 2026-04-18 — Initial standalone release, split from gajakannan/nebula-crm at commit 5150f7c`.

Tag this commit `v0.1.0` after the repo is set up.

---

## 6. nebula-insurance-crm: New and Rewritten Documents

### 6.1 README.md (rewrite from scratch)

Product-focused README:

- **What it is:** Nebula Insurance CRM — a commercial P&C CRM built with the Nebula Agent Framework
- **Tech stack:** C# .NET (engine/), React TypeScript (experience/), Python AI layer (neuron/), PostgreSQL, Authentik
- **How to run locally:** Docker compose commands, dev setup
- **Planning structure:** Where to find BLUEPRINT.md, feature planning, API specs
- **Feature roadmap:** Link to `planning-mds/features/ROADMAP.md` and `REGISTRY.md`
- **Active work:** F0018 Policy Lifecycle (planning complete, implementation starting)
- **Framework:** This product uses the Nebula Agent Framework from `gajakannan/nebula-agents` (pinned to `v0.1.0`)

### 6.2 docs/agent-framework-compatibility.md (new file)

This is the key integration document. Contents:

- **Framework version:** nebula-agents `v0.1.0` (commit `5150f7c16b43d8a639e111ffb2b9b9b3e8b595c5`)
- **Consumption model:** Sessions run with nebula-agents as working directory; agent implements in this repo via `../nebula-insurance-crm/`
- **Expected workspace layout:**
  ```
  workspace/
    nebula-agents/
    nebula-insurance-crm/
  ```
- **Local product conventions:** Any CRM-specific conventions layered on top of framework defaults (e.g., CRM uses `.NET 9 + EF Core`, React with Radix UI, Python AI in `neuron/`)
- **Action to artifact path mapping:** Concrete paths in this repo where each action writes its outputs
- **Lifecycle gates in this repo:** What `lifecycle-stage.yaml` is set to and what commands to run
- **How to upgrade framework version:** Update the pinned commit/tag reference in this doc and re-validate genericness

### 6.3 lifecycle-stage.yaml (update from nebula-crm version)

Copy `lifecycle-stage.yaml` from nebula-crm. Update:
- `current_stage`: `implementation`
- `active_feature`: `F0018-policy-lifecycle-and-policy-360`
- `summary`: Update to reflect post-split state and F0018 as active feature
- Gate commands that reference `agents/scripts/` — these now live in `../nebula-agents/agents/scripts/`. Update command paths or add a note that gates requiring framework scripts must be run from the nebula-agents session.

### 6.4 docs/migration-from-nebula-crm.md (new file)

Short note:
- Source commit from nebula-crm
- What was removed (agents/, blueprint-setup/, builder Dockerfile)
- What was preserved intact (all planning artifacts, all application code)
- F0018 status at time of migration

---

## 7. F0018 Handling

F0018 (Policy Lifecycle & Policy 360) is at the ideal split point: planning is complete, implementation has not started. There are 11 stories all in "In Refinement" status.

**Action:**
- `planning-mds/features/F0018-policy-lifecycle-and-policy-360/` moves to nebula-insurance-crm intact — no changes to the planning artifacts
- Do NOT start F0018 implementation before the split and the dry-run validation (Section 9) are complete
- After the split and dry-run, F0018 implementation begins in nebula-insurance-crm, driven from a session rooted in nebula-agents

**Why this order:**
- Prevents partial implementation straddling two repos
- Ensures the first real code commit to nebula-insurance-crm is under the new operating model
- Planning artifacts belong to the CRM product side, so they move cleanly

---

## 8. GitHub Repository Setup

Create both repos at:
- `https://github.com/gajakannan/nebula-agents`
- `https://github.com/gajakannan/nebula-insurance-crm`

Both should be initialized **empty** (no auto-generated README). The content will be pushed from local copies derived from nebula-crm.

Recommended initial setup for each:
- Default branch: `main`
- Branch protection on `main`: require PR, require CI pass
- No submodules configured

**Do not** create any GitHub Actions workflows that cross-reference the other repo. The two repos are independent.

---

## 9. Execution Order

Execute these steps in sequence. Do not skip or reorder.

### Step 1: Establish baseline (no file changes)

```bash
# Record the migration baseline — already documented above as:
# 5150f7c16b43d8a639e111ffb2b9b9b3e8b595c5

git -C /path/to/nebula-crm log --oneline -1
# Should output: 5150f7c Add JSON schemas for policy management
```

### Step 2: Create nebula-agents locally

```bash
mkdir -p workspace/nebula-agents
cd workspace/nebula-agents
git init
git branch -M main

# Copy framework content from nebula-crm
cp -r /path/to/nebula-crm/agents .
cp -r /path/to/nebula-crm/blueprint-setup .
cp -r /path/to/nebula-crm/docker/agent-builder docker/agent-builder
mkdir -p docker && cp -r /path/to/nebula-crm/docker/agent-builder docker/
cp /path/to/nebula-crm/Dockerfile .
cp /path/to/nebula-crm/docker-compose.agent-builder.yml .
cp /path/to/nebula-crm/BOUNDARY-POLICY.md .
cp /path/to/nebula-crm/CONTRIBUTING.md .
cp /path/to/nebula-crm/LICENSE .
# Copy only ci-gates.yml from workflows
mkdir -p .github/workflows
cp /path/to/nebula-crm/.github/workflows/ci-gates.yml .github/workflows/
```

### Step 3: Run genericness cleanup in nebula-agents

Fix all known leaks listed in Section 4:

1. `agents/templates/feature-assembly-plan-template.md` — replace `Nebula.*` C# namespace prefix and "Nebula API profile"
2. `agents/backend-developer/SKILL.md` — replace "Nebula API profile" in line 31
3. `agents/actions/feature.md` — replace hardcoded `nebula-api.yaml` reference
4. `agents/templates/prompts/feature-operator-friendly.md` — same fix
5. `agents/docs/FORK-AND-BUILD-APP.md` — replace specific "Nebula CRM" references with generic language
6. `agents/docs/ONBOARDING.md` — replace `nebula-agent-builder` Docker tag with generic `agent-builder`

After each file is fixed, run the genericness validator:
```bash
python3 agents/scripts/validate-genericness.py --glossary /path/to/nebula-crm/planning-mds/domain/glossary.md
```

Fix any additional leaks surfaced by the validator.

### Step 4: Write new nebula-agents documents

Create the following (per Section 5):

- `README.md` — rewrite
- `CONSUMER-CONTRACT.md` — new
- `agents/templates/lifecycle-stage-template.yaml` — new
- `agents/docs/migration-from-nebula-crm.md` — new
- `CHANGELOG.md` — new

### Step 5: Initial commit and push to GitHub

```bash
git add .
git commit -m "Initial release: nebula-agents v0.1.0 — split from gajakannan/nebula-crm@5150f7c"
git remote add origin https://github.com/gajakannan/nebula-agents.git
git push -u origin main
git tag v0.1.0
git push origin v0.1.0
```

### Step 6: Create nebula-insurance-crm locally

```bash
mkdir -p workspace/nebula-insurance-crm
cd workspace/nebula-insurance-crm
git init
git branch -M main

# Copy product content from nebula-crm
cp -r /path/to/nebula-crm/planning-mds .
cp -r /path/to/nebula-crm/engine .
cp -r /path/to/nebula-crm/experience .
cp -r /path/to/nebula-crm/neuron .
cp -r /path/to/nebula-crm/bruno .
cp -r /path/to/nebula-crm/scripts .
cp -r /path/to/nebula-crm/docker/authentik docker/authentik
cp -r /path/to/nebula-crm/docker/postgres docker/postgres
cp /path/to/nebula-crm/docker-compose.yml .
cp /path/to/nebula-crm/docker-compose.qe.yml .
cp /path/to/nebula-crm/pyproject.toml .
cp /path/to/nebula-crm/lifecycle-stage.yaml .
cp /path/to/nebula-crm/LICENSE .
# Copy all CRM workflows
mkdir -p .github/workflows
for f in dotnet-test.yml frontend-performance.yml frontend-ui.yml pact-contract.yml qe-api.yml smoke-test.yml sonarqube.yml vitest.yml; do
  cp /path/to/nebula-crm/.github/workflows/$f .github/workflows/
done
```

### Step 7: Update nebula-insurance-crm lifecycle-stage.yaml

Edit `lifecycle-stage.yaml`:
- `current_stage`: `implementation`
- `active_feature`: `F0018-policy-lifecycle-and-policy-360`
- `active_stories`: Remove old story list, set to `[]` (F0018 stories haven't started)
- Update the `summary` field to reflect post-split state
- Gate commands that call `agents/scripts/` or `agents/devops/scripts/` must be updated. Since those scripts now live in `../nebula-agents/`, update the paths or add a comment that these gates run from a nebula-agents session:
  ```yaml
  boundary_genericness:
    command: ["python3", "../nebula-agents/agents/scripts/validate-genericness.py", "--glossary", "planning-mds/domain/glossary.md"]
  ```

### Step 8: Write new nebula-insurance-crm documents

Create the following (per Section 6):

- `README.md` — rewrite
- `docs/agent-framework-compatibility.md` — new
- `docs/migration-from-nebula-crm.md` — new

### Step 9: Initial commit and push to GitHub

```bash
git add .
git commit -m "Initial release: nebula-insurance-crm — split from gajakannan/nebula-crm@5150f7c"
git remote add origin https://github.com/gajakannan/nebula-insurance-crm.git
git push -u origin main
```

### Step 10: Dry-run workflow validation

Before starting F0018 implementation, validate the new operating model end-to-end.

**Test 1 — Framework session reads correctly:**
Open a session rooted in `workspace/nebula-agents/`. Ask the agent:
> "Read the architect SKILL and the feature action. What are the required planning inputs and where should outputs be written for a feature implementation in ../nebula-insurance-crm/?"

The agent should correctly identify `planning-mds/` paths relative to the product repo.

**Test 2 — Cross-repo navigation works:**
From the same session, ask the agent to read `../nebula-insurance-crm/planning-mds/features/F0018-policy-lifecycle-and-policy-360/README.md` and summarize the feature scope.

The agent should read the file without any tool errors and produce an accurate summary.

**Test 3 — Non-coding task dry-run:**
Ask the agent to review F0018's STATUS.md and identify which stories are ready for implementation, per the backend-developer SKILL. It should reference `../nebula-insurance-crm/planning-mds/features/F0018-policy-lifecycle-and-policy-360/STATUS.md` and the SKILL from `agents/backend-developer/SKILL.md`.

All three tests passing confirms the operating model works.

---

## 10. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Gate commands in `lifecycle-stage.yaml` break because `agents/scripts/` no longer exists in the CRM repo | Update all gate command paths in Step 7; note in `docs/agent-framework-compatibility.md` that gate scripts run from nebula-agents session |
| Agent accidentally commits to nebula-agents instead of nebula-insurance-crm | Document explicitly in `nebula-agents/CONSUMER-CONTRACT.md` and in session start instructions: all git commits during product implementation go to `../nebula-insurance-crm/` |
| Genericness validator loses its glossary source after split | Fix the validator to support `--glossary` flag pointing to a sibling repo path (already supported), OR embed a minimal static blocklist as fallback (preferred) |
| `.github/workflows/ci-gates.yml` in nebula-agents tries to run CRM-specific gates (e.g., `solution_contract`, `knowledge_graph_sync`) | The workflow must be updated to run only framework-level gates: `boundary_genericness` and `skill_regression` |
| nebula-insurance-crm CI workflows reference `agents/scripts/` for framework validation | Update those workflow steps to run from a checkout of nebula-agents, or skip framework validation in CRM CI (framework gates are the framework repo's responsibility) |
| F0018 implementation starts before dry-run validation, creating hidden assumptions | Do not create the F0018 feature branch in nebula-insurance-crm until Step 10 passes |

---

## 11. What The Executing Agent Should Produce

By the end of this plan, both repos on GitHub should contain:

**gajakannan/nebula-agents:**
- [ ] All content from `agents/`, `blueprint-setup/`, `docker/agent-builder/`, builder `Dockerfile`, `docker-compose.agent-builder.yml`, `BOUNDARY-POLICY.md`, `CONTRIBUTING.md`, `LICENSE`
- [ ] Rewritten `README.md`
- [ ] New `CONSUMER-CONTRACT.md`
- [ ] New `agents/templates/lifecycle-stage-template.yaml`
- [ ] New `agents/docs/migration-from-nebula-crm.md`
- [ ] New `CHANGELOG.md`
- [ ] All genericness leaks fixed and validator passing
- [ ] Tagged `v0.1.0`

**gajakannan/nebula-insurance-crm:**
- [ ] All content from `planning-mds/`, `engine/`, `experience/`, `neuron/`, `bruno/`, `scripts/`, `docker/authentik/`, `docker/postgres/`, compose files, `pyproject.toml`, `LICENSE`
- [ ] Rewritten `README.md`
- [ ] New `docs/agent-framework-compatibility.md` pinned to nebula-agents `v0.1.0`
- [ ] New `docs/migration-from-nebula-crm.md`
- [ ] Updated `lifecycle-stage.yaml` with F0018 as active feature
- [ ] Gate command paths updated to reference `../nebula-agents/`

**Both repos:**
- [ ] Dry-run workflow validation (Section 9, Step 10) completed and passing
- [ ] F0018 implementation NOT yet started (it starts after this plan is complete)

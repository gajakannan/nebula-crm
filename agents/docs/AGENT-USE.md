# Agent and Action Usage Guide

## Purpose

This guide shows how to invoke the framework in a fresh session:

- when to use an **action** versus a direct **agent**
- how to structure prompts so agents read the right artifacts
- what each agent typically reads, updates, and validates

Use this guide with:

- `agents/README.md`
- `agents/actions/README.md`
- `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`
- `agents/agent-map.yaml`

`agents/agent-map.yaml` is the authoritative action-to-agent wiring source. Action `.md` files remain the human-readable execution docs.

## Default Rule

Prefer **actions** for end-to-end workflows, approval gates, and multi-agent sequencing.

Use a direct **agent** prompt when:

- the work belongs to one role
- you are revising only one phase artifact set
- a gate requested targeted rework from a specific role
- you already know the exact feature and artifact scope

## Prompt Anatomy

Good direct-agent prompts usually include these parts:

1. Explicit activation
2. Target feature or scope path
3. Current status or problem statement
4. Ontology context when coverage exists
5. Required context files to read
6. Deliverables to create or update
7. Precedence rules if artifacts conflict
8. Validation commands and tracker updates

### Direct Agent Template

```text
Switch to <Agent Name> agent mode (agents/<role>/SKILL.md).

Work on <feature or scope> at <path>. The current state is <status>.

Read:
- <required file 1>
- <required file 2>
- <dependent artifacts>

Deliverables:
1. <deliverable 1>
2. <deliverable 2>

Constraints:
- <scope boundary>
- <precedence rule if artifacts conflict>

When done:
- update <status/tracker/docs files>
- run <validation command(s)>
```

### Ontology-Backed Addendum

When the target feature or story exists in
`planning-mds/knowledge-graph/feature-mappings.yaml`, add this block before the
raw file list:

```text
Ontology context:
- target: <feature or story id>
- load:
  - planning-mds/knowledge-graph/solution-ontology.yaml
  - planning-mds/knowledge-graph/canonical-nodes.yaml
  - planning-mds/knowledge-graph/feature-mappings.yaml
  - planning-mds/knowledge-graph/code-index.yaml (when code routing or reverse lookup is needed)
  - planning-mds/knowledge-graph/coverage-report.yaml (when coverage/freshness status matters)
- use the matching mapping entry as the first-pass routing context
- source precedence: raw feature/ADR/schema/API artifacts win over ontology mappings
- if ontology drift is found, repair the authoritative source first if needed,
  then repair the ontology mapping in the same change set
```

Use the ontology to resolve canonical workflow, workflow state, capability,
schema, ADR, and entity links. Do not treat it as a substitute for reading the linked raw
artifacts when details or verification matter.
Use `python3 scripts/kg/lookup.py <feature-or-story-id>` to materialize the
scope, or `python3 scripts/kg/lookup.py --file <repo-path>` for reverse lookup.
Use `python3 scripts/kg/validate.py --write-coverage-report` when the committed
coverage/freshness artifact needs to be refreshed after ontology changes.

### Action Template

```text
Run the <action> action defined in agents/actions/<action>.md.

Scope:
- <feature, repo, or release target>

Inputs:
- <required files or feature folders>

Execution notes:
- stop at required gates
- capture evidence per agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md
```

## Common Prompt Clauses

Use these clauses when they apply:

- `If ontology coverage exists, load the matching knowledge-graph entry before reading raw files.`
- `Use ontology mappings as compressed retrieval context only; source artifacts win on conflict.`
- `If shared solution semantics changed, repair ontology drift in the same change set.`
- `Read the full feature folder at planning-mds/features/F{NNNN}-{slug}.`
- `Where the feature-assembly-plan conflicts with raw story text, follow the feature-assembly-plan.`
- `Do not invent scope outside the current feature boundary.`
- `Update STATUS.md before concluding.`
- `If trackers changed, re-run tracker validation before declaring done.`
- `If commands or examples are unverified, mark them explicitly instead of implying they were run.`

## Ontology Ownership

- Architect owns the canonical shared layer in `planning-mds/knowledge-graph/`
  for entities, workflows, workflow states, capabilities, schemas, API
  contracts, and ADR links.
- Product Manager owns feature/story/persona mappings and keeps feature and
  story links current.
- Implementation agents should not silently redefine canonical solution
  semantics. When they discover drift, they should flag it and route the update
  back to the Architect or Product Manager unless explicitly acting in that
  role.
- Code Reviewer and Security should treat unresolved ontology drift as a
  cross-artifact consistency issue when shared semantics changed.

## Agent Quick Reference

| Agent | Use When | Read First | Usually Updates | Typical Validation |
|------|----------|------------|-----------------|--------------------|
| `product-manager` | refining PRDs, stories, personas, MVP/future scope, tracker sync | `planning-mds/BLUEPRINT.md`, feature folder, dependency PRDs, `TRACKER-GOVERNANCE.md` | feature `PRD.md`, stories, `README.md`, `STATUS.md`, trackers | `python3 agents/product-manager/scripts/validate-stories.py`, `python3 agents/product-manager/scripts/generate-story-index.py planning-mds/features/`, `python3 agents/product-manager/scripts/validate-trackers.py` |
| `architect` | data model, workflows, API contracts, ADRs, authorization, assembly plans | feature folder, `planning-mds/architecture/decisions/`, `planning-mds/architecture/SOLUTION-PATTERNS.md`, dependent PRDs | `planning-mds/architecture/**`, `planning-mds/api/*.yaml`, `planning-mds/schemas/*.json`, feature `feature-assembly-plan.md`, feature `STATUS.md` | `python3 agents/architect/scripts/validate-architecture.py planning-mds/BLUEPRINT.md`, `python3 agents/architect/scripts/validate-api-contract.py <api-file>`, tracker validation if trackers changed |
| `backend-developer` | implementing `engine/` changes from approved feature plans | feature folder, `feature-assembly-plan.md`, `planning-mds/api/`, `planning-mds/schemas/`, `planning-mds/architecture/SOLUTION-PATTERNS.md` | `engine/**`, feature `STATUS.md`, feature `GETTING-STARTED.md` | `sh agents/backend-developer/scripts/run-tests.sh --strict` or repo-standard backend test command |
| `frontend-developer` | implementing `experience/` screens, forms, API wiring, UX fixes | feature folder, `feature-assembly-plan.md`, screen specs, `planning-mds/api/`, `planning-mds/schemas/`, `agents/frontend-developer/references/ux-audit-ruleset.md` | `experience/**`, feature `STATUS.md`, feature `GETTING-STARTED.md` | `pnpm --dir experience lint`, `pnpm --dir experience lint:theme`, `pnpm --dir experience build`, `pnpm --dir experience test`, plus `pnpm --dir experience test:visual:theme` when theme/styling changed |
| `ai-engineer` | implementing `neuron/`, LLM integrations, MCP servers, prompts, agent workflows | feature folder, architecture docs, AI requirements, backend integration contracts | `neuron/**`, feature `STATUS.md`, feature `GETTING-STARTED.md`, `neuron/README.md` | `pytest tests/` and project-standard AI integration/evaluation commands |
| `quality-engineer` | test planning, automated tests, coverage checks, E2E, performance, accessibility | stories, acceptance criteria, `feature-assembly-plan.md`, changed code, quality strategy | `engine/tests/**`, `experience/tests/**`, `neuron/tests/**`, feature `STATUS.md` | tier-specific test commands plus coverage artifacts; require evidence-backed pass decisions |
| `devops` | Docker, compose, CI/CD, env wiring, deployment architecture, ops scripts | architecture docs, changed app code, deployment requirements | `Dockerfile`, `docker-compose*.yml`, `.github/workflows/**`, `scripts/**`, deployment docs, feature `STATUS.md` | repo-standard container, CI, and health-check commands |
| `code-reviewer` | code quality review, acceptance criteria coverage, architecture/pattern compliance | changed code, feature folder, `planning-mds/architecture/SOLUTION-PATTERNS.md`, review action doc | feature `STATUS.md` and review findings artifacts | `python3 agents/code-reviewer/scripts/check-code-quality.py <path>`, `sh agents/code-reviewer/scripts/check-lint.sh`, `sh agents/code-reviewer/scripts/check-test-coverage.sh --min 80 --auto` as applicable |
| `security` | threat modeling, auth/authz review, OWASP review, security findings | feature folder, architecture/security artifacts, changed code | `planning-mds/security/**`, feature `STATUS.md` | `python3 agents/security/scripts/security-audit.py planning-mds/security`, plus available scan wrappers in `agents/security/scripts/` |
| `technical-writer` | API docs, runbooks, READMEs, developer guides, operator docs | implemented code, planning artifacts, architecture docs, existing docs | `docs/**`, `README.md` files, operator docs | validate commands/paths/links or mark them unverified |
| `blogger` | devlogs, release notes, technical posts, retrospectives | completed work, ADRs, feature docs, evidence artifacts | `docs/blog/**` or `blog/**` | technical accuracy review, redaction review, audience/objective check |

## Direct-Agent Prompt Starters

Use these as starting lines in fresh sessions:

- `product-manager`: `Switch to Product Manager agent mode (agents/product-manager/SKILL.md).`
- `architect`: `Switch to Architect agent mode (agents/architect/SKILL.md).`
- `backend-developer`: `Switch to Backend Developer agent mode (agents/backend-developer/SKILL.md).`
- `frontend-developer`: `Switch to Frontend Developer agent mode (agents/frontend-developer/SKILL.md).`
- `ai-engineer`: `Switch to AI Engineer agent mode (agents/ai-engineer/SKILL.md).`
- `quality-engineer`: `Switch to Quality Engineer agent mode (agents/quality-engineer/SKILL.md).`
- `devops`: `Switch to DevOps agent mode (agents/devops/SKILL.md).`
- `code-reviewer`: `Switch to Code Reviewer agent mode (agents/code-reviewer/SKILL.md).`
- `security`: `Switch to Security agent mode (agents/security/SKILL.md).`
- `technical-writer`: `Switch to Technical Writer agent mode (agents/technical-writer/SKILL.md).`
- `blogger`: `Switch to Blogger agent mode (agents/blogger/SKILL.md).`

## Detailed Examples

### Product Manager Example

```text
Switch to Product Manager agent mode (agents/product-manager/SKILL.md).

Refine F{NNNN} <feature slug> workflow (planning-mds/features/F{NNNN}-{slug}).

The PRD currently has a high-level feature statement, scope, architecture hints,
and traceability but zero user stories, no persona references, no screen specs,
and no workflows. The feature is in Draft status.

Read:
- planning-mds/BLUEPRINT.md
- planning-mds/COMMERCIAL-PC-CRM-RELEASE-PLAN.md
- the PRDs for F0006 dependency features
- planning-mds/features/TRACKER-GOVERNANCE.md

Deliverables:
1. Refine the target feature PRD and sharpen scope boundaries.
2. Clarify MVP versus Future scope explicitly.
3. Add user stories with acceptance criteria.
4. Update README.md and STATUS.md in the feature folder.
5. Update REGISTRY.md, ROADMAP.md, STORY-INDEX.md, and BLUEPRINT.md as needed.

Constraints:
- Clarify what this feature owns versus what it delegates to dependency features.
- Determine applicable rules and document them within the appropriate stories.

When done:
- run `python3 agents/product-manager/scripts/validate-stories.py planning-mds/features/F{NNNN}-{slug}`
- run `python3 agents/product-manager/scripts/generate-story-index.py planning-mds/features/`
- run `python3 agents/product-manager/scripts/validate-trackers.py`
```

### Architect Example

```text
Switch to Architect agent mode (agents/architect/SKILL.md).

Design the technical solution for F{NNNN} <feature slug> at
planning-mds/features/F{NNNN}-{slug}.

The Product Manager has completed story breakdown. Read the full feature folder
for PRD, stories, and acceptance criteria.

Also read:
- ontology files in `planning-mds/knowledge-graph/` first when the target feature has coverage
- planning-mds/architecture/decisions/
- planning-mds/architecture/SOLUTION-PATTERNS.md
- dependent feature PRDs

Deliverables as applicable:
1. Data model
2. Workflow state machine
3. API contract
4. Authorization model
5. ADRs to create or update
6. `feature-assembly-plan.md` with implementation sequence, agent handoffs,
   integration checkpoints, and dependency stubs

Constraints:
- Cross-check data model, API, schemas, ERD, and Casbin alignment.
- Read the full feature folder. The feature-assembly-plan is the primary
  implementation spec.
- Where the assembly plan conflicts with raw story acceptance criteria, follow
  the assembly plan.

When done:
- update feature `STATUS.md`
- run `python3 agents/architect/scripts/validate-architecture.py planning-mds/BLUEPRINT.md`
- run `python3 agents/architect/scripts/validate-api-contract.py <api-file>` for each changed contract
- run `python3 agents/product-manager/scripts/validate-trackers.py` if planning trackers changed
```

### Backend Developer Example

```text
Switch to Backend Developer agent mode (agents/backend-developer/SKILL.md).

Implement the backend slice for <feature> in `engine/`.

Read:
- the full feature folder
- `planning-mds/features/<feature>/feature-assembly-plan.md`
- `planning-mds/architecture/SOLUTION-PATTERNS.md`
- relevant files in `planning-mds/api/` and `planning-mds/schemas/`

Deliverables:
1. Implement the planned `engine/` changes.
2. Add or update backend tests in `engine/tests/`.
3. Update feature `STATUS.md` and `GETTING-STARTED.md` with evidence and key paths.

Constraints:
- Follow the feature-assembly-plan when it conflicts with raw story text.
- Do not widen scope beyond the assigned backend slice.

When done:
- run `sh agents/backend-developer/scripts/run-tests.sh --strict`
```

## Action Quick Reference

| Action | Use When | Composes | Example Prompt |
|--------|----------|----------|----------------|
| `init` | starting a new repo or bootstrapping framework files | Product Manager | `Run the init action defined in agents/actions/init.md for this repository.` |
| `plan` | moving from idea to approved product and architecture specs | Product Manager -> Architect | `Run the plan action defined in agents/actions/plan.md for <feature or project>.` |
| `build` | implementing a larger approved scope across the stack | Architect -> implementation agents -> reviews | `Run the build action defined in agents/actions/build.md for the approved scope in <feature folders>.` |
| `feature` | shipping one vertical slice end to end | Architect -> implementation agents -> parallel reviews | `Run the feature action defined in agents/actions/feature.md for <feature folder>.` |
| `review` | getting code-quality and security review on changed work | Code Reviewer + Security | `Run the review action defined in agents/actions/review.md for the current diff and affected feature folders.` |
| `validate` | checking planning, architecture, and implementation alignment | Product Manager + Architect | `Run the validate action defined in agents/actions/validate.md for <feature or repo scope>.` |
| `test` | expanding or executing automated test coverage | Quality Engineer | `Run the test action defined in agents/actions/test.md for <feature or changed components>.` |
| `document` | producing READMEs, API docs, runbooks, or operator guides | Technical Writer | `Run the document action defined in agents/actions/document.md for <implemented scope>.` |
| `blog` | creating devlogs, retrospectives, and technical posts | Blogger | `Run the blog action defined in agents/actions/blog.md for <change set or release>.` |

## Action Usage Notes

- Use the action docs as the execution checklist.
- Capture evidence per `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`.
- Stop at explicit approval, review, quality, or tracker-sync gates.
- If you need a one-role revision after a gate, switch back to the responsible agent directly instead of re-running the whole action blindly.

## Recommended Operator Pattern

1. Start with an **action** if the work spans multiple roles.
2. Switch to a direct **agent** only for targeted, scoped follow-up.
3. Keep prompts explicit about files, outputs, and validation.
4. Treat `STATUS.md` and tracker updates as part of the work, not optional cleanup.
5. For implementation work, prefer the feature folder and `feature-assembly-plan.md` over ad hoc verbal summaries.

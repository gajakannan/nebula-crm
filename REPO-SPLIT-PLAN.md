# Repository Split Plan: nebula-crm → nebula-agents + nebula-insurance-crm

**Authored:** 2026-04-18
**Last revised:** 2026-04-19

---

## 1. Intent

`nebula-crm` currently serves two distinct purposes in a single repository:

1. A **reusable, tool-agnostic agent framework** (`agents/`, `blueprint-setup/`, framework root docs)
2. A **concrete insurance CRM product** (`planning-mds/`, `engine/`, `experience/`, `neuron/`, runtime scripts)

The boundary between these two is already explicit and documented (see `BOUNDARY-POLICY.md`). This plan formalizes the split into two independent repos with no submodules, no subtree embedding, and no build-time coupling.

`nebula-crm` is not being deleted, modified, or committed to at any point during the split. It remains the historical source AND the rollback backup. Both new repos are initialized from a read-only snapshot of its content, then evolved independently.

We want to split this to two different repos, `https://github.com/gajakannan/nebula-agents` for framework agents and `https://github.com/gajakannan/nebula-insurance-crm` for an independent insurance crm solution which was built using the nebula-agents.

---

## 2. The Critical Design Decision: Consumption Model

This is the most important thing the executing agent needs to understand before doing anything else.

### How the two repos are used together

**nebula-agents is the session working directory.** When a developer or AI tool (Claude Code, Codex, Cursor, Aider, or any other) starts a session to work on the insurance CRM — or any other product — they open that session rooted in `nebula-agents`. The agent reads its role definitions, action protocols, and templates from `agents/` in that working directory. It then performs all implementation work in the sibling product repo (`../nebula-insurance-crm/` or any other `../product-repo/`).

**The contract is plain markdown, not a tool-specific config.** Because the framework guidance lives in ordinary `.md` files, any AI tool can consume it. There is no dependency on `CLAUDE.md`, `.cursorrules`, or any vendor-specific mechanism. Those files are ergonomic shortcuts for specific tools — they auto-load context that a human or tool would otherwise provide manually. They are not the contract.

**There is no build-time or runtime coupling.** nebula-insurance-crm does not import, reference, or depend on nebula-agents at compile time. The connection is entirely process-level: the agent running the session knows to look left for framework guidance and right for product artifacts.

### Expected workspace layout

```
WORKSPACE_ROOT/           ← operator-chosen parent dir; must be outside `nebula-crm/`
  nebula-agents/          ← session working directory for all agent work
  nebula-insurance-crm/   ← implementation target (or any other product repo)
```

Both repos sit as siblings beneath `WORKSPACE_ROOT`. The executing agent references the product repo via relative path `../nebula-insurance-crm/` from `nebula-agents/`. `WORKSPACE_ROOT` must not equal, or live inside, the source repo root. This layout must be documented in nebula-agents' README so any operator cloning the framework knows the expected setup.

### Path indirection: `{PRODUCT_ROOT}`

The framework (`agents/**`) must not hardcode any product-repo directory name. All references to product artifacts use the placeholder `{PRODUCT_ROOT}`, resolved at session start.

- **Resolution mechanism (concrete):** at session start the agent resolves `{PRODUCT_ROOT}` in this order:
  1. Environment variable `NEBULA_PRODUCT_ROOT`, if set
  2. Operator-provided value at session start ("the product repo is at X")
  3. Default fallback `../nebula-insurance-crm`
  The resolved absolute path is echoed back as the first agent turn's output before any shell command runs (see also Section 10 risk row).
- **Where defined:** `agents/docs/AGENT-USE.md` under a new "Session Setup" section; formal contract in `CONSUMER-CONTRACT.md` (see Section 5.2)
- **What it prefixes:** every reference from `agents/**` to product-owned paths. At baseline that includes `scripts/kg/…`, `planning-mds/…`, `engine/…`, `experience/…`, and `neuron/…`. `bruno/` currently has zero hits under `agents/**`, but if referenced later it follows the same convention.

Examples:
- `python3 scripts/kg/lookup.py <feature-id>` → `python3 {PRODUCT_ROOT}/scripts/kg/lookup.py <feature-id>`
- `pnpm --dir experience lint` → `pnpm --dir {PRODUCT_ROOT}/experience lint`
- `engine/**` write scope → `{PRODUCT_ROOT}/engine/**`

This is not cosmetic. Today the framework assumes its session-root IS the product repo. Splitting reverses that — the session root is `nebula-agents`, so every bare product path, or command that implicitly assumes a product-layer working directory, is broken until rewritten. Section 4 enumerates the rewrite scope.

### Discovery of product-specific concrete values

Some framework files currently bake in concrete product values (C# root namespace `Nebula.*`, API filename `nebula-api.yaml`). These should not be replaced with a single placeholder like `{AppRoot}` and then forgotten — the agent discovers them at session time from the product repo:

- **Tech stack** → `{PRODUCT_ROOT}/planning-mds/BLUEPRINT.md`
- **Entity-to-file bindings** → `{PRODUCT_ROOT}/planning-mds/knowledge-graph/code-index.yaml` and `canonical-nodes.yaml`
- **API spec location** → product repo declares its OpenAPI path in `BLUEPRINT.md`; agents do not assume a filename
- **Implementation layer roots** → when framework docs need concrete backend/frontend/AI paths, they use `{PRODUCT_ROOT}/engine/…`, `{PRODUCT_ROOT}/experience/…`, or `{PRODUCT_ROOT}/neuron/…` (or a product-declared equivalent from BLUEPRINT/code-index), never framework-root-relative paths

Framework templates that currently pre-fill concrete values should be reduced to shape-only skeletons with a leading "resolve real paths from the knowledge graph" instruction. See Section 4.3.

### Tool-Specific Config Files Are Optional

- There must be **no required dependency** on `.claude/settings.json`, `CLAUDE.md`, `AGENTS.md`, `.cursorrules`, or any other tool-specific bootstrap file in either repo.
- If any such files are added later for operator convenience, they are wrappers only. They are not part of the framework contract and they must not be required for the split to function.
- The only durable contract is the repository content itself: markdown guidance in `nebula-agents`, product artifacts in `nebula-insurance-crm`, and the sibling workspace layout documented above.

---

## 3. Repository Boundaries

### nebula-agents contains

Everything generic and reusable regardless of domain:

| Source path (in nebula-crm) | Destination | Notes |
|---|---|---|
| `agents/` | `agents/` | Entire directory — roles, actions, templates, scripts, docs |
| `blueprint-setup/` | `blueprint-setup/` | Framework bootstrap guidance |
| `docker/agent-builder/` | `docker/agent-builder/` | Builder container entrypoint |
| `Dockerfile` | `Dockerfile` | Builder image (framework runtime, not app runtime). **Rewrite in Step 5** to remove any dependency on product-owned root `scripts/` content; install framework Python deps from `agents/scripts/requirements.txt` instead. |
| `docker-compose.agent-builder.yml` | `docker-compose.agent-builder.yml` | Builder compose. Keep with the framework; verify only that it still targets the rewritten framework `Dockerfile`. |
| `BOUNDARY-POLICY.md` | `BOUNDARY-POLICY.md` | Framework policy doc |
| `CONTRIBUTING.md` | `CONTRIBUTING.md` | Framework contribution guide |
| `LICENSE` | `LICENSE` | Copy verbatim |
| `README.md` | `README.md` | **Rewrite** — see Section 5 |
| `lifecycle-stage.yaml` | `lifecycle-stage.yaml` | **New file** — framework-local stage config for nebula-agents only (do not copy the CRM one) |
| `.github/workflows/ci-gates.yml` | `.github/workflows/ci-gates.yml` | **Phase B only** — framework-level gates only (boundary_genericness, skill_regression) |

**Does NOT contain:**
- Any insurance/CRM planning artifacts
- Application source code (`engine/`, `experience/`, `neuron/`)
- CRM runtime scripts and product KG tooling (`scripts/deploy*.sh`, `scripts/run-*.sh`, `scripts/rollback.sh`, `scripts/check-policy-parity.py`, most of `scripts/kg/`; see the script-classification table below)
- CRM docker/compose files (`docker-compose.yml`, `docker-compose.qe.yml`, `docker/authentik/`, `docker/postgres/`)
- CRM API test collections (`bruno/`)
- `lifecycle-stage.yaml` from nebula-crm (that file is CRM-instance state). The generic `agents/templates/lifecycle-stage-template.yaml` already exists in nebula-crm and carries over automatically via the Step 2 `git archive HEAD agents` — see §5.4. The framework-local root `lifecycle-stage.yaml` is a separate authored file, per §5.3.
- `pyproject.toml` (CRM-scoped pytest config)
- Tool-specific bootstrap files: `.claude/`, `.codex`, `CLAUDE.md`, `AGENTS.md`, `.cursorrules` — none are copied into nebula-agents (see the hidden-files table for details)

**Framework dev/test dependencies:** because `pyproject.toml` does not move, nebula-agents needs its own minimal dev-dep declaration so framework validators and tests can run in CI and developer shells. Ensure `agents/scripts/requirements.txt` in nebula-agents contains at minimum `pyyaml` and `pytest`. CI and any local test invocation install via `python3 -m pip install -r agents/scripts/requirements.txt`. Do not author a `pyproject.toml` for the framework — a flat requirements file is sufficient and avoids implying the framework ships as a Python package.

### nebula-insurance-crm contains

Everything specific to the insurance CRM product:

| Source path (in nebula-crm) | Destination | Notes |
|---|---|---|
| `planning-mds/` | `planning-mds/` | All current product planning artifacts |
| `engine/` | `engine/` | C# .NET backend |
| `experience/` | `experience/` | React frontend |
| `neuron/` | `neuron/` | AI runtime layer |
| `docker/authentik/` | `docker/authentik/` | Auth runtime infra |
| `docker/postgres/` | `docker/postgres/` | Database runtime infra |
| `docker-compose.yml` | `docker-compose.yml` | App runtime compose |
| `docker-compose.qe.yml` | `docker-compose.qe.yml` | QE compose |
| `scripts/` | `scripts/` | Product deploy/runtime/QA scripts and product KG tooling. See the script-classification table below for the framework-coupled exceptions. |
| `bruno/` | `bruno/` | API test collections |
| `pyproject.toml` | `pyproject.toml` | Pytest config |
| `lifecycle-stage.yaml` | **Do not copy** | Authored fresh in Step 7 as a product-local file with only product-local gates (e.g., `solution_contract`, `knowledge_graph_sync`, `frontend_quality`). The original defines 10 gates and wires 7 of them to framework-owned validators (under `agents/**`); it is cheaper to rewrite than to strip. |
| `LICENSE` | `LICENSE` | Copy verbatim |
| `README.md` | `README.md` | **Rewrite** — see Section 6 |
| `.github/workflows/` (all except `ci-gates.yml`) | `.github/workflows/` | **Phase B only** — CRM CI workflows after all local `agents/**` references are removed or replaced with product-local checks |
| `.github/workflows/ci-gates.yml` | `.github/workflows/ci-gates.yml` (slimmed, **product-local**) | **Phase B only** — author a slimmed product-local version that runs the new product-local lifecycle runner against the freshly-authored CRM `lifecycle-stage.yaml`. The framework version of the same filename is materialized separately into `nebula-agents` (Step 10) and is unrelated to this product copy. |

**Does NOT contain:**
- `agents/` directory
- `blueprint-setup/` directory
- Builder `Dockerfile` or `docker-compose.agent-builder.yml`
- `BOUNDARY-POLICY.md` or `CONTRIBUTING.md` (framework-owned; reference by URL instead)
- Tool-specific bootstrap files: `.claude/`, `.codex`, `CLAUDE.md`, `AGENTS.md`, `.cursorrules` — none are copied into nebula-insurance-crm (see the hidden-files table for details)
- Any executable local dependency on `nebula-agents` paths, framework validators, or tool-specific framework config files in live operator-facing product surfaces

### Script classification inside `scripts/`

Not every file under the root `scripts/` directory has the same ownership boundary. The split should classify them by behavior, not by folder name alone.

| Source artifact | Destination | Handling |
|---|---|---|
| `scripts/deploy*.sh`, `scripts/deploy/**`, `scripts/run-*.sh`, `scripts/rollback.sh`, `scripts/dev-reset.sh`, `scripts/smoke-test.sh`, `scripts/check-policy-parity.py`, plus any other root-level `scripts/*` file not matched by the rows below | `nebula-insurance-crm` | Product runtime / QE / release operations. These scripts exist to run or validate the CRM application. Step 3's `git archive HEAD scripts` captures the whole `scripts/` tree wholesale; the framework-coupled exceptions in the rows below are pruned after the archive via explicit `rm`. |
| `scripts/kg/lookup.py`, `validate.py`, `hint.py`, `workstate.py`, `blast.py`, `cochange.py`, `eval.py`, `pagerank.py`, `telemetry_rotate.py`, `kg_common.py`, `scripts/kg/tests/test_lookup_tier.py` | `nebula-insurance-crm` | Product knowledge-graph tooling. These scripts read `planning-mds/knowledge-graph/*.yaml`, product source paths, and product runtime state. |
| `scripts/kg/validate_templates.py`, `scripts/kg/tests/test_validate_templates.py` | `nebula-agents` | Framework contract validator. Today it defaults to `agents/actions/plan.md`, `agents/actions/feature.md`, and `agents/templates/prompts/`, so it belongs with the framework, not the product. Move it into a framework-local path during the split. |
| `scripts/kg/pretool_hook.py` | **Drop** | Claude Code-specific bridge to `hint.py`, explicitly configured via `.claude/settings.json`. Not part of the durable repo-to-repo contract. |

### Validation ownership

The split treats validation as two scopes, not one:

- **Framework-owned validations** live in `nebula-agents` and run from the framework session root against `{PRODUCT_ROOT}` when product context is needed. This includes `boundary_genericness`, `skill_regression`, the moved `validate_templates.py`, and planning-governance validators such as `agents/product-manager/scripts/validate-stories.py`, `generate-story-index.py`, and `validate-trackers.py`.
- **Product-local validations** live in `nebula-insurance-crm` and must be runnable with no `agents/**` directory present. Phase A product-local set: `scripts/kg/validate.py`, `planning-mds/testing/validate-nebula-api-contract.py`, `planning-mds/testing/validate-frontend-quality-gate.py`. Phase B rehomes `api_contract`, `infra_strict`, `security_planning_strict`, and any frontend UX evidence validator that CI still needs.
- **Rule of thumb:** live product docs/workflows may mention framework-owned validation conceptually, but must not invoke it via local `agents/**` paths. Historical evidence and archived feature records may retain their original command traces as provenance.

### Hidden files, dotfiles, and ignored state

The two source-path tables above enumerate tracked content. The source repo also contains root-level dotfiles and gitignored working state. Because Steps 2/3 use `git archive` (tracked content only), most of these never enter either new repo by default — but each row below documents the intent so the few that do need explicit authoring (`.gitignore`, `.dockerignore`, `.env.example`) are routed correctly.

| Source artifact | Destination | Handling |
|---|---|---|
| `.gitignore` | **Split — author one per repo** | The current file covers both .NET/Node (product) and Python (framework). For nebula-agents, author a trimmed version covering Python, IDE, OS, and the framework builder only (Step 2 inlines this). For nebula-insurance-crm, archive the source `.gitignore` and trim framework-only agent patterns, but **keep generic Python artifact ignores** (`__pycache__/`, `*.pyc`, `.venv/`, `.pytest_cache/`) because the CRM repo still owns Python tooling under `scripts/`, `planning-mds/testing/`, and the future `neuron/` runtime. Each is the FIRST file authored in its repo so subsequent local edits don't stage ignored state by accident. |
| `.dockerignore` | nebula-insurance-crm | Tuned to the CRM build context (inclusion/exclusion rules for `engine/`, `planning-mds/security/policies/`, `experience/`, `neuron/`). Does not apply to the framework builder `Dockerfile`. If nebula-agents needs one later, author it fresh. |
| `.env.example` | nebula-insurance-crm | Contains CRM-specific variables (`DATABASE_URL`, `AUTHENTIK_*`, `ANTHROPIC_API_KEY` for the neuron layer). Framework has no runtime secrets. |
| `.env` | **Neither — do not commit to either repo** | Real development secrets. Already gitignored at source, so `git archive` will not include it; the destination `.gitignore` is belt-and-suspenders for any post-snapshot local edits. |
| `.kg-state/` | **Neither (ignored state)** | Runtime session state for `scripts/kg/workstate.py`; gitignored at source. Excluded by `git archive`. The CRM repo's `.gitignore` must continue to ignore `.kg-state/`. |
| `security-reports/` | **Neither (ignored state)** | Gitignored output directory. Empty at baseline; excluded by `git archive`. CRM repo's `.gitignore` continues to ignore it. |
| `.codex` | **Drop** | 0-byte Codex CLI marker. Per Section 2, tool-specific bootstrap files are not part of the framework contract. Steps 2/3 do not archive it. |
| `.claude/` | **Drop** | Claude Code-specific settings directory scoped to the CRM session. Not required by the contract. Steps 2/3 do not archive it. If an operator wants one in nebula-agents later, author it fresh. |
| `.pytest_cache/`, `.pnpm-store/`, `.vite/`, `.sonarqube/` | **Neither (ignored state)** | Build/cache artifacts, already gitignored. Excluded by `git archive`; the destination `.gitignore` continues to ignore them. |

**Snapshot method.** Section 9 Steps 2 and 3 use `git archive` from the operator-selected local source snapshot, not `cp -r`. `git archive` emits only content tracked at that snapshot — matching exactly what the chosen local source tree records — so untracked working state (`.env`, `.kg-state/`, `.pytest_cache/`, build caches, IDE files) cannot leak into either new repo by accident. The destination `.gitignore` is still authored as the first file in each new repo before any other content lands, so subsequent local edits don't accidentally stage ignored state either.

`cp -r` is rejected as the snapshot method: it would copy ignored and untracked files, requiring the destination `.gitignore` to be perfect to filter them at staging time. `git archive` makes the filter unconditional.

---

## 4. Genericness Cleanup Required in nebula-agents

Before nebula-agents can stand alone as a generic framework, three distinct categories of product coupling must be cleaned. This cleanup happens **inside nebula-agents after the copy** — do not modify nebula-crm.

The three categories are independent and need different treatment. Do not conflate them.

### 4.1 Path indirection — rewrite to `{PRODUCT_ROOT}` (largest scope)

This is the biggest cleanup and the one not addressed by the existing genericness validator. The framework currently hardcodes product-repo paths and layer roots (`scripts/kg/…`, `planning-mds/…`, `engine/…`, `experience/…`, `neuron/…`) as if the session root were the product repo. After the split the session root is `nebula-agents`, so every such reference must be rewritten to be product-root-aware (see Section 2).

**Scope (confirmed by grep at baseline, 2026-04-19):**

| Pattern | Occurrences | Files | Notes |
|---|---|---|---|
| `scripts/kg/` | 104 | 20 files under `agents/**` | KG commands and workstate/hint/blast guidance |
| `planning-mds/` | 744 | 84 files under `agents/**` | Planning inputs, validators, templates, and orchestration docs |
| `engine/` | 94 | 23 files under `agents/**` | Backend role docs, agent map write scopes, architecture refs, templates |
| `experience/` | 89 | 29 files under `agents/**` | Frontend role docs, command tables, architecture refs, templates |
| `neuron/` | 133 | 22 files under `agents/**` | AI role docs, agent map scopes, AI architecture refs, deployment templates |
| `bruno/` | 0 | 0 files under `agents/**` | No baseline hits today; future references still use `{PRODUCT_ROOT}` |

Note: the rewrite is not just planning/KG indirection. Framework docs also hardcode implementation layer roots and command contexts throughout role guides, `agents/agent-map.yaml`, and framework references. Budget Step 5 accordingly.

**Fix:** mechanical rewrite in `agents/**`.
- `scripts/kg/…` → `{PRODUCT_ROOT}/scripts/kg/…`
- `planning-mds/…` → `{PRODUCT_ROOT}/planning-mds/…`
- `engine/…` → `{PRODUCT_ROOT}/engine/…`
- `experience/…` → `{PRODUCT_ROOT}/experience/…`
- `neuron/…` → `{PRODUCT_ROOT}/neuron/…`
- `bruno/…` → `{PRODUCT_ROOT}/bruno/…` if/when introduced later

Commands that implicitly assume the current working directory is inside the product repo or one of its layers must be rewritten too. Prefer path-explicit forms from the framework session root (`pnpm --dir {PRODUCT_ROOT}/experience lint`, `pytest {PRODUCT_ROOT}/neuron/tests/`, `dotnet test {PRODUCT_ROOT}/engine/...`) over instructions that quietly assume a prior `cd` into the product repo.

Framework Python scripts that default to product-relative paths (for example `agents/product-manager/scripts/validate-trackers.py`, `validate-stories.py`, `generate-story-index.py`, `agents/architect/scripts/validate-architecture.py`, and `agents/architect/scripts/validate-api-contract.py`) must adopt a single uniform resolution convention so they behave identically:

1. **`--product-root <path>` CLI flag** takes precedence when supplied.
2. Otherwise, environment variable **`NEBULA_PRODUCT_ROOT`** (the same variable §2 defines for session-level `{PRODUCT_ROOT}` resolution).
3. Otherwise, fall back to **`../nebula-insurance-crm`** relative to the framework session root (matches §2 default).

The resolved root is prefixed onto each script's product-relative defaults (for example `<root>/planning-mds/features`). Echo the resolved root on script start so it's visible in CI logs. Do not invent per-script env vars or per-script flags — uniformity matters more than granularity here.

**Scripts `scripts/kg/` stays in nebula-insurance-crm.** The framework references it via `{PRODUCT_ROOT}` prefix. It is product-owned runtime state (it reads `planning-mds/knowledge-graph/*.yaml`), so moving it to nebula-agents would re-create the same coupling.

### 4.2 Brand vs namespace — what stays, what gets rewritten

The word "Nebula" appears in two very different roles in the codebase. Treat them differently.

**Keep (framework/brand identity, not product structure):**

- `author: "Nebula Framework Team"` in every `agents/*/SKILL.md` frontmatter — framework team attribution.
- "Nebula" mentioned by name as the brand of the framework or as the name of the reference product in framework docs (e.g., "this framework was originally extracted from Nebula CRM").

**Rewrite (product-structural references that break the moment the framework is reused elsewhere):**

| File | Leak | Fix |
|---|---|---|
| `agents/backend-developer/SKILL.md` (line 31) | "Follow Nebula API profile for route patterns…" | Replace with "Follow your project's API profile (see `{PRODUCT_ROOT}/planning-mds/BLUEPRINT.md` and `agents/architect/references/api-design-guide.md`)" |
| `agents/actions/feature.md` (line 68) | Hardcodes `planning-mds/api/nebula-api.yaml` as the OpenAPI filename | Use `{PRODUCT_ROOT}/planning-mds/api/<openapi-spec>.yaml` and point to BLUEPRINT.md for the canonical filename |
| `agents/actions/plan.md` (line 62) | Same hardcoded filename | Same fix |
| `agents/templates/prompts/feature-operator-friendly.md` | Hardcodes `nebula-api.yaml` | Same fix |
| `agents/templates/prompts/feature-automation-safe.md` | Hardcodes `nebula-api.yaml` | Same fix |
| `agents/templates/prompts/plan-operator-friendly.md` | Hardcodes `nebula-api.yaml` | Same fix |
| `agents/templates/prompts/plan-automation-safe.md` | Hardcodes `nebula-api.yaml` | Same fix |
| `agents/docs/ONBOARDING.md` (line 11) | References `nebula-agent-builder` Docker image tag | Leave as-is — this is the framework's own builder container; "nebula-agent-builder" names the *framework* tool, not the product |
| `agents/docs/FORK-AND-BUILD-APP.md` (lines 38, 50, 77) | References "Nebula CRM" as reference product | Keep mentions framed as reference product ("you can keep Nebula CRM as a working reference"); no change needed as long as the surrounding language makes clear it is one example, not a requirement |
| `agents/architect/references/architecture-best-practices.md` (line 994), `agents/architect/references/api-design-guide.md` (line 806) | Changelog entries mention "Nebula bounded contexts" / "Nebula-specific recommendations" | Leave as historical changelog entries — these are statements of past work, not current guidance. **Before leaving, verify the entries are still inside a section titled `Changelog` or `Version History`. If they have migrated into current guidance prose, they are leaks, not history, and must be rewritten.** |

Short rule: "Nebula" as a proper noun = stays. "Nebula." as a namespace/filename/path stand-in for "this product's structure" = rewrite.

### 4.3 Template generalization — `Nebula.Domain/` and friends

`agents/templates/feature-assembly-plan-template.md` lines 32–39 pre-fill concrete C# namespaces across all eight rows:

```
| `Nebula.Domain/Entities/{Entity}.cs`                           | … | Rewrite |
| `Nebula.Application/DTOs/{Dto}.cs`                             | … | Rewrite |
| `Nebula.Application/Services/{Service}.cs`                     | … | Expand  |
| `Nebula.Application/Validators/{Validator}.cs`                 | … | Rewrite |
| `Nebula.Application/Interfaces/I{Repository}.cs`               | … | Expand  |
| `Nebula.Infrastructure/Repositories/{Repository}.cs`           | … | Expand  |
| `Nebula.Infrastructure/Persistence/Configurations/{Config}.cs` | … | Rewrite |
| `Nebula.Api/Endpoints/{Endpoints}.cs`                          | … | Rewrite |
```

All eight rows must be generalized — not just the four shown in earlier drafts of this plan. These namespaces belong to the CRM's C# solution; a fork that builds a different product will not have `Nebula.Domain/`, `Nebula.Application/`, or `Nebula.Infrastructure/`.

**Fix:** reduce the template to a shape-only skeleton and add a leading instruction telling the agent to resolve real paths from the knowledge graph, which is the authoritative source of truth for entity-to-file bindings:

```
## Existing Code (Must Be Modified)

> Resolve real paths from {PRODUCT_ROOT}/planning-mds/knowledge-graph/code-index.yaml
> and canonical-nodes.yaml. The rows below are shape-only — replace placeholders
> with the concrete paths returned by `{PRODUCT_ROOT}/scripts/kg/lookup.py <feature-id>`.

| File | Current State | F{NNNN} Change |
|------|---------------|----------------|
| `{domain-project}/Entities/{Entity}.cs`                              | {field count}  | Rewrite — {summary} |
| `{application-project}/DTOs/{Dto}.cs`                                | {param count}  | Rewrite — {summary} |
| `{application-project}/Services/{Service}.cs`                        | {methods}      | Expand  — {summary} |
| `{application-project}/Validators/{Validator}.cs`                    | {rules}        | Rewrite — {summary} |
| `{application-project}/Interfaces/I{Repository}.cs`                  | {methods}      | Expand  — {summary} |
| `{infrastructure-project}/Repositories/{Repository}.cs`              | {methods}      | Expand  — {summary} |
| `{infrastructure-project}/Persistence/Configurations/{Config}.cs`    | {schema}       | Rewrite — {summary} |
| `{api-project}/Endpoints/{Endpoints}.cs`                             | {route count}  | Rewrite — {summary} |
```

On the insurance CRM, `{domain-project}` resolves to `engine/src/Nebula.Domain`, `{application-project}` to `engine/src/Nebula.Application`, `{infrastructure-project}` to `engine/src/Nebula.Infrastructure`, `{api-project}` to `engine/src/Nebula.Api` — values the agent reads from `code-index.yaml`. On any other product, they resolve to whatever that product's `code-index.yaml` says.

### 4.4 Domain-term leaks (caught by the existing validator)

The existing genericness validator catches only the CRM-domain-term category. At baseline it reports 3 hits, all of the blocked term `renewal`:

- `agents/architect/SKILL.md` (line 143) — example text uses `renewal` as a concrete canonical-node illustration
- `agents/backend-developer/SKILL.md` (line 147) — `Renewal.cs` / `entity:renewal` as concrete example
- `agents/frontend-developer/SKILL.md` (line 183) — `renewals/**` / `renewal-pipeline-list` as concrete examples

**Fix:** replace with neutral example terminology (e.g., `entity:order` / `orders/**`, matching the "Standard Example Entities" convention already established in `BOUNDARY-POLICY.md`).

### 4.5 Framework-internal self-reference cleanup

After the split, some framework docs may still point at impossible framework-local paths even when they no longer reference product-owned files. These are not caught by the product-path greps from §4.1 because the bad prefix is still `agents/`.

**Baseline example (confirmed):**
- `agents/templates/solution-patterns-template.md`
- `agents/architect/references/architecture-best-practices.md`
- `agents/architect/references/api-design-guide.md`
- `agents/architect/references/data-modeling-guide.md`
- `agents/architect/references/authorization-patterns.md`

All of the files above reference `agents/BOUNDARY-POLICY.md`, but post-split the file lives at framework repo root as `BOUNDARY-POLICY.md`.

**Fix:** Step 5 must rewrite stale framework-owned self-references, not just product-owned paths. At minimum:
- `agents/BOUNDARY-POLICY.md` → `BOUNDARY-POLICY.md`
- Any reference to framework-root-owned files (`README.md`, `CONTRIBUTING.md`, `CHANGELOG.md`, `CONSUMER-CONTRACT.md`, `lifecycle-stage.yaml`, `Dockerfile`, `docker-compose.agent-builder.yml`) must remain root-relative, never `agents/`-prefixed
- After the grep gates pass, do one short manual read pass over the rewritten framework root docs/templates to verify referenced framework-owned files still exist at the stated paths

### How the validator fits in — and where it falls short

```bash
# From nebula-agents root
python3 agents/scripts/validate-genericness.py
```

The validator reads blocked terms from `planning-mds/domain/glossary.md`. Its denylist is exactly the nine CRM domain terms: Broker, MGA, Underwriter, Underwriting, Premium, Claim, Insured, Submission, Renewal.

**The validator does NOT catch:**
- Path indirection leaks from Section 4.1 (no `scripts/kg/`, `planning-mds/`, `engine/`, `experience/`, `neuron/`, or `bruno/` in its denylist)
- Brand/namespace/filename leaks from Section 4.2 (no "Nebula" or `nebula-api.yaml` in its denylist)
- Template concrete-value leaks from Section 4.3 (no `Nebula.Domain/` in its denylist)
- Implicit working-directory assumptions in command examples (for example `pytest tests/`) — these require doc review, not just path grep

So "the validator passes" is a necessary but not sufficient condition for the split to be complete. Sections 4.1–4.3 require their own confirmation by grep:

```bash
# All must return zero hits (outside intentional retention sites) before declaring the split complete.
# The `[^{/]` class excludes both `{` (to skip `{PRODUCT_ROOT}`) and `/` (to skip the
# `{PRODUCT_ROOT}/planning-mds/...` case where the char before `planning-mds/` is `/`).
grep -rn -E '(^|[^{/])scripts/kg/'   agents/
grep -rn -E '(^|[^{/])planning-mds/' agents/
grep -rn -E '(^|[^{/])engine/'       agents/
grep -rn -E '(^|[^{/])experience/'   agents/
grep -rn -E '(^|[^{/])neuron/'       agents/
grep -rn -E '(^|[^{/])bruno/'        agents/
grep -rn 'Nebula\.[A-Z]' agents/
grep -rn 'nebula-api\.yaml' agents/
```

For running the validator itself, the split adopts a single approach in Phase A:

- Embed the blocked-terms list directly into `validate-genericness.py` as a static fallback. Extract the `Genericness-Blocked Terms` section from `planning-mds/domain/glossary.md` (in the nebula-crm snapshot) and inline it into the script. This makes nebula-agents fully self-contained — the validator runs with zero arguments and zero sibling-repo dependency, including in CI.
- The `--glossary <path>` CLI flag is preserved as an override so an integrator can point the validator at their own product's glossary if they want to extend the denylist; absent the flag, the embedded list is authoritative.
- **Drift policy (explicit):** the embedded list is a snapshot, not a live mirror. It will not auto-pick up new terms added to any downstream product glossary. Two acceptable evolutions, in order of preference:
  1. **Treat the embedded list as terminal for nebula-agents.** The framework should grow toward zero CRM-term-specificity over time; the nine-term list is a transitional safeguard while §4.1–4.3 cleanups bed in. New blocked terms in any product glossary are that product's concern, surfaced via `--glossary <path>`.
  2. **Periodic re-sync.** If the nine-term list itself needs to change (e.g., the framework decides to broaden or narrow what counts as a CRM-leak), update the embedded constant in a deliberate framework-side PR. Do not auto-sync from a sibling product repo.
  Either way, downstream products that want to extend enforcement always pass `--glossary` explicitly.

This eliminates the prior "Option 1 short-term, Option 2 later" split and means Step 5 validation (and Phase B CI) can run on nebula-agents alone.

---

## 5. nebula-agents: New and Rewritten Documents

### 5.1 README.md (rewrite from scratch)

The current README serves both purposes. Rewrite it as a pure framework product README with these sections:

- **What it is:** A tool-agnostic, orchestrator-agnostic agent-driven development framework
- **What it owns:** Role definitions, action protocols, templates, genericness enforcement, bootstrap guidance, builder runtime
- **What it does not own:** Domain planning, application code, product runtime infrastructure, deployment scripts
- **How downstream products consume it:** The workspace layout pattern — open session in nebula-agents, implement in sibling product repo. Plain markdown contract works with any AI tool.
- **Quick start:** Clone `nebula-agents` next to a product repo, resolve `{PRODUCT_ROOT}` (or set `NEBULA_PRODUCT_ROOT`), read `CONSUMER-CONTRACT.md`, then use `agents/docs/AGENT-USE.md` / the rewritten `agents/docs/FORK-AND-BUILD-APP.md`
- **Available actions:** Link to `agents/actions/README.md`
- **Framework architecture diagram:** Preserve the existing diagram but remove any CRM-specific references

### 5.2 CONSUMER-CONTRACT.md (new file at root)

This document is the formal interface between nebula-agents and any downstream product repo. Contents:

- **`{PRODUCT_ROOT}` path-indirection convention:** every framework reference to product-owned paths (`scripts/kg/…`, `planning-mds/…`, `engine/…`, `experience/…`, `neuron/…`) is prefixed with `{PRODUCT_ROOT}`. The placeholder is resolved at session start; default value is `../<product-repo-name>`. See also `agents/docs/AGENT-USE.md` → Session Setup.
- **Required planning structure** that the framework expects (all relative to `{PRODUCT_ROOT}`): `planning-mds/BLUEPRINT.md`, `planning-mds/domain/glossary.md`, `planning-mds/api/<api>.yaml`, `planning-mds/knowledge-graph/canonical-nodes.yaml`, `planning-mds/knowledge-graph/code-index.yaml`
- **Implementation layer path convention:** backend/frontend/AI paths are always referenced as product-owned paths under `{PRODUCT_ROOT}` (or a product-declared equivalent), never as framework-root-relative paths
- **Discovery convention for product-specific concretes:** framework agents do NOT hardcode product namespaces, API filenames, or entity names. They read BLUEPRINT.md for tech stack and knowledge-graph/code-index.yaml + canonical-nodes.yaml for real file bindings. Templates are shape-only skeletons; concrete values come from the product's knowledge graph.
- **Required action artifact paths** for each action (plan, build, feature, review, etc.)
- **Lifecycle gate contract:** what `lifecycle-stage.yaml` must contain, what stages are valid, what gates each stage requires
- **Validation ownership model:** which validations stay framework-owned in nebula-agents vs which validations must be product-local in nebula-insurance-crm
- **Genericness contract:** no framework files may reference product-specific terms; enforcement via `validate-genericness.py` (domain terms) plus grep-based checks (path/brand/namespace leaks, per Section 4)
- **Workspace layout convention:** `WORKSPACE_ROOT/nebula-agents/` and `WORKSPACE_ROOT/<product-repo>/` as siblings, with `WORKSPACE_ROOT` outside `SOURCE_REPO_ROOT`
- **API reference path convention:** product repos own their OpenAPI spec; framework agents reference it via path provided at session start, not hardcoded
- **Versioning policy:** how downstream products pin to a nebula-agents version (git tag, commit ref)
- **Stack adaptation:** how to replace stack-specific reference guides (existing `TECH-STACK-ADAPTATION.md` can be referenced or merged here)

### 5.3 lifecycle-stage.yaml (new file at root)

Create a framework-local lifecycle file for `nebula-agents` itself:

- `current_stage`: `framework-bootstrap`
- Required gates: framework-only gates (`boundary_genericness`, `skill_regression`)
- No CRM/product-specific gates
- Used only by `nebula-agents` local validation and CI

This file is distinct from the CRM lifecycle file in the source repo. Do not copy the CRM version into `nebula-agents`.

### 5.4 lifecycle-stage-template.yaml (no action — carried by Step 2 snapshot)

`agents/templates/lifecycle-stage-template.yaml` already exists in nebula-crm and is already generic — it uses `current_stage: framework-bootstrap` and two framework-only gates (`boundary_genericness`, `skill_regression`), with no CRM-specific values. Step 2 archives `agents/` wholesale, so the file lands in nebula-agents unchanged with no further work.

This subsection exists only to make that fact explicit (so a reader scanning §5 doesn't assume the template needs separate authoring). It is not a Step 6 action item.

### 5.5 docs/migration-from-nebula-crm.md (new file under agents/docs/)

A short migration note for anyone who was using nebula-crm as their framework source:

- What moved where
- The new workspace layout
- How to update their `lifecycle-stage.yaml` references
- Link to CONSUMER-CONTRACT.md

### 5.6 CHANGELOG.md (new file at root)

Initial entry: `v0.1.0 — <split-date> — Initial standalone release, split from gajakannan/nebula-crm at commit <baseline-hash> (the Section 9 Step 0 hash, recorded identically in .split-baseline)`.

Tag this commit `v0.1.0` after the repo is set up.

### 5.7 Existing framework docs/actions that must be rewritten

Because the new operating model is sibling-repo based, the following carried files must be rewritten in Step 5 rather than treated as already-correct snapshot content:

- `agents/README.md`
- `README.md`
- `agents/actions/init.md` — make it scaffold into `{PRODUCT_ROOT}` rather than the framework repo root
- `agents/docs/FORK-AND-BUILD-APP.md`
- `agents/docs/ONBOARDING.md` — Step 2 (lines 16–21) describes the old copy-in-place model (`Copy \`agents/\` to your new repo`). Rewrite to the sibling-repo workspace layout and the `{PRODUCT_ROOT}` contract.
- `agents/docs/ARCHITECTURE.md`
- `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`
- `blueprint-setup/README.md`
- `BOUNDARY-POLICY.md`
- `agents/templates/boundary-policy-template.md`
- `CONTRIBUTING.md`
- `agents/*/scripts/README.md` examples that still assume same-repo product paths
- Any other framework doc/action/template that still tells the operator to copy `agents/` into the product repo or scaffold into the current working directory

---

## 6. nebula-insurance-crm: New and Rewritten Documents

### 6.1 README.md (rewrite from scratch)

Product-focused README:

- **What it is:** Nebula Insurance CRM — a commercial P&C CRM product
- **Tech stack:** C# .NET (engine/), React TypeScript (experience/), Python AI layer (neuron/), PostgreSQL, Authentik
- **How to run locally:** Docker compose commands, dev setup
- **Planning structure:** Where to find BLUEPRINT.md, feature planning, API specs
- **Feature roadmap:** Link to `planning-mds/features/ROADMAP.md` and `REGISTRY.md`
- **Current work state:** `planning-mds/features/ROADMAP.md`, `REGISTRY.md`, and `lifecycle-stage.yaml` are authoritative; the split itself does not activate or start a feature
- **Source provenance:** Split from `gajakannan/nebula-crm` at commit `<migration-baseline-hash>` (the Step 0 / Step 1 operator-selected local source snapshot used for the split)

The product README must remain product-focused. It should not require or document local paths into `nebula-agents`, `agents/`, or tool-specific framework bootstrap files.

### 6.2 lifecycle-stage.yaml (authored fresh in Step 7)

`nebula-crm`'s `lifecycle-stage.yaml` is not copied (see §3 table). It defines 10 gates and wires 7 of them to framework-owned validators (`boundary_genericness`, `skill_regression`, `api_contract`, `infra_non_strict`, `infra_strict`, `security_planning_light`, `security_planning_strict`). Stripping them leaves so little that authoring fresh is cleaner.

Author a new file in nebula-insurance-crm with:
- `release_posture`: preserve the block from `nebula-crm`'s file but rewrite the `summary` for the post-split state. `mode` stays `public-preview-human-orchestrated` (the product is still in public preview after the split). The `summary` should cite: (a) the split provenance — same hash recorded in `.split-baseline`; (b) the current lifecycle stage; (c) confirmation that product-local validation gates remain enforced. Keep it short — the registry and roadmap own feature-by-feature truth.
- `current_stage`: `implementation`
- `implementation_scope`: keep the block in Phase A, but leave `active_feature` unset and `active_stories: []`; do not use the split itself to nominate a feature, and do not propagate the source repo's stale feature-specific note
- `summary`: post-split state, feature-agnostic
- **Phase A gate set:** only product-local gates that already have product-local implementations — `knowledge_graph_sync` via `scripts/kg/validate.py`, `solution_contract` via `planning-mds/testing/validate-nebula-api-contract.py`, `frontend_quality` via `planning-mds/testing/validate-frontend-quality-gate.py`.
- **Phase B gate expansion (required before CRM CI/branch protection is considered restored):** reintroduce product-local equivalents for the categories that previously depended on framework validators:
  - `api_contract` via a CRM-local validator script (for example `scripts/validate-api-contract.py`) rehomed from the current structural OpenAPI validation logic
  - `infra_strict` via a CRM-local validator script (for example `scripts/validate-infrastructure.py --strict`)
  - `security_planning_strict` via a CRM-local validator script (for example `scripts/security-audit.py --strict`)
- `boundary_genericness`, `skill_regression`, and the planning-governance validators remain framework-owned in nebula-agents and are not mirrored into the product repo or product CI.
- This file is **required in Phase A**. It is the authoritative product-local lifecycle contract for Step 8 validation and for the product-local lifecycle runner / CRM CI work restored in Phase B. Do not omit it.

### 6.3 docs/migration-from-nebula-crm.md (new file)

Short note:
- Source commit from nebula-crm (record the same hash that appears in `.split-baseline`)
- What was removed (agents/, blueprint-setup/, builder Dockerfile)
- What was preserved intact (application code, archived feature records, evidence packages)
- What was rewritten for split compatibility (live operator-facing docs and validation wiring)
- Where to find current roadmap / registry state after migration
- Explicitly state that the product repo has no local dependency on `nebula-agents`

### 6.4 CHANGELOG.md (new file at root)

Initial entry: `Initial release — <split-date> — Split from gajakannan/nebula-crm at commit <baseline-hash> (recorded in .split-baseline). Application code plus historical planning/archive evidence preserved; framework (agents/, blueprint-setup/, builder image) removed and lives in gajakannan/nebula-agents; live operator-facing docs and validation wiring rewritten to remove local framework-path dependency.`

No version tag is applied to the product repo at split time — the CRM is a continuously-deployed product and uses release-branch tagging, not semantic versioning. The split is recorded by the baseline hash, not by a tag.

---

## 7. Feature-Agnostic Split Policy

This split is not coupled to F0018 or any other single feature. Its job is to preserve the current product state and establish the repo boundary.

**Action:**
- Move current planning artifacts, application code, and historical evidence into `nebula-insurance-crm`
- Do NOT start new feature/story execution as part of Phase A
- After the split and dry-run, choose the next workstream through normal product planning; `planning-mds/features/REGISTRY.md`, `ROADMAP.md`, and the post-split `lifecycle-stage.yaml` own that decision
- Treat any pre-existing policy/account/renewal surfaces in `nebula-crm` as historical context, not as part of the split contract

**Why this order:**
- Keeps repository migration mechanically focused on ownership boundaries
- Avoids coupling the split to feature triage or readiness debates
- Lets the product team pick the next implementation target only after the new repo boundary is validated

---

## 8. GitHub Repository Setup

Create both repos at:
- `https://github.com/gajakannan/nebula-agents`
- `https://github.com/gajakannan/nebula-insurance-crm`

Both should be initialized **empty** (no auto-generated README). The content will be pushed from local copies derived from nebula-crm.

Recommended initial setup for each:
- Default branch: `main`
- Branch protection on `main`: require PR immediately; add required CI pass rules in Phase B once CI is restored
- No submodules configured

**Do not** create any GitHub Actions workflows that cross-reference the other repo. The two repos are independent.

---

## 9. Execution Order

Execute these steps in sequence. Do not skip or reorder.

### Phase A — Repository Split and Decoupling

Phase A is about producing two clean repositories with the correct ownership boundary. It is **not** the phase where CI is restored or hardened.

### Step 0: Capture the immutable baseline and define execution roots

`nebula-crm` is the rollback backup. Do not commit to it, push to it, or otherwise modify it during the split. The "prep commit" approach from earlier revisions is dropped — every prep edit is applied directly in the destination repos in Steps 5 and 7, where the content actually belongs.

Define the stable paths for the run, assert that they are safe, choose the local source snapshot for this run, then capture the baseline hash:

```bash
SOURCE_REPO_ROOT=/path/to/nebula-crm
WORKSPACE_ROOT=/path/to/workspace
AGENTS_REPO_ROOT="$WORKSPACE_ROOT/nebula-agents"
PRODUCT_REPO_ROOT="$WORKSPACE_ROOT/nebula-insurance-crm"
SNAPSHOT_ROOT=/tmp/nebula-crm-split
BASELINE_CACHE=/tmp/nebula-crm-split-baseline
SOURCE_REF_CACHE=/tmp/nebula-crm-split-source-ref

# Local source snapshot to split from.
# Default: the current checked-out local source commit.
SOURCE_SNAPSHOT_REF=${SOURCE_SNAPSHOT_REF:-HEAD}

mkdir -p "$WORKSPACE_ROOT"

test "$WORKSPACE_ROOT" != "$SOURCE_REPO_ROOT" || { echo "FAIL: WORKSPACE_ROOT must not equal SOURCE_REPO_ROOT" >&2; exit 1; }
case "$WORKSPACE_ROOT/" in
  "$SOURCE_REPO_ROOT"/*) echo "FAIL: WORKSPACE_ROOT must be outside SOURCE_REPO_ROOT" >&2; exit 1 ;;
esac

# Rerun safety: fail fast if a previous attempt left split outputs behind.
test ! -e "$AGENTS_REPO_ROOT" || { echo "FAIL: remove or move existing $AGENTS_REPO_ROOT before rerun" >&2; exit 1; }
test ! -e "$PRODUCT_REPO_ROOT" || { echo "FAIL: remove or move existing $PRODUCT_REPO_ROOT before rerun" >&2; exit 1; }
test ! -e "$SNAPSHOT_ROOT" || { echo "FAIL: remove existing $SNAPSHOT_ROOT before rerun" >&2; exit 1; }
test ! -e "$BASELINE_CACHE" || { echo "FAIL: remove existing $BASELINE_CACHE before rerun" >&2; exit 1; }
test ! -e "$SOURCE_REF_CACHE" || { echo "FAIL: remove existing $SOURCE_REF_CACHE before rerun" >&2; exit 1; }

BASELINE_HASH=$(git -C "$SOURCE_REPO_ROOT" rev-parse "$SOURCE_SNAPSHOT_REF")
echo "$BASELINE_HASH" > "$BASELINE_CACHE"   # convenience cache for this session only
echo "$SOURCE_SNAPSHOT_REF" > "$SOURCE_REF_CACHE"
```

The path variables above are part of the execution contract for the rest of the plan. If you resume in a fresh shell, re-export the same values before continuing. `SOURCE_SNAPSHOT_REF` is the operator-selected local source snapshot for this run (default `HEAD` in the checked-out local copy) and is cached in `SOURCE_REF_CACHE` for later sanity checks. The authoritative record of `BASELINE_HASH` is written into a tracked file (`.split-baseline`) inside each new repo in Steps 2 and 3. The cache files avoid retyping the snapshot ref/hash across shell invocations; Steps 1-3 re-read them rather than assuming the env vars are still set, so execution can resume across shell sessions on the same machine. If a host reboot or `/tmp` cleanup wipes the caches before Phase A finishes, re-run Step 0 to recompute them (the hash is deterministic for the chosen local source snapshot, which Step 0 / §1 forbid modifying during the split).

Stale-state edits that previously lived in the dropped "prep commit" step now happen in the destination repos instead:
- **`BOUNDARY-POLICY.md` example rewrites** → applied in Step 5 (in nebula-agents, where the file lives post-split). Both the ❌ BAD and ✅ GOOD persona examples are rewritten to a neutral non-insurance domain.
- **`lifecycle-stage.yaml` stale feature pin / implementation note** → handled in Step 7 (in nebula-insurance-crm, where the file is authored fresh per §6.2). The split does not nominate an active feature, and the source file's stale feature-specific note is not carried over.

### Step 1: Establish an immutable source snapshot

Do not copy from a moving working tree or from whatever branch happens to be current later in the day. Create a detached worktree at the Step 0 local source snapshot and use `git archive` against it for all subsequent copies — `git archive` reads only tracked content, so the snapshot is unconditionally clean.

```bash
BASELINE_HASH=$(cat "$BASELINE_CACHE")
SOURCE_SNAPSHOT_REF=$(cat "$SOURCE_REF_CACHE")
git -C "$SOURCE_REPO_ROOT" rev-parse "${BASELINE_HASH}^{commit}"   # validate hash exists
git -C "$SOURCE_REPO_ROOT" worktree add --detach "$SNAPSHOT_ROOT" "${BASELINE_HASH}"
```

All `git archive` commands in Steps 2/3 read from `"$SNAPSHOT_ROOT"`. The worktree is removed in Step 9 after both repos have committed and pushed; nebula-crm itself is untouched throughout.

### Step 2: Create nebula-agents locally

```bash
# Re-read the baseline hash from disk so this step stands alone across shell sessions.
# Step 0 defined SOURCE_REPO_ROOT / WORKSPACE_ROOT / AGENTS_REPO_ROOT / PRODUCT_REPO_ROOT /
# SNAPSHOT_ROOT / BASELINE_CACHE. Re-export the same values if you resumed in a fresh shell.
BASELINE_HASH=$(cat "$BASELINE_CACHE")

mkdir -p "$AGENTS_REPO_ROOT"
cd "$AGENTS_REPO_ROOT"
git init
git branch -M main

# Author the framework-tuned .gitignore as the FIRST file (before any content lands).
# Covers Python, IDE, OS, and the framework builder. Do not copy nebula-crm's .gitignore.
cat > .gitignore <<'EOF'
__pycache__/
*.py[cod]
*.egg-info/
.pytest_cache/
.venv/
venv/
.idea/
.vscode/
.DS_Store
*.swp
EOF

# Record the baseline hash as a tracked file (authoritative provenance — survives reboots).
echo "$BASELINE_HASH" > .split-baseline

# Snapshot framework content via git archive (tracked content only — no caches, no .env).
SRC="$SNAPSHOT_ROOT"
git -C "$SRC" archive HEAD agents | tar -x
git -C "$SRC" archive HEAD blueprint-setup | tar -x
git -C "$SRC" archive HEAD docker/agent-builder | tar -x
git -C "$SRC" archive HEAD Dockerfile docker-compose.agent-builder.yml \
                              BOUNDARY-POLICY.md CONTRIBUTING.md LICENSE | tar -x

# Move the framework-coupled template validator into agents/scripts/.
# Its imports MUST be rewritten in Step 5 — do NOT commit until that's done and tests pass.
mkdir -p agents/scripts/tests
git -C "$SRC" archive HEAD scripts/kg/validate_templates.py \
    | tar -x --strip-components=2 -C agents/scripts/
git -C "$SRC" archive HEAD scripts/kg/tests/test_validate_templates.py \
    | tar -x --strip-components=3 -C agents/scripts/tests/
```

Do **not** copy `.github/workflows/` in Phase A. The `validate_templates.py` move is staged on disk but not commit-ready until Step 5 fixes its imports.

### Step 3: Create nebula-insurance-crm locally

```bash
# Re-read the baseline hash from disk so this step stands alone across shell sessions.
# Step 0 defined SOURCE_REPO_ROOT / WORKSPACE_ROOT / AGENTS_REPO_ROOT / PRODUCT_REPO_ROOT /
# SNAPSHOT_ROOT / BASELINE_CACHE. Re-export the same values if you resumed in a fresh shell.
BASELINE_HASH=$(cat "$BASELINE_CACHE")

mkdir -p "$PRODUCT_REPO_ROOT"
cd "$PRODUCT_REPO_ROOT"
git init
git branch -M main

# Author the product .gitignore + .dockerignore as the FIRST files. Carry over nebula-crm's
# values (.NET, Node, Python artifacts, .kg-state/, security-reports/, .env, build caches),
# then trim only framework-only agent patterns that no longer apply.
SRC="$SNAPSHOT_ROOT"
git -C "$SRC" archive HEAD .gitignore .dockerignore .env.example | tar -x
# (Edit .gitignore here to keep generic Python artifact ignores, while dropping only
# framework-only agent patterns that no longer apply.)

# Record the baseline hash as a tracked file.
echo "$BASELINE_HASH" > .split-baseline

# Snapshot product content via git archive.
git -C "$SRC" archive HEAD planning-mds engine experience neuron bruno scripts | tar -x
git -C "$SRC" archive HEAD docker/authentik docker/postgres | tar -x
git -C "$SRC" archive HEAD docker-compose.yml docker-compose.qe.yml \
                              pyproject.toml LICENSE | tar -x

# Drop framework-coupled script exceptions that moved to nebula-agents in Step 2.
rm -f scripts/kg/validate_templates.py scripts/kg/tests/test_validate_templates.py
# Drop the Claude Code-specific bridge (not part of the durable contract).
rm -f scripts/kg/pretool_hook.py

# Sanity assertions: the CRM repo must NOT contain any framework directories or
# the carried-over lifecycle file at this point. Catches accidental over-archiving.
test ! -d agents           || { echo "FAIL: agents/ leaked into CRM repo" >&2; exit 1; }
test ! -d blueprint-setup  || { echo "FAIL: blueprint-setup/ leaked into CRM repo" >&2; exit 1; }
test ! -e Dockerfile       || { echo "FAIL: builder Dockerfile leaked into CRM repo" >&2; exit 1; }
test ! -e docker-compose.agent-builder.yml || { echo "FAIL: builder compose leaked into CRM repo" >&2; exit 1; }
test ! -e lifecycle-stage.yaml || { echo "FAIL: lifecycle-stage.yaml must be absent until Step 7 authors it fresh" >&2; exit 1; }
test ! -e BOUNDARY-POLICY.md || { echo "FAIL: BOUNDARY-POLICY.md is framework-owned, must not be in CRM repo" >&2; exit 1; }
test ! -e CONTRIBUTING.md  || { echo "FAIL: CONTRIBUTING.md is framework-owned, must not be in CRM repo" >&2; exit 1; }
```

Do **not** copy `lifecycle-stage.yaml`, `.codex`, `.claude/`, or `.github/workflows/` in Phase A. `lifecycle-stage.yaml` is authored fresh in Step 7.

### Step 4: Remove live product → framework dependencies from nebula-insurance-crm

Before the first CRM commit, scrub live/operator-facing product surfaces so the product repo has no executable local dependency on the framework repo. Preserve historical provenance separately. (`lifecycle-stage.yaml` is not copied per Step 3, the framework-coupled script exceptions are removed in Step 3, and `.github/workflows/` is not copied in Phase A — none of those need further action here.)

- Remove or rewrite any live reference to `agents/**` as a local path
- Remove or rewrite any reference to `../nebula-agents/**`
- Remove or rewrite any reference to `.claude/`, `CLAUDE.md`, `AGENTS.md`, `.cursorrules`, or similar framework/tool bootstrap files
- Remove or rewrite any live reference to other framework-owned local assets such as `blueprint-setup/`, `BOUNDARY-POLICY.md`, `CONTRIBUTING.md`, `docker-compose.agent-builder.yml`, or the framework builder image / bootstrap flow (`nebula-agent-builder`) when those references imply a local in-repo dependency
- Rewrite live product docs that currently instruct operators to read or execute framework-owned assets from inside the product repo
- Rewrite live planning/operator docs that currently embed framework commands. At minimum: `planning-mds/README.md`, `planning-mds/BLUEPRINT.md`, `planning-mds/features/TRACKER-GOVERNANCE.md`, `planning-mds/knowledge-graph/README.md`, non-archived `planning-mds/features/**/GETTING-STARTED.md`, `planning-mds/operations/evidence/README.md`, `planning-mds/operations/evidence/frontend-ux/README.md`, `planning-mds/operations/evidence/frontend-ux/TEMPLATE.md`, `planning-mds/operations/evidence/frontend-quality/README.md`, `planning-mds/examples/**`, `neuron/README.md`, `planning-mds/domain/glossary.md` (the Genericness-Blocked Terms section currently says "Parsed by `agents/scripts/validate-genericness.py`" — rewrite as an external framework-repo reference or a neutral description), and `planning-mds/architecture/SOLUTION-PATTERNS.md` (live guidance references `agents/*/references/*.md` as if it were a local path)
- The product root `README.md` and `docs/migration-from-nebula-crm.md` are authored fresh in Step 7, not copied in Step 3. Rerun this Step 4 scrub after Step 7 so those newly-authored files are checked before the first CRM commit.
- Keep historical evidence payloads and archived feature records (`planning-mds/features/archive/**` plus per-run evidence payloads under `planning-mds/operations/evidence/**`) unless there is a product reason to rewrite them; they are provenance, not live operator instructions. The evidence index/README/template docs listed in the previous bullet remain in scope and are NOT treated as archival.
- Framework-owned planning validators (`validate-stories.py`, `generate-story-index.py`, `validate-trackers.py`) stay in nebula-agents. Live product docs may mention them only as framework-session steps or by external repo URL; do not leave local `agents/**` command lines in the product repo.
- Validation gate (must return zero hits across live surfaces before commit):

```bash
rg -n '(^|[^A-Za-z0-9_/])agents/|(^|[^:/A-Za-z0-9_-])(blueprint-setup/|BOUNDARY-POLICY\.md|CONTRIBUTING\.md|docker-compose\.agent-builder\.yml|nebula-agent-builder)' . \
  -g '!**/.git/**' -g '!**/node_modules/**' -g '!**/bin/**' -g '!**/obj/**' \
  -g '!planning-mds/features/archive/**' -g '!planning-mds/operations/evidence/**'
rg -n '\.\./nebula-agents/|\.claude/|CLAUDE\.md|AGENTS\.md|\.cursorrules' . \
  -g '!**/.git/**' -g '!**/node_modules/**' -g '!**/bin/**' -g '!**/obj/**' \
  -g '!planning-mds/features/archive/**' -g '!planning-mds/operations/evidence/**'

# Evidence index/template docs stay in scope even though per-run evidence payloads are archival.
rg -n '(^|[^A-Za-z0-9_/])agents/|\.\./nebula-agents/|\.claude/|CLAUDE\.md|AGENTS\.md|\.cursorrules|(^|[^:/A-Za-z0-9_-])(blueprint-setup/|BOUNDARY-POLICY\.md|CONTRIBUTING\.md|docker-compose\.agent-builder\.yml|nebula-agent-builder)' \
  planning-mds/operations/evidence/README.md \
  planning-mds/operations/evidence/frontend-ux/README.md \
  planning-mds/operations/evidence/frontend-ux/TEMPLATE.md \
  planning-mds/operations/evidence/frontend-quality/README.md
```

The product repo must stand on its own as an application repository. Historical docs may preserve past framework command traces, but live operator surfaces may not require local framework paths.

### Step 5: Apply framework path indirection, genericness cleanup, and standalone fixes in nebula-agents

Before the first framework commit, make `nebula-agents` execution-ready from its own session root and self-contained for validation:

- Apply the `{PRODUCT_ROOT}` rewrite across `agents/**` for all product-owned path families
- Remove bare `scripts/kg/…`, `planning-mds/…`, `engine/…`, `experience/…`, and `neuron/…` references
- Rewrite command examples that implicitly assume the current working directory is inside the product repo or one of its layers (for example `pytest tests/`) to use product-root-aware forms
- Apply the brand/namespace/filename cleanup from Section 4.2
- Generalize `agents/templates/feature-assembly-plan-template.md` per Section 4.3
- Fix all domain-term leaks so `validate-genericness.py` passes
- Fix stale framework-internal self-references per Section 4.5 (for example impossible `agents/` prefixes on framework-root-owned files such as `BOUNDARY-POLICY.md`)
- Rewrite framework onboarding/bootstrap docs that still describe copy-in-place consumption or same-repo scaffolding. At minimum: `agents/README.md`, `agents/actions/init.md`, `agents/docs/FORK-AND-BUILD-APP.md`, `agents/docs/ONBOARDING.md` (Step 2 still reads "Copy `agents/` to your new repo" at lines 16–21), `agents/docs/ARCHITECTURE.md`, `agents/docs/AGENT-USE.md`, `agents/docs/ORCHESTRATION-CONTRACT.md`, `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`, `blueprint-setup/README.md`, `BOUNDARY-POLICY.md`, `agents/templates/boundary-policy-template.md`, `CONTRIBUTING.md`, and `agents/*/scripts/README.md` examples that still assume product-root execution from the framework repo
- The framework root `README.md`, `CONSUMER-CONTRACT.md`, and `agents/docs/migration-from-nebula-crm.md` are authored fresh in Step 6, not carried by Step 2. Validate those fresh docs in the Step 6 rerun described below rather than treating them as Step 2 snapshot rewrites.
- Update `init` action and related docs so bootstrapping writes into `{PRODUCT_ROOT}` (or creates that repo) rather than the framework repo root
- Update any framework Python script defaults that assumed the product repo was the session root, using the uniform `--product-root` / `NEBULA_PRODUCT_ROOT` / `../nebula-insurance-crm` resolution order from §4.1. Apply to the full baseline set:
  - `agents/product-manager/scripts/validate-trackers.py`
  - `agents/product-manager/scripts/validate-stories.py`
  - `agents/product-manager/scripts/generate-story-index.py`
  - `agents/architect/scripts/validate-architecture.py`
  - `agents/architect/scripts/validate-api-contract.py`
  - `agents/devops/scripts/validate-infrastructure.py`
  - `agents/security/scripts/security-audit.py`
  - `agents/frontend-developer/scripts/validate-frontend-ux-evidence.py` (its `EVIDENCE_DIR = Path("planning-mds/operations/evidence/frontend-ux")` at line 25 must become product-root-aware)
  - `agents/frontend-developer/scripts/scaffold-component.py` (its default `--components-dir experience/src/components` must become product-root-aware)
  - `agents/frontend-developer/scripts/scaffold-page.py` (its default `--pages-dir experience/src/pages` and route examples must become product-root-aware)
  - Plus any other framework script that takes a product-relative default path
- **Rewrite the framework root `Dockerfile`** so it remains buildable after the split:
  - keep it in `nebula-agents` (it is the framework builder image, not the CRM app image)
  - remove the current dependency on root `scripts/` content, since `scripts/` stays product-owned
  - install framework Python deps from `agents/scripts/requirements.txt`
  - verify `docker-compose.agent-builder.yml` still points at this rewritten Dockerfile without requiring further path changes. Open the file and confirm `build.context` / `build.dockerfile` resolve to the framework repo root and the rewritten `Dockerfile` filename — one quick read, but do not skip it since the compose file is not otherwise touched in Step 5
- **Rewrite both `BOUNDARY-POLICY.md` persona examples** (currently the ❌ BAD block at lines ~188–200 and the ✅ GOOD block at lines ~204–216) to a neutral non-insurance domain such as "Subscription Billing — Plan Manager" / "Subscription Billing — at AcmeBilling". Both must change — the GOOD example is also insurance-specific today and would be a leak inside a framework doc post-split.
- **Embed the genericness denylist into `agents/scripts/validate-genericness.py`** as a static fallback (per §4.4). Extract the `Genericness-Blocked Terms` section from `"$SNAPSHOT_ROOT"/planning-mds/domain/glossary.md` and inline it into the script. Preserve the `--glossary <path>` flag as an override.
- **Repair `agents/scripts/validate_templates.py` and its test** after the Step 2 move:
  - If the script imported `kg_common` (which stays in nebula-insurance-crm), either inline the small subset actually used or vendor a minimal helper into `agents/scripts/`
  - Update default arg paths in the script that assumed `agents/actions/plan.md` etc. resolved from the CRM session root (these should still resolve from nebula-agents' own root — no `{PRODUCT_ROOT}` prefix needed since the inputs are framework-owned)
  - Remove the default dependency on product-side `planning-mds/knowledge-graph/solution-ontology.yaml`. The script's `ontology_expectations()` only reads three boolean flags from `ownership.{product-manager, architect, implementation_agents}`, so inlining is trivial — either hardcode the expected owner list `["product-manager", "architect", "implementation_agents"]` or ship a minimal framework-local fixture (`agents/scripts/tests/fixtures/ownership.yaml`) with just those three keys. Do not vendor the full ontology.
  - Update the test to invoke the validator at its new path under `agents/scripts/` and pass with no product-side ontology input
  - Run `python3 -m pytest agents/scripts/tests/test_validate_templates.py` and confirm green before commit
- **Update `agents/scripts/requirements.txt` if needed** so it declares framework dev/test deps (`pyyaml`, `pytest` at minimum). This replaces the CRM-scoped `pyproject.toml` that does not move. Install in any framework shell or CI step via `python3 -m pip install -r agents/scripts/requirements.txt`
- **`agents/scripts/run-lifecycle-gates.py` needs no code changes in Step 5.** Its `DEFAULT_CONFIG_PATH = Path("lifecycle-stage.yaml")` resolves against the framework session root, which now holds the new framework-local `lifecycle-stage.yaml` authored in Step 6. Its `repo_root = Path(__file__).resolve().parents[2]` still lands at the nebula-agents root, which is correct for invoking gate commands. The cleanup happens indirectly via the framework `lifecycle-stage.yaml` gate list being framework-only (Step 10) — do not "generalize" the runner itself. The product-local fork of this runner is authored separately in Step 11 with its own repo-root/config-path fix.

Validation required before proceeding:

```bash
# Validator now runs with no arguments (embedded denylist).
python3 agents/scripts/validate-genericness.py

# Test for the moved template validator.
python3 -m pytest agents/scripts/tests/test_validate_templates.py

# These must return NO lines. The scope intentionally includes framework root docs and
# bootstrap assets, not just `agents/**`, because Step 5 rewrites those too.
# Use the anchored form from §4.1 so the match requires
# a *bare* reference — `{PRODUCT_ROOT}/planning-mds/...` correctly does NOT match
# because the char before `planning-mds/` is `/`, which the class excludes.
# A simple `grep -v '{PRODUCT_ROOT}'` would false-negative any line that mixes a
# valid `{PRODUCT_ROOT}/...` substitution with a bare leak elsewhere on the same line.
rg -n -e '(^|[^{/])scripts/kg/'   agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e '(^|[^{/])planning-mds/' agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e '(^|[^{/])engine/'       agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e '(^|[^{/])experience/'   agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e '(^|[^{/])neuron/'       agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e '(^|[^{/])bruno/'        agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml

# These two should return NO lines outside intentional retention sites documented in Section 4.2.
rg -n -e 'Nebula\.[A-Z]' agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
rg -n -e 'nebula-api\.yaml' agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml

# Framework-root-owned files must not be referenced via impossible `agents/` prefixes.
rg -n -e 'agents/(BOUNDARY-POLICY\.md|CONTRIBUTING\.md|README\.md|CHANGELOG\.md|CONSUMER-CONTRACT\.md|lifecycle-stage\.yaml|Dockerfile|docker-compose\.agent-builder\.yml)' \
  agents/ blueprint-setup/ CONTRIBUTING.md BOUNDARY-POLICY.md Dockerfile docker-compose.agent-builder.yml
```

Additionally, manually review command tables and scaffold script help text for implicit current-working-directory assumptions that string grep cannot detect (for example bare `pytest tests/`, `cd experience`, or default output directories that still target the framework repo instead of `{PRODUCT_ROOT}`).

`BOUNDARY-POLICY.md` stays valuable as framework documentation after the split, but it is not a separate blocking grep gate. Rewrite its example blocks per the Step 5 bullet above and verify them during normal doc review. As part of the same review pass, open a few rewritten framework docs/templates that previously referenced `agents/BOUNDARY-POLICY.md` and confirm the root-relative path now resolves.

All checks must produce the intended clean result before Phase A is considered execution-ready.

### Step 6: Write new nebula-agents documents

Create the following (per Section 5):

- `README.md` — rewrite
- `CONSUMER-CONTRACT.md` — new
- `lifecycle-stage.yaml` — new framework-local file
- `agents/docs/migration-from-nebula-crm.md` — new
- `CHANGELOG.md` — new

After authoring these fresh files, rerun the Step 5 validation block and extend the grep/manual-review scope to include the new operator-facing docs:
- `README.md`
- `CONSUMER-CONTRACT.md`
- `agents/docs/AGENT-USE.md`
- `agents/docs/ORCHESTRATION-CONTRACT.md`
- `agents/docs/migration-from-nebula-crm.md`

Do not proceed to Step 7 until that rerun is clean. This is the point where the fresh framework root docs enter the blocking validation scope.

### Step 7: Write new nebula-insurance-crm documents

Create the following (per Section 6):

- `README.md` — rewrite (§6.1)
- `lifecycle-stage.yaml` — author fresh per §6.2; do not derive from nebula-crm's copy. This file is required in Phase A. Limit it initially to product-local validators that already exist (`knowledge_graph_sync`, `solution_contract`, `frontend_quality`). In Phase B, expand it with CRM-local `api_contract`, `infra_strict`, and `security_planning_strict` once those validators are rehomed product-side.
- `docs/migration-from-nebula-crm.md` — new (§6.3)
- `CHANGELOG.md` — new (§6.4); record split provenance against the same hash as `.split-baseline`

After authoring these files, rerun the Step 4 scrub gate from the CRM repo root so the fresh `README.md`, `docs/migration-from-nebula-crm.md`, `lifecycle-stage.yaml`, and copied live planning docs all satisfy the no-local-framework-dependency rule before the first CRM commit.

### Step 8: Validate the operating model end-to-end

Before pushing Phase A, confirm that a session rooted in `"$AGENTS_REPO_ROOT"` can actually operate on the sibling CRM repo at `"$PRODUCT_REPO_ROOT"` (which resolves to the default `{PRODUCT_ROOT}` path `../nebula-insurance-crm` from the framework repo).

**Test 1 — Framework session reads correctly:**
Ask the agent:
> "Read the architect SKILL and the feature action. What are the required planning inputs and where should outputs be written for a feature implementation in the resolved `{PRODUCT_ROOT}`?"

The agent should correctly identify product paths in the sibling repo.

**Test 2 — Cross-repo navigation works:**
Ask the agent to read `{PRODUCT_ROOT}/planning-mds/BLUEPRINT.md` and summarize the product context plus where the authoritative roadmap and registry live.

The agent should read the file without tool errors and produce an accurate summary.

**Test 3 — Validation ownership is understood:**
Ask the agent:
> "From `CONSUMER-CONTRACT.md`, `agents/docs/AGENT-USE.md`, and `{PRODUCT_ROOT}/lifecycle-stage.yaml`, which validations run from nebula-agents and which run from nebula-insurance-crm?"

The agent should separate framework-owned vs product-local checks correctly.

All three tests passing confirms that Phase A is execution-ready: agents can work from `nebula-agents` into the CRM repo.

### Step 9: Initial Phase A commits, push, and snapshot cleanup

Commit the two repos as split artifacts only. Do not block Phase A on CI restoration. The baseline hash is read from each repo's tracked `.split-baseline` file (authoritative — does not depend on `/tmp/`).

```bash
# In $AGENTS_REPO_ROOT
cd "$AGENTS_REPO_ROOT"
BASELINE_HASH=$(cat .split-baseline)
git add .
git commit -m "Initial release: nebula-agents v0.1.0 — split from gajakannan/nebula-crm@${BASELINE_HASH:0:7}"
git remote add origin https://github.com/gajakannan/nebula-agents.git
git push -u origin main
git tag v0.1.0
git push origin v0.1.0

# In $PRODUCT_REPO_ROOT
cd "$PRODUCT_REPO_ROOT"
BASELINE_HASH=$(cat .split-baseline)
git add .
git commit -m "Initial release: nebula-insurance-crm — split from gajakannan/nebula-crm@${BASELINE_HASH:0:7}"
git remote add origin https://github.com/gajakannan/nebula-insurance-crm.git
git push -u origin main

# Cleanup: remove the snapshot worktree from nebula-crm. The chosen local source snapshot is untouched.
git -C "$SOURCE_REPO_ROOT" worktree remove "$SNAPSHOT_ROOT"
SOURCE_SNAPSHOT_REF=$(cat "$SOURCE_REF_CACHE")
rm -f "$BASELINE_CACHE" "$SOURCE_REF_CACHE"

# Sanity: confirm the chosen local source snapshot still resolves to the same baseline hash.
test "$(git -C "$SOURCE_REPO_ROOT" rev-parse "$SOURCE_SNAPSHOT_REF")" = "$BASELINE_HASH"
```

### Phase B — GitHub Actions Restoration

Phase B is only about restoring GitHub Actions in the two repos. It does not change the operating model established in Phase A.

### Step 10: Restore framework CI in nebula-agents

Only after Phase A is done:

- Materialize the baseline workflow file directly from `nebula-crm` at the recorded baseline hash, not from the transient snapshot worktree at `"$SNAPSHOT_ROOT"`:

```bash
cd "$AGENTS_REPO_ROOT"
BASELINE_HASH=$(cat .split-baseline)
mkdir -p .github/workflows
git -C "$SOURCE_REPO_ROOT" show "${BASELINE_HASH}:.github/workflows/ci-gates.yml" > .github/workflows/ci-gates.yml
```

- Modify that file in place — do not rewrite from scratch, since the source version already encodes the right Actions shape (job names, triggers, artifact handling).
- Ensure it uses the new framework-local `lifecycle-stage.yaml`
- Ensure it runs framework-only gates (`boundary_genericness`, `skill_regression`)
- Remove any CRM/runtime-specific checks such as policy parity or app-specific build assertions. **Concretely, drop the `BrokerUser policy parity check` step and its `python3 scripts/check-policy-parity.py` invocation.**
- **Keep the `docker build -t nebula-builder .` step in the framework copy of `ci-gates.yml`.** It builds the framework builder image from the rewritten `Dockerfile` (Step 5) and is a framework-level smoke test. Step 11's CRM copy of `ci-gates.yml` must drop this step — the CRM no longer owns the builder `Dockerfile`.
- The genericness validator is already standalone from Phase A Step 5; Phase B only wires that settled design into CI.

### Step 11: Restore product CI in nebula-insurance-crm

Only after live/operator-facing local `agents/**` references are removed and workflow files are product-local:

- Materialize baseline workflow files directly from `nebula-crm` at the recorded baseline hash, not from the transient snapshot worktree at `"$SNAPSHOT_ROOT"`. In `"$PRODUCT_REPO_ROOT"`, use `BASELINE_HASH=$(cat .split-baseline)` and `git -C "$SOURCE_REPO_ROOT" show "${BASELINE_HASH}:.github/workflows/<file>" > .github/workflows/<file>` for each workflow you reintroduce.
- Reintroduce the CRM workflows one by one. Files at baseline: `ci-gates.yml`, `dotnet-test.yml`, `frontend-performance.yml`, `frontend-ui.yml`, `pact-contract.yml`, `qe-api.yml`, `smoke-test.yml`, `sonarqube.yml`, `vitest.yml`.
- **Do not drop `ci-gates.yml` from the CRM repo.** The baseline file aggregates *product* gates (`api_contract`, `solution_contract`, `frontend_quality`, `infra_strict`, `security_planning_strict`, `knowledge_graph_sync`) and dropping it silently removes that aggregated gate run. Instead:
  - Author a product-local lifecycle runner (e.g., `scripts/run-lifecycle-gates.py`) that consumes the freshly-authored product `lifecycle-stage.yaml` from §6.2. Treat this as an intentional fork, not a raw file move:
    - change repo-root resolution for the new location (`scripts/` means the repo root is `Path(__file__).resolve().parents[1]`, not `parents[2]`)
    - resolve `--config` relative to that repo root so `python3 scripts/run-lifecycle-gates.py` works regardless of the caller's current working directory
    - leave the `subprocess.run(..., cwd=repo_root, ...)` call unchanged — after the `parents[1]` fix, `repo_root` is already the CRM repo root, so gate commands such as `python3 scripts/kg/validate.py` and `python3 planning-mds/testing/validate-nebula-api-contract.py` resolve correctly without further refactor
    - then trim any remaining framework-only assumptions from the copied runner
  - Rewrite `ci-gates.yml` in the CRM to invoke that product-local runner (no `python3 agents/scripts/run-lifecycle-gates.py` invocation; no `agents/**` paths, triggers, or checkouts).
  - The `ci-gates.yml` filename is reused for clarity; this product copy is unrelated to and does not coordinate with the framework copy that lives in nebula-agents (Step 10).
- Before CRM CI/branch protection is considered restored, re-home the remaining non-framework validation categories into product-local scripts and wire them into workflows plus `lifecycle-stage.yaml`:
  - `api_contract` → CRM-local validator (for example `scripts/validate-api-contract.py`) instead of `agents/architect/scripts/validate-api-contract.py`
  - `infra_strict` → CRM-local validator (for example `scripts/validate-infrastructure.py --strict`) instead of `agents/devops/scripts/validate-infrastructure.py`
  - `security_planning_strict` → CRM-local validator (for example `scripts/security-audit.py --strict`) instead of `agents/security/scripts/security-audit.py`
  - `solution_contract`, `knowledge_graph_sync`, and `frontend_quality` remain product-local as authored in Phase A
  - Planning-governance validators (`validate-stories.py`, `generate-story-index.py`, `validate-trackers.py`) remain framework-owned in `nebula-agents`. Do not copy them into the CRM repo or wire them into CRM-local Actions as part of this split.
- Known framework-coupling that must be rewritten before reintroduction:
  - **`frontend-ui.yml`** invokes `agents/frontend-developer/scripts/validate-frontend-ux-evidence.py` and triggers on changes to that script. Replace the validator step with a product-local equivalent (or drop the step) and remove the `paths:` triggers that reference `agents/**`.
  - **`ci-gates.yml`** runs `agents/scripts/run-lifecycle-gates.py` against the framework lifecycle config — do not reintroduce the framework runner in the CRM repo.
  - **`ci-gates.yml` `docker build -t nebula-builder .` step** must be dropped from the CRM copy. The framework builder `Dockerfile` lives in nebula-agents after the split, so this build is homed in nebula-agents' `ci-gates.yml` per Step 10, not here.
- Re-grep before commit: `grep -rn 'agents/' .github/workflows/` must return zero hits.
- Do **not** make the CRM repo check out or shell into `nebula-agents`.
- Product CI must remain product-local.

---

## 10. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Framework files in nebula-agents still contain bare product-owned paths (`scripts/kg/…`, `planning-mds/…`, `engine/…`, `experience/…`, `neuron/…`) after cleanup, causing agent commands to fail from the nebula-agents session root | Make the grep checks in Section 4.1 a Phase A blocking gate. Do not push nebula-agents until all path-indirection patterns return zero unintended hits |
| Product repo either retains live local references to framework paths or over-scrubs historical provenance trying to remove them everywhere | Scope the Phase A scrub to live/operator-facing surfaces only, exclude `planning-mds/features/archive/**` plus archival evidence payloads from the blocking grep, but keep live evidence index/template docs such as `planning-mds/operations/evidence/README.md` and `frontend-ux` / `frontend-quality` README/template files in scope |
| Workspace/output paths are created inside `nebula-crm`, or a rerun reuses stale target directories from a failed attempt | Step 0 defines `SOURCE_REPO_ROOT`, `WORKSPACE_ROOT`, `AGENTS_REPO_ROOT`, `PRODUCT_REPO_ROOT`, `SNAPSHOT_ROOT`, `BASELINE_CACHE`, and `SOURCE_REF_CACHE`, then fails fast unless `WORKSPACE_ROOT` sits outside `SOURCE_REPO_ROOT` and all target paths are absent before the run starts |
| Agent accidentally commits to nebula-agents instead of nebula-insurance-crm | Document explicitly in `nebula-agents/CONSUMER-CONTRACT.md` and in session start instructions: all product implementation commits go to `{PRODUCT_ROOT}` (default `../nebula-insurance-crm/`) |
| `{PRODUCT_ROOT}` placeholder is not resolved at session start, so commands execute against nothing | `agents/docs/AGENT-USE.md` Session Setup section defines the convention; CONSUMER-CONTRACT.md formalizes it; the first agent turn of every session should confirm the resolved value before issuing shell commands |
| Passing validator misinterpreted as "cleanup complete" when only domain-term leaks are covered | Section 4.4 makes explicit that the validator catches glossary terms only; path/brand/namespace leaks (Sections 4.1–4.3) need grep-based confirmation before declaring the split done |
| Framework docs/templates retain broken framework-internal self-references (for example `agents/BOUNDARY-POLICY.md`) after the split | Section 4.5 plus Step 5's explicit self-reference grep gate require rewriting impossible `agents/` prefixes for framework-root-owned files, followed by a short manual doc pass |
| Genericness validator loses its glossary source after split | Resolved in Phase A: Step 5 inlines the blocked-terms list into `validate-genericness.py` as a static fallback. Validator runs with zero arguments and zero sibling-repo dependency. The `--glossary` override flag is preserved for integrators who want to extend the denylist |
| Framework docs/actions still describe copying `agents/` into the product repo or running `init` in the current repo, so operators follow the old model after the split | Treat these docs as explicit Step 5 rewrite targets (`README.md`, `agents/README.md`, `agents/actions/init.md`, `agents/docs/FORK-AND-BUILD-APP.md`, `agents/docs/ONBOARDING.md`, `agents/docs/ARCHITECTURE.md`, `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`, `blueprint-setup/README.md`, `BOUNDARY-POLICY.md`, template docs, script READMEs, `CONTRIBUTING.md`) and block Phase A completion until the sibling-repo model is documented consistently |
| Validation ownership becomes blurred and the CRM either loses checks or reintroduces framework-local paths into product CI | Keep framework-owned vs product-local validations explicit in Section 3 and Section 5.2, and only restore CRM workflows once all product CI checks are runnable locally |
| `.github/workflows/ci-gates.yml` in nebula-agents runs CRM-specific gates or expects the CRM lifecycle file | Rebuild framework CI in Phase B against the new framework-local `lifecycle-stage.yaml` and keep the gate set framework-only |
| nebula-insurance-crm CI workflows still reference framework files | Reintroduce CRM workflows only after rewriting them to be fully product-local |
| The migration gets coupled to a feature-specific workstream and stalls on feature readiness questions | Keep the split feature-agnostic per Section 7; Phase A does not start or nominate feature execution |
| Operator accidentally commits to or modifies nebula-crm during the split, polluting the rollback backup | §1 and §9 Step 0 forbid any modification to nebula-crm. The split uses `git archive` against a detached worktree only — no write paths exist. Step 9 cleanup removes the worktree but never touches the chosen local source snapshot. Sanity check: `git -C "$SOURCE_REPO_ROOT" rev-parse "$SOURCE_SNAPSHOT_REF"` must still equal the baseline hash recorded in `.split-baseline` when Phase A finishes |
| `lifecycle-stage.yaml` carried over from nebula-crm references framework validators that no longer resolve from the CRM session root | Resolved by §3 / §6.2 / §9 Step 3+7: the file is not copied; it's authored fresh with product-local gates only |
| `agents/scripts/validate_templates.py` ships with broken imports or a hidden default dependency on product-side ontology files after being moved out of `scripts/kg/` | Step 5 explicitly requires fixing imports, removing product-side ontology defaults, and running its pytest before the framework commit lands |
| The CRM repo loses API / infrastructure / security enforcement by dropping framework-owned validators without product-local replacements | §6.2 and §9 Step 11 require those categories to be rehomed as CRM-local validators before Phase B CI restoration is considered complete |

---

## 11. What The Executing Agent Should Produce

By the end of this plan, the work should land in two phases.

**Phase A outputs**

**gajakannan/nebula-agents**
- [ ] All content from `agents/`, `blueprint-setup/`, `docker/agent-builder/`, builder `Dockerfile`, `docker-compose.agent-builder.yml`, `BOUNDARY-POLICY.md` (with **both** persona examples rewritten to a neutral non-insurance domain per Step 5), `CONTRIBUTING.md`, `LICENSE`
- [ ] Framework-coupled template validator moved from `scripts/kg/validate_templates.py` to `agents/scripts/validate_templates.py`, imports rewritten, test passing
- [ ] `agents/scripts/validate-genericness.py` carries an embedded denylist (no `--glossary` argument required for default operation)
- [ ] Rewritten `README.md`
- [ ] New `CONSUMER-CONTRACT.md` including the `{PRODUCT_ROOT}` convention with concrete env-var resolution mechanism, and the product-discovery convention
- [ ] `agents/docs/AGENT-USE.md` updated with the Session Setup section defining `{PRODUCT_ROOT}` resolution order
- [ ] Path indirection applied across `agents/**`: zero bare product-owned path references remain (`scripts/kg/`, `planning-mds/`, `engine/`, `experience/`, `neuron/`); all are product-root-aware
- [ ] Command examples no longer rely on implicit product-layer current working directories
- [ ] Framework scripts with product-relative defaults/examples are product-root-aware, including validator scripts plus frontend scaffolds (`scaffold-component.py`, `scaffold-page.py`)
- [ ] `agents/templates/feature-assembly-plan-template.md` reduced to shape-only skeleton with the knowledge-graph-discovery instruction (Section 4.3)
- [ ] Brand/namespace/filename cleanup applied per Section 4.2
- [ ] Framework-internal self-references cleaned per Section 4.5; no impossible `agents/`-prefixed references to framework-root-owned files remain
- [ ] All genericness leaks fixed and validator passing with zero arguments
- [ ] Existing framework onboarding/bootstrap docs rewritten for sibling-repo mode (`README.md`, `agents/README.md`, `agents/actions/init.md`, `agents/docs/FORK-AND-BUILD-APP.md`, `agents/docs/ONBOARDING.md`, `agents/docs/ARCHITECTURE.md`, `agents/docs/AGENT-USE.md`, `agents/docs/ORCHESTRATION-CONTRACT.md`, `agents/docs/MANUAL-ORCHESTRATION-RUNBOOK.md`, `blueprint-setup/README.md`, `BOUNDARY-POLICY.md`, `agents/templates/boundary-policy-template.md`, `CONTRIBUTING.md`, and script README examples)
- [ ] New root `lifecycle-stage.yaml` for framework-local validation only
- [ ] New `agents/docs/migration-from-nebula-crm.md`
- [ ] New `CHANGELOG.md`
- [ ] `.split-baseline` tracked file recording source commit
- [ ] `.gitignore` authored fresh (framework-tuned, not copied from nebula-crm)
- [ ] `agents/scripts/requirements.txt` updated/confirmed to include framework dev/test deps (pyyaml, pytest)
- [ ] Tagged `v0.1.0`

**gajakannan/nebula-insurance-crm**
- [ ] All content from `planning-mds/`, `engine/`, `experience/`, `neuron/`, `bruno/`, `docker/authentik/`, `docker/postgres/`, compose files, `pyproject.toml`, `LICENSE`, plus product-owned portions of `scripts/`
- [ ] Rewritten `README.md`
- [ ] New `docs/migration-from-nebula-crm.md`
- [ ] New `CHANGELOG.md` recording the split provenance
- [ ] `lifecycle-stage.yaml` freshly authored with product-local gates only — never derived from nebula-crm's version, and not used to nominate a split-driven active feature
- [ ] Product repo live/operator-facing surfaces contain no local references to `agents/**`, `../nebula-agents/**`, `.claude/`, `CLAUDE.md`, `AGENTS.md`, or similar framework bootstrap files (verified by Step 4 grep gates); historical archive/evidence provenance may retain past command traces
- [ ] Framework-coupled script exceptions removed from the product repo (`scripts/kg/validate_templates.py`, `scripts/kg/tests/test_validate_templates.py`, `scripts/kg/pretool_hook.py`)
- [ ] `.split-baseline` tracked file recording source commit
- [ ] `.gitignore` and `.dockerignore` carried over and trimmed for the CRM boundary, while retaining generic Python artifact ignores needed by `scripts/`, `planning-mds/testing/`, and future `neuron/` work

**Phase B outputs**

**gajakannan/nebula-agents**
- [ ] Framework CI restored and passing against framework-only gates

**gajakannan/nebula-insurance-crm**
- [ ] Product CI restored and passing with product-local checks only
- [ ] Product-local `api_contract`, `infra_strict`, and `security_planning_strict` validators rehomed and wired into CRM workflows / `lifecycle-stage.yaml`
- [ ] Product-local lifecycle runner (`scripts/run-lifecycle-gates.py` or equivalent) authored with corrected repo-root / config-path resolution for its new location, and `ci-gates.yml` rewritten to invoke it against the product `lifecycle-stage.yaml` with zero `agents/**` references

**Both repos**
- [ ] Cross-repo dry-run validation (Section 9, Step 8) completed and passing
- [ ] No feature/story execution is part of Phase A; the split ends at validated repo boundaries
- [ ] `nebula-crm` is unchanged: the chosen local source snapshot still matches the recorded baseline hash, snapshot worktree at `"$SNAPSHOT_ROOT"` removed

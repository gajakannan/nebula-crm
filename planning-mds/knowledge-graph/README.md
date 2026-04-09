# Nebula Solution Ontology Seed

This folder holds the lightweight ontology and mapping layer for the Nebula
solution graph.

It exists to compress repeated cross-feature context, not to replace the source
documents in `planning-mds/`.

## Current Files

- `solution-ontology.yaml` - node types, edge types, ID rules, precedence, and
  ownership
- `canonical-nodes.yaml` - v0 shared canonical nodes for entities, workflows,
  workflow states, capabilities, roles, policy rules, ADRs, schemas, and API
  contracts
- `feature-mappings.yaml` - v0 feature/story links into the canonical layer
- `code-index.yaml` - implementation bindings from stable node IDs into
  `engine/`, `experience/`, and other code-bearing paths
- `coverage-report.yaml` - generated coverage and freshness report for mapped
  scope, exclusions, and bound implementation surfaces

## Authority Rules

Use these files as retrieval aids only.

If they conflict with raw project artifacts, the raw artifacts win in this
order:

1. Target feature folder and `feature-assembly-plan.md` for feature-local
   implementation intent
2. `planning-mds/architecture/decisions/**` for architectural decisions
3. `planning-mds/api/*.yaml` and `planning-mds/schemas/*.json` for contracts
4. `planning-mds/architecture/data-model.md` and `planning-mds/domain/glossary.md`
   for domain definitions
5. `planning-mds/knowledge-graph/*.yaml` for compressed retrieval and routing

If drift is discovered, repair the authoritative source first when needed, then
repair the ontology mapping in the same change set.

## Ownership

- Architect owns canonical shared nodes: `entity`, `glossary_term`, `workflow`,
  `workflow_state`, `capability`, `endpoint`, `ui_route`, `event`, `config_key`,
  `migration`, `role`, `policy_rule`, `schema`, `api_contract`, and `adr`.
  Architect also maintains `code-index.yaml` (implementation bindings) and the
  generated `coverage-report.yaml` artifact.
- Product Manager owns planning-facing nodes and links: `feature`, `story`,
  `persona`, `evidence`, and feature/story mapping freshness.
- Implementation agents do not silently redefine canonical solution semantics.
  They should flag drift and route it back to the Architect or Product Manager
  unless explicitly working in one of those roles.

## Prompt Usage

When a target feature or story has coverage in `feature-mappings.yaml`:

1. Load `solution-ontology.yaml`.
2. Load `canonical-nodes.yaml`.
3. Load the matching feature/story entry from `feature-mappings.yaml`.
4. Load `code-index.yaml` when implementation file routing or reverse lookup is
   needed.
5. Use that subgraph as the first-pass routing context.
6. Open raw ADR/schema/API/feature files only when they are linked, changed, or
   needed for detail or verification.

When coverage does not exist yet, fall back to the current file-centric prompt
pattern.

## Tooling

- `python3 scripts/kg/validate.py` validates IDs, references, paths, feature
  coverage, code-index bindings, and report freshness.
- `python3 scripts/kg/validate.py --write-coverage-report` refreshes the
  committed `coverage-report.yaml` artifact.
- `python3 scripts/kg/lookup.py F0007-S0003` returns the merged ontology scope
  for a mapped feature or story.
- `python3 scripts/kg/lookup.py --file engine/src/Nebula.Domain/Entities/Submission.cs`
  performs reverse lookup from a code file back to ontology nodes and related
  planning scope.

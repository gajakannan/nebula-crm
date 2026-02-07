# Inception Setup (Meta)

This folder contains **process/meta** guidance for starting a new project. It is **not** part of solution-specific planning content.

## Bootstrap Steps (New Project)

1) Create `planning-mds/INCEPTION.md` with sections 0–2 (context, scope, constraints).
2) Create `planning-mds/domain/` and add your glossary + competitive analysis.
3) Create `planning-mds/architecture/SOLUTION-PATTERNS.md` from `agents/templates/solution-patterns-template.md`.
4) Create `planning-mds/examples/` with at least one persona, feature, and story example.
5) Create `planning-mds/features/` and `planning-mds/stories/` for actual requirements.
6) Create `planning-mds/screens/` and `planning-mds/workflows/` for UI and state specs.

## Minimal Folder Scaffold

```bash
mkdir -p planning-mds/{domain,examples,features,stories,screens,workflows,architecture,api,security,testing,operations}
mkdir -p planning-mds/examples/{personas,features,stories,screens,architecture,architecture/adrs}
```

## INCEPTION.md Outline (Minimal)

```
0) How we will work (process + roles)
1) Product context (what we’re building, users, core entities)
2) Platform/technology constraints (if any)
3) Product Manager spec (vision, personas, epics/features, stories, screens)
4) Architect spec (service boundaries, data model, workflows, auth, APIs, NFRs)
```

## Template Pointers

Use these generic templates from `agents/templates/`:
- `story-template.md`
- `feature-template.md`
- `persona-template.md`
- `screen-spec-template.md`
- `workflow-spec-template.md`
- `api-contract-template.yaml`
- `solution-patterns-template.md`

## Examples (Non-Insurance)

See `inception-setup/examples/` for small, partial examples across different domains.

## First 4 Artifacts (Minimum)

1) `planning-mds/INCEPTION.md`
2) `planning-mds/domain/[project]-glossary.md`
3) `planning-mds/architecture/SOLUTION-PATTERNS.md`
4) `planning-mds/examples/stories/[project]-story-example.md`

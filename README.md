# Agent-Driven Builder Framework + Insurance CRM Reference

This repository serves two purposes:

1) A reusable, agent-driven development framework (the generic parts you can copy to any project).
2) A concrete, solution-specific example (an insurance CRM called Nebula) to demonstrate how the framework is used.

The separation is intentional. Generic agents live in `agents/`. Solution-specific planning artifacts live in `planning-mds/`.

## Quick Orientation

- New project? Start with `inception-setup/README.md` and copy `agents/` into your repo.
- Exploring the insurance CRM example? Read `planning-mds/INCEPTION.md` and the example artifacts under `planning-mds/examples/`.
- Want the boundary rules? See `BOUNDARY-POLICY.md`.

## Repository Layout (By Intent)

- `agents/` - Generic, reusable agent roles, templates, and references. Copy as-is.
- `planning-mds/` - Solution-specific requirements, examples, and decisions (Nebula CRM in this repo). Replace for a new project.
- `inception-setup/` - Bootstrap guidance for starting a new project.
- `engine/`, `experience/`, `soma/` - Implementation layers (currently placeholders in this repo).
- `docs/` - Meta documentation and audits.

## Reuse Workflow (New Project)

1) Copy `agents/` into your new repo unchanged.
2) Create a fresh `planning-mds/` in your new repo.
3) Populate `planning-mds/` with your domain glossary, requirements, and examples.
4) Use the agent roles to produce outputs into `planning-mds/` and then implement code in your project.

This keeps the framework reusable and the solution content replaceable.

## The Example (Nebula Insurance CRM)

Everything under `planning-mds/` in this repo is specific to the Nebula insurance CRM. Treat it as a reference example only.
When you start a new project, replace all `planning-mds/` content with your own domain knowledge and requirements.

## Tech Stack Assumptions

The framework is opinionated about delivery practices and provides stack-specific references in some agent guides.
In this repo, the default references assume a modern .NET + React + PostgreSQL stack. If you adopt a different stack,
keep the agent roles but replace the stack-specific reference guides and examples with your own.

## Key Documents

- `agents/README.md` - How to use the generic agents.
- `planning-mds/README.md` - What belongs in solution-specific planning.
- `BOUNDARY-POLICY.md` - Rules that separate generic vs solution-specific content.
- `inception-setup/README.md` - Bootstrap steps for a new project.
- `docs/FAQ.md` - Common questions about reuse, stacks, and boundaries.

## Why This Exists

The goal is to prove out AI-agentic driven development in a reusable way, while also demonstrating the approach with a
real, end-to-end example (insurance CRM). This repo intentionally contains both; the boundary is the key.

## Boundary Policy (Short Version)

- `agents/` is generic and reusable across projects.
- `planning-mds/` is solution-specific and should be replaced for each new project.
- Agents read from `planning-mds/` but must not embed solution-specific requirements.
- Templates and reusable examples live under `agents/templates/` and `agents/**/references/`.
- Domain knowledge, examples, and decisions live under `planning-mds/`.

See `BOUNDARY-POLICY.md` for the full policy.

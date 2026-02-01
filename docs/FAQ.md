# Frequently Asked Questions

## Can I use this framework with a different tech stack?

Yes. The roles and templates are reusable; only stack-specific references and examples need changes. See `agents/TECH-STACK-ADAPTATION.md`.

## Can I use this for non-CRM projects?

Yes. The framework is domain-agnostic. See `inception-setup/examples/` for non-insurance examples.

## What do I copy to start a new project?

Copy:
- `agents/`
- `inception-setup/`
- `README.md` (update title and project framing)

Create new:
- `planning-mds/` from scratch

## Where do I find a step-by-step onboarding checklist?

See `docs/ONBOARDING.md`.

## How do I know if something belongs in agents/ vs planning-mds/?

Use the boundary rules in `BOUNDARY-POLICY.md`.

## Can I modify agent roles?

Yes, but keep them generic. Put project-specific notes and requirements in `planning-mds/`.

## What if my agents need different workflows?

Adapt the `SKILL.md` files, but keep them reusable across similar projects. Use `planning-mds/` for project-specific variations.

# Container Strategy

This framework uses a two-container model:

1. Builder runtime container (framework tooling)
2. Application runtime container(s) (generated solution services)

## 1) Builder Runtime Container

Purpose:
- Hosts agent role definitions, action docs, scripts, and templates used to orchestrate delivery.
- Provides a reproducible execution environment for running framework workflows.

Contains:
- `agents/`
- `inception-setup/`
- `scripts/`
- framework documentation

Does not contain:
- project databases
- production secrets
- long-lived application runtime state

Usage:
- Build via root `Dockerfile`.
- Run interactively to execute planning/build workflows in a mounted workspace.

## 2) Application Runtime Container(s)

Purpose:
- Run the generated application stack (backend, frontend, database, and optional services).

Produced by:
- `build` / `feature` actions and implementation agents (Backend, Frontend, AI Engineer, DevOps).

Typical services:
- API/backend service
- frontend service
- database service
- optional auth/cache/queue/worker services

Template:
- Start from `agents/templates/docker-compose.app-template.yml` and customize per project.

## Relationship

```text
Builder Container
  -> reads/writes planning and implementation artifacts
  -> coordinates agent workflows
  -> outputs app runtime configs

Application Containers
  -> run generated services
  -> validated by QA/review/security gates
```

## Notes

- The builder and application runtimes are intentionally separate concerns.
- The builder runtime should not be treated as a production app deployment container.
- Application container strategy is project-specific and evolves with architecture decisions in `planning-mds/`.

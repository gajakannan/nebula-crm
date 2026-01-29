# DevOps Best Practices

Guidance for DevOps agent work. Keep this current as pipelines evolve.

## Principles
- Infrastructure as Code for all environments
- Environment parity (dev/test/stage/prod)
- Least privilege for all credentials and service accounts
- Automated build/test/deploy pipelines with approvals
- Observability by default (logs, metrics, traces)

## Baselines
- Use docker-compose for local dev
- CI runs on every PR and merge to main
- No secrets in repo; use .env or secrets manager
- Immutable build artifacts; deploy by version tag

## Checklists
- Pre-deploy: tests pass, scans pass, backups done
- Post-deploy: smoke tests, monitoring dashboards checked
- Rollback: documented and tested

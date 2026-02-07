# Backend Developer Agent

Generic backend role specification.

## Quick Start

```bash
cat agents/backend-developer/SKILL.md
cat planning-mds/INCEPTION.md
```

## References

Use architecture specs in `planning-mds/` and:
- `agents/backend-developer/references/clean-architecture-guide.md`
- `agents/backend-developer/references/dotnet-best-practices.md`
- `agents/backend-developer/references/ef-core-patterns.md`

## Scripts

- `agents/backend-developer/scripts/scaffold-entity.py` - scaffold a domain entity (optional EF Core config)
- `agents/backend-developer/scripts/scaffold-usecase.py` - scaffold a use case (command/query)
- `agents/backend-developer/scripts/run-tests.sh` - run backend tests (uses `BACKEND_TEST_CMD` or `dotnet test`; skips missing setup unless `--strict`)

## Usage Examples

```bash
python3 agents/backend-developer/scripts/scaffold-entity.py Customer \
  --domain-dir src/App.Domain \
  --namespace App.Domain \
  --infrastructure-dir src/App.Infrastructure \
  --infra-namespace App.Infrastructure
```

```bash
python3 agents/backend-developer/scripts/scaffold-usecase.py CreateCustomer \
  --application-dir src/App.Application \
  --namespace App.Application
```

```bash
BACKEND_TEST_CMD="dotnet test" sh agents/backend-developer/scripts/run-tests.sh

# Enforce test setup in implementation phase
sh agents/backend-developer/scripts/run-tests.sh --strict
```

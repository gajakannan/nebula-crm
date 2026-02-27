# F0003 — Task Center + Reminders (API-only MVP) — Getting Started

## Prerequisites

- [ ] Backend API running (`engine/src/Nebula.Api`)
- [ ] Auth configured

## Services to Run

```bash
docker compose up -d postgres keycloak
dotnet run --project engine/src/Nebula.Api
```

## How to Verify

1. Confirm task write endpoints are intentionally deferred for current scope.
2. Validate task read behaviors used by dashboard widgets.

---
template: feature-getting-started
version: 1.0
applies_to: product-manager
---

# Feature GETTING-STARTED Template

Practical setup guide for developers and agents implementing this feature. The PM creates a skeleton; implementing agents fill in details as they build.

Place as `GETTING-STARTED.md` inside each feature folder.

---

# F{NNNN} — [Feature Name] — Getting Started

## Prerequisites

- [ ] [Required service or dependency]
- [ ] [Database migration applied]
- [ ] [Seed data loaded]

## Services to Run

```bash
# List the services needed to work on this feature
# Example:
# docker compose up -d postgres
# dotnet run --project engine/src/MyApp.Api
# cd experience && pnpm dev
```

## Environment Variables

| Variable | Purpose | Default |
|----------|---------|---------|
| [VAR_NAME] | [What it controls] | [Default value] |

## Seed Data

Describe any seed data required for this feature to function:
- [Entity]: [What is seeded and why]

## How to Verify

Steps to confirm the feature works end-to-end:

1. [Step 1 — e.g., navigate to a specific URL]
2. [Step 2 — e.g., perform an action]
3. [Step 3 — e.g., verify expected result]

## Key Files

| Layer | Path | Purpose |
|-------|------|---------|
| Backend | `engine/src/...` | [What this file does] |
| Frontend | `experience/src/...` | [What this file does] |

## Notes

- [Any gotchas, workarounds, or context that helps implementers]

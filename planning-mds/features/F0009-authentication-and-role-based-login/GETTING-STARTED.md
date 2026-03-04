# F0009 — Authentication + Role-Based Login — Getting Started

## Prerequisites

- [ ] Backend API running (`engine/src/Nebula.Api`)
- [ ] Frontend app running (`experience`)
- [ ] authentik services running and reachable
- [ ] Test identities provisioned with expected role claims

## Services to Run

```bash
docker compose up -d db authentik-server authentik-worker
dotnet run --project engine/src/Nebula.Api
cd experience && pnpm dev
```

## Test Users (Non-Production)

| Email | Expected Role | Expected Entry |
|------|---------------|----------------|
| `lisa.wong@nebula.local` | `DistributionUser` | Internal dashboard/work queues |
| `john.miller@nebula.local` | `Underwriter` | Underwriter-appropriate dashboard view |
| `broker001@example.local` | `BrokerUser` | Broker-constrained workspace |

Note: `lisa.wong@nebula.local` and `john.miller@nebula.local` already exist in `DevSeedData` user profiles. `broker001@example.local` exists as seeded broker data and requires a mapped IdP `BrokerUser` identity before login validation.

## How to Verify

1. Open the login screen route.
2. Sign in as each test user and confirm role-appropriate landing behavior.
3. Confirm protected routes redirect/deny when session is missing or expired.
4. Confirm broker user cannot access internal-only views/endpoints.
5. Confirm logout returns user to login screen and blocks previous routes.

## Troubleshooting

- If login succeeds but app denies access, verify `nebula_roles` claim mapping in authentik.
- If callback fails, verify redirect URI configuration for the Nebula client.
- If broker user sees internal data, block release and validate ABAC policy boundaries first.

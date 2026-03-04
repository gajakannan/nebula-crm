# F0009 — Authentication + Role-Based Login — Status

**Overall Status:** Draft
**Last Updated:** 2026-03-04

## Story Checklist

| Story | Title | Status | Notes |
|-------|-------|--------|-------|
| F0009-S0001 | Login Screen and OIDC Redirect | Draft | |
| F0009-S0002 | OIDC Callback and Session Bootstrap | Draft | |
| F0009-S0003 | Role-Based Entry and Protected Navigation | Draft | |
| F0009-S0004 | BrokerUser Access Boundaries | Draft | |
| F0009-S0005 | Seeded User Access Validation Matrix | Draft | |

## Prerequisite Tracking

| Prerequisite | Status | Notes |
|-------------|--------|-------|
| F0005 IdP migration remains stable | Ready | authentik stack and claim normalization available |
| BrokerUser role added to authorization matrix/policy | Not Started | required before broker login activation |
| Broker-visible data boundary list approved | Not Started | required for safe external access |
| Real-login frontend mode available (without `dev-auth.ts`) | Not Started | can ship behind feature flag initially |

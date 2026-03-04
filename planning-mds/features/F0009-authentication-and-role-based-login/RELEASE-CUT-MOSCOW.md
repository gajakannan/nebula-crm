# F0009 — Release Cut (MoSCoW)

**Feature:** F0009 — Authentication + Role-Based Login  
**Decision Date:** 2026-03-04  
**Owner:** Product Manager

## Must Have (Release Blocking)

- F0009-S0001: Login Screen and OIDC Redirect
- F0009-S0002: OIDC Callback and Session Bootstrap
- F0009-S0003: Role-Based Entry and Protected Navigation
- BrokerUser authorization policy delta approved and implemented server-side for allowed routes only
- Seed users provisioned and validated for:
  - `lisa.wong@nebula.local` (`DistributionUser`)
  - `john.miller@nebula.local` (`Underwriter`)
  - `broker001@example.local` (`BrokerUser`)
- Regression check: no protected route accessible without authenticated session
- Regression check: broker user cannot retrieve cross-broker records or InternalOnly fields

## Should Have (Strongly Preferred)

- F0009-S0004: BrokerUser Access Boundaries fully implemented with explicit broker-visible field mapping
- F0009-S0005: Seeded User Access Validation Matrix automated in test runbook
- Friendly unauthorized and expired-session UX with trace IDs
- Feature flag to disable `dev-auth` mode in staging by default

## Could Have (Valuable, Not Blocking)

- Enhanced login observability dashboard (error categories, callback failure trend)
- Multi-role landing preference override for users with more than one role
- In-app help panel for common sign-in issues

## Won't Have (This Release)

- User self-registration
- Password reset UX inside Nebula app (IdP-hosted flow only)
- MFA policy customization in Nebula
- Broker CRUD/submission/renewal self-service flows
- Cross-application single logout

## Release Guardrails

- If BrokerUser field-level boundaries are not verified, release defaults to internal-role login only.
- If role claims are inconsistent (`nebula_roles` missing/malformed), release is blocked.
- Any endpoint-level authorization gap found in protected routes is a release blocker.

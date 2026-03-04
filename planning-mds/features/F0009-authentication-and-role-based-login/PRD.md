# F0009: Authentication + Role-Based Login

**Feature ID:** F0009
**Feature Name:** Authentication + Role-Based Login
**Priority:** Critical
**Phase:** Phase 1

## Feature Statement

**As a** Nebula user (DistributionUser, Underwriter, or Broker)
**I want** to sign in through a real login screen and enter a role-appropriate workspace
**So that** access is secure, explicit, and no longer dependent on development token helpers

## Business Objective

- **Goal:** Replace implicit dev-token access with production-style login and session entry for internal and broker personas.
- **Metrics:**
  - 95% of successful logins complete in <= 5 seconds (excluding third-party IdP outage windows).
  - 100% of protected routes require authenticated session (no anonymous data access).
  - 100% of role checks enforce least-privilege visibility for DistributionUser, Underwriter, and Broker users.
- **Baseline:** Frontend currently injects bearer tokens via `dev-auth.ts`; no user-visible login screen or callback flow.
- **Target:** Development and staging environments use real sign-in and role-scoped navigation without `dev-auth.ts` dependency.

## Problem Statement

- **Current State:** Users are effectively auto-authenticated in frontend via development helper token generation.
- **Desired State:** Users sign in explicitly, are verified by authentik/OIDC, and land in role-appropriate pages with strict route guards.
- **Impact:** Reduces security risk, enables realistic user acceptance testing, and unblocks broker-role onboarding scenarios.

## Scope & Boundaries

**In Scope:**
- Login entry screen for Nebula web app.
- OIDC sign-in redirect/callback flow for real user sessions.
- Role-based post-login entry for:
  - DistributionUser (`lisa.wong@nebula.local`)
  - Underwriter (`john.miller@nebula.local`)
  - BrokerUser (`broker001@example.local`)
- Route protection for authenticated and authorized access.
- Session expiry, unauthorized, and logout UX states.
- Seeded non-production identities for acceptance testing.

**Out of Scope:**
- User registration and self-service account creation.
- Password reset implementation inside Nebula app (delegated to IdP hosted flow).
- MFA policy design changes (remain IdP-controlled).
- Full broker self-service servicing workflows (quotes, submissions, document upload).
- Replacing authentik with another IdP.

## Broker Visibility Rules (Product Requirements)

These rules apply whenever a BrokerUser is authenticated:

1. Broker users can view only records belonging to their broker organization.
2. Cross-broker visibility is not allowed under any condition.
3. Every exposed data element must be classified as either `BrokerVisible` or `InternalOnly`.
4. Broker user responses must not include `InternalOnly` fields.
5. If business scope or ownership linkage cannot be determined, access is denied (default deny).

## Gaps Identified in Current State

1. No login UI route exists in the frontend; app routes open directly into application pages.
2. API client always calls `getDevToken()`; no real browser session bootstrap exists.
3. Full OIDC callback/session lifecycle is documented as future in F0005-S0003, but not productized.
4. Authorization policy matrix currently does not define a dedicated `BrokerUser` role with explicit allowed actions.
5. Existing MVP non-goal excludes external broker portal access; this feature requires a scoped Phase 1 exception for broker login.

## Prerequisites

- F0005 IdP migration remains complete and stable in target environments.
- Authorization matrix and Casbin policy are extended to include `BrokerUser` rules and deny boundaries.
- Broker-visible vs internal-only data boundaries are explicitly listed for all pages exposed to BrokerUser.
- Test identities are provisioned in authentik with role claims in `nebula_roles`.

## Success Criteria

- All three target test users can authenticate through the login UI and reach allowed pages.
- Unauthorized navigation attempts return deterministic 403/redirect behavior.
- Session expiration and logout are handled without blank/error states.
- Frontend can run in real-login mode without relying on `dev-auth.ts`.
- Broker users cannot retrieve records or fields outside their broker organization scope.

## Risks & Assumptions

- **Risk:** Introducing broker login before broker-facing page hardening could expose internal-only content.
- **Risk:** Role claim mismatches (`role` vs `nebula_roles`) can cause false authorization denies.
- **Assumption:** `broker001@example.local` is a non-production pilot identity mapped to a constrained BrokerUser role and broker tenant scope.
- **Mitigation:** Enforce server-side authorization rules first; treat frontend visibility as defense-in-depth, not primary control.

## Edge Cases To Specify and Validate

- Shared account linked to multiple brokers: visible only when explicit broker linkage exists for the signed-in broker user.
- Deactivated or revoked broker relationship: access is removed immediately.
- Missing scope linkage (identity maps to no broker): deny access and provide permission-safe error state.

## Dependencies

- F0005 (authentik migration and claim normalization)
- Authorization artifacts in `planning-mds/security/policies/`
- Route-level and endpoint-level authorization checks across dashboard/broker/task/timeline modules

## Related User Stories

- F0009-S0001 - Login Screen and OIDC Redirect
- F0009-S0002 - OIDC Callback and Session Bootstrap
- F0009-S0003 - Role-Based Entry and Protected Navigation
- F0009-S0004 - BrokerUser Access Boundaries
- F0009-S0005 - Seeded User Access Validation Matrix

## Rollout & Enablement

- Add QA runbook for test-user sign-in paths and expected access results.
- Keep a temporary feature flag for dev-auth fallback during transition only.
- Publish support notes for common login failures (expired session, missing role claim, unauthorized role).

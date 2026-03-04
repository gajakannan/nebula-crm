# F0009-S0001: Login Screen and OIDC Redirect

**Story ID:** F0009-S0001
**Feature:** F0009 — Authentication + Role-Based Login
**Title:** Provide login entry screen and IdP sign-in redirect
**Priority:** Critical
**Phase:** Phase 1

## User Story

**As a** Nebula user
**I want** a clear login screen that starts sign-in with the identity provider
**So that** I can authenticate through a supported, explicit user flow

## Context & Background

The current frontend enters application routes directly and obtains tokens through a development helper. A dedicated login screen is required before real-user sign-in can be validated.

## Acceptance Criteria

- **Given** I open Nebula without an active session
- **When** I navigate to any protected route
- **Then** I am redirected to the login screen

- **Given** I do not have valid authentication or authorization for a protected resource
- **When** I attempt direct route access
- **Then** access is blocked and I am redirected to login

- **Given** I am not an authorized user for an internal route
- **When** I attempt to continue past login
- **Then** permission-safe unauthorized behavior is enforced

- **Given** I am on the login screen
- **When** I click "Sign in"
- **Then** I am redirected to the configured OIDC authorization endpoint

- **Given** the identity provider is unavailable
- **When** I attempt sign-in
- **Then** I see a deterministic error state with retry guidance

- **Given** a login entry event occurs
- **When** diagnostics are captured
- **Then** traceable telemetry is emitted for support triage (no credential payload logged)

- Edge case: attempting to load callback route directly without OIDC state returns a safe error and login retry path.

## Data Requirements

**Required Fields:**
- OIDC authority URL
- OIDC client ID
- Redirect URI

**Optional Fields:**
- Post-login default route override
- Support/help URL

**Validation Rules:**
- OIDC configuration must be present before rendering an enabled sign-in action
- Callback route must validate required OIDC state parameters

## Role-Based Visibility

**Roles that can use this flow:**
- DistributionUser — sign in
- Underwriter — sign in
- BrokerUser — sign in

**Data Visibility:**
- InternalOnly content: diagnostics metadata and support trace identifiers
- ExternalVisible content: login page copy and generic sign-in error messaging

## Non-Functional Expectations

- Performance: login screen first render p95 <= 1.5 seconds on broadband
- Security: no token, secret, or credential values logged in browser console
- Reliability: sign-in redirect failure rate <= 1% excluding IdP outages

## Dependencies

**Depends On:**
- F0005 authentik OIDC baseline
- Frontend route guard framework

**Related Stories:**
- F0009-S0002 — OIDC callback/session bootstrap

## Out of Scope

- Password reset UI implementation
- User registration
- MFA enrollment screens

## UI/UX Notes

- Login screen includes product identity, single primary "Sign in" call to action, and support link.
- Avoid role picker on login page; role resolution is claims-driven after sign-in.

## Questions & Assumptions

**Open Questions:**
- [ ] Should staging expose a temporary "use dev-auth" fallback toggle for support teams?

**Assumptions (to be validated):**
- IdP-hosted login handles credential entry, password policy, and MFA prompts.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Security/diagnostic telemetry validated
- [ ] Tests pass
- [ ] Documentation updated (if needed)

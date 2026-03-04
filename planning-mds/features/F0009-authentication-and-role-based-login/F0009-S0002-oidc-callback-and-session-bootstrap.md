# F0009-S0002: OIDC Callback and Session Bootstrap

**Story ID:** F0009-S0002
**Feature:** F0009 — Authentication + Role-Based Login
**Title:** Establish session from OIDC callback and bootstrap user context
**Priority:** Critical
**Phase:** Phase 1

## User Story

**As a** authenticated Nebula user
**I want** the app to process OIDC callback results and initialize my session
**So that** I can access protected data without manual token handling

## Context & Background

A login redirect alone is insufficient; the app must securely handle callback state, bootstrap identity context, and load protected routes only after session readiness.

## Acceptance Criteria

- **Given** the IdP returns a valid callback response
- **When** the callback route is processed
- **Then** the app establishes session state and navigates to authorized entry route

- **Given** callback state, nonce, or code validation fails
- **When** callback processing runs
- **Then** session creation is rejected and the user is returned to login with actionable error messaging

- **Given** I refresh the browser with an active session
- **When** app bootstrap runs
- **Then** protected routes remain accessible without forced re-login

- **Given** my session is expired
- **When** I navigate to a protected route
- **Then** I am redirected to login and stale session state is cleared

- Edge case: callback endpoint is opened multiple times for the same authorization code; second attempt fails safely without app crash.

## Data Requirements

**Required Fields:**
- Authorization code
- OIDC state
- ID token/access token claims required for user bootstrap

**Optional Fields:**
- Requested return URL after login

**Validation Rules:**
- Callback payload must include expected state correlation value
- Session bootstrap must fail closed on claim parsing errors

## Role-Based Visibility

**Roles that can complete callback/bootstrap:**
- DistributionUser — yes
- Underwriter — yes
- BrokerUser — yes

**Data Visibility:**
- InternalOnly content: session diagnostic details and claim parsing traces
- ExternalVisible content: generic callback success/failure outcomes

## Non-Functional Expectations

- Performance: callback processing and first authorized render p95 <= 3 seconds
- Security: callback validation failures do not disclose token internals
- Reliability: refresh with active session succeeds >= 99% in non-outage periods

## Dependencies

**Depends On:**
- F0009-S0001 login redirect flow
- Backend/user profile claim normalization in F0005

**Related Stories:**
- F0009-S0003 — role-based entry and protected navigation

## Out of Scope

- Cross-application single logout
- Session sharing across browser profiles/devices

## UI/UX Notes

- Callback route should render a short-lived loading state and deterministic fallback errors.
- Error messages should include a support trace identifier when available.

## Questions & Assumptions

**Open Questions:**
- [ ] Is silent token renewal required in Phase 1, or is re-authentication on expiry acceptable?

**Assumptions (to be validated):**
- Existing backend accepts authentik-issued JWTs and exposes required protected data endpoints after session bootstrap.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Session bootstrap telemetry verified
- [ ] Tests pass
- [ ] Documentation updated (if needed)

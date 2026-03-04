# F0009-S0005: Seeded User Access Validation Matrix

**Story ID:** F0009-S0005
**Feature:** F0009 — Authentication + Role-Based Login
**Title:** Provide seeded user identities and validate role-specific login outcomes
**Priority:** High
**Phase:** Phase 1

## User Story

**As a** QA or product reviewer
**I want** deterministic seeded users with expected role outcomes
**So that** I can verify login and authorization behavior quickly and repeatedly

## Context & Background

This feature requires clear test personas. Existing dev-auth flows do not guarantee real IdP-backed role behavior or broker-access constraints.

## Acceptance Criteria

- **Given** the environment is initialized
- **When** identity seeding completes
- **Then** the following users can authenticate successfully:
  - `lisa.wong@nebula.local` (`DistributionUser`)
  - `john.miller@nebula.local` (`Underwriter`)
  - `broker001@example.local` (`BrokerUser`)

- **Given** each seeded user signs in
- **When** post-login route resolution completes
- **Then** landing route and navigation visibility match the role matrix defined in this feature

- **Given** each seeded user calls protected APIs outside role scope
- **When** the API evaluates authorization
- **Then** a deterministic 403 is returned with standard problem details code

- **Given** each seeded user calls protected APIs within role scope
- **When** permission and authorization checks pass
- **Then** the API returns successful responses for allowed actions only

- **Given** `broker001@example.local` is signed in
- **When** data for another broker organization is requested
- **Then** access is denied and no cross-broker data is returned

- **Given** `broker001@example.local` requests data where ownership mapping is missing
- **When** access evaluation occurs
- **Then** access is denied by default

- **Given** seeded identities are re-applied
- **When** setup runs multiple times
- **Then** seeding remains idempotent and does not create duplicates

- **Given** seeded identities are created or updated
- **When** provisioning completes
- **Then** an audit log record is produced for seed operation outcomes

- Edge case: if one seeded user is missing required role claim, validation fails with explicit setup error before UI testing proceeds.

## Data Requirements

**Required Fields:**
- Email
- Role claim (`nebula_roles`)
- Stable subject identifier in IdP

**Optional Fields:**
- Display name
- Region claim

**Validation Rules:**
- Seeded emails must be unique
- Role claim must map to one supported role for this matrix
- Validation matrix must include expected allow/deny routes per user
- Validation matrix must include cross-organization deny checks for broker user paths

## Role-Based Visibility

**Roles covered in matrix:**
- DistributionUser
- Underwriter
- BrokerUser

**Data Visibility:**
- InternalOnly content: full matrix diagnostics and policy trace details
- ExternalVisible content: none (test artifact only)

## Non-Functional Expectations

- Performance: full seeded-user validation run completes in <= 10 minutes in local environment
- Security: seeded credentials are non-production and not committed as plaintext secrets in source
- Reliability: rerunning setup preserves deterministic user-role mappings

## Dependencies

**Depends On:**
- F0009-S0001 through F0009-S0004
- authentik blueprint/seeding workflow

**Related Stories:**
- F0005-S0001 — authentik infrastructure baseline

## Out of Scope

- Production identity provisioning
- HR-driven lifecycle automation for real users
- Enterprise directory synchronization

## UI/UX Notes

- Provide a simple validation checklist in `GETTING-STARTED.md` for each seeded user.
- Error output should identify which user/role check failed without exposing secrets.

## Questions & Assumptions

**Open Questions:**
- [ ] Should broker pilot user be limited to a single broker tenant in Phase 1?

**Assumptions (to be validated):**
- Seeded users are for development and staging validation only.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Seed idempotency and matrix checks pass
- [ ] Tests pass
- [ ] Documentation updated (if needed)

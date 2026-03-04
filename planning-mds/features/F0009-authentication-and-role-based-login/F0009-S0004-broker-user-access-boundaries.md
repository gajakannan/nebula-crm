# F0009-S0004: BrokerUser Access Boundaries

**Story ID:** F0009-S0004
**Feature:** F0009 — Authentication + Role-Based Login
**Title:** Define and enforce BrokerUser access boundaries
**Priority:** Critical
**Phase:** Phase 1

## User Story

**As a** broker user
**I want** to sign in and view only broker-appropriate data
**So that** I can collaborate without exposure to internal-only information

## Context & Background

Broker login is a new external-access capability. Existing policy artifacts focus on internal roles and require explicit BrokerUser rules plus strict deny defaults.

## Acceptance Criteria

- **Given** `broker001@example.local` signs in with `BrokerUser`
- **When** authorized pages load
- **Then** only broker-visible screens, actions, and data are available

- **Given** broker user attempts internal-only routes or APIs
- **When** authorization is evaluated
- **Then** access is denied and no restricted payload is returned

- **Given** data contains mixed visibility fields
- **When** broker-visible responses are generated
- **Then** internal-only attributes are omitted or masked server-side

- **Given** broker user session is active
- **When** navigation menu renders
- **Then** only broker-visible navigation items appear

- **Given** broker user A is signed in
- **When** list or detail data is requested
- **Then** only records mapped to broker organization A are returned

- **Given** scope ownership cannot be determined for a requested record
- **When** access evaluation runs
- **Then** access is denied by default and no sensitive payload is returned

- Edge case: if broker role claim is missing/malformed, sign-in completes but app treats user as unauthorized and blocks protected content.

## Data Requirements

**Required Fields:**
- BrokerUser role claim
- Broker-to-entity visibility mapping (which broker records and linked resources are accessible)
- InternalOnly vs BrokerVisible field definitions for exposed entities

**Optional Fields:**
- Broker-facing display labels/help content

**Validation Rules:**
- Authorization defaults to deny when BrokerUser policy rule is absent
- Broker-visible responses must never include InternalOnly fields
- Records are returned only when broker-organization linkage exists for the signed-in broker user

## Role-Based Visibility

**Roles with this capability:**
- BrokerUser — constrained read-oriented access
- Admin — can impersonation-test/verify policy outcomes in non-production environments

**Data Visibility:**
- InternalOnly content: underwriting notes, internal assignment metadata, policy diagnostics
- ExternalVisible content: broker-approved summary fields and timeline events marked broker-visible

## Non-Functional Expectations

- Performance: broker-visible page loads p95 <= 2 seconds
- Security: zero known exposures of internal-only fields in broker responses
- Reliability: authorization behavior consistent between UI and direct API calls

## Dependencies

**Depends On:**
- Authorization matrix and Casbin policy updates to include BrokerUser
- Route guards and role entry behavior from F0009-S0003

**Related Stories:**
- F0009-S0005 — seeded user validation matrix

## Out of Scope

- Broker self-service CRUD for internal entities
- Document exchange workflows
- Broker-to-broker collaboration features

## UI/UX Notes

- Broker workspace should include explicit context indicator ("Broker Access") to set expectation.
- Unauthorized states should avoid technical jargon and include support escalation guidance.

## Questions & Assumptions

**Open Questions:**
- [ ] Which exact timeline event types are broker-visible in Phase 1 vs deferred?
- [ ] For shared accounts linked to multiple brokers, should visibility be full-record or field-level constrained?

**Assumptions (to be validated):**
- BrokerUser is external and limited to read-first flows in this phase.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Broker-visible field boundary tests pass
- [ ] Tests pass
- [ ] Documentation updated (if needed)

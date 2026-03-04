# F0009-S0003: Role-Based Entry and Protected Navigation

**Story ID:** F0009-S0003
**Feature:** F0009 — Authentication + Role-Based Login
**Title:** Route users to role-appropriate entry points and enforce protected navigation
**Priority:** Critical
**Phase:** Phase 1

## User Story

**As a** signed-in user
**I want** navigation and landing routes to align with my role
**So that** I only see areas I am authorized to use

## Context & Background

Current routing is role-agnostic and assumes authenticated internal usage. With real login enabled, the app must enforce role-aware entry and navigation control.

## Acceptance Criteria

- **Given** I sign in as `lisa.wong@nebula.local` with `DistributionUser`
- **When** session bootstrap completes
- **Then** I land on internal dashboard and can access authorized internal routes only

- **Given** I sign in as `john.miller@nebula.local` with `Underwriter`
- **When** session bootstrap completes
- **Then** I land on the underwriter dashboard route and cannot access disallowed internal management routes

- **Given** I attempt to navigate to a route outside my permissions
- **When** route guard evaluates access
- **Then** I receive deterministic unauthorized behavior (redirect or 403 page)

- **Given** server-side authorization denies an API call
- **When** the UI receives 403
- **Then** the page presents a permission-safe error and does not leak restricted data

- Edge case: users with multiple roles are routed using deterministic priority order documented in this story.

## Data Requirements

**Required Fields:**
- User role claim list (`nebula_roles`)
- Route-to-permission mapping table

**Optional Fields:**
- Preferred landing route by role

**Validation Rules:**
- Role resolution must support at least DistributionUser, Underwriter, BrokerUser
- Route guards must validate session and role before route render

## Role-Based Visibility

**Roles with expected primary entry:**
- DistributionUser — internal dashboard
- Underwriter — underwriter workspace/dashboard
- BrokerUser — broker-constrained workspace (defined in S0004)

**Data Visibility:**
- InternalOnly content: internal navigation items and admin/ops views
- ExternalVisible content: only routes explicitly marked broker-visible

## Non-Functional Expectations

- Performance: post-login route resolution <= 300ms client-side
- Security: client guards are secondary; API enforcement remains authoritative
- Reliability: route guard behavior is deterministic across direct URL access and in-app navigation

## Dependencies

**Depends On:**
- F0009-S0002 callback/session bootstrap
- Authorization policy matrix updates for role-to-resource mapping

**Related Stories:**
- F0009-S0004 — BrokerUser access boundaries
- F0009-S0005 — seeded user validation matrix

## Out of Scope

- Fine-grained field-level UI personalization beyond role scope
- Feature-flag experimentation by user segment

## UI/UX Notes

- Unauthorized route state should include a "Back to allowed home" action.
- Sidebar/nav items should hide inaccessible routes to reduce navigation dead ends.

## Questions & Assumptions

**Open Questions:**
- [ ] For multi-role users, should landing preference be configurable per user later?

**Assumptions (to be validated):**
- Deterministic role precedence for entry routing: `Admin` > `DistributionManager` > `DistributionUser` > `Underwriter` > `BrokerUser`.

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Authorization-denied UX validated
- [ ] Tests pass
- [ ] Documentation updated (if needed)

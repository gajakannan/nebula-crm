# F0002-S0004: Update Broker

**Story ID:** F0002-S0004
**Feature:** F0002 — Broker & MGA Relationship Management
**Title:** Update broker profile information
**Priority:** High
**Phase:** MVP

## User Story

**As a** Distribution Manager
**I want** to update broker profile details
**So that** the broker record stays accurate and current

## Context & Background

Broker profiles change over time. The CRM must support updating core broker fields and record each change in the audit timeline.

## Acceptance Criteria

- **Given** I have `broker:update` permission and the broker is within my authorization scope
- **When** I edit broker fields and save
- **Then** the broker record is updated and I remain on Broker 360

- **Given** required fields are missing or invalid
- **When** I submit the update
- **Then** I see field-level validation errors and the record is not updated

- **Given** I attempt to set a license number that already exists
- **When** I submit the update
- **Then** I receive a deterministic conflict error and the record is not updated

- **Given** I attempt to change the broker's license number
- **When** I submit the update
- **Then** I receive a validation error and the record is not updated

- **Given** I am not authorized to update this broker
- **When** I attempt to update
- **Then** access is denied with a 403 response

- **Given** an update completes successfully
- **When** the change is persisted
- **Then** a broker update timeline event is stored with actor, timestamp, and changed fields

- Edge case: broker does not exist or has been deleted → return not found

## Data Requirements

**Required Fields:**
- BrokerId: identifier for the broker to update

**Updatable Fields:**
- LegalName
- State
- Email
- Phone
- Status

**Validation Rules:**
- LicenseNumber must remain unique
- LicenseNumber is immutable after creation
- Field formats must match create-broker validation rules

## Role-Based Visibility

**Roles that can update brokers:**
- DistributionUser — scoped update
- DistributionManager — region-scoped update
- RelationshipManager — scoped update
- Admin — unscoped update

**Data Visibility:**
- InternalOnly content: audit metadata and internal notes
- ExternalVisible content: none in MVP

## Non-Functional Expectations

- Performance: update response p95 < 500ms (excluding auth provider latency)
- Security: server-side authorization required for all update paths
- Reliability: partial updates must not leave the record in an inconsistent state

## Dependencies

**Depends On:**
- F0002-S0001 - Create Broker
- F0002-S0003 - Read Broker (Broker 360 View)

**Related Stories:**
- F0002-S0007 - View Broker Activity Timeline

## Out of Scope

- Bulk broker updates
- Automated enrichment from external data sources

## Questions & Assumptions

**Assumptions (to be validated):**
- Status updates (Active/Inactive) are allowed via broker update

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Audit/timeline logged for updates
- [ ] Tests pass
